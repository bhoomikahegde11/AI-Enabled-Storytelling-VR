import re
import random
from npc_engine.core.models import EngineDecision
from npc_engine.engine.memory import Memory


class NegotiationEngine:
    shared_memory = Memory()

    def __init__(self, buyer, item, all_items=None):
        self.buyer = buyer
        self.item = item
        self.all_items = all_items or [item]
        self.item_catalog = {catalog_item.name.lower(): catalog_item for catalog_item in self.all_items}
        self.memory = self.shared_memory

        self.market_price = item.market_price
        self.max_price = buyer.compute_max_price(self.market_price)

        self.current_offer = int(round(buyer.initial_offer(self.market_price)))
        if self.current_offer is None:
            self.current_offer = int(getattr(self, "market_price", 10))
        self.current_offer = int(self.current_offer)
        self.personality = self.buyer.personality

        self.started = False
        self.has_made_first_offer = False
        self.deal_locked = False
        self.ended = False
        self.stage = "OPENING"
        self.deliberate_delay_used = False
        self.quantity_given = False
        self.quantity_locked = False
        self.price_introduced = False

        self.turns = 0
        self.max_turns = max(3, int(3 + self.buyer.patience * 8))
        self.min_increment = max(2, int(0.02 * self.market_price))
        self.last_increment = self.min_increment

        self.rejection_count = 0
        self.counter_count = 0
        self.hostile_count = 0
        self.out_of_world_count = 0
        self.irrelevant_count = 0
        self.low_price_count = 0
        self.too_expensive_count = 0
        self.unclear_no_count = 0
        self.no_count = 0

        self.frustration = 0.1
        self.trust = 0.5
        self.interest = min(1.0, 0.4 + self.buyer.desperation)

        self.last_buyer_offer = self.current_offer
        self.anchor_price = self.current_offer
        self.last_seller_price = None
        self.last_seller_price_per_kg = None
        self.seller_min_price = None
        self.final_price = None
        self.final_quantity = None
        self.final_item = None
        self.current_quantity = self.normalize_quantity(item.quantity, item.unit, "g")
        self.max_available_quantity = None
        self.quantity_modified = False
        self.last_quantity = None
        self.last_unit = None
        self.last_quantity_grams = None
        self.active_bundle = [{
            "name": item.name.lower(),
            "quantity": self.normalize_quantity(item.quantity, item.unit, "g"),
            "unit": "g"
        }]
        self.bundle_label = self.describe_bundle(self.active_bundle)
        self.last_offer_per_kg = self.current_offer / max(0.001, self.current_bundle_quantity_kg())
        self.last_action = None
        self.last_intent = None
        self.last_seller_input = None
        self.reference_price = self.memory.get_reference_price()

        if self.reference_price is not None:
            anchored_initial_offer = (0.7 * self.current_offer) + (0.3 * self.reference_price)
            self.current_offer = self.clamp(min(self.max_price, max(self.current_offer, anchored_initial_offer)))

        if self.personality == "Aggressive Trader":
            self.frustration = 0.18
            self.trust = 0.45
            self.max_turns = max(3, self.max_turns - 1)
        elif self.personality == "Cautious Buyer":
            self.frustration = 0.08
            self.trust = 0.3
        else:
            self.frustration = 0.05
            self.trust = 0.65
            self.max_turns += 1

        self.last_buyer_offer = self.current_offer
        self.anchor_price = self.current_offer
        self.last_offer_per_kg = self.current_offer / max(0.001, self.current_bundle_quantity_kg())

    def extract_price(self, text):
        text = str(text).lower()

        explicit_price_patterns = [
            r"(?:price\s*(?:is)?|offer\s*(?:is)?|sell\s+(?:it|this|them)?\s*for|for|at)\s*(\d+)\b(?!\s*(?:kg|kilogram|kilograms|g|gram|grams))",
            r"\b(\d+)\s*varahas?\b",
            r"\b(\d+)\s+is\s+(?:fine|good|okay|ok|fair)\b(?!\s*(?:kg|kilogram|kilograms|g|gram|grams))"
        ]

        for pattern in explicit_price_patterns:
            matches = re.findall(pattern, text)
            if matches:
                return int(matches[-1])

        numbers = []
        for match in re.finditer(r"\b(\d+)\b", text):
            number = int(match.group(1))
            remainder = text[match.end():]
            if re.match(r"\s*(kg|kilogram|kilograms|g|gram|grams)\b", remainder):
                continue
            numbers.append(number)

        return numbers[-1] if numbers else None

    def clamp(self, v):
        return int(round(v))

    def desperation_factor(self):
        return 0.3 + (self.buyer.desperation * 0.4)

    def near_accept_gap(self):
        return max(1, int(round(self.min_increment * (0.5 + self.buyer.desperation))))

    def acceptable_price(self):
        return self.max_price * (0.9 - (0.3 * self.buyer.desperation))

    def update_stage(self):
        if not self.has_made_first_offer:
            self.stage = "OPENING"
            self.deliberate_delay_used = False
            return self.stage
        if self.current_offer >= int(self.max_price * 0.9):
            self.stage = "FINALIZATION"
            return self.stage
        self.stage = "BARGAINING"
        self.deliberate_delay_used = False
        return self.stage

    def compute_increment(self, target_price):
        gap = max(0, target_price - self.current_offer)
        if gap <= 0:
            return 0

        stage_resistance = {
            "OPENING": 0.2,
            "BARGAINING": 0.32,
            "FINALIZATION": 0.45,
        }.get(self.stage, 0.32)
        frustration_multiplier = 1.0 + (self.frustration * 0.25)
        desperation_multiplier = 0.9 + (self.buyer.desperation * 0.45)
        trust_multiplier = self.trust_factor()
        interest_multiplier = max(0.75, self.interest)
        quantity_multiplier = 1.0
        if self.is_small_quantity():
            quantity_multiplier *= 0.75
        elif self.is_bulk_quantity():
            quantity_multiplier *= 1.12
        anchor_multiplier = 1.0
        if self.anchor_price is not None:
            anchor_gap = max(0, self.anchor_price - self.current_offer)
            if anchor_gap > 0:
                anchor_ratio = min(1.0, anchor_gap / max(1.0, gap))
                anchor_multiplier = 0.9 + (0.35 * anchor_ratio)
            else:
                anchor_multiplier = 0.88

        raw_increment = gap * stage_resistance * frustration_multiplier * desperation_multiplier
        raw_increment *= trust_multiplier * interest_multiplier * quantity_multiplier * anchor_multiplier

        increment = max(self.min_increment, int(round(raw_increment)))
        increment = min(increment, max(self.min_increment, int(gap * 0.6)))

        if self.last_increment and increment > (self.last_increment * 2):
            increment = min(increment, self.last_increment * 2)

        increment = min(increment, gap)
        self.last_increment = max(1, increment)
        return self.last_increment

    def is_small_quantity(self):
        return self.current_quantity is not None and self.current_quantity < 200

    def is_bulk_quantity(self):
        return self.current_quantity is not None and self.current_quantity > 1000

    def should_hold_position(self, target_price):
        gap = max(0, target_price - self.current_offer)
        if gap <= 0:
            return False
        if self.stage != "BARGAINING" or self.frustration >= 0.7:
            return False

        stubbornness = ((1.0 - self.buyer.patience) * 0.65) + ((1.0 - self.buyer.desperation) * 0.35)
        if self.personality == "Aggressive Trader":
            stubbornness += 0.1
        elif self.personality == "Polite Merchant":
            stubbornness -= 0.08
        elif self.personality == "Cautious Buyer":
            stubbornness += 0.04

        stubbornness = max(0.1, min(0.85, stubbornness))
        hold_probability = stubbornness * 0.45
        return random.random() < hold_probability

    def adjust_frustration(self, delta):
        if self.personality == "Aggressive Trader":
            delta *= 1.35
        elif self.personality == "Polite Merchant":
            delta *= 0.7
        elif self.personality == "Cautious Buyer":
            delta *= 0.9
        self.frustration = max(0.0, min(1.0, self.frustration + delta))

    def adjust_trust(self, delta):
        if self.personality == "Cautious Buyer":
            delta *= 0.8
        elif self.personality == "Polite Merchant":
            delta *= 1.15
        self.trust = max(0.0, min(1.0, self.trust + delta))

    def current_tone(self, base_tone="neutral"):
        if self.personality == "Aggressive Trader" and self.frustration >= 0.65:
            return "annoyed"
        if self.frustration >= 0.8:
            return "annoyed"
        if self.frustration >= 0.55 and base_tone == "neutral":
            return "annoyed"
        return base_tone

    def trust_factor(self):
        base = 0.85 + (self.trust * 0.3)
        if self.personality == "Cautious Buyer":
            base *= 0.85
        elif self.personality == "Polite Merchant":
            base *= 1.05
        return base

    def normalize_quantity(self, quantity, from_unit, to_unit):
        if from_unit == to_unit:
            return quantity
        if from_unit == "g" and to_unit == "kg":
            return quantity / 1000.0
        if from_unit == "kg" and to_unit == "g":
            return quantity * 1000.0
        return quantity

    def compute_bundle_market_price(self, bundle_items):
        total = 0.0
        for bundle_item in bundle_items:
            item_name = bundle_item["name"].lower()
            catalog_item = self.item_catalog.get(item_name)
            if catalog_item is None:
                continue
            normalized_quantity = self.normalize_quantity(
                bundle_item["quantity"],
                bundle_item["unit"],
                catalog_item.unit
            )
            total += catalog_item.market_price_per_unit * normalized_quantity
        return total

    def round_quantity_grams(self, quantity_grams):
        rounded = int(round(quantity_grams / 50.0) * 50)
        return max(50, rounded)

    def describe_bundle(self, bundle_items):
        parts = []
        for bundle_item in bundle_items:
            quantity = bundle_item["quantity"]
            unit = bundle_item["unit"]
            if unit == "g" and quantity >= 1000 and quantity % 1000 == 0:
                quantity = quantity / 1000
                unit = "kg"
            quantity_text = int(quantity) if isinstance(quantity, float) and quantity.is_integer() else quantity
            parts.append(f"{quantity_text}{unit} {bundle_item['name']}")
        return " and ".join(parts) if parts else self.item.name

    def update_active_bundle(self, bundle_items):
        if not bundle_items:
            return

        bundle_market_price = self.compute_bundle_market_price(bundle_items)
        if bundle_market_price <= 0:
            return

        self.active_bundle = bundle_items
        self.bundle_label = self.describe_bundle(bundle_items)
        if len(bundle_items) == 1:
            self.current_quantity = bundle_items[0]["quantity"]
        else:
            self.current_quantity = sum(bundle_item["quantity"] for bundle_item in bundle_items)
        self.market_price = self.clamp(bundle_market_price)
        self.max_price = self.buyer.compute_max_price(self.market_price)
        self.min_increment = max(2, int(0.02 * self.market_price))

        fresh_offer = int(round(self.buyer.initial_offer(self.market_price)))
        if not self.has_made_first_offer:
            self.current_offer = fresh_offer
        else:
            if getattr(self, "current_offer", None) is None:
                self.current_offer = fresh_offer
            self.current_offer = min(self.max_price, max(self.current_offer, fresh_offer))

        self.current_offer = int(self.clamp(self.current_offer))
        self.update_stage()

    def should_block_accept(self):
        frustration_threshold = 0.72 if self.personality == "Aggressive Trader" else 0.88 if self.personality == "Polite Merchant" else 0.8
        return self.hostile_count >= 2 or self.frustration >= frustration_threshold or self.last_action == "HOSTILE"

    def seller_floor_price(self):
        return self.seller_min_price if self.seller_min_price is not None else None

    def anchored_seller_price(self, seller_price):
        floor_price = self.seller_floor_price()
        if floor_price is None:
            return seller_price
        if seller_price is None:
            return floor_price
        return max(seller_price, floor_price)

    def can_accept_now(self):
        if self.should_block_accept():
            return False, "DISRESPECT"
        if not self.started or self.deal_locked:
            return False, "INVALID_STATE"
        if self.last_action != "OFFER" or not self.has_made_first_offer:
            return False, "INVALID_STATE"
        if self.stage != "FINALIZATION":
            if self.last_seller_price is not None and self.last_seller_price <= self.current_offer:
                return True, None
            return False, "NOT_FINAL_STAGE"
        if not self.deliberate_delay_used and random.random() < 0.15:
            self.deliberate_delay_used = True
            return False, "DELIBERATE_DELAY"
        if self.quantity_modified:
            return False, "QUANTITY_COOLDOWN"
        if self.seller_min_price is not None and self.current_offer < self.seller_min_price:
            return False, "SELLER_FLOOR"
        if self.current_offer < self.acceptable_price():
            return False, "ACCEPT_THRESHOLD"
        return True, None

    def resolve_seller_floor_block(self, tone="neutral"):
        if self.seller_min_price is None:
            return self.respond("OFFER", price=self.current_offer, tone=self.current_tone(tone))
        if self.seller_min_price > self.max_price:
            self.adjust_trust(-0.08)
            self.adjust_frustration(0.08)
            return self.respond("WALK_AWAY", context={"reason": "TOO_EXPENSIVE"})

        self.current_offer = max(self.current_offer, self.seller_min_price)
        self.current_offer = min(self.current_offer, self.max_price)
        self.current_offer = self.clamp(self.current_offer)
        return self.respond(
            "OFFER",
            price=self.current_offer,
            context={"proposal_type": "seller_floor"},
            tone=self.current_tone(tone)
        )

    def current_bundle_quantity_kg(self):
        total_quantity_kg = 0.0
        for bundle_item in self.active_bundle:
            total_quantity_kg += self.normalize_quantity(bundle_item["quantity"], bundle_item["unit"], "kg")
        return total_quantity_kg

    def market_price_per_kg(self):
        bundle_quantity_kg = self.current_bundle_quantity_kg()
        if bundle_quantity_kg <= 0:
            return None
        return self.market_price / bundle_quantity_kg

    def seller_price_per_kg(self, seller_price):
        bundle_quantity_kg = self.current_bundle_quantity_kg()
        if seller_price is None or bundle_quantity_kg <= 0:
            return None
        return seller_price / bundle_quantity_kg

    def offer_price_per_kg(self, offer_price):
        bundle_quantity_kg = self.current_bundle_quantity_kg()
        if offer_price is None or bundle_quantity_kg <= 0:
            return None
        return offer_price / bundle_quantity_kg

    def current_deal_quantity(self):
        if self.active_bundle:
            if len(self.active_bundle) == 1:
                return self.active_bundle[0]["quantity"]
            return sum(bundle_item["quantity"] for bundle_item in self.active_bundle)
        return self.normalize_quantity(self.item.quantity, self.item.unit, "g")

    def current_deal_item(self):
        if self.active_bundle and len(self.active_bundle) > 1:
            return self.bundle_label
        return self.item.name

    def record_final_deal(self):
        if self.current_quantity is None:
            self.current_quantity = self.normalize_quantity(self.item.quantity, self.item.unit, "g")
        self.final_price = self.current_offer
        self.final_quantity = self.current_quantity
        self.final_item = self.current_deal_item()
        self.memory.update(self.final_price)

    def violates_offer_fairness(self):
        new_price_per_kg = self.offer_price_per_kg(self.current_offer)
        market_price_per_kg = self.market_price_per_kg()
        previous_price_per_kg = self.last_offer_per_kg
        if new_price_per_kg is None or market_price_per_kg is None or previous_price_per_kg is None:
            return False
        return new_price_per_kg > previous_price_per_kg * 1.1

    def quantity_value_misaligned(self, seller_price_per_kg, buyer_offer_per_kg):
        if seller_price_per_kg is None or buyer_offer_per_kg is None:
            return False
        return abs(seller_price_per_kg - buyer_offer_per_kg) > (buyer_offer_per_kg * 0.1)

    def buyer_offer_per_kg(self):
        bundle_quantity_kg = self.current_bundle_quantity_kg()
        if bundle_quantity_kg <= 0:
            return None
        return self.current_offer / bundle_quantity_kg

    def scale_bundle_for_total(self, target_total_price, reference_total_price):
        if reference_total_price is None or reference_total_price <= 0:
            return None
        scale_factor = target_total_price / reference_total_price
        scale_factor = max(0.5, min(scale_factor, 2.5))
        scaled_bundle = []
        for bundle_item in self.active_bundle:
            scaled_quantity = self.round_quantity_grams(bundle_item["quantity"] * scale_factor)
            scaled_quantity = max(100, scaled_quantity)
            if scaled_quantity < (bundle_item["quantity"] * 0.5):
                return None
            if self.max_available_quantity is not None:
                scaled_quantity = min(scaled_quantity, self.max_available_quantity)
            scaled_bundle.append({
                "name": bundle_item["name"],
                "quantity": scaled_quantity,
                "unit": "g"
            })
        return scaled_bundle

    def quantity_counter_context(self, seller_price):
        if self.quantity_locked or seller_price is None:
            return None

        if self.max_available_quantity is not None and self.current_quantity > self.max_available_quantity:
            self.current_quantity = self.max_available_quantity
        seller_price_per_kg = self.seller_price_per_kg(seller_price)
        buyer_offer_per_kg = self.buyer_offer_per_kg()
        market_price_per_kg = self.market_price_per_kg()
        if seller_price_per_kg is None or buyer_offer_per_kg is None or market_price_per_kg is None:
            return None

        default_quantity_grams = self.normalize_quantity(self.item.quantity, self.item.unit, "g")
        reduced_quantity = self.current_quantity < default_quantity_grams

        if seller_price > self.max_price:
            reduced_bundle = self.scale_bundle_for_total(self.current_offer, seller_price)
            if not reduced_bundle:
                return None
            if len(reduced_bundle) == 1 and reduced_bundle[0]["quantity"] == self.current_quantity:
                self.quantity_locked = True
                return None
            return {
                "proposal_type": "quantity_reduce" if len(reduced_bundle) == 1 else "bundle_adjust",
                "seller_price": seller_price,
                "counter_bundle_label": self.describe_bundle(reduced_bundle)
            }

        if reduced_quantity and seller_price_per_kg > market_price_per_kg:
            return {
                "proposal_type": "quantity_too_costly",
                "seller_price": seller_price
            }

        if self.is_small_quantity() and seller_price_per_kg > market_price_per_kg * 1.1:
            return {
                "proposal_type": "quantity_too_costly",
                "seller_price": seller_price
            }

        if seller_price_per_kg > buyer_offer_per_kg * 1.1:
            expanded_bundle = self.scale_bundle_for_total(seller_price, self.current_offer)
            if not expanded_bundle:
                return None
            if len(expanded_bundle) == 1 and expanded_bundle[0]["quantity"] == self.current_quantity:
                self.quantity_locked = True
                return None
            return {
                "proposal_type": "quantity_expect_more" if len(expanded_bundle) == 1 else "bundle_expect_more",
                "seller_price": seller_price,
                "counter_bundle_label": self.describe_bundle(expanded_bundle)
            }

        return None

    def respond(self, action, price=None, context=None, tone="neutral"):
        if getattr(self, "current_offer", None) is None:
            self.current_offer = int(getattr(self, "market_price", 10))
        self.current_offer = int(self.current_offer)

        if price is None:
            if hasattr(self, "current_offer") and self.current_offer is not None:
                price = int(self.current_offer)
            else:
                price = int(getattr(self, "market_price", 10))

        price = int(price)

        if getattr(self, "price_introduced", False) is False and action == "OFFER":
            action = "ASK_PRICE"
            if context and "proposal_type" in context:
                del context["proposal_type"]

        if action in ["WALK_AWAY", "NO_ITEM", "ACCEPT"]:
            self.ended = True
        self.last_action = action
        if action == "OFFER":
            self.has_made_first_offer = True
            self.update_stage()
            current_offer_per_kg = self.offer_price_per_kg(price if price is not None else self.current_offer)
            if current_offer_per_kg is not None:
                self.last_offer_per_kg = current_offer_per_kg
        if price is not None and action in ["OFFER", "ACCEPT", "OUT_OF_WORLD", "SOCIAL_RESPONSE"]:
            self.last_buyer_offer = price
        emotional_context = {
            "frustration": round(self.frustration, 3),
            "trust": round(self.trust, 3),
            "interest": round(self.interest, 3),
            "bundle_label": self.bundle_label,
            "stage": self.stage,
            "seller_min_price": self.seller_min_price,
            "current_quantity_grams": self.current_quantity,
            "last_quantity_grams": self.last_quantity_grams,
            "last_seller_price_per_kg": round(self.last_seller_price_per_kg, 3) if self.last_seller_price_per_kg is not None else None
        }
        if context is not None:
            merged_context = dict(context)
            merged_context.update(emotional_context)
        else:
            merged_context = emotional_context
        return EngineDecision(
            action=action,
            price=price,
            quantity=self.current_quantity,
            done=action in ["WALK_AWAY", "NO_ITEM", "ACCEPT", "END"],
            reason=merged_context.get("reason"),
            stage=self.stage
        )

    def next_step(self, player_action=None):
        if self.ended:
            return self.respond("END")

        if self.deal_locked:
            return self.respond("END", price=self.current_offer)

        self.turns += 1

        if self.turns > self.max_turns:
            return self.respond("WALK_AWAY", context={"reason": "TOO_LONG"})

        if not self.started:
            self.started = True
            return self.respond("ASK_ITEM")

        intent = player_action.intent if player_action is not None else "CONTINUE"
        seller_price = player_action.price if player_action is not None else None
        seller_quantity = player_action.quantity if player_action is not None else None
        tone = "neutral"
        social_sub_intent = None

        if intent == "NO_ITEM":
            return self.respond("NO_ITEM")

        if not self.quantity_given and seller_quantity is not None:
            self.current_quantity = seller_quantity
            self.max_available_quantity = seller_quantity
            self.quantity_given = True
            self.update_active_bundle([{
                "name": self.item.name.lower(),
                "quantity": seller_quantity,
                "unit": "g"
            }])

        if not self.quantity_given:
            self.quantity_given = True
            fixed_quantity = random.choice([500, 700, 1000])
            self.current_quantity = fixed_quantity
            self.update_active_bundle([{
                "name": self.item.name.lower(),
                "quantity": fixed_quantity,
                "unit": "g"
            }])
            return self.respond(
                "SET_QUANTITY",
                context={
                    "quantity": fixed_quantity,
                    "unit": "g",
                    "item": self.item.name
                }
            )

        self.last_intent = intent
        print(f"Detected intent: {intent}")
        quantity_modified_last_turn = self.quantity_modified
        self.quantity_modified = False

        previous_quantity = self.current_quantity

        if seller_quantity is not None and seller_quantity == self.current_quantity:
            self.quantity_locked = True

        if self.quantity_locked:
            if intent in ["QUANTITY_CHANGE", "QUANTITY_PRICE"]:
                intent = "PRICE"

        quantity_modified_this_turn = seller_quantity is not None

        if seller_quantity is not None:
            if self.max_available_quantity is not None:
                self.current_quantity = min(seller_quantity, self.max_available_quantity)
            else:
                self.current_quantity = seller_quantity

            self.last_quantity = self.current_quantity
            self.last_unit = "g"
            self.last_quantity_grams = self.current_quantity
            self.quantity_modified = True

            self.update_active_bundle([{
                "name": self.item.name.lower(),
                "quantity": self.current_quantity,
                "unit": "g"
            }])

            scarcity_ratio = self.current_quantity / 1000.0
            if scarcity_ratio < 0.5:
                self.max_price = self.clamp(self.max_price * 0.9)
                self.current_offer = min(self.current_offer, self.max_price)

        is_simple_no = intent == "REJECT" and seller_price is None and seller_quantity is None
        if is_simple_no:
            self.no_count += 1
            self.adjust_frustration(0.06 + (0.03 * min(self.no_count - 1, 2)))
        else:
            self.no_count = 0
        self.last_seller_input = None

        frustration_walkaway_threshold = 0.85 if self.personality == "Aggressive Trader" else 0.98 if self.personality == "Polite Merchant" else 0.93
        if self.frustration >= frustration_walkaway_threshold:
            return self.respond("WALK_AWAY", context={"reason": "TOO_LONG"})

        if not (is_simple_no and intent == "SOCIAL" and social_sub_intent == "CONFUSION"):
            self.unclear_no_count = 0

        if is_simple_no and intent == "SOCIAL" and social_sub_intent == "CONFUSION":
            self.unclear_no_count += 1
            self.adjust_frustration(0.08 + (0.04 * min(self.unclear_no_count - 1, 2)))
            self.adjust_trust(-0.04)
            if self.unclear_no_count >= 3:
                return self.respond("WALK_AWAY", context={"reason": "NO_INTEREST"})
            return self.respond(
                "SOCIAL_RESPONSE",
                price=self.current_offer,
                context={"social_sub_intent": "CONFUSION"},
                tone=self.current_tone("annoyed" if self.unclear_no_count >= 2 else tone)
            )

        if seller_price is None and not self.price_introduced:
            return self.respond("ASK_PRICE")

        if intent == "SOCIAL":
            return self.respond(
                "SOCIAL_RESPONSE",
                price=self.current_offer,
                context={"social_sub_intent": social_sub_intent or "GENERAL"},
                tone=self.current_tone(tone)
            )

        if intent == "CONTINUE":
            self.adjust_trust(0.03)
            return self.respond("OFFER", price=self.current_offer, tone=self.current_tone(tone))

        if intent == "QUERY_QUANTITY":
            return self.respond(
                "OFFER",
                price=self.current_offer,
                context={"proposal_type": "quantity_answer"},
                tone=self.current_tone(tone)
            )

        previous_seller_price = self.last_seller_price
        if intent == "BUNDLE_OFFER":
            self.adjust_trust(0.04)
            self.adjust_frustration(-0.02)
            return self.respond(
                "OFFER",
                price=self.current_offer,
                context={"proposal_type": "bundle_offer"},
                tone=self.current_tone(tone)
            )

        if intent == "QUANTITY_CHANGE":
            self.adjust_trust(0.03)
            self.adjust_frustration(-0.01)
            
            if self.current_quantity < previous_quantity:
                if self.personality == "Aggressive Trader":
                    behavior = "reject_small_quantity" if random.random() < 0.5 else "quantity_change"
                elif self.personality == "Polite Merchant":
                    behavior = "quantity_change"
                else:
                    behavior = "quantity_change"
                return self.respond("OFFER", price=self.current_offer, context={"proposal_type": behavior}, tone=self.current_tone(tone))
                
            return self.respond(
                "OFFER",
                price=self.current_offer,
                context={"proposal_type": "quantity_change"},
                tone=self.current_tone(tone)
            )

        if seller_price is not None:
            self.price_introduced = True
            if intent in ["PRICE", "COUNTER", "QUANTITY_PRICE"]:
                if seller_price <= self.current_offer:
                    self.current_offer = seller_price
                else:
                    self.current_offer += random.uniform(1, 5)
                
                self.current_offer = self.clamp(min(self.current_offer, self.max_price))
                self.current_offer = min(self.current_offer, seller_price)

            self.last_seller_price = seller_price
            self.anchor_price = (0.7 * self.anchor_price) + (0.3 * seller_price)
            self.last_seller_price_per_kg = self.seller_price_per_kg(seller_price)
            market_price_per_kg = self.market_price_per_kg()
            if self.reference_price is not None:
                price_gap_ratio = abs(seller_price - self.reference_price) / max(1, self.reference_price)
                if price_gap_ratio <= 0.1:
                    self.adjust_trust(0.08)
                    self.adjust_frustration(-0.03)
                elif price_gap_ratio >= 0.35:
                    self.adjust_trust(-0.08)
                    self.adjust_frustration(0.04)
            if self.last_seller_price_per_kg is not None and market_price_per_kg is not None and self.last_seller_price_per_kg > market_price_per_kg * 1.8:
                self.adjust_trust(-0.2)
                self.adjust_frustration(0.18)
            elif seller_price <= self.max_price:
                self.adjust_trust(0.08)
                self.adjust_frustration(-0.04)

            significant_price_increase = (
                previous_seller_price is not None and
                seller_price > previous_seller_price and
                (seller_price >= previous_seller_price * 1.1 or (seller_price - previous_seller_price) >= self.min_increment * 2)
            )
            if significant_price_increase:
                self.adjust_frustration(0.12)
                self.adjust_trust(-0.14)

                hold_probability = 0.35 + ((1.0 - self.buyer.patience) * 0.2) + (self.frustration * 0.15)
                if self.personality == "Aggressive Trader":
                    hold_probability += 0.1
                elif self.personality == "Polite Merchant":
                    hold_probability -= 0.08
                hold_probability = max(0.2, min(0.85, hold_probability))

                if random.random() < hold_probability:
                    return self.respond(
                        "OFFER",
                        price=self.current_offer,
                        context={
                            "proposal_type": "price_increase",
                            "previous_seller_price": previous_seller_price,
                            "seller_price": seller_price
                        },
                        tone=self.current_tone("annoyed")
                    )

        # -------------------------------
        # OUT OF WORLD
        # -------------------------------
        if intent == "OUT_OF_WORLD":
            self.out_of_world_count += 1
            self.adjust_frustration(0.18)
            self.adjust_trust(-0.08)

            if self.out_of_world_count >= 4:
                return self.respond("WALK_AWAY", context={"reason": "OUT_OF_WORLD"})

            stage = "confused"
            if self.out_of_world_count == 2:
                stage = "annoyed"
            elif self.out_of_world_count == 3:
                stage = "warning"

            return self.respond(
                "OUT_OF_WORLD",
                price=self.current_offer,
                context={"stage": stage},
                tone=self.current_tone(tone)
            )

        # -------------------------------
        # HOSTILE
        # -------------------------------
        if intent == "HOSTILE":
            self.hostile_count += 1
            self.adjust_frustration(0.28 + (0.04 * min(self.hostile_count - 1, 2)))
            self.adjust_trust(-0.12)

            if self.hostile_count >= 4 or (self.hostile_count >= 3 and self.frustration >= 0.75):
                return self.respond("WALK_AWAY", context={"reason": "HOSTILE"})

            stage = "warning"
            if self.hostile_count == 2:
                stage = "stronger"
            elif self.hostile_count == 3:
                stage = "final_warning"

            return self.respond(
                "HOSTILE",
                price=self.current_offer,
                context={"stage": stage},
                tone="annoyed"
            )

        # -------------------------------
        # REJECT
        # -------------------------------
        if intent == "REJECT":
            self.rejection_count += 1
            self.adjust_frustration(0.14)
            self.adjust_trust(-0.06)
            if is_simple_no:
                if self.no_count >= 3:
                    return self.respond("WALK_AWAY", context={"reason": "NO_INTEREST"})
                if self.no_count == 2:
                    return self.respond(
                        "OFFER",
                        price=self.current_offer,
                        context={"proposal_type": "hold_position"},
                        tone=self.current_tone("annoyed")
                    )

            max_rejections = 1 if self.buyer.patience < 0.5 else 2
            if self.personality == "Aggressive Trader":
                max_rejections = 1
            elif self.personality == "Polite Merchant":
                max_rejections += 1
            if self.frustration >= 0.75:
                max_rejections = max(1, max_rejections - 1)
            if self.rejection_count >= max_rejections:
                return self.respond("WALK_AWAY", context={"reason": "NO_INTEREST"})

            increment = max(1, int(self.current_offer * 0.05))
            new_offer = self.current_offer + increment
            max_price = self.buyer.compute_max_price(self.item.market_price)

            if new_offer >= max_price:
                return self.respond("REJECT", price=self.current_offer)

            self.current_offer = new_offer
            return self.respond("OFFER", price=self.current_offer)

        # -------------------------------
        # ULTIMATUM
        # -------------------------------
        if intent == "ULTIMATUM" and seller_price is not None:
            self.seller_min_price = seller_price
            self.adjust_frustration(0.1)
            self.adjust_trust(-0.05)
            target_price = self.anchored_seller_price(seller_price)

            if target_price is None:
                return self.respond(
                    "OFFER",
                    price=self.current_offer,
                    context={"proposal_type": "ultimatum"},
                    tone=self.current_tone("annoyed")
                )

            if target_price > self.max_price:
                self.adjust_trust(-0.2)
                self.adjust_frustration(0.14)
                return self.respond("WALK_AWAY", context={"reason": "TOO_EXPENSIVE"})

            ultimatum_gap = max(0, target_price - self.current_offer)
            if ultimatum_gap == 0:
                self.current_offer = self.clamp(min(target_price, self.max_price))
            else:
                if self.should_hold_position(target_price):
                    return self.respond(
                        "OFFER",
                        price=self.current_offer,
                        context={"proposal_type": "hold_position"},
                        tone=self.current_tone("annoyed")
                    )
                ultimatum_increment = self.compute_increment(target_price)
                self.current_offer += ultimatum_increment
                self.current_offer = min(self.current_offer, target_price)
            self.current_offer = min(self.current_offer, self.max_price)
            self.current_offer = self.clamp(self.current_offer)

            return self.respond(
                "OFFER",
                price=self.current_offer,
                context={"proposal_type": "ultimatum"},
                tone=self.current_tone("annoyed")
            )

        # -------------------------------
        # COUNTER (🔥 FIXED POSITION)
        # -------------------------------
        if intent in ["COUNTER", "COUNTER_MIDPOINT"]:
            self.counter_count += 1
            self.adjust_frustration(0.08)

            max_counters = 1 if self.buyer.patience < 0.45 else 2 if self.buyer.patience < 0.75 else 3
            if self.personality == "Aggressive Trader":
                max_counters = max(1, max_counters - 1)
            elif self.personality == "Polite Merchant":
                max_counters += 1
            if self.frustration >= 0.7:
                max_counters = max(1, max_counters - 1)
            if self.counter_count > max_counters:
                return self.respond("WALK_AWAY", context={"reason": "TOO_LONG"})

            if intent == "COUNTER_MIDPOINT" and self.last_seller_price is not None:
                midpoint_offer = (self.current_offer + self.last_seller_price) / 2
                if self.should_hold_position(midpoint_offer):
                    return self.respond(
                        "OFFER",
                        price=self.current_offer,
                        context={"proposal_type": "hold_position"},
                        tone=self.current_tone(tone)
                    )
                self.current_offer += self.compute_increment(midpoint_offer)
                self.adjust_trust(0.05)
            else:
                target_price = self.anchored_seller_price(self.last_seller_price)
                if target_price is not None and self.should_hold_position(target_price):
                    return self.respond(
                        "OFFER",
                        price=self.current_offer,
                        context={"proposal_type": "hold_position"},
                        tone=self.current_tone(tone)
                    )
                counter_increment = self.compute_increment(target_price) if target_price is not None else self.min_increment
                self.current_offer += counter_increment
                if target_price is not None:
                    self.current_offer = min(self.current_offer, target_price)

            self.current_offer = min(self.current_offer, self.max_price)
            self.current_offer = self.clamp(self.current_offer)

            return self.respond("OFFER", price=self.current_offer, tone=self.current_tone(tone))

        # -------------------------------
        # ACCEPT
        # -------------------------------
        if intent == "ACCEPT":
            self.current_offer = self.clamp(self.current_offer)
            self.deal_locked = True
            self.record_final_deal()
            self.adjust_trust(0.1)
            self.adjust_frustration(-0.1)
            return self.respond("ACCEPT", price=self.current_offer)

        # -------------------------------
        # PRICE LOGIC
        # -------------------------------
        if intent in ["PRICE", "QUANTITY_PRICE"] and seller_price is not None:
            if seller_price == self.current_offer:
                can_accept, _ = self.can_accept_now()
                if can_accept:
                    self.deal_locked = True
                    self.record_final_deal()
                    return self.respond("ACCEPT", price=self.current_offer)

            target_price = self.anchored_seller_price(seller_price)
            market_price_per_kg = self.market_price_per_kg()
            target_price_per_kg = self.seller_price_per_kg(target_price)
            buyer_offer_per_kg = self.buyer_offer_per_kg()
            quantity_counter = self.quantity_counter_context(target_price)
            if self.is_small_quantity():
                self.adjust_frustration(0.06)
                self.adjust_trust(-0.04)
            elif self.is_bulk_quantity():
                self.adjust_trust(0.05)
                self.adjust_frustration(-0.03)

            if target_price_per_kg is not None and market_price_per_kg is not None and target_price_per_kg < (0.35 * market_price_per_kg):
                self.low_price_count += 1
                self.adjust_trust(-0.06 - (0.03 * min(self.low_price_count - 1, 2)))
                self.adjust_frustration(0.05 + (0.04 * min(self.low_price_count - 1, 2)))

                if self.low_price_count >= 3:
                    return self.respond("WALK_AWAY", context={"reason": "SUSPICIOUS"})

                stage = "warning" if self.low_price_count == 1 else "suspicious"
                return self.respond(
                    "LOW_PRICE",
                    price=self.current_offer,
                    context={"stage": stage, "seller_price": seller_price},
                    tone=self.current_tone("annoyed")
                )

            if target_price_per_kg is not None and market_price_per_kg is not None and target_price_per_kg > market_price_per_kg * 1.8:
                
                if quantity_modified_this_turn and self.current_quantity < previous_quantity:
                    if self.personality == "Aggressive Trader":
                        behavior = "reject_small_quantity" if random.random() < 0.5 else "quantity_change"
                    elif self.personality == "Polite Merchant":
                        behavior = "quantity_change"
                    else:
                        behavior = "quantity_change"
                    self.current_offer += self.compute_increment(target_price)
                    self.current_offer = min(self.current_offer, target_price)
                    self.current_offer = self.clamp(min(self.current_offer, self.max_price))
                    return self.respond("OFFER", price=self.current_offer, context={"proposal_type": behavior}, tone=self.current_tone(tone))
                    
                self.too_expensive_count += 1
                self.adjust_trust(-0.2)
                self.adjust_frustration(0.2)
                if self.too_expensive_count == 1:
                    return self.respond(
                        "OFFER",
                        price=self.current_offer,
                        context={
                            "proposal_type": "too_expensive_warning",
                            "seller_price": seller_price
                        },
                        tone=self.current_tone("annoyed")
                    )
                return self.respond("WALK_AWAY", context={"reason": "TOO_EXPENSIVE"})

            if self.is_small_quantity() and target_price_per_kg is not None and market_price_per_kg is not None:
                if target_price_per_kg > market_price_per_kg * 1.5:
                    self.adjust_trust(-0.12)
                    self.adjust_frustration(0.12)
                    return self.respond("WALK_AWAY", context={"reason": "TOO_EXPENSIVE"})
                if target_price > self.current_offer and target_price_per_kg > market_price_per_kg * 1.1:
                    self.adjust_trust(-0.05)
                    self.adjust_frustration(0.06)
                    return self.respond(
                        "OFFER",
                        price=self.current_offer,
                        context={
                            "proposal_type": "quantity_too_costly",
                            "seller_price": seller_price
                        },
                        tone=self.current_tone("annoyed")
                    )

            if quantity_counter is not None and target_price > self.current_offer:
                self.adjust_trust(-0.03)
                self.adjust_frustration(0.04)
                return self.respond(
                    "OFFER",
                    price=self.current_offer,
                    context=quantity_counter,
                    tone=self.current_tone(tone)
                )

            quantity_value_mismatch = (
                (intent == "QUANTITY_PRICE" or quantity_modified_this_turn) and
                self.quantity_value_misaligned(target_price_per_kg, buyer_offer_per_kg)
            )
            if quantity_value_mismatch:
                if self.should_hold_position(target_price):
                    return self.respond(
                        "OFFER",
                        price=self.current_offer,
                        context={"proposal_type": "hold_position"},
                        tone=self.current_tone(tone)
                    )
                self.adjust_trust(-0.03 if target_price_per_kg is not None and buyer_offer_per_kg is not None and target_price_per_kg > buyer_offer_per_kg else 0.01)
                self.adjust_frustration(0.04)
                if target_price > self.current_offer:
                    adjusted_increment = self.compute_increment(target_price)
                    self.current_offer += adjusted_increment
                    self.current_offer = min(self.current_offer, target_price)
                    self.current_offer = min(self.current_offer, self.max_price)
                    self.current_offer = self.clamp(self.current_offer)
                return self.respond(
                    "OFFER",
                    price=self.current_offer,
                    context={"proposal_type": "quantity_change"},
                    tone=self.current_tone("annoyed" if target_price_per_kg is not None and buyer_offer_per_kg is not None and target_price_per_kg > buyer_offer_per_kg else tone)
                )

            if intent == "QUANTITY_PRICE" or quantity_modified_this_turn:
                if self.should_hold_position(target_price):
                    return self.respond(
                        "OFFER",
                        price=self.current_offer,
                        context={"proposal_type": "hold_position"},
                        tone=self.current_tone(tone)
                    )
                adjusted_increment = self.compute_increment(target_price)
                self.current_offer += adjusted_increment
                self.current_offer = min(self.current_offer, target_price)
                self.current_offer = min(self.current_offer, self.max_price)
                self.current_offer = self.clamp(self.current_offer)
                return self.respond(
                    "OFFER",
                    price=self.current_offer,
                    context={"proposal_type": "quantity_change"},
                    tone=self.current_tone(tone)
                )

            if target_price <= self.current_offer:
                can_accept, reject_reason = self.can_accept_now()
                if not can_accept:
                    if reject_reason == "DISRESPECT":
                        return self.respond("WALK_AWAY", context={"reason": "DISRESPECT"})
                    if reject_reason == "NOT_FINAL_STAGE":
                        return self.respond("OFFER", price=self.current_offer, tone=self.current_tone(tone))
                    if reject_reason == "DELIBERATE_DELAY":
                        return self.respond("OFFER", price=self.current_offer, tone=self.current_tone(tone))
                    if reject_reason == "SELLER_FLOOR":
                        return self.resolve_seller_floor_block(tone)
                    if reject_reason == "ACCEPT_THRESHOLD":
                        return self.respond("OFFER", price=self.current_offer, tone=self.current_tone(tone))
                    return self.respond("OFFER", price=self.current_offer, tone=self.current_tone(tone))
                if self.violates_offer_fairness():
                    return self.respond(
                        "OFFER",
                        price=self.current_offer,
                        context={"proposal_type": "reject_small_quantity"},
                        tone=self.current_tone("annoyed")
                    )
                self.deal_locked = True
                self.record_final_deal()
                self.adjust_trust(0.1)
                return self.respond("ACCEPT", price=self.current_offer)

            if target_price <= self.max_price and target_price - self.current_offer <= self.near_accept_gap():
                can_accept, reject_reason = self.can_accept_now()
                if not can_accept:
                    if reject_reason == "DISRESPECT":
                        return self.respond("WALK_AWAY", context={"reason": "DISRESPECT"})
                    if reject_reason == "NOT_FINAL_STAGE":
                        return self.respond("OFFER", price=self.current_offer, tone=self.current_tone(tone))
                    if reject_reason == "DELIBERATE_DELAY":
                        return self.respond("OFFER", price=self.current_offer, tone=self.current_tone(tone))
                    if reject_reason == "SELLER_FLOOR":
                        return self.resolve_seller_floor_block(tone)
                    if reject_reason == "ACCEPT_THRESHOLD":
                        return self.respond("OFFER", price=self.current_offer, tone=self.current_tone(tone))
                    return self.respond("OFFER", price=self.current_offer, tone=self.current_tone(tone))
                if self.violates_offer_fairness():
                    return self.respond(
                        "OFFER",
                        price=self.current_offer,
                        context={"proposal_type": "reject_small_quantity"},
                        tone=self.current_tone("annoyed")
                    )
                self.deal_locked = True
                self.record_final_deal()
                self.adjust_trust(0.08)
                return self.respond("ACCEPT", price=self.current_offer)

            if self.should_hold_position(target_price):
                return self.respond(
                    "OFFER",
                    price=self.current_offer,
                    context={"proposal_type": "hold_position"},
                    tone=self.current_tone(tone)
                )

            increment = self.compute_increment(target_price)
            self.current_offer += increment
            self.current_offer = min(self.current_offer, target_price)
            self.current_offer = min(self.current_offer, self.max_price)
            self.current_offer = self.clamp(self.current_offer)

            return self.respond("OFFER", price=self.current_offer, tone=self.current_tone(tone))

        # -------------------------------
        # DEFAULT
        # -------------------------------
        self.adjust_frustration(0.04)
        self.adjust_trust(-0.02)
        return self.respond("OFFER", price=self.current_offer, tone=self.current_tone(tone))

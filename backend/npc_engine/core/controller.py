from npc_engine.core.models import PlayerAction
from npc_engine.core.measurements import grams_to_traditional_label


class Controller:
    """
    Level-agnostic engine controller that manages session flows, 
    delegating NLP, classification, and dialogue generation to level-specific injected hooks.
    """
    def __init__(self, engine, classify_intent_fn, extract_quantity_info_fn, extract_price_fn, dialogue_fn):
        self.engine = engine
        self.classify_intent_fn = classify_intent_fn
        self.extract_quantity_info_fn = extract_quantity_info_fn
        self.extract_price_fn = extract_price_fn
        self.dialogue_fn = dialogue_fn

    def _build_debug_info(self):
        return {
            "stage": self.engine.stage,
            "current_offer": self.engine.current_offer,
            "seller_price": self.engine.last_seller_price,
            "desperation": round(self.engine.buyer.desperation, 2),
            "frustration": round(self.engine.frustration, 2),
            "turn": self.engine.turns
        }

    def format_final_quantity(self):
        quantity = self.engine.final_quantity
        if quantity is None:
            return grams_to_traditional_label(1000)
        return grams_to_traditional_label(quantity)

    def _build_player_action(self, seller_input):
        text = str(seller_input).strip()
        if not text:
            return PlayerAction(intent="CONTINUE", price=None, quantity=None)

        lowered = text.lower()
        quantity_info = self.extract_quantity_info_fn(text)
        quantity = quantity_info["quantity_grams"] if quantity_info is not None else None
        price = self.extract_price_fn(text)

        result = self.classify_intent_fn(text, context={
            "in_negotiation": self.engine.started,
            "item_name": self.engine.item.name,
            "last_system_action": self.engine.last_action,
            "last_seller_price": self.engine.last_seller_price,
            "current_offer": self.engine.current_offer
        })
        
        if result is None:
            result = {
                "intent": "IRRELEVANT"
            }

        intent = result.get("intent", "IRRELEVANT")
        price = result.get("price", price)
        quantity = result.get("quantity", quantity)

        if intent == "NO_ITEM" and quantity is not None and any(
            word in lowered for word in ["only", "left", "have", "but", "instead", "g", "gm", "kg", "palam", "palams", "seer", "seers", "veesai", "viss", "manangu", "maund", "bahar", "candy"]
        ):
            intent = "QUANTITY_CHANGE"

        if intent == "QUERY" and any(term in lowered for term in ["how many", "how much", "grams", "gram", "kg", "quantity", "seer", "seers", "veesai", "viss", "palam", "palams", "manangu", "maund", "maunds", "bahar", "bahars", "candy", "candies"]):
            intent = "QUERY_QUANTITY"

        if intent == "COUNTER" and any(phrase in lowered for phrase in ["middle", "meet in the middle", "split"]):
            intent = "COUNTER_MIDPOINT"

        return PlayerAction(intent=intent, price=price, quantity=quantity)

    def _format_response(self, decision, dialogue):
        return {
            "npc_text": dialogue,
            "action": decision.action,
            "price": decision.price,
            "quantity": decision.quantity,
            "done": decision.done,
            "debug": self._build_debug_info(),
            "tone": "neutral",
            "emotion": "idle"
        }

    def step(self, seller_input):
        import time
        intent_time_ms = 0
        llm_time_ms = 0

        if seller_input is None:
            decision = self.engine.next_step(None)
        else:
            self.engine.last_seller_input = seller_input
            
            start_intent = time.time()
            action = self._build_player_action(seller_input)
            intent_time_ms = int((time.time() - start_intent) * 1000)

            text_lower = str(seller_input).lower()
            text = text_lower

            # Level-1 Specific early exits (preserved as shortcuts within controller step flow for safety)
            if any(p in text for p in [
                "difference", 
                "isnt doing anything",
                "isn't doing anything",
                "almost same",
                "close enough"
            ]):
                return {
                    "npc_text": "We are very close. Let us settle this.",
                    "tone": "firm",
                    "emotion": "serious",
                    "action": "OFFER",
                    "price": self.engine.current_offer,
                    "quantity": self.engine.current_quantity,
                    "done": False,
                    "debug": self._build_debug_info(),
                    "perf_intent": intent_time_ms,
                    "perf_llm": 0
                }

            if (
                (any(q in text_lower for q in ["how much", "quantity", "how many", "quanitity"]) or action.intent == "QUERY_QUANTITY")
                and self.engine.last_seller_price is None
            ):
                quantity = self.engine.current_quantity
                quantity_text = grams_to_traditional_label(quantity)
                
                # Sync backend engine quantity and active bundle immediately to prevent logic desyncs
                self.engine.current_quantity = quantity
                self.engine.update_active_bundle([{
                    "name": self.engine.item.name.lower(),
                    "quantity": quantity,
                    "unit": "g"
                }])
                self.engine.quantity_given = True
                
                return {
                    "npc_text": f"I am looking for about {quantity_text}. What price do you offer?",
                    "tone": "neutral",
                    "emotion": "thinking",
                    "action": "QUERY_QUANTITY",
                    "price": self.engine.current_offer,
                    "quantity": quantity,
                    "done": False,
                    "debug": self._build_debug_info(),
                    "perf_intent": intent_time_ms,
                    "perf_llm": 0
                }

            if action.intent == "GENERAL_DIALOGUE":
                from npc_engine.levels.level1_market.dialogue_generator import generate_context_response
                current_state = {
                    "current_offer": self.engine.current_offer,
                    "seller_price": self.engine.last_seller_price,
                    "turns": self.engine.turns,
                    "personality": getattr(self.engine.buyer, "personality", "Polite Merchant")
                }
                start_llm = time.time()
                composed = generate_context_response(
                    player_text=seller_input,
                    buyer_name=getattr(self.engine.buyer, "name", "Abdul"),
                    buyer_origin=getattr(self.engine.buyer, "origin", "Persia"),
                    spice=self.engine.item.name,
                    current_negotiation_state=current_state
                )
                llm_time_ms = int((time.time() - start_llm) * 1000)
                
                return {
                    "npc_text": composed["text"],
                    "tone": composed["tone"],
                    "emotion": composed["emotion"],
                    "action": "WAIT",
                    "price": self.engine.current_offer,
                    "quantity": self.engine.current_quantity,
                    "done": False,
                    "debug": self._build_debug_info(),
                    "perf_intent": intent_time_ms,
                    "perf_llm": llm_time_ms
                }

            if action.intent == "NO_ITEM":
                return {
                    "npc_text": "I see. I will look elsewhere.",
                    "tone": "neutral",
                    "emotion": "idle",
                    "action": "WALK_AWAY",
                    "price": self.engine.current_offer,
                    "quantity": self.engine.current_quantity,
                    "done": True,
                    "debug": self._build_debug_info(),
                    "perf_intent": intent_time_ms,
                    "perf_llm": 0
                }

            if action.intent == "CLARIFICATION":
                return {
                    "npc_text": "I did not understand your offer. Could you repeat your price?",
                    "tone": "confused",
                    "emotion": "confused",
                    "action": "WAIT",
                    "price": self.engine.current_offer,
                    "quantity": self.engine.current_quantity,
                    "done": False,
                    "debug": self._build_debug_info(),
                    "perf_intent": intent_time_ms,
                    "perf_llm": 0
                }

            if action.intent == "OUT_OF_WORLD":
                self.engine.out_of_world_count += 1

                if self.engine.out_of_world_count >= 2:
                    return {
                        "npc_text": "I am not here for this. I will leave.",
                        "tone": "annoyed",
                        "emotion": "frustrated",
                        "action": "WALK_AWAY",
                        "price": self.engine.current_offer,
                        "quantity": self.engine.current_quantity,
                        "done": True,
                        "debug": self._build_debug_info(),
                        "perf_intent": intent_time_ms,
                        "perf_llm": 0
                    }

                return {
                    "npc_text": "Your words describe wonders unknown to me, friend. My world is of caravans, spices, and trade.",
                    "tone": "confused",
                    "emotion": "confused",
                    "action": "OUT_OF_WORLD",
                    "price": self.engine.current_offer,
                    "quantity": self.engine.current_quantity,
                    "done": False,
                    "debug": self._build_debug_info(),
                    "perf_intent": intent_time_ms,
                    "perf_llm": 0
                }

            decision = self.engine.next_step(action)

        if decision.action == "END":
            return self._format_response(decision, None)

        if decision.action == "WALK_AWAY":
            return {
                "npc_text": "I am leaving.",
                "tone": "annoyed",
                "emotion": "frustrated",
                "action": "WALK_AWAY",
                "price": self.engine.current_offer,
                "quantity": self.engine.current_quantity,
                "done": True,
                "debug": self._build_debug_info(),
                "perf_intent": intent_time_ms,
                "perf_llm": 0
            }

        # Generate dialogue using injected generator
        start_llm = time.time()
        composed = self.dialogue_fn(decision, self.engine)
        llm_time_ms = int((time.time() - start_llm) * 1000)

        if decision.action == "ACCEPT" and composed.get("action", decision.action) == "ACCEPT":
            final_quantity = self.format_final_quantity()
            final_item = self.engine.final_item or self.engine.item.name
            composed["text"] += f"\n\nTransaction complete.\nFinal Deal: {final_quantity} {final_item} for {decision.price} varahas"

        debug_info = self._build_debug_info()

        return {
            "npc_text": composed["text"],
            "tone": composed["tone"],
            "emotion": composed["emotion"],
            "action": composed.get("action", decision.action),
            "price": decision.price,
            "quantity": decision.quantity,
            "done": decision.done or composed.get("action") == "WALK_AWAY",
            "debug": debug_info,
            "perf_intent": intent_time_ms,
            "perf_llm": llm_time_ms
        }

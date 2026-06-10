import uuid
from npc_engine.levels.level1_market.buyer_model import Buyer
from npc_engine.levels.level1_market.item_model import Item
from npc_engine.levels.level1_market.negotiation_engine import NegotiationEngine
from npc_engine.levels.level1_market.dialogue_generator import generate_dialogue
from npc_engine.levels.level1_market.intent_classifier import classify_intent, extract_quantity_info
from npc_engine.levels.level1_market.input_interpreter import extract_price
from npc_engine.core.controller import Controller
from npc_engine.core.persistence import load_session, save_session, record_negotiation_deal
from npc_engine.core.measurements import get_random_spice_quantity
import random

# Default marketplace spices
ITEMS = [
    Item("pepper", base_price_per_unit=80, market_multiplier=1.2, unit="kg", quantity=1),
    Item("clove", base_price_per_unit=70, market_multiplier=1.3, unit="kg", quantity=1),
    Item("cinnamon", base_price_per_unit=80, market_multiplier=1.3, unit="kg", quantity=1),
    Item("cardamom", base_price_per_unit=100, market_multiplier=1.5, unit="kg", quantity=1),
]


class NPCSession:
    """
    Manages an active game level session (Level 1 Marketplace by default)
    by injecting level-specific components into the general runner controller.
    Supports persistent local serialization to disk.
    """
    def __init__(self, session_id: str = None, active_event: dict = None):
        self.session_id = session_id or str(uuid.uuid4())
        self.active_event = active_event
        
        # Load or initialize the persistent state on disk
        state = load_session(self.session_id)
        
        # Adjust buyer parameters based on persistent player reputation
        reputation = state.get("global_metrics", {}).get("reputation", 20)
        self.buyer = Buyer(reputation)
        self.buyer.adjust_from_reputation(reputation)
        
        # Randomize quantities capped by what the player has in stock
        self.session_items = []
        player_inventory = state.setdefault("inventory", {})
        
        for default_item in ITEMS:
            stock_grams = float(player_inventory.get(default_item.name.lower(), 0.0))
            if stock_grams <= 0:
                continue # Skip out-of-stock spices
                
            grams = get_random_spice_quantity(default_item.name)
            grams = min(grams, stock_grams) # Cap at available stock
            
            randomized_item = Item(
                name=default_item.name,
                base_price_per_unit=default_item.base_price_per_unit,
                market_multiplier=default_item.market_multiplier,
                unit="kg",
                quantity=grams / 1000.0
            )
            self.session_items.append(randomized_item)
            
        # Emergency Fallback: If all stocks are empty, supply a tiny emergency quantity of pepper
        if not self.session_items:
            self.session_items.append(Item("pepper", base_price_per_unit=80, market_multiplier=1.2, unit="kg", quantity=0.28))
            
        self.available_items = self.session_items.copy()
        random.shuffle(self.available_items)
        self.item = self.available_items.pop()
        
        # Instantiate Level 1 engine with session-specific items
        self.engine = NegotiationEngine(self.buyer, self.item, all_items=self.session_items, active_event=self.active_event)
        
        # Inject level-specific functions into the generic controller
        self.controller = Controller(
            self.engine,
            classify_intent_fn=classify_intent,
            extract_quantity_info_fn=extract_quantity_info,
            extract_price_fn=extract_price,
            dialogue_fn=generate_dialogue
        )

    # Start conversation
    def start(self):
        return self.controller.step(None)

    # Continue negotiation
    def step(self, player_input):
        response = self.controller.step(player_input)
        
        if response["done"]:
            # Commit the negotiation outcome to disk memory
            spice_name = self.engine.item.name
            final_price = response.get("price")
            final_quantity = response.get("quantity")  # grams
            trust = self.engine.trust
            frustration = self.engine.frustration
            out_count = self.engine.out_of_world_count
            action = response.get("action", "WALK_AWAY")
            
            # Deduct inventory stock if transaction is complete and accepted
            if action == "ACCEPT" and final_quantity is not None:
                from npc_engine.core.inventory import deduct_inventory_stock
                deduct_inventory_stock(self.session_id, spice_name, final_quantity)
            
            # Record deal using persistent module
            record_negotiation_deal(
                session_id=self.session_id,
                spice_name=spice_name,
                final_price=final_price,
                final_quantity=final_quantity,
                trust=trust,
                frustration=frustration,
                out_of_world_count=out_count,
                outcome=action,
                market_price=self.engine.market_price
            )

            # Compile transaction complete summary for acceptances
            if action == "ACCEPT" and final_price is not None and final_quantity is not None:
                from npc_engine.core.measurements import grams_to_traditional_label
                base_prices = {"pepper": 80, "clove": 70, "cinnamon": 80, "cardamom": 100}
                base_price = base_prices.get(spice_name.lower(), 80)
                profit = int(max(0, final_price - base_price * (final_quantity / 1000.0)))
                
                # Fetch last recorded respect change
                state = load_session(self.session_id)
                last_deal = state["level_history"]["level1_market"]["deals"][-1]
                respect_change = last_deal.get("respect_change", 5)
                
                response["transaction"] = {
                    "item": spice_name.capitalize(),
                    "quantity": grams_to_traditional_label(final_quantity),
                    "earned": int(final_price),
                    "profit": profit,
                    "respect_change": respect_change,
                    "buyer_name": getattr(self.buyer, "name", "Abdul Rahman"),
                    "buyer_origin": getattr(self.buyer, "origin", "Persian Trader")
                }
            
            # Check if there are more items to negotiate
            if self.available_items:
                self.item = self.available_items.pop()
                self.engine = NegotiationEngine(self.buyer, self.item, all_items=self.session_items, active_event=self.active_event)
                self.controller = Controller(
                    self.engine,
                    classify_intent_fn=classify_intent,
                    extract_quantity_info_fn=extract_quantity_info,
                    extract_price_fn=extract_price,
                    dialogue_fn=generate_dialogue
                )
            else:
                # Level 1 is fully completed! Mark completion flag in JSON state
                state = load_session(self.session_id)
                state["level_history"]["level1_market"]["completed"] = True
                completed_levels = state["global_metrics"].setdefault("completed_levels", [])
                if "level1_market" not in completed_levels:
                    completed_levels.append("level1_market")
                save_session(self.session_id, state)
                
        return response


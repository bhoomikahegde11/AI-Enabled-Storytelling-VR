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
    def __init__(self, session_id: str = None):
        self.session_id = session_id or str(uuid.uuid4())
        
        # Load or initialize the persistent state on disk
        load_session(self.session_id)
        
        self.buyer = Buyer()
        
        # Randomize quantities based on commercial density for each item
        self.session_items = []
        for default_item in ITEMS:
            grams = get_random_spice_quantity(default_item.name)
            randomized_item = Item(
                name=default_item.name,
                base_price_per_unit=default_item.base_price_per_unit,
                market_multiplier=default_item.market_multiplier,
                unit="kg",
                quantity=grams / 1000.0
            )
            self.session_items.append(randomized_item)
            
        self.available_items = self.session_items.copy()
        random.shuffle(self.available_items)
        self.item = self.available_items.pop()
        
        # Instantiate Level 1 engine with session-specific items
        self.engine = NegotiationEngine(self.buyer, self.item, all_items=self.session_items)
        
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
            final_quantity = response.get("quantity")
            trust = self.engine.trust
            frustration = self.engine.frustration
            out_count = self.engine.out_of_world_count
            action = response.get("action", "WALK_AWAY")
            
            # Record deal using persistent module
            record_negotiation_deal(
                session_id=self.session_id,
                spice_name=spice_name,
                final_price=final_price,
                final_quantity=final_quantity,
                trust=trust,
                frustration=frustration,
                out_of_world_count=out_count,
                outcome=action
            )
            
            # Check if there are more items to negotiate
            if self.available_items:
                self.item = self.available_items.pop()
                self.engine = NegotiationEngine(self.buyer, self.item, all_items=self.session_items)
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


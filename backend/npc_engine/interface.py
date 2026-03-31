from npc_engine.engine.buyer_model import Buyer
from npc_engine.engine.item_model import Item
from npc_engine.engine.negotiation_engine import NegotiationEngine
from npc_engine.llm.dialogue_generator import generate_dialogue
from npc_engine.core.controller import Controller
import random

# Default items (same as main.py)
ITEMS = [
    Item("pepper", base_price_per_unit=80, market_multiplier=1.2, unit="kg", quantity=1),
    Item("clove", base_price_per_unit=70, market_multiplier=1.3, unit="kg", quantity=1),
    Item("cinnamon", base_price_per_unit=80, market_multiplier=1.3, unit="kg", quantity=1),
    Item("cardamom", base_price_per_unit=100, market_multiplier=1.5, unit="kg", quantity=1),
]


class NPCSession:
    def __init__(self):
        self.buyer = Buyer()
        self.item = random.choice(ITEMS)
        self.engine = NegotiationEngine(self.buyer, self.item, all_items=ITEMS)
        self.controller = Controller(self.engine, generate_dialogue)

    # 🔥 Helper: format structured response
    def _format_response(self, action, price, dialogue):
        quantity = getattr(self.engine, "current_quantity_grams", 1000)
        item_name = getattr(self.engine.item, "name", "item")

        return {
            "action": action,
            "price": price,
            "quantity_grams": quantity,
            "item": item_name,
            "total_price": price,  # can refine later if needed
            "dialogue": dialogue
        }

    # Start conversation
    def start(self):
        action, price, dialogue = self.controller.step(None)
        return self._format_response(action, price, dialogue)

    # Continue negotiation
    def step(self, player_input):
        action, price, dialogue = self.controller.step(player_input)
        return self._format_response(action, price, dialogue)
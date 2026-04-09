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
        self.available_items = ITEMS.copy()
        random.shuffle(self.available_items)
        self.item = self.available_items.pop()
        self.engine = NegotiationEngine(self.buyer, self.item, all_items=ITEMS)
        self.controller = Controller(self.engine, generate_dialogue)

    # Start conversation
    def start(self):
        return self.controller.step(None)

    # Continue negotiation
    def step(self, player_input):
        response = self.controller.step(player_input)
        if response["done"] and self.available_items:
            self.item = self.available_items.pop()
            self.engine = NegotiationEngine(self.buyer, self.item, all_items=ITEMS)
            self.controller = Controller(self.engine, generate_dialogue)
        return response

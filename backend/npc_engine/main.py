from npc_engine.engine.buyer_model import Buyer
from npc_engine.engine.item_model import Item
from npc_engine.engine.negotiation_engine import NegotiationEngine
from npc_engine.llm.dialogue_generator import generate_dialogue
from npc_engine.core.controller import Controller
from npc_engine.engine.item_model import Item
import random

ITEMS = [
    Item("pepper", base_price_per_unit=80, market_multiplier=1.2, unit="kg", quantity=1),      # ~96/kg
    Item("clove", base_price_per_unit=70, market_multiplier=1.3, unit="kg", quantity=1),       # ~91/kg
    Item("cinnamon", base_price_per_unit=80, market_multiplier=1.3, unit="kg", quantity=1),    # ~104/kg
    Item("cardamom", base_price_per_unit=100, market_multiplier=1.5, unit="kg", quantity=1),   # ~150/kg
]


def run():
    buyer = Buyer()
    item = random.choice(ITEMS)

    engine = NegotiationEngine(buyer, item, all_items=ITEMS)

    while True:
        buyer = Buyer()

        print("\nA new customer approaches your stall.")
        print(f"Personality: {buyer.personality}")
        print(f"(Desperation: {buyer.desperation}, Patience: {buyer.patience})\n")

        engine = NegotiationEngine(buyer, item, all_items=ITEMS)
        controller = Controller(engine, generate_dialogue)

        response = controller.step(None)
        print(f"Buyer: {response['npc_text']}")

        while True:
            seller_input = input("You: ")

            response = controller.step(seller_input)
            if response["action"] == "END":
                break
            print(f"Buyer: {response['npc_text']}")

            if response["done"]:
                print("\nCustomer leaves the stall.\n")
                break


if __name__ == "__main__":
    run()

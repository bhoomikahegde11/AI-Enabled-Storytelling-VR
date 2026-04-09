from npc_engine.core.models import PlayerAction
from npc_engine.engine.input_interpreter import extract_price
from npc_engine.llm.intent_classifier import classify_intent, extract_quantity_info


class Controller:
    def __init__(self, engine, dialogue_fn):
        self.engine = engine
        self.dialogue_fn = dialogue_fn

    def format_final_quantity(self):
        quantity = self.engine.final_quantity
        if quantity is None:
            return "1kg"
        if quantity >= 1000:
            quantity_kg = quantity / 1000
            quantity_text = int(quantity_kg) if float(quantity_kg).is_integer() else round(quantity_kg, 2)
            return f"{quantity_text}kg"
        return f"{int(quantity) if float(quantity).is_integer() else quantity}g"

    def _build_player_action(self, seller_input):
        text = str(seller_input).strip()
        if not text:
            return PlayerAction(intent="CONTINUE", price=None, quantity=None)

        lowered = text.lower()
        quantity_info = extract_quantity_info(text)
        quantity = quantity_info["quantity_grams"] if quantity_info is not None else None
        price = extract_price(text)

        result = classify_intent(text, context={
            "in_negotiation": self.engine.started,
            "item_name": self.engine.item.name,
            "last_system_action": self.engine.last_action,
            "last_seller_price": self.engine.last_seller_price,
            "current_offer": self.engine.current_offer
        })
        intent = result["intent"]

        if intent == "NO_ITEM" and quantity is not None and any(
            word in lowered for word in ["only", "left", "have", "but", "instead", "g", "gm", "kg"]
        ):
            intent = "QUANTITY_CHANGE"

        if intent == "QUERY" and any(term in lowered for term in ["how many", "how much", "grams", "gram", "kg", "quantity"]):
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
        }

    def step(self, seller_input):
        if seller_input is None:
            decision = self.engine.next_step(None)
        else:
            action = self._build_player_action(seller_input)
            decision = self.engine.next_step(action)

        if decision.action == "END":
            return self._format_response(decision, None)

        dialogue = self.dialogue_fn(decision, self.engine.buyer.personality, self.engine.item.name)

        if decision.action == "ACCEPT":
            final_quantity = self.format_final_quantity()
            final_item = self.engine.final_item or self.engine.item.name
            dialogue += f"\n\nTransaction complete.\nFinal Deal: {final_quantity} {final_item} for {decision.price} varahas"

        return self._format_response(decision, dialogue)

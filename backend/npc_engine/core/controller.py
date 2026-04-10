from npc_engine.core.models import PlayerAction
from npc_engine.engine.input_interpreter import extract_price
from npc_engine.llm.intent_classifier import classify_intent, extract_quantity_info
from npc_engine.dialogue.dialogue_composer import DialogueComposer


class Controller:
    def __init__(self, engine, dialogue_fn):
        self.engine = engine
        self.dialogue_fn = dialogue_fn
        self.composer = DialogueComposer()

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
        
        if result is None:
            result = {
                "intent": "IRRELEVANT"
            }

        intent = result.get("intent", "IRRELEVANT")
        price = result.get("price", price)
        quantity = result.get("quantity", quantity)

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
            "action": composed.get("action", decision.action),
            "price": decision.price,
            "quantity": decision.quantity,
            "done": decision.done or composed.get("action") == "WALK_AWAY",
        }

    def step(self, seller_input):
        if seller_input is None:
            decision = self.engine.next_step(None)
        else:
            action = self._build_player_action(seller_input)

            text_lower = str(seller_input).lower()
            text = text_lower

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
                    "price": self.engine.current_offer
                }

            if (
                any(q in text_lower for q in ["how much", "quantity", "how many"])
                and self.engine.last_seller_price is None
            ):
                quantity = self.engine.final_quantity or 1000
                quantity_text = f"{quantity}g" if quantity < 1000 else f"{int(quantity)//1000}kg"
                return {
                    "npc_text": f"I am looking for about {quantity_text}. What price do you offer?",
                    "tone": "neutral",
                    "emotion": "thinking",
                    "action": "QUERY_QUANTITY",
                    "done": False
                }

            if action.intent == "NO_ITEM":
                return {
                    "npc_text": "I see. I will look elsewhere.",
                    "tone": "neutral",
                    "emotion": "idle",
                    "action": "WALK_AWAY",
                    "done": True
                }

            if action.intent == "OUT_OF_WORLD":
                self.engine.out_of_world_count += 1

                if self.engine.out_of_world_count >= 2:
                    return {
                        "npc_text": "I am not here for this. I will leave.",
                        "tone": "annoyed",
                        "emotion": "frustrated",
                        "action": "WALK_AWAY",
                        "done": True
                    }

                return {
                    "npc_text": "Let us focus on the trade.",
                    "tone": "firm",
                    "emotion": "serious",
                    "action": "OUT_OF_WORLD",
                    "done": False
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
                "done": True
            }

        composed = self.composer.compose(decision, self.engine)

        if decision.action == "ACCEPT" and composed.get("action", decision.action) == "ACCEPT":
            final_quantity = self.format_final_quantity()
            final_item = self.engine.final_item or self.engine.item.name
            composed["text"] += f"\n\nTransaction complete.\nFinal Deal: {final_quantity} {final_item} for {decision.price} varahas"

        debug_info = {
            "stage": self.engine.stage,
            "current_offer": self.engine.current_offer,
            "seller_price": self.engine.last_seller_price,
            "desperation": round(self.engine.buyer.desperation, 2),
            "frustration": round(self.engine.frustration, 2),
            "turn": self.engine.turns
        }

        return {
            "npc_text": composed["text"],
            "tone": composed["tone"],
            "emotion": composed["emotion"],
            "action": composed.get("action", decision.action),
            "price": decision.price,
            "quantity": decision.quantity,
            "done": decision.done or composed.get("action") == "WALK_AWAY",
            "debug": debug_info
        }

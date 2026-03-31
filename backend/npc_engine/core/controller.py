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

    def step(self, seller_input):
        # -------------------------------
        # 🔥 FIX: handle None / blank input safely
        # -------------------------------
        if seller_input is None:
            result = self.engine.next_step(seller_input)
        elif str(seller_input).strip() == "":
            result = self.engine.respond("OFFER", price=self.engine.current_offer, tone="neutral")
        else:
            result = self.engine.next_step(seller_input)

        action = result["action"]
        price = result.get("price")
        context = result.get("context", {})
        tone = result.get("tone", "neutral")

        if action == "END":
            return None, None, None

        # -------------------------------
        # GENERATE DIALOGUE
        # -------------------------------
        dialogue = self.dialogue_fn(
            action,
            price,
            self.engine.buyer.personality,
            self.engine.item.name,
            context,
            politeness=self.engine.buyer.politeness,  # keep this
            tone=tone
        )

        # -------------------------------
        # 🔥 HANDLE DEAL COMPLETION
        # -------------------------------
        if action == "ACCEPT":
            final_quantity = self.format_final_quantity()
            final_item = self.engine.final_item or self.engine.item.name
            summary = f"\n\nTransaction complete.\nFinal Deal: {final_quantity} {final_item} for {price} varahas"
            return action, price, dialogue + summary

        # -------------------------------
        # 🔥 SAFE TERMINATION
        # -------------------------------
        return action, price, dialogue

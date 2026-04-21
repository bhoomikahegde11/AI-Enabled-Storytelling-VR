import random

class DialogueComposer:

    def compose(self, decision, engine):
        action = decision.action
        price = decision.price
        personality = engine.buyer.personality
        frustration = engine.frustration
        stage = engine.stage

        has_price = engine.last_seller_price is not None
        has_quantity = engine.quantity_given
        has_item = engine.started
        
        market_price = engine.market_price
        seller_price = engine.last_seller_price
        desperation = engine.buyer.desperation
        turns = engine.turns
        item_name = engine.item.name
        item_known = item_name is not None
        out_count = engine.out_of_world_count
        
        prev_price = engine.prev_seller_price
        current_price = engine.last_seller_price

        text = self.generate_text(action, price, stage, has_price, has_quantity, market_price, seller_price, desperation, turns, item_name, out_count, prev_price, current_price, personality)
        
        # Internal override for loop breakers to ensure engine knows we walked away
        if "going nowhere" in text or "leave if this continues" in text or "done here" in text:
            action = "WALK_AWAY"
            
        tone = self.select_tone(action, frustration)
        emotion = self.select_emotion(action, frustration)

        return {
            "text": text,
            "tone": tone,
            "emotion": emotion,
            "action": action,
            "price": price
        }

    def generate_text(self, action, price, stage, has_price, has_quantity, market_price, seller_price, desperation, turns, item_name, out_count, prev_price=None, current_price=None, personality="Polite Merchant"):
        if prev_price is not None and current_price is not None:
            if current_price > prev_price:
                return random.choice([
                    "That price just went up.",
                    "You are raising the price?",
                    "That is not how this works.",
                    "If anything, the price should go down."
                ])

            if current_price < prev_price:
                return random.choice([
                    "That is better.",
                    "Now we are talking.",
                    "That is a reasonable drop."
                ])

        if not has_quantity and not has_price:
            open_lines = [
                f"I am looking to buy {item_name}. How much do you have?",
                f"Do you have {item_name}? How much can you sell?",
                f"I need some {item_name}. What quantity do you have?",
                f"Are you selling {item_name}? How much is available?"
            ]
            return random.choice(open_lines)

        if turns > 5 and seller_price == price:
            return "We are not reaching an agreement."

        if seller_price is not None and market_price is not None:
            if seller_price > market_price * 1.8:
                return random.choice([
                    "That price is far too high.",
                    "That is unreasonable.",
                    "I cannot pay anything close to that."
                ])

            if seller_price < market_price * 0.3:
                return random.choice([
                    "That is far too low.",
                    "That will not work at all.",
                    "You must offer more than that."
                ])

        gap = None
        prefix = ""
        if seller_price is not None and price is not None:
            gap = abs(seller_price - price)

        if gap is not None:
            if gap > max(10, seller_price * 0.3):
                prefix = random.choice([
                    "That is far from my expectation.",
                    "We are quite far apart.",
                ])
            elif max(3, seller_price * 0.1) < gap <= max(10, seller_price * 0.3):
                prefix = random.choice([
                    "We are getting closer.",
                    "That is better.",
                ])
            elif gap <= max(3, seller_price * 0.1):
                prefix = random.choice([
                    "We are very close.",
                    "This is a small difference.",
                ])

        text = "Speak clearly."

        if action == "ASK_ITEM":
            text = random.choice([
                "Do you have this item?",
                "Are you selling this?",
                "Can I buy this here?"
            ])

        elif action == "OFFER":
            if price is None:
                text = f"What price do you want for this {item_name}?"
            elif personality == "Aggressive Trader":
                text = random.choice([
                    f"{price}. Take it or leave it.",
                    f"I will not go beyond {price}.",
                    f"{price}. Final."
                ])
            elif personality == "Cautious Buyer":
                text = random.choice([
                    f"I can offer {price}… but I am unsure.",
                    f"Maybe {price} is fair?",
                    f"I can go up to {price}, I think."
                ])
            else:
                text = random.choice([
                    f"I can offer {price} for this {item_name}.",
                    f"Perhaps {price} would be reasonable.",
                    f"I would be comfortable at {price}."
                ])

        elif action == "REJECT":
            if personality == "Aggressive Trader":
                text = random.choice([
                    "That is unacceptable.",
                    "Do not waste my time.",
                    "That price is ridiculous."
                ])
            elif personality == "Cautious Buyer":
                text = random.choice([
                    "That seems a bit high…",
                    "I am not comfortable with that.",
                    "That feels too much for me."
                ])
            else:
                text = random.choice([
                    "I am afraid that is too high.",
                    "I cannot agree to that price.",
                    "Perhaps you could reconsider?"
                ])

        elif action == "ACCEPT":
            if personality == "Aggressive Trader":
                text = "Fine. Deal."
            elif personality == "Cautious Buyer":
                text = "Alright… I think this works."
            else:
                text = "Very well, we have a deal."

        elif action == "WALK_AWAY":
            text = random.choice([
                "This is going nowhere. I am leaving.",
                "We are not reaching an agreement. I will take my leave.",
                "This deal is not worth it."
            ])

        elif action == "NO_ITEM":
            text = random.choice([
                "Then I will look elsewhere.",
                "I see. I will find another seller.",
                "Alright, I will move on."
            ])
            
        elif action == "OUT_OF_WORLD":
            if out_count == 1:
                return "Let us stay focused on the trade."
            if out_count == 2:
                return "This is not the place for that. Talk business."
            if out_count >= 3:
                return "I am done here."

        elif action == "ASK_PRICE":
            if has_price:
                return random.choice([
                    "That price is noted.",
                    "Alright, I understand your price.",
                    "Let us work with that price."
                ])

            return f"What price do you want for this {item_name}?"

        elif action == "SET_QUANTITY":
            text = f"Alright, {item_name}. Now tell me your price."

        if prefix and action in ["OFFER", "REJECT", "COUNTER"]:
            text = f"{prefix} {text}"

        seller_words = ["give", "i have", "available", "i sell"]

        if any(word in text.lower() for word in seller_words):
            # fallback safe buyer line
            return "Let us discuss the price."

        return text

    def select_tone(self, action, frustration):
        if action == "ASK_ITEM":
            return "neutral"

        if action == "SET_QUANTITY":
            return "neutral"

        if action == "REJECT":
            return "firm"

        if action == "OFFER":
            return "neutral"

        if action == "ACCEPT":
            return "friendly"

        if frustration > 0.7:
            return "annoyed"

        return "neutral"

    def select_emotion(self, action, frustration):
        if action == "ACCEPT":
            return "happy"

        if frustration > 0.7:
            return "frustrated"

        if action == "OFFER":
            return "thinking"

        if action == "REJECT":
            return "serious"

        return "idle"

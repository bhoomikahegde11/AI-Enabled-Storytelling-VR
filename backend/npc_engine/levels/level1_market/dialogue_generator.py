import random
import re
from npc_engine.core.rag import RAGRetriever
from npc_engine.llm.llm_client import run_llm, llm_loaded
from npc_engine.core.measurements import grams_to_traditional_label

_LAST_VARIANT_INDEX = {}
_rag = RAGRetriever()


def pick_varied(key, options):
    if len(options) == 1:
        return options[0]

    last_index = _LAST_VARIANT_INDEX.get(key)
    available_indexes = [i for i in range(len(options)) if i != last_index]
    choice_index = random.choice(available_indexes)
    _LAST_VARIANT_INDEX[key] = choice_index
    return options[choice_index]


def generate_dialogue(decision, engine):
    """
    Unified Dialogue Generator for Level 1 (Marketplace Negotiation).
    Consolidates simple templates, personality filters, and emotional variations.
    Enriches with historical facts via keyword-based RAG and rephrases using local GGUF LLM.
    """
    action = decision.action
    price = decision.price
    personality = engine.buyer.personality
    frustration = engine.frustration
    stage = engine.stage

    has_price = engine.last_seller_price is not None
    has_quantity = engine.quantity_given
    
    market_price = engine.market_price
    seller_price = engine.last_seller_price
    desperation = engine.buyer.desperation
    turns = engine.turns
    item_name = engine.item.name
    out_count = engine.out_of_world_count
    
    prev_price = engine.prev_seller_price
    current_price = engine.last_seller_price

    # 1. Generate text
    if action == "QUERY_BUYER_BUDGET":
        buyer = engine.buyer
        if not hasattr(buyer, "target_price") or buyer.target_price is None:
            buyer.target_price = int(engine.current_offer)
        buyer.current_offer = int(engine.current_offer)
        buyer.max_budget = int(engine.max_price)

        # Randomly choose one of the three metrics to reveal in the response
        metric = random.choice(["current_offer", "target_price", "max_budget"])
        
        if personality == "Aggressive Trader":
            if metric == "current_offer":
                text = f"My offer is {buyer.current_offer} varahas. No more."
            elif metric == "target_price":
                text = f"I'm willing to give {buyer.target_price} varahas for this."
            else:
                text = f"My maximum budget is {buyer.max_budget} varahas. Do not ask for more."
        elif personality == "Cautious Buyer":
            if metric == "current_offer":
                text = f"Perhaps {buyer.current_offer} varahas is what I can offer."
            elif metric == "target_price":
                text = f"I was hoping to spend around {buyer.target_price} varahas."
            else:
                text = f"I could go up to {buyer.max_budget} varahas, but that is my absolute limit."
        else: # Polite Merchant / default
            if metric == "current_offer":
                text = f"My offer would be {buyer.current_offer} varahas."
            elif metric == "target_price":
                text = f"I could pay around {buyer.target_price} varahas."
            else:
                text = f"I might stretch to {buyer.max_budget} varahas, but no higher."
    else:
        text = _generate_text(
            action, price, stage, has_price, has_quantity, market_price, 
            seller_price, desperation, turns, item_name, out_count, 
            prev_price, current_price, personality
        )
    
    # Internal override for loop breakers to ensure engine knows we walked away
    if "going nowhere" in text or "leave if this continues" in text or "done here" in text:
        action = "WALK_AWAY"
        
    # 2. Select tone and emotion
    tone = _select_tone(action, frustration)
    emotion = _select_emotion(action, frustration)

    # 3. LLM Dialogue Rephrasing Layer (GGUF Offline fallback)
    if llm_loaded and text:
        fact_context = _rag.retrieve_context(item_name, stage)
        
        # Determine which numbers are active in the template to avoid forcing unrelated numbers
        active_numbers_instructions = []
        
        if action == "QUERY_BUYER_BUDGET":
            buyer = engine.buyer
            for val in [buyer.target_price, buyer.current_offer, buyer.max_budget]:
                if str(val) in text:
                    active_numbers_instructions.append(f"1. You MUST preserve the exact budget/price value of '{val}' varahas exactly as it is in the template.")
        else:
            has_price_in_template = str(price) in text if price is not None else False
            if has_price_in_template:
                active_numbers_instructions.append(f"1. You MUST preserve the exact price offer of '{price}' varahas exactly as it is in the template.")
        
        # Check quantity representations in template
        has_qty_in_template = False
        if engine.current_quantity is not None:
            qty_g = f"{int(engine.current_quantity)}g"
            qty_kg = f"{int(engine.current_quantity)//1000}kg"
            if qty_g in text or qty_kg in text or str(int(engine.current_quantity)) in text or any(
                term in text.lower() for term in ["palam", "seer", "veesai", "viss", "manangu", "maund", "bahar", "candy"]
            ):
                has_qty_in_template = True
                
        if has_qty_in_template:
            qty_label = grams_to_traditional_label(engine.current_quantity)
            active_numbers_instructions.append(f"1. You MUST preserve the exact traditional quantity of '{qty_label}' exactly as it is in the template.")
            
        numbers_clause = "\n".join(active_numbers_instructions) if active_numbers_instructions else "1. DO NOT invent, modify, or add any numbers."

        # Extract buyer identity, reputation, and market event context
        buyer_name = getattr(engine.buyer, "name", "a buyer")
        buyer_origin = getattr(engine.buyer, "origin", "a merchant")
        buyer_interest = getattr(engine.buyer, "interest", "spices")
        buyer_wealth = getattr(engine.buyer, "wealth", "medium")
        player_reputation = getattr(engine.buyer, "reputation", 50.0)
        
        reputation_context = ""
        if player_reputation < 35:
            reputation_context = "\nPlayer Reputation Context: You know this seller as a Greedy Haggler who overcharges other buyers. Start suspicious, impatient, and irritated by their demands."
        elif player_reputation > 75:
            reputation_context = "\nPlayer Reputation Context: You know this seller as an honest, Fair Trader. Be highly respectful, patient, and cooperative."
            
        event_context = ""
        active_event = getattr(engine, "active_event", None)
        if active_event:
            event_context = f"\nActive Market Event Context: {active_event['name']} - {active_event['description']}. You can occasionally reference this event in your speech if it fits natural conversation."

        player_context = ""
        if getattr(engine, "last_seller_input", None):
            player_context = f"\nThe seller (player) just said: \"{engine.last_seller_input}\"\n"

        # Use Llama-3 official instruction tags for direct, zero-preamble, blazing fast responses
        prompt = f"""<|begin_of_text|><|start_header_id|>system<|end_header_id|>
You are an NPC buyer in the Hampi bazaars of the Vijayanagara Empire (1500s).
Role Context: You are visiting the player's stall to BUY spices from them. The player is the SELLER. You are the BUYER. You must NEVER act as the seller.
Your Name: {buyer_name}
Your Identity/Origin: {buyer_origin}
Your Spice Interest: {buyer_interest}
Your Wealth Class: {buyer_wealth}
Your Personality is {personality}, your current tone is {tone}, and your current emotion is {emotion}.{reputation_context}{event_context}
Historical Context: {fact_context}
{player_context}
Task: Rephrase the dialogue template in character, responding naturally to what the seller just said if relevant. Keep it extremely concise (1-2 sentences).
Rules:
{numbers_clause}
2. DO NOT add any preambles, explanations, or quotes. Output ONLY the rephrased spoken dialogue.
3. NEVER use modern terms: no rupees, dollars, pounds, euros, sir, okay, website, phone, or any post-1500s concept. Use only 'varahas' for currency.
<|eot_id|><|start_header_id|>user<|end_header_id|>
Dialogue template to rephrase: "{text}"
<|eot_id|><|start_header_id|>assistant<|end_header_id|>
"""

        # Call local LLM with stop tokens enabled
        rephrased = run_llm(prompt, max_tokens=96)
        
        if rephrased:
            rephrased = rephrased.strip()
            
            # Robust Post-Processing Preamble Scrubbing:
            # If the model put the rephrased speech in quotes inside explanatory text, extract it.
            quotes = re.findall(r'"([^"]*)"', rephrased)
            if quotes:
                for q in quotes:
                    q_numbers = re.findall(r'\b\d+\b', q)
                    template_numbers = re.findall(r'\b\d+\b', text)
                    if all(num in q for num in template_numbers) and len(q) > 5:
                        rephrased = q
                        break
            
            # Remove typical LLM prefix tags
            rephrased = re.sub(r'^(rephrased\s+dialogue|character\s+speech|here\s+is\s+the\s+rephrased\s+dialogue|here\s+is\s+your\s+rephrased\s+speech|rephrased):', '', rephrased, flags=re.IGNORECASE)
            rephrased = rephrased.strip().strip('"').strip("'").strip()
            
            # Safety validation checks
            template_numbers = re.findall(r'\b\d+\b', text)
            rephrased_numbers = re.findall(r'\b\d+\b', rephrased)
            numbers_match = True
            for num in template_numbers:
                if num not in rephrased:
                    numbers_match = False
                    break
            
            # If template had NO numbers, the LLM should not invent any
            no_number_hallucination = True
            if not template_numbers and rephrased_numbers:
                no_number_hallucination = False
            
            # Immersion-breaking modern terms that should NEVER appear in 1500s dialogue
            immersion_breakers = [
                "rupee", "rupees", "dollar", "dollars", "euro", "euros", "pound", "pounds",
                "bitcoin", "crypto", "website", "internet", "phone", "mobile", "online",
                "email", "app", "computer", "laptop", "google", "amazon",
                "sir,", "ma'am", "ok", "okay",  # overly modern address
                "electric", "battery", "camera", "photograph",
                "train", "bus", "car", "truck", "airplane",
                "download", "upload", "screenshot", "password",
            ]
            has_immersion_breaker = any(word in rephrased.lower() for word in immersion_breakers)
            
            # Future purchase validation for ACCEPT responses
            has_future_accept_phrase = False
            if action == "ACCEPT":
                forbidden_accept_phrases = [
                    "return later", "will return", "shall return", "return with more", 
                    "may purchase", "another time", "perhaps", "buy later", "purchase later",
                    "next time", "return again", "come back", "will buy", "shall buy",
                    "might buy", "might purchase", "return to buy", "return to purchase"
                ]
                has_future_accept_phrase = any(phrase in rephrased.lower() for phrase in forbidden_accept_phrases)

            # Spice name hallucination check for ACCEPT and WALK_AWAY responses
            has_wrong_spice = False
            if action in ["ACCEPT", "WALK_AWAY"]:
                all_spices = ["pepper", "cardamom", "cinnamon", "clove", "ginger"]
                correct_spice = item_name.lower()
                for spice in all_spices:
                    if spice != correct_spice and spice in rephrased.lower():
                        has_wrong_spice = True
                        break
                    
            is_valid = (
                numbers_match 
                and no_number_hallucination
                and not has_immersion_breaker
                and not has_future_accept_phrase
                and not has_wrong_spice
                and len(rephrased) > 5 
                and not any(bad in rephrased.lower() for bad in ["assistant:", "rephrased dialogue:", "system:", "npc:", "prompt:"])
            )
            
            if is_valid:
                print(f"[INFO LLM] Rephrasing successful: \"{text}\" -> \"{rephrased}\"")
                text = rephrased
            else:
                reasons = []
                if not numbers_match:
                    reasons.append("missing template numbers")
                if not no_number_hallucination:
                    reasons.append(f"hallucinated numbers {rephrased_numbers}")
                if has_immersion_breaker:
                    broken = [w for w in immersion_breakers if w in rephrased.lower()]
                    reasons.append(f"immersion breakers: {broken}")
                if has_future_accept_phrase:
                    reasons.append("future purchase language in ACCEPT")
                print(f"[WARNING LLM] Failsafe trigger: LLM output \"{rephrased}\" failed validations ({', '.join(reasons)}). Defaulting to template.")

    return {
        "text": text,
        "tone": tone,
        "emotion": emotion,
        "action": action,
        "price": price
    }


def _generate_text(action, price, stage, has_price, has_quantity, market_price, seller_price, desperation, turns, item_name, out_count, prev_price=None, current_price=None, personality="Polite Merchant"):
    if prev_price is not None and current_price is not None:
        if current_price > prev_price:
            return pick_varied("price_up", [
                "That price just went up.",
                "You are raising the price?",
                "That is not how this works.",
                "If anything, the price should go down."
            ])

        if current_price < prev_price:
            return pick_varied("price_down", [
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
        return pick_varied(f"open_lines:{item_name}", open_lines)

    if turns > 5 and seller_price == price:
        return "We are not reaching an agreement."

    if seller_price is not None and market_price is not None:
        if seller_price > market_price * 1.8:
            return pick_varied("too_high", [
                "That price is far too high.",
                "That is unreasonable.",
                "I cannot pay anything close to that."
            ])

        if seller_price < market_price * 0.3:
            return pick_varied("too_low", [
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
            prefix = pick_varied("gap_large", [
                "That is far from my expectation. ",
                "We are quite far apart. ",
            ])
        elif max(3, seller_price * 0.1) < gap <= max(10, seller_price * 0.3):
            prefix = pick_varied("gap_medium", [
                "We are getting closer. ",
                "That is better. ",
            ])
        elif gap <= max(3, seller_price * 0.1):
            prefix = pick_varied("gap_small", [
                "We are very close. ",
                "This is a small difference. ",
            ])

    text = "Speak clearly."

    if action == "ASK_ITEM":
        text = pick_varied("ask_item", [
            "Do you have this item?",
            "Are you selling this?",
            "Can I buy this here?"
        ])

    elif action == "OFFER":
        if price is None:
            text = f"What price do you want for this {item_name}?"
        elif personality == "Aggressive Trader":
            text = pick_varied("offer:aggressive", [
                f"{price}. Take it or leave it.",
                f"I will not go beyond {price}.",
                f"{price}. Final."
            ])
        elif personality == "Cautious Buyer":
            text = pick_varied("offer:cautious", [
                f"I can offer {price}… but I am unsure.",
                f"Maybe {price} is fair?",
                f"I can go up to {price}, I think."
            ])
        else:
            text = pick_varied("offer:polite", [
                f"I can offer {price} for this {item_name}.",
                f"Perhaps {price} would be reasonable.",
                f"I would be comfortable at {price}."
            ])

    elif action == "REJECT":
        if personality == "Aggressive Trader":
            text = pick_varied("reject:aggressive", [
                "That is unacceptable.",
                "Do not waste my time.",
                "That price is ridiculous."
            ])
        elif personality == "Cautious Buyer":
            text = pick_varied("reject:cautious", [
                "That seems a bit high…",
                "I am not comfortable with that.",
                "That feels too much for me."
            ])
        else:
            text = pick_varied("reject:polite", [
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
            text = "A fair bargain. I will remember your honesty, trader."

    elif action == "WALK_AWAY":
        text = "Perhaps another day we shall agree."

    elif action == "NO_ITEM":
        text = "Perhaps another day we shall agree."
        
    elif action == "OUT_OF_WORLD":
        if out_count == 1:
            return "Let us stay focused on the trade."
        if out_count == 2:
            return "This is not the place for that. Talk business."
        if out_count >= 3:
            return "I am done here."

    elif action == "ASK_PRICE":
        if has_price:
            return pick_varied("ask_price_noted", [
                "That price is noted.",
                "Alright, I understand your price.",
                "Let us work with that price."
            ])

        return f"What price do you want for this {item_name}?"

    elif action == "SET_QUANTITY":
        text = f"Alright, {item_name}. Now tell me your price."

    if prefix and action in ["OFFER", "REJECT", "COUNTER"]:
        text = f"{prefix}{text}"

    seller_words = ["give", "i have", "available", "i sell"]

    if any(word in text.lower() for word in seller_words):
        return "Let us discuss the price."

    return text


def _select_tone(action, frustration):
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


def _select_emotion(action, frustration):
    if action == "ACCEPT":
        return "happy"

    if frustration > 0.7:
        return "frustrated"

    if action == "OFFER":
        return "thinking"

    if action == "REJECT":
        return "serious"

    return "idle"

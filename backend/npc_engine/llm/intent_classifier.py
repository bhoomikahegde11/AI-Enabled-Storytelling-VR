import re
from llama_cpp import Llama
import os
from npc_engine.engine.input_interpreter import extract_price
BASE_DIR = os.path.dirname(os.path.dirname(__file__))
MODEL_PATH = os.path.join(BASE_DIR, "models", "model.gguf")

llm = Llama(
    model_path=MODEL_PATH,
    n_ctx=2048,
    n_threads=6,
    verbose=False
)

OUT_OF_WORLD_TERMS = [
    "phone", "internet", "computer", "app", "mobile", "online", "wifi", "robot",
    "screen", "tablet", "google", "youtube", "instagram", "facebook", "twitter",
    "tiktok", "netflix", "spotify", "xbox", "playstation", "barbie", "hot wheels",
    "lego", "pokemon", "disney", "marvel", "avengers", "gta", "fortnite", "doomsday",
    "nike", "adidas", "apple", "samsung",
    "sony", "coca cola", "pepsi"
]

MODERN_KEYWORDS = [
    "phone", "mobile", "laptop", "computer",
    "fortnite", "call of duty", "cod", "csgo",
    "internet", "wifi", "youtube", "google",
    "app", "instagram", "whatsapp",
    "playstation", "xbox", "game", "gaming"
]

OUT_OF_WORLD_GROUPS = [
    ["video", "game"], ["social", "media"], ["mobile", "phone"], ["cell", "phone"],
    ["smart", "phone"], ["brand", "toy"], ["toy", "car"], ["toy", "doll"],
    ["plastic", "toy"], ["computer", "game"], ["internet", "site"], ["online", "store"],
    ["digital", "device"]
]


def contains_out_of_world_concept(text: str):
    return any(term in text for term in OUT_OF_WORLD_TERMS) or any(term in text for term in MODERN_KEYWORDS) or any(
        all(part in text for part in group) for group in OUT_OF_WORLD_GROUPS
    )


def has_modern_action_pattern(text: str, trade_terms):
    modern_action_words = ["play", "game", "download", "install"]
    if not any(word in text for word in modern_action_words):
        return False
    return not any(term in text for term in trade_terms)


def is_hostile_input(text: str, user_input: str):
    hostile_phrases = [
        "fuck off",
        "fuck",
        "bitch",
        "retarded",
        "idiot",
        "broken",
        "kill yourself",
        "go kill yourself",
        "go die",
        "die",
        "nonsense",
        "dumb",
        "stupid",
        "shut up",
        "get out",
        "leave"
    ]
    hostile_groups = [
        ["kill", "yourself"],
        ["go", "die"],
        ["get", "out"],
        ["shut", "up"]
    ]
    insult_terms = [
        "bitch", "idiot", "retarded", "dumb", "stupid", "broken", "nonsense", "fuck"
    ]

    if any(phrase in text for phrase in hostile_phrases) or any(
        all(part in text for part in group) for group in hostile_groups
    ):
        return True

    word_count = len(text.split())
    if word_count <= 4 and any(term in text for term in insult_terms):
        return True

    hostility_prompt = f"""
Does this sentence contain insult, aggression, or abusive language?
Answer YES or NO.

Sentence: "{user_input}"
"""

    try:
        hostility_output = llm(hostility_prompt, max_tokens=3)["choices"][0]["text"].strip().upper()
        return hostility_output == "YES"
    except:
        return False


def has_price_statement_pattern(text: str):
    price_patterns = [
        r"\btake it\s+\d+\b",
        r"\bi(?:\s+will|['’]?ll)?\s+give\s+(?:it|you|this|the\s+\w+)?\s*(?:for\s+)?\d+\b",
        r"\b\d+\s+is\s+(?:fine|good|okay|ok|fair)\b",
        r"\bfor\s+\d+\b",
        r"\bprice\s+is\s+\d+\b",
        r"\bsell(?:ing)?\s+(?:it|this|the\s+\w+)?\s*(?:for\s+)?\d+\b"
    ]
    return any(re.search(pattern, text) for pattern in price_patterns)


def parse_quantity(text: str):
    text = str(text).lower().strip()
    match = re.search(r"\b(\d+(?:\.\d+)?)\s*(g|gm|gram|grams|kg|kgs|kilogram|kilograms)\b", text)
    if not match:
        return None

    quantity_value = float(match.group(1))
    raw_unit = match.group(2)
    unit_map = {
        "g": "g",
        "gm": "g",
        "gram": "g",
        "grams": "g",
        "kg": "kg",
        "kgs": "kg",
        "kilogram": "kg",
        "kilograms": "kg"
    }
    normalized_unit = unit_map[raw_unit]
    quantity_grams = quantity_value * 1000.0 if normalized_unit == "kg" else quantity_value

    if quantity_value.is_integer():
        quantity_value = int(quantity_value)
    if quantity_grams.is_integer():
        quantity_grams = int(quantity_grams)

    return {
        "quantity": quantity_value,
        "unit": normalized_unit,
        "quantity_grams": quantity_grams
    }


def extract_quantity_price_offer(user_input: str):
    text = str(user_input).lower().strip()
    patterns = [
        r"\b(\d+(?:\.\d+)?)\s*(g|gm|gram|grams|kg|kgs|kilogram|kilograms)\s+for\s+(\d+)\b",
        r"\bfor\s+(\d+)\s*,?\s*(\d+(?:\.\d+)?)\s*(g|gm|gram|grams|kg|kgs|kilogram|kilograms)\b",
        r"\b(\d+(?:\.\d+)?)\s*(g|gm|gram|grams|kg|kgs|kilogram|kilograms)\s+(?:is|for)\s+(\d+)\b"
    ]

    for pattern in patterns:
        match = re.search(pattern, text)
        if not match:
            continue

        groups = match.groups()
        if pattern.startswith(r"\bfor"):
            price_raw, quantity_raw, raw_unit = groups
        else:
            quantity_raw, raw_unit, price_raw = groups

        quantity_info = parse_quantity(f"{quantity_raw}{raw_unit}")
        if quantity_info is None:
            continue

        return {
            "price": int(price_raw),
            "quantity": quantity_info["quantity"],
            "unit": quantity_info["unit"],
            "quantity_grams": quantity_info["quantity_grams"]
        }

    return None


def extract_quantity_info(user_input: str):
    return parse_quantity(user_input)


def extract_bundle_items(user_input: str, known_items=None):
    text = str(user_input).lower().strip()
    known_items = [item.lower() for item in (known_items or [])]
    if not known_items:
        known_items = ["pepper", "clove", "cinnamon", "cardamom"]

    item_pattern = "|".join(re.escape(item) for item in sorted(known_items, key=len, reverse=True))
    pattern = rf"\b(\d+(?:\.\d+)?)\s*(g|gm|gram|grams|kg|kgs|kilogram|kilograms)\s+({item_pattern})\b"

    bundle_items = []
    for quantity_raw, raw_unit, item_name in re.findall(pattern, text):
        quantity_info = parse_quantity(f"{quantity_raw}{raw_unit}")
        if quantity_info is None:
            continue
        bundle_items.append({
            "name": item_name,
            "quantity": quantity_info["quantity"],
            "unit": quantity_info["unit"],
            "quantity_grams": quantity_info["quantity_grams"]
        })

    return bundle_items


def classify_trade_vs_world(user_input: str, item_name: str, current_offer, last_buyer_offer, last_seller_price, last_system_action, last_intent):
    prompt = f"""
You are in a 1500s spice market.
Classify the following input as:
A) trade-related
B) normal conversation
C) out-of-world / modern concept

Negotiation context:
- Item: {item_name}
- Buyer's current offer: {current_offer if current_offer is not None else "unknown"} varahas
- Buyer's last offer: {last_buyer_offer if last_buyer_offer is not None else "unknown"} varahas
- Seller's last stated price: {last_seller_price if last_seller_price is not None else "none"}
- Last system action: {last_system_action if last_system_action is not None else "none"}
- Last detected seller intent: {last_intent if last_intent is not None else "none"}
- Seller input: "{user_input}"

Return ONLY A, B, or C.
"""
    return llm(prompt, max_tokens=3)["choices"][0]["text"].strip().upper()


def apply_intent_corrections(text: str, candidate_intent: str, context=None):
    context = context or {}
    last_system_action = context.get("last_system_action")
    in_negotiation = context.get("in_negotiation", False)
    has_active_offer = last_system_action == "OFFER"
    negative_words = ["low", "high", "not", "no", "increase", "decrease", "more", "less"]

    agreement_markers = [
        "perfect", "done", "fine", "okay", "ok", "agreed", "sure",
        "that works", "lets do it", "let's do it", "call it a deal"
    ]
    item_transfer_markers = [
        "it's yours",
        "its yours",
        "you can have it"
    ]
    short_positive_responses = ["sure", "okay", "ok", "fine", "alright", "perfect"]

    if "give" in text and any(char.isdigit() for char in text):
        return None

    if any(char.isdigit() for char in text):
        return None

    if any(word in text for word in negative_words):
        return None

    if has_active_offer and (
        any(marker in text for marker in agreement_markers) or
        any(marker in text for marker in item_transfer_markers)
    ):
        return {"intent": "ACCEPT", "tone": "neutral", "persuasion": 1}

    if has_active_offer and text in short_positive_responses:
        return {"intent": "ACCEPT", "tone": "neutral", "persuasion": 1}

    if candidate_intent == "OUT_OF_WORLD" and not contains_out_of_world_concept(text):
        if has_active_offer and (
            any(marker in text for marker in agreement_markers) or
            any(marker in text for marker in item_transfer_markers)
        ):
            return {"intent": "ACCEPT", "tone": "neutral", "persuasion": 1}
        if text in short_positive_responses or "business" in text or "trade" in text:
            return {"intent": "CONTINUE", "tone": "neutral", "persuasion": 0}

    return None


def has_accept_blockers(text: str):
    insult_markers = [
        "retarded", "idiot", "dumb", "stupid", "broken", "shut up"
    ]
    question_markers = [
        "?", "what", "why", "how", "who", "when"
    ]
    return any(marker in text for marker in insult_markers) or any(marker in text for marker in question_markers)


def detect_social_sub_intent(text: str):
    if any(phrase in text for phrase in ["hello", "hi", "hey", "greetings", "how are you", "good morning", "good evening"]):
        return "GREETING"
    if "weather" in text or "rain" in text or "sun" in text or "wind" in text:
        return "WEATHER"
    if any(phrase in text for phrase in ["what did you do today", "how was your day", "how goes the day", "what have you done today", "how is your day"]):
        return "DAILY_LIFE"
    if any(phrase in text for phrase in ["nice day", "how goes it", "how is the day", "all well"]):
        return "GENERAL"
    if text in ["what", "what?", "huh", "huh?"] or "what do you mean" in text or "i do not understand" in text:
        return "CONFUSION"
    return None


def fallback_context_classification(text: str, item_name: str):
    explicit_continue_phrases = [
        "sure",
        "okay",
        "ok",
        "fine",
        "alright",
        "yes",
        "go ahead",
        "yes we do",
        "yes you can"
    ]

    if text in explicit_continue_phrases:
        return {"intent": "CONTINUE", "tone": "neutral", "persuasion": 0}

    social_sub_intent = detect_social_sub_intent(text)
    if social_sub_intent is not None:
        return {
            "intent": "SOCIAL",
            "tone": "neutral",
            "persuasion": 0,
            "social_sub_intent": social_sub_intent
        }

    trade_terms = [
        item_name,
        "price",
        "offer",
        "trade",
        "deal",
        "sell",
        "buy",
        "goods",
        "item",
        "pepper",
        "clove",
        "cinnamon",
        "cardamom",
        "spice",
        "varahas",
        "market",
        "stall"
    ]

    if contains_out_of_world_concept(text):
        return {"intent": "OUT_OF_WORLD", "tone": "confused", "persuasion": 0}

    if text and not any(term in text for term in trade_terms):
        social_sub_intent = detect_social_sub_intent(text)
        if social_sub_intent is not None:
            return {
                "intent": "SOCIAL",
                "tone": "neutral",
                "persuasion": 0,
                "social_sub_intent": social_sub_intent
            }

    return {"intent": "IRRELEVANT", "tone": "neutral", "persuasion": 0}


def is_agreement(text, context):
    if context.get("last_system_action") != "OFFER":
        return False

    has_number = any(char.isdigit() for char in text)
    has_question = "?" in text
    has_quantity = any(unit in text for unit in ["kg", "g", "gram"])
    negative_words = ["low", "high", "not", "no", "more", "less"]

    if has_number or has_question or has_quantity:
        return False

    if any(word in text for word in negative_words):
        return False

    if len(text.split()) <= 4:
        return True

    return False


def is_price_statement(text):
    return any(char.isdigit() for char in text)


def is_rejection(text):
    negative_words = ["no", "not", "too low", "too high", "reject", "leave"]
    return any(word in text for word in negative_words)


def classify_intent(user_input: str, context=None):
    context = context or {}
    text = user_input.lower().strip()

    item_name = context.get("item_name", "item")
    current_offer = context.get("current_offer")
    last_buyer_offer = context.get("last_buyer_offer")
    last_seller_price = context.get("last_seller_price")
    last_system_action = context.get("last_system_action")
    last_intent = context.get("last_intent")
    in_negotiation = context.get("in_negotiation", False)
    known_items = context.get("known_items", [item_name])
    bundle_items = extract_bundle_items(user_input, known_items)
    quantity_info = extract_quantity_info(user_input)
    quantity_price_offer = extract_quantity_price_offer(user_input)

    item_mentions = [item_name.lower()] + [item.lower() for item in known_items if item.lower() != item_name.lower()]

    # -------------------------------
    # 🔥 PRIORITY LAYER: OUT OF WORLD
    # -------------------------------
    trade_terms_for_oow = [
        item_name.lower(),
        "price", "offer", "trade", "deal", "sell", "buy", "goods", "item",
        "shop", "market", "varahas", "stall"
    ] + [item.lower() for item in known_items]

    if contains_out_of_world_concept(text) or has_modern_action_pattern(text, trade_terms_for_oow):
        return {"intent": "OUT_OF_WORLD", "tone": "confused", "persuasion": 0}

    # -------------------------------
    # 🔥 PRIORITY LAYER: STRICT NO_ITEM
    # -------------------------------
    no_item_strict_phrases = [
        "we are out", "we are out of", "out of stock", "no stock",
        "not available", "we don't have", "we do not have", "we dont have",
        "its over", "it's over", "finished", "sold out", "nothing left",
        "no we do not", "got over", "it is over"
    ]
    if any(phrase in text for phrase in no_item_strict_phrases):
        return {"intent": "NO_ITEM", "tone": "neutral", "persuasion": 0}
        
    no_item_patterns = [
        r"\b(?:we\s+)?do\s+not\s+have\b",
        r"\b(?:we\s+)?don't\s+have\b",
        r"\b(?:we\s+)?dont\s+have\b",
        r"\bno\s+.*(?:have|stock|sell)\b",
        r"\b(?:dont|don't)\s+.*have\b",
        r"\b(?:dont|don't)\s+.*sell\b",
        r"\bnot\s+selling\b"
    ]
    if any(re.search(pattern, text) for pattern in no_item_patterns):
        return {"intent": "NO_ITEM", "tone": "neutral", "persuasion": 0}

    if last_system_action == "ASK_ITEM" and text in ["no", "nope", "nah"]:
        return {"intent": "NO_ITEM", "tone": "neutral", "persuasion": 0}

    if re.search(r"\bno\b", text) and any(item in text for item in item_mentions):
        return {"intent": "NO_ITEM", "tone": "neutral", "persuasion": 0}

    # -------------------------------
    # 🔥 EARLY HYBRID LAYER
    # -------------------------------
    if is_agreement(text, context):
        return {"intent": "ACCEPT", "tone": "neutral", "persuasion": 1}

    if quantity_price_offer is not None:
        return {"intent": "QUANTITY_PRICE", "tone": "neutral", "persuasion": 1}

    if bundle_items:
        if len(bundle_items) > 1:
            return {"intent": "BUNDLE_OFFER", "tone": "neutral", "persuasion": 1}
        if bundle_items[0]["name"] != item_name.lower():
            return {"intent": "BUNDLE_OFFER", "tone": "neutral", "persuasion": 1}
        return {"intent": "QUANTITY_CHANGE", "tone": "neutral", "persuasion": 1}

    if quantity_info is not None and any(word in text for word in ["take", "give", "want", "need", "for", item_name.lower()]):
        return {"intent": "QUANTITY_CHANGE", "tone": "neutral", "persuasion": 1}

    if quantity_info is not None and any(word in text for word in ["only", "left", "remaining"]):
        return {"intent": "QUANTITY_CHANGE", "tone": "neutral", "persuasion": 1}

    if is_price_statement(text):
        return {"intent": "PRICE", "tone": "neutral", "persuasion": 1}

    if is_rejection(text):
        return {"intent": "REJECT", "tone": "neutral", "persuasion": 0}

    availability_phrases = [
        "yes we have",
        "yes we do",
        "we have",
        "available"
    ]
    if any(phrase in text for phrase in availability_phrases) and any(item in text for item in item_mentions):
        return {"intent": "CONTINUE", "tone": "neutral", "persuasion": 0}

    if text in ["no", "nope", "nah"]:
        if last_system_action == "ASK_ITEM":
            return {"intent": "NO_ITEM", "tone": "neutral", "persuasion": 0}
        if last_system_action == "OFFER":
            return {"intent": "REJECT", "tone": "neutral", "persuasion": 0}
        if last_seller_price is not None:
            return {"intent": "COUNTER", "tone": "neutral", "persuasion": 1}
        return {
            "intent": "SOCIAL",
            "tone": "neutral",
            "persuasion": 0,
            "social_sub_intent": "CONFUSION"
        }

    if text == "yes":
        if last_system_action == "OFFER":
            return {"intent": "ACCEPT", "tone": "neutral", "persuasion": 1}
        return {"intent": "CONTINUE", "tone": "neutral", "persuasion": 0}

    if is_hostile_input(text, user_input):
        return {"intent": "HOSTILE", "tone": "annoyed", "persuasion": 0}

    query_phrases = [
        "what do you want",
        "how much",
        "for how much",
        "what price",
        "what are you offering",
        "what is your offer",
        "what do you offer",
        "how many",
        "for how many grams",
        "for how many gram",
        "for how many g",
        "for how many kg",
        "how many grams",
        "how many gram",
        "how many g",
        "how many kg",
        "what quantity",
        "what amount"
    ]

    if any(phrase in text for phrase in query_phrases):
        return {"intent": "QUERY", "tone": "neutral", "persuasion": 0}

    ultimatum_phrases = [
        "take it or leave it",
        "final price",
        "this is my final price"
    ]
    ultimatum_patterns = [
        r"not going lower than\s+\d+",
        r"nothing less than\s+\d+",
        r"not less than\s+\d+",
        r"final price\s*(?:is)?\s*\d+",
        r"minimum(?: price)?\s*(?:is)?\s*\d+"
    ]

    if any(phrase in text for phrase in ultimatum_phrases) or any(re.search(pattern, text) for pattern in ultimatum_patterns):
        return {"intent": "ULTIMATUM", "tone": "annoyed", "persuasion": 0}

    explicit_accept_phrases = [
        "deal",
        "done",
        "done deal",
        "ok deal",
        "okay deal",
        "yes deal",
        "fine deal",
        "confirm",
        "confirmed",
        "take it",
        "ok take it",
        "yes take it",
        "fine take it"
    ]

    if text in explicit_accept_phrases and not has_accept_blockers(text):
        if any(char.isdigit() for char in text):
            return None
        negative_words = ["low", "high", "not", "no", "increase", "decrease", "more", "less"]
        if any(word in text for word in negative_words):
            return None
        return {"intent": "ACCEPT", "tone": "neutral", "persuasion": 1}

    if text.strip() in ["done", "deal"]:
        return {"intent": "ACCEPT", "tone": "neutral", "persuasion": 1}

    accept_phrases = [
        "deal",
        "done",
        "done deal",
        "ok deal",
        "okay deal",
        "yes deal",
        "fine deal",
        "agreed",
        "agree",
        "accept",
        "take it",
        "fine take it",
        "ok take it",
        "yes take it",
        "okay take it",
        "ok fine",
        "fine",
        "ok done",
        "confirmed",
        "it's yours",
        "its yours",
        "yours",
        "you can have it",
        "yes ill give it to you",
        "yes i will give it to you",
        "yes here it is",
        "ok lets confirm it",
        "okay lets confirm it",
        "let's confirm it",
        "lets confirm it"
    ]

    if last_system_action == "OFFER":
        has_number = any(char.isdigit() for char in text)
        negative_words = ["low", "high", "not", "no", "more", "less"]

        if has_number:
            return None

        if any(word in text for word in negative_words):
            return None

        if len(text.split()) <= 4:
            return {"intent": "ACCEPT", "tone": "neutral", "persuasion": 1}

    offer_accept_phrases = [
        "sure",
        "okay",
        "ok",
        "fine",
        "alright",
        "perfect",
        "that works",
        "sounds good",
        "ok done",
        "take it",
        "fine take it",
        "ok take it",
        "okay take it",
        "ok fine",
        "done",
        "its yours",
        "yours"
    ]

    if last_system_action == "OFFER" and text in offer_accept_phrases and not has_accept_blockers(text):
        if any(char.isdigit() for char in text):
            return None
        negative_words = ["low", "high", "not", "no", "increase", "decrease", "more", "less"]
        if any(word in text for word in negative_words):
            return None
        return {"intent": "ACCEPT", "tone": "neutral", "persuasion": 1}

    if last_system_action == "OFFER" and any(phrase in text for phrase in accept_phrases) and not has_accept_blockers(text) and not any(word in text for word in ["give", "for", "price"]):
        if any(char.isdigit() for char in text):
            return None
        negative_words = ["low", "high", "not", "no", "increase", "decrease", "more", "less"]
        if any(word in text for word in negative_words):
            return None
        return {"intent": "ACCEPT", "tone": "neutral", "persuasion": 1}

    if last_system_action == "OFFER" and not has_accept_blockers(text) and len(text.split()) > 2:
        agreement_prompt = f"""
Is this clearly agreeing to a deal?
Answer YES or NO.

Sentence: "{user_input}"
"""

        try:
            agreement_output = llm(agreement_prompt, max_tokens=3)["choices"][0]["text"].strip().upper()
            if agreement_output == "YES":
                return {"intent": "ACCEPT", "tone": "neutral", "persuasion": 1}
        except:
            pass

    reject_phrases = [
        "get out",
        "leave",
        "no deal",
        "not interested"
    ]

    if any(phrase in text for phrase in reject_phrases):
        return {"intent": "REJECT", "tone": "neutral", "persuasion": 0}

    counter_phrases = [
        "too low",
        "too less",
        "not enough",
        "increase",
        "more",
        "higher",
        "less",
        "little more",
        "still less",
        "middle",
        "meet in the middle",
        "split"
    ]

    if any(phrase in text for phrase in counter_phrases):
        return {"intent": "COUNTER", "tone": "neutral", "persuasion": 1}

    counter_groups = [
        ["not", "price"],
        ["take", "price"],
        ["low", "price"],
        ["raise", "price"],
        ["make", "higher"],
        ["make", "more"],
        ["go", "higher"],
        ["give", "more"],
        ["offer", "more"]
    ]

    if any(all(part in text for part in group) for group in counter_groups):
        return {"intent": "COUNTER", "tone": "neutral", "persuasion": 1}

    if ("low" in text or "higher" in text or "more" in text) and any(word in text for word in ["that", "it", "offer", "price"]):
        return {"intent": "COUNTER", "tone": "neutral", "persuasion": 1}

    affirm_pattern = r"\b(?:yes|ok|okay|sure|fine|that works|deal)\b"
    if re.search(affirm_pattern, text):
        return {"intent": "AFFIRM", "tone": "neutral", "persuasion": 0}

    continue_phrases = [
        "sure",
        "yes you can",
        "yes we do",
        "go ahead",
        "okay",
        "ok",
        "yes",
        "fine",
        "alright"
    ]

    if text in continue_phrases:
        return {"intent": "CONTINUE", "tone": "neutral", "persuasion": 0}

    # -------------------------------
    # 🔥 LAYER 3: LLM CONTEXT CLASSIFICATION
    # -------------------------------
    try:
        output = classify_trade_vs_world(
            user_input,
            item_name,
            current_offer,
            last_buyer_offer,
            last_seller_price,
            last_system_action,
            last_intent
        )

        if output == "A":
            result = {"intent": "QUERY", "tone": "neutral", "persuasion": 0}
            corrected = apply_intent_corrections(text, result["intent"], context)
            return corrected or result
        if output == "B":
            result = {
                "intent": "SOCIAL",
                "tone": "neutral",
                "persuasion": 0,
                "social_sub_intent": detect_social_sub_intent(text) or "GENERAL"
            }
            corrected = apply_intent_corrections(text, result["intent"], context)
            return corrected or result
        if output == "C":
            result = {"intent": "OUT_OF_WORLD", "tone": "confused", "persuasion": 0}
            corrected = apply_intent_corrections(text, result["intent"], context)
            return corrected or result

        result = fallback_context_classification(text, item_name)
        final_intent = corrected or result
        
        if final_intent["intent"] == "IRRELEVANT":
            abuse_prompt = f"""
Does this sentence contain sexual references, insults, meaningless disruptive phrases, or strong inappropriate emotion not related to trade?
Answer YES or NO.

Sentence: "{user_input}"
"""
            try:
                abuse_output = llm(abuse_prompt, max_tokens=3)["choices"][0]["text"].strip().upper()
                if "YES" in abuse_output:
                    return {"intent": "HOSTILE", "tone": "annoyed", "persuasion": 0}
            except:
                pass
                
        return final_intent

    except:
        result = fallback_context_classification(text, item_name)
        final_intent = apply_intent_corrections(text, result["intent"], context) or result
        
        if final_intent["intent"] == "IRRELEVANT":
            abuse_prompt = f"""
Does this sentence contain sexual references, insults, meaningless disruptive phrases, or strong inappropriate emotion not related to trade?
Answer YES or NO.

Sentence: "{user_input}"
"""
            try:
                abuse_output = llm(abuse_prompt, max_tokens=3)["choices"][0]["text"].strip().upper()
                if "YES" in abuse_output:
                    return {"intent": "HOSTILE", "tone": "annoyed", "persuasion": 0}
            except:
                pass
                
        return final_intent

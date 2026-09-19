import re
from rapidfuzz import fuzz

# Templates for fuzzy matching
QUERY_TEMPLATES = [
    "how much will you pay",
    "how much do you want it for",
    "name your price",
    "what can you offer",
    "your best price",
    "what price are you willing to pay",
    "how much are you going to give",
    "how much are you going to pay"
]

PRICE_TEMPLATES = [
    "how does {price} sound",
    "what about {price}",
    "deal at {price}",
    "settle at {price}",
    "take it for {price}",
    "so let's deal at {price}",
    "let's deal at {price}",
    "let's settle at {price}"
]

ACCEPT_TEMPLATES = [
    "deal",
    "done",
    "fine",
    "agreed",
    "ok",
    "okay",
    "yes",
    "sure",
    "yup",
    "okay deal",
    "that's a deal",
    "accepted",
    "sounds good"
]

REJECT_TEMPLATES = [
    "too much",
    "too low",
    "not enough"
]

# Short acceptance phrases/words that require specific negotiation state
SHORT_ACCEPT_WORDS = {"fine", "ok", "okay", "yes", "sure", "yup", "deal", "done", "agreed"}

def word_to_digit(word):
    """
    Locally translate a word representation of a number to its digit.
    """
    from npc_engine.utils.text_normalizer import units, tens
    w_lower = word.lower()
    if w_lower in units:
        return units[w_lower]
    if w_lower in tens:
        return tens[w_lower]
    return None

def preprocess_intent(user_input: str, context=None):
    context = context or {}
    text = str(user_input).lower().strip()
    
    # Strip basic trailing/leading punctuation
    text_clean = re.sub(r'[?.!,;:]', '', text).strip()
    if not text_clean:
        return {"confidence": "LOW"}

    # Direct budget query routing bypass
    budget_explicit_phrases = [
        "how much will you pay",
        "how much are you willing to pay",
        "what will you give",
        "what can you offer",
        "your offer",
        "your price",
        "name your price"
    ]
    if any(phrase in text_clean for phrase in budget_explicit_phrases):
        return {
            "intent": "QUERY_BUYER_BUDGET",
            "confidence": "HIGH"
        }
        
    from npc_engine.levels.level1_market.input_interpreter import extract_price
    price = extract_price(text_clean)
    
    # Prepare text with placeholder for price matching
    text_placeholder = text_clean
    if price is not None:
        text_placeholder = re.sub(r'\b\d+\b', '{price}', text_clean)
        spoken_numbers = ["seventy", "eighty", "ninety", "thirty", "forty", "fifty", "sixty", "twenty", "ten", "sixty seven"]
        for sn in spoken_numbers:
            text_placeholder = text_placeholder.replace(sn, "{price}")
            
    last_system_action = context.get("last_system_action")
    in_negotiation = context.get("in_negotiation", False)
    
    # 1. State-aware check for raw numbers (e.g. "36")
    # "After NPC asks price: '36' => PRICE"
    # A raw number should be PRICE if we are in active negotiation or start (last_system_action is GREETING, ASK_PRICE, etc.)
    # Let's check if the clean text consists ONLY of a number and optional currency words
    words = text_clean.split()
    is_pure_number = False
    if price is not None and len(words) <= 3:
        price_stop_words = {"varahas", "varaha", "coins", "coin", "gold", "silver", "only", "it", "is", "for", "tara"}
        non_price_words = [w for w in words if not w.isdigit() and w not in price_stop_words]
        clean_non_price = [w for w in non_price_words if word_to_digit(w) is None]
        if len(clean_non_price) == 0:
            is_pure_number = True
            
    if is_pure_number:
        # Resolve state-aware: Raw number is HIGH confidence PRICE if last system action asks price or is greeting
        # If last system action is GREETING, ASK_PRICE, or OFFER/COUNTER (counter proposal)
        if last_system_action in [None, "GREETING", "ASK_PRICE", "OFFER", "COUNTER", "ASK_ITEM"]:
            return {
                "intent": "PRICE",
                "price": float(price),
                "confidence": "HIGH"
            }
            
    # 2. Check QUERY_BUYER_BUDGET
    # QUERY: requires >= 80 score
    for template in QUERY_TEMPLATES:
        score = fuzz.ratio(text_clean, template)
        if score >= 80:
            return {
                "intent": "QUERY_BUYER_BUDGET",
                "confidence": "HIGH"
            }
            
    # 3. Check PRICE (requires price + >= 80 score)
    if price is not None:
        for template in PRICE_TEMPLATES:
            score = fuzz.ratio(text_placeholder, template)
            if score >= 80:
                return {
                    "intent": "PRICE",
                    "price": float(price),
                    "confidence": "HIGH"
                }
                
    # 4. Check ACCEPT (requires >= 95 score and valid conversation state)
    # Valid Accept State: must be after OFFER, COUNTER, FINAL_OFFER, ASK_CONFIRMATION
    # Never after: GREETING, ASK_PRICE
    valid_accept_state = last_system_action in ["OFFER", "COUNTER", "FINAL_OFFER", "ASK_CONFIRMATION"]
    
    for template in ACCEPT_TEMPLATES:
        score = fuzz.ratio(text_clean, template)
        if score >= 95:
            if valid_accept_state:
                return {
                    "intent": "ACCEPT",
                    "confidence": "HIGH"
                }
                
    # 5. Check REJECT (requires >= 95 score and valid conversation state)
    # Valid Reject State: generally in negotiation (last_system_action is not None and not GREETING)
    valid_reject_state = last_system_action not in [None, "GREETING"]
    
    for template in REJECT_TEMPLATES:
        score = fuzz.ratio(text_clean, template)
        if score >= 95:
            if valid_reject_state:
                return {
                    "intent": "REJECT",
                    "confidence": "HIGH"
                }
                
    return {"confidence": "LOW"}

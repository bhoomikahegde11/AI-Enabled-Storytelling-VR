def interpret_input(text):
    text = str(text).lower()

    signals = {
        "confidence": 0,
        "urgency": 0,
        "firmness": 0
    }

    if any(w in text for w in ["best", "premium", "high quality"]):
        signals["confidence"] += 0.2

    if any(w in text for w in ["sold", "demand", "others bought"]):
        signals["confidence"] += 0.2

    if any(w in text for w in ["final", "fixed", "not less"]):
        signals["firmness"] += 0.3

    if any(w in text for w in ["fast", "quick", "today"]):
        signals["urgency"] += 0.2

    return signals

import re

def extract_price(text):
    text = str(text).lower()
    
    numbers = []
    for match in re.finditer(r"\b(\d+)\b", text):
        number = int(match.group(1))
        remainder = text[match.end():]
        if re.match(r"\s*(kg|kilogram|kilograms|g|gram|grams)\b", remainder):
            continue
        numbers.append(number)
        
    if not numbers:
        return None
        
    valid_indicators = ["for", "price", "offer", "give", "take", "sell", "cost"]
    invalid_indicators = ["difference", "more", "less", "increase", "decrease", "only"]
    
    has_valid = any(word in text for word in valid_indicators)
    has_invalid = any(word in text for word in invalid_indicators)
    
    if has_valid and not has_invalid:
        return numbers[-1]
    
    if has_invalid:
        # Ignore number
        return None
        
    explicit_price_patterns = [
        r"(?:price\s*(?:is)?|offer\s*(?:is)?|sell\s+(?:it|this|them)?\s*for|for|at)\s*(\d+)\b(?!\s*(?:kg|kilogram|kilograms|g|gram|grams))",
        r"\b(\d+)\s*varahas?\b",
        r"\b(\d+)\s+is\s+(?:fine|good|okay|ok|fair)\b(?!\s*(?:kg|kilogram|kilograms|g|gram|grams))"
    ]

    for pattern in explicit_price_patterns:
        matches = re.findall(pattern, text)
        if matches:
            return int(matches[-1])
            
    difference_phrases = [
        "difference",
        "isnt doing anything",
        "isn't doing anything",
        "close enough",
        "almost there",
        "just"
    ]

    if any(phrase in text for phrase in difference_phrases):
        return None

    return numbers[-1]
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
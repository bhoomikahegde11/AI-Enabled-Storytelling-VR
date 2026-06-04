import sys
import os

# Add backend directory to sys.path so we can import from npc_engine
sys.path.append(os.path.join(os.path.dirname(__file__), "../backend"))

from npc_engine.levels.level1_market.intent_classifier import classify_intent, is_price_statement
from npc_engine.levels.level1_market.dialogue_generator import generate_dialogue
from npc_engine.core.models import EngineDecision

def test_intent_classification_safety():
    print("============================================================")
    print(" TESTING PRICE INTENT CLASSIFICATION SAFETY ")
    print("============================================================")
    
    # 1. Test is_price_statement directly
    assert is_price_statement("100 varaha") == True
    assert is_price_statement("sell for 100") == True
    assert is_price_statement("my price is 100") == True
    assert is_price_statement("I want 100") == True
    assert is_price_statement("give it for 100") == True
    assert is_price_statement("100") == True  # pure digits check
    assert is_price_statement("100!") == True  # pure digits + punctuation check
    
    assert is_price_statement("I have 1 question") == False
    assert is_price_statement("one thing") == False
    assert is_price_statement("one moment") == False
    assert is_price_statement("How does one twist me while I have some?") == False
    
    print("[PASS] is_price_statement validation behaves exactly as expected.")
    
    # 2. Test classify_intent output
    context = {"item_name": "pepper"}
    
    # Low confidence should return CLARIFICATION
    res1 = classify_intent("I have 1 question", context)
    assert res1["intent"] == "CLARIFICATION", f"Expected CLARIFICATION, got {res1}"
    
    res2 = classify_intent("How does 1 twist me while I have some?", context)
    assert res2["intent"] == "CLARIFICATION", f"Expected CLARIFICATION, got {res2}"
    
    # High confidence should return PRICE
    res3 = classify_intent("100 varahas", context)
    assert res3["intent"] == "PRICE", f"Expected PRICE, got {res3}"
    
    res4 = classify_intent("I want 100", context)
    assert res4["intent"] == "PRICE", f"Expected PRICE, got {res4}"
    
    print("[PASS] classify_intent safety override behaves exactly as expected.")


def test_dialogue_accept_validation():
    print("============================================================")
    print(" TESTING ACCEPT DIALOGUE SAFETY VALIDATION ")
    print("============================================================")
    
    class FakeBuyer:
        name = "Abdul"
        origin = "Persia"
        spice_interest = "Pepper"
        wealth_class = "Wealthy"
        personality = "Polite Merchant"
        trust = 0.5
        frustration = 0.0
        interest = 0.5
        desperation = 0.5
        event_multipliers = {}
        
    class FakeEngine:
        buyer = FakeBuyer()
        item = type('Item', (object,), {'name': 'Pepper'})()
        stage = "standard"
        current_offer = 100
        current_quantity = 1000
        last_seller_price_per_kg = None
        bundle_label = ""
        seller_min_price = 80
        last_quantity_grams = 1000
        final_item = None
        event_active = False
        frustration = 0.0
        trust = 0.5
        last_seller_price = None
        out_of_world_count = 0
        active_event = None
        last_action = None
        quantity_given = True
        market_price = 100
        turns = 1
        prev_seller_price = None
        last_seller_price = 100
        
    # Test valid accept dialogues (mocking LLM loaded = False so we just check template fallback,
    # but wait, let's verify if our has_future_accept_phrase blocks rephrased texts)
    # Since we want to test the validator, we can import/test the check inside generate_dialogue.
    # When generating dialogue for ACCEPT, it defaults to: "A fair bargain. I will remember your honesty, trader."
    dec = EngineDecision(action="ACCEPT", price=100, quantity=1000, done=True)
    eng = FakeEngine()
    
    result = generate_dialogue(dec, eng)
    assert "bargain" in result["text"] or "Deal" in result["text"] or "works" in result["text"], f"Expected fallback template, got {result['text']}"
    
    # Deterministic testing of LLM rephrased validations for ACCEPT
    import npc_engine.levels.level1_market.dialogue_generator as dg
    orig_run_llm = dg.run_llm
    orig_llm_loaded = dg.llm_loaded
    
    try:
        dg.llm_loaded = True
        
        # Test allowed examples (must NOT trigger fallback, i.e., exact string is returned)
        allowed_cases = [
            "A fair bargain. I will remember your honesty.",
            "These spices are worth the price. We have a deal.",
            "I accept your offer, trader."
        ]
        for sentence in allowed_cases:
            dg.run_llm = lambda prompt, max_tokens=96: sentence
            res = dg.generate_dialogue(dec, eng)
            assert res["text"] == sentence, f"Expected allowed phrase '{sentence}' to pass, but it got modified or rejected to '{res['text']}'"
            
        # Test forbidden examples (must trigger fallback, i.e., template string is returned)
        forbidden_cases = [
            "I will return later",
            "I may purchase",
            "another time",
            "perhaps",
            "I shall return with more varahas to purchase...",
            "Perhaps I will return with more varahas to purchase next time."
        ]
        for sentence in forbidden_cases:
            dg.run_llm = lambda prompt, max_tokens=96: sentence
            res = dg.generate_dialogue(dec, eng)
            assert res["text"] != sentence, f"Expected forbidden phrase '{sentence}' to be rejected, but it was accepted."
            assert "bargain" in res["text"] or "Deal" in res["text"] or "works" in res["text"], f"Expected fallback template for '{sentence}', got '{res['text']}'"
            
        # Test spice hallucination safety
        # FakeEngine's item is "Pepper", so "cloves" or "cinnamon" are wrong spices
        dg.run_llm = lambda prompt, max_tokens=96: "I will take cloves"
        res = dg.generate_dialogue(dec, eng)
        assert res["text"] != "I will take cloves", "Expected wrong spice rephrase to be rejected"
        assert "bargain" in res["text"] or "Deal" in res["text"] or "works" in res["text"]
        
        dg.run_llm = lambda prompt, max_tokens=96: "Fine, I accept pepper"
        res = dg.generate_dialogue(dec, eng)
        assert res["text"] == "Fine, I accept pepper", "Expected correct spice rephrase to be accepted"

        print("[PASS] Allowed/Forbidden ACCEPT dialogue validations behave exactly as expected.")
    finally:
        dg.run_llm = orig_run_llm
        dg.llm_loaded = orig_llm_loaded
    
    print("[PASS] Dialogue accept validation runs smoothly.")

def test_tts_cleaner():
    print("============================================================")
    print(" TESTING TTS CLEANER ")
    print("============================================================")
    from api import clean_text_for_speech
    
    assert clean_text_for_speech("1 Veesai (~1.4 kg) cinnamon for 100 varaha") == "one veesai cinnamon for 100 varaha"
    assert clean_text_for_speech("4 palam (~113.2g) pepper") == "4 palam pepper"
    assert clean_text_for_speech("I will give 100 varahas for ~4 palams of cloves") == "I will give 100 varahas for 4 palams of cloves"
    
    print("[PASS] clean_text_for_speech cleans input exactly as required.")

if __name__ == "__main__":
    test_intent_classification_safety()
    test_dialogue_accept_validation()
    test_tts_cleaner()
    print("============================================================")
    print(" ALL SAFETY INTEGRATION TESTS PASSED SUCCESSFULLY! ")
    print("============================================================")
    sys.exit(0)

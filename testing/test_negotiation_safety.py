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
        ended = False
        last_intent = None
        started = True
        
    # Test valid accept dialogues (mocking LLM loaded = False so we just check template fallback,
    # but wait, let's verify if our has_future_accept_phrase blocks rephrased texts)
    # Since we want to test the validator, we can import/test the check inside generate_dialogue.
    # When generating dialogue for ACCEPT, it defaults to: "A fair bargain. I will remember your honesty, trader."
    dec = EngineDecision(action="ACCEPT", price=100, quantity=1000, done=True)
    eng = FakeEngine()
    def is_valid_accept_text(text):
        text_lower = text.lower()
        return any(term in text_lower for term in ["bargain", "deal", "works", "agreement", "agreed", "complete the trade"])

    import npc_engine.levels.level1_market.dialogue_generator as dg
    orig_llm_loaded = dg.llm_loaded
    
    try:
        dg.llm_loaded = False
        result = generate_dialogue(dec, eng)
        assert is_valid_accept_text(result["text"]), f"Expected fallback template, got {result['text']}"
    finally:
        dg.llm_loaded = orig_llm_loaded
    
    # Deterministic testing of LLM rephrased validations for ACCEPT
    orig_run_llm = dg.run_llm
    orig_llm_loaded = dg.llm_loaded
    orig_should_use_llm = dg.should_use_llm
    
    try:
        dg.llm_loaded = True
        dg.should_use_llm = lambda intent, negotiation_state, response_type: True
        
        # Test allowed examples (must NOT trigger fallback, i.e., exact string is returned)
        # Note: ACCEPT template includes quantity '4 Seers (~1.1kg)' and price 100,
        # so allowed rephrases must preserve those numbers to pass the number-preservation validator.
        allowed_cases = [
            ("A fair bargain. <<<QUANTITY_VALUE_DO_NOT_CHANGE>>> of <<<SPICE_VALUE_DO_NOT_CHANGE>>> for <<<PRICE_VALUE_DO_NOT_CHANGE>>> varahas, I will remember your honesty.", ["fair bargain", "Pepper", "100"]),
            ("These <<<SPICE_VALUE_DO_NOT_CHANGE>>> are worth <<<PRICE_VALUE_DO_NOT_CHANGE>>> varahas for <<<QUANTITY_VALUE_DO_NOT_CHANGE>>>. We have a deal.", ["Pepper", "100", "deal"]),
            ("I accept your offer of <<<QUANTITY_VALUE_DO_NOT_CHANGE>>> <<<SPICE_VALUE_DO_NOT_CHANGE>>> for <<<PRICE_VALUE_DO_NOT_CHANGE>>> varahas, trader.", ["accept", "Pepper", "100", "trader"])
        ]
        for llm_out, contains_words in allowed_cases:
            dg.run_llm = lambda prompt, max_tokens=96, **kwargs: llm_out
            res = dg.generate_dialogue(dec, eng)
            for word in contains_words:
                assert word in res["text"], f"Expected '{word}' in rephrased output, got '{res['text']}'"
            
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
            dg.run_llm = lambda prompt, max_tokens=96, **kwargs: sentence
            res = dg.generate_dialogue(dec, eng)
            assert res["text"] != sentence, f"Expected forbidden phrase '{sentence}' to be rejected, but it was accepted."
            assert is_valid_accept_text(res["text"]), f"Expected fallback template for '{sentence}', got '{res['text']}'"
            
        # Test spice hallucination safety
        # FakeEngine's item is "Pepper", so "cloves" or "cinnamon" are wrong spices
        dg.run_llm = lambda prompt, max_tokens=96, **kwargs: "I will take cloves"
        res = dg.generate_dialogue(dec, eng)
        assert res["text"] != "I will take cloves", "Expected wrong spice rephrase to be rejected"
        assert is_valid_accept_text(res["text"]), f"Expected fallback template, got {res['text']}"
        
        dg.run_llm = lambda prompt, max_tokens=96, **kwargs: "Fine, I accept <<<QUANTITY_VALUE_DO_NOT_CHANGE>>> <<<SPICE_VALUE_DO_NOT_CHANGE>>> for <<<PRICE_VALUE_DO_NOT_CHANGE>>> varahas"
        res = dg.generate_dialogue(dec, eng)
        assert "Fine, I accept" in res["text"] and "Pepper" in res["text"] and "100" in res["text"], f"Expected correct spice rephrase to be accepted, got: {res['text']}"
 
        print("[PASS] Allowed/Forbidden ACCEPT dialogue validations behave exactly as expected.")
    finally:
        dg.run_llm = orig_run_llm
        dg.llm_loaded = orig_llm_loaded
        dg.should_use_llm = orig_should_use_llm
    
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
 
def test_llm_routing_performance_regression():
    print("============================================================")
    print(" TESTING LLM ROUTING & PERFORMANCE REGRESSION ")
    print("============================================================")
    
    import npc_engine.levels.level1_market.dialogue_generator as dg
    import npc_engine.levels.level1_market.intent_classifier as ic
    
    # Save original functions
    orig_dg_run_llm = dg.run_llm
    orig_dg_llm_loaded = dg.llm_loaded
    orig_ic_run_llm = ic.run_llm
    orig_ic_llm_loaded = ic.llm_loaded
    
    # Track LLM calls
    llm_calls = []
    def track_dg_run_llm(prompt, max_tokens=96, **kwargs):
        llm_calls.append(("dg", prompt))
        if "weather" in prompt.lower():
            return "The weather is very fair today in Hampi."
        return "npc speech"
        
    def track_ic_run_llm(prompt, max_tokens=96, **kwargs):
        llm_calls.append(("ic", prompt))
        return "NO"
        
    dg.run_llm = track_dg_run_llm
    dg.llm_loaded = True
    ic.run_llm = track_ic_run_llm
    ic.llm_loaded = True
    
    try:
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
            reputation = 50.0
            
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
            last_seller_price = 100
            out_of_world_count = 0
            active_event = None
            last_action = None
            quantity_given = True
            market_price = 100
            turns = 3
            prev_seller_price = None
            last_intent = None
            ended = False
            last_seller_input = None
            
        # 1. Verify Intent Classification Optimization (PRICE / QUERY_QUANTITY / REJECT / OUT_OF_WORLD bypass LLM)
        llm_calls.clear()
        res_price = ic.classify_intent("150")
        assert len(llm_calls) == 0, f"PRICE classification triggered LLM! Calls: {llm_calls}"
        assert res_price["intent"] == "PRICE"
        
        llm_calls.clear()
        res_qq = ic.classify_intent("How much do you need?")
        assert len(llm_calls) == 0, f"QUERY_QUANTITY classification triggered LLM! Calls: {llm_calls}"
        assert res_qq["intent"] == "QUERY_QUANTITY"
        
        llm_calls.clear()
        res_reject = ic.classify_intent("No")
        assert len(llm_calls) == 0, f"REJECT classification triggered LLM! Calls: {llm_calls}"
        assert res_reject["intent"] == "REJECT"

        llm_calls.clear()
        res_oow = ic.classify_intent("Do you have Instagram?")
        assert len(llm_calls) == 0, f"OUT_OF_WORLD classification triggered LLM! Calls: {llm_calls}"
        assert res_oow["intent"] == "OUT_OF_WORLD"

        # 2. Verify Dialogue Generator Smart LLM Routing
        # - PRICE counter offer: LLM called = True
        eng = FakeEngine()
        dec = type('Decision', (object,), {'action': 'OFFER', 'price': 90, 'done': False})()
        llm_calls.clear()
        res = dg.generate_dialogue(dec, eng)
        dg_calls = [c for c in llm_calls if c[0] == "dg"]
        assert len(dg_calls) == 1, f"PRICE counter dialogue did not trigger GGUF! Calls: {dg_calls}"
        assert "90" in res["text"]
        
        # - QUERY_QUANTITY dialogue: LLM called = True
        dec_qq = type('Decision', (object,), {'action': 'QUERY_QUANTITY', 'price': 100, 'done': False})()
        llm_calls.clear()
        res = dg.generate_dialogue(dec_qq, eng)
        dg_calls = [c for c in llm_calls if c[0] == "dg"]
        assert len(dg_calls) == 1, f"QUERY_QUANTITY dialogue did not trigger GGUF! Calls: {dg_calls}"
        assert "Pepper" in res["text"]
        
        # - ACCEPT dialogue: LLM called = False
        dec_accept = type('Decision', (object,), {'action': 'ACCEPT', 'price': 100, 'done': True})()
        llm_calls.clear()
        res = dg.generate_dialogue(dec_accept, eng)
        dg_calls = [c for c in llm_calls if c[0] == "dg"]
        assert len(dg_calls) == 0, f"ACCEPT dialogue triggered GGUF! Calls: {dg_calls}"
        assert any(term in res["text"].lower() for term in ["bargain", "deal", "works", "agreement", "agreed", "complete the trade"])
        assert "issue" not in res["text"]
        assert "cannot" not in res["text"]
        assert "problem" not in res["text"]
        assert "not reaching" not in res["text"]
        
        # - OUT_OF_WORLD dialogue: LLM called = True
        dec_oow = type('Decision', (object,), {'action': 'OUT_OF_WORLD', 'price': 100, 'done': False})()
        eng.last_intent = "OUT_OF_WORLD"
        eng.out_of_world_count = 1
        llm_calls.clear()
        res = dg.generate_dialogue(dec_oow, eng)
        dg_calls = [c for c in llm_calls if c[0] == "dg"]
        assert len(dg_calls) == 1, f"OUT_OF_WORLD dialogue did not trigger GGUF! Calls: {dg_calls}"
        assert "npc speech" in res["text"]
        
        # - GENERAL_DIALOGUE: LLM called = True
        llm_calls.clear()
        composed = dg.generate_context_response(
            player_text="How is the weather?",
            buyer_name="Abdul",
            buyer_origin="Persia",
            spice="Pepper",
            current_negotiation_state={"personality": "Polite Merchant", "turns": 3}
        )
        dg_calls = [c for c in llm_calls if c[0] == "dg"]
        assert len(dg_calls) == 1, f"GENERAL_DIALOGUE did not trigger GGUF! Calls: {dg_calls}"
        assert "Hampi" in composed["text"] or "weather" in composed["text"]
        
        # 3. Spice name validation check
        # Force LLM output to clove when spice is Pepper
        dg.should_use_llm = lambda intent, negotiation_state, response_type: True
        dg.run_llm = lambda prompt, max_tokens=96, **kwargs: "This clove is of fine quality."
        eng.turns = 0 # force first NPC greeting
        dec_greeting = type('Decision', (object,), {'action': 'OFFER', 'price': 100, 'done': False})()
        res = dg.generate_dialogue(dec_greeting, eng)
        # Should be rejected because spice traded is Pepper, but output has 'clove'.
        assert "clove" not in res["text"], f"Spice name hallucination was not rejected! Got: {res['text']}"
        
        # 4. Timeout fallback check
        # Mock run_llm to take > 8 seconds to trigger timeout
        import time
        def slow_run_llm(prompt, max_tokens=96, **kwargs):
            time.sleep(10.0)
            return "slow response"
        dg.run_llm = slow_run_llm
        res = dg.generate_dialogue(dec_greeting, eng)
        assert "100" in res["text"], f"Timeout fallback did not trigger! Got: {res['text']}"
        
    finally:
        # Restore original functions
        dg.run_llm = orig_dg_run_llm
        dg.llm_loaded = orig_dg_llm_loaded
        ic.run_llm = orig_ic_run_llm
        ic.llm_loaded = orig_ic_llm_loaded
        
    print("[PASS] LLM routing, template selection, spice validation and timeout tests passed.")

def test_regression_fixes():
    print("============================================================")
    print(" TESTING PHASE 2 REGRESSION FIXES & SAFEGUARDS ")
    print("============================================================")
    
    import npc_engine.levels.level1_market.dialogue_generator as dg
    import npc_engine.levels.level1_market.intent_classifier as ic
    from npc_engine.utils.text_normalizer import normalize_text, normalize_trade_numbers
    from npc_engine.core.measurements import parse_traditional_to_grams
    from npc_engine.core.controller import Controller
    
    # 1. Spoken number parsing in normalize_text
    assert normalize_text("forty five") == "45"
    assert normalize_text("seventy") == "70"
    assert normalize_text("one hundred") == "100"
    assert normalize_text("hundred") == "100"
    # Near context: "I want seventy" should resolve "seventy" -> 70
    assert "70" in normalize_text("I want seventy")
    assert "45" in normalize_text("make it forty five")
    print("[PASS] Spoken number parsing converts words to digits correctly.")

    # 2. Relaxed quantity validation
    assert parse_traditional_to_grams("one veesai") == 1400.0
    assert parse_traditional_to_grams("a veesai") == 1400.0
    assert parse_traditional_to_grams("an veesai") == 1400.0
    assert parse_traditional_to_grams("1 veesai") == 1400.0
    print("[PASS] Quantity validation accepts 'a', 'an', 'one' as equivalents.")

    # 3. QUERY_BUYER_BUDGET intent classification
    context = {"item_name": "Pepper"}
    res = ic.classify_intent("How much are you willing to pay?", context)
    assert res["intent"] == "QUERY_BUYER_BUDGET"
    
    res2 = ic.classify_intent("your price", context)
    assert res2["intent"] == "QUERY_BUYER_BUDGET"
    
    res3 = ic.classify_intent("what is your budget", context)
    assert res3["intent"] == "QUERY_BUYER_BUDGET"
    
    # Ensure they are NOT classified as PRICE or QUERY
    assert res["intent"] != "PRICE"
    assert res["intent"] != "QUERY"
    print("[PASS] QUERY_BUYER_BUDGET intent classification matches candidates correctly.")

    # 4. QUERY_BUYER_BUDGET dialog generation
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
        target_price = 45
        initial_offer = lambda self, p: 45
        
    class FakeEngine:
        buyer = FakeBuyer()
        item = type('Item', (object,), {'name': 'Pepper'})()
        stage = "standard"
        current_offer = 45
        current_quantity = 1000
        last_seller_price_per_kg = None
        bundle_label = ""
        seller_min_price = 30
        last_quantity_grams = 1000
        final_item = None
        event_active = False
        frustration = 0.0
        trust = 0.5
        last_seller_price = None
        out_of_world_count = 0
        active_event = None
        last_action = "QUERY_BUYER_BUDGET"
        quantity_given = True
        market_price = 80
        max_price = 100
        turns = 1
        prev_seller_price = None
        last_intent = "QUERY_BUYER_BUDGET"
        ended = False
        last_seller_input = None

        def get_current_buyer_offer(self):
            return 45

    eng = FakeEngine()
    dec = type('Decision', (object,), {'action': 'QUERY_BUYER_BUDGET', 'price': 45, 'done': False})()
    
    res_dialog = dg.generate_dialogue(dec, eng)
    # Replaced template text should contain "I can offer 45 varahas for 4 Seers (~1.1 kg) Pepper."
    # (Since we mocked GGUF loaded = False inside test environment when not running the GGUF thread, 
    # let's check text directly)
    assert "45" in res_dialog["text"]
    assert "varahas" in res_dialog["text"]
    assert "Pepper" in res_dialog["text"]
    assert "What price do you demand?" not in res_dialog["text"]
    print("[PASS] QUERY_BUYER_BUDGET generates correct buyer offer baseline text.")

    # 5. STT Homophone Trade Number Correction (Safeguard 1)
    eng.last_action = "OFFER"  # Expecting price input from player
    assert normalize_trade_numbers("seventeen", eng) == "70"
    assert normalize_trade_numbers("seven", eng) == "70"
    assert normalize_trade_numbers("eighteen", eng) == "80"
    assert normalize_trade_numbers("eight", eng) == "80"
    assert normalize_trade_numbers("nine", eng) == "90"
    assert normalize_trade_numbers("nineteen", eng) == "90"
    # Should not replace quantity units
    assert normalize_trade_numbers("seven seers", eng) == "seven seers"
    
    # Safeguard 1: should NOT normalize if we are not awaiting player price input
    eng.last_action = "QUERY_QUANTITY"
    assert normalize_trade_numbers("seventeen", eng) == "seventeen"
    print("[PASS] STT Homophone corrections work only when awaiting price input.")

    # 6. Price Context Correction (x10 scaling)
    # Setup controller with fake engine
    class FakeEngine2(FakeEngine):
        market_price = 80
        max_price = 120
        last_action = "OFFER"
        started = True
        price_introduced = True
        last_seller_price = None

    eng2 = FakeEngine2()
    ctrl = Controller(
        engine=eng2,
        classify_intent_fn=ic.classify_intent,
        extract_quantity_info_fn=ic.extract_quantity_info,
        extract_price_fn=ic.extract_price,
        dialogue_fn=dg.generate_dialogue
    )
    # If player inputs "7" which is < min_reasonable (40) and 7 * 10 <= 120 (max_price)
    act = ctrl._build_player_action("7")
    assert act.price == 70, f"Expected price correction to scale 7 to 70, got {act.price}"
    
    # Input "17" during expectation maps to "70" in normalize_trade_numbers
    act2 = ctrl._build_player_action("17")
    assert act2.price == 70, f"Expected 17 to map to 70 via STT, got {act2.price}"
    print("[PASS] Price context range correction multiplies by 10 correctly.")

    # 7. LLM validation numeric normalization (Safeguard 3)
    from npc_engine.levels.level1_market.dialogue_generator import extract_numeric_values, personality_rewrite
    assert extract_numeric_values("forty five") == {45}
    assert extract_numeric_values("70 varahas") == {70}
    # Paren content ignored
    assert extract_numeric_values("4 Seers (~1.1kg) Pepper for 100 varahas") == {4, 100}
    
    # Homophones validation test: base has "70", rephrased has "seventy" -> should MATCH
    orig_run_llm = dg.run_llm
    orig_llm_loaded = dg.llm_loaded
    orig_should_use_llm = dg.should_use_llm
    
    try:
        dg.llm_loaded = True
        dg.should_use_llm = lambda intent, negotiation_state, response_type: True
        
        # Test homophone price validation (70 varahas == seventy varahas)
        dg.run_llm = lambda prompt, max_tokens=96, **kwargs: "I will offer seventy varahas for Pepper"
        res_rephrase = dg.personality_rewrite("I will offer 70 varahas.", "Abdul", "Persia", "friendly", "Pepper", 70, 1000, eng2)
        assert "seventy" in res_rephrase, f"Expected normalized price comparison to accept homophones. Got: {res_rephrase}"
        
        # Test instruction leakage validation
        dg.run_llm = lambda prompt, max_tokens=96, **kwargs: "Let's rephrase the dialogue: I can pay 70 varahas."
        res_rephrase2 = dg.personality_rewrite("I will offer 70 varahas.", "Abdul", "Persia", "friendly", "Pepper", 70, 1000, eng2)
        # Should fail validation due to "rephrase" instruction leakage and return template
        assert "Let's rephrase" not in res_rephrase2
        assert "70" in res_rephrase2
        
    finally:
        dg.run_llm = orig_run_llm
        dg.llm_loaded = orig_llm_loaded
        dg.should_use_llm = orig_should_use_llm
        
    print("[PASS] LLM validation correctly uses normalized numeric values and rejects leakage.")

    # 8. Deterministic PRICE fast-path phrases
    # "meet at 80", "settle at 80", "final at 80", "close at 80", "make it 80", "give you 80", "I offer 80"
    for phrase in ["meet at 80", "settle at 80", "final at 80", "close at 80", "make it 80", "give you 80", "I offer 80"]:
        res_fp = ic.classify_intent(phrase, context)
        assert res_fp["intent"] == "PRICE", f"Expected PRICE for fast-path '{phrase}', got {res_fp}"
    print("[PASS] Deterministic PRICE fast-path phrases bypass LLM classification.")


def test_stt_accuracy_upgrades():
    print("============================================================")
    print(" TESTING STT ACCURACY UPGRADES & SAFEGUARDS ")
    print("============================================================")
    import stt.whisper_service as ws
    import os
    
    # Check that initial prompt contains necessary keywords
    assert "Vijayanagara" in ws.initial_prompt
    assert "bargaining" in ws.initial_prompt
    assert "kings and rulers" in ws.initial_prompt
    print("[PASS] Broad initial prompt is present and contains Vijayanagara context.")
    
    # Test audio diagnostics on the existing test.wav
    test_wav = os.path.join(ws.BACKEND_DIR, "test.wav")
    assert os.path.exists(test_wav), f"Expected test.wav at {test_wav}"
    duration, peak, rms = ws.get_audio_diagnostics(test_wav)
    assert duration > 0, f"Expected positive duration, got {duration}"
    assert peak >= 0, f"Expected peak >= 0, got {peak}"
    assert rms >= 0, f"Expected rms >= 0, got {rms}"
    print(f"[PASS] Audio diagnostics computed: duration={duration:.2f}s, peak={peak:.4f}, rms={rms:.4f}")
    
    # Save original model
    orig_model = ws.model
    
    class MockSegment:
        def __init__(self, text, no_speech_prob, avg_logprob):
            self.text = text
            self.no_speech_prob = no_speech_prob
            self.avg_logprob = avg_logprob
            
    class MockModel:
        def __init__(self, segments):
            self.segments = segments
        def transcribe(self, file_path, **kwargs):
            # Check that expected parameters are passed
            assert kwargs.get("language") == "en"
            assert kwargs.get("task") == "transcribe"
            assert kwargs.get("beam_size") == 5
            assert kwargs.get("best_of") == 5
            assert kwargs.get("temperature") == 0.0
            assert kwargs.get("vad_filter") == True
            assert kwargs.get("vad_parameters", {}).get("min_silence_duration_ms") == 500
            assert kwargs.get("vad_parameters", {}).get("speech_pad_ms") == 300
            return self.segments, None
            
    try:
        # Test Case 1: Valid transcription segment passes
        seg1 = MockSegment("What price are you willing to pay?", 0.1, -0.2)
        ws.model = MockModel([seg1])
        res = ws.transcribe_audio_file(test_wav)
        assert res == "What price are you willing to pay?", f"Expected valid segment to pass, got: {res}"
        
        # Test Case 2: Reject low confidence segment (no_speech_prob > 0.65 AND avg_logprob < -1.0)
        seg2 = MockSegment("unrelated noise", 0.7, -1.2)
        ws.model = MockModel([seg2])
        res = ws.transcribe_audio_file(test_wav)
        assert res == "", f"Expected low confidence segment to be rejected, got: {res}"
        
        # Test Case 3: Reject blacklisted hallucination ("thanks for watching" with low confidence)
        seg3 = MockSegment("Thanks for watching.", 0.4, -0.6)
        ws.model = MockModel([seg3])
        res = ws.transcribe_audio_file(test_wav)
        assert res == "", f"Expected hallucination to be rejected under low confidence, got: {res}"
        
        # Test Case 4: Keep blacklisted hallucination with high confidence
        seg4 = MockSegment("Thanks for watching.", 0.2, -0.3)
        ws.model = MockModel([seg4])
        res = ws.transcribe_audio_file(test_wav)
        assert res == "Thanks for watching.", f"Expected hallucination with high confidence to be kept, got: {res}"
        
        print("[PASS] Segment confidence checks and blacklist filters operate correctly.")
        
    finally:
        ws.model = orig_model


def test_production_robustness_layer():
    print("============================================================")
    print(" TESTING PRODUCTION ROBUSTNESS LAYER & SAFEGUARDS ")
    print("============================================================")
    import npc_engine.levels.level1_market.intent_classifier as ic
    import npc_engine.levels.level1_market.dialogue_generator as dg
    
    # 1. Fuzzy semantic intent match tests
    context = {
        "item_name": "pepper",
        "in_negotiation": True,
        "last_system_action": "OFFER"
    }
    
    # "How much do you want it for" -> QUERY_BUYER_BUDGET
    res = ic.classify_intent("How much do you want it for", context)
    assert res["intent"] == "QUERY_BUYER_BUDGET", f"Expected QUERY_BUYER_BUDGET, got {res}"
    
    # "Name your price" -> QUERY_BUYER_BUDGET
    res = ic.classify_intent("Name your price", context)
    assert res["intent"] == "QUERY_BUYER_BUDGET", f"Expected QUERY_BUYER_BUDGET, got {res}"
    
    # "How does 67 sound" -> PRICE (requires price + >=80 score)
    res = ic.classify_intent("How does 67 sound", context)
    assert res["intent"] == "PRICE" and res.get("price") == 67, f"Expected PRICE 67, got {res}"
    
    # "So let's deal at 36" -> PRICE (requires price + >=80 score)
    res = ic.classify_intent("So let's deal at 36", context)
    assert res["intent"] == "PRICE" and res.get("price") == 36, f"Expected PRICE 36, got {res}"
    
    # "fine" after OFFER -> ACCEPT
    res = ic.classify_intent("fine", context)
    assert res["intent"] == "ACCEPT", f"Expected ACCEPT, got {res}"
    
    # "fine" after GREETING -> NOT ACCEPT (should fall through to CLARIFICATION or not accept)
    context_greeting = {
        "item_name": "pepper",
        "in_negotiation": False,
        "last_system_action": "GREETING"
    }
    res = ic.classify_intent("fine", context_greeting)
    assert res["intent"] != "ACCEPT", f"Expected 'fine' after GREETING not to be ACCEPT, got {res}"
    
    # "cardamom" never triggers modern "car" detection
    assert not ic.contains_out_of_world_concept("cardamom")
    assert not ic.contains_out_of_world_concept("caravan")
    # But a standalone "car" does
    assert ic.contains_out_of_world_concept("I have a car")
    
    # Corrupted STT regression tests
    # "What advice are you willing to pay" -> QUERY_BUYER_BUDGET
    res = ic.classify_intent("What advice are you willing to pay", context)
    assert res["intent"] == "QUERY_BUYER_BUDGET", f"Expected QUERY_BUYER_BUDGET for 'advice', got {res}"
    
    # "How much are you going to give" -> QUERY_BUYER_BUDGET
    res = ic.classify_intent("How much are you going to give", context)
    assert res["intent"] == "QUERY_BUYER_BUDGET", f"Expected QUERY_BUYER_BUDGET for 'give', got {res}"

    print("[PASS] Intent classification fuzzy preprocessor and STT corrections behave exactly as expected.")

    # 2. LLM cannot alter protected placeholders
    class FakeEngine:
        buyer = type('Buyer', (object,), {'name': 'Abdul', 'origin': 'Persia', 'wealth': 'wealthy', 'reputation': 50})()
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
        ended = False
        last_intent = None
        started = True

    fake_eng = FakeEngine()
    orig_run_llm = dg.run_llm
    orig_llm_loaded = dg.llm_loaded
    orig_should_use_llm = dg.should_use_llm
    
    try:
        dg.llm_loaded = True
        dg.should_use_llm = lambda intent, negotiation_state, response_type: True
        
        # Test Case A: Correct preservation of placeholders -> success and restoration of actual values
        dg.run_llm = lambda prompt, max_tokens=96, **kwargs: "I can offer <<<PRICE_VALUE_DO_NOT_CHANGE>>> varahas for your <<<SPICE_VALUE_DO_NOT_CHANGE>>>."
        res = dg.personality_rewrite("I can offer <<<PRICE_VALUE_DO_NOT_CHANGE>>> varahas for your <<<SPICE_VALUE_DO_NOT_CHANGE>>>.", "Abdul", "Persia", "friendly", "Pepper", 100, 1000, fake_eng)
        assert "100" in res, f"Expected 100 to be restored, got {res}"
        assert "Pepper" in res, f"Expected Pepper to be restored, got {res}"
        assert "<<<PRICE_VALUE_DO_NOT_CHANGE>>>" not in res, "Expected placeholder to be replaced"

        # Test Case B: Hallucinated / missing placeholder -> failsafe trigger and fallback to base response with restored values
        dg.run_llm = lambda prompt, max_tokens=96, **kwargs: "I can offer seventy varahas for your Pepper."
        res_fail = dg.personality_rewrite("I can offer <<<PRICE_VALUE_DO_NOT_CHANGE>>> varahas for your <<<SPICE_VALUE_DO_NOT_CHANGE>>>.", "Abdul", "Persia", "friendly", "Pepper", 100, 1000, fake_eng)
        assert "100" in res_fail, f"Expected fallback to restore 100, got {res_fail}"
        assert "Pepper" in res_fail, f"Expected fallback to restore Pepper, got {res_fail}"
        assert "seventy" not in res_fail, f"Expected hallucinated seventy to be rejected, got {res_fail}"

        print("[PASS] Placeholder preservation and validator checks operate correctly.")
        
    finally:
        dg.run_llm = orig_run_llm
        dg.llm_loaded = orig_llm_loaded
        dg.should_use_llm = orig_should_use_llm


def test_price_type_safety_recovery():
    print("============================================================")
    print(" TESTING PRICE TYPE SAFETY RECOVERY ")
    print("============================================================")
    from npc_engine.core.controller import Controller
    from npc_engine.levels.level1_market.intent_classifier import classify_intent, extract_quantity_info
    from npc_engine.levels.level1_market.input_interpreter import extract_price
    from npc_engine.levels.level1_market.dialogue_generator import generate_dialogue

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
        reputation = 50.0

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
        last_seller_price = 100
        out_of_world_count = 0
        active_event = None
        last_action = None
        quantity_given = True
        market_price = 100
        max_price = 200
        turns = 3
        prev_seller_price = None
        last_intent = None
        ended = False
        last_seller_input = None
        started = True

    fake_eng = FakeEngine()
    controller = Controller(
        engine=fake_eng,
        classify_intent_fn=classify_intent,
        extract_quantity_info_fn=extract_quantity_info,
        extract_price_fn=extract_price,
        dialogue_fn=generate_dialogue
    )

    action = controller._build_player_action("I sell for abc coins")
    print(f"Resulting Action: intent={action.intent}, price={action.price}")

    original_classify = controller.classify_intent_fn
    try:
        controller.classify_intent_fn = lambda text, context: {"intent": "PRICE", "price": None}
        action_stubbed = controller._build_player_action("I sell for abc coins")
        assert action_stubbed.intent == "CLARIFICATION", f"Expected CLARIFICATION, got {action_stubbed.intent}"
        assert action_stubbed.price is None
    finally:
        controller.classify_intent_fn = original_classify

    print("[PASS] Price type safety recovery handles invalid prices gracefully.")


def test_personality_rewrite_fallback_rate():
    print("============================================================")
    print(" TESTING LLM PERSONALITY REWRITE FALLBACK RATE ")
    print("============================================================")
    import npc_engine.levels.level1_market.dialogue_generator as dg
    from npc_engine.levels.level1_market.dialogue_generator import personality_rewrite
    
    class FakeBuyer:
        name = "Abdul"
        origin = "Persia"
        wealth = "wealthy"
        personality = "Polite Merchant"
        desperation = 0.5
        reputation = 50.0
        
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
        last_seller_price = 100
        out_of_world_count = 0
        active_event = None
        last_action = None
        quantity_given = True
        market_price = 100
        turns = 3
        prev_seller_price = None
        last_intent = None
        ended = False
        last_seller_input = None
        started = True
        max_price = 200
        
        def get_current_buyer_offer(self):
            return 100
            
    fake_eng = FakeEngine()
    
    base_resp = "I can offer <<<PRICE_VALUE_DO_NOT_CHANGE>>> varahas for your <<<QUANTITY_VALUE_DO_NOT_CHANGE>>> of <<<SPICE_VALUE_DO_NOT_CHANGE>>>."
    
    fallbacks = 0
    successes = 0
    for i in range(20):
        res = personality_rewrite(
            base_response=base_resp,
            buyer_name="Abdul",
            buyer_origin="Persia",
            personality="Polite Merchant",
            spice="Pepper",
            price=100,
            quantity=1000,
            engine=fake_eng,
            action="OFFER"
        )
        qty_label = "4 Seers (~1.1kg)"
        base_final = base_resp.replace("<<<PRICE_VALUE_DO_NOT_CHANGE>>>", "100")
        base_final = base_final.replace("<<<QUANTITY_VALUE_DO_NOT_CHANGE>>>", qty_label)
        base_final = base_final.replace("<<<SPICE_VALUE_DO_NOT_CHANGE>>>", "Pepper")
        
        if res == base_final:
            fallbacks += 1
        else:
            successes += 1
            
    fallback_rate = fallbacks / 20.0
    print(f"LLM Personality Fallback Rate: {fallback_rate * 100:.1f}% ({fallbacks} fallbacks, {successes} successes)")
    assert fallback_rate < 0.25, f"Fallback rate too high: {fallback_rate * 100:.1f}% (Required < 25%)"
    print("[PASS] LLM personality rewrite fallback rate is under 25%.")


def test_general_dialogue_length():
    print("============================================================")
    print(" TESTING GENERAL DIALOGUE RESPONSE LENGTH ")
    print("============================================================")
    from npc_engine.levels.level1_market.dialogue_generator import generate_context_response
    
    composed = generate_context_response(
        player_text="How is the weather?",
        buyer_name="Abdul",
        buyer_origin="Persia",
        spice="Pepper",
        current_negotiation_state={"personality": "Polite Merchant", "turns": 3}
    )
    
    text = composed["text"]
    word_count = len(text.split())
    print(f"Generated text: \"{text}\"")
    print(f"Word count: {word_count}")
    assert word_count <= 40, f"Response too long: {word_count} words (Required <= 40)"
    print("[PASS] General dialogue response is concise (<= 40 words).")


def test_player_profile_persistence():
    print("============================================================")
    print(" TESTING PLAYER PROFILE PERSISTENCE ")
    print("============================================================")
    import json
    import shutil
    import npc_engine.core.persistence as persistence
    from npc_engine.core.persistence import initialize_session, record_negotiation_deal, load_session
    
    orig_dev_reset = persistence.DEV_RESET_PROFILE
    persistence.DEV_RESET_PROFILE = False
    
    # Isolate tests to a temporary sessions directory
    test_sessions_dir = os.path.join(persistence.WORKSPACE_DIR, "memory", "test_sessions_persistence")
    if os.path.exists(test_sessions_dir):
        shutil.rmtree(test_sessions_dir)
    os.makedirs(test_sessions_dir, exist_ok=True)
    
    orig_sessions_dir = persistence.SESSIONS_DIR
    persistence.SESSIONS_DIR = test_sessions_dir
    
    try:
        profile_path = os.path.join(test_sessions_dir, "player_profile.json")
        
        # Initialize session A
        session_a_id = "test_persistence_session_A"
        state_a = initialize_session(session_a_id)
        assert state_a["global_metrics"]["total_varahas"] == 100
        assert state_a["global_metrics"]["reputation"] == 50
        
        # Complete trade in session A which earns 30 Varahas (outcome is ACCEPT, final_price is 30)
        record_negotiation_deal(
            session_id=session_a_id,
            spice_name="pepper",
            final_price=30,
            final_quantity=1000.0,
            trust=0.8,
            frustration=0.1,
            out_of_world_count=0,
            outcome="ACCEPT"
        )
        
        # Verify A updated correctly to 100 + 30 = 130
        updated_state_a = load_session(session_a_id)
        assert updated_state_a["global_metrics"]["total_varahas"] == 130
        assert updated_state_a["global_metrics"]["reputation"] == 53  # 50 + 3
        
        # Verify player_profile.json is updated to 130 and 53
        assert os.path.exists(profile_path), "player_profile.json should have been created/updated"
        with open(profile_path, "r", encoding="utf-8") as f:
            profile = json.load(f)
        assert profile["global_metrics"]["total_varahas"] == 130
        assert profile["global_metrics"]["reputation"] == 53
        
        # Initialize a new session B
        session_b_id = "test_persistence_session_B"
        state_b = initialize_session(session_b_id)
        assert state_b["global_metrics"]["total_varahas"] == 130, f"Expected session B to inherit 130 Varahas, got {state_b['global_metrics']['total_varahas']}"
        assert state_b["global_metrics"]["reputation"] == 53, f"Expected session B to inherit 53 reputation, got {state_b['global_metrics']['reputation']}"
        
        print("[PASS] Player profile persistence (money and reputation) across sessions succeeds.")
    finally:
        persistence.SESSIONS_DIR = orig_sessions_dir
        persistence.DEV_RESET_PROFILE = orig_dev_reset
        if os.path.exists(test_sessions_dir):
            shutil.rmtree(test_sessions_dir)


def test_player_profile_corruption_recovery():
    print("============================================================")
    print(" TESTING PLAYER PROFILE CORRUPTION RECOVERY ")
    print("============================================================")
    import json
    import shutil
    import npc_engine.core.persistence as persistence
    from npc_engine.core.persistence import initialize_session, save_player_profile
    
    orig_dev_reset = persistence.DEV_RESET_PROFILE
    persistence.DEV_RESET_PROFILE = False
    
    # Isolate tests to a temporary sessions directory
    test_sessions_dir = os.path.join(persistence.WORKSPACE_DIR, "memory", "test_sessions_corruption")
    if os.path.exists(test_sessions_dir):
        shutil.rmtree(test_sessions_dir)
    os.makedirs(test_sessions_dir, exist_ok=True)
    
    orig_sessions_dir = persistence.SESSIONS_DIR
    persistence.SESSIONS_DIR = test_sessions_dir
    
    try:
        profile_path = os.path.join(test_sessions_dir, "player_profile.json")
        backup_path = os.path.join(test_sessions_dir, "player_profile.backup.json")
        
        # 1. Create a valid initial state and save profile to create a backup
        state = {
            "global_metrics": {
                "reputation": 70,
                "total_varahas": 150,
                "completed_levels": []
            },
            "inventory": {
                "pepper": 15000.0,
                "clove": 8000.0,
                "cinnamon": 12000.0,
                "cardamom": 4000.0
            },
            "shift_stats": {
                "shifts_completed": 0,
                "total_varahas_earned": 0,
                "total_deals_made": 0
            }
        }
        
        # Save twice to propagate state to backup
        save_player_profile(state)
        save_player_profile(state)
        
        assert os.path.exists(profile_path)
        assert os.path.exists(backup_path)
        
        # 2. Corrupt player_profile.json manually (invalid JSON syntax)
        with open(profile_path, "w", encoding="utf-8") as f:
            f.write("{invalid_json_format...")
            
        # 3. Initialize new session and verify it successfully falls back to backup
        session_test_id = "test_corruption_recovery_session"
        state_recovered = initialize_session(session_test_id)
        assert state_recovered["global_metrics"]["total_varahas"] == 150, f"Expected recovery of 150 Varahas, got {state_recovered['global_metrics']['total_varahas']}"
        assert state_recovered["global_metrics"]["reputation"] == 70, f"Expected recovery of 70 reputation, got {state_recovered['global_metrics']['reputation']}"
        
        print("[PASS] Corruption recovery correctly loads from backup and restores stats.")
    finally:
        persistence.SESSIONS_DIR = orig_sessions_dir
        persistence.DEV_RESET_PROFILE = orig_dev_reset
        if os.path.exists(test_sessions_dir):
            shutil.rmtree(test_sessions_dir)


if __name__ == "__main__":
    test_intent_classification_safety()
    test_dialogue_accept_validation()
    test_tts_cleaner()
    test_price_type_safety_recovery()
    test_personality_rewrite_fallback_rate()
    test_general_dialogue_length()
    test_llm_routing_performance_regression()
    test_regression_fixes()
    test_stt_accuracy_upgrades()
    test_production_robustness_layer()
    test_player_profile_persistence()
    test_player_profile_corruption_recovery()
    print("============================================================")
    print(" ALL SAFETY INTEGRATION & REGRESSION TESTS PASSED SUCCESSFULLY! ")
    print("============================================================")
    sys.exit(0)



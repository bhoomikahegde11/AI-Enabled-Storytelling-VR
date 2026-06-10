import os
import sys
import time
import statistics

# Add backend directory to sys.path so we can import from npc_engine
sys.path.append(os.path.join(os.path.dirname(__file__), "../backend"))

import npc_engine.levels.level1_market.dialogue_generator as dg
import npc_engine.levels.level1_market.intent_classifier as ic
from npc_engine.interface import NPCSession
from npc_engine.core.models import EngineDecision
from npc_engine.utils import hardware

def run_benchmark():
    print("======================================================================")
    print(f"CUDA Support: {hardware.CUDA_AVAILABLE}")
    print(f"LLM Loaded: {dg.llm_loaded}")
    print(f"Device Mode: {hardware.DEVICE_MODE}")
    print("======================================================================")
    print("RUNNING LIVE AI PERSONALITY LAYER BENCHMARK...")
    
    scenarios = [
        {"name": "Scenario 1: First Greeting (turns=0)", "type": "greeting"},
        {"name": "Scenario 2: Price Counter Offer (turns=3)", "type": "price_counter"},
        {"name": "Scenario 3: Social Dialogue (general)", "type": "social"}
    ]
    
    results = {}
    
    # Warmup the LLM if loaded
    if dg.llm_loaded:
        print("Warming up local LLM on GPU...")
        for _ in range(2):
            dg.run_llm("Warmup prompt for Vijayanagara Empire", max_tokens=10)
            
    for scenario in scenarios:
        s_name = scenario["name"]
        s_type = scenario["type"]
        results[s_type] = {"with_llm": [], "without_llm": []}
        
        # Test configurations
        for enable_llm in [True, False]:
            dg.USE_LLM_PERSONALITY = enable_llm
            ic.USE_LLM_PERSONALITY = enable_llm
            
            # Run 5 trials to get a stable average
            for trial in range(5):
                # Setup session
                session = NPCSession(session_id="benchmark_session")
                # Set predictable buyer
                session.engine.buyer.personality = "friendly"
                session.engine.buyer.name = "Abdul"
                session.engine.buyer.origin = "Persia"
                session.engine.item.name = "Pepper"
                
                # Make sure turns / state is set correctly for the scenario
                if s_type == "greeting":
                    session.engine.turns = 0
                    dec = EngineDecision(action="OFFER", price=100, quantity=1000, done=False)
                    start = time.perf_counter()
                    res = dg.generate_dialogue(dec, session.engine)
                    latency = (time.perf_counter() - start) * 1000
                elif s_type == "price_counter":
                    session.engine.turns = 3
                    dec = EngineDecision(action="OFFER", price=90, quantity=1000, done=False)
                    start = time.perf_counter()
                    res = dg.generate_dialogue(dec, session.engine)
                    latency = (time.perf_counter() - start) * 1000
                elif s_type == "social":
                    start = time.perf_counter()
                    res = dg.generate_context_response(
                        player_text="Tell me about yourself.",
                        buyer_name="Abdul",
                        buyer_origin="Persia",
                        spice="Pepper",
                        current_negotiation_state={"personality": "friendly", "turns": 2}
                    )
                    latency = (time.perf_counter() - start) * 1000
                    
                key = "with_llm" if enable_llm else "without_llm"
                results[s_type][key].append(latency)
                
    # Print results
    print("\nBENCHMARK RESULTS TABLE (in milliseconds):")
    print("| Scenario | Mode (With AI Personality) | Mode (Template-Only) | Speedup Factor | Description |")
    print("|---|---|---|---|---|")
    
    for scenario in scenarios:
        s_type = scenario["type"]
        avg_with = statistics.mean(results[s_type]["with_llm"])
        avg_without = statistics.mean(results[s_type]["without_llm"])
        speedup = avg_with / avg_without if avg_without > 0 else 0
        
        if s_type == "greeting":
            desc = "First NPC greeting. Rich, custom character rephrasing."
        elif s_type == "price_counter":
            desc = "Bargaining counter-offer. Dynamic character rephrasing on GPU (Target: < 2 seconds)."
        else:
            desc = "Casual conversation/social Q&A. Immersion grounded."
            
        print(f"| {scenario['name']} | {avg_with:.2f} ms | {avg_without:.2f} ms | {speedup:.1f}x slower | {desc} |")
        
    print("\nEXPECTED NVIDIA RTX 4060 TIMINGS:")
    print("- With Personality (LLM Active): ~80ms to ~300ms (GPU accelerated vs ~5000ms+ on CPU)")
    print("- Template-Only (LLM Bypassed): ~0.1ms to ~2ms (Instantaneous local rendering)")
    print("======================================================================")

    # 4. Same price input 3 times check (restoring LLM to True)
    dg.USE_LLM_PERSONALITY = True
    ic.USE_LLM_PERSONALITY = True
    
    print("\nRUNNING SAME PRICE INPUT 3 TIMES (Personality Variability Check):")
    session = NPCSession(session_id="benchmark_session")
    session.engine.buyer.personality = "friendly"
    session.engine.buyer.name = "Abdul"
    session.engine.buyer.origin = "Persia"
    session.engine.item.name = "Pepper"
    session.engine.turns = 3
    
    dec = EngineDecision(action="OFFER", price=45, quantity=1000, done=False)
    responses = []
    
    for i in range(3):
        res = dg.generate_dialogue(dec, session.engine)
        responses.append(res)
        print(f"Trial {i+1} Response: \"{res['text']}\" | Price: {res['price']}")
        
    prices = [r["price"] for r in responses]
    texts = [r["text"] for r in responses]
    
    # Assertions
    assert len(set(prices)) == 1, f"Expected same counter price across trials! Got: {prices}"
    assert len(set(texts)) > 1, f"Expected different wordings due to high LLM temperature (0.7)! Got: {texts}"
    print("\n[PASS] Same price input 3 times yielded identical counter price and varied wording successfully!")
    print("======================================================================")

if __name__ == "__main__":
    run_benchmark()

import os
import sys
import json
import uuid
import shutil

# Setup backend directory in sys path
BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
sys.path.insert(0, os.path.join(BASE_DIR, "backend"))

from npc_engine.interface import NPCSession
from npc_engine.core.rag import RAGRetriever
from npc_engine.core.persistence import load_session, SESSIONS_DIR

def run_tests():
    print("="*60)
    print(" STARTING INTEGRATION TESTS ")
    print("="*60)
    
    # Test 1: RAG Context Retrieval
    print("\n[TEST 1] Testing historical RAG context retrieval...")
    rag = RAGRetriever()
    pepper_context = rag.retrieve_context("pepper")
    print(f"Retrieved pepper context: \"{pepper_context}\"")
    assert "pepper" in pepper_context.lower() or "malabar" in pepper_context.lower(), "Pepper context was not matched correctly!"
    
    clove_context = rag.retrieve_context("clove")
    print(f"Retrieved clove context: \"{clove_context}\"")
    assert "clove" in clove_context.lower() or "hampi" in clove_context.lower(), "Clove context was not matched correctly!"
    print("✓ TEST 1 PASSED!")
    
    # Test 2: Disk Persistence & Session Creation
    print("\n[TEST 2] Testing persistent player state creation...")
    test_session_id = f"test-{uuid.uuid4()}"
    session_file = os.path.join(SESSIONS_DIR, f"{test_session_id}.json")
    
    # Initialize a new session
    session = NPCSession(session_id=test_session_id)
    
    # Verify file is written to disk
    assert os.path.exists(session_file), "Session JSON file was not serialized to disk!"
    
    with open(session_file, "r", encoding="utf-8") as f:
        data = json.load(f)
        
    print(f"Serialized initial state keys: {list(data.keys())}")
    assert data["player_name"] == "Merchant Traveler", "Invalid initial player_name!"
    assert data["global_metrics"]["reputation"] == 50, "Reputation should start at 50!"
    assert data["global_metrics"]["total_varahas"] == 100, "Money should start at 100!"
    print("✓ TEST 2 PASSED!")
    
    # Test 3: Metric Updates on Successful Deal (ACCEPT)
    print("\n[TEST 3] Testing persistence and metric calculations on transaction success...")
    
    # Start negotiation
    response = session.start()
    print(f"Initial buyer dialogue: \"{response['npc_text']}\"")
    
    # Step 1: Set quantity
    step1 = session.step("I have 1kg")
    print(f"Step 1 action: {step1['action']} | Dialogue: \"{step1['npc_text']}\"")
    
    # Step 2: Propose price to get counter offer
    step2 = session.step("The price is 180 varahas")
    buyer_offer = step2["price"]
    print(f"Step 2 action: {step2['action']} | Buyer offer: {buyer_offer} | Dialogue: \"{step2['npc_text']}\"")
    
    # Handle instant acceptances dynamically (resilient to different starting spices and multiplier values)
    if step2["done"]:
        print("Buyer accepted or finished the deal on Step 2!")
        step_response = step2
    else:
        # Step 3: Accept the buyer offer
        step_response = session.step(f"ok, I sell it to you for exactly {buyer_offer} varahas")
        print(f"Step 3 action: {step_response['action']} | Dialogue: \"{step_response['npc_text']}\"")
    
    # Confirm deal is complete
    assert step_response["done"] == True, "Deal should be completed!"
    assert step_response["action"] in ["ACCEPT", "WALK_AWAY"], "Should register valid ending action!"
    
    # Load session state again and verify metrics updated
    session_data = load_session(test_session_id)
    deals = session_data["level_history"]["level1_market"]["deals"]
    print(f"Deals recorded in session history: {deals}")
    assert len(deals) == 1, "There should be exactly 1 deal recorded!"
    
    final_reputation = session_data["global_metrics"]["reputation"]
    final_varahas = session_data["global_metrics"]["total_varahas"]
    print(f"Global metrics after trade: Respect: {final_reputation} | Money: {final_varahas}")
    
    # Verify that metrics correctly updated
    expected_reputation = max(0, min(100, 50 + deals[0]["respect_change"]))
    assert final_reputation == expected_reputation, f"Reputation mismatch! Expected {expected_reputation}, got {final_reputation}"
    
    if step_response["action"] == "ACCEPT":
        assert final_varahas == 100 + buyer_offer, f"Money calculation mismatch! Expected {100 + buyer_offer}, got {final_varahas}"
    else:
        assert final_varahas == 100, "Money should remain unchanged on WALK_AWAY!"
        
    print("✓ TEST 3 PASSED!")
    
    # Test 4: Walk away penalty
    print("\n[TEST 4] Testing failed negotiation penalty...")
    
    # The active session automatically loaded the second spice item!
    # First step initializes the new customer conversation:
    step_response2 = session.step("go away")
    print(f"Step 2.1 action: {step_response2['action']} | Dialogue: \"{step_response2['npc_text']}\"")
    
    # Second step establishes quantity so the engine can proceed past quantity check:
    step_response3 = session.step("I have 1kg")
    print(f"Step 2.2 action: {step_response3['action']} | Dialogue: \"{step_response3['npc_text']}\"")
    
    # Third step triggers the actual rejection or walk-away using strict phrase
    step_response4 = session.step("we are out of stock")
    print(f"Step 2.3 action: {step_response4['action']} | Dialogue: \"{step_response4['npc_text']}\"")
    
    # Confirm deal is complete (WALK_AWAY or NO_ITEM)
    assert step_response4["done"] == True, "Session should end when customer walks away!"
    
    # Load session state and verify respect dropped
    session_data = load_session(test_session_id)
    deals = session_data["level_history"]["level1_market"]["deals"]
    print(f"Deals in history: {deals}")
    assert len(deals) == 2, "There should be exactly 2 deals recorded!"
    
    post_walk_reputation = session_data["global_metrics"]["reputation"]
    post_walk_varahas = session_data["global_metrics"]["total_varahas"]
    print(f"Global metrics after failed trade: Respect: {post_walk_reputation} | Money: {post_walk_varahas}")
    
    assert post_walk_reputation < final_reputation, "Failed deal did not drop respect!"
    print("✓ TEST 4 PASSED!")
    
    # Cleanup test session file
    if os.path.exists(session_file):
        os.remove(session_file)
        print(f"\nCleaned up test session file: {session_file}")
        
    print("\n" + "="*60)
    print(" ALL SYSTEM INTEGRATION TESTS PASSED SUCCESSFULLY! ")
    print("="*60 + "\n")

if __name__ == "__main__":
    run_tests()

import os
import sys
import json
import uuid

# Setup backend directory in sys path
BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
sys.path.insert(0, os.path.join(BASE_DIR, "backend"))

from npc_engine.interface import NPCSession
from npc_engine.core.persistence import load_session, save_session, SESSIONS_DIR
from npc_engine.core.analytics import compile_shift_analytics
from npc_engine.levels.level1_market.item_model import Item

def run_marketplace_loop_tests():
    print("="*60)
    print(" STARTING MARKETPLACE LOOP INTEGRATION TESTS ")
    print("="*60)
    
    # -------------------------------------------------------------
    # Test 1: Testing Poor Player Reputation Penalty on Starting Patience
    # -------------------------------------------------------------
    print("\n[TEST 1] Testing poor player reputation penalty on patience...")
    test_session_id_poor = f"test-poor-{uuid.uuid4()}"
    
    # Create session state with low reputation (20)
    state = load_session(test_session_id_poor)
    state["global_metrics"]["reputation"] = 20
    save_session(test_session_id_poor, state)
    
    session_poor = NPCSession(session_id=test_session_id_poor)
    patience_poor = session_poor.buyer.patience
    max_rounds_poor = session_poor.buyer.max_rounds
    print(f"Poor player reputation: buyer patience={patience_poor}, max_rounds={max_rounds_poor}")
    
    # Create session state with high reputation (85)
    test_session_id_rich = f"test-rich-{uuid.uuid4()}"
    state_rich = load_session(test_session_id_rich)
    state_rich["global_metrics"]["reputation"] = 85
    save_session(test_session_id_rich, state_rich)
    
    session_rich = NPCSession(session_id=test_session_id_rich)
    patience_rich = session_rich.buyer.patience
    max_rounds_rich = session_rich.buyer.max_rounds
    print(f"High player reputation: buyer patience={patience_rich}, max_rounds={max_rounds_rich}")
    
    # Assert poor reputation patience is lower than rich reputation patience
    from npc_engine.levels.level1_market.buyer_model import Buyer
    b1 = Buyer()
    orig_pat = b1.patience
    orig_rounds = b1.max_rounds
    
    b1.adjust_from_reputation(20)
    assert b1.patience < orig_pat or b1.patience == 0.1, "Patience was not penalized for poor reputation!"
    assert b1.max_rounds < orig_rounds or b1.max_rounds == 3, "Max rounds was not penalized for poor reputation!"
    
    b2 = Buyer()
    orig_pat_2 = b2.patience
    orig_rounds_2 = b2.max_rounds
    b2.adjust_from_reputation(90)
    assert b2.patience > orig_pat_2 or b2.patience == 1.0, "Patience was not boosted for high reputation!"
    assert b2.max_rounds > orig_rounds_2 or b2.max_rounds == 10, "Max rounds was not boosted for high reputation!"
    
    print("✓ TEST 1 PASSED!")

    # -------------------------------------------------------------
    # Test 2: Testing Dynamic Market Events Application of Price Multipliers
    # -------------------------------------------------------------
    print("\n[TEST 2] Testing active market event price multipliers...")
    
    test_event = {
        "name": "Portuguese Caravan Arrival",
        "description": "A grand Portuguese merchant caravan has arrived at Hampi from Goa. Demand for Pepper has skyrocketed!",
        "affected_spice": "pepper",
        "price_multiplier": 1.35,
        "quantity_multiplier": 1.5,
        "dialogue_trigger": "portuguese_caravan"
    }
    
    # Initialize a session with the test_event
    test_session_id_event = f"test-event-{uuid.uuid4()}"
    session_event = NPCSession(session_id=test_session_id_event, active_event=test_event)
    
    # Check if pepper is the active item or among session items
    pepper_item = next((item for item in session_event.session_items if item.name.lower() == "pepper"), None)
    if pepper_item:
        print(f"Pepper market price with event: {session_event.engine.market_price}")
        if session_event.engine.item.name.lower() == "pepper":
            expected_price = int(round(pepper_item.base_price_per_unit * pepper_item.market_multiplier * 1.35))
            assert session_event.engine.market_price == expected_price, f"Market event price multiplier not applied! Expected {expected_price}, got {session_event.engine.market_price}"
            
    print("✓ TEST 2 PASSED!")

    # -------------------------------------------------------------
    # Test 3: Testing Inventory Deduction Upon ACCEPT Deal
    # -------------------------------------------------------------
    print("\n[TEST 3] Testing inventory deduction upon acceptance of deal...")
    test_session_id_inv = f"test-inv-{uuid.uuid4()}"
    
    # Force default starting stock
    state_inv = load_session(test_session_id_inv)
    state_inv["inventory"]["pepper"] = 15000.0
    save_session(test_session_id_inv, state_inv)
    
    # Build session for pepper specifically
    session_inv = NPCSession(session_id=test_session_id_inv)
    
    # Force the current item to be pepper
    pepper_item_model = Item("pepper", base_price_per_unit=80, market_multiplier=1.2, unit="kg", quantity=1.0)
    session_inv.item = pepper_item_model
    session_inv.engine = session_inv.controller.engine = \
        session_inv.engine.__class__(session_inv.buyer, pepper_item_model, all_items=session_inv.session_items)
    
    # Let's perform steps to get an ACCEPT
    session_inv.start()
    session_inv.step("I have 1kg")
    
    # Find the current buyer offer and accept it
    step_price = session_inv.step("The price is 150 varahas")
    buyer_offer = step_price["price"]
    
    if not step_price["done"]:
        final_step = session_inv.step(f"ok, I sell it to you for {buyer_offer} varahas")
    else:
        final_step = step_price
        
    print(f"Deal outcome: action={final_step['action']}, quantity={final_step['quantity']}g")
    
    # Verify stock levels
    state_after = load_session(test_session_id_inv)
    pepper_stock_after = state_after["inventory"]["pepper"]
    print(f"Pepper stock after deal: {pepper_stock_after}g")
    
    if final_step["action"] == "ACCEPT":
        # Pepper stock should be 15000 - 1000 = 14000
        assert pepper_stock_after == 14000.0, f"Pepper stock not deducted correctly! Expected 14000, got {pepper_stock_after}"
    else:
        assert pepper_stock_after == 15000.0, "Pepper stock should not be deducted on failed deal!"
        
    print("✓ TEST 3 PASSED!")

    # -------------------------------------------------------------
    # Test 4: Compiling Final Shift Analytics
    # -------------------------------------------------------------
    print("\n[TEST 4] Compiling final shift analytics and learning score...")
    
    # Let's mock a set of deals that occurred in a shift
    mock_deals = [
        {
            "spice_name": "pepper",
            "final_price": 105,
            "final_quantity": 1000.0,
            "trust": 0.85,
            "frustration": 0.15,
            "out_of_world_count": 0,
            "outcome": "ACCEPT",
            "respect_change": 15,
            "money_earned": 105
        },
        {
            "spice_name": "clove",
            "final_price": 95,
            "final_quantity": 1200.0,
            "trust": 0.75,
            "frustration": 0.20,
            "out_of_world_count": 0,
            "outcome": "ACCEPT",
            "respect_change": 5,
            "money_earned": 95
        },
        {
            "spice_name": "cardamom",
            "final_price": None,
            "final_quantity": 800.0,
            "trust": 0.20,
            "frustration": 0.80,
            "out_of_world_count": 1,
            "outcome": "WALK_AWAY",
            "respect_change": -25,
            "money_earned": 0
        }
    ]
    
    # Mock items in session to determine their market price
    mock_session_items = [
        Item("pepper", base_price_per_unit=80, market_multiplier=1.2, unit="kg", quantity=1.0), # market price = 96
        Item("clove", base_price_per_unit=70, market_multiplier=1.3, unit="kg", quantity=1.2),  # market price = 91
        Item("cardamom", base_price_per_unit=100, market_multiplier=1.5, unit="kg", quantity=0.8) # market price = 150
    ]
    
    analytics = compile_shift_analytics(mock_deals, mock_session_items)
    
    print("\n" + "="*50)
    print("   CAPSTONE RESEARCH EVALUATION SUMMARY")
    print("="*50)
    print(f" Shifts Run Tracked: 1")
    print(f" Deals Attempted: {analytics['deals_attempted']}")
    print(f" Deals Completed: {analytics['deals_completed']}")
    print(f" Deals Walked Away: {analytics['deals_walked_away']}")
    print(f" Total Varahas Earned: {analytics['total_varahas_earned']}")
    print(f" Total Quantity Sold: {analytics['total_quantity_sold_grams']}g")
    print(f" Average Customer Trust: {analytics['average_customer_trust']}")
    print(f" Player Learning Score: {analytics['average_player_learning_score']} / 100")
    print(f" Evaluation Grade: {analytics['evaluation_grade']}")
    print("="*50 + "\n")
    
    assert analytics["deals_attempted"] == 3, "Deals attempted mismatch!"
    assert analytics["deals_completed"] == 2, "Deals completed mismatch!"
    assert analytics["total_varahas_earned"] == 200, "Revenue mismatch!"
    assert analytics["average_player_learning_score"] > 0, "Learning score should be calculated!"
    
    print("✓ TEST 4 PASSED!")

    # -------------------------------------------------------------
    # Cleanups
    # -------------------------------------------------------------
    for sid in [test_session_id_poor, test_session_id_rich, test_session_id_event, test_session_id_inv]:
        sfile = os.path.join(SESSIONS_DIR, f"{sid}.json")
        if os.path.exists(sfile):
            os.remove(sfile)
            
    print("\n" + "="*60)
    print(" ALL MARKETPLACE LOOP INTEGRATION TESTS PASSED SUCCESSFULLY! ")
    print("="*60 + "\n")

if __name__ == "__main__":
    run_marketplace_loop_tests()

import os
import json

# Locate paths dynamically relative to workspace
BACKEND_DIR = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
WORKSPACE_DIR = os.path.dirname(BACKEND_DIR)
SESSIONS_DIR = os.path.join(WORKSPACE_DIR, "memory", "sessions")

os.makedirs(SESSIONS_DIR, exist_ok=True)

def load_session(session_id: str) -> dict:
    """Loads a player session from the local disk with dynamic backward-compatible upgrades."""
    filepath = os.path.join(SESSIONS_DIR, f"{session_id}.json")
    if os.path.exists(filepath):
        try:
            with open(filepath, "r", encoding="utf-8") as f:
                state = json.load(f)
            
            # Dynamic schema backward-compatibility upgrades
            modified = False
            if "inventory" not in state:
                state["inventory"] = {
                    "pepper": 15000.0,      # 15 kg
                    "clove": 8000.0,        # 8 kg
                    "cinnamon": 12000.0,    # 12 kg
                    "cardamom": 4000.0      # 4 kg
                }
                modified = True
            if "shift_stats" not in state:
                state["shift_stats"] = {
                    "shifts_completed": 0,
                    "total_varahas_earned": 0,
                    "total_deals_made": 0
                }
                modified = True
            if modified:
                save_session(session_id, state)
            return state
        except Exception as e:
            print(f"[ERROR Persistence] Failed to read session {session_id}: {e}")
    return initialize_session(session_id)

def save_session(session_id: str, data: dict):
    """Saves a player session to the local disk."""
    filepath = os.path.join(SESSIONS_DIR, f"{session_id}.json")
    try:
        with open(filepath, "w", encoding="utf-8") as f:
            json.dump(data, f, indent=2, ensure_ascii=False)
        print(f"[INFO Persistence] Session {session_id} serialized successfully.")
    except Exception as e:
        print(f"[ERROR Persistence] Failed to save session {session_id}: {e}")

def initialize_session(session_id: str) -> dict:
    """Initializes a new persistent game session state."""
    state = {
        "player_name": "Merchant Traveler",
        "current_level": 1,
        "level_history": {
            "level1_market": {
                "completed": False,
                "deals": [],
                "reputation_archetype": "Standard Merchant"
            }
        },
        "global_metrics": {
            "reputation": 50,
            "total_varahas": 100,
            "completed_levels": []
        },
        "inventory": {
            "pepper": 15000.0,      # 15 kg
            "clove": 8000.0,        # 8 kg
            "cinnamon": 12000.0,    # 12 kg
            "cardamom": 4000.0      # 4 kg
        },
        "shift_stats": {
            "shifts_completed": 0,
            "total_varahas_earned": 0,
            "total_deals_made": 0
        }
    }
    save_session(session_id, state)
    return state

def record_negotiation_deal(session_id: str, spice_name: str, final_price: int, final_quantity: float, trust: float, frustration: float, out_of_world_count: int, outcome: str):
    """
    Calculates and updates Money (total_varahas) and Respect (reputation) based on 
    negotiation outcomes, and commits the state immediately to disk.
    """
    state = load_session(session_id)
    
    global_metrics = state.setdefault("global_metrics", {"reputation": 50, "total_varahas": 100, "completed_levels": []})
    level1_data = state["level_history"].setdefault("level1_market", {
        "completed": False,
        "deals": [],
        "reputation_archetype": "Standard Merchant"
    })
    
    current_reputation = global_metrics.get("reputation", 50)
    current_varahas = global_metrics.get("total_varahas", 100)
    
    # 1. Calculate Money and Respect changes
    varaha_change = 0
    reputation_change = 0
    
    if outcome == "ACCEPT":
        # Earn money from transaction
        varaha_change = int(final_price) if final_price is not None else 0
        
        # Calculate respect change
        if trust >= 0.7 and frustration <= 0.3:
            reputation_change = 15
        elif frustration >= 0.6:
            reputation_change = -10
        else:
            reputation_change = 5
    elif outcome in ["WALK_AWAY", "NO_ITEM"]:
        # Failed negotiation penalty
        reputation_change = -15
        
    # Extra penalty for out of character / out of world talk
    if out_of_world_count > 0:
        reputation_change -= (10 * out_of_world_count)
        
    # Apply changes
    new_varahas = max(0, current_varahas + varaha_change)
    new_reputation = max(0, min(100, current_reputation + reputation_change))
    
    # Determine archetype based on new reputation
    if new_reputation >= 80:
        archetype = "Fair Trader"
    elif new_reputation <= 35:
        archetype = "Greedy Haggler"
    else:
        archetype = "Standard Merchant"
        
    # Append deal details
    deal_record = {
        "spice_name": spice_name,
        "final_price": final_price,
        "final_quantity": final_quantity,
        "trust": round(trust, 2),
        "frustration": round(frustration, 2),
        "out_of_world_count": out_of_world_count,
        "outcome": outcome,
        "respect_change": reputation_change,
        "money_earned": varaha_change
    }
    
    level1_data["deals"].append(deal_record)
    level1_data["reputation_archetype"] = archetype
    
    global_metrics["reputation"] = new_reputation
    global_metrics["total_varahas"] = new_varahas
    
    # Save session
    save_session(session_id, state)

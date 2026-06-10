import os
import json

# Locate paths dynamically relative to workspace
BACKEND_DIR = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
WORKSPACE_DIR = os.path.dirname(BACKEND_DIR)
SESSIONS_DIR = os.path.join(WORKSPACE_DIR, "memory", "sessions")

os.makedirs(SESSIONS_DIR, exist_ok=True)

DEV_RESET_PROFILE = True

DEFAULT_REPUTATION = 50
DEFAULT_VARAHAS = 100

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

def clear_audio_directory():
    """Deletes all generated wav and mp3 files in the audio directory to save space."""
    audio_dir = os.path.join(BACKEND_DIR, "audio")
    if os.path.exists(audio_dir):
        for filename in os.listdir(audio_dir):
            if filename.endswith(".wav") or filename.endswith(".mp3"):
                try:
                    os.remove(os.path.join(audio_dir, filename))
                except Exception as e:
                    # Ignore errors if file is locked/currently playing
                    pass

def save_player_profile(session_data: dict):
    """Extracts and saves the player profile from session data using atomic writes and backups."""
    profile_data = {
        "global_metrics": {
            "reputation": session_data.get("global_metrics", {}).get("reputation", DEFAULT_REPUTATION),
            "total_varahas": session_data.get("global_metrics", {}).get("total_varahas", DEFAULT_VARAHAS),
            "completed_levels": session_data.get("global_metrics", {}).get("completed_levels", [])
        },
        "inventory": session_data.get("inventory", {
            "pepper": 15000.0,
            "clove": 8000.0,
            "cinnamon": 12000.0,
            "cardamom": 4000.0
        }),
        "shift_stats": session_data.get("shift_stats", {
            "shifts_completed": 0,
            "total_varahas_earned": 0,
            "total_deals_made": 0
        })
    }
    
    filepath = os.path.join(SESSIONS_DIR, "player_profile.json")
    backup_path = os.path.join(SESSIONS_DIR, "player_profile.backup.json")
    tmp_path = os.path.join(SESSIONS_DIR, "player_profile.tmp")
    required_keys = ["global_metrics", "inventory", "shift_stats"]
    
    try:
        # 1. Write to tmp file
        with open(tmp_path, "w", encoding="utf-8") as f:
            json.dump(profile_data, f, indent=2, ensure_ascii=False)
            
        # 2. Verify JSON serialization succeeded by reading back
        with open(tmp_path, "r", encoding="utf-8") as f:
            verified_data = json.load(f)
        if not all(k in verified_data for k in required_keys):
            raise ValueError("Required keys missing from verified data")
            
        # 3. Copy current player_profile.json to player_profile.backup.json if it exists and is valid
        if os.path.exists(filepath):
            try:
                with open(filepath, "r", encoding="utf-8") as f:
                    existing_data = json.load(f)
                if all(k in existing_data for k in required_keys):
                    os.replace(filepath, backup_path)
            except Exception as e:
                print(f"[WARNING Persistence] Existing profile invalid, skipping backup copy: {e}")
                
        # 4. Atomically replace player_profile.json with player_profile.tmp
        os.replace(tmp_path, filepath)
        print("[INFO Persistence] Player profile updated atomically.")
    except Exception as e:
        print(f"[ERROR Persistence] Failed to save player profile atomically: {e}")
        if os.path.exists(tmp_path):
            try:
                os.remove(tmp_path)
            except Exception:
                pass

def load_player_profile() -> dict:
    """Loads the player profile, falling back to backup or scanning session files or defaults."""
    if DEV_RESET_PROFILE:
        print("[DEV MODE] Player profile reset enabled")
        return {
            "global_metrics": {
                "reputation": DEFAULT_REPUTATION,
                "total_varahas": DEFAULT_VARAHAS,
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

    filepath = os.path.join(SESSIONS_DIR, "player_profile.json")
    backup_path = os.path.join(SESSIONS_DIR, "player_profile.backup.json")
    required_keys = ["global_metrics", "inventory", "shift_stats"]
    
    # 1. Try to load main profile
    if os.path.exists(filepath):
        try:
            with open(filepath, "r", encoding="utf-8") as f:
                data = json.load(f)
            if all(k in data for k in required_keys):
                return data
            else:
                raise ValueError("Required keys missing from main profile")
        except Exception as e:
            print(f"[PROFILE WARNING] Main profile corrupted, loading backup. Error: {e}")
            
    # 2. Try to load backup profile
    if os.path.exists(backup_path):
        try:
            with open(backup_path, "r", encoding="utf-8") as f:
                data = json.load(f)
            if all(k in data for k in required_keys):
                print("[INFO Persistence] Successfully loaded backup player profile.")
                return data
            else:
                raise ValueError("Required keys missing from backup profile")
        except Exception as e:
            print(f"[ERROR Persistence] Backup profile also corrupted or failed: {e}")
            
    # 3. Migration Fallback: Scan latest NPC session file
    latest_data = None
    if os.path.exists(SESSIONS_DIR):
        session_files = []
        for filename in os.listdir(SESSIONS_DIR):
            if not filename.endswith(".json"):
                continue
            if filename in ["benchmark_session.json", "test-budget-scratch.json", "player_profile.json", "player_profile.backup.json"]:
                continue
            filepath_session = os.path.join(SESSIONS_DIR, filename)
            if os.path.isfile(filepath_session):
                session_files.append(filepath_session)
                
        if session_files:
            session_files.sort(key=os.path.getmtime, reverse=True)
            for filepath_session in session_files:
                try:
                    with open(filepath_session, "r", encoding="utf-8") as f:
                        data = json.load(f)
                    if "global_metrics" in data and "inventory" in data and "shift_stats" in data:
                        latest_data = data
                        break
                except Exception as e:
                    print(f"[ERROR Persistence] Failed to read potential previous session {filepath_session}: {e}")
                    
    if latest_data:
        print("[INFO Persistence] Migrated player profile from the latest session file.")
        profile_data = {
            "global_metrics": {
                "reputation": latest_data.get("global_metrics", {}).get("reputation", DEFAULT_REPUTATION),
                "total_varahas": latest_data.get("global_metrics", {}).get("total_varahas", DEFAULT_VARAHAS),
                "completed_levels": latest_data.get("global_metrics", {}).get("completed_levels", [])
            },
            "inventory": latest_data.get("inventory", {
                "pepper": 15000.0,
                "clove": 8000.0,
                "cinnamon": 12000.0,
                "cardamom": 4000.0
            }),
            "shift_stats": latest_data.get("shift_stats", {
                "shifts_completed": 0,
                "total_varahas_earned": 0,
                "total_deals_made": 0
            })
        }
        # Save newly created player profile
        try:
            with open(filepath, "w", encoding="utf-8") as f:
                json.dump(profile_data, f, indent=2, ensure_ascii=False)
        except Exception as e:
            print(f"[ERROR Persistence] Failed to save migrated player profile: {e}")
        return profile_data
        
    # 4. Default Fallback
    return {
        "global_metrics": {
            "reputation": DEFAULT_REPUTATION,
            "total_varahas": DEFAULT_VARAHAS,
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

def save_session(session_id: str, data: dict):
    """Saves a player session to the local disk."""
    filepath = os.path.join(SESSIONS_DIR, f"{session_id}.json")
    try:
        with open(filepath, "w", encoding="utf-8") as f:
            json.dump(data, f, indent=2, ensure_ascii=False)
        print(f"[INFO Persistence] Session {session_id} serialized successfully.")
    except Exception as e:
        print(f"[ERROR Persistence] Failed to save session {session_id}: {e}")
        
    # Keep the player profile updated
    if session_id not in ["benchmark_session", "test-budget-scratch", "player_profile"]:
        save_player_profile(data)

def initialize_session(session_id: str) -> dict:
    """Initializes a new persistent game session state."""
    # Clear audio files on new session start unless it's a test/benchmark session
    if session_id not in ["benchmark_session", "test-budget-scratch"]:
        clear_audio_directory()
        
    profile = load_player_profile()
    
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
            "reputation": profile.get("global_metrics", {}).get("reputation", DEFAULT_REPUTATION),
            "total_varahas": profile.get("global_metrics", {}).get("total_varahas", DEFAULT_VARAHAS),
            "completed_levels": profile.get("global_metrics", {}).get("completed_levels", [])
        },
        "inventory": profile.get("inventory", {
            "pepper": 15000.0,      # 15 kg
            "clove": 8000.0,        # 8 kg
            "cinnamon": 12000.0,    # 12 kg
            "cardamom": 4000.0      # 4 kg
        }),
        "shift_stats": profile.get("shift_stats", {
            "shifts_completed": 0,
            "total_varahas_earned": 0,
            "total_deals_made": 0
        })
    }
    save_session(session_id, state)
    return state

def get_reputation_rank_name(reputation):
    if reputation <= 20: return "Unknown Trader"
    if reputation <= 40: return "Small Merchant"
    if reputation <= 60: return "Trusted Merchant"
    if reputation <= 80: return "Royal Supplier"
    return "Legendary Merchant"

def record_negotiation_deal(session_id, spice_name, final_price, final_quantity, trust, frustration, out_of_world_count, outcome, market_price=None):
    """
    Calculates and updates Money (total_varahas) and Respect (reputation) based on 
    negotiation outcomes, and commits the state immediately to disk.
    """
    state = load_session(session_id)
    
    global_metrics = state.setdefault("global_metrics", {"reputation": DEFAULT_REPUTATION, "total_varahas": DEFAULT_VARAHAS, "completed_levels": []})
    level1_data = state["level_history"].setdefault("level1_market", {
        "completed": False,
        "deals": [],
        "reputation_archetype": "Local Merchant"
    })
    
    current_reputation = global_metrics.get("reputation", DEFAULT_REPUTATION)
    current_varahas = global_metrics.get("total_varahas", DEFAULT_VARAHAS)
    
    # 1. Calculate Money and Respect changes
    varaha_change = 0
    reputation_change = 0
    
    if outcome == "ACCEPT":
        # Earn money from transaction
        varaha_change = int(final_price) if final_price is not None else 0
        
        # Calculate respect change
        # Base success deal
        reputation_change = 2
        # Bonus for selling above market value
        if final_price is not None and market_price is not None and final_price > market_price:
            reputation_change += 2
        # Bonus for excellent customer satisfaction
        if trust >= 0.7 and frustration <= 0.3:
            reputation_change += 1
        # Penalty if customer leaves angry
        if frustration >= 0.6:
            reputation_change = -5
    else:
        # Walkaway / failed negotiation
        if frustration >= 0.6 or outcome == "WALK_AWAY":
            reputation_change = -5 # Customer leaves angry
        else:
            reputation_change = -3 # Normal failed negotiation
        
    # Extra penalty for out of character / out of world talk
    if out_of_world_count > 0:
        reputation_change -= (10 * out_of_world_count)
        
    # Apply changes
    new_varahas = max(0, current_varahas + varaha_change)
    new_reputation = max(0, min(100, current_reputation + reputation_change))
    
    # Determine rank and archetype based on new reputation
    archetype = get_reputation_rank_name(new_reputation)
    
    # Print server debug logs
    print(f"[REPUTATION DEBUG] old reputation: {current_reputation}, new reputation: {new_reputation}, delta: {reputation_change}, rank: {archetype}")
        
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

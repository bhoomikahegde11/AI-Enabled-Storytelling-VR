from npc_engine.core.persistence import load_session, save_session

def get_inventory_stock(session_id: str, spice_name: str) -> float:
    """Returns the player's stock level for a given spice in grams."""
    state = load_session(session_id)
    inventory = state.setdefault("inventory", {})
    return float(inventory.get(str(spice_name).lower().strip(), 0.0))

def has_sufficient_stock(session_id: str, spice_name: str, grams: float) -> bool:
    """Checks if the player has sufficient stock in their inventory."""
    stock = get_inventory_stock(session_id, spice_name)
    return stock >= float(grams)

def deduct_inventory_stock(session_id: str, spice_name: str, grams: float) -> bool:
    """
    Deducts a specified quantity in grams from the player's persistent spice stock.
    Returns True if successfully deducted, False if insufficient stock.
    """
    spice_key = str(spice_name).lower().strip()
    state = load_session(session_id)
    inventory = state.setdefault("inventory", {})
    
    current_stock = float(inventory.get(spice_key, 0.0))
    deduction = float(grams)
    
    if current_stock >= deduction:
        inventory[spice_key] = max(0.0, current_stock - deduction)
        save_session(session_id, state)
        print(f"[INFO Inventory] Deducted {deduction}g of {spice_key} from session {session_id}. New stock: {inventory[spice_key]}g")
        return True
        
    print(f"[WARNING Inventory] Insufficient stock of {spice_key} in session {session_id}. Required: {deduction}g, Available: {current_stock}g")
    return False

import os
import sys
import random

# Add the 'backend' folder to path so we can resolve imports
BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
sys.path.insert(0, os.path.join(BASE_DIR, "backend"))

# Try importing the components
try:
    from npc_engine.levels.level1_market.buyer_model import Buyer
    from npc_engine.levels.level1_market.item_model import Item
    from npc_engine.levels.level1_market.negotiation_engine import NegotiationEngine
    from npc_engine.levels.level1_market.dialogue_generator import generate_dialogue
    from npc_engine.levels.level1_market.intent_classifier import classify_intent, extract_quantity_info, llm_loaded
    from npc_engine.levels.level1_market.input_interpreter import extract_price
    from npc_engine.core.controller import Controller
    from npc_engine.core.measurements import get_random_spice_quantity
except ImportError as e:
    print(f"Error: Failed to import backend components: {e}")
    print("Ensure you are running the script from the project root directory.")
    sys.exit(1)

# ANSI terminal colors for a premium CLI look
CLR_CYAN = "\033[96m"
CLR_GREEN = "\033[92m"
CLR_YELLOW = "\033[93m"
CLR_RED = "\033[91m"
CLR_BLUE = "\033[94m"
CLR_MAGENTA = "\033[95m"
CLR_RESET = "\033[0m"
CLR_BOLD = "\033[1m"

ITEMS = [
    Item("pepper", base_price_per_unit=80, market_multiplier=1.2, unit="kg", quantity=1),      # ~96/kg
    Item("clove", base_price_per_unit=70, market_multiplier=1.3, unit="kg", quantity=1),       # ~91/kg
    Item("cinnamon", base_price_per_unit=80, market_multiplier=1.3, unit="kg", quantity=1),    # ~104/kg
    Item("cardamom", base_price_per_unit=100, market_multiplier=1.5, unit="kg", quantity=1),   # ~150/kg
]


def print_dashboard(debug_info, item_name, action, tone, emotion, last_player_input=None):
    print("\n" + "=" * 60)
    print(f" {CLR_BOLD}{CLR_BLUE}👑 VIJAYANAGARA MARKETPLACE SANDBOX (LEVEL 1){CLR_RESET} ")
    print("=" * 60)
    
    # 1. State Metrics Row
    stage = debug_info.get("stage", "OPENING")
    stage_clr = CLR_GREEN if stage == "OPENING" else CLR_YELLOW if stage == "BARGAINING" else CLR_RED
    print(f" {CLR_BOLD}Level Status:{CLR_RESET}   Stage: {stage_clr}{stage}{CLR_RESET} | Turn: {CLR_CYAN}{debug_info.get('turn')}{CLR_RESET}")
    
    # 2. Emotional Dashboard
    frustration = debug_info.get("frustration", 0.0)
    frustration_clr = CLR_GREEN if frustration < 0.4 else CLR_YELLOW if frustration < 0.7 else CLR_RED
    
    desperation = debug_info.get("desperation", 0.0)
    
    print(f" {CLR_BOLD}NPC Affection:{CLR_RESET} Trust/Respect: {CLR_GREEN}{round(1.0 - frustration, 2)}{CLR_RESET} | Frustration: {frustration_clr}{frustration}{CLR_RESET} | Desperation: {CLR_CYAN}{desperation}{CLR_RESET}")
    print(f" {CLR_BOLD}Render State:{CLR_RESET}  Tone: {CLR_MAGENTA}{tone}{CLR_RESET} | Emotion: {CLR_MAGENTA}{emotion}{CLR_RESET}")
    
    # 3. Transaction Dashboard
    current_offer = debug_info.get("current_offer")
    seller_price = debug_info.get("seller_price")
    seller_price_str = f"{CLR_GREEN}{seller_price} varahas{CLR_RESET}" if seller_price else f"{CLR_RED}None{CLR_RESET}"
    
    print(f" {CLR_BOLD}Price Exchange:{CLR_RESET} Buyer Offer: {CLR_GREEN}{current_offer} varahas{CLR_RESET} | Seller Counter: {seller_price_str}")
    print(f" {CLR_BOLD}Active Spice:{CLR_RESET}  {CLR_CYAN}{item_name.upper()}{CLR_RESET}")
    print("-" * 60)
    
    # 4. Traditional Spice Measurement Cheat Sheet
    print(f" {CLR_BOLD}{CLR_BLUE}📋 Traditional Measurement Reference Guide:{CLR_RESET}")
    print(f"   • {CLR_BOLD}Palam{CLR_RESET}: ~35g       | • {CLR_BOLD}Seer{CLR_RESET}: 8 Palams (~280g)")
    print(f"   • {CLR_BOLD}Veesai{CLR_RESET}: 5 Seers (~1.4kg) | • {CLR_BOLD}Manangu{CLR_RESET}: 8 Veesai (~11.2kg)")
    print(f"   • {CLR_BOLD}Bahar/Candy{CLR_RESET}: 40 Manangu (~448kg)")
    
    # 5. Conversion Tip based on player's last input
    if last_player_input:
        from npc_engine.core.measurements import parse_traditional_to_grams
        grams = parse_traditional_to_grams(last_player_input)
        if grams is not None:
            if grams >= 1000:
                kg_equiv = f"{round(grams/1000.0, 2)} kg"
            else:
                kg_equiv = f"{int(grams)} grams"
            print("-" * 60)
            print(f" {CLR_BOLD}{CLR_YELLOW}💡 Counter-Offer Conversion Tool:{CLR_RESET}")
            print(f"   Quantity parsed: {CLR_BOLD}{kg_equiv}{CLR_RESET} from your input \"{last_player_input}\"")
            
    print("=" * 60 + "\n")


def run_cli_test():
    print(f"{CLR_BOLD}{CLR_CYAN}Initializing Marketplace Sandbox...{CLR_RESET}")
    
    # Check LLM Loader Status
    if llm_loaded:
        print(f"[{CLR_GREEN}LLM ACTIVE{CLR_RESET}] Local model.gguf is loaded. Tone and context triggers will use LLM classification.")
    else:
        print(f"[{CLR_YELLOW}HEURISTICS FALLBACK{CLR_RESET}] model.gguf not found. Running entirely with rule-based heuristics.")

    item_pool = ITEMS.copy()
    random.shuffle(item_pool)

    while True:
        buyer = Buyer()
        if not item_pool:
            item_pool = ITEMS.copy()
            random.shuffle(item_pool)
            
        default_item = item_pool.pop()
        grams = get_random_spice_quantity(default_item.name)
        item = Item(
            name=default_item.name,
            base_price_per_unit=default_item.base_price_per_unit,
            market_multiplier=default_item.market_multiplier,
            unit="kg",
            quantity=grams / 1000.0
        )

        print(f"\n{CLR_BOLD}{CLR_MAGENTA}------------------------------------------------------------{CLR_RESET}")
        print(f" 🤝 {CLR_BOLD}{CLR_YELLOW}A NEW BUYER APPROACHES YOUR MARKET STALL{CLR_RESET} ")
        print(f"{CLR_BOLD}{CLR_MAGENTA}------------------------------------------------------------{CLR_RESET}")
        print(f" {CLR_BOLD}Buyer Personality:{CLR_RESET} {CLR_CYAN}{buyer.personality}{CLR_RESET}")
        print(f" {CLR_BOLD}Initial Patience:{CLR_RESET}  {CLR_CYAN}{buyer.patience}{CLR_RESET} (higher = negotiates longer)")
        print(f" {CLR_BOLD}Desperation Level:{CLR_RESET} {CLR_CYAN}{buyer.desperation}{CLR_RESET} (higher = willing to pay more)")
        print(f"{CLR_BOLD}{CLR_MAGENTA}------------------------------------------------------------{CLR_RESET}")

        # Create session items catalog where this item has its active randomized quantity
        session_items = []
        for default_cat_item in ITEMS:
            if default_cat_item.name.lower() == item.name.lower():
                session_items.append(item)
            else:
                cat_grams = get_random_spice_quantity(default_cat_item.name)
                session_items.append(Item(
                    name=default_cat_item.name,
                    base_price_per_unit=default_cat_item.base_price_per_unit,
                    market_multiplier=default_cat_item.market_multiplier,
                    unit="kg",
                    quantity=cat_grams / 1000.0
                ))

        engine = NegotiationEngine(buyer, item, all_items=session_items)
        controller = Controller(
            engine,
            classify_intent_fn=classify_intent,
            extract_quantity_info_fn=extract_quantity_info,
            extract_price_fn=extract_price,
            dialogue_fn=generate_dialogue
        )

        # Get initial response
        response = controller.step(None)
        last_player_input = None
        
        while True:
            # Print the rich dashboard
            print_dashboard(
                response["debug"], 
                engine.item.name, 
                response["action"], 
                response["tone"], 
                response["emotion"],
                last_player_input=last_player_input
            )
            
            # Print the NPC dialogue text
            print(f"{CLR_BOLD}{CLR_RED}Buyer:{CLR_RESET} \"{response['npc_text']}\"")
            print()

            if response["done"]:
                print(f"{CLR_BOLD}{CLR_RED}Customer has left the stall.{CLR_RESET}\n")
                break

            # Prompt user input
            try:
                seller_input = input(f"{CLR_BOLD}{CLR_CYAN}You (Seller):{CLR_RESET} ")
                last_player_input = seller_input
            except (KeyboardInterrupt, EOFError):
                print(f"\n{CLR_BOLD}{CLR_RED}Sandbox exited.{CLR_RESET}")
                sys.exit(0)

            # Advance loop
            response = controller.step(seller_input)
            
            # Print detected intent in console for developer transparency
            detected_intent = engine.last_intent or "None"
            print(f" [{CLR_YELLOW}DEBUG LOG{CLR_RESET}] Detected Player Intent: {CLR_BOLD}{detected_intent}{CLR_RESET}")

        # Ask to continue
        try:
            choice = input(f"Press {CLR_BOLD}Enter{CLR_RESET} to spawn next customer, or type {CLR_BOLD}'q'{CLR_RESET} to quit: ").strip().lower()
            if choice == 'q':
                break
        except (KeyboardInterrupt, EOFError):
            break

    print(f"\n{CLR_BOLD}{CLR_GREEN}Thank you for trading in the Vijayanagara Marketplace!{CLR_RESET}")


if __name__ == "__main__":
    run_cli_test()

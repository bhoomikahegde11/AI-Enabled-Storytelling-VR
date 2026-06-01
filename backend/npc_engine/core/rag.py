import os
import json

class RAGRetriever:
    """
    Retrieves high-fidelity, historical context facts from the Vijayanagara Empire fact sheets
    to ground dialogue rephrasing in true 1500s terms.
    """
    def __init__(self):
        self.facts = {}
        # Locate the workspace directory
        base_dir = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
        self.facts_path = os.path.join(base_dir, "knowledge", "vijayanagara_facts.json")
        
        try:
            if os.path.exists(self.facts_path):
                with open(self.facts_path, "r", encoding="utf-8") as f:
                    self.facts = json.load(f)
                print("[INFO RAG] Historical fact sheet loaded successfully.")
            else:
                print(f"[WARNING RAG] Fact sheet file not found at: {self.facts_path}")
        except Exception as e:
            print(f"[ERROR RAG] Failed to initialize RAG fact loader: {e}")

    def retrieve_context(self, item_name: str, stage: str = None) -> str:
        """
        Keyword-based retrieval that fetches a maximum of two highly relevant fact snippets
        based on active spice name and current session stage.
        """
        if not self.facts:
            return ""

        context_snippets = []
        item_lower = item_name.lower()

        # 1. Match specific spice exports and context
        if "pepper" in item_lower:
            context_snippets.append("Pepper is imported from Malabar. Under treaties with Emperor Krishnadevaraya, the Portuguese held monopolistic rights on importing war horses in exchange for spice trade access.")
        elif "clove" in item_lower or "cinnamon" in item_lower or "cardamom" in item_lower:
            context_snippets.append(f"{item_name.capitalize()} is a premium spice exported through temple-adjacent Hampi bazaars, highly demanded by Persian and European merchants.")

        # 2. Add Hampi Bazaar location details
        bazaars = self.facts.get("trade_dynamics", {}).get("bazaar_locations", [])
        if bazaars:
            selected_bazaar = bazaars[0]  # Default to Virupaksha Bazaar
            if "clove" in item_lower:
                selected_bazaar = bazaars[1] if len(bazaars) > 1 else selected_bazaar # Krishna Bazaar
            context_snippets.append(f"Negotiations take place in the busy {selected_bazaar} in Hampi.")

        # 3. Add coin denomination insight
        monetary = self.facts.get("monetary_system", {})
        currency_name = monetary.get("currency_name", "Varaha")
        context_snippets.append(f"Standard imperial currency is the gold {currency_name}. Smaller silver tara or copper coins are used for fractional weights.")

        # Return a unified, grounded context paragraph
        return " ".join(context_snippets[:2])

import random


class Buyer:
    def __init__(self):
        self.reputation = 50.0
        self.personality = random.choice([
            "Aggressive Trader",
            "Cautious Buyer",
            "Polite Merchant"
        ])

        if self.personality == "Aggressive Trader":
            self.desperation = random.uniform(0.5, 0.9)
            self.patience = random.uniform(0.3, 0.6)
            self.politeness = random.uniform(0.2, 0.4)

        elif self.personality == "Cautious Buyer":
            self.desperation = random.uniform(0.3, 0.6)
            self.patience = random.uniform(0.6, 0.9)
            self.politeness = random.uniform(0.4, 0.7)

        else:
            self.desperation = random.uniform(0.4, 0.7)
            self.patience = random.uniform(0.5, 0.8)
            self.politeness = random.uniform(0.7, 0.95)

        self.max_rounds = int(4 + self.patience * 6)
        
        # Dynamic historically authentic NPC Character Identities
        self.identity = random.choice([
            {"name": "Abdul Rahman", "origin": "Persian Spice Merchant", "interest": "pepper", "wealth": "High"},
            {"name": "Francisco de Almeida", "origin": "Portuguese Trade Agent", "interest": "cinnamon", "wealth": "Very High"},
            {"name": "Chinappa Naik", "origin": "Vijayanagara Wholesale Buyer", "interest": "clove", "wealth": "Medium"},
            {"name": "Siddharth Chetti", "origin": "Local Retail Shopkeeper", "interest": "cardamom", "wealth": "Medium"},
            {"name": "Father Penteado", "origin": "Jesuit Missionary", "interest": "cinnamon", "wealth": "Low"},
        ])
        self.name = self.identity["name"]
        self.origin = self.identity["origin"]
        self.interest = self.identity["interest"]
        self.wealth = self.identity["wealth"]

    def compute_max_price(self, market_price):
        return market_price * (1 + 0.25 * self.desperation)

    def initial_offer(self, market_price):
        return round(market_price * random.uniform(0.6, 0.75))

    def adjust_from_reputation(self, reputation):
        """
        Dynamically adjusts starting patience and initial respect thresholds 
        based on the persistent player reputation loaded from disk memory.
        """
        reputation = float(reputation)
        self.reputation = reputation
        if reputation < 35:
            # Player is known as a Greedy Haggler: buyer starts very impatient and frustrated
            self.patience = max(0.1, self.patience - 0.25)
            self.max_rounds = max(3, self.max_rounds - 2)
            print(f"[INFO Reputation] Buyer {self.name} adjusted starting patience down due to player poor reputation: {reputation}")
        elif reputation > 75:
            # Player is known as a Fair Trader: buyer starts highly patient and respectful
            self.patience = min(1.0, self.patience + 0.20)
            self.max_rounds = min(10, self.max_rounds + 2)
            print(f"[INFO Reputation] Buyer {self.name} adjusted starting patience up due to player great reputation: {reputation}")
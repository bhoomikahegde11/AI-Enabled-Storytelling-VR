import random


class Buyer:
    def __init__(self, reputation=50):
        # 1. Define player reputation (controls buyer wealth class spawn weights)
        self.reputation = float(reputation)
        
        if self.reputation <= 40:
            # Low reputation spawn weights: Cheap 60%, Normal 35%, Rich 5%
            weights = [0.60, 0.35, 0.05]
        elif self.reputation <= 70:
            # Medium reputation spawn weights: Cheap 25%, Normal 55%, Rich 20%
            weights = [0.25, 0.55, 0.20]
        else:
            # High reputation spawn weights: Cheap 5%, Normal 35%, Rich 60%
            weights = [0.05, 0.35, 0.60]
            
        wealth_type = random.choices(["Cheap", "Normal", "Rich"], weights=weights, k=1)[0]
        
        if wealth_type == "Cheap":
            self.wealth = "Low"
        elif wealth_type == "Normal":
            self.wealth = "Medium"
        else: # Rich
            self.wealth = random.choice(["High", "Very High"])

        # 2. Select personality & properties
        self.personality = random.choice([
            "friendly",
            "strict",
            "wealthy trader",
            "impatient",
            "curious traveler"
        ])

        if self.personality == "strict":
            self.desperation = random.uniform(0.3, 0.6)
            self.patience = random.uniform(0.3, 0.5)
            self.politeness = random.uniform(0.2, 0.4)
        elif self.personality == "friendly":
            self.desperation = random.uniform(0.4, 0.7)
            self.patience = random.uniform(0.6, 0.9)
            self.politeness = random.uniform(0.7, 0.95)
        elif self.personality == "wealthy trader":
            self.desperation = random.uniform(0.5, 0.8)
            self.patience = random.uniform(0.5, 0.8)
            self.politeness = random.uniform(0.6, 0.9)
        elif self.personality == "impatient":
            self.desperation = random.uniform(0.5, 0.9)
            self.patience = random.uniform(0.2, 0.4)
            self.politeness = random.uniform(0.3, 0.5)
        else: # curious traveler
            self.desperation = random.uniform(0.3, 0.6)
            self.patience = random.uniform(0.7, 0.95)
            self.politeness = random.uniform(0.7, 0.9)

        # 3. Apply wealth effects on patience
        if self.wealth == "Low": # Cheap buyer
            self.patience = max(0.1, self.patience * 0.75)
        elif self.wealth == "Medium": # Normal buyer
            pass
        else: # Rich buyer
            self.patience = min(1.0, self.patience * 1.20)

        # 4. Set max rounds based on patience
        self.max_rounds = int(4 + self.patience * 6)
        
        # 5. Select identity independently of wealth
        identities = [
            {"name": "Abdul Rahman", "origin": "Persian Spice Merchant", "interest": "pepper"},
            {"name": "Francisco de Almeida", "origin": "Portuguese Trade Agent", "interest": "cinnamon"},
            {"name": "Chinappa Naik", "origin": "Vijayanagara Wholesale Buyer", "interest": "clove"},
            {"name": "Siddharth Chetti", "origin": "Local Retail Shopkeeper", "interest": "cardamom"},
            {"name": "Father Penteado", "origin": "Jesuit Missionary", "interest": "cinnamon"},
        ]
        self.identity = random.choice(identities)
        self.name = self.identity["name"]
        self.origin = self.identity["origin"]
        self.interest = self.identity["interest"]

    def compute_max_price(self, market_price):
        if self.wealth == "Low": # Cheap buyer
            # max offer: 90-100% of market value
            multiplier = random.uniform(0.9, 1.0)
        elif self.wealth == "Medium": # Normal buyer
            # max offer: 110-125% of market value
            multiplier = random.uniform(1.1, 1.25)
        else: # Rich buyer
            # max offer: 130-160% of market value
            multiplier = random.uniform(1.3, 1.6)
        return int(round(market_price * multiplier))

    def initial_offer(self, market_price):
        if self.wealth == "Low": # Cheap buyer
            # starting offer: 60-80% of market value
            multiplier = random.uniform(0.6, 0.8)
        elif self.wealth == "Medium": # Normal buyer
            # starting offer: 75-90% of market value
            multiplier = random.uniform(0.75, 0.9)
        else: # Rich buyer
            # starting offer: 90-110% of market value
            multiplier = random.uniform(0.9, 1.1)
        return int(round(market_price * multiplier))

    def adjust_from_reputation(self, reputation):
        """
        Dynamically adjusts starting patience and initial respect thresholds 
        based on the persistent player reputation loaded from disk memory.
        """
        reputation = float(reputation)
        self.reputation = reputation
        if reputation < 26:
            # Poor reputation: buyer starts very impatient and frustrated
            self.patience = max(0.1, self.patience - 0.25)
            self.max_rounds = max(3, self.max_rounds - 2)
            print(f"[INFO Reputation] Buyer {self.name} adjusted starting patience down due to player poor reputation: {reputation}")
        elif reputation > 75:
            # Great reputation: buyer starts highly patient and respectful
            self.patience = min(1.0, self.patience + 0.20)
            self.max_rounds = min(10, self.max_rounds + 2)
            print(f"[INFO Reputation] Buyer {self.name} adjusted starting patience up due to player great reputation: {reputation}")
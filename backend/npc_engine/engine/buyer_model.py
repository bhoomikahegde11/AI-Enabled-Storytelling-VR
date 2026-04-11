import random


class Buyer:
    def __init__(self):
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

    def compute_max_price(self, market_price):
        return market_price * (1 + 0.25 * self.desperation)

    # 🔥 FIX: Lower initial offer (more realistic bargaining)
    def initial_offer(self, market_price):
        return round(market_price * random.uniform(0.6, 0.75))
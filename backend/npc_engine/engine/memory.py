class Memory:
    def __init__(self):
        self.last_deal_price = None
        self.interactions = 0

    def update(self, price):
        self.last_deal_price = price
        self.interactions += 1

    def get_reference_price(self):
        return self.last_deal_price
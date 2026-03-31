class Item:
    def __init__(self, name, base_price_per_unit, market_multiplier, unit="kg", quantity=1):
        self.name = name
        self.base_price_per_unit = base_price_per_unit
        self.market_multiplier = market_multiplier
        self.unit = unit
        self.quantity = quantity

    @property
    def base_cost(self):
        return self.base_price_per_unit * self.quantity

    @property
    def market_price_per_unit(self):
        return self.base_price_per_unit * self.market_multiplier

    @property
    def market_price(self):
        return self.market_price_per_unit * self.quantity

import random

MARKET_EVENTS = [
    {
        "name": "Portuguese Caravan Arrival",
        "description": "A grand Portuguese merchant caravan has arrived at Hampi from Goa. Demand for Pepper has skyrocketed!",
        "affected_spice": "pepper",
        "price_multiplier": 1.35,
        "quantity_multiplier": 1.5,
        "dialogue_trigger": "portuguese_caravan"
    },
    {
        "name": "Temple Chariot Festival",
        "description": "The annual Virupaksha Temple festival has begun. Religious offerings demand cloves and cardamom in massive amounts!",
        "affected_spice": "clove",
        "price_multiplier": 1.25,
        "quantity_multiplier": 1.3,
        "dialogue_trigger": "temple_festival"
    },
    {
        "name": "Krishna Bazaar Wholesale Demand",
        "description": "Wholesale merchants are buying up cardamom stocks for bulk shipments. Cardamom demand increases!",
        "affected_spice": "cardamom",
        "price_multiplier": 1.2,
        "quantity_multiplier": 1.4,
        "dialogue_trigger": "wholesale_demand"
    },
    {
        "name": "Malabar Monsoon Deluge",
        "description": "Heavy monsoon rains have flooded the southern spice roads. Cinnamon supply is severely restricted!",
        "affected_spice": "cinnamon",
        "price_multiplier": 1.4,
        "quantity_multiplier": 0.5,
        "dialogue_trigger": "monsoon_flood"
    }
]

def get_random_market_event():
    """Selects a random active market event for the shift (35% occurrence probability)."""
    if random.random() < 0.35:
        return random.choice(MARKET_EVENTS)
    return None

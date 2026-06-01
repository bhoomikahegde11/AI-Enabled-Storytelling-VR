import re
import random

# Traditional weights to modern grams conversions
PALAM_GRAMS = 35
SEER_GRAMS = 280
VEESAI_GRAMS = 1400  # 1.4 kg
MANANGU_GRAMS = 11200 # 11.2 kg
BAHAR_GRAMS = 448000  # 448 kg

def snap_to_nearest_traditional_unit(grams: float) -> float:
    """
    Snaps a grams value to the nearest whole multiple of the best-fitting
    traditional unit. This ensures final deal quantities are always clean
    integer multiples (e.g. 3 Seers, not 3.57 Seers) — historically accurate
    for 1500s Vijayanagara trade.
    Uses the same tier thresholds as grams_to_traditional_label.
    """
    grams = float(grams)
    if grams <= 0:
        return PALAM_GRAMS  # minimum 1 Palam

    # Use same tier thresholds as grams_to_traditional_label
    if grams >= BAHAR_GRAMS * 0.9:
        val = max(1, round(grams / BAHAR_GRAMS))
        return val * BAHAR_GRAMS
    if grams >= MANANGU_GRAMS * 0.9:
        val = max(1, round(grams / MANANGU_GRAMS))
        return val * MANANGU_GRAMS
    if grams >= VEESAI_GRAMS * 0.9:
        val = max(1, round(grams / VEESAI_GRAMS))
        return val * VEESAI_GRAMS
    if grams >= SEER_GRAMS * 0.8:
        val = max(1, round(grams / SEER_GRAMS))
        return val * SEER_GRAMS
    # Palam
    val = max(1, round(grams / PALAM_GRAMS))
    return val * PALAM_GRAMS


def grams_to_traditional_label(grams: float) -> str:
    """
    Converts modern grams weight into a highly immersive dual-unit label
    representing the closest standard traditional Vijayanagara unit.
    Always outputs whole-number unit counts (no decimals) — historically
    accurate for 1500s marketplace trade.
    """
    grams = float(grams)
    if grams <= 0:
        return "0 Palams (~0g)"
        
    # Bahar (Wholesale Bulk)
    if grams >= BAHAR_GRAMS * 0.9:
        val = max(1, round(grams / BAHAR_GRAMS))
        snapped_g = val * BAHAR_GRAMS
        unit = "Bahar" if val == 1 else "Bahars"
        return f"{val} {unit} (~{round(snapped_g/1000, 1)} kg)"
        
    # Manangu (Maund)
    if grams >= MANANGU_GRAMS * 0.9:
        val = max(1, round(grams / MANANGU_GRAMS))
        snapped_g = val * MANANGU_GRAMS
        unit = "Manangu" if val == 1 else "Manangus"
        return f"{val} {unit} (~{round(snapped_g/1000, 1)} kg)"
        
    # Veesai (Viss)
    if grams >= VEESAI_GRAMS * 0.9:
        val = max(1, round(grams / VEESAI_GRAMS))
        snapped_g = val * VEESAI_GRAMS
        # Veesai is both singular and plural in traditional Telugu/Kannada context
        return f"{val} Veesai (~{round(snapped_g/1000, 1)} kg)"
        
    # Seer
    if grams >= SEER_GRAMS * 0.8:
        val = max(1, round(grams / SEER_GRAMS))
        snapped_g = val * SEER_GRAMS
        unit = "Seer" if val == 1 else "Seers"
        if snapped_g < 1000:
            modern_label = f"{int(snapped_g)}g"
        else:
            modern_label = f"{round(snapped_g/1000, 1)}kg"
        return f"{val} {unit} (~{modern_label})"
        
    # Palam
    val = max(1, round(grams / PALAM_GRAMS))
    snapped_g = val * PALAM_GRAMS
    unit = "Palam" if val == 1 else "Palams"
    return f"{val} {unit} (~{int(snapped_g)}g)"

def parse_traditional_to_grams(text: str) -> float:
    """
    Robustly parses a text string representing traditional or modern weights
    and converts them to modern grams representation for internal backend logic.
    """
    text = str(text).lower().strip()
    
    # 1. Traditional Units Matchers
    # Bahar / Candy
    match = re.search(r"\b(\d+(?:\.\d+)?)\s*(bahar|bahars|candy|candies)\b", text)
    if match:
        return float(match.group(1)) * BAHAR_GRAMS
        
    # Manangu / Maund
    match = re.search(r"\b(\d+(?:\.\d+)?)\s*(manangu|manangus|maund|maunds)\b", text)
    if match:
        return float(match.group(1)) * MANANGU_GRAMS
        
    # Veesai / Viss
    match = re.search(r"\b(\d+(?:\.\d+)?)\s*(veesai|viss|veesais)\b", text)
    if match:
        return float(match.group(1)) * VEESAI_GRAMS
        
    # Seer
    match = re.search(r"\b(\d+(?:\.\d+)?)\s*(seer|seers)\b", text)
    if match:
        return float(match.group(1)) * SEER_GRAMS
        
    # Palam
    match = re.search(r"\b(\d+(?:\.\d+)?)\s*(palam|palams)\b", text)
    if match:
        return float(match.group(1)) * PALAM_GRAMS
        
    # 2. Modern Units Matchers (Fallback for player inputs)
    match = re.search(r"\b(\d+(?:\.\d+)?)\s*(kg|kgs|kilogram|kilograms)\b", text)
    if match:
        return float(match.group(1)) * 1000.0
        
    match = re.search(r"\b(\d+(?:\.\d+)?)\s*(g|gm|gram|grams)\b", text)
    if match:
        return float(match.group(1))
        
    return None

def get_random_spice_quantity(spice_name: str) -> float:
    """
    Selects a historically plausible randomized quantity in grams 
    based on the commercial density and value of the spice in the 1500s.
    Uses weighted selections to make large wholesale quantities (>10kg) and massive amounts (>100kg) extremely rare.
    """
    spice_name = str(spice_name).lower().strip()
    if "cardamom" in spice_name:
        # High value, rare/lightweight: mostly Palams and Seers
        options = [140, 210, 280, 560, 840, 1400]
        weights = [0.35, 0.30, 0.20, 0.10, 0.04, 0.01]  # 1.4kg (Veesai) is extremely rare for cardamom
        return random.choices(options, weights=weights)[0]
    elif "clove" in spice_name:
        # Premium spice: mostly 1 to 2 Seers
        options = [280, 560, 840, 1400, 2800]
        weights = [0.35, 0.35, 0.18, 0.10, 0.02]  # 2 Veesai (2.8kg) is rare
        return random.choices(options, weights=weights)[0]
    elif "cinnamon" in spice_name:
        # Standard spice: mostly 1 Seer to 1 Veesai
        options = [840, 1400, 2800, 4200]
        weights = [0.30, 0.45, 0.20, 0.05]  # 3 Veesai (4.2kg) is rare
        return random.choices(options, weights=weights)[0]
    elif "pepper" in spice_name:
        # Black Gold commodity: retail is 1 to 2 Veesai, 1 Manangu (11.2kg) is rare, Bahar (448kg) is extremely rare
        options = [1400, 2800, 7000, 11200, 448000]
        weights = [0.58, 0.35, 0.05, 0.018, 0.002]  # 1 Manangu (11.2kg) is 1.8% rare, 1 Bahar (448kg) is 0.2% extremely rare
        return random.choices(options, weights=weights)[0]
    else:
        # Default: 1 Veesai (~1.4 kg) or 1 Seer (~280g) or 2 Seers (~560g)
        options = [280, 560, 1400]
        weights = [0.30, 0.50, 0.20]
        return random.choices(options, weights=weights)[0]

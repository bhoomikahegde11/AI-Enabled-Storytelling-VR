import re

# Context words for domain-aware number conversion proximity check
context_words = {
    # Currency
    "varaha", "varahas", "waraha", "warahas", "vara", "varas", "baraha", "barahas",
    "price", "prices", "offer", "offers", "pay", "pays", "paying",
    "sell", "sells", "selling", "buy", "buys", "buying", "cost", "costs",
    # Quantity / Weight
    "palam", "palams", "palm", "palms", "palum", "palums", "palan", "palans",
    "veesai", "viss", "seer", "seers", "manangu", "maund", "maunds", "bahar", "bahars", "candy", "candies",
    "kg", "kgs", "kilogram", "kilograms", "bag", "bags",
    # Spice names
    "pepper", "peppers", "paper", "peper", "pepers",
    "cardamom", "cardamoms", "cardamon", "cardamons", "cardimum", "cardamum", "cardam",
    "cinnamon", "cinnamons", "cinamon", "cinnamun", "clove", "cloves",
    # Bargaining confirmations
    "ok", "okay", "fine", "good", "deal", "agree", "accept"
}

# Mapping for converting single words to numbers
units = {
    "zero": 0, "one": 1, "two": 2, "three": 3, "four": 4, "five": 5,
    "six": 6, "seven": 7, "eight": 8, "nine": 9, "ten": 10,
    "eleven": 11, "twelve": 12, "thirteen": 13, "fourteen": 14, "fifteen": 15,
    "sixteen": 16, "seventeen": 17, "eighteen": 18, "nineteen": 19
}

tens = {
    "twenty": 20, "thirty": 30, "forty": 40, "fourty": 40, "fifty": 50,
    "sixty": 60, "seventy": 70, "eighty": 80, "ninety": 90
}

scales = {
    "hundred": 100, "hundreds": 100, "thousand": 1000, "thousands": 1000
}

digit_map = {
    "zero": "0", "one": "1", "two": "2", "three": "3", "four": "4",
    "five": "5", "six": "6", "seven": "7", "eight": "8", "nine": "9"
}

def parse_number_word_sequence(words):
    """
    Parses a list of consecutive number words into a digit string.
    """
    if not words:
        return ""
        
    # Case: consecutive single digits, e.g. ["one", "four", "zero"] -> "140"
    if len(words) > 1 and all(w in digit_map for w in words):
        return "".join(digit_map[w] for w in words)
        
    # Case: single digit followed by tens word, e.g. "one forty" -> 140
    if len(words) == 2:
        w1, w2 = words[0], words[1]
        if w1 in digit_map and w1 != "zero" and w2 in tens:
            val1 = int(digit_map[w1])
            val2 = tens[w2]
            return str(val1 * 100 + val2)
            
    # Standard spoken number word parsing
    total = 0
    current = 0
    for w in words:
        if w in units:
            current += units[w]
        elif w in tens:
            current += tens[w]
        elif w in scales:
            scale = scales[w]
            if current == 0:
                current = 1
            current *= scale
            if scale >= 1000:
                total += current
                current = 0
        elif w == "and":
            continue
    total += current
    return str(total)

def is_near_context(tokens, group, window=3):
    """
    Checks if a number word group is near any marketplace context word.
    """
    first_idx = group[0]
    last_idx = group[-1]
    
    # Extract left words
    left_words = []
    for i in range(first_idx - 1, -1, -1):
        token = tokens[i]
        if re.match(r'^[a-zA-Z]+$', token):
            left_words.append(token.lower())
            if len(left_words) == window:
                break
                
    # Extract right words
    right_words = []
    for i in range(last_idx + 1, len(tokens)):
        token = tokens[i]
        if re.match(r'^[a-zA-Z]+$', token):
            right_words.append(token.lower())
            if len(right_words) == window:
                break
                
    for w in left_words + right_words:
        if w in context_words:
            return True
            
    return False

def normalize_numbers(text):
    """
    Finds sequences of spoken number words and normalizes them to digits
    if they appear near marketplace context words.
    """
    # Regex to find words (alphabetic) and non-words
    tokens = re.findall(r'\b[a-zA-Z]+\b|[^a-zA-Z]+', text)
    
    number_words_set = set(list(units.keys()) + list(tens.keys()) + list(scales.keys()))
    
    # Identify indices of tokens that are part of a number phrase
    is_num_token = []
    for i, token in enumerate(tokens):
        token_lower = token.lower()
        if token_lower in number_words_set:
            is_num_token.append(True)
        elif token_lower == "and":
            # Check if "and" is surrounded by number words
            has_prev = False
            has_next = False
            for j in range(i - 1, -1, -1):
                if tokens[j].strip():
                    if tokens[j].lower() in number_words_set:
                        has_prev = True
                    break
            for j in range(i + 1, len(tokens)):
                if tokens[j].strip():
                    if tokens[j].lower() in number_words_set:
                        has_next = True
                    break
            is_num_token.append(has_prev and has_next)
        else:
            is_num_token.append(False)
            
    # Group consecutive True values, separating if there is non-space punctuation
    groups = []
    current_group = []
    
    for i, token in enumerate(tokens):
        if is_num_token[i]:
            current_group.append(i)
        else:
            if token.strip() == "":
                if current_group:
                    current_group.append(i)
            else:
                if current_group:
                    while current_group and tokens[current_group[-1]].strip() == "":
                        current_group.pop()
                    if current_group:
                        groups.append(current_group)
                    current_group = []
                    
    if current_group:
        while current_group and tokens[current_group[-1]].strip() == "":
            current_group.pop()
        if current_group:
            groups.append(current_group)
            
    # Replace groups in-place ONLY if they are near context words
    new_tokens = list(tokens)
    for group in groups:
        group_words = [tokens[idx].lower() for idx in group if tokens[idx].lower() in number_words_set or tokens[idx].lower() == "and"]
        if not group_words:
            continue
            
        if not is_near_context(tokens, group, window=3):
            continue
        
        parsed_val = parse_number_word_sequence(group_words)
        
        first_idx = group[0]
        new_tokens[first_idx] = parsed_val
        for idx in group[1:]:
            new_tokens[idx] = ""
            
    return "".join(new_tokens)

def normalize_text(text: str) -> str:
    """
    Main normalization pipeline. Converts spoken text into canonical bazaar terms and digits.
    """
    if not text:
        return ""
        
    # 1. Preprocess common Whisper errors/homophones
    # Handle "one for tea" -> "140"
    text = re.sub(r'\bone for tea\b', '140', text, flags=re.IGNORECASE)
    
    # Handle "one fivety" -> "150"
    text = re.sub(r'\bone fivety\b', '150', text, flags=re.IGNORECASE)
    text = re.sub(r'\bfivety\b', 'fifty', text, flags=re.IGNORECASE)
    
    # Handle "tree hundred" -> "300"
    text = re.sub(r'\btree hundred\b', '300', text, flags=re.IGNORECASE)
    text = re.sub(r'\btree\b', 'three', text, flags=re.IGNORECASE)
    
    # Handle "for <units/scales/currencies>" -> "four \1"
    # Target units: palams, palms, palans, palan, palm, palum, tulas, tola, tulla, thula, manas, mana, manna, bags
    # Target currencies: varaha, varahas, waraha, warahas, vara, varas, baraha, barahas
    # Target numbers/scales: hundred, hundreds, thousand, thousands, ten, tens, twenty, thirty, forty, fourty, fifty, sixty, seventy, eighty, ninety
    pattern_for = r'\bfor\s+(palams?|palms?|palans?|palan|palm|palum|tulas?|tolas?|tullas?|thulas?|manas?|manas?|mannas?|bags?|varahas?|warahas?|varas?|barahas?|hundreds?|thousands?|tens?|twent(y|ies)|thirt(y|ies)|fort(y|ies)|fourt(y|ies)|fift(y|ies)|sixt(y|ies)|sevent(y|ies)|eight(y|ies)|ninet(y|ies))\b'
    text = re.sub(pattern_for, r'four \1', text, flags=re.IGNORECASE)
    
    # 2. Number conversion
    text = normalize_numbers(text)
    
    # 3. Currency normalization (canonical: "varaha")
    currency_pattern = r'\b(warahas?|varas?|varaha\'s|barahas?)\b'
    text = re.sub(currency_pattern, 'varaha', text, flags=re.IGNORECASE)
    # Ensure "varahas" also maps to "varaha"
    text = re.sub(r'\bvarahas\b', 'varaha', text, flags=re.IGNORECASE)
    
    # 4. Weight unit normalization (canonical: "palam", "tula", "mana")
    text = re.sub(r'\b(palms?|palans?|palum)\b', 'palam', text, flags=re.IGNORECASE)
    text = re.sub(r'\b(tolas?|tullas?|thulas?)\b', 'tula', text, flags=re.IGNORECASE)
    text = re.sub(r'\b(manas?|mannas?)\b', 'mana', text, flags=re.IGNORECASE)
    
    # 5. Spice names normalization (canonical: "pepper", "cardamom", "cinnamon", "clove")
    text = re.sub(r'\b(paper|peper)\b', 'pepper', text, flags=re.IGNORECASE)
    text = re.sub(r'\b(cardamon|cardimum|cardamum|cardam)\b', 'cardamom', text, flags=re.IGNORECASE)
    text = re.sub(r'\b(cinamon|cinnamun)\b', 'cinnamon', text, flags=re.IGNORECASE)
    text = re.sub(r'\bcloves\b', 'clove', text, flags=re.IGNORECASE)
    
    # 6. Historical terms normalization
    text = re.sub(r'\b(humpy|hampi bazaar|hampi market)\b', 'Hampi', text, flags=re.IGNORECASE)
    text = re.sub(r'\b(vijaynagar|vijayanagar|vijayanagara empire)\b', 'Vijayanagara', text, flags=re.IGNORECASE)
    text = re.sub(r'\b(portugese|portuguese traders)\b', 'Portuguese', text, flags=re.IGNORECASE)
    
    # Clean up double spaces that might arise
    text = re.sub(r'\s+', ' ', text).strip()
    
    return text

import sys
import os

# Add backend directory to sys.path so we can import from npc_engine
sys.path.append(os.path.join(os.path.dirname(__file__), "../backend"))

from npc_engine.utils.text_normalizer import normalize_text

def test_normalizer():
    tests = [
        # 1. Spoken Numbers & Typo Cases (with context where needed)
        ("one hundred forty varahas", "140 varaha"),
        ("one forty varaha", "140 varaha"),
        ("one fifty varaha", "150 varaha"),
        ("two twenty varaha", "220 varaha"),
        ("one four zero varaha", "140 varaha"),
        ("four palams", "4 palams"),
        ("ten bags", "10 bags"),
        ("one for tea", "140"),
        ("one fivety", "150"),
        ("tree hundred", "300"),
        ("for palams", "4 palams"),
        ("one thing", "one thing"),
        ("one moment", "one moment"),
        
        # 2. Currency cases
        ("waraha", "varaha"),
        ("vara", "varaha"),
        ("varas", "varaha"),
        ("varaha's", "varaha"),
        ("baraha", "varaha"),
        ("barahas", "varaha"),
        ("varahas", "varaha"),
        ("warahas", "varaha"),
        ("140 warahas", "140 varaha"),
        
        # 3. Weight cases
        ("palms", "palam"),
        ("palans", "palam"),
        ("palan", "palam"),
        ("palm", "palam"),
        ("palum", "palam"),
        ("tola", "tula"),
        ("tulla", "tula"),
        ("thula", "tula"),
        ("mana", "mana"),
        ("manas", "mana"),
        ("manna", "mana"),
        
        # 4. Spice cases
        ("paper", "pepper"),
        ("peper", "pepper"),
        ("black paper", "black pepper"),
        ("black peper", "black pepper"),
        ("cardamon", "cardamom"),
        ("cardimum", "cardamom"),
        ("cardamum", "cardamom"),
        ("cardam", "cardamom"),
        ("cinamon", "cinnamon"),
        ("cinnamun", "cinnamon"),
        ("cloves", "clove"),
        
        # 5. Historical cases
        ("humpy", "Hampi"),
        ("hampi bazaar", "Hampi"),
        ("hampi market", "Hampi"),
        ("vijaynagar", "Vijayanagara"),
        ("vijayanagar", "Vijayanagara"),
        ("vijayanagara empire", "Vijayanagara"),
        ("portugese", "Portuguese"),
        ("portuguese traders", "Portuguese"),
        
        # 6. Intent cleanup (should not rewrite meaning, only fix spelling/typos)
        ("how much money you have", "how much money you have"),
        ("how much can you give", "how much can you give"),
        
        # 7. Multi-phrase sentence tests (User's specific test cases)
        ("I sell four palms of cardamon for one forty warahas", "I sell 4 palam of cardamom for 140 varaha"),
        ("how much vara can you give for black paper", "how much varaha can you give for black pepper")
    ]
    
    passed = 0
    failed = 0
    print("============================================================")
    print(" RUNNING TEXT NORMALIZER UNIT TESTS ")
    print("============================================================")
    
    for i, (input_text, expected) in enumerate(tests, 1):
        actual = normalize_text(input_text)
        if actual == expected:
            passed += 1
            print(f"[PASS] Test {i}: '{input_text}' -> '{actual}'")
        else:
            failed += 1
            print(f"[FAIL] Test {i}: '{input_text}'")
            print(f"       Expected: '{expected}'")
            print(f"       Actual:   '{actual}'")
            
    print("============================================================")
    print(f" RESULTS: {passed} passed, {failed} failed")
    print("============================================================")
    
    if failed > 0:
        sys.exit(1)
    else:
        sys.exit(0)

if __name__ == "__main__":
    test_normalizer()

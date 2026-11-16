import unittest
from tts_text import integer_words, normalize_tts_text


class NumberNormalizationTests(unittest.TestCase):
    def test_requested_prices(self):
        self.assertEqual(normalize_tts_text("Ayyo, 250 varahas? That is too much. My offer remains 173 varahas."),
                         "Ayyo, two hundred and fifty varahas? That is too much. My offer remains one hundred and seventy-three varahas.")
        self.assertEqual(normalize_tts_text("Ora! 300 varahas; 180 varahas."),
                         "Ora! Three hundred varahas; one hundred and eighty varahas.")

    def test_integer_boundaries(self):
        cases = {0:"zero",19:"nineteen",20:"twenty",21:"twenty-one",100:"one hundred",101:"one hundred and one",
                 999:"nine hundred and ninety-nine",1000:"one thousand",1001:"one thousand and one",
                 1250:"one thousand two hundred and fifty",1000000:"one million"}
        for n, expected in cases.items():
            with self.subTest(n=n): self.assertEqual(integer_words(n), expected)

    def test_quantities_thousands_decimals_and_punctuation(self):
        self.assertEqual(normalize_tts_text("2 seers, 280 grams; 1,250 varahas. 1.05 kg!"),
                         "Two seers, two hundred and eighty grams; one thousand two hundred and fifty varahas. One point zero five kg!")
        self.assertEqual(normalize_tts_text("Price:173. (180)?"), "Price:one hundred and seventy-three. (one hundred and eighty)?")

    def test_source_tokens_preserved(self):
        text = r"L1_LAK_105 SAMPLE_02 ID-173 v1.2.3 clip_173.wav G:\voices\173.wav Assets/Audio/250.wav https://example.test/180 {counterPrice}"
        self.assertEqual(normalize_tts_text(text), text)

    def test_does_not_mutate_original_or_repeat_normalization(self):
        source = "173 varahas for 2 seers."
        spoken = normalize_tts_text(source)
        self.assertEqual(source, "173 varahas for 2 seers.")
        self.assertEqual(normalize_tts_text(spoken), spoken)

    def test_out_of_range_is_explicit(self):
        with self.assertRaises(ValueError): normalize_tts_text("1000000000 varahas")

    def test_attached_metric_quantity(self):
        self.assertEqual(normalize_tts_text("Weigh 280g, or 1.4kg."), "Weigh two hundred and eighty grams, or one point four kilograms.")

    def test_ambiguous_grouping_is_explicit(self):
        with self.assertRaises(ValueError): normalize_tts_text("12,34 varahas")


if __name__ == "__main__": unittest.main()

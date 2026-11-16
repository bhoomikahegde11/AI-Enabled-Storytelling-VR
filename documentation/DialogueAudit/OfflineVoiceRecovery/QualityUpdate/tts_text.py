"""Deterministic speech-only normalization. Never apply to Unity data or IDs."""
import re

NORMALIZATION_VERSION = "english-numbers-v1"
SMALL = ("zero one two three four five six seven eight nine ten eleven twelve thirteen fourteen fifteen sixteen seventeen eighteen nineteen").split()
TENS = "zero ten twenty thirty forty fifty sixty seventy eighty ninety".split()


def integer_words(value):
    if not isinstance(value, int) or isinstance(value, bool) or not 0 <= value <= 999_999_999:
        raise ValueError("Supported spoken integer range is 0–999,999,999.")
    if value < 20:
        return SMALL[value]
    if value < 100:
        return TENS[value // 10] + ("-" + SMALL[value % 10] if value % 10 else "")
    if value < 1000:
        return SMALL[value // 100] + " hundred" + (" and " + integer_words(value % 100) if value % 100 else "")
    divisor, label = (1_000_000, "million") if value >= 1_000_000 else (1000, "thousand")
    head, tail = divmod(value, divisor)
    return integer_words(head) + " " + label + ((" and " if tail < 100 else " ") + integer_words(tail) if tail else "")


# Protect source-like tokens. Digits in identifiers/filenames/URLs/paths are not speech amounts.
PROTECTED = re.compile(r"https?://\S+|\S*[/\\]\S*|\b[\w.-]+\.(?:wav|mp3|ogg|json|asset|unity|py|onnx)\b|\b[A-Za-z_][\w.-]*\d[\w.-]*\b|\{[^{}]+\}", re.I)
NUMBER = re.compile(r"(?<![\w])(?:\d{1,3}(?:,\d{3})+|\d+)(?:\.\d+)?(?![\w])")


def normalize_tts_text(text):
    if not isinstance(text, str):
        raise TypeError("Expected spoken dialogue text.")
    def numbers(chunk):
        chunk = re.sub(r"(?<!\w)(\d+(?:\.\d+)?)(kg|g)\b",
                       lambda m: m[1] + " " + ({"kg":"kilograms", "g":"grams"}[m[2].lower()]), chunk, flags=re.I)
        for token in re.findall(r"(?<!\w)\d[\d,]*,\d[\d,]*", chunk):
            if not re.fullmatch(r"\d{1,3}(?:,\d{3})+", token):
                raise ValueError("Ambiguous numeric grouping; supply an ungrouped integer.")
        def replace(match):
            token = match.group().replace(",", "")
            whole, point, fraction = token.partition(".")
            result = integer_words(int(whole))
            if point: result += " point " + " ".join(SMALL[int(d)] for d in fraction)
            if not chunk[:match.start()].strip() or re.search(r"[.!?]\s*$", chunk[:match.start()]):
                result = result[0].upper() + result[1:]
            return result
        return NUMBER.sub(replace, chunk)
    pieces = []
    end = 0
    for match in PROTECTED.finditer(text):
        pieces.extend((numbers(text[end:match.start()]), match.group()))
        end = match.end()
    pieces.append(numbers(text[end:]))
    return "".join(pieces)

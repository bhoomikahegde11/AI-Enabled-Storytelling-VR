# Chinnamma Naik — language and separate proposals

Original Unity strings remain verbatim in EXACT_LINES.md/CSV/JSON. Nothing here is an applied rewrite. This is a current-source audit, not historical/linguistic research or a listening approval.

## Evidence-based identity and voice

Registry: `chinnamma_naik`, Chinnamma Naik, **Vijayanagara Wholesale Spice Buyer**, Strict. The current personality guide (`Core/Dialogues/DIALOGUE_AUTHORING_GUIDE.txt:378`) calls her a **senior, strict, experienced, respected merchant woman**, with warehouses, large orders, city trade, discipline and accounts as reference material. Her actual lines establish onward trade and familiarity with the local market. Do not invent a royal appointment, birthplace, caste, exact age or documented historical identity from Naik.

`ChinappaNaikDialogue.cs:3–5` explicitly says she is fictional, the guide establishes no mother tongue, and the Kannada colour Howdu/Beda/Saaku is conservative modern regional representation, not period quotations. Preserve the current **Chinnamma** display name despite the historical Chinappa filename/class. Never produce a male Chinappa voice because of that filename.

Retain **Indic Parler / Priya**. Direction: a senior, confident local wholesale buyer, direct and dry-witted, close conversational distance, clear amounts, restrained authority rather than shouting. Kannada-influenced Indian English is a source-supported creative colour; a precise native accent is not established. The studio voice description supports confidence/dry wit but sets a blanket `mood=friendly`; that must not be mistaken for the runtime Strict personality or a command to make low-patience lines cheerful. No voice settings were changed.

| Scenario | Delivery |
|---|---|
| Greeting/terms | Businesslike and welcoming enough to continue; no ceremonial announcement |
| Raised counter | Deliberate concession with a clear figure; no triumphant flourish |
| HOLD/Strict | Settled arithmetic, firm endpoint; never imply a raise |
| Friendly effective personality | Dry warmth; still a capable wholesale buyer |
| Low patience | Brief, decisive, civil; not a new voice or exaggerated anger |
| Skeptical low price | Caution and a real question; no enthusiasm suggesting agreement |
| Acceptance | Satisfied price agreement followed by goods request |
| Receipt | Thanks only after delivery/settlement |
| No deal | Practical disappointment or time pressure as selected, not automatic rage |

## Pronunciation review

| Words | Context / approval need |
|---|---|
| Chinnamma Naik | Preserve full correct name; doubled consonants and Naik pronunciation require native-speaker approval. Do not silently use Chinappa. |
| Howdu, Beda, Saaku | Actual authored words. Kannada colour is explicit, but pronunciation/intensity and code-switching naturalness need approval. Saaku in a warning should not become a comic yell. |
| Vijayanagara, Hampi | Long city name and short local place-name in authored/shared sources; consistent pronunciation, not tourist narration. |
| Krishnadevaraya; Virupaksha / Virupaksha's | Prefer complete history sentences; verify the entire proper name and possessive ending, not just isolated syllables. |
| varaha / varahas | Singular and plural; neither rupees nor dollars; preserve amount exactly. |
| Palam(s), Seer(s), Veesai, Manangu(s), Bahar(s) | Game output spellings/conversions, not proof of one historical dialect. Confirm pronunciations and audible plurals. |
| pepper / black pepper; cinnamon / cinnamon bark; cardamom / green cardamom | Three actual products plus precise brain descriptors; no clove generation. |
| wholesale, measure, figure, accounts, purchase, arithmetic, onward trade | Avoid formal letter-reading or an artificially superior tone. |
| thirteen/thirty, fourteen/forty, fifteen/fifty, nineteen/ninety, hundred/and | Numerical meaning must remain clear inside the sentence; consistent vocal effort for component recordings. |

No authoritative IPA/historical pronunciation is asserted. Phonetic TTS spellings, if later approved, belong in a separate spoken-input field, not in original Unity text or IDs.

## Actual defects versus optional style — no edits applied

| Source / wording | Assessment | Minimal proposal for later approval |
|---|---|---|
| Any `{currency}` amount rendered as1varahas | Confirmed provider grammar defect | Singular one varaha for1, same price; synchronize any resulting subtitle/audio difference. Contextual currency tails need singular versions too. |
| Shared greeting/origin with `{buyerOrigin}` | Confirmed role-as-place defect if fallback becomes audible: “of/from Vijayanagara Wholesale Spice Buyer” | Bind her identity/local trading background properly; do not turn the entire role into a country or manufacture a distant journey. Authored AskOrigin already supplies safe alternatives. |
| Shared reason “carry ... onward and sell at a profit” | Role-compatible for wholesale buyer, unlike Lakshmamma | Preserve economic meaning; do not automatically apply Father's priest or Lakshmamma's household rewrite. |
| Shared “Your price meets my offer, yet I will hold...” | Misdescribes below-offer guard and sounds contradictory; normally masked | Neutral nonacceptance/HOLD with the actual amount. Do not make it an agreement. |
| Shared “I can improve a little, but not much.” with no raise | Suggests movement not made; normally masked | State unchanged offer instead. |
| “Goods received, and my thanks. A fair dealing is worth remembering.” | Authored TransactionSuccess; awkward article/collocation, not gameplay defect | Optional “Fair dealing is worth remembering.” Keep receipt gated after fulfillment. |
| “The spice has weight; that price has ambition.” | Deliberate dry humour; valid, not an error | Keep if delivery sounds conversational; avoid theatrical narration. |
| “Put the measure at ...”, “My buying today is ...” | Slightly formal but intelligible marketplace English | Performance review first; optional simpler wording only with approval. |
| “Repeating the price will not improve the case.” / “A louder price...” | Not every HOLD follows repetition/loudness; broad reusable reaction may presume player behaviour | Consider neutral HOLD wording if runtime examples feel accusatory. Do not alter patience or assume loud speech is detected. |
| Agreement instructions “bring/measure out” | Correctly pending delivery | Preserve item, quantity and price. Do not change into goods-received thanks. |

Her four OfferReason lines are routed for ordinary explicit price-pressure questions, unlike Lakshmamma. Do not defer those on the basis of the Lakshmamma exception. Her low-patience groups override general Strict lines; not every Strict sentence is an anger line. There is no dedicated high-respect dialogue set. The normal generator does not actually reserve huge quantities for a wholesale character, so do not add unsupported bulk-order requirements to spoken text.

## Recording readiness and approvals

The12 actual-ID audition selections with exact text and rendered context are in AUDIT.md. Priya remains the selected identity. Ready for a **reviewed small audition**, not an automatically approved bulk queue: approve name/regional/unit pronunciations, singular currency/subtitle handling, any awkward-line corrections and whether to cover all masked defensive sources. Keep fixed lines whole and prices/quantities/items dynamic. Do not independently randomize a new audio line or borrow another character's fragments.

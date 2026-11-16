# Lakshmamma — language, delivery and separate proposals

All material here is guidance or a proposal. The exact inventory remains unchanged. No historical-language or pronunciation research was performed; the game’s own configuration and authored character comments are the evidence. Native-speaker pronunciation approval is still needed. Accessible modern English/Kannada influence is a creative representation, not a reconstruction of sixteenth-century speech.

## Evidence and appropriate voice direction

`DialogueCharacterRegistry.CreateDefaultProfiles` specifies **Local Household Buyer / Friendly**. Her table calls her local to Vijayanagara, buying provisions for the house, familiar with market bargaining, and interested in keeping enough money for other purchases. `LakshmiAmmaDialogue.cs:3–6` explicitly calls for accessible period-representing English and selective Kannada expressions. Her own lines use Namaskara, Sari, Ayyo and Swalpa. Nothing specifies a precise district, caste, exact age, native-language biography or a documented historical accent. Do not invent those.

Recommended interpretation: **a warm, mature local Kannada-influenced Indian-English household buyer, experienced and alert, speaking directly to one nearby merchant**. “Mature/motherly” is a performance choice consistent with the character, not a measured age from an asset. Warmth is social confidence, not weakness or agreement. Use a steady conversational pace; clearly articulate amounts and units without announcing every word. Preserve Swapna’s selected identity. The configuration does not prove that the model can render a convincing Kannada accent; approval must come from listening.

| Runtime context | Delivery direction | Avoid |
|---|---|---|
| Greeting/repeat visit | Open, approachable; smile audible, concise price question | Ceremonial tour-guide introduction |
| Ask goods/quantity/budget | Practical and clear; make quantity and amount intelligible | Classroom reading of placeholders |
| Friendly counter | Light conversational persuasion, not conceding too easily | Excited rising pitch on every number |
| Strict hold | Warmth reduced, precise and grounded | Changing age/voice identity or sounding hostile |
| Impatient / low patience | Shorter cadence, firmer endpoint, still civil | Shouting or breathless faster playback |
| Suspiciously low price | Genuine pause/skepticism, no pleased acceptance tone | Celebrating a bargain the brain refused |
| Hostile warning | Hurt or firm boundary, controlled volume | Cartoon anger or a different persona |
| Clarification | Helpful and exact; wrapper and core sound like one thought | Blaming the player, theatrical confusion |
| Price agreement | Satisfied, then practical request for goods | Receipt/farewell before fulfillment |
| Successful delivery | Relaxed and genuinely pleased | Repeating an acceptance as though unpaid |
| No deal | Disappointed or matter-of-fact, depending on available context | Rage in an otherwise calm failed bargain |

There is no independent mood FSM for “skeptical,” “satisfied” or “disappointed.” These are delivery interpretations of existing scenarios. Existing mood-restricted groups are Strict/Impatient HOLD and low-patience variants; do not propose dozens of emotion files unless selection can distinguish them. A single unrestricted line can appear in more than one effective personality, so its default take should be restrained and broadly appropriate.

## Pronunciation-sensitive review list

These are recording-review targets, not authoritative phonetic transcriptions. Keep exact subtitle spelling; any phonetic synthesis spelling must live in a separate reviewed spoken-input field.

| Term(s) | Where needed | Review concern / approval needed |
|---|---|---|
| **Lakshmamma** | Four AskName lines; defensive identity text | Preserve consonant cluster and doubled `m`; one coherent name, not “Lakshmi Amma.” Use a local-speaker approved reference pronunciation; do not silently rename runtime ID `lakshmi_amma`. |
| **Namaskara** | First greeting L1_LAK_002 | Natural brief greeting, not a long ceremonial salutation; vowel/stress approval. |
| **Ayyo** | Price disbelief, off-topic, warnings, low-patience reminders | Context-sensitive interjection, not comic shriek or identical exclamation every time. |
| **Swalpa** | Slightly-high price, counter and unrouted TimePressure | Initial consonant sequence can be swallowed or anglicized; native-speaker check, no indiscriminate substitution with Hindi. |
| **Sari** | Agreement, clarification, social and hold lines | Meaning here is “alright”; avoid English garment-word delivery or sing-song emphasis. |
| **Vijayanagara** | Origin and history | Long name, syllable clarity and regional stress; not a modern tourist narration. |
| **Hampi** | Shared brain fallbacks / user topic | Short name; no extra syllables. Does not itself require adding Hampi to every authored line. |
| **Krishnadevaraya** | Four HistoryKing answers | High-risk long proper name; keep a consistent approved pronunciation across complete sentences. |
| **Virupaksha / Virupaksha’s** | Four HistoryTemple answers | Consonant cluster and possessive ending must survive; record in full sentence when possible. |
| **varaha / varahas** | All price-bearing dialogue | Both singular/plural and consonants intelligible. Not “rupees,” “dollars,” or a swallowed syllable; check teen/tens boundaries. |
| **Palam/Palams** | Small quantities and changed requests | Singular/plural audible; game conversion is not historical proof. |
| **Seer/Seers** | Common starting quantities | Avoid confusing amount and homophone; make plural endings audible in connected speech. |
| **Veesai** | Common starting quantities | Spelling/model pronunciation needs approval; same displayed form for one and several. Input alias `viss` does not mean both must be spoken. |
| **Manangu/Manangus** | Larger changed quantities | Less familiar to English TTS; spelling and plural are game conventions; approve pronunciation before producing many takes. |
| **Bahar/Bahars** | Very large changed quantities | Do not introduce an invented regional pronunciation as historical fact. |
| **pepper / black pepper** | Authored item and defensive descriptor | Preserve hard consonants; avoid “paper.” |
| **cinnamon / cinnamon bark** | Authored item and defensive descriptor | Avoid dropped syllables or clipped `bark`; source does not say “cassia.” |
| **cardamom / green cardamom** | Authored item and defensive descriptor | Do not confuse cardamom/cardamon; preserve ending in contextual phrases. |
| **merchant, bargain, provisions, purse, arithmetic, bazaar, household** | Many complete lines | Some sound stilted when stressed word-by-word. Read as everyday conversation, not period-drama declamation. |
| **thirteen/thirty, fourteen/forty, fifteen/fifty, eighteen/eighty, nineteen/ninety** | Reusable number vocabulary | Numerical meaning must survive joins and quiet endings. Test in amounts, not isolated words only. |
| **hundred, and; thousand/million/billion if supported** | Compound/extended amounts | One consistent voice and speech rate; no emphatic announcer treatment of isolated numbers. |

The generic fallback role **Local Household Buyer** is metadata, not a pronunciation vocabulary item to say as her homeland. `maund`, `viss`, `candy`, `bag` and metric input aliases are not separately required spoken unit names because the game re-formats them to traditional output labels. No new historical units are proposed.

## Language assessment — preserve what already works

- “A friendly smile will not change my arithmetic” is characterful, firm and mildly humorous. It can work with dry warmth; it is not a factual error merely because it is idiomatic English.
- Household budgeting reasons are appropriate for her role; the shared resale-profit fallback is not.
- Current agreement lines carefully distinguish agreed price from goods received. Preserve that distinction, all explicit amounts and quantities, and the present-tense buyer role.
- Her history answers are brief, local and deliberately modest about court knowledge. Avoid turning her into a documentary narrator or adding unverified historical anecdotes.
- Kannada expressions already appear selectively, across moods. More regional words alone will not produce an authentic accent or natural rhythm.
- Most lines are grammatical. Repetition of “My offer remains…” is partly necessary when the buyer genuinely holds. Better performance and sufficient existing variation should precede another comprehensive rewrite.

## Optional wording proposals — approval required, no edits applied

| Original source / exact wording | Issue type | Proposed direction |
|---|---|---|
| Any price line rendered as “1 varahas” | Confirmed presentation grammar defect: provider always supplies plural currency | Speak/display **one varaha** for amount1; preserve every other amount and frozen price. Handle suffix inside a contextual fragment too, not only a standalone currency word. |
| Shared brain greeting “I am {buyerName} of {buyerOrigin}…” | Confirmed metadata-as-location defect if exposed; normally masked | Lakshmamma-specific voice binding can use “I am Lakshmamma. I live here in Vijayanagara…” while preserving the requested quantity/spice. Needs approved mapping/subtitle difference; do not change generic code during this audit. |
| Shared origin “I have travelled far from {buyerOrigin} to this bazaar…” | Contradicts local buyer profile if spoken | Use her existing authored local-origin answers rather than invent a journey. |
| Shared reason “...carry the {spiceDescriptor} onward and sell at a profit.” | Conflicts with household role if exposed | Use an approved household-budget reason with the same current offer; do not change economics. |
| Shared brain “Your price meets my offer, yet I will hold at {currentOffer} varahas.” | Misleading if seller price was actually below current offer; ordinarily authored skepticism masks it | “I am not ready to agree to that price. My offer remains…” No acceptance implication. |
| Shared brain “I can improve a little, but not much.” on a zero-increase result | Implies a concession not actually made; ordinarily HOLD masks it | “My offer has not changed.” Keep the actual standing amount when useful. |
| L1_LAK_049: “Only a little separates us. Would you take my {counterPrice} {currency}?” | Possible factual overstatement: an absolute close-price threshold can be a large relative gap on a tiny trade | Optional neutral “Would you take my offer of…”; not a parser/economic change. |
| L1_LAK_116: “The sky keeps its own counsel. My errands do not wait for it.” | Stylistic, somewhat literary | Optional “Whatever the sky is doing, I still have my errands.” Existing text is valid if performed lightly. |
| L1_LAK_112: “Here in Vijayanagara is home. I have done plenty of buying in these streets.” | Slightly awkward spoken opening | Optional “Vijayanagara is home. I have done plenty of buying in these streets.” |
| L1_LAK_182: “I respect your asking price, but my offer remains…” | Slightly formal bargaining phrase, not a defect | Optional “I hear your price, but my offer remains…”; avoid unnecessary regeneration if the current take sounds natural. |
| L1_LAK_103: “I am buying for a household, merchant…” | More impersonal than established “the house” voice | Optional “I am buying for the house, merchant…”; meaning unchanged. |
| L1_LAK_107: “My name is Lakshmamma. It is good to meet you.” | Can sound like a chatbot if every phrase gets equal stress | Prefer natural delivery first; optional contraction only if approved. |
| L1_LAK_166: “Let us not spend the whole morning on one purchase.” | Time-of-day assumption; **unrouted** current TimePressure line | Defer rather than rewrite/record now. If activated later, check whether morning is guaranteed. |
| Many uses of “merchant” and “Sari” in successive turns | Frequency/performance issue, not a single defective sentence | Use restrained delivery and existing nonrepeating variants; do not randomly omit words at playback. Any text simplification must be reviewed and reflected in subtitles. |

The long unresolved-terms clarification asks acceptance versus changing price/quantity in one sentence. It is semantically important. Do not shorten it to “Do you accept?” if that drops the reason an earlier apparent acceptance was unsafe.

## Recording strategy for review

1. Freeze spelling and approve the pronunciation list, especially Lakshmamma, long proper names, varaha and historical-unit forms. Retain Indic Parler/Swapna; no model selection experiment is proposed.
2. Review the 73 routed fixed authored lines as complete sentences. They include personality-sensitive warnings/reminders, thanks, name/origin/history and rejection. A pleasant but controlled conversational default is safer than exaggerated emotion.
3. Review the129 dynamic/nested templates by **complete reconstructed sentences**, not isolated chunks. Keep quantities, item and exact current/accepted price. Validate at least1/19/34/105/234/999 and a referenced amount above999; all three spices; one/two Seers and Veesai, plus changed Palam/Manangu/Bahar quantities.
4. Plan the compact speaker-specific vocabulary and phrase pieces; use complete common quantity expressions where they improve connected speech. Prefer larger meaningful context when safe; do not promise that one isolated number recording works equally naturally in every sentence position.
5. Preserve the exact selected dialogue and nested rule core in future bank mapping. Every unrecorded eligible random alternative needs explicit text-only/TTS fallback; an ESSENTIAL spreadsheet flag does not restrict Unity selection.
6. Listen for performance, numeric correctness and endings before any broad production. This is a quality gate on the already selected voice, not permission for another engine experiment. No audio is generated by this audit.

Approval is required for suggested wording changes, final regional pronunciation, defensive role-text correction policy, and generation scope. No further approval is needed merely to retain the already selected Swapna assignment.

## Four-character verification addendum — 27 October 2026

The current authoring guide (`Core/Dialogues/DIALOGUE_AUTHORING_GUIDE.txt`, CURRENT CHARACTER PERSONALITY GUIDE) explicitly describes Lakshmamma as a **warm elder**, witty, practical and caring, with household/provision buying as her role. This strengthens the earlier mature/motherly performance direction; an exact age, district or historically reconstructed accent remains unspecified. All original authored lines and shared-script hashes still match the earlier audit. The saved Editor scene now forces Father for testing, so ordinary Lakshmamma availability requires clearing/changing that existing override later. No source, voice assignment or audio was changed.

Twelve current source-ID audition selections are appended to AUDIT.md. New canonical EXACT_LINES.md and DIALOGUE_INVENTORY.csv/.json add explicit coverage classes and current source hashes; the original prefixed inventory files remain preserved. The language and production-approval caveats above still apply.

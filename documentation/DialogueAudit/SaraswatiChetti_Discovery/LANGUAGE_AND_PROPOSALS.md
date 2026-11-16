# Saraswati Chetti — language and separate proposals

No Unity wording, profile, voice setting or audio is changed. EXACT_LINES.md and the inventories preserve every original string. Proposed changes below require review, not automatic voice-side substitution.

## Character and speaking direction from actual sources

Registry/profile: `saraswati_chetti`, **Saraswati Chetti**, Local Vijayanagara Retail Shop Owner, Friendly. The current authoring guide (`Core/Dialogues/DIALOGUE_AUTHORING_GUIDE.txt:388`) describes a **friendly, knowledgeable, energetic merchant woman** and references customers, shop, resale, crowd, shelves and jars. Her authored OfferReason explicitly discusses reselling in her shop. This is not a household buyer or wholesale-warehouse role.

The active class/file is historically named **SiddharthChettiDialogue**, but its current return value, registry routing and every identity line are Saraswati. Do not record Siddharth, infer male gender from that filename or restore an older identity.

Retain **Indic Parler / Sneha**. Recommended delivery: engaged Indian-English shopkeeper, practical, numerate, lively enough to sound human, with precise but conversational terms. The source words **Namaskaram, Avunu, Konchem** provide a regional-colour cue; interpreting them as a Telugu-influenced performance is a **creative direction requiring linguistic approval**, not a declared mother tongue or a historically proven Vijayanagara accent. Do not infer caste, birthplace or language solely from Chetti, and do not replace these words indiscriminately with Lakshmamma's Kannada interjections.

| Context | Delivery direction |
|---|---|
| Greeting/shop information | Engaged, brisk, interested in the goods |
| Counter | Clear amount with measured persuasion; avoid an arithmetic announcer |
| Friendly/Normal HOLD | Attentive and reasoned, offer unchanged |
| Strict effective personality | Less warmth, more precision, same speaker identity |
| Impatient/low patience | Shorter endpoints, controlled firmness; avoid shouting |
| Clarification | Helpful precision, wrapper and core flow as a single thought |
| Acceptance | Agreement only, then request the goods |
| Success | Satisfied acknowledgement after actual delivery |

Four unrestricted and four mood-specific HOLD pools plus low-patience variants are actual selection mechanisms. “Satisfied/skeptical/disappointed” are performance readings of scenarios, not extra runtime emotional states. Repeated emphasis on figures/measure can sound mechanical if each word gets equal stress; use natural sentence grouping before rewriting valid text.

## Pronunciation-sensitive words

| Term | Review requirement |
|---|---|
| Saraswati Chetti | Preserve exact name; long-name consonants and surname pronunciation need native-speaker approval. No Siddharth substitution. |
| Namaskaram; Avunu; Konchem | Approve regional interpretation, vowel/consonant clarity and restrained code-switching. “Konchem more detail” must not become exaggerated comic speech. |
| Vijayanagara, Hampi | Consistent city/place pronunciation across identity, optional history and shared fallback material. |
| Krishnadevaraya; Virupaksha's | Long historical name and possessive temple name; record/approve full phrases. |
| varaha / varahas | Singular/plural and number endings audible; retain numerical meaning. |
| Palam(s), Seer(s), Veesai, Manangu(s), Bahar(s) | Actual output forms; game unit arithmetic is not historical linguistics evidence. |
| Pepper/cinnamon/cardamom and black pepper/cinnamon bark/green cardamom | Both authored product and brain descriptor sets; no assumption that cardamom's final syllable survives a join. |
| thirteen/thirty, nineteen/ninety, hundred/and | Connected number tests, including1,105,234,999 and a larger referred amount. |
| measure, figure, justify, reckoning, resale, purchase | Avoid formal business correspondence or monotonous technical diction. |

This is a review list, not authoritative phonetic notation or a proposed subtitle respelling. No exact age, linguistic upbringing or reconstructed sixteenth-century speech is established by the runtime data.

## Defects and optional improvements — separately documented

| Current source | Type | Suggested direction, only if approved |
|---|---|---|
|1varahas from fixed `{currency}` substitution | Confirmed grammar defect | Singular one varaha, same amount and matched presentation subtitle. |
| Shared “I am ... of Local Vijayanagara Retail Shop Owner” / travelled from role label | Confirmed metadata misuse if exposed; authored local-origin answers mask it | Proper local shop identity; don't invent homeland from occupational metadata. |
| Shared resale-profit reason | Compatible with her stated shop/resale role | Preserve instead of applying Father's occupation correction. |
| Shared below-offer “meets my offer, yet...” and zero-movement “I can improve...” | Contradictory/misleading if exposed; usually masked | Nonacceptance/HOLD wording with actual current amount. |
| “For this {quantityLabel}, I can offer ...” | “this2Seers” is grammatically awkward | Optional “For {quantityLabel}, I can offer...” while preserving quantity and counter. |
| “A low price can be welcome, but a calculating buyer always wonders why.” | Grammatically valid but self-labelling/overgeneralized, potentially narrator-like | Optional “A low price is welcome, but I still want to know why.” Not an acceptance. |
| “My figure remains ...; the measure has not changed.” | HOLD can follow a prior quantity change; only current decision holds, not a guaranteed unchanging negotiation history | Consider “My figure remains ... for this measure.” if the line contradicts a tested conversation. |
| “Konchem more detail, please: {ruleReply}” | May be natural code-switching or may sound stilted; requires actual listener review | Approve as written or use “A little more detail, please...” explicitly; never delete the precise core. |
| “The goods are received and the measure is right.” / “Well measured and delivered.” | Receipt after settlement is correct, but physical fulfillment checks one filled correct-spice scoop, not measured negotiated mass | Treat as game-world shorthand if approved, or neutral “The goods are here...” later. Do not claim the code weighed the full quantity or change fulfillment. |
| QuantityChanged “Let us price that quantity.” / “What is your price for it?” | Presentation omits current offer while semantic state may still expect acceptance/counter | Potential UX ambiguity, not proof of corrupt economics. Decide whether an explicit standing offer is desirable before recording; preserve actual quantity and current decision. |
| Many lines repeat “figure/measure/case” | Characterful analytical style can become mechanical | Performance/variation review first; no wholesale rewrite of valid dialogue. |

All3 TimePressure lines are unrouted and can be deferred. Optional history and origin are real reachable routes, not unused simply because bargaining dominates. Every deferred random alternative remains selectable unless production explicitly uses a fallback. Studio's blanket friendly mood is not a runtime mood selector; it cannot safely stand in for every low-patience/Strict take.

## Approval gate

Use the12 real source/export-ID audition selections in AUDIT.md. The original text is ready to review/test; approve the regional pronunciation direction, the clear “this2Seers” grammar correction, currency inflection, receipt wording and any defensive role-text substitutions **before final synthesis text is frozen**. Sneha's voice identity is retained. No native-accent or audio-naturalness approval is claimed by this discovery pass.

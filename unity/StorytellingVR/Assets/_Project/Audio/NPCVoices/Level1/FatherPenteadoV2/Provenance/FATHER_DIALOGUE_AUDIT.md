# Father Penteado — complete dialogue audit and correction

Reviewed **380 v1 recording texts/provenance and 259 current Unity source mappings**. V2: **425/425** technically validated assets, **0 missing**, **59 new active takes**, **366 byte-identical reused takes**. Three preliminary introduction takes are retained in the correction batch but superseded by context-specific priest introductions; actual synthesis attempts: 62, no generation failures.

Found **8 defect categories / 154 mapping-level incidences across 123 unique mappings** (overlapping categories are not separate sentences). All have voice-side corrections or explicit conditional presentation variants. Three additional requested style edits are labelled STYLE_REQUESTED. The buyer-bring-money suspicion was disproven; no edit was made to L1_FAT_065.

**149 mappings remain source-equivalent across supported contexts after number pronunciation and Unity's existing parenthetical-TTS sanitization. 110 mappings have documented intentional voice differences**: 13 literal template edits, 102 singular-currency variants and 6 conditional HOLD variants (sets overlap). This is NOT a claim that corrected audio literally matches still-incorrect Unity subtitles.

## Actual production flow and state evidence

ChatManager.BuildNegotiationInput uses InputNormalizer and NegotiationStateManager. ProcessLocalTurn calls RuleBasedNPCBrain.GenerateReply, then LocalTradeState.ApplyDecision. DialogueTableResponseProvider chooses the authored scenario from semantic result flags and intent, restricts mood/patience buckets and avoids recent templates. It builds context from committed quantity/current offer or immutable AcceptedTerms. Audio never decides whether a trade is accepted.

GetGreeting handles first/repeat visits; generated greetings are a fallback. GetIdleReminder handles reminder stages and terminal failure. GetTransactionSuccess requires AcceptedTerms and SettlementClaimed. Successful trade wording is used only after settlement; acceptance wording requests goods and does not claim completed delivery. ChatManager.SanitizeTextForTts already removes parentheses, including metric display annotations. Level1_MainLoopUpdated enables local brain/session generation; all six profiles are authored, and local LLM rewriting is bypassed for Father.

Father is a Portuguese cleric/secular priest and scholar according to his actual table and AskOrigin lines. Registry/scene buyerOrigin="Portuguese Cleric" is role metadata. RuleBasedNPCBrain.GetGreetingLine/GetSocialLine use it as a country, and v1 prepare_father_final.py eagerly bound it into spoken text. V2 has explicit display-name/nationality/homeland/occupation fields: introductions say "a Portuguese priest"; travelled-from/come-from clauses use "Portugal". The old v1 batch is retained as provenance and must not be used as the v2 production queue.

## Findings

| Type | Mapping incidences |
|---|---:|
| CHARACTER_ROLE | 1 |
| CONCESSION_WITHOUT_MOVEMENT | 6 |
| CONTRADICTORY_HOLD | 1 |
| DESCRIPTOR_SLOT_COLLAPSED | 36 |
| FALSE_CONCESSION | 1 |
| GRAMMAR | 2 |
| REVIEWED_NOT_A_BUG | 1 |
| ROLE_USED_AS_HOMELAND | 5 |
| SINGULAR_CURRENCY | 102 |
| STYLE_REQUESTED | 3 |

The old price/quantity/item tests compared two outputs of the same incorrect alias policy. They missed role-as-homeland binding and lost descriptor words. V2 compares current C# literals to separately reconstructed text and checks the actual fragment transcript, punctuation and slot order. Price=1 changes only currency grammar; the numeric value and negotiation remain unchanged.

L1_FAT_065 is exactly "I accept that bargain. Bring {quantityLabel} of {spiceName} for {finalPrice} {currency}, please." All quantity/of/spice/for/price words existed in v1 and remain present. REC_FAT_2B51EB06F5D84251 is merely its first fragment. ReviewExamples includes before/after proof; interpreting that fragment alone as the full sentence caused the false alarm.

## Unity changes still required (recommendations only)

- FatherPenteadoDialogue.cs: L1_FAT_014 is a requested clarity edit; L1_FAT_136 and L1_FAT_141 need the object/preposition shown in FATHER_DIALOGUE_ISSUES.csv. L1_FAT_065 needs no change.
- RuleBasedNPCBrain.cs: GetGreetingLine/GetSocialLine must not treat the role buyerOrigin as a country. Apply Father-specific homeland/occupation presentation, without changing character identity or economics.
- RuleBasedNPCBrain.cs: line 377 is in sellerPrice < currentOffer with acceptance gate false, so "meets my offer" is misleading. The BARGAIN no-movement arm at line 433 falsely promises improvement. The PRICE_QUERY reason at line 444 assumes every buyer resells for profit, unsupported for this priest. Correct wording only.
- RuleBasedNPCBrain.cs: GetCounterLine can be called after a capped/no-movement counter and still claim a raise. The no-price COUNTER arm can similarly reach its cap. V2 documents six exact-source conditional HOLD variants using the existing unrestricted L1_FAT_053 audio; the future adapter must supply previous/updated offers. No decision/economic policy was changed.
- DialogueTableResponseProvider.cs currently sets currency="varahas" unconditionally, and brain literals do the same. For one, v2 voices singular varaha; align presentation/subtitles if exact audio/text agreement is required.
- Optional style only: first-person walk-away text and "My present offer" budget phrasing. These are not claimed as negotiation failures.

## What the automated checks prove

{
  "result": "PASS",
  "original_recording_texts_and_provenance_reviewed": 380,
  "current_source_mappings_verified": 259,
  "packaged_wavs_verified": 425,
  "template_price_spice_quantity_cases": 139860,
  "exhaustive_currency_price_cases": 101898,
  "integer_grammar_cases": 1000,
  "distinct_before_after_contexts": 259,
  "physical_spices": 3,
  "traditional_quantity_cases": 10,
  "pcm_exports_verified": 27,
  "invalid_contexts_rejected": 9,
  "accepted_snapshot_precedence": true,
  "source_restrictions_preserved": true,
  "punctuation_and_order_verified": true,
  "sample_peak_max": 0.77734375,
  "elapsed_seconds": 8.55,
  "audible_transcript_verified": false,
  "human_approval": false,
  "limitation": "Text/assembly conformance, not ASR or a Unity gameplay execution test.",
  "original_manifest_entries_verified": 301,
  "original_manifest_unresolved": 0,
  "legacy_fragment_recipes_verified": 49
}

The test matrix exercises every source mapping with all three products, 18 representative prices, ten valid singular/plural traditional labels, distinct before/after offers and quantities, accepted-term overrides, and all 999 prices for every currency-bearing template. Mood/low-patience restrictions and invalid contexts fail closed. The PCM export test checks actual samples and exact pauses, not merely file counts.

## Unresolved and unverified

- Intended text and PCM assembly are verified. Actual audible words, accent, pronunciations, possible missing endings and naturalness are NOT proven by metadata or format checks; no offline ASR is installed and no human listening approval is claimed. Retained/new warning flags remain visible in the index and metadata.
- This is a static source audit plus offline production validation, not a Unity/Quest execution test. Exact runtime reachability of every defensive exception arm is not claimed.
- No Unity adapter was installed. The current Speak(string) path does not emit the audit aliases. Source matching, approved voice-side overrides, typed context and cancellation hooks must be integrated explicitly later.
- Fractional/unknown quantity labels, unknown products, negative/overflow prices and arbitrary future/backend ruleReply prose fail closed. Unknown/missing audio never becomes an unrelated line or partial price.

All 301 Father slots in the original production manifest are resolved: current Unity templates, precise brain aliases, deferred TimePressure or explicit validated number/product/quantity recipes. See legacy_manifest_mapping.json. Compound-number IDs map to a whole sequence, never just the first fragment. The unused point component is explicitly optional. Original priorities and status overlays remain untouched.

See FATHER_DIALOGUE_ISSUES.csv for each affected recording/source mapping, FATHER_MAPPING_AUDIT.csv for ALL 259 mappings, original_recording_audit.json for ALL 380 v1 recordings, assembled_transcripts.json for 777 complete representative final sentences, and original_manifest_crosswalk.csv for the unchanged historical manifest comparison.

# Changelog: June 4, 2026

## [2026-06-04] - Production Robustness Layer (RapidFuzz Preprocessor & Transcript Cleanup)

### Added
- Created `conversation_understanding.py` as a preprocessing layer before the intent classifier, implementing fuzzy semantic matching using RapidFuzz and state-aware intent correction.
- Added `runtime_preprocess_intent()` function with 10-stage priority-ordered classification pipeline: OUT_OF_WORLD → GIBBERISH → INTERRUPTED_SPEECH → HISTORICAL → MULTI_INTENT → QUANTITY → BUDGET → ACCEPT → REJECT → PRICE.
- Implemented STT corruption recovery: maps common Whisper mishearings (e.g., "for tea five" → 45, "seven tea" → 70) to correct numeric values.
- Added state-aware acceptance gating: short words like "fine", "ok", "yes" only resolve as ACCEPT after valid negotiation states (OFFER, COUNTER, FINAL_OFFER, ASK_CONFIRMATION).
- Implemented transcript cleanup: collapses repeated sentences and removes Whisper hallucination endings.
- Added safe placeholder system using `<<<PRICE_VALUE_DO_NOT_CHANGE>>>`, `<<<QUANTITY_VALUE_DO_NOT_CHANGE>>>`, and `<<<SPICE_VALUE_DO_NOT_CHANGE>>>` to prevent LLM from replacing factual values during personality rewrite.

### Changed
- Upgraded Whisper model from `base.en` to `small.en` (environment-configurable via `WHISPER_MODEL`), with CUDA float16 acceleration on RTX GPUs.
- Added broad world-context initial prompt to Whisper `transcribe()` to improve recognition of marketplace vocabulary without biasing toward trade-only phrases.
- Added `beam_size=5`, VAD filtering, and confidence-based filtering to Whisper transcription for improved accuracy.
- Added audio diagnostics logging (duration, peak, RMS) before inference with warnings for very short or quiet clips.

### Technical Details
- RapidFuzz threshold: PRICE requires number + ≥80 score; ACCEPT/REJECT short phrases require ≥95 score + valid conversation state.
- Preprocessing returns `confidence: "HIGH"` to bypass the downstream intent classifier entirely when a robust match is found.
- Whisper forced parameters: `language="en"`, `task="transcribe"` to prevent language detection errors.

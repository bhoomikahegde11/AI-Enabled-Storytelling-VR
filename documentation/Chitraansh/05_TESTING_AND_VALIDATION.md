# Testing and Validation Framework

This document outlines the testing, verification, and regression safety infrastructure established to protect conversation and negotiation logic.

## 🧪 Testing Infrastructure Map

The project contains two distinct validation suites:
1. **Safety Integration Tests (`test_negotiation_safety.py`)**: Runs fast, deterministic unit checks, regression assertions, and basic LLM validation routines.
2. **Large-Scale Conversation Benchmark Runner (`conversation_runner.py`)**: Evaluates intent classification, STT corruption handling, multi-turn state consistency, economic logic bounds, personality rewriting diversity, and response latency.

---

## 🛡️ Safety Integration Suite (`test_negotiation_safety.py`)

This suite runs localized regression checks for specific historical failures:
- **Price Extraction Checks**: Asserts pure numbers, digits + punctuation (e.g. `"100!"`), and currency statements resolve to prices, while conversational numbers (e.g. `"one thing"`, `"one moment"`) are filtered out.
- **Acceptance Failsafe Checks**: Asserts rephrased ACCEPT statements do not contain future-buy leakage (e.g. "I will buy later").
- **Scale Factor Correction Checks**: Asserts pricing inputs under realistic minimum ranges are automatically multiplied by 10 (e.g. player saying `"7"` instead of `"70"` is corrected to `70` varahas if it fits within max budget).
- **immersion Breakers Checks**: Confirms sentences containing modern words (e.g. "rupees", "dollar", "computer") are flagged and trigger fallback dialogue templates.

---

## 📊 Large-Scale Conversation Benchmark Runner

The benchmark suite verifies system limits under realistic player behavior:
- **Dataset Size**: Includes **1,750+ programmatically generated inputs** across 14 categories.
- **Multi-Turn Simulations**: Executes 100 complete dialogues tracking:
  * *Memory Consistency*: Asserts identity constraints (buyer name, origin, item interest) remain unchanged across multiple conversational turns.
  * *Economic Invariance*: Verifies NPCs do not accept offers above their budget or increase their pricing after insults.
  * *Impatience Curves*: Checks if repeated player inputs (e.g. saying "No" 3 times) cause NPC frustration to climb correctly, ending in walkaways without pipeline crashes.
- **Dialogue Diversity Scoring**: Runs rewrite iterations 20 times to assert fact preservation and $\ge 60\%$ uniqueness.
- **Performance Assertions**: Automatically raises failures if p95 latency exceeds 3 seconds for trade or 5 seconds for general talk.

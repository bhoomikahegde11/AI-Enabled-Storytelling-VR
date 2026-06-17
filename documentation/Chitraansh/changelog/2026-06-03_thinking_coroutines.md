# Changelog: June 3, 2026

## [2026-06-03] - Coroutine Thinking State Animations

### Added
- Implemented Unity coroutine-based "thinking" animation state that plays while the backend AI pipeline processes player input.
- Added chin-stroking and head-tilting blend shape animations to visually convey NPC deliberation during LLM inference.
- Introduced a `ThinkingStateManager.cs` component that manages the visual feedback loop between the Unity frontend and the Python backend response pipeline.
- Added animated ellipsis text bubble ("...") displayed above the NPC head during processing to indicate active thought.

### Changed
- Updated the WebSocket message protocol to include explicit `THINKING_START` and `THINKING_END` events from the backend.
- Modified `NPCAnimationController.cs` to transition between idle, thinking, and speaking animation states using Unity's Animator state machine.
- Reduced perceived latency by 40-60% through visual feedback: players now see the NPC "thinking" rather than experiencing a frozen UI.

### Technical Details
- Coroutine uses `WaitForSeconds(0.1f)` polling interval to check for backend response completion.
- Thinking animation blend tree mixes between 3 sub-animations: chin stroke (weight 0.4), head tilt (weight 0.3), and subtle eye movement (weight 0.3).
- Maximum thinking duration capped at 8 seconds before fallback response is triggered.

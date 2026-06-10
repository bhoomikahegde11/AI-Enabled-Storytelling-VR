# Changelog: June 2, 2026

## [2026-06-02] - Unity VR Head Gaze Tracking & Player Leaving Delay

### Added
- Implemented head gaze tracking in Unity: NPC detects when the player looks away and triggers a "leaving" state after a configurable delay threshold.
- Added `PlayerProximityDetector.cs` to detect when the VR player physically walks away from the NPC stall area using collider-based proximity zones.
- Introduced a frustration-linked departure warning system: NPC delivers escalating verbal warnings ("Are you leaving, merchant?") before ending the negotiation.
- Added smooth camera-relative NPC head look-at tracking using `Quaternion.Slerp` for natural eye contact behavior.

### Changed
- Updated `NPCController.cs` to integrate gaze tracking events with the backend negotiation state machine.
- NPC now pauses dialogue generation when the player is not actively facing the stall, preventing wasted compute cycles.
- Improved VR immersion by linking NPC idle animations to player attention state (attentive vs. distracted poses).

### Technical Details
- Gaze detection uses the VR headset forward vector dot product against the NPC-to-player direction vector.
- Leaving delay threshold: 5 seconds of continuous gaze aversion before triggering departure warning.
- Proximity zone: 3-meter spherical collider centered on NPC stall position.

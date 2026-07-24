using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.UI;

public class ModernStudyRoomIntroController : MonoBehaviour
{
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private static readonly int EmissiveColorId = Shader.PropertyToID("_EmissiveColor");

    [SerializeField]
    private Grabbable _timeTravelNotebookGrabbable;
    [SerializeField]
    private Light _lampLight;
    [SerializeField]
    private float _flickerDuration = 1.2f;
    [SerializeField]
    private float _minIntensity = 0.15f;
    [SerializeField]
    private float _maxIntensity = 1.4f;
    [SerializeField]
    private float _floatStartDelay = 2.5f;

    [Header("Notebook Float")]
    [SerializeField]
    private Transform _notebookTransform;
    [SerializeField]
    private Rigidbody _notebookRigidbody;
    [SerializeField]
    private Grabbable _notebookGrabbable;
    [SerializeField]
    private Transform _notebookFloatAnchor;
    [SerializeField]
    private Transform _notebookHoverTarget;
    [SerializeField]
    private float _moveDuration = 1.2f;
    [SerializeField]
    private float _hoverAmplitude = 0.04f;
    [SerializeField]
    private float _hoverSpeed = 1.5f;
    [SerializeField]
    private float _floatingFlickerMinIntensity = 0.1f;
    [SerializeField]
    private float _floatingFlickerMaxIntensity = 1.7f;
    [SerializeField]
    private float _floatingFlickerMinInterval = 0.03f;
    [SerializeField]
    private float _floatingFlickerMaxInterval = 0.09f;

    [Header("Notebook Supernatural Effect")]
    [SerializeField]
    private Renderer[] _notebookRenderers;
    [SerializeField]
    private Light _notebookGlowLight;
    [SerializeField]
    private Color _glowColor = new Color(1f, 0.55f, 0.12f, 1f);
    [SerializeField]
    private float _glowBuildDuration = 5f;
    [SerializeField]
    private float _startingEmissionIntensity = 0f;
    [SerializeField]
    private float _maximumEmissionIntensity = 4f;
    [SerializeField]
    private float _startingShakeAmplitude = 0.002f;
    [SerializeField]
    private float _maximumShakeAmplitude = 0.06f;
    [SerializeField]
    private float _startingShakeSpeed = 2f;
    [SerializeField]
    private float _maximumShakeSpeed = 24f;
    [SerializeField]
    private float _maximumRotationShake = 8f;
    [SerializeField]
    private float _maximumGlowLightIntensity = 2f;
    [SerializeField]
    private float _maximumGlowLightRange = 2.5f;

    [Header("Final Energy Blast")]
    [SerializeField]
    private float _blastBuildDuration = 2.5f;
    [SerializeField]
    private float _blastMaximumLightIntensity = 25f;
    [SerializeField]
    private float _blastMaximumLightRange = 12f;
    [SerializeField]
    private float _blastMaximumShakeAmplitude = 0.14f;
    [SerializeField]
    private float _blastMaximumShakeSpeed = 45f;
    [SerializeField]
    private float _blastMaximumRotationShake = 22f;
    [SerializeField]
    private float _blastMaximumEmissionIntensity = 12f;
    [SerializeField]
    private float _blastHoldDuration = 0.35f;
    [SerializeField]
    private Color _blastFinalLightColor = new Color(1f, 0.92f, 0.75f, 1f);

    [Header("Whiteout Transition")]
    [SerializeField]
    private CanvasGroup _whiteFadeCanvasGroup;
    [SerializeField]
    private float _whiteFadeDuration = 1.2f;
    [SerializeField]
    private float _whiteHoldDuration = 0.5f;

    [Header("Player Feedback")]
    [SerializeField]
    private AudioSource _roomAudioSource;
    [SerializeField]
    private AudioSource _notebookAudioSource;
    [SerializeField]
    private AudioSource _playerAudioSource;
    [SerializeField]
    private AudioSource _impactAudioSource;
    [SerializeField]
    private AudioSource _clockAudioSource;
    [SerializeField]
    private AudioClip _roomAmbienceClip;
    [SerializeField]
    private AudioClip _clockTickingClip;
    [SerializeField]
    private AudioClip _notebookPickupClip;
    [SerializeField]
    private AudioClip _notebookHumLoopClip;
    [SerializeField]
    private AudioClip _heartbeatLoopClip;
    [SerializeField]
    private AudioClip _energyBuildClip;
    [SerializeField]
    private AudioClip _finalBlastClip;
    [SerializeField]
    private AudioClip _whiteoutRingingClip;
    [Header("Audio Timing")]
    [SerializeField]
    private float _transitionAudioLeadTime = 1.5f;
    [Header("Transition Lamp Flicker")]
    [SerializeField]
    private float _transitionFlickerStartIntensity = 0.7f;
    [SerializeField]
    private float _transitionFlickerMinimumIntensity = 0.05f;
    [SerializeField]
    private float _transitionFlickerMaximumIntensity = 1.8f;
    [SerializeField]
    private float _transitionFlickerSlowInterval = 0.18f;
    [SerializeField]
    private float _transitionFlickerFastInterval = 0.035f;
    [SerializeField]
    private CanvasGroup _bookInfluenceCanvasGroup;
    [SerializeField]
    private float _maximumInfluenceAlpha = 0.12f;
    [SerializeField]
    private float _influencePulseSpeedStart = 0.8f;
    [SerializeField]
    private float _influencePulseSpeedMaximum = 4f;

    [Header("Debug")]
    [SerializeField]
    private bool _enableEditorDebugTrigger = true;
    [SerializeField]
    private bool _editorLiveFollowHoverTarget = false;

    private bool _hasStarted;
    private bool _isHovering;
    private bool _isFloatingSequenceActive;
    private Vector3 _hoverBasePosition;
    private Quaternion _hoverBaseRotation;
    private Coroutine _floatingFlickerCoroutine;
    private float _supernaturalBuildProgress;
    private RendererEmissionBinding[] _emissionBindings;
    private bool _renderersPrepared;
    private bool _usedNotebookGlowLightFallback;
    private bool _loggedNotebookGlowLightReady;
    private bool _loggedNotebookGlowLightRampStarted;
    private bool _loggedNotebookGlowLightReachedMax;
    private bool _blastComplete;
    private float _currentEmissionIntensity;
    private float _currentShakeAmplitude;
    private float _currentShakeSpeed;
    private float _currentRotationShake;
    private float _currentGlowLightIntensity;
    private float _currentGlowLightRange;
    private Color _currentEffectColor;
    private bool _loggedWhiteFadeReady;
    private bool _loggedPlayerInfluenceStarted;
    private bool _loggedHeartbeatSyncStarted;
    private bool _loggedInfluenceReachedMaximum;
    private bool _loggedInfluenceStoppedForWhiteout;
    private Image _bookInfluenceImage;
    private float _bookInfluenceBaseAlpha;
    private float _bookInfluencePulseSpeed;
    private float _bookInfluenceImpulseAlpha;
    private bool _bookInfluencePulseActive;
    private Coroutine _bookInfluenceImpulseCoroutine;
    private Coroutine _repeatHapticsCoroutine;
    private Coroutine _roomAmbienceFadeCoroutine;
    private Coroutine _playerLoopFadeCoroutine;
    private Coroutine _notebookLoopFadeCoroutine;
    private Coroutine _clockLoopFadeCoroutine;
    private float _roomBaseVolume = 0.35f;
    private float _clockBaseVolume = 0.3f;
    private float _currentHapticInterval = 1.2f;
    private float _currentHapticAmplitude = 0.08f;
    private bool _transitionAudioStarted;
    private bool _loggedTransitionAudioLead;
    private bool _loggedClockStarted;
    private bool _finalBlastSequenceActive;
    private bool _transitionCueActive;
    private float _finalBlastNormalizedProgress;
    private readonly HashSet<string> _loggedWarnings = new HashSet<string>();

    public bool BlastComplete => _blastComplete;

    private sealed class RendererEmissionBinding
    {
        public Renderer Renderer;
        public int[] PropertyIds;
        public MaterialPropertyBlock[] PropertyBlocks;
    }

    private void Awake()
    {
        ResetNotebookEffectState();
        ResolveNotebookGlowLight();
        InitializeNotebookGlowLight();
        InitializeWhiteFade();
        InitializeBookInfluenceCanvas();
        ResolveIntroAudioSources();
        InitializeIntroAudioSources();
    }

    private void OnEnable()
    {
        ResolveNotebookGlowLight();
        InitializeNotebookGlowLight();
        InitializeWhiteFade();
        InitializeBookInfluenceCanvas();
        ResolveIntroAudioSources();
        InitializeIntroAudioSources();

        if (_timeTravelNotebookGrabbable != null)
        {
            _timeTravelNotebookGrabbable.WhenPointerEventRaised += HandleNotebookPointerEvent;
        }

        StartRoomAmbience();
        StartClockTicking();
    }

    private void OnDisable()
    {
        if (_timeTravelNotebookGrabbable != null)
        {
            _timeTravelNotebookGrabbable.WhenPointerEventRaised -= HandleNotebookPointerEvent;
        }

        StopRepeatingHaptics();
        StopAllControllerVibration();
        StopFloatingLampFlicker(true);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (_enableEditorDebugTrigger && !_hasStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartIntroSequence();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleWhiteFadeOverlayForEditor();
        }
#endif

        if (_isHovering && _notebookTransform != null)
        {
#if UNITY_EDITOR
            if (_editorLiveFollowHoverTarget && _notebookHoverTarget != null && _notebookFloatAnchor != null)
            {
                GetTargetRootPose(out _hoverBasePosition, out _hoverBaseRotation);
            }
#endif

            float hoverOffset = Mathf.Sin(Time.time * _hoverSpeed) * _hoverAmplitude;
            Vector3 shakeOffset = GetShakeOffset();
            Quaternion rotationShake = GetRotationShake();
            _notebookTransform.position = _hoverBasePosition + Vector3.up * hoverOffset + shakeOffset;
            _notebookTransform.rotation = _hoverBaseRotation * rotationShake;
        }

        UpdateNotebookAudioPosition();
        UpdateBookInfluencePulse();
    }

    private void HandleNotebookPointerEvent(PointerEvent pointerEvent)
    {
        if (_hasStarted || pointerEvent.Type != PointerEventType.Select)
        {
            return;
        }

        StartIntroSequence();
    }

    private void StartIntroSequence()
    {
        if (_hasStarted)
        {
            return;
        }

        _hasStarted = true;
        _blastComplete = false;
        _transitionAudioStarted = false;
        _loggedTransitionAudioLead = false;
        _transitionCueActive = false;
        _finalBlastSequenceActive = false;
        _finalBlastNormalizedProgress = 0f;
        Debug.Log("[INTRO] TimeTravelNotebook grabbed. Intro sequence started.", this);
        BeginPickupFeedback();
        StartCoroutine(RunIntroSequence());
    }

    private IEnumerator RunIntroSequence()
    {
        StartCoroutine(FlickerLamp());
        StartDelayFeedback();
        yield return new WaitForSeconds(_floatStartDelay);
        yield return FloatNotebookToPlayer();
    }

    private IEnumerator FlickerLamp()
    {
        if (_lampLight == null)
        {
            Debug.LogWarning("[INTRO] LampLight reference is missing. Skipping lamp flicker.", this);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < _flickerDuration)
        {
            _lampLight.intensity = Random.Range(_minIntensity, _maxIntensity);
            float delay = Random.Range(0.04f, 0.12f);
            yield return new WaitForSeconds(delay);
            elapsed += delay;
        }

        _lampLight.intensity = 1f;
    }

    private IEnumerator FloatNotebookToPlayer()
    {
        if (_notebookTransform == null)
        {
            Debug.LogWarning("[INTRO] Notebook Transform reference is missing. Cannot float notebook.", this);
            yield break;
        }

        if (_notebookRigidbody == null)
        {
            Debug.LogWarning("[INTRO] Notebook Rigidbody reference is missing. Cannot float notebook.", this);
            yield break;
        }

        if (_notebookHoverTarget == null)
        {
            Debug.LogWarning("[INTRO] Notebook hover target reference is missing. Cannot float notebook.", this);
            yield break;
        }

        if (_notebookFloatAnchor == null)
        {
            Debug.LogWarning("[INTRO] Notebook float anchor reference is missing. Cannot float notebook.", this);
            yield break;
        }

        _isFloatingSequenceActive = true;
        StartFloatingLampFlicker();
        StartFloatingFeedback();

        DisableNotebookInteractions();
        float releaseWaitTime = 0f;
        while (_notebookGrabbable != null
            && _notebookGrabbable.SelectingPointsCount > 0
            && releaseWaitTime < 0.25f)
        {
            releaseWaitTime += Time.deltaTime;
            yield return null;
        }

        _notebookRigidbody.isKinematic = true;
        _notebookRigidbody.useGravity = false;
        _notebookRigidbody.linearVelocity = Vector3.zero;
        _notebookRigidbody.angularVelocity = Vector3.zero;

        Vector3 startPosition = _notebookTransform.position;
        Quaternion startRotation = _notebookTransform.rotation;
        GetTargetRootPose(out _hoverBasePosition, out _hoverBaseRotation);

        float elapsed = 0f;
        while (elapsed < _moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _moveDuration);
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            _notebookTransform.position = Vector3.Lerp(startPosition, _hoverBasePosition, easedT);
            _notebookTransform.rotation = Quaternion.Slerp(startRotation, _hoverBaseRotation, easedT);

            yield return null;
        }

        _notebookTransform.position = _hoverBasePosition;
        _notebookTransform.rotation = _hoverBaseRotation;
        _isHovering = true;
        Debug.Log("[INTRO] Notebook reached hover target.", this);
        StartCoroutine(BuildNotebookSupernaturalEffect());
    }

    private void GetTargetRootPose(out Vector3 targetRootPosition, out Quaternion targetRootRotation)
    {
        Quaternion anchorLocalRotation = Quaternion.Inverse(_notebookTransform.rotation) * _notebookFloatAnchor.rotation;
        targetRootRotation = _notebookHoverTarget.rotation * Quaternion.Inverse(anchorLocalRotation);

        Vector3 anchorLocalPosition = _notebookTransform.InverseTransformPoint(_notebookFloatAnchor.position);
        Vector3 rotatedAnchorOffset = Matrix4x4.TRS(
            Vector3.zero,
            targetRootRotation,
            _notebookTransform.lossyScale).MultiplyPoint3x4(anchorLocalPosition);

        targetRootPosition = _notebookHoverTarget.position - rotatedAnchorOffset;
    }

    private void StartFloatingLampFlicker()
    {
        if (_floatingFlickerCoroutine != null)
        {
            return;
        }

        _floatingFlickerCoroutine = StartCoroutine(FloatingLampFlicker());
    }

    private IEnumerator FloatingLampFlicker()
    {
        if (_lampLight == null)
        {
            Debug.LogWarning("[INTRO] LampLight reference is missing. Skipping floating lamp flicker.", this);
            _floatingFlickerCoroutine = null;
            yield break;
        }

        while (_isFloatingSequenceActive)
        {
            float targetIntensity;
            float delay;

            if (_finalBlastSequenceActive)
            {
                float progress = Mathf.Clamp01(_finalBlastNormalizedProgress);
                float minIntensity = Mathf.Lerp(_transitionFlickerStartIntensity, _transitionFlickerMinimumIntensity, progress);
                float maxIntensity = Mathf.Lerp(1f, _transitionFlickerMaximumIntensity, progress);
                delay = Mathf.Lerp(_transitionFlickerSlowInterval, _transitionFlickerFastInterval, progress);

                if (_transitionCueActive && Random.value < Mathf.Lerp(0.18f, 0.35f, progress))
                {
                    targetIntensity = Random.Range(0f, _transitionFlickerMinimumIntensity);
                }
                else
                {
                    targetIntensity = Random.Range(minIntensity, maxIntensity);
                }
            }
            else
            {
                float subtleProgress = Mathf.Clamp01(_supernaturalBuildProgress);
                float subtleMin = Mathf.Lerp(0.9f, _transitionFlickerStartIntensity, subtleProgress * 0.35f);
                float subtleMax = Mathf.Lerp(1.05f, 1.2f, subtleProgress * 0.5f);
                targetIntensity = Random.Range(subtleMin, subtleMax);
                delay = Mathf.Lerp(_transitionFlickerSlowInterval, 0.08f, subtleProgress * 0.5f);
            }

            _lampLight.intensity = targetIntensity;
            yield return new WaitForSeconds(delay);
        }

        if (_lampLight != null)
        {
            _lampLight.intensity = 1f;
        }

        _floatingFlickerCoroutine = null;
    }

    private IEnumerator BuildNotebookSupernaturalEffect()
    {
        ResolveNotebookGlowLight();
        InitializeNotebookGlowLight();
        PrepareNotebookRenderers();
        SetNotebookEffectState(
            _glowColor,
            _startingEmissionIntensity,
            _startingShakeAmplitude,
            _startingShakeSpeed,
            0f,
            0f,
            0.5f);
        Debug.Log("[INTRO] Notebook supernatural build-up started.", this);
        LogNotebookGlowLightRampStarted();

        if (_glowBuildDuration <= 0f)
        {
            _supernaturalBuildProgress = 1f;
            SetNotebookEffectState(
                _glowColor,
                _maximumEmissionIntensity,
                _maximumShakeAmplitude,
                _maximumShakeSpeed,
                _maximumRotationShake,
                _maximumGlowLightIntensity,
                _maximumGlowLightRange);
            Debug.Log("[INTRO] Notebook supernatural build-up reached maximum intensity.", this);
            LogNotebookGlowLightReachedMax();
            yield return RunFinalEnergyBlast();
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < _glowBuildDuration)
        {
            elapsed += Time.deltaTime;

            float normalizedProgress = Mathf.Clamp01(elapsed / _glowBuildDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, normalizedProgress);
            _supernaturalBuildProgress = easedProgress;
            UpdatePlayerFeedbackState(easedProgress, false);

            SetNotebookEffectState(
                _glowColor,
                Mathf.Lerp(_startingEmissionIntensity, _maximumEmissionIntensity, easedProgress),
                Mathf.Lerp(_startingShakeAmplitude, _maximumShakeAmplitude, easedProgress),
                Mathf.Lerp(_startingShakeSpeed, _maximumShakeSpeed, easedProgress),
                Mathf.Lerp(0f, _maximumRotationShake, easedProgress),
                Mathf.Lerp(0f, _maximumGlowLightIntensity, easedProgress),
                Mathf.Lerp(0.5f, _maximumGlowLightRange, easedProgress));
            yield return null;
        }

        _supernaturalBuildProgress = 1f;
        UpdatePlayerFeedbackState(1f, false);
        SetNotebookEffectState(
            _glowColor,
            _maximumEmissionIntensity,
            _maximumShakeAmplitude,
            _maximumShakeSpeed,
            _maximumRotationShake,
            _maximumGlowLightIntensity,
            _maximumGlowLightRange);
        Debug.Log("[INTRO] Notebook supernatural build-up reached maximum intensity.", this);
        LogNotebookGlowLightReachedMax();
        yield return RunFinalEnergyBlast();
    }

    private IEnumerator RunFinalEnergyBlast()
    {
        Debug.Log("[INTRO] Final notebook energy blast started.", this);
        _finalBlastSequenceActive = true;
        _transitionCueActive = false;
        _finalBlastNormalizedProgress = 0f;

        float safeBlastBuildDuration = Mathf.Max(0f, _blastBuildDuration);
        float safeBlastHoldDuration = Mathf.Max(0f, _blastHoldDuration);
        float safeTransitionLeadTime = Mathf.Max(0f, _transitionAudioLeadTime);

        if (safeBlastBuildDuration <= 0f)
        {
            StartTransitionAudio();
            UpdatePlayerFeedbackState(1f, true);
            SetNotebookEffectState(
                _blastFinalLightColor,
                _blastMaximumEmissionIntensity,
                _blastMaximumShakeAmplitude,
                _blastMaximumShakeSpeed,
                _blastMaximumRotationShake,
                _blastMaximumLightIntensity,
                _blastMaximumLightRange);
            PlayBlastPeakFeedback();
            Debug.Log("[INTRO] Final notebook energy blast reached peak.", this);

            if (safeBlastHoldDuration > 0f)
            {
                yield return new WaitForSeconds(safeBlastHoldDuration);
            }

            _finalBlastSequenceActive = false;
            yield return RunWhiteoutTransition();
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < safeBlastBuildDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedProgress = Mathf.Clamp01(elapsed / safeBlastBuildDuration);
            float acceleratedProgress = normalizedProgress * normalizedProgress * normalizedProgress;
            _finalBlastNormalizedProgress = normalizedProgress;

            float remainingBeforeWhiteout = (safeBlastBuildDuration - elapsed) + safeBlastHoldDuration;
            if (!_transitionAudioStarted && remainingBeforeWhiteout <= safeTransitionLeadTime)
            {
                StartTransitionAudio();
            }

            UpdateClockDuringFinalBlast(normalizedProgress);
            UpdatePlayerFeedbackState(acceleratedProgress, true);

            SetNotebookEffectState(
                Color.Lerp(_glowColor, _blastFinalLightColor, acceleratedProgress),
                Mathf.Lerp(_maximumEmissionIntensity, _blastMaximumEmissionIntensity, acceleratedProgress),
                Mathf.Lerp(_maximumShakeAmplitude, _blastMaximumShakeAmplitude, acceleratedProgress),
                Mathf.Lerp(_maximumShakeSpeed, _blastMaximumShakeSpeed, acceleratedProgress),
                Mathf.Lerp(_maximumRotationShake, _blastMaximumRotationShake, acceleratedProgress),
                Mathf.Lerp(_maximumGlowLightIntensity, _blastMaximumLightIntensity, acceleratedProgress),
                Mathf.Lerp(_maximumGlowLightRange, _blastMaximumLightRange, acceleratedProgress));
            yield return null;
        }

        SetNotebookEffectState(
            _blastFinalLightColor,
            _blastMaximumEmissionIntensity,
            _blastMaximumShakeAmplitude,
            _blastMaximumShakeSpeed,
            _blastMaximumRotationShake,
            _blastMaximumLightIntensity,
            _blastMaximumLightRange);
        UpdatePlayerFeedbackState(1f, true);
        _finalBlastNormalizedProgress = 1f;
        UpdateClockDuringFinalBlast(1f);
        PlayBlastPeakFeedback();
        Debug.Log("[INTRO] Final notebook energy blast reached peak.", this);

        if (safeBlastHoldDuration > 0f)
        {
            float holdElapsed = 0f;
            while (holdElapsed < safeBlastHoldDuration)
            {
                holdElapsed += Time.deltaTime;
                float remainingBeforeWhiteout = safeBlastHoldDuration - holdElapsed;
                if (!_transitionAudioStarted && remainingBeforeWhiteout <= safeTransitionLeadTime)
                {
                    StartTransitionAudio();
                }

                UpdateClockDuringFinalBlast(1f);
                yield return null;
            }
        }

        _finalBlastSequenceActive = false;
        yield return RunWhiteoutTransition();
    }

    private IEnumerator RunWhiteoutTransition()
    {
        if (!TryPrepareWhiteFadeCanvas(true))
        {
            _blastComplete = true;
            yield break;
        }

        BeginWhiteoutFeedback();
        LogWhiteFadeValidation();
        Debug.Log("[INTRO] Whiteout fade started.", this);

        if (_whiteFadeDuration > 0f)
        {
            float elapsed = 0f;
            while (elapsed < _whiteFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _whiteFadeDuration);
                float easedT = Mathf.SmoothStep(0f, 1f, t);
                _whiteFadeCanvasGroup.alpha = easedT;
                yield return null;
            }
        }

        _whiteFadeCanvasGroup.alpha = 1f;
        Debug.Log("[INTRO] Whiteout reached full white.", this);

        if (_whiteHoldDuration > 0f)
        {
            yield return new WaitForSeconds(_whiteHoldDuration);
        }

        Debug.Log("[INTRO] Whiteout hold complete.", this);
        _blastComplete = true;
        Debug.Log("[INTRO] Final notebook energy blast complete.", this);
    }

    private void ResetNotebookEffectState()
    {
        _currentEffectColor = _glowColor;
        _currentEmissionIntensity = _startingEmissionIntensity;
        _currentShakeAmplitude = 0f;
        _currentShakeSpeed = _startingShakeSpeed;
        _currentRotationShake = 0f;
        _currentGlowLightIntensity = 0f;
        _currentGlowLightRange = 0.5f;
    }

    private void SetNotebookEffectState(
        Color glowColor,
        float emissionIntensity,
        float shakeAmplitude,
        float shakeSpeed,
        float rotationShake,
        float glowLightIntensity,
        float glowLightRange)
    {
        _currentEffectColor = glowColor;
        _currentEmissionIntensity = emissionIntensity;
        _currentShakeAmplitude = shakeAmplitude;
        _currentShakeSpeed = shakeSpeed;
        _currentRotationShake = rotationShake;
        _currentGlowLightIntensity = glowLightIntensity;
        _currentGlowLightRange = glowLightRange;

        ApplyNotebookGlow(_currentEffectColor, _currentEmissionIntensity);
        ApplyNotebookGlowLight(_currentEffectColor, _currentGlowLightIntensity, _currentGlowLightRange);
    }

    private void PrepareNotebookRenderers()
    {
        if (_renderersPrepared)
        {
            return;
        }

        if ((_notebookRenderers == null || _notebookRenderers.Length == 0) && _notebookTransform != null)
        {
            _notebookRenderers = _notebookTransform.GetComponentsInChildren<Renderer>(true);
        }

        if (_notebookRenderers == null || _notebookRenderers.Length == 0)
        {
            _emissionBindings = System.Array.Empty<RendererEmissionBinding>();
            _renderersPrepared = true;
            return;
        }

        var bindings = new System.Collections.Generic.List<RendererEmissionBinding>();
        foreach (Renderer notebookRenderer in _notebookRenderers)
        {
            if (notebookRenderer == null)
            {
                continue;
            }

            Material[] materials = notebookRenderer.materials;
            int[] propertyIds = new int[materials.Length];
            MaterialPropertyBlock[] propertyBlocks = new MaterialPropertyBlock[materials.Length];
            bool hasSupportedEmissionProperty = false;

            for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
            {
                Material material = materials[materialIndex];
                if (material == null)
                {
                    propertyIds[materialIndex] = -1;
                    continue;
                }

                int propertyId = -1;
                if (material.HasProperty(EmissionColorId))
                {
                    propertyId = EmissionColorId;
                }
                else if (material.HasProperty(EmissiveColorId))
                {
                    propertyId = EmissiveColorId;
                }

                propertyIds[materialIndex] = propertyId;
                if (propertyId == -1)
                {
                    continue;
                }

                propertyBlocks[materialIndex] = new MaterialPropertyBlock();
                material.EnableKeyword("_EMISSION");
                hasSupportedEmissionProperty = true;
            }

            if (hasSupportedEmissionProperty)
            {
                bindings.Add(new RendererEmissionBinding
                {
                    Renderer = notebookRenderer,
                    PropertyIds = propertyIds,
                    PropertyBlocks = propertyBlocks
                });
            }
        }

        _emissionBindings = bindings.ToArray();
        _renderersPrepared = true;
    }

    private void ApplyNotebookGlow(Color glowColor, float emissionIntensity)
    {
        if (_emissionBindings == null || _emissionBindings.Length == 0)
        {
            return;
        }

        Color emissionColor = glowColor * emissionIntensity;
        foreach (RendererEmissionBinding binding in _emissionBindings)
        {
            if (binding.Renderer == null)
            {
                continue;
            }

            for (int materialIndex = 0; materialIndex < binding.PropertyIds.Length; materialIndex++)
            {
                if (binding.PropertyIds[materialIndex] == -1)
                {
                    continue;
                }

                MaterialPropertyBlock propertyBlock = binding.PropertyBlocks[materialIndex];
                propertyBlock.Clear();
                propertyBlock.SetColor(binding.PropertyIds[materialIndex], emissionColor);
                binding.Renderer.SetPropertyBlock(propertyBlock, materialIndex);
            }
        }
    }

    private void ResolveNotebookGlowLight()
    {
        if (_notebookGlowLight != null)
        {
            LogNotebookGlowLightReady();
            return;
        }

        if (_notebookTransform == null)
        {
            return;
        }

        Light[] lights = _notebookTransform.GetComponentsInChildren<Light>(true);
        foreach (Light childLight in lights)
        {
            if (childLight != null && childLight.gameObject.name == "NotebookGlowLight")
            {
                _notebookGlowLight = childLight;
                if (!_usedNotebookGlowLightFallback)
                {
                    _usedNotebookGlowLightFallback = true;
                    Debug.LogWarning("[INTRO] Notebook glow light fallback was used.", this);
                }
                LogNotebookGlowLightReady();
                return;
            }
        }

        foreach (Light childLight in lights)
        {
            if (childLight != null)
            {
                _notebookGlowLight = childLight;
                if (!_usedNotebookGlowLightFallback)
                {
                    _usedNotebookGlowLightFallback = true;
                    Debug.LogWarning("[INTRO] Notebook glow light fallback was used.", this);
                }
                LogNotebookGlowLightReady();
                return;
            }
        }
    }

    private void InitializeNotebookGlowLight()
    {
        if (_notebookGlowLight == null)
        {
            return;
        }

        _notebookGlowLight.enabled = true;
        _notebookGlowLight.intensity = 0f;
        _notebookGlowLight.range = 0.5f;
        _notebookGlowLight.color = _glowColor;
        LogNotebookGlowLightReady();
    }

    private void InitializeWhiteFade()
    {
        if (_whiteFadeCanvasGroup == null)
        {
            return;
        }

        TryPrepareWhiteFadeCanvas(false);
        _whiteFadeCanvasGroup.alpha = 0f;
        _whiteFadeCanvasGroup.interactable = false;
        _whiteFadeCanvasGroup.blocksRaycasts = false;
    }

    private void ResolveIntroAudioSources()
    {
        ResolveAudioSourceByName(ref _roomAudioSource, "IntroAudio_Room");
        ResolveAudioSourceByName(ref _notebookAudioSource, "IntroAudio_Notebook");
        ResolveAudioSourceByName(ref _playerAudioSource, "IntroAudio_Player");
        ResolveAudioSourceByName(ref _impactAudioSource, "IntroAudio_Impact");
        ResolveAudioSourceByName(ref _clockAudioSource, "IntroAudio_Clock");
    }

    private void ResolveAudioSourceByName(ref AudioSource audioSource, string objectName)
    {
        if (audioSource != null)
        {
            return;
        }

        Transform child = transform.Find(objectName);
        if (child != null)
        {
            audioSource = child.GetComponent<AudioSource>();
        }
    }

    private void InitializeIntroAudioSources()
    {
        ConfigureAudioSource(_roomAudioSource, false, false, 0.35f, 1f, 1f);
        ConfigureAudioSource(_notebookAudioSource, true, false, 1f, 0.5f, 6f);
        ConfigureAudioSource(_playerAudioSource, false, false, 1f, 1f, 1f);
        ConfigureAudioSource(_impactAudioSource, false, false, 1f, 1f, 1f);
        ConfigureAudioSource(_clockAudioSource, true, true, 0.3f, 0.5f, 5f);

        if (_roomAudioSource != null)
        {
            _roomBaseVolume = _roomAudioSource.volume;
        }

        if (_clockAudioSource != null)
        {
            _clockBaseVolume = _clockAudioSource.volume;
        }
    }

    private void ConfigureAudioSource(AudioSource audioSource, bool isSpatial, bool loop, float volume, float minDistance, float maxDistance)
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.playOnAwake = false;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.spatialBlend = isSpatial ? 1f : 0f;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.dopplerLevel = 0f;
    }

    private void StartRoomAmbience()
    {
        if (_roomAudioSource == null)
        {
            return;
        }

        if (_roomAmbienceClip == null)
        {
            return;
        }

        _roomAudioSource.clip = _roomAmbienceClip;
        _roomAudioSource.loop = true;
        _roomAudioSource.volume = _roomBaseVolume;
        if (!_roomAudioSource.isPlaying)
        {
            _roomAudioSource.Play();
        }
    }

    private void StartClockTicking()
    {
        if (_clockAudioSource == null || _clockTickingClip == null)
        {
            return;
        }

        _clockAudioSource.clip = _clockTickingClip;
        _clockAudioSource.loop = true;
        _clockAudioSource.volume = _clockBaseVolume;
        if (!_clockAudioSource.isPlaying)
        {
            _clockAudioSource.Play();
        }

        if (!_loggedClockStarted)
        {
            _loggedClockStarted = true;
            Debug.Log("[INTRO] Clock ticking started.", this);
        }
    }

    private void InitializeBookInfluenceCanvas()
    {
        ResolveBookInfluenceCanvas();

        if (_bookInfluenceCanvasGroup == null)
        {
            return;
        }

        _bookInfluenceCanvasGroup.alpha = 0f;
        _bookInfluenceCanvasGroup.interactable = false;
        _bookInfluenceCanvasGroup.blocksRaycasts = false;

        if (_bookInfluenceImage != null)
        {
            _bookInfluenceImage.enabled = true;
        }
    }

    private void ResolveBookInfluenceCanvas()
    {
        if (_bookInfluenceCanvasGroup == null)
        {
            CanvasGroup[] groups = FindObjectsByType<CanvasGroup>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (CanvasGroup group in groups)
            {
                if (group != null && group.gameObject.name == "BookInfluenceCanvas")
                {
                    _bookInfluenceCanvasGroup = group;
                    break;
                }
            }
        }

        if (_bookInfluenceCanvasGroup != null)
        {
            _bookInfluenceImage = _bookInfluenceCanvasGroup.GetComponentInChildren<Image>(true);
        }
    }

    private void BeginPickupFeedback()
    {
        if (!_loggedPlayerInfluenceStarted)
        {
            _loggedPlayerInfluenceStarted = true;
            Debug.Log("[INTRO] Player influence feedback started.", this);
        }

        PlayClipOnSource(_notebookAudioSource, _notebookPickupClip, "notebook pickup clip");
        PlayHapticPulseOnConnectedControllers(0.16f, 0.45f, 0.06f);
        TriggerInfluenceImpulse(0.04f, 0.3f);
    }

    private void StartDelayFeedback()
    {
        StartHeartbeatLoop(0.15f);
        FadeRoomAmbience(0.22f, _floatStartDelay);
    }

    private void StartTransitionAudio()
    {
        if (_transitionAudioStarted)
        {
            return;
        }

        _transitionAudioStarted = true;
        _transitionCueActive = true;
        PlayClipOnSource(_impactAudioSource, _energyBuildClip, "energy build clip");

        if (!_loggedTransitionAudioLead)
        {
            _loggedTransitionAudioLead = true;
            Debug.Log("[INTRO] Hampi transition audio started 1.5 seconds before whiteout.", this);
        }
    }

    private void StartFloatingFeedback()
    {
        if (!_loggedHeartbeatSyncStarted)
        {
            _loggedHeartbeatSyncStarted = true;
            Debug.Log("[INTRO] Notebook heartbeat synchronization started.", this);
        }

        StartNotebookHumLoop(0.12f);
        _bookInfluencePulseActive = true;
        _bookInfluenceBaseAlpha = 0.025f;
        _bookInfluencePulseSpeed = _influencePulseSpeedStart;
        _currentHapticInterval = 1.2f;
        _currentHapticAmplitude = 0.08f;
        StartRepeatingHaptics();
    }

    private void UpdatePlayerFeedbackState(float progress, bool isBlastPhase)
    {
        float clampedProgress = Mathf.Clamp01(progress);
        float maxInfluence = Mathf.Min(0.18f, _maximumInfluenceAlpha);
        _bookInfluencePulseActive = true;
        _bookInfluencePulseSpeed = Mathf.Lerp(_influencePulseSpeedStart, _influencePulseSpeedMaximum, clampedProgress);
        _bookInfluenceBaseAlpha = Mathf.Lerp(0.025f, maxInfluence, clampedProgress);

        if (_playerAudioSource != null && _playerAudioSource.isPlaying)
        {
            float targetHeartbeatVolume = isBlastPhase
                ? Mathf.Lerp(0.45f, 0.62f, clampedProgress)
                : Mathf.Lerp(0.15f, 0.45f, clampedProgress);
            _playerAudioSource.volume = targetHeartbeatVolume;
        }

        if (_notebookAudioSource != null && _notebookAudioSource.isPlaying && _notebookAudioSource.loop)
        {
            float targetHumVolume = isBlastPhase
                ? Mathf.Lerp(0.45f, 0.7f, clampedProgress)
                : Mathf.Lerp(0.12f, 0.45f, clampedProgress);
            _notebookAudioSource.volume = targetHumVolume;
        }

        if (_roomAudioSource != null)
        {
            _roomAudioSource.volume = isBlastPhase
                ? Mathf.Lerp(0.22f, 0.02f, clampedProgress)
                : Mathf.Lerp(0.22f, 0.16f, clampedProgress);
        }

        _currentHapticInterval = isBlastPhase
            ? Mathf.Lerp(0.5f, 0.16f, clampedProgress)
            : Mathf.Lerp(1.2f, 0.5f, clampedProgress);
        _currentHapticAmplitude = isBlastPhase
            ? Mathf.Lerp(0.22f, 0.5f, clampedProgress)
            : Mathf.Lerp(0.08f, 0.22f, clampedProgress);

        if (clampedProgress >= 0.999f && !_loggedInfluenceReachedMaximum)
        {
            _loggedInfluenceReachedMaximum = true;
            Debug.Log("[INTRO] Player influence reached maximum.", this);
        }
    }

    private void BeginWhiteoutFeedback()
    {
        if (!_loggedInfluenceStoppedForWhiteout)
        {
            _loggedInfluenceStoppedForWhiteout = true;
            Debug.Log("[INTRO] Player influence stopped for whiteout.", this);
        }

        _bookInfluencePulseActive = false;
        if (_bookInfluenceCanvasGroup != null)
        {
            _bookInfluenceCanvasGroup.alpha = 0f;
        }

        StopRepeatingHaptics();
        StopAllControllerVibration();
        _transitionCueActive = false;
        _isFloatingSequenceActive = false;
        StopFloatingLampFlicker(true);
        FadeLoopSource(_playerAudioSource, 0.2f, ref _playerLoopFadeCoroutine);
        FadeLoopSource(_notebookAudioSource, 0.2f, ref _notebookLoopFadeCoroutine);
        FadeClockLoopSource(0.15f);
        PlayClipOnSource(_impactAudioSource != null ? _impactAudioSource : _playerAudioSource, _whiteoutRingingClip, "whiteout ringing clip");
    }

    private void PlayBlastPeakFeedback()
    {
        PlayClipOnSource(_impactAudioSource, _finalBlastClip, "final blast clip");
        PlayHapticPulseOnConnectedControllers(0.22f, 0.7f, 0.09f);
    }

    private void StartHeartbeatLoop(float volume)
    {
        if (_playerAudioSource == null)
        {
            return;
        }

        if (_heartbeatLoopClip == null)
        {
            WarnOnce("heartbeatLoopClip", "[INTRO] Heartbeat loop clip is missing.");
            return;
        }

        _playerAudioSource.clip = _heartbeatLoopClip;
        _playerAudioSource.loop = true;
        _playerAudioSource.volume = volume;
        if (!_playerAudioSource.isPlaying)
        {
            _playerAudioSource.Play();
        }
    }

    private void StartNotebookHumLoop(float volume)
    {
        if (_notebookAudioSource == null)
        {
            return;
        }

        if (_notebookHumLoopClip == null)
        {
            WarnOnce("notebookHumLoopClip", "[INTRO] Notebook hum loop clip is missing.");
            return;
        }

        _notebookAudioSource.clip = _notebookHumLoopClip;
        _notebookAudioSource.loop = true;
        _notebookAudioSource.volume = volume;
        if (!_notebookAudioSource.isPlaying)
        {
            _notebookAudioSource.Play();
        }
    }

    private void PlayClipOnSource(AudioSource audioSource, AudioClip clip, string warningKey)
    {
        if (audioSource == null)
        {
            return;
        }

        if (clip == null)
        {
            WarnOnce(warningKey, "[INTRO] " + warningKey + " is missing.");
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    private void FadeRoomAmbience(float targetVolume, float duration)
    {
        if (_roomAmbienceFadeCoroutine != null)
        {
            StopCoroutine(_roomAmbienceFadeCoroutine);
        }

        _roomAmbienceFadeCoroutine = StartCoroutine(FadeAudioSourceVolume(_roomAudioSource, targetVolume, duration, true));
    }

    private void FadeLoopSource(AudioSource audioSource, float duration, ref Coroutine coroutineHandle)
    {
        if (audioSource == null)
        {
            return;
        }

        if (coroutineHandle != null)
        {
            StopCoroutine(coroutineHandle);
        }

        coroutineHandle = StartCoroutine(FadeAudioSourceVolume(audioSource, 0f, duration, false));
    }

    private void FadeClockLoopSource(float duration)
    {
        FadeLoopSource(_clockAudioSource, duration, ref _clockLoopFadeCoroutine);
    }

    private IEnumerator FadeAudioSourceVolume(AudioSource audioSource, float targetVolume, float duration, bool keepPlaying)
    {
        if (audioSource == null)
        {
            yield break;
        }

        float startVolume = audioSource.volume;
        if (duration <= 0f)
        {
            audioSource.volume = targetVolume;
        }
        else
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }
        }

        audioSource.volume = targetVolume;
        if (!keepPlaying && Mathf.Approximately(targetVolume, 0f))
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    private void UpdateNotebookAudioPosition()
    {
        if (_notebookAudioSource == null || _notebookTransform == null)
        {
            return;
        }

        Transform audioTarget = _notebookFloatAnchor != null ? _notebookFloatAnchor : _notebookTransform;
        _notebookAudioSource.transform.position = audioTarget.position;
    }

    private void UpdateClockDuringFinalBlast(float normalizedProgress)
    {
        if (_clockAudioSource == null)
        {
            return;
        }

        float progress = Mathf.Clamp01(normalizedProgress);
        float targetVolume = Mathf.Lerp(_clockBaseVolume, 0.04f, progress);
        _clockAudioSource.volume = targetVolume;
    }

    private void StopFloatingLampFlicker(bool restoreLampIntensity)
    {
        if (_floatingFlickerCoroutine != null)
        {
            StopCoroutine(_floatingFlickerCoroutine);
            _floatingFlickerCoroutine = null;
        }

        if (restoreLampIntensity && _lampLight != null)
        {
            _lampLight.intensity = 1f;
        }
    }

    private void UpdateBookInfluencePulse()
    {
        if (_bookInfluenceCanvasGroup == null)
        {
            return;
        }

        float pulseAlpha = 0f;
        if (_bookInfluencePulseActive)
        {
            float wave = (Mathf.Sin(Time.time * _bookInfluencePulseSpeed) + 1f) * 0.5f;
            pulseAlpha = wave * _bookInfluenceBaseAlpha;
        }

        float finalAlpha = Mathf.Clamp(Mathf.Max(pulseAlpha, _bookInfluenceImpulseAlpha), 0f, 0.18f);
        _bookInfluenceCanvasGroup.alpha = finalAlpha;
    }

    private void TriggerInfluenceImpulse(float alpha, float duration)
    {
        if (_bookInfluenceCanvasGroup == null)
        {
            return;
        }

        if (_bookInfluenceImpulseCoroutine != null)
        {
            StopCoroutine(_bookInfluenceImpulseCoroutine);
        }

        _bookInfluenceImpulseCoroutine = StartCoroutine(InfluenceImpulseRoutine(Mathf.Min(alpha, 0.18f), duration));
    }

    private IEnumerator InfluenceImpulseRoutine(float alpha, float duration)
    {
        _bookInfluenceImpulseAlpha = alpha;
        float elapsed = 0f;
        float safeDuration = Mathf.Max(0.01f, duration);
        while (elapsed < safeDuration)
        {
            elapsed += Time.deltaTime;
            _bookInfluenceImpulseAlpha = Mathf.Lerp(alpha, 0f, Mathf.Clamp01(elapsed / safeDuration));
            yield return null;
        }

        _bookInfluenceImpulseAlpha = 0f;
        _bookInfluenceImpulseCoroutine = null;
    }

    private void StartRepeatingHaptics()
    {
        if (_repeatHapticsCoroutine != null)
        {
            return;
        }

        _repeatHapticsCoroutine = StartCoroutine(RepeatingHapticsRoutine());
    }

    private void StopRepeatingHaptics()
    {
        if (_repeatHapticsCoroutine != null)
        {
            StopCoroutine(_repeatHapticsCoroutine);
            _repeatHapticsCoroutine = null;
        }
    }

    private IEnumerator RepeatingHapticsRoutine()
    {
        while (_bookInfluencePulseActive)
        {
            PlayHapticPulseOnConnectedControllers(0.12f, _currentHapticAmplitude, 0.05f);
            yield return new WaitForSeconds(Mathf.Max(0.08f, _currentHapticInterval));
        }

        _repeatHapticsCoroutine = null;
    }

    private void PlayHapticPulseOnConnectedControllers(float frequency, float amplitude, float duration)
    {
        StartCoroutine(HapticPulseRoutine(OVRInput.Controller.LTouch, frequency, amplitude, duration));
        StartCoroutine(HapticPulseRoutine(OVRInput.Controller.RTouch, frequency, amplitude, duration));
    }

    private IEnumerator HapticPulseRoutine(OVRInput.Controller controller, float frequency, float amplitude, float duration)
    {
        if (!IsControllerConnected(controller))
        {
            yield break;
        }

        OVRInput.SetControllerVibration(frequency, Mathf.Clamp01(amplitude), controller);
        yield return new WaitForSecondsRealtime(Mathf.Max(0.01f, duration));
        OVRInput.SetControllerVibration(0f, 0f, controller);
    }

    private bool IsControllerConnected(OVRInput.Controller controller)
    {
        OVRInput.Controller connectedControllers = OVRInput.GetConnectedControllers();
        return (connectedControllers & controller) != 0;
    }

    private void StopAllControllerVibration()
    {
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.RTouch);
    }

    private void WarnOnce(string key, string message)
    {
        if (_loggedWarnings.Add(key))
        {
            Debug.LogWarning(message, this);
        }
    }

    private bool TryPrepareWhiteFadeCanvas(bool logIssues)
    {
        ResolveWhiteFadeCanvasGroup(logIssues);

        if (_whiteFadeCanvasGroup == null)
        {
            if (logIssues)
            {
                Debug.LogWarning("[INTRO] White fade canvas is missing. Cannot render whiteout.", this);
            }

            return false;
        }

        Canvas whiteFadeCanvas = _whiteFadeCanvasGroup.GetComponent<Canvas>();
        Image whiteFadeImage = _whiteFadeCanvasGroup.GetComponentInChildren<Image>(true);

        _whiteFadeCanvasGroup.gameObject.SetActive(true);
        _whiteFadeCanvasGroup.enabled = true;
        _whiteFadeCanvasGroup.interactable = false;
        _whiteFadeCanvasGroup.blocksRaycasts = false;

        if (whiteFadeCanvas != null)
        {
            whiteFadeCanvas.enabled = true;
        }

        if (whiteFadeImage != null)
        {
            whiteFadeImage.enabled = true;
            whiteFadeImage.color = Color.white;
        }

        return true;
    }

    private void ResolveWhiteFadeCanvasGroup(bool logIssues)
    {
        if (_whiteFadeCanvasGroup != null)
        {
            LogWhiteFadeCanvasReady(_whiteFadeCanvasGroup);
            return;
        }

        CanvasGroup[] canvasGroups = FindObjectsByType<CanvasGroup>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        CanvasGroup firstActiveMatch = null;
        CanvasGroup firstInactiveMatch = null;
        int matchCount = 0;

        foreach (CanvasGroup canvasGroup in canvasGroups)
        {
            if (canvasGroup == null || canvasGroup.gameObject.name != "IntroWhiteFadeCanvas")
            {
                continue;
            }

            matchCount++;
            if (canvasGroup.gameObject.activeInHierarchy && firstActiveMatch == null)
            {
                firstActiveMatch = canvasGroup;
            }

            if (firstInactiveMatch == null)
            {
                firstInactiveMatch = canvasGroup;
            }
        }

        if (matchCount == 0)
        {
            return;
        }

        _whiteFadeCanvasGroup = firstActiveMatch != null ? firstActiveMatch : firstInactiveMatch;

        if (matchCount > 1 && logIssues)
        {
            Debug.LogError("[INTRO] Multiple IntroWhiteFadeCanvas objects found. Using the first active one.", this);
        }

        LogWhiteFadeCanvasReady(_whiteFadeCanvasGroup);
    }

    private void LogWhiteFadeCanvasReady(CanvasGroup canvasGroup)
    {
        if (_loggedWhiteFadeReady || canvasGroup == null)
        {
            return;
        }

        _loggedWhiteFadeReady = true;
        Debug.Log("[INTRO] White fade canvas ready: " + GetHierarchyPath(canvasGroup.transform), this);
    }

    private void LogWhiteFadeValidation()
    {
        if (_whiteFadeCanvasGroup == null)
        {
            return;
        }

        Canvas whiteFadeCanvas = _whiteFadeCanvasGroup.GetComponent<Canvas>();
        Image whiteFadeImage = _whiteFadeCanvasGroup.GetComponentInChildren<Image>(true);
        Camera fadeCamera = whiteFadeCanvas != null ? whiteFadeCanvas.worldCamera : null;
        float imageAlpha = whiteFadeImage != null ? whiteFadeImage.color.a : -1f;

        Debug.Log(
            "[INTRO] White fade validation: " +
            "canvasActive=" + _whiteFadeCanvasGroup.gameObject.activeInHierarchy +
            ",canvasEnabled=" + (whiteFadeCanvas != null && whiteFadeCanvas.enabled) +
            ",canvasGroupAlpha=" + _whiteFadeCanvasGroup.alpha +
            ",imageEnabled=" + (whiteFadeImage != null && whiteFadeImage.enabled) +
            ",imageAlpha=" + imageAlpha +
            ",camera=" + (fadeCamera != null ? fadeCamera.gameObject.name : "null"),
            this);
    }

    private static string GetHierarchyPath(Transform current)
    {
        if (current == null)
        {
            return string.Empty;
        }

        string path = current.name;
        while (current.parent != null)
        {
            current = current.parent;
            path = current.name + "/" + path;
        }

        return path;
    }

#if UNITY_EDITOR
    private void ToggleWhiteFadeOverlayForEditor()
    {
        if (!TryPrepareWhiteFadeCanvas(true))
        {
            return;
        }

        _whiteFadeCanvasGroup.alpha = _whiteFadeCanvasGroup.alpha < 0.5f ? 1f : 0f;
    }
#endif

    private void ApplyNotebookGlowLight(Color lightColor, float intensity, float range)
    {
        if (_notebookGlowLight == null)
        {
            return;
        }

        _notebookGlowLight.enabled = true;
        _notebookGlowLight.color = lightColor;
        _notebookGlowLight.intensity = intensity;
        _notebookGlowLight.range = range;
    }

    private void LogNotebookGlowLightReady()
    {
        if (_loggedNotebookGlowLightReady || _notebookGlowLight == null)
        {
            return;
        }

        _loggedNotebookGlowLightReady = true;
        Debug.Log("[INTRO] Notebook glow light ready: " + _notebookGlowLight.gameObject.name, this);
    }

    private void LogNotebookGlowLightRampStarted()
    {
        if (_loggedNotebookGlowLightRampStarted || _notebookGlowLight == null)
        {
            return;
        }

        _loggedNotebookGlowLightRampStarted = true;
        Debug.Log("[INTRO] Notebook glow light ramp started.", this);
    }

    private void LogNotebookGlowLightReachedMax()
    {
        if (_loggedNotebookGlowLightReachedMax || _notebookGlowLight == null)
        {
            return;
        }

        _loggedNotebookGlowLightReachedMax = true;
        Debug.Log("[INTRO] Notebook glow light reached intensity: " + _notebookGlowLight.intensity, this);
    }

    private Vector3 GetShakeOffset()
    {
        if (_currentShakeAmplitude <= 0f)
        {
            return Vector3.zero;
        }

        float time = Time.time * _currentShakeSpeed;

        return new Vector3(
            (Mathf.PerlinNoise(time, 11.3f) - 0.5f) * 2f,
            (Mathf.PerlinNoise(17.7f, time) - 0.5f) * 2f,
            (Mathf.PerlinNoise(time, 23.9f) - 0.5f) * 2f) * _currentShakeAmplitude;
    }

    private Quaternion GetRotationShake()
    {
        if (_currentRotationShake <= 0f)
        {
            return Quaternion.identity;
        }

        float time = Time.time * (_currentShakeSpeed * 0.8f);
        Vector3 shakeEuler = new Vector3(
            (Mathf.PerlinNoise(time, 31.1f) - 0.5f) * 2f * _currentRotationShake,
            (Mathf.PerlinNoise(37.5f, time) - 0.5f) * 2f * _currentRotationShake,
            (Mathf.PerlinNoise(time, 43.2f) - 0.5f) * 2f * _currentRotationShake);

        return Quaternion.Euler(shakeEuler);
    }

    private void DisableNotebookInteractions()
    {
        if (_notebookTransform == null)
        {
            return;
        }

        GrabInteractable[] grabInteractables = _notebookTransform.GetComponentsInChildren<GrabInteractable>(true);
        foreach (GrabInteractable grabInteractable in grabInteractables)
        {
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
            }
        }

        HandGrabInteractable[] handGrabInteractables = _notebookTransform.GetComponentsInChildren<HandGrabInteractable>(true);
        foreach (HandGrabInteractable handGrabInteractable in handGrabInteractables)
        {
            if (handGrabInteractable != null)
            {
                handGrabInteractable.enabled = false;
            }
        }
    }

}

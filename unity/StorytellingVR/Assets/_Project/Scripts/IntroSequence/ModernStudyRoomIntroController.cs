using System.Collections;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

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
    }

    private void OnEnable()
    {
        ResolveNotebookGlowLight();
        InitializeNotebookGlowLight();

        if (_timeTravelNotebookGrabbable != null)
        {
            _timeTravelNotebookGrabbable.WhenPointerEventRaised += HandleNotebookPointerEvent;
        }
    }

    private void OnDisable()
    {
        if (_timeTravelNotebookGrabbable != null)
        {
            _timeTravelNotebookGrabbable.WhenPointerEventRaised -= HandleNotebookPointerEvent;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (_enableEditorDebugTrigger && !_hasStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartIntroSequence();
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
        Debug.Log("[INTRO] TimeTravelNotebook grabbed. Intro sequence started.", this);
        StartCoroutine(RunIntroSequence());
    }

    private IEnumerator RunIntroSequence()
    {
        StartCoroutine(FlickerLamp());
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
            _lampLight.intensity = Random.Range(_floatingFlickerMinIntensity, _floatingFlickerMaxIntensity);
            float delay = Random.Range(_floatingFlickerMinInterval, _floatingFlickerMaxInterval);
            yield return new WaitForSeconds(delay);
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

        if (_blastBuildDuration <= 0f)
        {
            SetNotebookEffectState(
                _blastFinalLightColor,
                _blastMaximumEmissionIntensity,
                _blastMaximumShakeAmplitude,
                _blastMaximumShakeSpeed,
                _blastMaximumRotationShake,
                _blastMaximumLightIntensity,
                _blastMaximumLightRange);
            Debug.Log("[INTRO] Final notebook energy blast reached peak.", this);
            yield return HoldFinalBlastState();
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < _blastBuildDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedProgress = Mathf.Clamp01(elapsed / _blastBuildDuration);
            float acceleratedProgress = normalizedProgress * normalizedProgress * normalizedProgress;

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
        Debug.Log("[INTRO] Final notebook energy blast reached peak.", this);
        yield return HoldFinalBlastState();
    }

    private IEnumerator HoldFinalBlastState()
    {
        if (_blastHoldDuration > 0f)
        {
            yield return new WaitForSeconds(_blastHoldDuration);
        }

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

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Level1HUDManager : MonoBehaviour
{
    [Header("Subtitle References")]
    public GameObject subtitlePanel;
    public TMP_Text speakerNameText;
    public TMP_Text npcSubtitleText;

    [Header("Subtitle Animation References")]
    [SerializeField] private CanvasGroup dialogueCanvasGroup;
    [SerializeField] private RectTransform dividerLine;
    [SerializeField] private TMP_Text subtitleText;

    [Header("Bargain Input Animation References")]
    [SerializeField] private CanvasGroup inputCanvasGroup;
    [SerializeField] private RectTransform inputBoxTransform;
    [SerializeField] private UnityEngine.UI.Image inputGlowImage;
    [SerializeField] private float inputPanelFadeDuration = 0.2f;

    private Coroutine inputAnimationCoroutine;
    private Coroutine inputIdleCoroutine;
    private Coroutine inputVisibilityCoroutine;

    [Header("Player References")]
    public TMP_InputField playerInput;
    public TMP_Text voiceStatusText;

    [Header("Economy References")]
    public TMP_Text varahaText;
    public Slider reputationSlider;
    public TMP_Text reputationText;

    [Header("Merchant Honour References")]
    [SerializeField] private UnityEngine.UI.Image reputationFillImage;
    [SerializeField] private TMP_Text reputationRankText;
    [SerializeField] private TMP_Text reputationDeltaText;

    private Coroutine reputationAnimCoroutine;

    [Header("Varaha Money References")]
    [SerializeField] private TMP_Text varahaAmountText;
    [SerializeField] private TMP_Text varahaLabelText;
    [SerializeField] private RectTransform moneyPanelTransform;

    private int lastMoneyAmount = -1;
    private Coroutine moneyAnimCoroutine;

    [Header("NPC Intro References")]
    public GameObject npcIntroPanel;
    public TMP_Text introNameText;
    public TMP_Text introOriginText;

    [Header("Transaction References")]
    public GameObject tradeCompletePanel;
    public TMP_Text tradeSummaryText;
    [SerializeField] private TMP_Text tradeTitleText;
    [SerializeField] private TMP_Text tradeRewardText;
    [SerializeField] private TMP_Text tradeItemText;

    [Header("Ledger Animations")]
    [SerializeField] private CanvasGroup ledgerCanvasGroup;
    [SerializeField] private RectTransform ledgerTransform;

    [Header("Ledger Text Components")]
    [SerializeField] private TMP_Text ledgerTitleText;
    [SerializeField] private TMP_Text ledgerResultText;
    [SerializeField] private TMP_Text ledgerDetailsText;
    [SerializeField] private TMP_Text ledgerRewardText;
    [SerializeField] private TMP_Text ledgerFooterText;

    [Header("Current Trade References")]
    public GameObject currentTradePanel;
    public TMP_Text tradeSpiceText;
    public TMP_Text tradeQuantityText;
    public TMP_Text tradeBuyerText;
    [SerializeField] private TMP_Text tradeNPCOfferText;
    [SerializeField] private TMP_Text tradeMarketValueText;
    [Header("Marketplace Timer References")]
    [SerializeField] private TMP_Text nextCustomerCountdownText;

    private Coroutine introFadeCoroutine;
    private Coroutine tradeCompleteCoroutine;
    private Coroutine subtitleAnimCoroutine;
    private bool ledgerOpen = false;

    // Cached original divider width for animation
    private float originalDividerWidth;
    private Vector3 ledgerOriginalScale = Vector3.one;
    private Vector3 ledgerRewardOriginalScale = Vector3.one;
    [SerializeField] private float tradeCompleteIntroScaleMultiplier = 0.85f;
    private bool hasWarnedMissingNextCustomerCountdownText;

    private void Start()
    {
        // Set initial UI panel states
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (npcIntroPanel != null) npcIntroPanel.SetActive(false);
        if (tradeCompletePanel != null) tradeCompletePanel.SetActive(false);
        if (currentTradePanel != null) currentTradePanel.SetActive(false);

        // Cache the original divider width before any animation zeroes it out
        if (dividerLine != null)
        {
            originalDividerWidth = dividerLine.sizeDelta.x;
        }
        if (ledgerTransform != null)
        {
            ledgerOriginalScale = ledgerTransform.localScale;
        }
        if (ledgerRewardText != null)
        {
            ledgerRewardOriginalScale = ledgerRewardText.transform.localScale;
        }

        // Ensure canvas group starts invisible
        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.alpha = 0f;
        }

        ledgerOpen = false;
        SetLedgerWaitingState();
        StartInputIdleAnimation();
        HidePlayerInputPanelImmediate();
        HideNextCustomerCountdown();

        // Initialize Merchant Honour UI
        if (reputationSlider != null)
        {
            int currentRep = (int)reputationSlider.value;
            if (reputationText != null)
            {
                reputationText.text = $"MERCHANT HONOUR\n{currentRep} / 100";
            }
            if (reputationRankText != null)
            {
                reputationRankText.text = GetReputationRank(currentRep);
            }
        }
        else
        {
            if (reputationText != null)
            {
                reputationText.text = "MERCHANT HONOUR\n20 / 100";
            }
        }
        if (reputationDeltaText != null)
        {
            Color c = reputationDeltaText.color;
            c.a = 0f;
            reputationDeltaText.color = c;
        }
    }

    private void Update()
    {
        // Toggle ledger with TAB key (ignored if player is typing in inputField)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (playerInput != null && playerInput.isFocused)
                return;

            ToggleTradeLedger();
        }
    }

    // ─────────────────────────────────────────────
    //  INPUT PANEL ANIMATIONS (RPG GLOW/SCALE)
    // ─────────────────────────────────────────────

    private void StartInputIdleAnimation()
    {
        if (inputIdleCoroutine != null)
        {
            StopCoroutine(inputIdleCoroutine);
        }
        inputIdleCoroutine = StartCoroutine(InputIdleGlowRoutine());
    }

    private IEnumerator InputIdleGlowRoutine()
    {
        while (true)
        {
            if (inputGlowImage != null)
            {
                // Pulse slowly around 2 second cycle
                float alpha = 0.40f + Mathf.Sin(Time.time * Mathf.PI) * 0.15f;
                Color c = inputGlowImage.color;
                c.a = alpha;
                inputGlowImage.color = c;
            }
            yield return null;
        }
    }

    public void StartListeningAnimation()
    {
        if (inputIdleCoroutine != null)
        {
            StopCoroutine(inputIdleCoroutine);
            inputIdleCoroutine = null;
        }
        if (inputAnimationCoroutine != null)
        {
            StopCoroutine(inputAnimationCoroutine);
            inputAnimationCoroutine = null;
        }
        inputAnimationCoroutine = StartCoroutine(StartListeningRoutine());
    }

    private IEnumerator StartListeningRoutine()
    {
        float duration = 0.15f;
        float elapsed = 0f;

        Vector3 startScale = inputBoxTransform != null ? inputBoxTransform.localScale : Vector3.one;
        Vector3 targetScale = Vector3.one * 1.08f;

        float startGlowAlpha = inputGlowImage != null ? inputGlowImage.color.a : 0.4f;
        float targetGlowAlpha = 0.85f;

        float startCanvasAlpha = inputCanvasGroup != null ? inputCanvasGroup.alpha : 1f;
        float targetCanvasAlpha = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (inputBoxTransform != null)
            {
                inputBoxTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            }
            if (inputGlowImage != null)
            {
                Color c = inputGlowImage.color;
                c.a = Mathf.Lerp(startGlowAlpha, targetGlowAlpha, t);
                inputGlowImage.color = c;
            }
            if (inputCanvasGroup != null)
            {
                inputCanvasGroup.alpha = Mathf.Lerp(startCanvasAlpha, targetCanvasAlpha, t);
            }
            yield return null;
        }

        if (inputBoxTransform != null) inputBoxTransform.localScale = targetScale;
        if (inputGlowImage != null)
        {
            Color c = inputGlowImage.color;
            c.a = targetGlowAlpha;
            inputGlowImage.color = c;
        }
        if (inputCanvasGroup != null) inputCanvasGroup.alpha = targetCanvasAlpha;

        inputAnimationCoroutine = null;
    }

    public void StopListeningAnimation()
    {
        if (inputAnimationCoroutine != null)
        {
            StopCoroutine(inputAnimationCoroutine);
            inputAnimationCoroutine = null;
        }
        inputAnimationCoroutine = StartCoroutine(StopListeningRoutine());
    }

    private IEnumerator StopListeningRoutine()
    {
        float duration = 0.2f;
        float elapsed = 0f;

        Vector3 startScale = inputBoxTransform != null ? inputBoxTransform.localScale : Vector3.one;
        Vector3 targetScale = Vector3.one;

        float startGlowAlpha = inputGlowImage != null ? inputGlowImage.color.a : 0.85f;
        float targetGlowAlpha = 0.40f + Mathf.Sin(Time.time * Mathf.PI) * 0.15f; 

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (inputBoxTransform != null)
            {
                inputBoxTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            }
            if (inputGlowImage != null)
            {
                Color c = inputGlowImage.color;
                c.a = Mathf.Lerp(startGlowAlpha, targetGlowAlpha, t);
                inputGlowImage.color = c;
            }
            yield return null;
        }

        if (inputBoxTransform != null) inputBoxTransform.localScale = targetScale;
        
        inputAnimationCoroutine = null;

        // Restart the idle animation
        StartInputIdleAnimation();
    }

    public void DisablePlayerTyping()
    {
        if (playerInput != null)
        {
            playerInput.interactable = false;
        }
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void ShowPlayerInputPanel()
    {
        FadePlayerInputPanel(true);
    }

    public void HidePlayerInputPanel()
    {
        FadePlayerInputPanel(false);
    }

    private void HidePlayerInputPanelImmediate()
    {
        if (inputVisibilityCoroutine != null)
        {
            StopCoroutine(inputVisibilityCoroutine);
            inputVisibilityCoroutine = null;
        }

        if (inputCanvasGroup != null)
        {
            inputCanvasGroup.alpha = 0f;
            inputCanvasGroup.interactable = false;
            inputCanvasGroup.blocksRaycasts = false;
        }
    }

    private void FadePlayerInputPanel(bool visible)
    {
        if (inputVisibilityCoroutine != null)
        {
            StopCoroutine(inputVisibilityCoroutine);
        }

        inputVisibilityCoroutine = StartCoroutine(FadePlayerInputPanelRoutine(visible));
    }

    private IEnumerator FadePlayerInputPanelRoutine(bool visible)
    {
        if (inputCanvasGroup == null)
        {
            yield break;
        }

        float startAlpha = inputCanvasGroup.alpha;
        float targetAlpha = visible ? 1f : 0f;
        float elapsed = 0f;

        inputCanvasGroup.interactable = false;
        inputCanvasGroup.blocksRaycasts = false;

        while (elapsed < inputPanelFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = inputPanelFadeDuration > 0f ? Mathf.Clamp01(elapsed / inputPanelFadeDuration) : 1f;
            inputCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        inputCanvasGroup.alpha = targetAlpha;
        inputCanvasGroup.interactable = visible;
        inputCanvasGroup.blocksRaycasts = visible;
        inputVisibilityCoroutine = null;
    }

    public void EnablePlayerTyping()
    {
        if (playerInput != null)
        {
            playerInput.interactable = true;
        }
    }

    // ─────────────────────────────────────────────
    //  TRADE LEDGER
    // ─────────────────────────────────────────────

    public void ToggleTradeLedger()
    {
        ledgerOpen = !ledgerOpen;
        if (currentTradePanel != null)
        {
            currentTradePanel.SetActive(ledgerOpen);
        }
    }

    private void SetLedgerWaitingState()
    {
        if (tradeBuyerText != null) tradeBuyerText.text = "Waiting for customer...";
        if (tradeQuantityText != null) tradeQuantityText.text = "";
        if (tradeSpiceText != null) tradeSpiceText.text = "";
    }

    // ─────────────────────────────────────────────
    //  SUBTITLE SYSTEM – RPG-STYLE ANIMATED
    // ─────────────────────────────────────────────

    public void ShowSubtitle(string speaker, string text)
    {
        if (subtitlePanel == null) return;

        // Stop any in-progress subtitle animation (show or hide)
        if (subtitleAnimCoroutine != null)
        {
            StopCoroutine(subtitleAnimCoroutine);
            subtitleAnimCoroutine = null;
        }

        // Set text content (speaker stays uppercase per existing convention)
        if (speakerNameText != null) speakerNameText.text = speaker.ToUpper();
        if (npcSubtitleText != null) npcSubtitleText.text = text;

        subtitleAnimCoroutine = StartCoroutine(AnimateSubtitleIn());
    }

    public void HideSubtitle()
    {
        if (subtitlePanel == null) return;

        // Stop any in-progress subtitle animation
        if (subtitleAnimCoroutine != null)
        {
            StopCoroutine(subtitleAnimCoroutine);
            subtitleAnimCoroutine = null;
        }

        subtitleAnimCoroutine = StartCoroutine(AnimateSubtitleOut());
    }

    public void ClearSubtitle()
    {
        // Stop any in-progress subtitle animation
        if (subtitleAnimCoroutine != null)
        {
            StopCoroutine(subtitleAnimCoroutine);
            subtitleAnimCoroutine = null;
        }

        // Immediately reset everything without animation (used for hard session resets)
        if (dialogueCanvasGroup != null) dialogueCanvasGroup.alpha = 0f;

        if (dividerLine != null)
        {
            Vector2 sz = dividerLine.sizeDelta;
            sz.x = 0f;
            dividerLine.sizeDelta = sz;
        }

        SetTextAlpha(speakerNameText, 0f);
        SetTextAlpha(npcSubtitleText, 0f);

        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (speakerNameText != null) speakerNameText.text = "";
        if (npcSubtitleText != null) npcSubtitleText.text = "";
    }

    // ─── SHOW ANIMATION ────────────────────────
    // Phase 1: Fade canvas alpha 0 → 1         (0.25s)
    // Phase 2: Expand divider 0 → original      (0.35s, SmoothStep)
    // Phase 3: Fade speaker + subtitle text in   (0.20s)
    private IEnumerator AnimateSubtitleIn()
    {
        // Activate panel and reset visual state before animating
        subtitlePanel.SetActive(true);

        if (dialogueCanvasGroup != null) dialogueCanvasGroup.alpha = 0f;

        if (dividerLine != null)
        {
            Vector2 sz = dividerLine.sizeDelta;
            sz.x = 0f;
            dividerLine.sizeDelta = sz;
        }

        SetTextAlpha(speakerNameText, 0f);
        SetTextAlpha(npcSubtitleText, 0f);

        // ── Phase 1: Canvas fade-in ──
        if (dialogueCanvasGroup != null)
        {
            float duration = 0.25f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                dialogueCanvasGroup.alpha = t;
                yield return null;
            }
            dialogueCanvasGroup.alpha = 1f;
        }

        // ── Phase 2: Divider line expand (SmoothStep easing) ──
        if (dividerLine != null && originalDividerWidth > 0f)
        {
            float duration = 0.35f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float smoothed = Mathf.SmoothStep(0f, 1f, t);

                Vector2 sz = dividerLine.sizeDelta;
                sz.x = originalDividerWidth * smoothed;
                dividerLine.sizeDelta = sz;
                yield return null;
            }

            Vector2 finalSz = dividerLine.sizeDelta;
            finalSz.x = originalDividerWidth;
            dividerLine.sizeDelta = finalSz;
        }

        // ── Phase 3: Text fade-in ──
        {
            float duration = 0.20f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                SetTextAlpha(speakerNameText, t);
                SetTextAlpha(npcSubtitleText, t);
                yield return null;
            }
            SetTextAlpha(speakerNameText, 1f);
            SetTextAlpha(npcSubtitleText, 1f);
        }

        subtitleAnimCoroutine = null;
    }

    // ─── HIDE ANIMATION ────────────────────────
    // Phase 1: Fade text alpha out               (0.20s)
    // Phase 2: Shrink divider back to 0           (0.30s, SmoothStep)
    // Phase 3: Fade canvas group alpha to 0       (0.20s)
    // Final:   Deactivate subtitle panel
    private IEnumerator AnimateSubtitleOut()
    {
        // ── Phase 1: Text fade-out ──
        {
            float duration = 0.20f;
            float elapsed = 0f;
            float startSpeakerAlpha = GetTextAlpha(speakerNameText);
            float startSubtitleAlpha = GetTextAlpha(npcSubtitleText);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                SetTextAlpha(speakerNameText, Mathf.Lerp(startSpeakerAlpha, 0f, t));
                SetTextAlpha(npcSubtitleText, Mathf.Lerp(startSubtitleAlpha, 0f, t));
                yield return null;
            }
            SetTextAlpha(speakerNameText, 0f);
            SetTextAlpha(npcSubtitleText, 0f);
        }

        // ── Phase 2: Divider shrink (SmoothStep) ──
        if (dividerLine != null)
        {
            float startWidth = dividerLine.sizeDelta.x;
            float duration = 0.30f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float smoothed = Mathf.SmoothStep(0f, 1f, t);

                Vector2 sz = dividerLine.sizeDelta;
                sz.x = Mathf.Lerp(startWidth, 0f, smoothed);
                dividerLine.sizeDelta = sz;
                yield return null;
            }

            Vector2 finalSz = dividerLine.sizeDelta;
            finalSz.x = 0f;
            dividerLine.sizeDelta = finalSz;
        }

        // ── Phase 3: Canvas fade-out ──
        if (dialogueCanvasGroup != null)
        {
            float startAlpha = dialogueCanvasGroup.alpha;
            float duration = 0.20f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                dialogueCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
                yield return null;
            }
            dialogueCanvasGroup.alpha = 0f;
        }

        // ── Final: Deactivate panel ──
        if (subtitlePanel != null)
        {
            subtitlePanel.SetActive(false);
        }

        subtitleAnimCoroutine = null;
    }

    // ─── TEXT ALPHA HELPERS ─────────────────────
    // TMP_Text vertex colors control per-character alpha independently of CanvasGroup

    private void SetTextAlpha(TMP_Text tmpText, float alpha)
    {
        if (tmpText == null) return;
        Color c = tmpText.color;
        c.a = alpha;
        tmpText.color = c;
    }

    private float GetTextAlpha(TMP_Text tmpText)
    {
        if (tmpText == null) return 0f;
        return tmpText.color.a;
    }

    // ─────────────────────────────────────────────
    //  NPC INTRO CARD
    // ─────────────────────────────────────────────

    public void ShowNPCIntro(string name, string origin)
    {
        if (npcIntroPanel == null) return;
        npcIntroPanel.SetActive(true);

        if (introNameText != null) introNameText.text = name.ToUpper();
        if (introOriginText != null) introOriginText.text = origin;

        if (introFadeCoroutine != null)
        {
            StopCoroutine(introFadeCoroutine);
        }
        introFadeCoroutine = StartCoroutine(NPCIntroFadeRoutine(3f));
    }

    private IEnumerator NPCIntroFadeRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (npcIntroPanel != null)
        {
            npcIntroPanel.SetActive(false);
        }
    }

    // ─────────────────────────────────────────────
    //  ECONOMY HUD
    // ─────────────────────────────────────────────

    public void UpdateMoney(int varaha)
    {
        bool isInitial = (lastMoneyAmount == -1);
        
        if (varaha == lastMoneyAmount) return;
        lastMoneyAmount = varaha;

        if (varahaText != null)
        {
            varahaText.text = $"{varaha} Varahas";
        }

        if (varahaAmountText != null)
        {
            varahaAmountText.text = varaha.ToString();
        }

        if (varahaLabelText != null)
        {
            varahaLabelText.text = "VARAHAS";
        }

        if (!isInitial)
        {
            if (moneyAnimCoroutine != null)
            {
                StopCoroutine(moneyAnimCoroutine);
            }
            moneyAnimCoroutine = StartCoroutine(AnimateMoneyChange());
        }
    }

    private IEnumerator AnimateMoneyChange()
    {
        float duration = 0.3f;
        float elapsed = 0f;

        Vector3 originalScale = Vector3.one;
        Color originalColor = Color.white;

        if (varahaAmountText != null)
        {
            originalColor = varahaAmountText.color;
        }

        Color flashColor = new Color(1f, 0.95f, 0.6f, 1f); // Bright golden-white flash

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Sine pulse for scale: 1.0 -> 1.12 -> 1.0
            float scaleMultiplier = 1.0f + Mathf.Sin(t * Mathf.PI) * 0.12f;
            if (moneyPanelTransform != null)
            {
                moneyPanelTransform.localScale = originalScale * scaleMultiplier;
            }

            // Sine pulse for color flash
            if (varahaAmountText != null)
            {
                float colorT = Mathf.Sin(t * Mathf.PI);
                varahaAmountText.color = Color.Lerp(originalColor, flashColor, colorT);
            }

            yield return null;
        }

        if (moneyPanelTransform != null)
        {
            moneyPanelTransform.localScale = originalScale;
        }
        if (varahaAmountText != null)
        {
            varahaAmountText.color = originalColor;
        }

        moneyAnimCoroutine = null;
    }

    public void UpdateRespect(int respect)
    {
        if (reputationText != null)
        {
            reputationText.text = $"MERCHANT HONOUR\n{respect} / 100";
        }

        if (reputationRankText != null)
        {
            reputationRankText.text = GetReputationRank(respect);
        }

        if (reputationSlider != null)
        {
            if (reputationAnimCoroutine != null)
            {
                StopCoroutine(reputationAnimCoroutine);
            }
            reputationAnimCoroutine = StartCoroutine(AnimateReputation(respect));
        }
    }

    private string GetReputationRank(float value)
    {
        if (value <= 20f) return "Unknown Trader";
        if (value <= 40f) return "Small Merchant";
        if (value <= 60f) return "Trusted Merchant";
        if (value <= 80f) return "Royal Supplier";
        return "Legendary Merchant";
    }

    private IEnumerator AnimateReputation(float targetValue)
    {
        if (reputationSlider == null) yield break;

        float startValue = reputationSlider.value;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            reputationSlider.value = Mathf.Lerp(startValue, targetValue, t);

            if (reputationRankText != null)
            {
                reputationRankText.text = GetReputationRank(reputationSlider.value);
            }

            if (reputationText != null)
            {
                reputationText.text = $"MERCHANT HONOUR\n{(int)reputationSlider.value} / 100";
            }

            yield return null;
        }

        reputationSlider.value = targetValue;
        if (reputationRankText != null)
        {
            reputationRankText.text = GetReputationRank(targetValue);
        }
        if (reputationText != null)
        {
            reputationText.text = $"MERCHANT HONOUR\n{(int)targetValue} / 100";
        }

        reputationAnimCoroutine = null;
    }

    public void ShowReputationChange(int delta)
    {
        if (reputationDeltaText == null) return;
        
        if (reputationDeltaAnimCoroutine != null)
        {
            StopCoroutine(reputationDeltaAnimCoroutine);
        }
        
        reputationDeltaAnimCoroutine = StartCoroutine(AnimateReputationDelta(delta));
    }

    private Coroutine reputationDeltaAnimCoroutine;

    private IEnumerator AnimateReputationDelta(int delta)
    {
        if (reputationDeltaText == null) yield break;

        reputationDeltaText.text = delta > 0 ? $"+{delta} Honour" : $"{delta} Honour";
        Color startColor = delta > 0 ? new Color(1f, 0.85f, 0.1f, 1f) : new Color(0.9f, 0.1f, 0.1f, 1f); 
        reputationDeltaText.color = startColor;

        Vector3 startLocalPosition = reputationDeltaText.transform.localPosition;
        Vector3 targetLocalPosition = startLocalPosition + new Vector3(0f, delta > 0 ? 50f : -50f, 0f);

        if (delta > 0)
        {
            StartCoroutine(PulseGlowRoutine());
        }
        else
        {
            StartCoroutine(PulseWarningRoutine());
        }

        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            float alpha = 1f;
            if (t > 0.5f)
            {
                alpha = Mathf.Lerp(1f, 0f, (t - 0.5f) / 0.5f);
            }
            
            Color c = reputationDeltaText.color;
            c.a = alpha;
            reputationDeltaText.color = c;

            reputationDeltaText.transform.localPosition = Vector3.Lerp(startLocalPosition, targetLocalPosition, t);

            yield return null;
        }

        reputationDeltaText.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        reputationDeltaText.transform.localPosition = startLocalPosition;
        reputationDeltaAnimCoroutine = null;
    }

    private IEnumerator PulseWarningRoutine()
    {
        if (reputationFillImage == null) yield break;

        Color originalColor = reputationFillImage.color;
        Color pulseColor = new Color(0.9f, 0.1f, 0.1f, 1f); 
        
        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float pulseIntensity = Mathf.Sin(t * Mathf.PI);

            reputationFillImage.color = Color.Lerp(originalColor, pulseColor, pulseIntensity);
            yield return null;
        }

        reputationFillImage.color = originalColor;
    }

    private IEnumerator PulseGlowRoutine()
    {
        if (reputationFillImage == null) yield break;

        Color originalColor = reputationFillImage.color;
        Color pulseColor = new Color(1f, 0.85f, 0.4f, 1f); 
        
        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float pulseIntensity = Mathf.Sin(t * Mathf.PI);

            reputationFillImage.color = Color.Lerp(originalColor, pulseColor, pulseIntensity);
            yield return null;
        }

        reputationFillImage.color = originalColor;
    }

    // ─────────────────────────────────────────────
    //  TRADE COMPLETE POPUP
    // ─────────────────────────────────────────────

    public void ShowTradeComplete(TransactionSummary transaction, bool isSuccess, int reputationDelta)
    {
        if (tradeCompletePanel == null) return;
        tradeCompletePanel.SetActive(true);

        if (tradeTitleText != null)
        {
            tradeTitleText.text = isSuccess ? "TRADE COMPLETE" : "NO DEAL";
        }
        if (tradeRewardText != null)
        {
            tradeRewardText.text = isSuccess ? $"+{transaction.earned} Varahas" : "Customer Left";
        }
        if (tradeItemText != null)
        {
            tradeItemText.text = isSuccess ? $"{transaction.item} Sold" : "No goods exchanged";
        }

        if (tradeSummaryText != null)
        {
            if (isSuccess)
            {
                tradeSummaryText.text = $"TRADE COMPLETE\n\n+{transaction.earned} Varahas\n\n{transaction.item} Sold";
            }
            else
            {
                tradeSummaryText.text = "NO DEAL\n\nCustomer Left\n\nNo goods exchanged";
            }
        }

        if (tradeCompleteCoroutine != null)
        {
            StopCoroutine(tradeCompleteCoroutine);
        }
        tradeCompleteCoroutine = StartCoroutine(ShowLedgerRoutine(transaction, isSuccess, reputationDelta));
    }

    private IEnumerator ShowLedgerRoutine(TransactionSummary transaction, bool isSuccess, int reputationDelta)
    {
        string spice = transaction != null ? transaction.item : "Unknown Spice";
        string quantity = transaction != null ? transaction.quantity : "0";
        string buyer = transaction != null ? (!string.IsNullOrEmpty(transaction.buyer_name) ? transaction.buyer_name : "Customer") : "Customer";
        int earned = transaction != null ? transaction.earned : 0;
        Vector3 baseLedgerScale = ledgerTransform != null ? ledgerTransform.localScale : ledgerOriginalScale;
        if (baseLedgerScale == Vector3.zero)
        {
            baseLedgerScale = ledgerOriginalScale == Vector3.zero ? Vector3.one : ledgerOriginalScale;
        }

        Vector3 introLedgerScale = baseLedgerScale * tradeCompleteIntroScaleMultiplier;

        if (ledgerTitleText != null) ledgerTitleText.text = "HAMPI MARKET LEDGER";
        if (ledgerResultText != null) ledgerResultText.text = isSuccess ? "TRADE COMPLETE" : "NO DEAL";
        
        if (ledgerDetailsText != null)
        {
            if (isSuccess)
            {
                ledgerDetailsText.text = $"Spice: {spice}\nQuantity: {quantity}\nBuyer: {buyer}";
            }
            else
            {
                ledgerDetailsText.text = $"Buyer: {buyer}\n\nNo goods exchanged";
            }
        }

        if (ledgerRewardText != null)
        {
            if (isSuccess)
            {
                string repSign = reputationDelta >= 0 ? "+" : "";
                ledgerRewardText.text = $"+{earned} Varahas\n{repSign}{reputationDelta} Honour";
            }
            else
            {
                string repSign = reputationDelta > 0 ? "+" : "";
                ledgerRewardText.text = $"{repSign}{reputationDelta} Honour";
            }
        }

        if (ledgerFooterText != null)
        {
            ledgerFooterText.text = isSuccess ? "Your name spreads through the market" : "The customer leaves your stall";
        }

        // Set initial scale and alpha
        if (ledgerCanvasGroup != null) ledgerCanvasGroup.alpha = 0f;
        if (ledgerTransform != null) ledgerTransform.localScale = introLedgerScale;
        if (ledgerRewardText != null) ledgerRewardText.transform.localScale = ledgerRewardOriginalScale;

        // 1. Play Show Animation
        float elapsed = 0f;
        float duration = 0.35f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = Mathf.SmoothStep(0f, 1f, t);

            if (ledgerCanvasGroup != null) ledgerCanvasGroup.alpha = eased;
            if (ledgerTransform != null) ledgerTransform.localScale = Vector3.Lerp(introLedgerScale, baseLedgerScale, eased);
            yield return null;
        }

        if (ledgerCanvasGroup != null) ledgerCanvasGroup.alpha = 1f;
        if (ledgerTransform != null) ledgerTransform.localScale = baseLedgerScale;

        // 2. Play Reward Text Animation (for successful trades)
        if (isSuccess && ledgerRewardText != null)
        {
            float rewardElapsed = 0f;
            float rewardDuration = 0.4f;
            Transform rewardTransform = ledgerRewardText.transform;
            while (rewardElapsed < rewardDuration)
            {
                rewardElapsed += Time.deltaTime;
                float t = rewardElapsed / rewardDuration;
                float scaleVal = 1.0f;
                if (t < 0.5f)
                {
                    scaleVal = Mathf.Lerp(0.8f, 1.15f, t * 2f);
                }
                else
                {
                    scaleVal = Mathf.Lerp(1.15f, 1.0f, (t - 0.5f) * 2f);
                }
                rewardTransform.localScale = ledgerRewardOriginalScale * scaleVal;
                yield return null;
            }
            rewardTransform.localScale = ledgerRewardOriginalScale;
        }

        // 3. Wait for display duration (5 seconds total display time minus animation times)
        float waitTime = isSuccess ? 4.25f : 4.65f; 
        yield return new WaitForSeconds(waitTime);

        // 4. Play Hide Animation
        elapsed = 0f;
        duration = 0.25f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (ledgerCanvasGroup != null) ledgerCanvasGroup.alpha = 1f - t;
            if (ledgerTransform != null) ledgerTransform.localScale = Vector3.Lerp(baseLedgerScale, introLedgerScale, t);
            yield return null;
        }

        if (ledgerCanvasGroup != null) ledgerCanvasGroup.alpha = 0f;
        if (ledgerTransform != null) ledgerTransform.localScale = baseLedgerScale;
        if (tradeCompletePanel != null) tradeCompletePanel.SetActive(false);
    }

    // ─────────────────────────────────────────────
    //  ACTIVE TRADE LEDGER DETAILS
    // ─────────────────────────────────────────────

    public void ShowCurrentTrade(string spice, string quantity, string buyerName)
    {
        if (currentTradePanel == null) return;

        if (string.IsNullOrEmpty(buyerName))
        {
            SetLedgerWaitingState();
        }
        else
        {
            if (tradeBuyerText != null) tradeBuyerText.text = $"Customer:\n{buyerName}";
            if (tradeQuantityText != null) tradeQuantityText.text = $"Seeking:\n{quantity}";
            if (tradeSpiceText != null) tradeSpiceText.text = spice;
        }
    }

    public void UpdateCurrentTrade(CurrentTrade trade)
    {
        if (trade == null) return;

        if (tradeSpiceText != null)
        {
            tradeSpiceText.text = trade.spice;
        }

        if (tradeNPCOfferText != null)
        {
            tradeNPCOfferText.text = $"NPC Offer:\n{trade.npc_offer} Varahas";
        }

        if (tradeMarketValueText != null)
        {
            tradeMarketValueText.text = $"Market Value:\n{trade.market_value} Varahas";
        }
    }

    public void HideCurrentTrade()
    {
        if (currentTradePanel != null)
        {
            currentTradePanel.SetActive(false);
        }
        ledgerOpen = false;
        SetLedgerWaitingState();
    }

    public void SetVoiceStatus(string status)
    {
        if (voiceStatusText != null)
        {
            voiceStatusText.text = status;
        }
    }

    public void ShowNextCustomerCountdown(int secondsRemaining)
    {
        if (nextCustomerCountdownText == null)
        {
            WarnMissingHudTextOnce(ref hasWarnedMissingNextCustomerCountdownText, "NextCustomerCountdownText");
            return;
        }

        nextCustomerCountdownText.gameObject.SetActive(true);
        nextCustomerCountdownText.text = $"Next customer arriving in {Mathf.Max(0, secondsRemaining)}s...";
    }

    public void HideNextCustomerCountdown()
    {
        if (nextCustomerCountdownText == null)
        {
            return;
        }

        nextCustomerCountdownText.gameObject.SetActive(false);
    }

    public bool HasNextCustomerCountdownText => nextCustomerCountdownText != null;

    private void WarnMissingHudTextOnce(ref bool hasWarned, string objectName)
    {
        if (hasWarned)
        {
            return;
        }

        hasWarned = true;
        Debug.LogWarning($"[Level1HUDManager] Missing HUD reference for {objectName}. Create and assign the TMP text to display marketplace timers.");
    }
}

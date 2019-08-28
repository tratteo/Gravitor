using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Manager of gameplay HUD
/// </summary>
public class HUDManager : MonoBehaviour
{

    public enum ToastType { GAME_TOAST, ACHIEVEMENT_TOAST }

    private static HUDManager instance = null;

    public static HUDManager GetInstance() { return instance; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //USING CUSTOM EDITOR
    #region Variables
    //Editor variables
    public bool showHUDField;
    public Text scoreText;
    public Text timeRelativeText = null;
    public Text timeProperText = null;
    public Text fpsText = null;
    public Text gameOverScoreText = null;
    public Text gameOverGravityPointsText = null;
    public Text gameOverInfoText = null;
    public Text healthText = null;
    public Text loadingAdText = null;
    public Image healthBar = null;
    public GameObject shieldBtn = null;
    public Image shieldChargeIcon = null;
    public Text shieldsText = null;
    public GameObject antigravityBtn = null;
    public GameObject quantumTunnelBtn = null;
    public GameObject solarflareBtn = null;
    public GameObject gammaRayBurstBtn = null;
    public GameObject adButton = null;

    public bool showPanelsField;
    public GameObject controlPanel;
    public GameObject HUDPanel;
    public GameObject timerPanel = null;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject tutorialPanel;
    public GameObject highScorePanel = null;
    public GameObject quantumTunnelPanel = null;
    public GameObject highGravityFieldPanel = null;

    public bool showToastsField;
    public ToastScript inGameToast = null;
    public ToastScript achievementToast = null;

    //References
    public GameMode gameMode;
    private SkillManager skillManager;
    private PlayerManager playerManager;
    private Tutorial tutorial;

    //Local variables
    private PlayerData playerData;
    private SettingsData settingsData;
    private IEnumerator shieldHUDAnim_C = null;
    private bool showFPS = false;
    private readonly int resumeTimerDuration = 3;
    private int resumeTimer;
    private Sound levelSound;
    private IEnumerator checkAdC = null;
    private Text enqueuedShieldsText = null;
    private bool isAdLoading = false;

    //Events

    #endregion


    void OnDisable()
    {
        playerManager.UnsubscribeToHealthChanged(UpdatePlayerHealth);
        gameMode.UnsubrscribeToHighScoreReachedEvent(ShowHighScorePanel);
        gameMode.UnsubscribeToOnAttemptEvent(Attempt);
    }

    private void Start()
    {
        tutorial = GetComponent<Tutorial>();
        gameMode = FindObjectOfType<GameMode>();
        if (gameMode)
        {
            skillManager = gameMode.playerManager.skillManager;
            playerManager = gameMode.playerManager;
            playerManager.SubscribeToHealthChanged(UpdatePlayerHealth);
            //Events subscriptions
            gameMode.SubrscribeToHighScoreReachedEvent(ShowHighScorePanel);
            gameMode.SubscribeToOnAttemptEvent(Attempt);
        }

        playerData = SaveManager.GetInstance().LoadPersistentData(SaveManager.PLAYER_DATA).GetData<PlayerData>();
        settingsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SETTINGS_PATH).GetData<SettingsData>();
        showFPS = settingsData.showFPS;
        SetControls(settingsData.controlsLayout);
        EnableHUDBasedOnPlayerState();


        //Start tutorial
        if (SharedUtilities.GetInstance().IsFirstLaunch())
        {
            tutorial.enabled = true;
            scoreText.gameObject.SetActive(false);
            timeRelativeText.gameObject.SetActive(false);
            healthText.gameObject.SetActive(false);
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f;
            gameMode.isPaused = true;
        }
        if (showFPS)
        {
            StartCoroutine(FPSCoroutine());
        }

        float delay = 0.1f;
        int quality = QualitySettings.GetQualityLevel();
        switch (quality)
        {
            case SettingsManager.LOW:
                delay = 0.75f;
                break;
            case SettingsManager.MEDIUM:
                delay = 0.55f;
                break;
            case SettingsManager.HIGH:
                delay = 0.25f;
                break;
            case SettingsManager.ULTRA:
                delay = 0.1f;
                break;
        }
        StartCoroutine(UpdateStats_C(delay));

        levelSound = AudioManager.GetInstance().PlaySound(AudioManager.LEVEL_SONG);

        enqueuedShieldsText = shieldChargeIcon.GetComponentInChildren<Text>();

        checkAdC = CheckAd_C();
        StartCoroutine(checkAdC);
    }

    private void Update()
    {
        if (gameMode.playerManager.resilience > 0)
        {
            scoreText.text = gameMode.sessionScore.ToString("0");
        }
        int enqShields = playerManager.extraManager.enqueuedShields;
        
        if(enqShields > 0)
        {
            enqueuedShieldsText.text = enqShields.ToString();
        }
        else
        {
            enqueuedShieldsText.text = "";
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (gameMode != null && !gameMode.isGameOver && !gameMode.isPaused)
        {
            Pause(true);
        }
    }


    private void EnableHUDBasedOnPlayerState()
    {
        switch (playerData.playerState)
        {
            case PlayerManager.PlayerState.ASTEROID:
                gammaRayBurstBtn.gameObject.SetActive(false);

                antigravityBtn.gameObject.SetActive(true);
                quantumTunnelBtn.gameObject.SetActive(false);
                solarflareBtn.gameObject.SetActive(false);
                break;
            case PlayerManager.PlayerState.COMET:
                gammaRayBurstBtn.gameObject.SetActive(false);

                antigravityBtn.gameObject.SetActive(true);
                quantumTunnelBtn.gameObject.SetActive(true);
                solarflareBtn.gameObject.SetActive(false);
                break;
            case PlayerManager.PlayerState.DENSE_PLANET:
                gammaRayBurstBtn.gameObject.SetActive(false);

                antigravityBtn.gameObject.SetActive(true);
                quantumTunnelBtn.gameObject.SetActive(true);
                solarflareBtn.gameObject.SetActive(true);
                break;
            case PlayerManager.PlayerState.STAR:
                gammaRayBurstBtn.gameObject.SetActive(true);

                antigravityBtn.gameObject.SetActive(true);
                quantumTunnelBtn.gameObject.SetActive(true);
                solarflareBtn.gameObject.SetActive(true);
                break;
        }
    }

    private void SetControls(SettingsData.ControlsLayout layout)
    {
        RectTransform rectTransform;
        Vector2 zero = Vector2.zero;
        switch (layout)
        {
            case SettingsData.ControlsLayout.JOYSTICK_RIGHT:
                rectTransform = gameMode.playerManager.movementManager.joystick.gameObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.offsetMin = zero;
                rectTransform.offsetMax = zero;
                rectTransform.sizeDelta = zero;

                rectTransform = antigravityBtn.GetComponent<RectTransform>();
                rectTransform.anchorMin = zero;
                rectTransform.anchorMax = zero;
                rectTransform.anchoredPosition = new Vector2(150f, 180f);

                rectTransform = quantumTunnelBtn.GetComponent<RectTransform>();
                rectTransform.anchorMin = zero;
                rectTransform.anchorMax = zero;
                rectTransform.anchoredPosition = new Vector2(360f, 180f);

                rectTransform = solarflareBtn.GetComponent<RectTransform>();
                rectTransform.anchorMin = zero;
                rectTransform.anchorMax = zero;
                rectTransform.anchoredPosition = new Vector2(570f, 180f);

                rectTransform = gammaRayBurstBtn.GetComponent<RectTransform>();
                rectTransform.anchorMin = zero;
                rectTransform.anchorMax = zero;
                rectTransform.anchoredPosition = new Vector2(150f, 425f);

                rectTransform = shieldBtn.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0f, 0.5f); ;
                rectTransform.anchorMax = new Vector2(0f, 0.5f); ;
                rectTransform.anchoredPosition = new Vector2(150f, 150f);
                break;

            case SettingsData.ControlsLayout.JOYSTICK_LEFT:
                rectTransform = gameMode.playerManager.movementManager.joystick.gameObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                rectTransform.offsetMin = zero;
                rectTransform.offsetMax = zero;
                rectTransform.sizeDelta = zero;

                rectTransform = antigravityBtn.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(1f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 0f);
                rectTransform.anchoredPosition = new Vector2(-570f, 180f);

                rectTransform = quantumTunnelBtn.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(1f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 0f);
                rectTransform.anchoredPosition = new Vector2(-360f, 180f);

                rectTransform = solarflareBtn.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(1f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 0f);
                rectTransform.anchoredPosition = new Vector2(-150f, 180f);

                rectTransform = gammaRayBurstBtn.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(1f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 0f);
                rectTransform.anchoredPosition = new Vector2(-150f, 425f);

                rectTransform = shieldBtn.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(1f, 0.5f); ;
                rectTransform.anchorMax = new Vector2(1f, 0.5f); ;
                rectTransform.anchoredPosition = new Vector2(-150f, 150);
                break;
        }
    }

    public void Pause(bool state)
    {
        if (state)
        {
            pausePanel.SetActive(true);
            QuantumTunnelSelectionActive(false);
            gameMode.isPaused = true;
            HUDPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
            Time.timeScale = 0f;
        }
        else
        {
            StartCoroutine(PauseResumeCountdown());
        }
    }

    private IEnumerator PauseResumeCountdown()
    {
        pausePanel.SetActive(false);
        resumeTimer = resumeTimerDuration;
        Text timerText = timerPanel.GetComponentInChildren<Text>();
        timerText.text = resumeTimer.ToString();
        timerPanel.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.25f);
        while (resumeTimer > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            resumeTimer--;
            timerText.text = resumeTimer.ToString();
        }
        Time.timeScale = 1;
        gameMode.isPaused = false;
        timerPanel.gameObject.SetActive(false);
        HUDPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }


    public void Attempt()
    {
        if (checkAdC != null)
        {
            StopCoroutine(checkAdC);
        }

        Executer.GetInstance().AddJob(() =>
        {
            timeRelativeText.gameObject.SetActive(true);
            timeProperText.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(true);
            gameOverPanel.SetActive(false);
            QuantumTunnelSelectionActive(false);
            gameMode.isPaused = true;
            HUDPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
            Time.timeScale = 0f;
            StartCoroutine(PauseResumeCountdown());
        });
    }

    private IEnumerator CheckAd_C()
    {
        while (true)
        {
            if (!gameMode.attemptUsed)
            {
                if (GoogleAdsManager.GetInstance().IsRewardedAdLoaded(GoogleAdsManager.RewardedAdType.EXTRA_ATTEMPT))
                {
                    adButton.SetActive(true);
                    loadingAdText.gameObject.SetActive(false);
                }
                else
                {
                    adButton.SetActive(false);
                    loadingAdText.gameObject.SetActive(true);
                    if (!isAdLoading)
                    {
                        GoogleAdsManager.GetInstance().LoadAd(GoogleAdsManager.RewardedAdType.EXTRA_ATTEMPT);
                        isAdLoading = true;
                    }
                }
            }
            else
            {
                adButton.SetActive(false);
                loadingAdText.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(4f);
        }
    }

    //SKILLS//
    public void AntiGravity()
    {
        skillManager.AntiGravity();
    }

    public void QuantumTunnel()
    {
        if (skillManager.canCastSkill && !skillManager.isQuantumTunnelSelectionActive)
        {
            QuantumTunnelSelectionActive(true);
            quantumTunnelBtn.GetComponentsInChildren<Image>()[1].fillAmount = 1f;
        }
    }

    public void QuantumTunnelSelectionActive(bool state)
    {
        if (skillManager)
        {
            skillManager.isQuantumTunnelSelectionActive = state;
        }

        quantumTunnelPanel.SetActive(state);
    }

    public void SolarFlare()
    {
        skillManager.SolarFlare();
    }

    public void GammaRayBurst()
    {
        skillManager.GammaRayBurst();
    }


    public void CollectShield(int number)
    {
        if (shieldsText != null)
        {
            shieldsText.text = number.ToString();
        }
        if (shieldBtn != null)
        {
            Image image;
            image = shieldBtn.GetComponent<Image>();
            image.color = new Color32(0, 210, 40, 150);
            shieldBtn.GetComponent<EventTrigger>().enabled = true;
        }
    }
    public void UseShield()
    {
        int res = playerManager.extraManager.Shield();
        if (res != -1)
        {
            shieldsText.text = res.ToString();
            if (res <= 0)
            {
                Image image = shieldBtn.GetComponent<Image>();
                image.color = new Color32(200, 200, 200, 100);
                shieldBtn.GetComponent<EventTrigger>().enabled = false;
            }
        }
    }
    public void ShieldUsed()
    {
        if (shieldHUDAnim_C != null)
        {
            StopCoroutine(shieldHUDAnim_C);
        }

        shieldHUDAnim_C = SharedUtilities.GetInstance().UnfillImage(this, shieldChargeIcon, playerManager.extraManager.GetCurrentShield().duration);

    }
    public void ShieldDestroyed()
    {
        StopCoroutine(shieldHUDAnim_C);
        shieldChargeIcon.fillAmount = 0f;
    }


    //

    //SHOW PANELS
    public void ShowHighGravityPanel(bool state)
    {
        highGravityFieldPanel.SetActive(state);
        if (!state)
        {
            gameMode.playerManager.dangerZoneCount = 0;
        }
    }

    public void ShowHighScorePanel()
    {
        highScorePanel.SetActive(true);
    }

    public void DisplayGameOver()
    {
        float score = gameMode.sessionScore;
        float gravityPoints = gameMode.sessionGravityPoints;
        QuantumTunnelSelectionActive(false);
        highScorePanel.SetActive(false);
        gameOverInfoText.text = "Difference between distorted time and normal time flow: " + SharedUtilities.GetInstance().GetTimeStringFromSeconds(playerManager.relativeExTime - playerManager.properTime);
        scoreText.gameObject.SetActive(false);
        timeRelativeText.gameObject.SetActive(false);
        timeProperText.gameObject.SetActive(false);
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = score.ToString("0");
        gameOverGravityPointsText.text = gravityPoints.ToString();

        if (GoogleAdsManager.GetInstance().IsRewardedAdLoaded(GoogleAdsManager.RewardedAdType.EXTRA_ATTEMPT) && !gameMode.attemptUsed)
        {
            adButton.SetActive(true);
        }
        else
        {
            adButton.SetActive(false);
        }
    }
    //

    public void UnloadLevelAndLoadMenu()
    {
        AudioManager instance = AudioManager.GetInstance();
        if (instance != null)
        {
            instance.SmoothOutSound(levelSound, 0.05f, 1f);
        }
        GetComponent<SceneLoader>().LoadScene("Menu");
    }

    private IEnumerator FPSCoroutine()
    {
        while (showFPS)
        {
            fpsText.text = ("FPS: " + (1f / Time.unscaledDeltaTime).ToString("0.0"));
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    public void ShowRewardedAd()
    {
        GoogleAdsManager.GetInstance().ShowRewardedAd(GoogleAdsManager.RewardedAdType.EXTRA_ATTEMPT);
        isAdLoading = false;
    }

    private void UpdatePlayerHealth(float health, float initialHealth)
    {
        Executer.GetInstance().AddJob(() =>
        {
            if (healthBar != null)
            {
                healthBar.transform.localScale = new Vector2(health / initialHealth, 1);
                healthText.text = health.ToString("0.0");
            }
        });
    }


    public void Toast(ToastType type, string message, Sprite sprite, float duration, float delay, bool enqueue)
    {
        ToastScript toast = null;
        switch (type)
        {
            case ToastType.GAME_TOAST:
                toast = inGameToast;
                break;
            case ToastType.ACHIEVEMENT_TOAST:
                toast = achievementToast;
                break;
        }
        StartCoroutine(ToastCoroutine(toast, message, sprite, duration, delay, enqueue));
    }
    private IEnumerator ToastCoroutine(ToastScript toast, string message, Sprite sprite, float duration, float delay, bool enqueue)
    {
        yield return new WaitForSeconds(delay);
        if (enqueue)
        {
            toast.EnqueueToast(message, sprite, duration);
        }
        else
        {
            toast.ShowToast(message, sprite, duration);
        }
    }


    public void CoolDownSkill(SkillManager.Skill skill, float duration)
    {
        //StartCoroutine(CoolDown(skill, duration));
        EventTrigger eventTrigger = null;
        Image cooldownOverlay = null;
        switch (skill)
        {
            case SkillManager.Skill.ANTI_GRAVITY:
                eventTrigger = antigravityBtn.GetComponent<EventTrigger>();
                cooldownOverlay = SharedUtilities.GetInstance().GetFirstChildrenWithComponent<Image>(antigravityBtn).GetComponent<Image>();
                break;

            case SkillManager.Skill.QUANTUM_TUNNEL:
                eventTrigger = quantumTunnelBtn.GetComponent<EventTrigger>();
                cooldownOverlay = SharedUtilities.GetInstance().GetFirstChildrenWithComponent<Image>(quantumTunnelBtn).GetComponent<Image>();
                break;

            case SkillManager.Skill.SOLAR_FLARE:
                eventTrigger = solarflareBtn.GetComponent<EventTrigger>();
                cooldownOverlay = SharedUtilities.GetInstance().GetFirstChildrenWithComponent<Image>(solarflareBtn).GetComponent<Image>();
                break;

            case SkillManager.Skill.GAMMARAY_BURST:
                eventTrigger = gammaRayBurstBtn.GetComponent<EventTrigger>();
                cooldownOverlay = SharedUtilities.GetInstance().GetFirstChildrenWithComponent<Image>(gammaRayBurstBtn).GetComponent<Image>();
                break;
        }
        eventTrigger.enabled = false;
        SharedUtilities.GetInstance().UnfillImage<EventTrigger>(this, cooldownOverlay, duration, EnableEventTrigger, eventTrigger);
    }
    private void EnableEventTrigger(EventTrigger eventTrigger)
    {
        eventTrigger.enabled = true;
    }


    private IEnumerator UpdateStats_C(float delay)
    {
        while (true)
        {
            timeRelativeText.text = "Tr = " + playerManager.relativeExTime.ToString("0.0");
            timeProperText.text = "Tp = " + playerManager.properTime.ToString("0.0");
            yield return new WaitForSeconds(delay);
        }
    }
}

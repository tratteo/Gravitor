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
    public Text objectiveText = null;
    public Text fpsText = null;
    public Text gameOverScoreText = null;
    public Text gameOverGravityPointsText = null;
    public Text gravitonsText = null;
    public Text gameOverInfoText = null;
    public Text healthText = null;
    public Text loadingAdText = null;
    public Text levelText = null;
    public Image healthBar = null;
    public GameObject shieldBtn = null;
    public Image gameOverGrade = null;
    public Text gameOverGradeGP = null;
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
    public GameObject levelCompletedPanel;
    public GameObject levelObjectivePanel;
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
    private readonly int resumeTimerDuration = 3;
    private int resumeTimer;
    private IEnumerator checkAdC = null;
    private Text enqueuedShieldsText = null;

    private Level level;

    //Events

    #endregion
    private void OnEnable()
    {
        gameMode = FindObjectOfType<GameMode>();
        level = gameMode.currentLevel;
    }

    private void Start()
    {
        tutorial = GetComponent<Tutorial>();
        
        if (gameMode)
        {
            skillManager = gameMode.playerManager.skillManager;
            playerManager = gameMode.playerManager;
        }

        playerData = SaveManager.GetInstance().LoadPersistentData(SaveManager.PLAYER_DATA).GetData<PlayerData>();

        settingsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SETTINGS_PATH).GetData<SettingsData>();
        if (settingsData.showFPS)
        {
            StartCoroutine(FPSCoroutine());
        }

        SetControls(settingsData.controlsLayout);

        //Start tutorial
        if (playerData.showTutorial)
        {

            EnableHUDPanel(false);
            Time.timeScale = 0f;
            gameMode.isPaused = true;

            tutorialPanel.SetActive(true);
            tutorial.enabled = true;
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

        AudioManager.GetInstance().currentMusic = AudioManager.GetInstance().PlaySound(AudioManager.LEVEL_SONG);

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
        timerPanel.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.2f);
        resumeTimer = resumeTimerDuration;
        Text timerText = timerPanel.GetComponentInChildren<Text>();
        timerText.text = resumeTimer.ToString();
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
        timerText.text = "";
    }


    public void Attempt()
    {
        if (checkAdC != null)
        {
            Executer.GetInstance().AddJob(() => { StopCoroutine(checkAdC); });
        }

        Executer.GetInstance().AddJob(() =>
        {
            adButton.SetActive(false);
            loadingAdText.gameObject.SetActive(false);
            levelText.transform.parent.gameObject.SetActive(false);

            EnableHUDPanel(true);

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
                    if (gameMode.isGameOver)
                    {
                        adButton.SetActive(true);
                        loadingAdText.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (gameMode.isGameOver)
                    {
                        adButton.SetActive(false);
                        loadingAdText.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                adButton.SetActive(false);
                loadingAdText.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(3f);
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

        shieldHUDAnim_C = SharedUtilities.GetInstance().UnfillImage(this, shieldChargeIcon, playerManager.extraManager.shieldDuration);

    }
    public void ShieldDestroyed()
    {
        StopCoroutine(shieldHUDAnim_C);
        shieldChargeIcon.fillAmount = 0f;
    }


    //SHOW PANELS
    public void EnableHighGravityFieldPanel(bool state)
    {
        highGravityFieldPanel.SetActive(state);
        if (!state)
        {
            gameMode.playerManager.dangerZoneCount = 0;
        }
    }

    public void DisplayHighscorePanel()
    {
        highScorePanel.SetActive(true);
    }

    public void DisplayGameOverPanel(bool showGradeButton, bool displayGravitonsEarned)
    {
        QuantumTunnelSelectionActive(false);
        highScorePanel.SetActive(false);
        gameOverInfoText.text = "Difference between distorted time and normal time flow: " + SharedUtilities.GetInstance().GetTimeStringFromSeconds(playerManager.relativeExTime - playerManager.properTime);

        EnableHUDPanel(false);

        gameOverPanel.SetActive(true);
        gameOverScoreText.text = gameMode.sessionScore.ToString("0");
        gameOverGravityPointsText.text = gameMode.sessionGravityPoints.ToString();

        int targetGP = 0;
        gameOverGrade.gameObject.SetActive(false);
        gameOverGradeGP.gameObject.SetActive(false);
        if (showGradeButton)
        {
            if (gameMode.sessionScore >= level.goldScore)
            {
                gameOverGrade.gameObject.SetActive(true);
                gameOverGradeGP.gameObject.SetActive(true);
                gameOverGrade.color = Level.GOLD_COLOR;
                targetGP = level.goldGP;
                //gameOverGradeGP.text = level.goldGP.ToString("0");
            }
            else if (gameMode.sessionScore >= level.silverScore)
            {
                gameOverGrade.gameObject.SetActive(true);
                gameOverGradeGP.gameObject.SetActive(true);
                gameOverGrade.color = Level.SILVER_COLOR;
                targetGP = level.silverGP;
                //gameOverGradeGP.text = level.silverGP.ToString("0");
            }
            else if (gameMode.sessionScore >= level.bronzeScore)
            {
                gameOverGrade.gameObject.SetActive(true);
                gameOverGradeGP.gameObject.SetActive(true);
                gameOverGrade.color = Level.BRONZE_COLOR;
                targetGP = level.bronzeGP;
                //gameOverGradeGP.text = level.bronzeGP.ToString("0");
            }
            StartCoroutine(GradeGPAnim_C(targetGP));
        }
        if(displayGravitonsEarned)
        {
            gravitonsText.text = "+"+ gameMode.sessionGravitons.ToString();
            gravitonsText.gameObject.SetActive(true);
        }
        else
        {
            gravitonsText.gameObject.SetActive(false);
        }
    }

    private IEnumerator GradeGPAnim_C(int targetPoints)
    {
        Animator animator = gameOverPanel.GetComponent<Animator>();
        float length = animator.runtimeAnimatorController.animationClips[0].length;
        yield return new WaitForSeconds(length);
        int current = 0;
        int stride = targetPoints / 65;
        while(current + stride <= targetPoints)
        {
            gameOverGradeGP.text = current.ToString();
            current += stride;
            yield return new WaitForFixedUpdate();
        }
        gameOverGradeGP.text = targetPoints.ToString();
    }

    public void DisplayLevelCompletedPanel()
    {
        StartCoroutine(DisplayLevelCompleted_C());
    }

    public void DisplayLevelObjectivePanel()
    {
        levelObjectivePanel.SetActive(true);
        levelObjectivePanel.GetComponentInChildren<Text>().text = level.levelObjective;
    }

    public void EnableHUDPanel(bool state)
    {
        HUDPanel.SetActive(state);
    }

    private IEnumerator DisplayLevelCompleted_C()
    {
        levelCompletedPanel.SetActive(true);
        loadingAdText.gameObject.SetActive(false);
        Animator animator = levelCompletedPanel.GetComponent<Animator>();
        float length = animator.runtimeAnimatorController.animationClips[0].length;
        yield return new WaitForSeconds(length);
        DisplayGameOverPanel(true, false);
    }
    
    public void DisplayPlayerLevelUp(int level)
    {
        levelText.transform.parent.gameObject.SetActive(true);
        levelText.text = level.ToString("0");
    }

    public void UnloadLevelAndLoadScene(string scene)
    {
        AudioManager instance = AudioManager.GetInstance();
        if (instance != null)
        {
            instance.SmoothOutSound(AudioManager.GetInstance().currentMusic, 0.05f, 1f);
        }
        GetComponent<SceneLoader>().LoadScene(scene);
    }

    private IEnumerator FPSCoroutine()
    {
        while (true)
        {
            fpsText.text = ("FPS: " + (1f / Time.unscaledDeltaTime).ToString("0.0"));
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    public void ShowRewardedAd()
    {
        GoogleAdsManager.GetInstance().ShowRewardedAd(GoogleAdsManager.RewardedAdType.EXTRA_ATTEMPT);
    }

    public void UpdatePlayerHealthUI(float health, float initialHealth)
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
        Image cooldownOverlay = null, skillImage = null;
        switch (skill)
        {
            case SkillManager.Skill.ANTI_GRAVITY:
                skillImage = antigravityBtn.GetComponent<Image>();
                cooldownOverlay = SharedUtilities.GetInstance().GetFirstChildrenWithComponent<Image>(antigravityBtn).GetComponent<Image>();
                break;

            case SkillManager.Skill.QUANTUM_TUNNEL:
                skillImage = quantumTunnelBtn.GetComponent<Image>();
                cooldownOverlay = SharedUtilities.GetInstance().GetFirstChildrenWithComponent<Image>(quantumTunnelBtn).GetComponent<Image>();
                break;

            case SkillManager.Skill.SOLAR_FLARE:
                skillImage = solarflareBtn.GetComponent<Image>();
                cooldownOverlay = SharedUtilities.GetInstance().GetFirstChildrenWithComponent<Image>(solarflareBtn).GetComponent<Image>();
                break;

            case SkillManager.Skill.GAMMARAY_BURST:
                skillImage = gammaRayBurstBtn.GetComponent<Image>();
                cooldownOverlay = SharedUtilities.GetInstance().GetFirstChildrenWithComponent<Image>(gammaRayBurstBtn).GetComponent<Image>();
                break;
        }
        skillImage.raycastTarget = false;
        SharedUtilities.GetInstance().UnfillImage<Image>(this, cooldownOverlay, duration, EnableRayCastTarget, skillImage);
    }
    private void EnableRayCastTarget(Image skillImage)
    {
        skillImage.raycastTarget = true;
    }


    private IEnumerator UpdateStats_C(float delay)
    {
        switch (level.category)
        {
            case Level.LevelCategory.TIME:
                while (true)
                {
                    timeRelativeText.text = "Tr = " + playerManager.relativeExTime.ToString("0.0");
                    timeProperText.text = "Tp = " + playerManager.properTime.ToString("0.0");
                    objectiveText.text = playerManager.properTime.ToString("0.0") + " s";
                    yield return new WaitForSeconds(delay);
                }

            case Level.LevelCategory.TIME_DILATED:
                while (true)
                {
                    timeRelativeText.text = "Tr = " + playerManager.relativeExTime.ToString("0.0");
                    timeProperText.text = "Tp = " + playerManager.properTime.ToString("0.0");
                    objectiveText.text = (playerManager.relativeExTime - playerManager.properTime).ToString("0.0") + " s";
                    yield return new WaitForSeconds(delay);
                }

            case Level.LevelCategory.DISTANCE:      
                while (true)
                {
                    timeRelativeText.text = "Tr = " + playerManager.relativeExTime.ToString("0.0");
                    timeProperText.text = "Tp = " + playerManager.properTime.ToString("0.0");
                    objectiveText.text = playerManager.movementManager.distance.ToString("0") + " Km";
                    yield return new WaitForSeconds(delay);
                }

            case Level.LevelCategory.MAX_SPEED:
                while (true)
                {
                    timeRelativeText.text = "Tr = " + playerManager.relativeExTime.ToString("0.0");
                    timeProperText.text = "Tp = " + playerManager.properTime.ToString("0.0");
                    objectiveText.text = playerManager.movementManager.relativeSpeed.ToString("0.000") + " c";
                    yield return new WaitForSeconds(delay);
                }

            case Level.LevelCategory.OBSTACLES_DESTROY:
                while (true)
                {
                    timeRelativeText.text = "Tr = " + playerManager.relativeExTime.ToString("0.0");
                    timeProperText.text = "Tp = " + playerManager.properTime.ToString("0.0");
                    objectiveText.text = playerManager.skillManager.sessionObstaclesDestroyed.ToString("0") + " destroyed";
                    yield return new WaitForSeconds(delay);
                }

            case Level.LevelCategory.ENDLESS:
                while (true)
                {
                    timeRelativeText.text = "Tr = " + playerManager.relativeExTime.ToString("0.0");
                    timeProperText.text = "Tp = " + playerManager.properTime.ToString("0.0");
                    switch (settingsData.hudConf)
                    {
                        case SettingsData.EndlessModeHUD.DISTANCE:
                            objectiveText.text = playerManager.movementManager.distance.ToString("0") + " Km";
                            break;
                        case SettingsData.EndlessModeHUD.OBSTACLES_DESTROYED:
                            objectiveText.text = playerManager.skillManager.sessionObstaclesDestroyed.ToString("0") + " destroyed";
                            break;
                        case SettingsData.EndlessModeHUD.SPEED:
                            objectiveText.text = playerManager.movementManager.relativeSpeed.ToString("0.000") + " c";
                            break;
                        case SettingsData.EndlessModeHUD.TIME:
                            objectiveText.text = playerManager.properTime.ToString("0.0") + " s";
                            break;
                        case SettingsData.EndlessModeHUD.TIME_DILATED:
                            objectiveText.text = (playerManager.relativeExTime - playerManager.properTime).ToString("0.0") + " s";
                            break;
                        default:
                            objectiveText.text = playerManager.movementManager.distance.ToString("0") + " Km";
                            break;
                    }
                    yield return new WaitForSeconds(delay);
                }
            
            default:
                while (true)
                {
                    timeRelativeText.text = "Tr = " + playerManager.relativeExTime.ToString("0.0");
                    timeProperText.text = "Tp = " + playerManager.properTime.ToString("0.0");
                    objectiveText.text = playerManager.movementManager.distance.ToString("0") + " Km";
                    yield return new WaitForSeconds(delay);
                }
        }
    }
}

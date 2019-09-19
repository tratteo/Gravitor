using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UpgradeMenu manager
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    public const int INIT_HEALTH_COST = 10000;
    public const int INIT_THRUST_COST = 2500;
    public const int INIT_ANTIGRAVITY_COST = 8000;
    public const int INIT_QUANTUMTUNNEL_COST = 6500;
    public const int INIT_SOLARFLARE_COST = 6250;

    public const int EVOLVE_COST = 90000;

    #region Variables
    [Header("Preview")]
    [SerializeField] private GameObject previewStateSphere = null;
    [SerializeField] private Material asteroid = null;
    [SerializeField] private Material comet = null;
    [SerializeField] private ToastScript toast = null;
    [SerializeField] private Text gravityPointsText = null;
    [SerializeField] private Text unlockSkillText = null;
    [SerializeField] private Text unlockGammaRayBurstText = null;
    [Header("Evolve")]
    [SerializeField] private GameObject evolveBtn = null;
    [SerializeField] private Text stateText = null;
    [SerializeField] private Text evolveCostText = null;
    [Header("Health")]
    [SerializeField] private Text healthPointsText = null;
    [SerializeField] private Text healthCostText = null;
    [SerializeField] private Text resilienceInfoText = null;
    [Header("ThrustForce")]
    [SerializeField] private Text thrustPointsText = null;
    [SerializeField] private Text thrustCostText = null;
    [SerializeField] private Text thrustForceText = null;
    [Header("AntiGravity")]
    [SerializeField] private GameObject upgradeAntigravityBtn = null;
    [SerializeField] private Text antigravityPointsText = null;
    [SerializeField] private Text antigravityInfoText = null;
    [SerializeField] private Text antigravityCostText = null;
    [Header("QuantumTunnel")]
    [SerializeField] private GameObject upgradeQuantumTunnelBtn = null;
    [SerializeField] private Text quantumTunnelPointsText = null;
    [SerializeField] private Text quantumTunnelInfoText = null;
    [SerializeField] private Text quantumTunnelCostText = null;
    [Header("SolarFlare")]
    [SerializeField] private GameObject upgradeSolarflareBtn = null;
    [SerializeField] private Text solarflarePointsText = null;
    [SerializeField] private Text solarflareInfoText = null;
    [SerializeField] private Text solarflareCostText = null;
    [Header("GammaRayBurstUI")]
    [SerializeField] private GameObject GRBEffectParent = null;
    [SerializeField] private GameObject GRB1Effect = null;
    [SerializeField] private GameObject GRB2Effect = null;
    [SerializeField] private GameObject GRB3Effect = null;
    [SerializeField] private GameObject GRB4Effect = null;
    [SerializeField] private Text GRBCostText = null;
    [SerializeField] private Text GRBInfoText = null;
    private GameObject currentGRBEffect = null;


    private int resilienceUpgradeCost;
    private int thrustForceUpgradeCost;
    private int antigravityUpgradeCost;
    private int quantumTunnelUpgradeCost;
    private int solarflareUpgradeCost;
    private int GRBUpgradeCost;

    private PlayerData playerData;
    private PlayerSkillsData skillsData;
    private CurrencyData currencyData;
    #endregion

    private void Start()
    {
        playerData = SaveManager.GetInstance().LoadPersistentData(SaveManager.PLAYER_DATA).GetData<PlayerData>();

        thrustForceUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(playerData.thrustForcePoints, INIT_THRUST_COST, GameplayMath.DEFAULT_RATIO);
        resilienceUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(playerData.GetHealthPoints(), INIT_HEALTH_COST, GameplayMath.RESILIENCE_RATIO);

        SaveObject objectData;
        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.CURRENCY_PATH);
        currencyData = objectData.GetData<CurrencyData>();
        skillsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH).GetData<PlayerSkillsData>();

        antigravityUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(skillsData.antigravityPoints, INIT_ANTIGRAVITY_COST, GameplayMath.DEFAULT_RATIO);
        quantumTunnelUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(skillsData.quantumTunnelPoints, INIT_QUANTUMTUNNEL_COST, GameplayMath.DEFAULT_RATIO);
        solarflareUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(skillsData.solarflarePoints, INIT_SOLARFLARE_COST, GameplayMath.DEFAULT_RATIO);
        GRBUpgradeCost = GameplayMath.GetInstance().GetGRBCost(skillsData.gammaRayBurstPoints);

        UpdateUI();
        EnableUIBasedOnPlayerState();
    }

    private void FixedUpdate()
    {
        previewStateSphere.transform.Rotate(new Vector3(0f, 1f, 0f) * 0.35f);
    }

    private void EnableUIBasedOnPlayerState()
    {
        switch (playerData.playerState)
        {
            case PlayerManager.PlayerState.ASTEROID:
                previewStateSphere.GetComponent<MeshRenderer>().material = asteroid;
                upgradeAntigravityBtn.gameObject.SetActive(true);
                antigravityPointsText.gameObject.SetActive(true);
                antigravityInfoText.gameObject.SetActive(true);

                upgradeQuantumTunnelBtn.gameObject.SetActive(true);
                quantumTunnelPointsText.gameObject.SetActive(true);
                quantumTunnelInfoText.gameObject.SetActive(true);

                upgradeSolarflareBtn.gameObject.SetActive(false);
                solarflarePointsText.gameObject.SetActive(false);
                solarflareInfoText.gameObject.SetActive(false);

                evolveBtn.gameObject.SetActive(true);
                evolveCostText.gameObject.SetActive(true);
                unlockSkillText.gameObject.SetActive(true);

                GRBEffectParent.SetActive(false);
                unlockGammaRayBurstText.gameObject.SetActive(true);
                currentGRBEffect?.SetActive(false);
                break;

            case PlayerManager.PlayerState.COMET:
                previewStateSphere.GetComponent<MeshRenderer>().material = comet;
                upgradeAntigravityBtn.gameObject.SetActive(true);
                antigravityPointsText.gameObject.SetActive(true);
                antigravityInfoText.gameObject.SetActive(true);

                upgradeQuantumTunnelBtn.gameObject.SetActive(true);
                quantumTunnelPointsText.gameObject.SetActive(true);
                quantumTunnelInfoText.gameObject.SetActive(true);

                upgradeSolarflareBtn.gameObject.SetActive(true);
                solarflarePointsText.gameObject.SetActive(true);
                solarflareInfoText.gameObject.SetActive(true);

                evolveBtn.gameObject.SetActive(false);
                evolveCostText.gameObject.SetActive(false);
                unlockSkillText.gameObject.SetActive(false);

                GRBEffectParent.SetActive(true);
                unlockGammaRayBurstText.gameObject.SetActive(false);
                currentGRBEffect?.SetActive(true);
                break;
        }
    }

    private void UpdateUI()
    {
        stateText.text = playerData.playerState.ToString();

        if (currencyData.gravityPoints != -1)
        {
            gravityPointsText.text = currencyData.gravityPoints.ToString();
        }

        if (playerData.thrustForcePoints < PlayerData.THRUSTFORCE_MAX_POINTS)
        {
            thrustCostText.text = thrustForceUpgradeCost.ToString();
        }
        else
        {
            thrustCostText.gameObject.SetActive(false);
        }

        if (playerData.GetHealthPoints() < PlayerData.RESILIENCE_MAX_POINTS)
        {
            healthCostText.text = resilienceUpgradeCost.ToString();
        }
        else
        {
            healthCostText.gameObject.SetActive(false);
        }

        //ANTIGRAVITY
        if (skillsData.antigravityPoints < PlayerSkillsData.ANTIGRAVITY_MAX_POINTS)
        {
            antigravityCostText.text = antigravityUpgradeCost.ToString();
        }
        else
        {
            antigravityCostText.gameObject.SetActive(false);
        }
        //QUANTUM TUNNEL
        if (skillsData.quantumTunnelPoints < PlayerSkillsData.QUANTUMTUNNEL_MAX_POINTS)
        {
            quantumTunnelCostText.text = quantumTunnelUpgradeCost.ToString();
        }
        else
        {
            quantumTunnelCostText.gameObject.SetActive(false);
        }
        //SOLAR FLARE
        if (skillsData.solarflarePoints < PlayerSkillsData.SOLARFLARE_MAX_POINTS)
        {
            solarflareCostText.text = solarflareUpgradeCost.ToString();
        }
        else
        {
            solarflareCostText.gameObject.SetActive(false);
        }
        //GRB
        if(skillsData.gammaRayBurstPoints < PlayerSkillsData.GRB_MAX_POINTS - 1)
        {
            GRBCostText.text = GRBUpgradeCost.ToString();
        }
        else if(skillsData.gammaRayBurstPoints == PlayerSkillsData.GRB_MAX_POINTS - 1)
        {
            GRBCostText.text = "Visit the store to unlock <color=cyan><b>GRB level 4</b></color>";
        }
        else
        {
            GRBCostText.gameObject.SetActive(false);
        }
        UpdateGRBEffect();
        evolveCostText.text = EVOLVE_COST.ToString();

        if (playerData != null)
        {
            thrustPointsText.text = playerData.thrustForcePoints.ToString();
            healthPointsText.text = playerData.GetHealthPoints().ToString();
            thrustForceText.text = "Thrust Force: " + GameplayMath.GetInstance().GetPlayerThrustForceFromPoints(playerData.thrustForcePoints).ToString("0.00");
            resilienceInfoText.text = "Resilience: " + playerData.resilience.ToString();
        }

        if (skillsData != null)
        {
            antigravityPointsText.text = skillsData.antigravityPoints.ToString();
            quantumTunnelPointsText.text = skillsData.quantumTunnelPoints.ToString();
            solarflarePointsText.text = skillsData.solarflarePoints.ToString();

            if (skillsData.antigravityPoints < PlayerSkillsData.ANTIGRAVITY_MAX_POINTS)
            {
                antigravityInfoText.text = "Duration: <b>" + GameplayMath.GetInstance().GetAntigravityDuration(skillsData.antigravityPoints).ToString("0.0") + "s</b>"
                                           + ", cooldown: <b>" + GameplayMath.GetInstance().GetAntigravityCooldown(skillsData.antigravityPoints).ToString("0.0") + "s</b>"
                                           + "\n<color=cyan>Next Upgrade: </color>\n"
                                           + "Duration: " + GameplayMath.GetInstance().GetAntigravityDuration(skillsData.antigravityPoints + 1).ToString("0.0") + "s"
                                           + ", cooldown: " + GameplayMath.GetInstance().GetAntigravityCooldown(skillsData.antigravityPoints + 1).ToString("0.0") + "s";
            }
            else
            {
                antigravityInfoText.text = "Duration: <b>" + GameplayMath.GetInstance().GetAntigravityDuration(skillsData.antigravityPoints).ToString("0.0") + "s</b>"
                                           + ", cooldown: <b>" + GameplayMath.GetInstance().GetAntigravityCooldown(skillsData.antigravityPoints).ToString("0.0") + "s</b>";
            }

            if (skillsData.quantumTunnelPoints < PlayerSkillsData.QUANTUMTUNNEL_MAX_POINTS)
            {
                quantumTunnelInfoText.text = "Cooldown: <b>" + GameplayMath.GetInstance().GetQuantumTunnelCooldown(skillsData.quantumTunnelPoints).ToString("0.0") + "s</b>"
                                          + "\n<color=cyan>Next Upgrade: </color>\n"
                                          + "Cooldown: " + GameplayMath.GetInstance().GetQuantumTunnelCooldown(skillsData.quantumTunnelPoints + 1).ToString("0.0") + "s";
            }
            else
            {
                quantumTunnelInfoText.text = "Cooldown: <b>" + GameplayMath.GetInstance().GetQuantumTunnelCooldown(skillsData.quantumTunnelPoints).ToString("0.0") + "s</b>";
            }

            if (skillsData.solarflarePoints < PlayerSkillsData.SOLARFLARE_MAX_POINTS)
            {
                solarflareInfoText.text = "Cooldown: <b>" + GameplayMath.GetInstance().GetSolarflareCooldown(skillsData.solarflarePoints).ToString("0.0") + "s</b>"
                                          + ", radius: <b>" + GameplayMath.GetInstance().GetSolarflareRadius(skillsData.solarflarePoints).ToString("0.0") + "</b>"
                                          + "\n<color=cyan>Next Upgrade: </color>\n"
                                          + "Cooldown: " + GameplayMath.GetInstance().GetSolarflareCooldown(skillsData.solarflarePoints + 1).ToString("0.0") + "s"
                                          + ", radius: " + GameplayMath.GetInstance().GetSolarflareRadius(skillsData.solarflarePoints + 1).ToString("0.0");
            }
            else
            {
                solarflareInfoText.text = "Cooldown: <b>" + GameplayMath.GetInstance().GetSolarflareCooldown(skillsData.solarflarePoints).ToString("0.0") + "s</b>"
                                          + ", radius: <b>" + GameplayMath.GetInstance().GetSolarflareRadius(skillsData.solarflarePoints).ToString("0.0") + "</b>";
            }

            if(skillsData.gammaRayBurstPoints < PlayerSkillsData.GRB_MAX_POINTS)
            {
                GRBInfoText.text = "Cooldown: <b>" + GameplayMath.GetInstance().GetGRBCooldown(skillsData.gammaRayBurstPoints).ToString("0.0") + "s</b>"
                                   + ", ray radius: <b>" + GameplayMath.GetInstance().GetGRBUnscaledRadius(skillsData.gammaRayBurstPoints).ToString() + "</b>"
                                   + "\n<color=cyan>Next Upgrade: </color>\n"
                                   + "Cooldown: <b>" + GameplayMath.GetInstance().GetGRBCooldown(skillsData.gammaRayBurstPoints + 1).ToString("0.0") + "s</b>"
                                   + ", ray radius: <b>" + GameplayMath.GetInstance().GetGRBUnscaledRadius(skillsData.gammaRayBurstPoints + 1).ToString() + "</b>";
            }
            else
            {
                GRBInfoText.text = "Cooldown: <b>" + GameplayMath.GetInstance().GetGRBCooldown(skillsData.gammaRayBurstPoints).ToString("0.0") + "s</b>"
                                    + ", ray radius: <b>" + GameplayMath.GetInstance().GetGRBUnscaledRadius(skillsData.gammaRayBurstPoints).ToString() + "</b>";
            }
        }
    }


    //Skills
    public void UpgradeAntiGravity()
    {
        if (skillsData.antigravityPoints >= PlayerSkillsData.ANTIGRAVITY_MAX_POINTS)
        {
            toast.ShowToast("Already at max level", null, 2f);
            return;
        }

        if (antigravityUpgradeCost <= currencyData.gravityPoints)
        {
            if (skillsData != null)
            {
                skillsData.antigravityPoints++;
                currencyData.gravityPoints -= antigravityUpgradeCost;
                antigravityUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(skillsData.antigravityPoints, INIT_ANTIGRAVITY_COST, GameplayMath.DEFAULT_RATIO);
            }
            SaveManager.GetInstance().SavePersistentData(skillsData, SaveManager.SKILLSDATA_PATH);
            SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
            UpdateUI();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }
    }

    public void UpgradeQuantumTunnel()
    {
        if (skillsData.quantumTunnelPoints >= PlayerSkillsData.QUANTUMTUNNEL_MAX_POINTS)
        {
            toast.ShowToast("Already at max level", null, 2f);
            return;
        }

        if (quantumTunnelUpgradeCost <= currencyData.gravityPoints)
        {
            if (skillsData != null)
            {
                skillsData.quantumTunnelPoints++;
                currencyData.gravityPoints -= quantumTunnelUpgradeCost;
                quantumTunnelUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(skillsData.quantumTunnelPoints, INIT_QUANTUMTUNNEL_COST, GameplayMath.DEFAULT_RATIO);
            }
            SaveManager.GetInstance().SavePersistentData(skillsData, SaveManager.SKILLSDATA_PATH);
            SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
            UpdateUI();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }
    }

    public void UpgradeSolarFlare()
    {
        if (skillsData.solarflarePoints >= PlayerSkillsData.SOLARFLARE_MAX_POINTS)
        {
            toast.ShowToast("Already at max level", null, 2f);
            return;
        }

        if (solarflareUpgradeCost <= currencyData.gravityPoints)
        {
            if (skillsData != null)
            {
                skillsData.solarflarePoints++;
                currencyData.gravityPoints -= solarflareUpgradeCost;
                solarflareUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(skillsData.solarflarePoints, INIT_SOLARFLARE_COST, GameplayMath.DEFAULT_RATIO);
            }
            SaveManager.GetInstance().SavePersistentData(skillsData, SaveManager.SKILLSDATA_PATH);
            SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
            UpdateUI();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }
    }


    public void UpgradeThrustForce()
    {
        if (playerData.thrustForcePoints >= PlayerData.THRUSTFORCE_MAX_POINTS)
        {
            toast.ShowToast("Already at max level", null, 1.5f);
            return;
        }
        if (thrustForceUpgradeCost <= currencyData.gravityPoints)
        {
            if (playerData != null)
            {
                playerData.thrustForcePoints++;
                currencyData.gravityPoints -= thrustForceUpgradeCost;
                thrustForceUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(playerData.thrustForcePoints, INIT_THRUST_COST, GameplayMath.DEFAULT_RATIO);
            }
            SaveManager.GetInstance().SavePersistentData(playerData, SaveManager.PLAYER_DATA);
            SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
            UpdateUI();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }
    }

    public void UpgradeResilience()
    {
        if (playerData.GetHealthPoints() >= PlayerData.RESILIENCE_MAX_POINTS)
        {
            toast.ShowToast("Already at max level", null, 1.5f);
            return;
        }
        if (resilienceUpgradeCost <= currencyData.gravityPoints)
        {
            if (playerData != null)
            {
                playerData.IncreaseHealth();
                currencyData.gravityPoints -= resilienceUpgradeCost;
                resilienceUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(playerData.GetHealthPoints(), INIT_HEALTH_COST, GameplayMath.RESILIENCE_RATIO);
            }
            SaveManager.GetInstance().SavePersistentData(playerData, SaveManager.PLAYER_DATA);
            SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
            UpdateUI();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }

    }

    public void UpgradeGRB()
    {
        if (skillsData.gammaRayBurstPoints == PlayerSkillsData.GRB_MAX_POINTS - 1)
        {
            toast.ShowToast("GRB level 4 can be unlocked only in the store", null, 2f);
            return;
        }
        else if(skillsData.gammaRayBurstPoints >= PlayerSkillsData.GRB_MAX_POINTS)
        {
            toast.ShowToast("GRB already at max level", null, 1.5f);
            return;
        }
        if (GRBUpgradeCost <= currencyData.gravityPoints)
        {
            if (skillsData != null)
            {
                skillsData.gammaRayBurstPoints++;
                currencyData.gravityPoints -= GRBUpgradeCost;
                GRBUpgradeCost = GameplayMath.GetInstance().GetGRBCost(skillsData.gammaRayBurstPoints);
            }
            SaveManager.GetInstance().SavePersistentData(skillsData, SaveManager.SKILLSDATA_PATH);
            SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
            UpdateUI();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }

    }

    private void UpdateGRBEffect()
    {
        GameObject currentEff = GRBEffectParent.GetComponentInChildren<ParticleSystem>()?.gameObject;
        if (currentEff != null) Destroy(currentEff);

        switch (skillsData.gammaRayBurstPoints)
        {
            case 1:
                currentGRBEffect = Instantiate(GRB1Effect);
                break;
            case 2:
                currentGRBEffect = Instantiate(GRB2Effect);
                break;
            case 3:
                currentGRBEffect = Instantiate(GRB3Effect);
                break;
            case 4:
                currentGRBEffect = Instantiate(GRB4Effect);
                break;
        }
        currentGRBEffect.transform.SetParent(GRBEffectParent.transform);
        currentGRBEffect.transform.localScale = new Vector3(1, 1, 1);
        currentGRBEffect.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void Evolve()
    {
        if (currencyData.gravityPoints >= EVOLVE_COST && playerData.playerState == PlayerManager.PlayerState.ASTEROID)
        {
            currencyData.gravityPoints -= EVOLVE_COST;
            playerData.playerState = PlayerManager.PlayerState.COMET;
            SaveManager.GetInstance().SavePersistentData(playerData, SaveManager.PLAYER_DATA);
            SaveManager.GetInstance().SavePersistentData(currencyData, SaveManager.CURRENCY_PATH);
            UpdateUI();
            EnableUIBasedOnPlayerState();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }
    }

}

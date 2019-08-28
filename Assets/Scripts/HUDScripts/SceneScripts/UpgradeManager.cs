using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UpgradeMenu manager
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    public const int INIT_HEALTH_COST = 8000;
    public const int INIT_THRUST_COST = 3500;
    public const int INIT_ANTIGRAVITY_COST = 7500;
    public const int INIT_QUANTUMTUNNEL_COST = 5500;
    public const int INIT_SOLARFLARE_COST = 4500;

    public const int COMET_EVOLVE_COST = 25000;
    public const int DENSEPLANET_EVOLVE_COST = 85000;
    public const int STAR_EVOLVE_COST = 250000;

    public const short ANTIGRAVITY_MAX_POINTS = 20;
    public const short QUANTUMTUNNEL_MAX_POINTS = 20;
    public const short SOLARFLARE_MAX_POINTS = 20;

    public const short GRB_MAX_POINTS = 3;
    public const short RESILIENCE_MAX_POINTS = 15;
    public const short THRUSTFORCE_MAX_POINTS = 30;

    #region Variables
    [Header("Preview")]
    [SerializeField] private GameObject previewStateSphere = null;
    [SerializeField] private Material asteroid = null;
    [SerializeField] private Material comet = null;
    [SerializeField] private Material densePlanet = null;
    [SerializeField] private Material star = null;
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
    [SerializeField] private Text healthInfoText = null;
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
    [SerializeField] private GameObject gammaRayBurstEffect = null;
    [SerializeField] private Text GRBCostText = null;
    [SerializeField] private Text GRBInfoText = null;
    [SerializeField] private Color GRB1;
    [SerializeField] private Color GRB2;
    [SerializeField] private Color GRB3;


    private int healthUpgradeCost;
    private int thrustForceUpgradeCost;
    private int antigravityUpgradeCost;
    private int quantumTunnelUpgradeCost;
    private int solarflareUpgradeCost;
    private int evolveCost;
    private int GRBUpgradeCost;

    private int gravityPoints;
    private PlayerData playerData;
    private PlayerSkillsData skillsData;
    #endregion

    private void Start()
    {
        playerData = SaveManager.GetInstance().LoadPersistentData(SaveManager.PLAYER_DATA).GetData<PlayerData>();

        thrustForceUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(playerData.thrustForcePoints, INIT_THRUST_COST);
        healthUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(playerData.GetHealthPoints(), INIT_HEALTH_COST);

        switch (playerData.playerState)
        {
            case PlayerManager.PlayerState.ASTEROID:
                evolveCost = COMET_EVOLVE_COST;
                break;
            case PlayerManager.PlayerState.COMET:
                evolveCost = DENSEPLANET_EVOLVE_COST;
                break;
            case PlayerManager.PlayerState.DENSE_PLANET:
                evolveCost = STAR_EVOLVE_COST;
                break;
        }
        SaveObject objectData;
        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.GRAVITYPOINTS_PATH);
        gravityPoints = objectData != null ? gravityPoints = objectData.GetData<int>() : 0;

        skillsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH).GetData<PlayerSkillsData>();

        antigravityUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(skillsData.antigravityPoints, INIT_ANTIGRAVITY_COST);
        quantumTunnelUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(skillsData.quantumTunnelPoints, INIT_QUANTUMTUNNEL_COST);
        solarflareUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(skillsData.solarflarePoints, INIT_SOLARFLARE_COST);
        GRBUpgradeCost = GameplayMath.GetInstance().GetGRBCost(skillsData.gammaRayBurstPoints);

        UpdateUIBasedOnPlayerState();
        UpdateUI();
    }

    private void FixedUpdate()
    {
        previewStateSphere.transform.Rotate(new Vector3(0f, 1f, 0f) * 0.35f);
    }

    private void UpdateUIBasedOnPlayerState()
    {
        switch (playerData.playerState)
        {
            case PlayerManager.PlayerState.ASTEROID:
                previewStateSphere.GetComponent<MeshRenderer>().material = asteroid;
                upgradeAntigravityBtn.gameObject.SetActive(true);
                antigravityPointsText.gameObject.SetActive(true);
                antigravityInfoText.gameObject.SetActive(true);

                upgradeQuantumTunnelBtn.gameObject.SetActive(false);
                quantumTunnelPointsText.gameObject.SetActive(false);
                quantumTunnelInfoText.gameObject.SetActive(false);

                upgradeSolarflareBtn.gameObject.SetActive(false);
                solarflarePointsText.gameObject.SetActive(false);
                solarflareInfoText.gameObject.SetActive(false);

                evolveBtn.gameObject.SetActive(true);
                evolveCostText.gameObject.SetActive(true);
                unlockSkillText.gameObject.SetActive(true);

                unlockGammaRayBurstText.gameObject.SetActive(true);
                gammaRayBurstEffect.SetActive(false);
                break;

            case PlayerManager.PlayerState.COMET:
                previewStateSphere.GetComponent<MeshRenderer>().material = comet;
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

                unlockGammaRayBurstText.gameObject.SetActive(true);
                gammaRayBurstEffect.SetActive(false);
                break;

            case PlayerManager.PlayerState.DENSE_PLANET:
                previewStateSphere.GetComponent<MeshRenderer>().material = densePlanet;
                upgradeAntigravityBtn.gameObject.SetActive(true);
                antigravityPointsText.gameObject.SetActive(true);
                antigravityInfoText.gameObject.SetActive(true);

                upgradeQuantumTunnelBtn.gameObject.SetActive(true);
                quantumTunnelPointsText.gameObject.SetActive(true);
                quantumTunnelInfoText.gameObject.SetActive(true);

                upgradeSolarflareBtn.gameObject.SetActive(true);
                solarflarePointsText.gameObject.SetActive(true);
                solarflareInfoText.gameObject.SetActive(true);

                evolveBtn.gameObject.SetActive(true);
                evolveCostText.gameObject.SetActive(true);
                unlockSkillText.gameObject.SetActive(false);

                unlockGammaRayBurstText.gameObject.SetActive(true);
                gammaRayBurstEffect.SetActive(false);
                break;

            case PlayerManager.PlayerState.STAR:
                previewStateSphere.GetComponent<MeshRenderer>().material = star;
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

                unlockGammaRayBurstText.gameObject.SetActive(false);
                gammaRayBurstEffect.SetActive(true);
                break;
        }
    }

    private void UpdateUI()
    {
        stateText.text = playerData.playerState.ToString();

        if (gravityPoints != -1)
        {
            gravityPointsText.text = gravityPoints.ToString();
        }

        if (playerData.thrustForcePoints < THRUSTFORCE_MAX_POINTS)
        {
            thrustCostText.text = thrustForceUpgradeCost.ToString();
        }
        else
        {
            thrustCostText.gameObject.SetActive(false);
        }

        if (playerData.GetHealthPoints() < RESILIENCE_MAX_POINTS)
        {
            healthCostText.text = healthUpgradeCost.ToString();
        }
        else
        {
            healthCostText.gameObject.SetActive(false);
        }

        //ANTIGRAVITY
        if (skillsData.antigravityPoints < ANTIGRAVITY_MAX_POINTS)
        {
            antigravityCostText.text = antigravityUpgradeCost.ToString();
        }
        else
        {
            antigravityCostText.gameObject.SetActive(false);
        }
        //QUANTUM TUNNEL
        if (skillsData.quantumTunnelPoints < QUANTUMTUNNEL_MAX_POINTS)
        {
            quantumTunnelCostText.text = quantumTunnelUpgradeCost.ToString();
        }
        else
        {
            quantumTunnelCostText.gameObject.SetActive(false);
        }
        //SOLAR FLARE
        if (skillsData.solarflarePoints < SOLARFLARE_MAX_POINTS)
        {
            solarflareCostText.text = solarflareUpgradeCost.ToString();
        }
        else
        {
            solarflareCostText.gameObject.SetActive(false);
        }
        //GRB
        if(skillsData.gammaRayBurstPoints < GRB_MAX_POINTS)
        {
            GRBCostText.text = GRBUpgradeCost.ToString();
        }
        else
        {
            GRBCostText.gameObject.SetActive(false);
        }
        UpdateGRBColor();

        evolveCostText.text = evolveCost.ToString();

        if (playerData != null)
        {
            thrustPointsText.text = playerData.thrustForcePoints.ToString();
            healthPointsText.text = playerData.GetHealthPoints().ToString();
            thrustForceText.text = "Thrust Force: " + GameplayMath.GetInstance().GetPlayerThrustForceFromPoints(playerData.thrustForcePoints).ToString("0.00");
            healthInfoText.text = "Health: " + playerData.health.ToString();
        }

        if (skillsData != null)
        {
            antigravityPointsText.text = skillsData.antigravityPoints.ToString();
            quantumTunnelPointsText.text = skillsData.quantumTunnelPoints.ToString();
            solarflarePointsText.text = skillsData.solarflarePoints.ToString();

            if (skillsData.antigravityPoints < ANTIGRAVITY_MAX_POINTS)
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

            if (skillsData.quantumTunnelPoints < QUANTUMTUNNEL_MAX_POINTS)
            {
                quantumTunnelInfoText.text = "Cooldown: <b>" + GameplayMath.GetInstance().GetQuantumTunnelCooldown(skillsData.quantumTunnelPoints).ToString("0.0") + "s</b>"
                                          + "\n<color=cyan>Next Upgrade: </color>\n"
                                          + "Cooldown: " + GameplayMath.GetInstance().GetQuantumTunnelCooldown(skillsData.quantumTunnelPoints + 1).ToString("0.0") + "s";
            }
            else
            {
                quantumTunnelInfoText.text = "Cooldown: <b>" + GameplayMath.GetInstance().GetQuantumTunnelCooldown(skillsData.quantumTunnelPoints).ToString("0.0") + "s</b>";
            }

            if (skillsData.solarflarePoints < SOLARFLARE_MAX_POINTS)
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

            if(skillsData.gammaRayBurstPoints < GRB_MAX_POINTS)
            {
                GRBInfoText.text = "Cooldown: <b>" + GameplayMath.GetInstance().GetGRBCooldown(skillsData.gammaRayBurstPoints).ToString("0.0") + "s</b>"
                                   + "\n<color=cyan>Next Upgrade: </color>\n"
                                   + "Cooldown: <b>" + GameplayMath.GetInstance().GetGRBCooldown(skillsData.gammaRayBurstPoints + 1).ToString("0.0") + "s</b>";
            }
            else
            {
                GRBInfoText.text = "Cooldown: <b>" + GameplayMath.GetInstance().GetGRBCooldown(skillsData.gammaRayBurstPoints).ToString("0.0") + "s</b>";
            }
        }
    }


    //Skills
    public void UpgradeAntiGravity()
    {
        if (skillsData.antigravityPoints >= ANTIGRAVITY_MAX_POINTS)
        {
            toast.ShowToast("Already at max level", null, 2f);
            return;
        }

        if (antigravityUpgradeCost <= gravityPoints)
        {
            if (skillsData != null)
            {
                skillsData.antigravityPoints++;
                gravityPoints -= antigravityUpgradeCost;
                antigravityUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(skillsData.antigravityPoints, INIT_ANTIGRAVITY_COST);
            }
            SaveManager.GetInstance().SavePersistentData(skillsData, SaveManager.SKILLSDATA_PATH);
            SaveManager.GetInstance().SavePersistentData(gravityPoints, SaveManager.GRAVITYPOINTS_PATH);
            UpdateUI();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }
    }

    public void UpgradeQuantumTunnel()
    {
        if (skillsData.quantumTunnelPoints >= QUANTUMTUNNEL_MAX_POINTS)
        {
            toast.ShowToast("Already at max level", null, 2f);
            return;
        }

        if (quantumTunnelUpgradeCost <= gravityPoints)
        {
            if (skillsData != null)
            {
                skillsData.quantumTunnelPoints++;
                gravityPoints -= quantumTunnelUpgradeCost;
                quantumTunnelUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(skillsData.quantumTunnelPoints, INIT_QUANTUMTUNNEL_COST);
            }
            SaveManager.GetInstance().SavePersistentData(skillsData, SaveManager.SKILLSDATA_PATH);
            SaveManager.GetInstance().SavePersistentData(gravityPoints, SaveManager.GRAVITYPOINTS_PATH);
            UpdateUI();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }
    }

    public void UpgradeSolarFlare()
    {
        if (skillsData.solarflarePoints >= SOLARFLARE_MAX_POINTS)
        {
            toast.ShowToast("Already at max level", null, 2f);
            return;
        }

        if (solarflareUpgradeCost <= gravityPoints)
        {
            if (skillsData != null)
            {
                skillsData.solarflarePoints++;
                gravityPoints -= solarflareUpgradeCost;
                solarflareUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(skillsData.solarflarePoints, INIT_SOLARFLARE_COST);
            }
            SaveManager.GetInstance().SavePersistentData(skillsData, SaveManager.SKILLSDATA_PATH);
            SaveManager.GetInstance().SavePersistentData(gravityPoints, SaveManager.GRAVITYPOINTS_PATH);
            UpdateUI();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }
    }


    public void UpgradeThrustForce()
    {
        if (playerData.thrustForcePoints >= THRUSTFORCE_MAX_POINTS)
        {
            toast.ShowToast("Already at max level", null, 1.5f);
            return;
        }
        if (thrustForceUpgradeCost <= gravityPoints)
        {
            if (playerData != null)
            {
                playerData.thrustForcePoints++;
                gravityPoints -= thrustForceUpgradeCost;
                thrustForceUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(playerData.thrustForcePoints, INIT_THRUST_COST);
            }
            SaveManager.GetInstance().SavePersistentData(playerData, SaveManager.PLAYER_DATA);
            SaveManager.GetInstance().SavePersistentData(gravityPoints, SaveManager.GRAVITYPOINTS_PATH);
            UpdateUI();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }
    }

    public void UpgradeResilience()
    {
        if (playerData.GetHealthPoints() >= RESILIENCE_MAX_POINTS)
        {
            toast.ShowToast("Already at max level", null, 1.5f);
            return;
        }
        if (healthUpgradeCost <= gravityPoints)
        {
            if (playerData != null)
            {
                playerData.IncreaseHealth();
                gravityPoints -= healthUpgradeCost;
                healthUpgradeCost = GameplayMath.GetInstance().GetCostFromInitCost(playerData.GetHealthPoints(), INIT_HEALTH_COST);
            }
            SaveManager.GetInstance().SavePersistentData(playerData, SaveManager.PLAYER_DATA);
            SaveManager.GetInstance().SavePersistentData(gravityPoints, SaveManager.GRAVITYPOINTS_PATH);
            UpdateUI();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }

    }

    public void UpgradeGRB()
    {
        if (skillsData.gammaRayBurstPoints >= GRB_MAX_POINTS)
        {
            toast.ShowToast("Already at max level", null, 1.5f);
            return;
        }
        if (GRBUpgradeCost <= gravityPoints)
        {
            if (skillsData != null)
            {
                skillsData.gammaRayBurstPoints++;
                gravityPoints -= GRBUpgradeCost;
                GRBUpgradeCost = GameplayMath.GetInstance().GetGRBCost(skillsData.gammaRayBurstPoints);
            }
            SaveManager.GetInstance().SavePersistentData(skillsData, SaveManager.SKILLSDATA_PATH);
            SaveManager.GetInstance().SavePersistentData(gravityPoints, SaveManager.GRAVITYPOINTS_PATH);
            UpdateUI();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }

    }

    private void UpdateGRBColor()
    {
        ParticleSystemRenderer[] systems = gammaRayBurstEffect.GetComponentsInChildren<ParticleSystemRenderer>();
        Color colorToSet = new Color(255, 255, 255, 255);
        switch (skillsData.gammaRayBurstPoints)
        {
            case 1:
                colorToSet = GRB1;
                break;
            case 2:
                colorToSet = GRB2;
                break;
            case 3:
                colorToSet = GRB3;
                break;
        }
        foreach (ParticleSystemRenderer system in systems)
        {
            system.material.SetColor("_TintColor", colorToSet);
        }
    }

    public void Evolve()
    {
        if (gravityPoints >= evolveCost && playerData.playerState != PlayerManager.PlayerState.STAR)
        {
            gravityPoints -= evolveCost;
            switch (playerData.playerState)
            {
                //Asteroid -> Comet
                case PlayerManager.PlayerState.ASTEROID:
                    playerData.playerState = PlayerManager.PlayerState.COMET;
                    evolveCost = DENSEPLANET_EVOLVE_COST;
                    break;
                //Comet -> Dense planet
                case PlayerManager.PlayerState.COMET:
                    playerData.playerState = PlayerManager.PlayerState.DENSE_PLANET;
                    evolveCost = STAR_EVOLVE_COST;
                    break;
                //Dense planet -> Star
                case PlayerManager.PlayerState.DENSE_PLANET:
                    playerData.playerState = PlayerManager.PlayerState.STAR;
                    break;
            }
            SaveManager.GetInstance().SavePersistentData(playerData, SaveManager.PLAYER_DATA);
            SaveManager.GetInstance().SavePersistentData(gravityPoints, SaveManager.GRAVITYPOINTS_PATH);
            UpdateUIBasedOnPlayerState();
            UpdateUI();
        }
        else
        {
            toast.ShowToast("Not enough Gravity Points", null, 1.5f);
        }
    }

}

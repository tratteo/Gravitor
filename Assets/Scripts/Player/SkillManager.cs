using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerManager))]
public class SkillManager : MonoBehaviour
{
    public enum Skill { ANTI_GRAVITY, QUANTUM_TUNNEL, SOLAR_FLARE, GAMMARAY_BURST }

    #region Variables

    //Inspector variables
    public GameObject skillSpawn = null;
    [Header("Anti Gravity")]
    [SerializeField] private GameObject antigravityEffect = null;
    [Header("Quantum tunnel")]
    [SerializeField] private GameObject quantumAppearEff = null;
    [SerializeField] private GameObject quantumDisappearEff = null;
    [Header("Solar Flare")]
    [SerializeField] private GameObject solarflareEffect = null;
    [Header("Gamma Ray Burst")]
    [Header("Channels")]
    [SerializeField] private GameObject channel1 = null;
    [SerializeField] private GameObject channel2 = null;
    [SerializeField] private GameObject channel3 = null;
    [SerializeField] private GameObject channel4 = null;
    [Header("Effects")]
    [SerializeField] private GameObject GRB1 = null;
    [SerializeField] private GameObject GRB2 = null;
    [SerializeField] private GameObject GRB3 = null;
    [SerializeField] private GameObject GRB4 = null;


    private GameObject GRBEffect = null;
    private GameObject GRBChannelEffect = null;

    [SerializeField] private float gammaRayLength = 800f;
    [SerializeField] private float unscaledGammaRayRadius = 70f;
    private float scaledGammaRayRadius;
    //Bools
    [HideInInspector] public bool isAntiGravityActive = false;
    [HideInInspector] public bool isGravitable = true;
    [HideInInspector] public bool isQuantumTunnelSelectionActive = false;
    [HideInInspector] public int sessionObstaclesDestroyed = 0;

    //References
    private PlayerManager playerManager = null;

    //Local variables
    private LayerMask mask;
    private ParticleSystem channelGammaRaySys;
    private PlayerSkillsData skillsData = null;
    private float antigravityCooldown, quantumtunnelCooldown, solarflareCooldown;
    private float solarflareRadius;
    [HideInInspector] public bool canCastSkill = true;
    private HUDManager hudManager;

    #endregion

    private void Start()
    {
        playerManager = gameObject.GetComponent<PlayerManager>();
        skillsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH).GetData<PlayerSkillsData>();

        unscaledGammaRayRadius = GameplayMath.GetInstance().GetGRBUnscaledRadius(skillsData.gammaRayBurstPoints);
        UpdateGRBEffect();
        antigravityCooldown = GameplayMath.GetInstance().GetAntigravityCooldown(skillsData.antigravityPoints);
        quantumtunnelCooldown = GameplayMath.GetInstance().GetQuantumTunnelCooldown(skillsData.quantumTunnelPoints);
        solarflareCooldown = GameplayMath.GetInstance().GetSolarflareCooldown(skillsData.solarflarePoints);
        solarflareRadius = GameplayMath.GetInstance().GetSolarflareRadius(skillsData.solarflarePoints);
        scaledGammaRayRadius = unscaledGammaRayRadius * ((transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3);
        hudManager = HUDManager.GetInstance();
    }

    private void UpdateGRBEffect()
    {
        switch (skillsData.gammaRayBurstPoints)
        {
            case 1:
                GRBEffect = GRB1;
                GRBChannelEffect = channel1;
                break;
            case 2:
                GRBEffect = GRB2;
                GRBChannelEffect = channel2;
                break;
            case 3:
                GRBEffect = GRB3;
                GRBChannelEffect = channel3;
                break;
            case 4:
                GRBEffect = GRB4;
                GRBChannelEffect = channel4;
                break;
        }

    }

    private void Update()
    {
        if (canCastSkill && isQuantumTunnelSelectionActive)
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    StartCoroutine(QuantumTunnel_C(Input.mousePosition));
            //}

            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                StartCoroutine(QuantumTunnel_C(Input.GetTouch(0).position));
            }
            else if (Input.touchCount > 1 && Input.GetTouch(1).phase == TouchPhase.Began)
            {
                StartCoroutine(QuantumTunnel_C(Input.GetTouch(1).position));
            }
        }
    }


    //AntiGravity
    public void AntiGravity()
    {
        if (canCastSkill)
        {
            StartCoroutine(AntiGravity_C());
        }
    }
    private IEnumerator AntiGravity_C()
    {
        float duration = GameplayMath.GetInstance().GetAntigravityDuration(skillsData.antigravityPoints);
        isAntiGravityActive = true;
        isGravitable = false;

        GameObject antigravityEffRef = InstantiateEffect(antigravityEffect);

        Image cooldownOverlay = hudManager.antigravityBtn.GetComponentsInChildren<Image>()[1];
        cooldownOverlay.fillAmount = 1f;
        hudManager.antigravityBtn.GetComponent<Image>().raycastTarget = false;
        yield return new WaitForSeconds(duration);
        Destroy(antigravityEffRef);

        isGravitable = true;
        isAntiGravityActive = false;

        hudManager.CoolDownSkill(Skill.ANTI_GRAVITY, antigravityCooldown);
    }


    //QuantumTunnel
    public IEnumerator QuantumTunnel_C(Vector3 clickPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(clickPos);
        int layerMask = LayerMask.GetMask("UI");
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.yellow, 2f);
            Debug.Log("x: " + hit.point.x + ", y: " + hit.point.y);
            Vector3 pos;
            pos.x = hit.point.x;
            pos.y = 2f * hit.point.y - transform.position.y;
            pos.z = transform.position.z;

            hudManager.QuantumTunnelSelectionActive(false);
            InstantiateEffect(quantumDisappearEff);
            yield return new WaitForSeconds(0.1f);
            SharedUtilities.GetInstance().MakeGameObjectVisible(gameObject, false);
            SharedUtilities.GetInstance().MakeGameObjectVisible(skillSpawn, false);

            isGravitable = false;
            transform.position = pos;
            yield return new WaitForSeconds(0.1f);
            if (!playerManager.isDead)
            {
                InstantiateEffect(quantumAppearEff);
                SharedUtilities.GetInstance().MakeGameObjectVisible(gameObject, true);
                SharedUtilities.GetInstance().MakeGameObjectVisible(skillSpawn, true);
            }
            if (isAntiGravityActive)
            {
                isGravitable = false;
            }
            else
            {
                isGravitable = true;
            }

            hudManager.CoolDownSkill(Skill.QUANTUM_TUNNEL, quantumtunnelCooldown);
        }
    }

    //SolarFlare
    public void SolarFlare()
    {
        if (canCastSkill)
        {
            StartCoroutine(SolarFlare_C());
        }
    }
    private IEnumerator SolarFlare_C()
    {
        float animDuration = solarflareEffect.GetComponent<ParticleSystem>().main.duration;
        InstantiateEffect(solarflareEffect);

        hudManager.CoolDownSkill(Skill.SOLAR_FLARE, solarflareCooldown);
        yield return new WaitForSeconds(animDuration);

        mask = LayerMask.GetMask("Obstacles");
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, solarflareRadius, mask);
        int castedObstacles = 0;
        float bonusScore = 0;
        foreach (Collider collider in hitColliders)
        {
            GameObstacle obstacleToDestroy;
            obstacleToDestroy = collider.transform.gameObject.GetComponent<GameObstacle>();
            if (obstacleToDestroy != null)
            {
                castedObstacles++;
                obstacleToDestroy.Destroy(true);
                bonusScore += GameplayMath.GetInstance().GetBonusPointsFromObstacleMass(obstacleToDestroy.mass);
                playerManager.gravityFieldsGens.Remove(obstacleToDestroy);
            }
        }
        playerManager.gameMode.BonusScore(bonusScore);

        sessionObstaclesDestroyed += castedObstacles;

        if (playerManager.level.category == Level.LevelCategory.OBSTACLES_DESTROY && sessionObstaclesDestroyed >= playerManager.level.targetObstaclesDestoryed)
        {
            playerManager.LevelCompleted();
        }

        playerManager.dangerZoneCount = playerManager.dangerZoneCount > castedObstacles ? playerManager.dangerZoneCount - (short)castedObstacles : 0;

        if (playerManager.dangerZoneCount <= 0)
        {
            hudManager.EnableHighGravityFieldPanel(false);
        }
    }


    //GammaRayBurst
    public void GammaRayBurst()
    {
        if (canCastSkill)
        {
            StartCoroutine(GammaRayBurst_C());
        }
    }

    private IEnumerator GammaRayAsyncLoad_C()
    {
        Image cooldownOverlay = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<Image>(hudManager.gammaRayBurstBtn, "FilledOverlay");
        cooldownOverlay.fillAmount = 1f;
        hudManager.gammaRayBurstBtn.GetComponent<Image>().raycastTarget = false;
        GameObject channelGammaRayRef = InstantiateEffect(GRBChannelEffect);

        mask = LayerMask.GetMask("Obstacles");
        channelGammaRaySys = channelGammaRayRef.GetComponentInChildren<ParticleSystem>();
        yield return null;
    }

    private IEnumerator GammaRayBurst_C()
    {
        StartCoroutine(GammaRayAsyncLoad_C());
        yield return new WaitForSeconds(1.5f);

        if (channelGammaRaySys)
        {
            channelGammaRaySys.Stop();
        }

        Vector3 exceptionCentre = new Vector3(transform.position.x, transform.position.y, playerManager.gameMode.randZSpawn.x);
        playerManager.gameMode.NotifySpawnException(exceptionCentre, scaledGammaRayRadius, scaledGammaRayRadius, 1f, GameplayMath.GetInstance().GetGRBSpawnExceptionTime(skillsData.gammaRayBurstPoints));

        InstantiateEffect(GRBEffect);
        hudManager.CoolDownSkill(Skill.GAMMARAY_BURST, GameplayMath.GetInstance().GetGRBCooldown(skillsData.gammaRayBurstPoints));

        RaycastHit[] hitColliders = Physics.SphereCastAll(transform.position, scaledGammaRayRadius, new Vector3(0f, 0f, 1f), gammaRayLength, mask);

        int castedObstacles = 0;
        float bonusScore = 0;
        foreach (RaycastHit hit in hitColliders)
        {
            GameObstacle obstacleToDestroy;
            obstacleToDestroy = hit.transform.gameObject.GetComponent<GameObstacle>();
            if (obstacleToDestroy != null)
            {
                castedObstacles++;
                obstacleToDestroy.Destroy(true);
                bonusScore += GameplayMath.GetInstance().GetBonusPointsFromObstacleMass(obstacleToDestroy.mass);
                playerManager.gravityFieldsGens.Remove(obstacleToDestroy);
            }
        }
        playerManager.gameMode.BonusScore(bonusScore);
        Debug.Log("GRB D: " + castedObstacles);
        sessionObstaclesDestroyed += castedObstacles;
        if (playerManager.level.category == Level.LevelCategory.OBSTACLES_DESTROY && sessionObstaclesDestroyed >= playerManager.level.targetObstaclesDestoryed)
        {
            playerManager.LevelCompleted();
        }

        playerManager.dangerZoneCount = playerManager.dangerZoneCount > castedObstacles ? playerManager.dangerZoneCount - (short)castedObstacles : 0;

        if (playerManager.dangerZoneCount <= 0)
        {
            hudManager.EnableHighGravityFieldPanel(false);
        }
        //yield return new WaitForSeconds(raySystem.main.duration);
    }

    //Utils
    public GameObject InstantiateEffect(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, skillSpawn.transform.position, Quaternion.identity);
        obj.transform.SetParent(skillSpawn.transform);
        obj.transform.localScale = new Vector3(1, 1, 1);
        return obj;
    }
}

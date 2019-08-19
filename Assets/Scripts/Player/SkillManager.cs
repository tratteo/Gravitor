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
    [SerializeField] private GameObject gammaRayEffect = null;
    [SerializeField] private GameObject channelGammaRayEffect = null;
    [SerializeField] private float gammaRayLength = 800f;
    [SerializeField] private float unscaledGammaRayRadius = 15f;
    private float scaledGammaRayRadius;
    //Bools
    [HideInInspector] public bool isAntiGravityActive = false;
    [HideInInspector] public bool isGravitable = true;
    [HideInInspector] public bool isQuantumTunnelSelectionActive = false;

    //References
    private PlayerManager playerManager = null;

    //Local variables
    private LayerMask mask;
    private ParticleSystem channelGammaRaySys;
    private ParticleSystem raySystem;
    private PlayerSkillsData skillsData = null;
    private float antigravityCooldown, quantumtunnelCooldown, solarflareCooldown;
    private float solarflareRadius;
    [HideInInspector] public bool canCastSkill = true;

    #endregion

    private void Start()
    {
        playerManager = gameObject.GetComponent<PlayerManager>();
        skillsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH).GetData<PlayerSkillsData>();

        antigravityCooldown = GameplayMath.GetInstance().GetAntigravityCooldown(skillsData.antigravityPoints);
        quantumtunnelCooldown = GameplayMath.GetInstance().GetQuantumTunnelCooldown(skillsData.quantumTunnelPoints);
        solarflareCooldown = GameplayMath.GetInstance().GetSolarflareCooldown(skillsData.solarflarePoints);
        solarflareRadius = GameplayMath.GetInstance().GetSolarflareRadius(skillsData.solarflarePoints);
        scaledGammaRayRadius = unscaledGammaRayRadius * ((transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3);
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

        Image cooldownOverlay = HUDManager.GetInstance().antigravityBtn.GetComponentsInChildren<Image>()[1];
        cooldownOverlay.fillAmount = 1f;
        EventTrigger eventTrigger = HUDManager.GetInstance().antigravityBtn.GetComponent<EventTrigger>();
        eventTrigger.enabled = false;
        yield return new WaitForSeconds(duration);
        Destroy(antigravityEffRef);

        isGravitable = true;
        isAntiGravityActive = false;

        HUDManager.GetInstance().CoolDownSkill(Skill.ANTI_GRAVITY, antigravityCooldown);
    }


    //QuantumTunnel
    public IEnumerator QuantumTunnel_C(Vector3 clickPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(clickPos);
        int layerMask = LayerMask.GetMask("UI");
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            playerManager.timeDistortion = 1f;
            Vector3 pos = transform.position;
            pos.x = hit.point.x;
            pos.y = hit.point.y;

            HUDManager.GetInstance().QuantumTunnelSelectionActive(false);
            InstantiateEffect(quantumDisappearEff);
            yield return new WaitForSeconds(0.2f);
            SharedUtilities.GetInstance().MakeGameObjectVisible(gameObject, false);
            SharedUtilities.GetInstance().MakeGameObjectVisible(skillSpawn, false);

            isGravitable = false;
            transform.position = pos;
            yield return new WaitForSeconds(0.2f);
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

            HUDManager.GetInstance().CoolDownSkill(Skill.QUANTUM_TUNNEL, quantumtunnelCooldown);
        }
    }

    private bool IsValidQuantumTunnelLocation(RaycastHit hit)
    {
        return (hit.point.x > playerManager.gameMode.obstacleRandXSpawn.x && hit.point.x < playerManager.gameMode.obstacleRandXSpawn.y &&
                hit.point.y > playerManager.gameMode.obstacleRandYSpawn.x && hit.point.y < playerManager.gameMode.obstacleRandYSpawn.y);
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

        HUDManager.GetInstance().CoolDownSkill(Skill.SOLAR_FLARE, solarflareCooldown);
        yield return new WaitForSeconds(animDuration);

        mask = LayerMask.GetMask("Obstacles");
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, solarflareRadius, mask);
        int castedObstacles = 0;
        foreach (Collider collider in hitColliders)
        {
            GameObstacle obstacleToDestroy;
            obstacleToDestroy = collider.transform.gameObject.GetComponent<GameObstacle>();
            if (obstacleToDestroy != null)
            {
                castedObstacles++;
                obstacleToDestroy.Destroy(true);
                playerManager.gameMode.BonusScore(GameplayMath.GetInstance().GetBonusPointsFromObstacleMass(obstacleToDestroy.gameObject.GetComponent<GameObstacle>().mass));
            }
        }
        playerManager.timeDistortion = 1;
        playerManager.dangerZoneCount = playerManager.dangerZoneCount > castedObstacles ? playerManager.dangerZoneCount -= (short)castedObstacles : playerManager.dangerZoneCount = 0;
        playerManager.gravityFieldCount = playerManager.gravityFieldCount > castedObstacles ? playerManager.gravityFieldCount -= (short)castedObstacles : playerManager.gravityFieldCount = 0;

        if (playerManager.dangerZoneCount <= 0)
        {
            HUDManager.GetInstance().ShowDangerZoneUI(false);
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
        Image cooldownOverlay = HUDManager.GetInstance().gammaRayBurstBtn.GetComponentsInChildren<Image>()[1];
        cooldownOverlay.fillAmount = 1f;
        EventTrigger eventTrigger = HUDManager.GetInstance().gammaRayBurstBtn.GetComponent<EventTrigger>();
        eventTrigger.enabled = false;
        GameObject channelGammaRayRef = InstantiateEffect(channelGammaRayEffect);

        mask = LayerMask.GetMask("Obstacles");
        channelGammaRaySys = channelGammaRayRef.GetComponentInChildren<ParticleSystem>();
        raySystem = gammaRayEffect.GetComponentInChildren<ParticleSystem>();
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

        if (playerManager.gameMode.GetType().Name.Equals("LinearMode"))
        {
            Vector3 exceptionCentre = new Vector3(transform.position.x, transform.position.y, playerManager.gameMode.obstacleRandZSpawn.x);
            playerManager.gameMode.NotifySpawnException(exceptionCentre, scaledGammaRayRadius, scaledGammaRayRadius, 1f, 6f);
        }

        InstantiateEffect(gammaRayEffect);
        HUDManager.GetInstance().CoolDownSkill(Skill.GAMMARAY_BURST, 45f);

        RaycastHit[] hitColliders = Physics.SphereCastAll(transform.position, scaledGammaRayRadius, new Vector3(0f, 0f, 1f), gammaRayLength, mask);

        int castedObstacles = 0;
        foreach (RaycastHit hit in hitColliders)
        {
            GameObstacle obstacleToDestroy;
            obstacleToDestroy = hit.transform.gameObject.GetComponent<GameObstacle>();
            if (obstacleToDestroy != null)
            {
                castedObstacles++;
                obstacleToDestroy.Destroy(true);
                playerManager.gameMode.BonusScore(GameplayMath.GetInstance().GetBonusPointsFromObstacleMass(obstacleToDestroy.gameObject.GetComponent<GameObstacle>().mass));
            }
        }
        playerManager.timeDistortion = 1;
        playerManager.dangerZoneCount = playerManager.dangerZoneCount > castedObstacles ? playerManager.dangerZoneCount -= (short)castedObstacles : playerManager.dangerZoneCount = 0;
        playerManager.gravityFieldCount = playerManager.gravityFieldCount > castedObstacles ? playerManager.gravityFieldCount -= (short)castedObstacles : playerManager.gravityFieldCount = 0;
        yield return new WaitForSeconds(raySystem.main.duration);
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

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerManager))]
public class ExtraManager : MonoBehaviour
{
    private int maxCollectableShields = 3;
    [HideInInspector] public int shieldDuration = 7;

    [HideInInspector] public bool isShielded = false;
    private GameObject shieldEffectRef = null;
    private PlayerManager playerManager = null;

    private Queue<Shield> shields;
    private IEnumerator shield_c;
    private Shield currentShield;

    private int shieldCount = 0;
    private Wormhole warpDrive;
    [HideInInspector] public int enqueuedShields = 0;

    private void Start()
    {
        shields = new Queue<Shield>();
        shieldCount = 0;
        playerManager = GetComponent<PlayerManager>();
        PlayerSkillsData skillsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SKILLSDATA_PATH).GetData<PlayerSkillsData>();
        if(skillsData.magneticShieldBundleUnlocked)
        {
            maxCollectableShields = 5;
            shieldDuration = 14;
        }
        else
        {
            maxCollectableShields = 3;
            shieldDuration = 8;
        }
    }

    //PICKUPS
    public void ApplyPickUp(Extra pickup)
    {
        switch (pickup.gameObject.tag)
        {
            case "Shield":
                if(shieldCount < maxCollectableShields)
                {
                    Shield shield = (Shield)pickup;
                    shields.Enqueue(shield);
                    shieldCount++;
                    HUDManager.GetInstance().CollectShield(shieldCount);
                }
                break;
            case "WarpDrive":
                warpDrive = (Wormhole)pickup;
                StartCoroutine(Wormhole_C(warpDrive));
                break;
        }
    }

    public int Shield()
    {
        if (shieldCount <= 0) return -1;
        enqueuedShields++;
        shieldCount--;
        Shield shield = shields.Dequeue();
        currentShield = shield;
        if (!isShielded)
        {
            shield_c = Shield_C(shield);
            StartCoroutine(shield_c);
        }
        else
        {
            StartCoroutine(WaitForNewShield(shield));
        }
        return shieldCount;
    }
    private IEnumerator WaitForNewShield(Shield shield)
    {
        while (isShielded)
        {
            yield return null;
        }
        shield_c = Shield_C(shield);
        StartCoroutine(shield_c);
    }
    private IEnumerator Shield_C(Shield shield)
    {
        HUDManager.GetInstance().ShieldUsed();
        isShielded = true;
        shieldEffectRef = playerManager.skillManager.InstantiateEffect(currentShield.shieldEffect);
        ParticleSystem shieldSys = shieldEffectRef.GetComponent<ParticleSystem>();
        float duration = shieldSys.main.duration;
        yield return new WaitForSeconds(shieldDuration - duration);
        if (shieldSys)
        {
            shieldSys.Stop();
        }
        yield return new WaitForSeconds(duration);
        enqueuedShields--;
        isShielded = false;
    }
    public void DestroyShield()
    {
        enqueuedShields--;
        StopCoroutine(shield_c);
        HUDManager.GetInstance().ShieldDestroyed();
        isShielded = false;
        if (shieldEffectRef != null)
        {
            Destroy(shieldEffectRef);
        }
        currentShield.DestroyShield(transform.position);
    }

    private IEnumerator Wormhole_C(Wormhole wormhole)
    {
        playerManager.ExitedDamageNebula(Margin.MarginLocation.BOTTOM);
        playerManager.ExitedDamageNebula(Margin.MarginLocation.TOP);
        playerManager.ExitedDamageNebula(Margin.MarginLocation.LEFT);
        playerManager.ExitedDamageNebula(Margin.MarginLocation.RIGHT);

        playerManager.skillManager.isGravitable = false;
        playerManager.skillManager.canCastSkill = false;
        playerManager.isGravityTdActive = false;

        float currentVTd = playerManager.movementManager.velocityTimeDistrotion;

        playerManager.movementManager.DisableMovement();
        Collider playerCollider = GetComponent<Collider>();
        playerCollider.enabled = false;
        playerManager.gravityFieldsGens.Clear();

        playerManager.dangerZoneCount = 0;
        HUDManager.GetInstance().EnableHighGravityFieldPanel(false);

        CameraManager.GetInstance().SmoothInAndOutFOV(null, wormhole.viewDistortion, 0.2f, wormhole.duration - 0.4f);
        playerManager.movementManager.SpeedEffect(wormhole.duration, wormhole.relativeSpeed, true, true);
        playerManager.scoreMultiplier = wormhole.scoreMultiplier;

        yield return new WaitForSeconds(wormhole.duration);

        playerManager.movementManager.velocityTimeDistrotion = currentVTd;

        playerCollider.enabled = true;
        playerManager.movementManager.EnableMovement();
        playerManager.skillManager.canCastSkill = true;
        playerManager.skillManager.isGravitable = true;
        playerManager.isGravityTdActive = true;
    }
}

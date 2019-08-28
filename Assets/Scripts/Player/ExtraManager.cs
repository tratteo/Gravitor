using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerManager))]
public class ExtraManager : MonoBehaviour
{
    [SerializeField] private int maxCollectableShields = 4;
    [HideInInspector] public bool isShielded = false;
    private GameObject shieldEffectRef = null;
    private PlayerManager playerManager = null;

    private Queue<Shield> shields;
    private IEnumerator shield_c;
    private Shield currentShield;
    public Shield GetCurrentShield() { return currentShield; }

    private int shieldCount = 0;
    private Wormhole warpDrive;
    [HideInInspector] public int enqueuedShields = 0;

    private void Start()
    {
        shields = new Queue<Shield>();
        shieldCount = 0;
        playerManager = GetComponent<PlayerManager>();
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
                StartCoroutine(WarpDrive(warpDrive));
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
        yield return new WaitForSeconds(currentShield.duration - 1f);
        if (shieldSys)
        {
            shieldSys.Stop();
        }
        yield return new WaitForSeconds(0.75f);
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

    
    private IEnumerator WarpDrive(Wormhole wormhole)
    {
        playerManager.skillManager.isGravitable = false;
        playerManager.skillManager.canCastSkill = false;
        Collider playerCollider = GetComponent<Collider>();

        playerCollider.enabled = false;
        playerManager.dangerZoneCount = 0;
        HUDManager.GetInstance().ShowHighGravityPanel(false);
        playerManager.movementManager.DisableMovement();

        CameraManager.GetInstance().SmoothInAndOutFOV(null, wormhole.viewDistortion, 0.3f, wormhole.duration - 0.6f);
        playerManager.movementManager.SpeedEffect(wormhole.duration, 5f);
        playerManager.timeDistortion = GameplayMath.GetInstance().GetSpeedTd(wormhole.speed);
        playerManager.scoreMultiplier = wormhole.scoreMultiplier;

        yield return new WaitForSeconds(wormhole.duration);

        playerManager.timeDistortion = 1f;
        playerCollider.enabled = true;
        playerManager.movementManager.EnableMovement();
        playerManager.skillManager.canCastSkill = true;
        playerManager.skillManager.isGravitable = true;
    }
}

using UnityEngine;

public abstract class Extra : MonoBehaviour
{
    protected ParticleSystem effect;

    protected void Start()
    {
        effect = GetComponentInChildren<ParticleSystem>();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PlayerManager playerManager = other.gameObject.GetComponent<PlayerManager>();
            playerManager.extraManager.ApplyPickUp(this);
            if(effect)
                effect.Stop();
            PoolManager.GetInstance().DeactivateObject(gameObject);
        }
        else if (other.tag.Equals("KillVolume"))
        {
            PoolManager.GetInstance().DeactivateObject(gameObject);
        }
    } 
}

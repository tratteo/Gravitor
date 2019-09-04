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
            gameObject.SetActive(false);
        }
        else if (other.tag.Equals("KillVolume"))
        {
            gameObject.SetActive(false);
        }
        else if (other.tag.Equals("Obstacle"))
        {
            gameObject.SetActive(false);
        }
    } 
}

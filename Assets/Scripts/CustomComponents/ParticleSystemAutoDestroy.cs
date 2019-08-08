using UnityEngine;
using System.Collections;

public class ParticleSystemAutoDestroy : MonoBehaviour
{
    private new ParticleSystem particleSystem;

    public void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if (particleSystem)
        {
            if (!particleSystem.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}
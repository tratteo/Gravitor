using UnityEngine;
using System.Collections;

/// <summary>
/// Attach this component to a particleSystem, once the particle system is stopped od finished the script will destory the gameObject
/// </summary>
public class ParticleSystemAutoDestroy : MonoBehaviour
{
    public enum Type { DEACTIVATE, DESTROY }
    private new ParticleSystem particleSystem;
    public Type destroyType;

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
                switch(destroyType)
                {
                    case Type.DEACTIVATE:
                        gameObject.SetActive(false);
                        break;
                    case Type.DESTROY:
                        Destroy(gameObject);
                        break;
                }
            }
        }
    }
}
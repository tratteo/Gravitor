using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour, IPooledObject
{
    public enum ObstacleType { PLANET, STAR, WHITE_DWARF, NEUTRON_STAR };
    //USING CUSTOM EDITOR
    public ObstacleType type;
    protected MeshRenderer meshRenderer;
    public GameMode gameMode;
    protected float alpha;
    public bool randomizeMaterials;
    public Material[] materialsPool;
    public int minScale;
    public int maxScale;
    public GameObject starFlare;
    public bool rotate = false;


    [HideInInspector] public float targetScale;
    private Vector3 rotationVelocity;
    protected void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        gameMode = FindObjectOfType<GameMode>();
    }

    public virtual void OnObjectSpawn()
    {
        targetScale = Random.Range(minScale, maxScale);

        if (randomizeMaterials)
        {
            if (materialsPool.Length > 0 && meshRenderer != null)
            {
                meshRenderer.material = materialsPool[Random.Range(0, materialsPool.Length)];
            }
        }
        SetupObstacle();
    }

    protected virtual void FixedUpdate()
    {
        if (rotate)
        {
            transform.Rotate(rotationVelocity);
        }
    }

    public virtual void SetupObstacle()
    {
        if (rotate)
        {
            float rotationSpeed = Random.Range(0.1f, 0.4f);
            rotationVelocity = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)) * rotationSpeed;
        }

        StartCoroutine(ScaleCoroutine());

        Light pointLight;
        switch (type)
        {
            case ObstacleType.STAR:
                GameObject starFlareRef = Instantiate(starFlare, transform.position, transform.rotation);
                starFlareRef.transform.SetParent(transform);
                starFlareRef.transform.localScale = Vector3.one;
                ParticleSystem.ShapeModule shapeModule = starFlareRef.GetComponent<ParticleSystem>().shape;
                shapeModule.radius = transform.localScale.x / 2;
                pointLight = GetComponentInChildren<Light>();
                pointLight.range = transform.localScale.x * 2f;
                pointLight.intensity = transform.localScale.x * 10;
                break;

            case ObstacleType.WHITE_DWARF:
                pointLight = GetComponentInChildren<Light>();
                pointLight.range = transform.localScale.x * 5f;
                pointLight.intensity = transform.localScale.x * 60;
                break;
        }
    }

    public IEnumerator ScaleCoroutine()
    {
        float scale = 0f;
        float stride = 20f;
        int quality = QualitySettings.GetQualityLevel();
        switch (quality)
        {
            case SettingsManager.LOW:
                stride = 75f;
                break;
            case SettingsManager.MEDIUM:
                stride = 50f;
                break;
            case SettingsManager.HIGH:
                stride = 35f;
                break;
            case SettingsManager.ULTRA:
                stride = 20f;
                break;
        }

        transform.localScale = new Vector3(0f, 0f, 0f);
        while (scale + stride <= targetScale)
        {
            scale += stride;
            transform.localScale = new Vector3(scale, scale, scale);
            yield return new WaitForFixedUpdate();
        }
        transform.localScale = new Vector3(targetScale, targetScale, targetScale);
    }

    public virtual void DeactivateObstacle()
    {
        PoolManager.GetInstance().DeactivateObject(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "KillVolume")
        {
            PoolManager.GetInstance().DeactivateObject(gameObject);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Obstacle"))
        {
            if (transform.localScale.x < collision.transform.localScale.x)
            {
                PoolManager.GetInstance().DeactivateObject(gameObject);
            }
        }
    }
}

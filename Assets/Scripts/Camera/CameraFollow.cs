using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float hardness = 5f;
    public bool lookAt;

    void Start()
    {
        SettingsData settingsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SETTINGS_PATH).GetData<SettingsData>();
        lookAt = settingsData.cameraLookAt;
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 expectedPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, expectedPosition, hardness * Time.deltaTime);
            transform.position = smoothedPosition;
            if(lookAt)
                transform.LookAt(target);
        }
    }
}

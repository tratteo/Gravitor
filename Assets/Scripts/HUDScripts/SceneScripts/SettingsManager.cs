using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class SettingsManager : MonoBehaviour
{
    public const int LOW = 0;
    public const int MEDIUM = 1;
    public const int HIGH = 2;
    public const int ULTRA = 3;
    private string qualitySettings = "";
    [SerializeField] private ToastScript toast = null;
    [SerializeField] private Sprite enabled = null;
    [SerializeField] private Sprite disabled = null;
    [Header("Quality settings")]
    [SerializeField] private Image lowEnabledIcon = null;
    [SerializeField] private Image mediumEnabledIcon = null;
    [SerializeField] private Image highEnabledIcon = null;
    [SerializeField] private Image ultraEnabledIcon = null;
    [Header("In Game Options")]
    [SerializeField] private GameObject soundBtn = null;
    [SerializeField] private GameObject fpsBtn = null;
    [SerializeField] private GameObject cameraLookAtBtn = null;
    [SerializeField] private GameObject rightJoystickBtn = null;
    [SerializeField] private GameObject leftJoystickBtn = null;
    [SerializeField] private Dropdown dropdown = null;


    private SettingsData settingsData;

    void Start()
    {
        settingsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SETTINGS_PATH).GetData<SettingsData>();
        UpdateControlsLayoutUI(settingsData.controlsLayout);
        UpdateButton(fpsBtn, settingsData.showFPS);
        UpdateButton(soundBtn, settingsData.audioActive);
        UpdateButton(cameraLookAtBtn, settingsData.cameraLookAt);

        switch (settingsData.hudConf)
        {
            case SettingsData.EndlessModeHUD.DISTANCE:
                dropdown.value = dropdown.options.FindIndex(option => option.text == "Distance");
                break;
            case SettingsData.EndlessModeHUD.TIME:
                dropdown.value = dropdown.options.FindIndex(option => option.text == "Proper time");
                break;
            case SettingsData.EndlessModeHUD.TIME_DILATED:
                dropdown.value = dropdown.options.FindIndex(option => option.text == "Dilated time");
                break;
            case SettingsData.EndlessModeHUD.SPEED:
                dropdown.value = dropdown.options.FindIndex(option => option.text == "Speed");
                break;
            case SettingsData.EndlessModeHUD.OBSTACLES_DESTROYED:
                dropdown.value = dropdown.options.FindIndex(option => option.text == "Celestial bodies destroyed");
                break;
            default:
                break;
        }
    }

    public void SetQualitySettings(string level)
    {
        if (qualitySettings.Equals(level)) return;

        switch(level)
        {
            case "Low":
                QualitySettings.SetQualityLevel(LOW);
                QualitySettings.masterTextureLimit = 2;
                toast.EnqueueToast("Quality set to low", null, 1f);
                qualitySettings = "Low";
                lowEnabledIcon.gameObject.SetActive(true);
                mediumEnabledIcon.gameObject.SetActive(false);
                highEnabledIcon.gameObject.SetActive(false);
                ultraEnabledIcon.gameObject.SetActive(false);
                break;

            case "Medium":
                QualitySettings.SetQualityLevel(MEDIUM);
                QualitySettings.masterTextureLimit = 1;
                toast.EnqueueToast("Quality set to medium", null, 1f);
                qualitySettings = "Medium";
                lowEnabledIcon.gameObject.SetActive(false);
                mediumEnabledIcon.gameObject.SetActive(true);
                highEnabledIcon.gameObject.SetActive(false);
                ultraEnabledIcon.gameObject.SetActive(false);
                break;

            case "High":
                QualitySettings.SetQualityLevel(HIGH);
                QualitySettings.masterTextureLimit = 0;
                toast.EnqueueToast("Quality set to high", null, 1f);
                qualitySettings = "High";
                lowEnabledIcon.gameObject.SetActive(false);
                mediumEnabledIcon.gameObject.SetActive(false);
                highEnabledIcon.gameObject.SetActive(true);
                ultraEnabledIcon.gameObject.SetActive(false);
                break;

            case "Ultra":
                QualitySettings.SetQualityLevel(ULTRA);
                QualitySettings.masterTextureLimit = 0;
                toast.EnqueueToast("Quality set to ultra", null, 1f);
                qualitySettings = "Ultra";
                lowEnabledIcon.gameObject.SetActive(false);
                mediumEnabledIcon.gameObject.SetActive(false);
                highEnabledIcon.gameObject.SetActive(false);
                ultraEnabledIcon.gameObject.SetActive(true);
                break;
        }
    }


    private void UpdateControlsLayoutUI(SettingsData.ControlsLayout layout)
    {
        GameObject rightCB, leftCB;
        SharedUtilities sharedUtilsInstance = SharedUtilities.GetInstance();
        switch (layout)
        {
            case SettingsData.ControlsLayout.JOYSTICK_RIGHT:

                rightCB = sharedUtilsInstance.GetFirstChildrenWithComponent<Image>(rightJoystickBtn);
                leftCB = sharedUtilsInstance.GetFirstChildrenWithComponent<Image>(leftJoystickBtn);
                rightCB.gameObject.SetActive(true);
                leftCB.gameObject.SetActive(false);
                break;
            case SettingsData.ControlsLayout.JOYSTICK_LEFT:

                rightCB = sharedUtilsInstance.GetFirstChildrenWithComponent<Image>(rightJoystickBtn);
                leftCB = sharedUtilsInstance.GetFirstChildrenWithComponent<Image>(leftJoystickBtn);
                rightCB.gameObject.SetActive(false);
                leftCB.gameObject.SetActive(true);
                break;
        }

        int qLevel = QualitySettings.GetQualityLevel();
        switch(qLevel)
        {
            case LOW:
                lowEnabledIcon.gameObject.SetActive(true);
                mediumEnabledIcon.gameObject.SetActive(false);
                highEnabledIcon.gameObject.SetActive(false);
                ultraEnabledIcon.gameObject.SetActive(false);
                break;
            case MEDIUM:
                lowEnabledIcon.gameObject.SetActive(false);
                mediumEnabledIcon.gameObject.SetActive(true);
                highEnabledIcon.gameObject.SetActive(false);
                ultraEnabledIcon.gameObject.SetActive(false);
                break;
            case HIGH:
                lowEnabledIcon.gameObject.SetActive(false);
                mediumEnabledIcon.gameObject.SetActive(false);
                highEnabledIcon.gameObject.SetActive(true);
                ultraEnabledIcon.gameObject.SetActive(false);
                break;
            case ULTRA:
                lowEnabledIcon.gameObject.SetActive(false);
                mediumEnabledIcon.gameObject.SetActive(false);
                highEnabledIcon.gameObject.SetActive(false);
                ultraEnabledIcon.gameObject.SetActive(true);
                break;
        }
    }

    public void SetControlsLayout(string layout)
    {
        SettingsData settingsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SETTINGS_PATH).GetData<SettingsData>();
        switch (layout)
        {
            case "Right":
                settingsData.controlsLayout = SettingsData.ControlsLayout.JOYSTICK_RIGHT;
                break;
            case "Left":
                settingsData.controlsLayout = SettingsData.ControlsLayout.JOYSTICK_LEFT;
                break;
        }
        UpdateControlsLayoutUI(settingsData.controlsLayout);
        SaveManager.GetInstance().SavePersistentData<SettingsData>(settingsData, SaveManager.SETTINGS_PATH);
    }

    public void ToggleAudio()
    {
        settingsData.audioActive = !settingsData.audioActive;
        AudioManager.GetInstance().NotifyAudioSettings(settingsData);
        SaveManager.GetInstance().SavePersistentData(settingsData, SaveManager.SETTINGS_PATH);
        UpdateButton(soundBtn, settingsData.audioActive);
    }

    public void ToggleFPS()
    {
        settingsData.showFPS = !settingsData.showFPS;
        SaveManager.GetInstance().SavePersistentData(settingsData, SaveManager.SETTINGS_PATH);
        UpdateButton(fpsBtn, settingsData.showFPS);
    }

    public void ToogleCameraLookAt()
    {
        settingsData.cameraLookAt = !settingsData.cameraLookAt;
        SaveManager.GetInstance().SavePersistentData(settingsData, SaveManager.SETTINGS_PATH);
        UpdateButton(cameraLookAtBtn, settingsData.cameraLookAt);
    }

    private void UpdateButton(GameObject button, bool newState)
    {
        Image image = SharedUtilities.GetInstance().GetFirstChildrenWithComponent<Image>(button).GetComponent<Image>();
        if(newState)
        {
            image.sprite = enabled;
            image.color = new Color32(0, 235, 255, 210);
        }
        else
        {
            image.sprite = disabled;
            image.color = new Color32(200, 0, 0, 210);
        }
    }

    public void Dropdown_IndexChanged(int index)
    {
        switch(dropdown.options[index].text)
        {
            case "Distance":
                settingsData.hudConf = SettingsData.EndlessModeHUD.DISTANCE;
                break;
            case "Proper time":
                settingsData.hudConf = SettingsData.EndlessModeHUD.TIME;
                break;
            case "Dilated time":
                settingsData.hudConf = SettingsData.EndlessModeHUD.TIME_DILATED;
                break;
            case "Speed":
                settingsData.hudConf = SettingsData.EndlessModeHUD.SPEED;
                break;
            case "Celestial bodies destroyed":
                settingsData.hudConf = SettingsData.EndlessModeHUD.OBSTACLES_DESTROYED;
                break;
            default:
                break;
        }
        SaveManager.GetInstance().SavePersistentData(settingsData, SaveManager.SETTINGS_PATH);
    }
}

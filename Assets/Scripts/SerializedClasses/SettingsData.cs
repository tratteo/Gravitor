[System.Serializable]
public class SettingsData
{
    public enum ControlsLayout { JOYSTICK_RIGHT, JOYSTICK_LEFT };
    public enum EndlessModeHUD { DISTANCE, SPEED, OBSTACLES_DESTROYED, TIME_DILATED, TIME}

    public ControlsLayout controlsLayout;
    public bool audioActive;
    public bool showFPS;
    public bool cameraLookAt;
    public EndlessModeHUD hudConf;
    public int qualityLevel = 2;

    public SettingsData()
    {
        audioActive = true;
        showFPS = false;
        controlsLayout = ControlsLayout.JOYSTICK_RIGHT;
        cameraLookAt = false;
        hudConf = EndlessModeHUD.DISTANCE;
        qualityLevel = 2;
    }
}

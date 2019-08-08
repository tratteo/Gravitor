using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public enum ControlsLayout { JOYSTICK_RIGHT, JOYSTICK_LEFT };

    public ControlsLayout controlsLayout;
    public bool audioActive;
    public bool showFPS;
    public bool cameraLookAt;

    public SettingsData()
    {
        audioActive = true;
        showFPS = false;
        controlsLayout = ControlsLayout.JOYSTICK_RIGHT;
        cameraLookAt = false;
    }
}

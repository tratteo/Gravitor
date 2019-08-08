using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    private HUDManager hudManagerInstance;
    [SerializeField] private GameObject rightControlTutorial;
    [SerializeField] private GameObject leftControlTutorial;
    private GameObject tutorialElements;
    int index;
    GameObject[] elementsArray;

    private void Start()
    {
        hudManagerInstance = HUDManager.GetInstance();

        SettingsData settingsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.SETTINGS_PATH).GetData<SettingsData>();

        if(settingsData.controlsLayout == SettingsData.ControlsLayout.JOYSTICK_RIGHT)
        {
            tutorialElements = rightControlTutorial;
        }
        else if(settingsData.controlsLayout == SettingsData.ControlsLayout.JOYSTICK_LEFT)
        {
            tutorialElements = leftControlTutorial;
        }

        tutorialElements.SetActive(true);

        List<GameObject> objects = new List<GameObject>();
        foreach (Transform child in tutorialElements.transform)
        {
            objects.Add(child.gameObject);
        }
        elementsArray = objects.ToArray();
        index = 0;
    }

    private void Update()
    {
        if (index < elementsArray.Length - 1)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    elementsArray[index].SetActive(false);
                    elementsArray[++index].SetActive(true);
                }
            }
        }
        else if(index == elementsArray.Length - 1)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    elementsArray[index++].SetActive(false);
                    hudManagerInstance.Pause(false);
                    hudManagerInstance.tutorialPanel.SetActive(false);
                    hudManagerInstance.scoreText.gameObject.SetActive(true);
                    hudManagerInstance.timeRelativeText.gameObject.SetActive(true);
                    hudManagerInstance.healthText.gameObject.SetActive(true);
                    this.enabled = false;
                }
            }
        }
    }
}

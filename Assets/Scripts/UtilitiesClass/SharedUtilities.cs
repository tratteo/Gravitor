using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SharedUtilities
{
    private static SharedUtilities instance = null;
    public static SharedUtilities GetInstance()
    {
        if (instance == null)
        {
            instance = new SharedUtilities();
        }

        return instance;
    }
    private SharedUtilities() { }


    public IEnumerator StartSceneWithDelay(int delay, int index)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(index);
    }


    public Type GetFirstComponentInChildrenWithTag<Type>(GameObject parent, string tag)
    {
        Transform[] transforms = parent.GetComponentsInChildren<Transform>();
        for (int i = 1; i < transforms.Length; i++)
        {
            if (transforms[i].tag == tag)
            {
                return transforms[i].gameObject.GetComponent<Type>();
            }
        }
        return default(Type);
    }

    public Type GetFirstComponentInParentWithTag<Type>(GameObject parent, string tag)
    {
        Transform[] transforms = parent.GetComponentsInParent<Transform>();
        for (int i = 1; i < transforms.Length; i++)
        {
            if (transforms[i].tag == tag)
            {
                return transforms[i].gameObject.GetComponent<Type>();
            }
        }
        return default(Type);
    }


    public GameObject GetFirstChildrenWithComponent<Type>(GameObject parent)
    {
        Type parentComponent = parent.GetComponent<Type>(), childComponent;
        foreach (Transform transform in parent.transform)
        {
            childComponent = transform.GetComponent<Type>();
            if (childComponent != null && !childComponent.Equals(parentComponent))
            {
                return transform.gameObject;
            }
        }
        return null;
    }


    public IEnumerator UnfillImage(MonoBehaviour context, Image image, float duration)
    {
        IEnumerator coroutine = UnfillImage_C(image, duration);
        context.StartCoroutine(coroutine);
        return coroutine;
    }
    public IEnumerator UnfillImage<T>(MonoBehaviour context, Image image, float duration, Action<T> funcToCall, T funcParameters)
    {
        IEnumerator coroutine = UnfillImage_C<T>(image, duration, funcToCall, funcParameters);
        context.StartCoroutine(coroutine);
        return coroutine;
    }

    private IEnumerator UnfillImage_C(Image image, float duration)
    {
        image.fillAmount = 1f;
        float stride = Time.fixedDeltaTime / duration;
        while (image.fillAmount - stride > 0)
        {
            image.fillAmount -= stride;
            yield return new WaitForFixedUpdate();
        }
        image.fillAmount = 0f;
    }

    private IEnumerator UnfillImage_C<T>(Image image, float duration, Action<T> funcToCall, T funcParameters)
    {
        image.fillAmount = 1f;
        float stride = Time.fixedDeltaTime / duration;
        while (image.fillAmount - stride > 0)
        {
            yield return new WaitForFixedUpdate();
            image.fillAmount -= stride;
        }
        image.fillAmount = 0f;
        if (funcToCall != null)
        {
            funcToCall(funcParameters);
        }
    }


    public bool IsFirstLaunch()
    {
        PlayerData data = SaveManager.GetInstance().LoadPersistentData(SaveManager.PLAYER_DATA).GetData<PlayerData>();
        if(data.isFirstLaunch == true)
        {
            data.isFirstLaunch = false;
            SaveManager.GetInstance().SavePersistentData<PlayerData>(data, SaveManager.PLAYER_DATA);
            return true;
        }
        return false;
    }

    public void MakeGameObjectVisible(GameObject obj, bool state)
    {
        Transform[] transforms = obj.GetComponentsInChildren<Transform>();
        Renderer renderer;
        int length = transforms.Length;
        for (int i = 0; i < length; i++)
        {
            renderer = transforms[i].gameObject.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.enabled = state;
            }
        }
    }

    public string GetTimeStringFromSeconds(float _seconds)
    {
        int hours = 0;
        int minutes = 0;
        float seconds = 0;
        while (_seconds / 3600 >= 1)
        {
            hours++;
            _seconds -= 3600;
        }
        while (_seconds / 60 >= 1)
        {
            minutes++;
            _seconds -= 60;
        }
        seconds = _seconds;
        return hours.ToString() + "h " + minutes.ToString() + "m " + seconds.ToString("0.0") + "s";
    }


    public void SetQualitySettings(int level)
    {
        switch (level)
        {
            case 0:
                QualitySettings.SetQualityLevel(0);
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 30;
                break;

            case 1:
                QualitySettings.SetQualityLevel(1);
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 40;
                break;

            case 2:
                QualitySettings.SetQualityLevel(2);
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 50;
                break;

            case 3:
                QualitySettings.SetQualityLevel(3);
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 500;
                break;
        }
    }
}

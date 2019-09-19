using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToastScript : MonoBehaviour
{
    private Text text;
    private Image image;
    private Image toastIcon;
    private bool showing = false;
    private bool coroutineRunning = false;

    private float initImageAlpha;
    private float initTextAlpha;
    private Sprite initialIcon;

    private struct ToastStruct
    {
        public Sprite sprite;
        public string message;
        public float duration;
        public ToastStruct(string message, Sprite sprite, float duration)
        {
            this.sprite = sprite;
            this.message = message;
            this.duration = duration;
        }
    }

    Queue<ToastStruct> toastsQueue = null;

    private void Awake()
    {
        text = GetComponentInChildren<Text>();
        image = GetComponent<Image>();
        Image[] images = GetComponentsInChildren<Image>();
        if (images.Length > 1)
        {
            toastIcon = images[1];
        }
        else
        {
            toastIcon = null;
        }
        initImageAlpha = image.canvasRenderer.GetAlpha();
        initTextAlpha = text.canvasRenderer.GetAlpha();
        initialIcon = toastIcon?.sprite;
    }

    private void Start()
    {
        SetVisible(false);
        toastsQueue = new Queue<ToastStruct>();
    }

    private void OnEnable()
    {
        SetVisible(false);
    }

    public void EnqueueToast(string message, Sprite sprite, float duration)
    {
        ToastStruct newToast = new ToastStruct(message, sprite, duration);
        toastsQueue.Enqueue(newToast);
        if (!coroutineRunning)
        {
            StartCoroutine(ToastsQueueCoroutine());
        }
    }
    private IEnumerator ToastsQueueCoroutine()
    {
        coroutineRunning = true;
        while (toastsQueue.Count != 0)
        {
            if (!showing)
            {
                showing = true;
                ToastStruct toast = toastsQueue.Dequeue();
                StartCoroutine(Toast(toast));
            }
            yield return new WaitForEndOfFrame();
        }
        coroutineRunning = false;
    }


    public void ShowToast(string message, Sprite sprite, float duration)
    {
        if (!showing)
        {
            showing = true;
            ToastStruct toast = new ToastStruct(message, sprite, duration);
            StartCoroutine(Toast(toast));
        }
    }

    private IEnumerator Toast(ToastStruct toast)
    {
        text.text = toast.message;
        if (toast.sprite != null && toastIcon != null)
        {
            toastIcon.sprite = toast.sprite;
        }
        StartCoroutine(FadeIn());
        yield return new WaitForSecondsRealtime(toast.duration);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        float alpha = 0;
        image.canvasRenderer.SetAlpha(alpha);
        toastIcon?.canvasRenderer.SetAlpha(alpha);
        text.canvasRenderer.SetAlpha(alpha);
        while (alpha <= 1f)
        {
            image.canvasRenderer.SetAlpha(alpha);
            toastIcon?.canvasRenderer.SetAlpha(alpha);
            text.canvasRenderer.SetAlpha(alpha);
            alpha += 0.1f;
            yield return new WaitForEndOfFrame();
        }
        SetVisible(true);
    }

    private IEnumerator FadeOut()
    {
        float alpha = 1f;
        while (alpha >= 0f)
        {
            image.canvasRenderer.SetAlpha(alpha);
            toastIcon?.canvasRenderer.SetAlpha(alpha);
            text.canvasRenderer.SetAlpha(alpha);
            alpha -= 0.1f;
            yield return new WaitForEndOfFrame();
        }
        SetVisible(false);
        if (toastIcon != null) toastIcon.sprite = initialIcon;
        showing = false;
    }

    private void SetVisible(bool state)
    {
        if (state)
        {
            toastIcon?.canvasRenderer.SetAlpha(initImageAlpha);
            image.canvasRenderer.SetAlpha(initImageAlpha);
            text.canvasRenderer.SetAlpha(initTextAlpha);
        }
        else
        {
            toastIcon?.canvasRenderer.SetAlpha(0);
            image.canvasRenderer.SetAlpha(0);
            text.canvasRenderer.SetAlpha(0);
        }
    }
}

using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ScrollRectButton : Button, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    private ScrollRect parentScroll;
    public EventTrigger mainTrigger;
    
    void Start()
    {
        base.Start();
        parentScroll = GetComponentInParent<ScrollRect>();
        mainTrigger = GetComponent<EventTrigger>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        mainTrigger.enabled = false;
        parentScroll.OnBeginDrag(eventData);
    }


    public void OnDrag(PointerEventData eventData)
    {
        parentScroll.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        mainTrigger.enabled = true;
        parentScroll.OnEndDrag(eventData);
    }


    public void OnScroll(PointerEventData data)
    {
        parentScroll.OnScroll(data);
    }
}

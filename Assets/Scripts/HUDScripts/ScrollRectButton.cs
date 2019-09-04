using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ScrollRectButton : Button, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    private ScrollRect parentScroll;
    
    void Start()
    {
        base.Start();
        parentScroll = GetComponentInParent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentScroll.OnBeginDrag(eventData);
    }


    public void OnDrag(PointerEventData eventData)
    {
        parentScroll.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        parentScroll.OnEndDrag(eventData);
    }


    public void OnScroll(PointerEventData data)
    {
        parentScroll.OnScroll(data);
    }
}

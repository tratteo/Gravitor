using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscoverManager : MonoBehaviour
{
    #region Variables

    [Header("Scroll View")]
    [SerializeField] private GameObject parentScrollView = null;
    [SerializeField] private GameObject obstacleViewport = null;
    [SerializeField] private GameObject pickUpsViewport = null;
    [SerializeField] private GameObject abilitiesViewport = null;

    private ScrollRect parentScrollRect;
    #endregion


    private void Start()
    {
        parentScrollRect = parentScrollView.GetComponent<ScrollRect>();
        parentScrollRect.content = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<RectTransform>(obstacleViewport, "ViewContent");
    }

    public void EnableViewport(string viewPortId)
    {
        switch (viewPortId)
        {
            case "Obstacles":
                obstacleViewport.gameObject.SetActive(true);
                pickUpsViewport.gameObject.SetActive(false);
                abilitiesViewport.gameObject.SetActive(false);
                parentScrollRect.content = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<RectTransform>(obstacleViewport, "ViewContent");
                break;

            case "Pickups":
                obstacleViewport.gameObject.SetActive(false);
                pickUpsViewport.gameObject.SetActive(true);
                abilitiesViewport.gameObject.SetActive(false);
                parentScrollRect.content = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<RectTransform>(pickUpsViewport, "ViewContent");
                break;

            case "Abilities":
                obstacleViewport.gameObject.SetActive(false);
                pickUpsViewport.gameObject.SetActive(false);
                abilitiesViewport.gameObject.SetActive(true);
                parentScrollRect.content = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<RectTransform>(abilitiesViewport, "ViewContent");
                break;
        }
    }
}

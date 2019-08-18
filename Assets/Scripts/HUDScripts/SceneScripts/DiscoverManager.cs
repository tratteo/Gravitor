using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscoverManager : MonoBehaviour
{
    #region Variables

    private Color32 SELECTED_COLOR = new Color32(0, 225, 255, 225);
    private Color32 DESELECTED_COLOR = new Color32(225, 225, 225, 255); 

    [Header("Scroll View")]
    [SerializeField] private GameObject parentScrollView = null;
    [SerializeField] private GameObject obstacleViewport = null;
    [SerializeField] private GameObject pickUpsViewport = null;
    [SerializeField] private GameObject abilitiesViewport = null;
    [Header("Buttons")]
    [SerializeField] private Image obstaclesBtn = null;
    [SerializeField] private Image extrasBtn = null;
    [SerializeField] private Image abilitiesBtn = null;

    private ScrollRect parentScrollRect;
    #endregion


    private void Start()
    {
        parentScrollRect = parentScrollView.GetComponent<ScrollRect>();
        parentScrollRect.content = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<RectTransform>(obstacleViewport, "ViewContent");
        EnableViewport("Obstacles");
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
                obstaclesBtn.color = SELECTED_COLOR;
                extrasBtn.color = DESELECTED_COLOR;
                abilitiesBtn.color = DESELECTED_COLOR;
                break;

            case "Extras":
                obstacleViewport.gameObject.SetActive(false);
                pickUpsViewport.gameObject.SetActive(true);
                abilitiesViewport.gameObject.SetActive(false);
                parentScrollRect.content = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<RectTransform>(pickUpsViewport, "ViewContent");
                obstaclesBtn.color = DESELECTED_COLOR;
                extrasBtn.color = SELECTED_COLOR;
                abilitiesBtn.color = DESELECTED_COLOR;
                break;

            case "Abilities":
                obstacleViewport.gameObject.SetActive(false);
                pickUpsViewport.gameObject.SetActive(false);
                abilitiesViewport.gameObject.SetActive(true);
                parentScrollRect.content = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<RectTransform>(abilitiesViewport, "ViewContent");
                obstaclesBtn.color = DESELECTED_COLOR;
                extrasBtn.color = DESELECTED_COLOR;
                abilitiesBtn.color = SELECTED_COLOR;
                break;
        }
    }
}

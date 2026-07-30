using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


public class PortraitHoverInfo : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{


    [TextArea]
    public string infoText;



    public GameObject hoverPanel;

    public TMP_Text hoverText;




    public void OnPointerEnter(
        PointerEventData eventData)
    {

        if (hoverPanel != null)
        {
            hoverPanel.SetActive(true);
        }


        if (hoverText != null)
        {
            hoverText.text = infoText;
        }

    }





    public void OnPointerExit(
        PointerEventData eventData)
    {

        if (hoverPanel != null)
        {
            hoverPanel.SetActive(false);
        }

    }

}
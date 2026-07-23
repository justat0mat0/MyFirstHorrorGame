using UnityEngine;


public class BoardOpenButton : MonoBehaviour
{

    public EvidenceBoardController controller;


    private void OnMouseDown()
    {
        controller.OpenBoard();
    }

}
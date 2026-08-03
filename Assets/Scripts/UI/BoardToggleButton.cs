using UnityEngine;

public class BoardToggleButton : MonoBehaviour
{

    public GameObject smallBoard;
    public GameObject largeBoard;


    private bool isOpen = false;



    private void Start()
    {
        if (largeBoard != null)
            largeBoard.SetActive(false);
    }



    private void OnMouseDown()
    {

        if (isOpen)
        {
            CloseBoard();
        }
        else
        {
            OpenBoard();
        }

    }



    private void OpenBoard()
    {

        isOpen = true;


        if (smallBoard != null)
            smallBoard.SetActive(false);


        if (largeBoard != null)
            largeBoard.SetActive(true);

    }



    private void CloseBoard()
    {

        isOpen = false;


        if (largeBoard != null)
            largeBoard.SetActive(false);


        if (smallBoard != null)
            smallBoard.SetActive(true);

    }

}
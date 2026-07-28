using System.Collections;
using TMPro;
using UnityEngine;


public class VerificationTextController : MonoBehaviour
{

    public TMP_Text text;


    [TextArea]
    public string message;


    public float typingSpeed = 0.05f;



    public void StartTyping()
    {

        StartCoroutine(TypeText());

    }



    private IEnumerator TypeText()
    {

        text.text = "";


        foreach (char c in message)
        {

            text.text += c;


            yield return new WaitForSeconds(typingSpeed);

        }

    }

}
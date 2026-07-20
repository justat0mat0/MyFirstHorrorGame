using UnityEngine;

public class Stage1AudioController : MonoBehaviour
{
    void Start()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopAmbient();
        }
    }
}
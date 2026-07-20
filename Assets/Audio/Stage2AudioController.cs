using UnityEngine;

public class Stage2AudioController : MonoBehaviour
{
    public AudioClip stage2Ambient;


    public void EnterStage2()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayAmbient(stage2Ambient);


            AudioManager.instance.SetBGMVolume(0.7f);
            AudioManager.instance.SetAmbientVolume(0.3f);
        }
    }
}
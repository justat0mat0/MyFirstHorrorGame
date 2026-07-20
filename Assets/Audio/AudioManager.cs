using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;


    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource ambientSource;
    public AudioSource sfxSource;



    [Header("BGM")]
    public AudioClip mainMenuBGM;

    [Range(0f, 1f)]
    public float defaultBGMVolume = 1f;



    [Header("Ambient")]
    [Range(0f, 1f)]
    public float defaultAmbientVolume = 0f;



    [Header("SFX")]
    [Range(0f, 1f)]
    public float defaultSFXVolume = 1f;



    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }


        instance = this;

        DontDestroyOnLoad(gameObject);



        // 初始化音量

        if (bgmSource != null)
        {
            bgmSource.volume = defaultBGMVolume;
        }


        if (ambientSource != null)
        {
            ambientSource.volume = defaultAmbientVolume;

            ambientSource.Stop();
            ambientSource.clip = null;
        }


        if (sfxSource != null)
        {
            sfxSource.volume = defaultSFXVolume;
        }



        // 播放主菜单BGM

        if (bgmSource != null && mainMenuBGM != null)
        {
            bgmSource.clip = mainMenuBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }






    // =========================
    // Ambient 控制
    // =========================


    public void PlayAmbient(AudioClip clip)
    {
        if (ambientSource == null)
            return;


        if (ambientSource.clip == clip && ambientSource.isPlaying)
            return;


        ambientSource.Stop();


        if (clip != null)
        {
            ambientSource.clip = clip;
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }



    public void StopAmbient()
    {
        if (ambientSource == null)
            return;


        ambientSource.Stop();
        ambientSource.clip = null;
    }







    // =========================
    // SFX 控制
    // =========================


    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;


        sfxSource.PlayOneShot(clip);
    }







    // =========================
    // BGM控制
    // =========================


    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }



    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null)
            return;


        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void EnsureBGM()
    {
        if (bgmSource != null && !bgmSource.isPlaying)
        {
            if (bgmSource.clip != null)
            {
                bgmSource.Play();
            }
            else if (mainMenuBGM != null)
            {
                bgmSource.clip = mainMenuBGM;
                bgmSource.loop = true;
                bgmSource.Play();
            }
        }
    }





    // =========================
    // Volume Control
    // =========================


    public void SetBGMVolume(float value)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = value;
        }
    }



    public void SetAmbientVolume(float value)
    {
        if (ambientSource != null)
        {
            ambientSource.volume = value;
        }
    }



    public void SetSFXVolume(float value)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = value;
        }
    }
}
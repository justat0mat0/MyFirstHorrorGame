using Cinemachine;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{

    public static CameraController Instance;


    public List<GameObject> allScenes;


    [SerializeField]
    private List<CinemachineVirtualCamera> allCameras;



    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }



    private void Start()
    {

        allScenes.Clear();
        allCameras.Clear();



        foreach (var obj in GameObject.FindGameObjectsWithTag("Resteround"))
        {

            Transform camPos =
                obj.transform.Find("CamPos");


            if (camPos == null)
            {
                continue;
            }


            CinemachineVirtualCamera cam =
                camPos.GetComponentInChildren<CinemachineVirtualCamera>();


            if (cam != null)
            {
                allScenes.Add(obj);
                allCameras.Add(cam);
            }

        }

    }




    public void SwitchToCamera(CinemachineVirtualCamera target)
    {

        foreach (var cam in allCameras)
        {

            if (cam != null)
            {
                cam.Priority = 0;
            }

        }



        if (target == null)
        {
            return;
        }



        target.Priority = 10;

    }

}
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public List<GameObject> allScenes;

    [SerializeField] private List<CinemachineVirtualCamera> allCameras;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach(var obj in GameObject.FindGameObjectsWithTag("Resteround"))
        {
            allScenes.Add(obj);
            allCameras.Add(obj.transform.Find("CamPos").GetComponentInChildren<CinemachineVirtualCamera>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SwitchToCamera(CinemachineVirtualCamera target)
    {
        foreach (var cam in allCameras)
        {
            cam.Priority = 0;          // 禁用所有相机
        }
        target.Priority = 10;           // 激活目标相机
    }
}

using UnityEngine;

public class GlobalUpdate : MonoBehaviour
{
    public CameraWASD cameraWASD;
    public ClickHandler clickHandler;

    void Start()
    {
        if (cameraWASD == null)
        {
            cameraWASD = FindFirstObjectByType<CameraWASD>();
        }
    }


    void Update()
    {
        cameraWASD.CameraInput();
    }
}

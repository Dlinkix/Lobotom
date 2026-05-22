using UnityEngine;
using UnityEngine.InputSystem;

public class CameraWASD : MonoBehaviour
{
    public Camera Camera;
    public float cameraSpeed = 0.5f;

    public void CameraInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(moveX, moveY) * cameraSpeed; 

        Camera.transform.Translate(move);
    }
}

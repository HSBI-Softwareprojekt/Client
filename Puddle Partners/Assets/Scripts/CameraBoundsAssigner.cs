using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraBoundsAssigner : MonoBehaviour
{
    public GameObject player; // Reference to the player prefab
    public string virtualCameraName = "Virtual Camera"; // Name of the virtual camera within the player prefab

    private CinemachineConfiner confiner;

    void Start()
    {
        // Find the virtual camera within the player prefab
        Transform virtualCameraTransform = player.transform.Find(virtualCameraName);

        if (virtualCameraTransform == null)
        {
            Debug.LogError("Virtual Camera not found in player prefab");
            return;
        }

        CinemachineVirtualCamera virtualCamera = virtualCameraTransform.GetComponent<CinemachineVirtualCamera>();

        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineVirtualCamera component not found on the virtual camera");
            return;
        }

        // Get the CinemachineConfiner component from the virtual camera
        confiner = virtualCamera.GetComponent<CinemachineConfiner>();

        // Find and set the bounds for the current level
        SetBounds();

        DontDestroyOnLoad(this.gameObject);
    }

    // Function to set the bounds for the current level
    public void SetBounds()
    {
        // Find the CamBounds GameObject in the scene
        GameObject camBounds = GameObject.Find("CamBounds");

        if (camBounds == null)
        {
            Debug.LogError("CamBounds GameObject not found in the scene");
            return;
        }

        // Get the PolygonCollider2D component from the CamBounds GameObject
        PolygonCollider2D polygonCollider = camBounds.GetComponent<PolygonCollider2D>();

        if (polygonCollider == null)
        {
            Debug.LogError("PolygonCollider2D component not found on CamBounds GameObject");
            return;
        }

        // Assign the PolygonCollider2D to the CinemachineConfiner
        confiner.m_BoundingShape2D = polygonCollider;

        // Call InvalidatePathCache to refresh the confiner's path cache
        confiner.InvalidatePathCache();
    }
}
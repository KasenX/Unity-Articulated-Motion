using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject lampJr;
    public GameObject lampSr;

    public float defaultOrthographicSize = 5; // Default camera size (no zoom-in smaller than this)
    public float fixedCameraBottomY = -2; // The fixed Y value for the camera's bottom

    // Update is called once per frame
    void Update()
    {
        AdjustCamera();
    }

    // Adjust camera view so that objects remain on-screen
    private void AdjustCamera()
    {
        // Get the root limbs of both objects
        Limb rootLimb1 = lampJr.GetComponent<LampJr>().rootObject.GetComponent<Limb>();
        Limb rootLimb2 = lampSr.GetComponent<LampSr>().rootObject.GetComponent<Limb>();

        // Find the x distance between both objects
        float limbsDistance = Mathf.Abs(rootLimb1.position.x - rootLimb2.position.x);

        // Find the x midpoint between the two objects
        float midpoint = (rootLimb1.position.x + rootLimb2.position.x) / 2;

        // Set the camera's position to the midpoint
        Vector3 position = Camera.main.transform.position;
        position.x = midpoint;

        // Get the aspect ratio of the camera view
        float aspectRatio = Camera.main.aspect;

        // Calculate the minimum orthographic size to fit both lamps
        float padding = 0.4f; // 40% padding
        float cameraWidth = limbsDistance / (1.0f - padding);
        float cameraHeight = cameraWidth / aspectRatio;
        float requiredOrthographicSize = cameraHeight / 2.0f;

        // Keep the camera at the default size unless the lamps are too far apart
        float orthographicSize = Mathf.Max(defaultOrthographicSize, requiredOrthographicSize);
        
        // Adjust the camera orthographic size
        Camera.main.orthographicSize = orthographicSize;

        // Keep the bottom of the camera at the same fixed level
        float newYPosition = fixedCameraBottomY + orthographicSize;

        // Set the camera's position to the new midpoint and adjusted Y position
        position.y = newYPosition;
        Camera.main.transform.position = position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOVController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Calculates the required FOV of the camera for maximum resolution without clipping out the world screen
    float CalculateMinimumViableFOV()
    {
        // First get the positions of the following corners of the screen
        GameObject screen = GameObject.Find("PerspectiveScreen");

        // Using dot product, calculate the two diagonal angles from the camera's perspective and
        //  then return the minimum of the two
        Vector3 top_left = screen.transform.TransformPoint(new Vector3(0.5f, 0.5f, 0.5f));
        Vector3 top_right = screen.transform.TransformPoint(new Vector3(-0.5f, 0.5f, 0.5f));
        Vector3 bottom_left = screen.transform.TransformPoint(new Vector3(0.5f, -0.5f, 0.5f));
        Vector3 bottom_right = screen.transform.TransformPoint(new Vector3(-0.5f, -0.5f, 0.5f));
        Vector3 cam_to_topleft = top_left - transform.position;
        Vector3 cam_to_topright = top_right - transform.position;
        Vector3 cam_to_bottomleft = bottom_left - transform.position;
        Vector3 cam_to_bottomright = bottom_right - transform.position;

        // Dot product between the camera->corner vectors
        float left_diagonal_dot = Vector3.Dot(cam_to_topleft, cam_to_bottomright);
        float right_diagonal_dot = Vector3.Dot(cam_to_topright, cam_to_bottomleft);

        // Calculate angle in radians
        float left_diagonal_angle = Mathf.Acos(left_diagonal_dot / (cam_to_topleft.magnitude * cam_to_bottomright.magnitude));
        float right_diagonal_angle = Mathf.Acos(right_diagonal_dot / (cam_to_topright.magnitude * cam_to_bottomleft.magnitude));

        // Convert to degrees
        left_diagonal_angle = left_diagonal_angle * (180.0f / Mathf.PI);
        right_diagonal_angle = right_diagonal_angle * (180.0f / Mathf.PI);

        return Mathf.Max(left_diagonal_angle, right_diagonal_angle) + 0.2f * Mathf.Max(left_diagonal_angle, right_diagonal_angle);
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.fieldOfView = CalculateMinimumViableFOV();
    }
}

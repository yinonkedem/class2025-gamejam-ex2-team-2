
using System.Collections.Generic;
using UnityEngine;

public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> targets; // List of target objects
    public Vector3 offset; // Camera offset
    private Vector3 velocity; // Used for smooth damping
    public float smoothTime = 0.5f; // Smooth damping time
    public float minZoom = 5f; // Minimum orthographic size
    public float maxZoom = 10f; // Maximum orthographic size
    public float zoomLimiter = 50f; // Adjusts zoom based on target distance

    private Camera cam; // Reference to the camera

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true; // Set camera to orthographic for 2D
    }

    private void LateUpdate()
    {
        if (targets.Count == 0)
        {
            return;
        }

        Move();
        Zoom();
    }

    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    // private void Zoom()
    // {
    //     float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
    //     cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    // }

    
    private void Zoom()
    {
        float greatestDistance = GetGreatestDistance();
        float newZoom = Mathf.Lerp(minZoom, maxZoom, greatestDistance / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    
    private float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.x > bounds.size.y ? bounds.size.x : bounds.size.y;
    }

    private Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }
}

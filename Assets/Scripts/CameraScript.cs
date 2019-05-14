using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    public Transform target;

    public float distance = -10f;
    public float height = 0f;
    public float damping = 5.0f;


    void Start()
    {

    }

    void FixedUpdate()
    {

        Vector3 targetPosition = target.TransformPoint(0, height, distance);
        transform.position = Vector3.Lerp(transform.position, targetPosition, (Time.deltaTime * damping));
    }
}

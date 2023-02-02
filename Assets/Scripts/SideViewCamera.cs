using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideViewCamera : MonoBehaviour
{
    public Camera mainCamera;
    public Transform cameraArm;
    public Transform sideViewCamera;


    void Update()
    {
        Vector3 direction = (cameraArm.position - mainCamera.transform.position).normalized;
        direction.y = 0;

        Quaternion a = Quaternion.LookRotation(direction);
        Quaternion b = a * Quaternion.AngleAxis(90, Vector3.up);

        Vector3 _targetPos = cameraArm.position + b * Vector3.forward * 1.0f + Vector3.up * .25f;

        Quaternion c = a * Quaternion.AngleAxis(-90, Vector3.up);

        sideViewCamera.position = _targetPos;
        sideViewCamera.rotation = c;
    }
}

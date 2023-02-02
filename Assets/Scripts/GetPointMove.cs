using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPointMove : MonoBehaviour
{
    private Transform ChildBall;

    public float yawSpeed = 5f;
    public float rollSpeed = 5f;
    public float pitchSpeed = 5f;

    public float sinY;
    public float upDownSpeed = 0;
    public float freq = 0;

    Vector3 rotateYaw;
    Vector3 rotateRoll;
    Vector3 rotatePitch;

    private void Awake()
    {
        ChildBall = transform.GetChild(1).transform;
        rotateYaw = new Vector3(0, ChildBall.transform.position.y, 0);
        rotateRoll = new Vector3(0, 0, ChildBall.transform.position.z);
        rotatePitch = new Vector3(ChildBall.transform.position.x, 0, 0);
    }

    float length = 0;
    private void FixedUpdate()
    {
        ChildBall.Rotate(rotateYaw, yawSpeed);
        ChildBall.Rotate(rotateRoll, rollSpeed);
        ChildBall.Rotate(rotatePitch, pitchSpeed);

        length += freq;

        sinY = Mathf.Sin(length);

        gameObject.transform.position = new Vector3(transform.localPosition.x, transform.localPosition.y + sinY * upDownSpeed, transform.localPosition.z);
    }

}

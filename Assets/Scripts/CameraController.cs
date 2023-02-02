using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �ִ� ī�޶� ȸ�� �� �� ��� �� ������ ī�޶� ���� �̿��Ͽ� ����
/// </summary>

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform dominoMakerBody;

    [SerializeField] Transform cameraArm;

    [Header("�¿�ȸ�� �ӵ�")]
    [SerializeField] float rotationSpeedHorizontal = 1f;      // �¿� ȸ���� �ӵ�

    [Header("����ȸ�� �ӵ�")]
    [SerializeField] float rotationSpeedVertical = 1f;      // ���� ȸ���� �ӵ�

    [Header("���ξƿ� �ӵ�")]
    [SerializeField] float zoomSpeed = 1f;
    [Header("���ξƿ� �ִ� �ּ�")]
    [SerializeField] float zoomIn = 30f;
    [SerializeField] float zoomOut = 80f;

    [Header("ī�޶��")]
    [SerializeField]
    Camera[] cameras;

    public Transform sideViewCamera;

    private void Update()
    {
        CameraCharacterMove();
        Zoom();
        FollowingCameraArm();

        // ���� ī�޶�

        //Vector3 angle = cameraArm.rotation.eulerAngles;
        ////angle.y = 0;
        //angle.Normalize();
        //Quaternion lookRotation = Quaternion.LookRotation(angle);
        //Quaternion temp = lookRotation * Quaternion.AngleAxis(90, Vector3.up);


        //Vector3 nextPos = cameraArm.transform.position + temp * Vector3.right * 2f;
        //sideViewCamera.position = nextPos;
    }

    private void CameraChange()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            bool isActive = cameras[0].gameObject.activeSelf;
            if (isActive)
                cameras[0].gameObject.SetActive(false);
            else
                cameras[0].gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            bool isActive = cameras[1].gameObject.activeSelf;
            if (isActive)
                cameras[1].gameObject.SetActive(false);
            else
                cameras[1].gameObject.SetActive(true);
        }

    }

    private void FollowingCameraArm()
    {
        cameraArm.transform.position = dominoMakerBody.transform.position; // ī�޶���� �׻� ĳ���͸� ����ٴѴ�;
    }

    private void Zoom()
    {
        var scroll = Input.mouseScrollDelta;
        scroll *= zoomSpeed;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - scroll.y, zoomIn, zoomOut);
    }

    private void CameraCharacterMove()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        Vector3 cameraAngle = cameraArm.rotation.eulerAngles; // ī�޶���� �����̼ǰ��� ���Ϸ������� �޾Ƶд�.

        mouseDelta.x *= rotationSpeedHorizontal;
        mouseDelta.y *= rotationSpeedVertical;

        float xValue = cameraAngle.x - mouseDelta.y; // ���� ȸ����
        float yValue = cameraAngle.y + mouseDelta.x; // �¿� ȸ����

        if (xValue < 180f) xValue = Mathf.Clamp(xValue, -1f, 75f);
        else xValue = Mathf.Clamp(xValue, 300f, 361);

        dominoMakerBody.rotation = Quaternion.Euler(dominoMakerBody.eulerAngles.x, yValue, dominoMakerBody.eulerAngles.z); // �¿��̵�

        cameraArm.rotation = Quaternion.Euler(xValue, dominoMakerBody.rotation.eulerAngles.y, cameraAngle.z); // �����̵�
    }
}

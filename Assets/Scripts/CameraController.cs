using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 기존에 있던 카메라 회전 및 줌 폐기 후 새로이 카메라 암을 이용하여 정리
/// </summary>

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform dominoMakerBody;

    [SerializeField] Transform cameraArm;

    [Header("좌우회전 속도")]
    [SerializeField] float rotationSpeedHorizontal = 1f;      // 좌우 회전시 속도

    [Header("상하회전 속도")]
    [SerializeField] float rotationSpeedVertical = 1f;      // 상하 회전시 속도

    [Header("줌인아웃 속도")]
    [SerializeField] float zoomSpeed = 1f;
    [Header("줌인아웃 최대 최소")]
    [SerializeField] float zoomIn = 30f;
    [SerializeField] float zoomOut = 80f;

    [Header("카메라들")]
    [SerializeField]
    Camera[] cameras;

    public Transform sideViewCamera;

    private void Update()
    {
        CameraCharacterMove();
        Zoom();
        FollowingCameraArm();

        // 옆면 카메라

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
        cameraArm.transform.position = dominoMakerBody.transform.position; // 카메라암은 항상 캐릭터를 따라다닌다;
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

        Vector3 cameraAngle = cameraArm.rotation.eulerAngles; // 카메라암의 로테이션값을 오일러각으로 받아둔다.

        mouseDelta.x *= rotationSpeedHorizontal;
        mouseDelta.y *= rotationSpeedVertical;

        float xValue = cameraAngle.x - mouseDelta.y; // 상하 회전용
        float yValue = cameraAngle.y + mouseDelta.x; // 좌우 회전용

        if (xValue < 180f) xValue = Mathf.Clamp(xValue, -1f, 75f);
        else xValue = Mathf.Clamp(xValue, 300f, 361);

        dominoMakerBody.rotation = Quaternion.Euler(dominoMakerBody.eulerAngles.x, yValue, dominoMakerBody.eulerAngles.z); // 좌우이동

        cameraArm.rotation = Quaternion.Euler(xValue, dominoMakerBody.rotation.eulerAngles.y, cameraAngle.z); // 상하이동
    }
}

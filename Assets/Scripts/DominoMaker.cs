using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  �������� �÷��̾� �̵��ϰ� ���̳븦 �����Ѵ�.
/// </summary>

public class DominoMaker : MonoBehaviour
{
    static public GameObject dominosParent;

    [Header("shift������ �ӵ�")]
    [SerializeField] protected float speedUp = 1f;

    [Header("�⺻�̵� �ӵ�")]
    [SerializeField] protected float moveSpeed = 2.2f;

    [Header("����ϰ� �ӵ�")]
    [SerializeField] protected float translateSpeed = 5.0f;     // ����ϰ��� �ӵ�

    float rotationSpeedHorizontal = 1f;      // �¿� ȸ���� �ӵ�
    float rotationSpeedVertical = 1f;
    float zoomSpeed = 1f;

    protected Renderer makerRenderer;
    protected BoxCollider makerCollider;

    [Header("�ε������� �ٲ� ���͸���")]
    public Material[] hitenMaterial;
    protected Material[] originalMaterial;

    [Header("���̳�Ž���")]
    public Mesh[] dominoMeshs;
    public MeshFilter MakerMeshFilter;

    [Header("���̳���͸����")]
    protected Material[] dominosMaterial;

    [Header("ī�޶��� �⺻��")]
    [SerializeField] Vector3 cameraPos = new Vector3(0, 0.5f, -0.85f);
    [SerializeField] Vector3 cameraRot = new Vector3(15, 0, 0);
    [SerializeField] GameObject cameraArm;

    Camera cam;

    protected bool isHited = false;
    public bool isInportal = true; // ���� �����ϴ� ��Ż�� ���� ������ ������ ������ �Ǵ�
    bool rayHit = false; // ���̿� �Ҹ�
    protected int selectIndex = 0;

    protected GameObject destroyTarget;

    DominoCase dominoCaseComponent; // ���̽��� ����

    /// �׸���
    RaycastHit hit;
    protected GameObject makerShadow;

    Transform[] rayPoint = new Transform[6];

    private void Awake()
    {
        makerRenderer = gameObject.GetComponent<MeshRenderer>();
        makerCollider = gameObject.GetComponent<BoxCollider>();

        originalMaterial = makerRenderer.materials;
        cam = Camera.main;

        makerShadow = this.transform.GetChild(1).gameObject;

        MakerMeshFilter = gameObject.GetComponent<MeshFilter>();

        SoundMG.Instance.SetBgmPlayer();
    }

    private void Start()
    {
        dominosParent = new GameObject("CreatedDominos");

        dominoCaseComponent = GameObject.Find("Domino Case").GetComponent<DominoCase>();

        JsonSerialize.dominoMaker = this;

        SoundMG.Instance.PlayBGM();

        //GameManager.Instance.dInit();
        UIMG.Instance.SpeedUI.text = 1.0f.ToString();
    }
    virtual public void Update()
    {
        CreateDomino();
    }

    virtual public void FixedUpdate()
    {
        MoveDominoMaker();

        RayShadow();
    }

    private void RaycastTrick()
    {
        float higest = 0;

        foreach (var rays in rayPoint)
        {
            Physics.Raycast(rays.position, (Vector3.down), out hit);

            float posY = hit.point.y;

            if (higest < posY)
            {
                higest = posY;
            }
            Debug.DrawRay(rays.position, Vector3.down * 100f, Color.cyan);
        }

        Vector3 _ghostPos = makerShadow.transform.position;
        _ghostPos.y = higest;
        makerShadow.transform.position = _ghostPos;
    }

    protected void RayShadow()
    {
        //Debug.DrawRay(this.transform.position, Vector3.down);
        //if (Physics.BoxCast(ghostCollider.transform.position, ghostCollider.size / 2, Vector3.down, out hit))
        var colSize = makerCollider.size;
        colSize.x *= 0.5f;
        colSize.y = 0.001f;
        colSize.z *= 0.5f;

        rayHit = Physics.BoxCast(this.transform.position, colSize, Vector3.down, out hit, this.transform.rotation);

        if (rayHit)
        {
            Vector3 _ghostPos = makerShadow.transform.position;
            _ghostPos.y = hit.point.y;
            makerShadow.transform.position = _ghostPos;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        //var colSize = makerCollider.size;
        if (rayHit)
        {
            Gizmos.DrawRay(transform.position, transform.up * -1 * hit.distance);

            //Gizmos.DrawWireCube(transform.position + transform.up * -1 * hit.distance, colSize);
        }
        else
        {
            Gizmos.color = Color.red;
        }
    }
    public void MoveDominoMaker()
    {
        #region WASD �̵�
        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.LeftShift)) { speedUp = 3f; }
            else { speedUp = 1f; }
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * speedUp); // ĳ������ �̵�

            /// ����ī�޶��� �����̼��� X���� 0���� ���� �� forward�������� �̵�

            //             Transform oriRota = cam.transform;
            //             oriRota.eulerAngles = new Vector3(0, oriRota.eulerAngles.y, oriRota.eulerAngles.z);
            // 
            //cam.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * speedUp, cam.transform);
            //cam.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * speedUp, transform);

            /// (ī�޶� Ŭ������� �����ϰ�)
            /// Following ī�޶��� Transform = ����ٴϱ⸦ ���ϴ� ������Ʈ�� Transform + ����ٴϸ鼭 ���� ������ Transform
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.LeftShift)) { speedUp = 3f; }
            else { speedUp = 1f; }
            transform.Translate(Vector3.forward * -moveSpeed * Time.deltaTime * speedUp);
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.LeftShift)) { speedUp = 3f; }
            else { speedUp = 1f; }
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime * speedUp);
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.LeftShift)) { speedUp = 3f; }
            else { speedUp = 1f; }
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * speedUp);
        }

        #endregion

        #region ����ϰ�
        if (Input.GetKey(KeyCode.E))
        {
            if (Input.GetKey(KeyCode.LeftShift)) { speedUp = 3f; }
            else { speedUp = 1f; }
            transform.Translate(Vector3.up * translateSpeed * Time.deltaTime * speedUp);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            if (Input.GetKey(KeyCode.LeftShift)) { speedUp = 3f; }
            else { speedUp = 1f; }
            transform.Translate(Vector3.down * translateSpeed * Time.deltaTime * speedUp);
        }
        #endregion

        //RotateMakerLeftRight(); // �¿� ī�޶�� ����Ŀ ���� ȸ����
        //RotateCameraUpDownThirdPerson();

        //RotateCameraUpDown(); // ���� ī�޶� ȸ���� ��������

        //Zoom(); // �ܱ��

        #region ���� ���� ����ϴ� ȸ��
        //mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        //transform.Rotate(0, mouseDelta.x, 0);
        //Camera.main.transform.Rotate(-mouseDelta.y, 0, 0);

        //         #region �¿� ȸ�� (Ű����) // ���콺�� �ٲپ �Ⱦ�
        //         if (Input.GetKey(KeyCode.A))
        //         {
        //             transform.Rotate(Vector3.up * -RotationSpeed * Time.deltaTime);
        //         }
        // 
        //         if (Input.GetKey(KeyCode.D))
        //         {
        //             transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
        //         }
        //         #endregion
        #endregion
    }

    protected void ChangeSelectIndex()
    {
        selectIndex++;

        if ((int)eDominoType.InPortal < selectIndex) selectIndex = (int)eDominoType.Default; // 4�ʰ��� 0����

        ChangeMakerMesh(selectIndex);
    }
    private void ChangeMakerMesh(int _selectIndex)
    {
        MakerMeshFilter.mesh = dominoMeshs[_selectIndex];
    }
    private void ChangeMakerMaterialLong()
    {
        Material[] tempM = makerRenderer.materials;

        tempM[0] = dominosMaterial[2];
        tempM[1] = dominosMaterial[2];
        tempM[2] = dominosMaterial[2];

        makerRenderer.materials = tempM;
    }
    private void ChangeMakerMaterialOriginal()
    {
        makerRenderer.materials = originalMaterial;
    }

    private void ChangeMakerMaterial(int _selectIndex)
    {
        Material[] tempM = makerRenderer.materials;

        if (_selectIndex == (int)eDominoType.InPortal)
        {
            tempM[0] = dominosMaterial[_selectIndex];
            tempM[1] = dominosMaterial[_selectIndex];
            tempM[2] = dominosMaterial[_selectIndex];
        }
        else
        {
            tempM[0] = dominosMaterial[_selectIndex];
            tempM[1] = dominosMaterial[_selectIndex];
            tempM[2] = dominosMaterial[_selectIndex];
        }

        originalMaterial = tempM;

        makerRenderer.materials = originalMaterial;

    }

    #region ��������ϴ� ī�޶�ȸ�� �� ��
    private void RotateMakerLeftRight()
    {
        if (Input.GetAxis("Mouse X") != 0)
        {
            Vector3 rot = transform.rotation.eulerAngles; // ����Ŀ ȸ���� Vector3�� ��ȯ

            float rotaSpeed = Input.GetAxis("Mouse X") * rotationSpeedHorizontal;

            rot.y += rotaSpeed; // ���콺 X ��ġ * ȸ�� ���ǵ�
                                //rot.x += -1 * Input.GetAxis("Mouse Y") * rotateSpeed; // ���콺 Y ��ġ * ȸ�� ���ǵ�
            Quaternion q = Quaternion.Euler(rot); // Quaternion���� ��ȯ
            q.z = 0;

            transform.rotation = Quaternion.Slerp(transform.rotation, q, 2f); // // �������Ѽ� �ڿ������� ȸ��
                                                                              //Camera.main.transform.rotation = this.transform.rotation;
                                                                              //Camera.main.transform.RotateAround(this.transform.position, Vector3.up, rotaSpeed);

            cam.transform.RotateAround(this.transform.position, Vector3.up, rotaSpeed);
            //         Vector3 camPos = cam.transform.position;
            //         cam.transform.position = new Vector3(camPos.x, camPos.y, this.transform.position.z - cameraDistance);
        }
    }

    private void RotateCameraUpDown()
    {
        Vector3 rot = Camera.main.transform.rotation.eulerAngles; // ����Ŀ ȸ���� Vector3�� ��ȯ
                                                                  //rot.y += Input.GetAxis("Mouse X") * rotateSpeed; // ���콺 X ��ġ * ȸ�� ���ǵ�
        rot.x += -1 * Input.GetAxis("Mouse Y") * rotationSpeedVertical; // ���콺 Y ��ġ * ȸ�� ���ǵ�
        Quaternion q = Quaternion.Euler(rot); // Quaternion���� ��ȯ
                                              //transform.rotation = Quaternion.Slerp(transform.rotation, q, 2f); // // �������Ѽ� �ڿ������� ȸ��
        Camera.main.transform.rotation = Quaternion.Slerp(transform.rotation, q, 2f);
    }

    private void RotateCameraUpDownThirdPerson()
    {
        if (Input.GetAxis("Mouse Y") != 0)
        {
            float nowCamRotaX = cam.transform.rotation.eulerAngles.x;

            Debug.Log(cam.transform.rotation.eulerAngles.x);
            float rotaSpeed = Input.GetAxis("Mouse Y") * rotationSpeedVertical;

            //Vector3 v3 = transform.InverseTransformDirection(transform.position);
            //Vector3 v3 = transform.TransformDirection(transform.position);
            //this.transform.TransformDirection(transform.position);
            Vector3 v3 = transform.localPosition;
            v3.y = 0;
            v3.z = 0;

            if (75 < nowCamRotaX && nowCamRotaX < 285)
            {
                Vector3 camEulerAngle = cam.transform.rotation.eulerAngles;

                if (75 < nowCamRotaX && nowCamRotaX < 180)
                {
                    cam.transform.RotateAround(this.transform.position, transform.rotation * v3, rotaSpeed * 5);
                    return;
                }
                else if ((180 < nowCamRotaX && nowCamRotaX < 285))
                {
                    cam.transform.RotateAround(this.transform.position, transform.rotation * v3, rotaSpeed * 5);
                    return;
                }
            }
            cam.transform.RotateAround(this.transform.position, transform.rotation * v3, -rotaSpeed);
            //Debug.Log(axis + "<= ��Ʈ���ú���  �븻���� =>" + axis.normalized);
        }
    }

    private void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float distance = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

            Vector3 zoomDistance = transform.position - cam.transform.position;

            if (zoomDistance.magnitude < 0.2f && 0 < distance) return;
            if (2.0f < zoomDistance.magnitude && distance < 0) return;
            cam.transform.localPosition += Vector3.Slerp(Camera.main.transform.localPosition, zoomDistance, 1f) * distance;
        }

        #region pov�� �̿��� ��
        //         if (distance != 0)
        //         {
        //             beforeDistance = distance;
        //             Camera.main.fieldOfView += distance;
        //         }
        //         if (Camera.main.fieldOfView < 15)
        //         {
        //             Camera.main.fieldOfView = 15;
        //             return;
        //         }
        //         else
        //         if (100 < Camera.main.fieldOfView)
        //         {
        //             Camera.main.fieldOfView = 100;
        //             return;
        //         }
        #endregion
    }
    #endregion
    protected virtual void CreateDomino()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeSelectIndex();
            return;
        }

        if (GameManager.Instance.isStarted == true) return;

        if (Input.GetKeyDown(KeyCode.Space) && !isHited && !JsonSerialize.loadingNow) // ���𰡿� ������ �ʴ� �����϶��� ������ �� �ִ�.
        {
            switch (selectIndex)
            {
                case (int)eDominoType.Default:
                    if (dominoCaseComponent.stackDominosInCase[(int)eDominoType.Default].Count != 0)
                    {
                        CreateDefaultDomino();
                        SoundMG.Instance.PlaySFX(eSoundType.CreateDomino, transform.position, DominoManager.Instance.createDominoVolume);
                    }
                    return;
                case (int)eDominoType.JetPack:
                    if (dominoCaseComponent.stackDominosInCase[(int)eDominoType.JetPack].Count != 0)
                    {
                        CreateJetPackDomino();
                        SoundMG.Instance.PlaySFX(eSoundType.CreateDomino, transform.position, DominoManager.Instance.createDominoVolume);
                    }
                    return;
                case (int)eDominoType.LongLong:
                    if (dominoCaseComponent.stackDominosInCase[(int)eDominoType.LongLong].Count != 0)
                    {
                        CreateLongLongDomino();
                        SoundMG.Instance.PlaySFX(eSoundType.CreateDomino, transform.position, DominoManager.Instance.createDominoVolume);
                    }
                    return;
                case (int)eDominoType.Bomb:
                    if (dominoCaseComponent.stackDominosInCase[(int)eDominoType.Bomb].Count != 0)
                    {
                        CreateBombDomino();
                        SoundMG.Instance.PlaySFX(eSoundType.CreateDomino, transform.position, DominoManager.Instance.createDominoVolume);
                    }
                    return;
                case (int)eDominoType.InPortal:
                    if (dominoCaseComponent.stackDominosInCase[(int)eDominoType.InPortal].Count != 0)
                    {
                        CreatePortalDomino();
                        SoundMG.Instance.PlaySFX(eSoundType.CreateDomino, transform.position, DominoManager.Instance.createDominoVolume);
                    }
                    return;
            }
        }
        else if (Input.GetKey(KeyCode.C) && destroyTarget != null) // ���̳� ����� �κ�;
        {
            DeleteDomino();
        }
    }

    protected virtual void CreateDefaultDomino()
    {
        eDominoType tempType = eDominoType.Default;

        var i = Instantiate(DominoManager.Instance.lDominoPrefab[(int)tempType], gameObject.transform.position, gameObject.transform.rotation);
        i.transform.SetParent(dominosParent.transform);
        dominoCaseComponent.PopAndDestroyInCase(tempType);
    }
    protected virtual void CreateJetPackDomino()
    {
        eDominoType tempType = eDominoType.JetPack;

        var jet = Instantiate(DominoManager.Instance.lDominoPrefab[(int)tempType], gameObject.transform.position, gameObject.transform.rotation);
        jet.transform.SetParent(dominosParent.transform);
        dominoCaseComponent.PopAndDestroyInCase(tempType);
    }
    protected virtual void CreateLongLongDomino()
    {
        eDominoType tempType = eDominoType.LongLong;

        var _LongDomino = Instantiate(DominoManager.Instance.lDominoPrefab[(int)tempType], gameObject.transform.position, gameObject.transform.rotation);
        _LongDomino.transform.SetParent(dominosParent.transform);
        dominoCaseComponent.PopAndDestroyInCase(tempType);
    }
    protected virtual void CreateBombDomino()
    {
        eDominoType tempType = eDominoType.Bomb;

        var _BombDomino = Instantiate(DominoManager.Instance.lDominoPrefab[(int)tempType], gameObject.transform.position, gameObject.transform.rotation);
        _BombDomino.transform.SetParent(dominosParent.transform);
        dominoCaseComponent.PopAndDestroyInCase(tempType);
    }
    protected virtual void CreatePortalDomino()
    {
        Debug.Log("��Ż");

        if (isInportal == true && dominoCaseComponent.stackDominosInCase[(int)eDominoType.InPortal].Count != 0)
        {
            eDominoType tempType = eDominoType.InPortal;
            var _InPortalDomino = Instantiate(DominoManager.Instance.lDominoPrefab[(int)tempType], gameObject.transform.position, gameObject.transform.rotation);
            _InPortalDomino.transform.SetParent(dominosParent.transform);
            isInportal = false;
        }
        else
        {
            if (isInportal == false)
            {
                var _OutPortalDomino = Instantiate(DominoManager.Instance.lDominoPrefab[(int)eDominoType.OutPortal], gameObject.transform.position, gameObject.transform.rotation);
                _OutPortalDomino.transform.SetParent(dominosParent.transform);
                isInportal = true;
                dominoCaseComponent.PopAndDestroyInCase(eDominoType.InPortal);
            }
        }
    }

    protected virtual void DeleteDomino()
    {
        var dt = destroyTarget.GetComponent<Domino>().GetDominoType();

        if (dt == eDominoType.InPortal)
        {
            var _portaldomino = destroyTarget.GetComponent<PortalDomino>();

            if (_portaldomino) Destroy(_portaldomino.outPortal);

            isInportal = true;
        }
        else if (dt == eDominoType.OutPortal)
        {
            isInportal = false;
        }

        FindObjectOfType<DominoCase>().CreateDominoInCase(dt);
        Destroy(destroyTarget.gameObject);
        isHited = false;
        makerRenderer.materials = originalMaterial;
        destroyTarget = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Domino domicomp) == true)
        {
            if (other.CompareTag("Domino") && domicomp.isStageDomin == false)
            {
                destroyTarget = other.gameObject;
                //                Debug.Log("��Ʈ���� Ÿ����" + destroyTarget);
            }
            //Debug.Log(other.transform.name + "���̴�.");
            isHited = true;
            makerRenderer.materials = hitenMaterial;
        }

        if (other.CompareTag("BgObject"))
        {
            makerRenderer.materials = hitenMaterial;
            isHited = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        isHited = false;
        makerRenderer.materials = originalMaterial;
        destroyTarget = null;
    }

    //     void OnDrawGizmos()
    //     {
    //         BoxCollider boxCol = gameObject.GetComponent<BoxCollider>();
    // 
    //         Gizmos.color = Color.blue;
    //         //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
    //         if (true)
    //             //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
    //             Gizmos.DrawWireCube(boxCol.center, boxCol.size);
    //     }
}

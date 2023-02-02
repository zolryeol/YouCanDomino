using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  실질적인 플레이어 이동하고 도미노를 생성한다.
/// </summary>

public class DominoMaker : MonoBehaviour
{
    static public GameObject dominosParent;

    [Header("shift누를때 속도")]
    [SerializeField] protected float speedUp = 1f;

    [Header("기본이동 속도")]
    [SerializeField] protected float moveSpeed = 2.2f;

    [Header("상승하강 속도")]
    [SerializeField] protected float translateSpeed = 5.0f;     // 상승하강시 속도

    float rotationSpeedHorizontal = 1f;      // 좌우 회전시 속도
    float rotationSpeedVertical = 1f;
    float zoomSpeed = 1f;

    protected Renderer makerRenderer;
    protected BoxCollider makerCollider;

    [Header("부딪혔을때 바뀔 머터리얼")]
    public Material[] hitenMaterial;
    protected Material[] originalMaterial;

    [Header("도미노매쉬들")]
    public Mesh[] dominoMeshs;
    public MeshFilter MakerMeshFilter;

    [Header("도미노머터리얼들")]
    protected Material[] dominosMaterial;

    [Header("카메라의 기본값")]
    [SerializeField] Vector3 cameraPos = new Vector3(0, 0.5f, -0.85f);
    [SerializeField] Vector3 cameraRot = new Vector3(15, 0, 0);
    [SerializeField] GameObject cameraArm;

    Camera cam;

    protected bool isHited = false;
    public bool isInportal = true; // 현재 생성하는 포탈이 들어가는 것인지 나오는 것인지 판단
    bool rayHit = false; // 레이용 불린
    protected int selectIndex = 0;

    protected GameObject destroyTarget;

    DominoCase dominoCaseComponent; // 케이스의 정보

    /// 그림자
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
        #region WASD 이동
        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.LeftShift)) { speedUp = 3f; }
            else { speedUp = 1f; }
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * speedUp); // 캐릭터의 이동

            /// 현재카메라의 로테이션의 X값을 0으로 만든 후 forward방향으로 이동

            //             Transform oriRota = cam.transform;
            //             oriRota.eulerAngles = new Vector3(0, oriRota.eulerAngles.y, oriRota.eulerAngles.z);
            // 
            //cam.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * speedUp, cam.transform);
            //cam.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * speedUp, transform);

            /// (카메라 클래스라고 가정하고)
            /// Following 카메라의 Transform = 따라다니기를 원하는 오브젝트의 Transform + 따라다니면서 보는 각도의 Transform
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

        #region 상승하강
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

        //RotateMakerLeftRight(); // 좌우 카메라와 메이커 같이 회전함
        //RotateCameraUpDownThirdPerson();

        //RotateCameraUpDown(); // 상하 카메라만 회전함 구버전용

        //Zoom(); // 줌기능

        #region 보간 없이 사용하던 회전
        //mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        //transform.Rotate(0, mouseDelta.x, 0);
        //Camera.main.transform.Rotate(-mouseDelta.y, 0, 0);

        //         #region 좌우 회전 (키보드) // 마우스로 바꾸어서 안씀
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

        if ((int)eDominoType.InPortal < selectIndex) selectIndex = (int)eDominoType.Default; // 4초과시 0으로

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

    #region 전에사용하던 카메라회전 및 줌
    private void RotateMakerLeftRight()
    {
        if (Input.GetAxis("Mouse X") != 0)
        {
            Vector3 rot = transform.rotation.eulerAngles; // 메이커 회전을 Vector3로 반환

            float rotaSpeed = Input.GetAxis("Mouse X") * rotationSpeedHorizontal;

            rot.y += rotaSpeed; // 마우스 X 위치 * 회전 스피드
                                //rot.x += -1 * Input.GetAxis("Mouse Y") * rotateSpeed; // 마우스 Y 위치 * 회전 스피드
            Quaternion q = Quaternion.Euler(rot); // Quaternion으로 변환
            q.z = 0;

            transform.rotation = Quaternion.Slerp(transform.rotation, q, 2f); // // 보간시켜서 자연스럽게 회전
                                                                              //Camera.main.transform.rotation = this.transform.rotation;
                                                                              //Camera.main.transform.RotateAround(this.transform.position, Vector3.up, rotaSpeed);

            cam.transform.RotateAround(this.transform.position, Vector3.up, rotaSpeed);
            //         Vector3 camPos = cam.transform.position;
            //         cam.transform.position = new Vector3(camPos.x, camPos.y, this.transform.position.z - cameraDistance);
        }
    }

    private void RotateCameraUpDown()
    {
        Vector3 rot = Camera.main.transform.rotation.eulerAngles; // 메이커 회전을 Vector3로 반환
                                                                  //rot.y += Input.GetAxis("Mouse X") * rotateSpeed; // 마우스 X 위치 * 회전 스피드
        rot.x += -1 * Input.GetAxis("Mouse Y") * rotationSpeedVertical; // 마우스 Y 위치 * 회전 스피드
        Quaternion q = Quaternion.Euler(rot); // Quaternion으로 변환
                                              //transform.rotation = Quaternion.Slerp(transform.rotation, q, 2f); // // 보간시켜서 자연스럽게 회전
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
            //Debug.Log(axis + "<= 고스트로컬벡터  노말벡터 =>" + axis.normalized);
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

        #region pov를 이용한 줌
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

        if (Input.GetKeyDown(KeyCode.Space) && !isHited && !JsonSerialize.loadingNow) // 무언가에 닿이지 않는 상태일때만 생성할 수 있다.
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
        else if (Input.GetKey(KeyCode.C) && destroyTarget != null) // 도미노 지우는 부분;
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
        Debug.Log("포탈");

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
                //                Debug.Log("디스트로이 타겟은" + destroyTarget);
            }
            //Debug.Log(other.transform.name + "닿이다.");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBoxMaker : DominoMaker
{
    GameObject copyObject;
    MeshFilter shadowMeshFilter;
    Transform[] childTransform;

    MeshFilter copyMeshFilter;

    Transform pastedObject;

    [SerializeField]
    public GameObject[] sandBoxOBJs;

    private void Start()
    {
        originalMaterial = makerRenderer.materials;

        dominosParent = new GameObject("CreatedDominos");

        makerShadow = transform.GetChild(1).gameObject;

        shadowMeshFilter = makerShadow.GetComponent<MeshFilter>();

        MakerMeshFilter = gameObject.GetComponent<MeshFilter>();

        copyMeshFilter = GameObject.Find("CopyFilter").GetComponent<MeshFilter>();

        childTransform = gameObject.GetComponentsInChildren<Transform>();

        Debug.Log("속도" + moveSpeed);
        Debug.Log("속도" + speedUp);

        SoundMG.Instance.SetBgmPlayer();

        SoundMG.Instance.PlayBGMforSandBox();
    }
    // Start is called before the first frame update
    override public void Update()
    {
        CreateDomino();

        if ((Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Equals)) && copyObject != null)
        {
            if (copyObject.name == "Hinge") return;
            copyMeshFilter.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
        }

        if ((Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus)) && copyObject != null)
        {
            if (copyObject.name == "Hinge") return;
            copyMeshFilter.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            var startDominos = GameObject.FindGameObjectsWithTag("StartDominoSandBox");
            foreach (var stds in startDominos)
            {
                stds.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 50);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other)
        {
            makerRenderer.materials = hitenMaterial;

            isHited = true;
            makerRenderer.materials = hitenMaterial;
        }

        if (other.CompareTag("Bottom")) return;

        if (Input.GetKey(KeyCode.V)) CopyObjectSimple(other);

        if (Input.GetKey(KeyCode.C)) this.DeleteDomino(other);


    }

    void CopyObjectSimple(Collider other)
    {
        if (other.CompareTag("ShowCase")) return;

        copyObject = other.transform.root.gameObject;

        copyMeshFilter.mesh = copyObject.GetComponent<MeshFilter>().mesh;

        copyMeshFilter.gameObject.transform.localScale = other.transform.root.localScale;
    }

    void CopyFilterReset()
    {
        copyMeshFilter.mesh = null;
        copyMeshFilter.gameObject.transform.localScale = Vector3.one;
    }

    void CopyObject(Collider other)
    {
        copyObject = other.transform.root.gameObject;

        MeshFilter[] meshFilters = copyObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        Debug.Log("랭쓰" + meshFilters.Length);
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;

            combine[i].transform = transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix * Matrix4x4.Rotate(transform.rotation);

            //combine[i].transform = Matrix4x4.TRS(transform.localPosition, transform.localRotation, Vector3.one) * meshFilters[i].transform.localToWorldMatrix;

            //meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

        transform.gameObject.SetActive(true);

    }
    void PasteObject()
    {
        Quaternion q = Quaternion.Euler(transform.rotation.eulerAngles);
        pastedObject = Instantiate<GameObject>(copyObject, transform.position, q).transform;
        if (pastedObject.TryGetComponent<HingeJoint>(out HingeJoint nu) == true) { }
        else pastedObject.transform.localScale = copyMeshFilter.transform.localScale;
        // Instantiate<GameObject>(copyObject, transform.position, Quaternion.identity);
        if (pastedObject.CompareTag("StartDominoSandBox")) return;
        pastedObject.tag = "Replica";
    }

    protected override void CreateDomino()
    {
        if (Input.GetKeyDown(KeyCode.Space) && copyObject)
        {
            PasteObject();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            copyObject = null;
            CopyFilterReset();
            ChangeSelectIndex();
            return;
        }

        if (GameManager.Instance.isStarted == true) return;

        if (Input.GetKeyDown(KeyCode.Space) && !isHited && !JsonSerialize.loadingNow) // 무언가에 닿이지 않는 상태일때만 생성할 수 있다.
        {
            switch (selectIndex)
            {
                case (int)eDominoType.Default:
                    CreateDefaultDomino();
                    SoundMG.Instance.PlaySFX(eSoundType.CreateDomino, transform.position);
                    return;
                case (int)eDominoType.JetPack:
                    CreateJetPackDomino();
                    SoundMG.Instance.PlaySFX(eSoundType.CreateDomino, transform.position);
                    return;
                case (int)eDominoType.LongLong:
                    CreateLongLongDomino();
                    SoundMG.Instance.PlaySFX(eSoundType.CreateDomino, transform.position);
                    return;
                case (int)eDominoType.Bomb:
                    CreateBombDomino();
                    SoundMG.Instance.PlaySFX(eSoundType.CreateDomino, transform.position);
                    return;
                case (int)eDominoType.InPortal:
                    CreatePortalDomino();
                    SoundMG.Instance.PlaySFX(eSoundType.CreateDomino, transform.position);
                    return;
            }
        }
    }

    protected override void CreateDefaultDomino()
    {
        eDominoType tempType = eDominoType.Default;

        var i = Instantiate(DominoManager.Instance.lDominoPrefab[(int)tempType], gameObject.transform.position, gameObject.transform.rotation);
        i.transform.SetParent(dominosParent.transform);
    }
    protected override void CreateJetPackDomino()
    {
        eDominoType tempType = eDominoType.JetPack;

        var jet = Instantiate(DominoManager.Instance.lDominoPrefab[(int)tempType], gameObject.transform.position, gameObject.transform.rotation);
        jet.transform.SetParent(dominosParent.transform);
    }
    protected override void CreateLongLongDomino()
    {
        eDominoType tempType = eDominoType.LongLong;

        var _LongDomino = Instantiate(DominoManager.Instance.lDominoPrefab[(int)tempType], gameObject.transform.position, gameObject.transform.rotation);
        _LongDomino.transform.SetParent(dominosParent.transform);
    }
    protected override void CreateBombDomino()
    {
        eDominoType tempType = eDominoType.Bomb;

        var _BombDomino = Instantiate(DominoManager.Instance.lDominoPrefab[(int)tempType], gameObject.transform.position, gameObject.transform.rotation);
        _BombDomino.transform.SetParent(dominosParent.transform);
    }
    protected override void CreatePortalDomino()
    {
        Debug.Log("포탈");

        if (isInportal == true)
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
            }
        }
    }

    private void DeleteDomino(Collider _collider)
    {
        if (_collider.TryGetComponent<Domino>(out Domino _domino) == true)
        {
            var dt = _domino.GetDominoType();

            if (dt == eDominoType.InPortal)
            {
                var _portaldomino = _collider.GetComponent<PortalDomino>();

                if (_portaldomino) Destroy(_portaldomino.outPortal);

                isInportal = true;
            }
            else if (dt == eDominoType.OutPortal)
            {
                isInportal = false;
            }
        }

        if (_collider.CompareTag("Domino")) Destroy(_collider.gameObject);
        else if (_collider.transform.CompareTag("ShowCase") || _collider.transform.root.CompareTag("Models") || _collider.transform.name == "StartDomino_SandBox") { }
        else Destroy(_collider.transform.root.gameObject);

        isHited = false;
        makerRenderer.materials = originalMaterial;
    }
}

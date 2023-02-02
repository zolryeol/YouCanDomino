using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �߸� ��Ż�� ����ڴ�.
/// ���鿡 ��� ��ü�� ���߰��ѵ� �ݴ����� �о��.
/// </summary>

public class PortalDomino : MonoBehaviour
{
    public GameObject outPortal; // �ƿ���Ż 

    public bool isIn = true;

    bool usedSkill = false;

    string originalTag = null;

    Rigidbody rigidBody;
    MeshCollider meshCollider;

    private void Start()
    {
        rigidBody = transform.GetComponent<Rigidbody>();
        meshCollider = transform.GetComponent<MeshCollider>();
    }

    /// ��Ż�� ��� ������ �ٷ� Kinematic���� �����Ѵ�.
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("DominoMakerGhost"))
        {
            rigidBody.isKinematic = true;   // ��򰡿� ���̸� ������.
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Domino") && usedSkill == false && isIn == true && other.gameObject.layer != 6)
        {
            ForceRotateJetpack(other); // ��Ʈ���϶� ������ �����ٲ��� �ӽù����ڵ�

            usedSkill = true;

            if (other.gameObject.name == "ExplosionScale") return; /// ���߽������� �ɸ��� �ʵ���

            var targetRigidBody = other.GetComponent<Rigidbody>();

            targetRigidBody.constraints = RigidbodyConstraints.FreezeAll;

            /// ����� �� �̵��ϴ� ����
            Vector3 distance = this.transform.position - other.transform.position; //�Ÿ��� ���Ѵ�.
            var direction = distance.normalized; //Ÿ���� �̵��� �����̴�.
            direction.y = 0;

            /// �±� ����
            originalTag = other.transform.tag;
            other.transform.tag = "Changing";

            // �ƿ���Ż���� ���ö� �ٷ� �۵����� �ʵ��� �Ž��ݶ��̴��� ��� ���д�.
            other.GetComponent<MeshCollider>().enabled = false;

            // �ƿ���Ż�� �����Ѵٸ� 
            if (outPortal) StartCoroutine(Move(direction, other));

            SoundMG.Instance.PlaySFX(eSoundType.Portal, transform.position, DominoManager.Instance.potalvolume);
        }
    }

    void ForceRotateJetpack(Collider other)
    {
        if (other.GetComponent<Domino>().GetDominoType() == eDominoType.JetPack)
        {
            //other.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.transform.localRotation = Quaternion.Euler(45, 0, 0);
        }
    }

    float GetTopVertice(Collider c) // ������Ʈ�� ���ؽ� �� ����󿡼� ���� ���� ģ���� ��ȯ
    {
        Matrix4x4 localToWorld = c.transform.localToWorldMatrix;

        var mf = c.GetComponent<MeshFilter>(); // 
        Vector3 world_v = Vector3.zero;

        for (int i = 0; i < mf.mesh.vertices.Length; ++i)
        {
            if (world_v.y < localToWorld.MultiplyPoint3x4(mf.mesh.vertices[i]).y)
            {
                world_v = localToWorld.MultiplyPoint3x4(mf.mesh.vertices[i]);
            }
        }
        //Vector3 topVertex = Vector3.zero;

        //foreach (var v in vertexs)
        // {
        //     Debug.Log("���ؽ� = " + v);
        //     if (topVertex.y < v.y)
        //         topVertex.y = v.y; // ���� �����ִ� ���ý��� ���Ѵ�.
        //  }

        //Debug.LogWarning("TopV = " + world_v);
        return world_v.y;
    }

    int repeatCountUpper;
    int repeatCountDownSide;

    IEnumerator Move(Vector3 _drt, Collider _targetCollider)
    {
        WaitForFixedUpdate wfu = new WaitForFixedUpdate();
        WaitForSeconds wfs = new WaitForSeconds(0.02f);
        var _shrinkSize = DominoManager.Instance.shrinkSize;

        Vector3 _localScale = _targetCollider.transform.localScale; // Ÿ���� ���ý�����
        //Debug.LogWarning("���� ����" + _targetCollider.bounds.size.y);

        var mc = this.GetComponent<MeshCollider>(); // ����Ż�� �Ž��ݶ��̴�

        // ��Ż���̳뺸�� �����κ��� �ϴ� ���δ�.
        while (mc.bounds.max.y < GetTopVertice(_targetCollider))
        {
            repeatCountUpper++;
            _localScale.y -= 0.05f;
            _targetCollider.transform.localScale = _localScale;

            yield return wfs;
        }

        Vector3 lengthdist = transform.position - _targetCollider.transform.position;
        //Debug.LogError("�����Ÿ�" + lengthdist.magnitude);

        /// renderer.bounds �� ���� ���� ��ǥ ���� �� ���� ��� ���ڸ� ��ȯ�Ѵ�.
        /// mesh.bounds �� ���� ���� ��ǥ ���� �� ���� ��� ���ڸ� ��ȯ�Ѵ�.
        /// collider �� ���� ��ǥ ��ȯ�Ѵ�.

        var repCount = _localScale.y / _shrinkSize; // �ڷ�ƾ�� �� Ƚ��
        var dist = lengthdist.magnitude / repCount;
        //Debug.LogError("�ѹ����� ������ �Ÿ�" + dist);


        // Ÿ���� ���̸鼭 �̵���Ų��.
        while (0 <= _localScale.y)
        {
            repeatCountDownSide++;
            _localScale.y -= _shrinkSize;
            _targetCollider.transform.localScale = _localScale;

            _targetCollider.transform.position += _drt * dist;

            yield return wfs;
        }

        /// ���� ��Ż�� ������ ��Ż�� yȸ����(world) ���̸�ŭ�� !!!�����ش� by lehide
        /// //���Ϸ� �ޱ�

        float _deltaYAngle = transform.rotation.eulerAngles.y - outPortal.transform.rotation.eulerAngles.y;

        _targetCollider.transform.Rotate(new Vector3(0, -1 * _deltaYAngle, 0), Space.World);

        /// ȸ�� ���¸� �����ϴ� ���� �´�.
        ///c.transform.localRotation = outPortal.transform.localRotation;

        ///���͸� ȸ����Ű�� ��� by lehide
        // ���� ����
        //         Vector3 _originalVector = Vector3.one;
        //         Quaternion _yRotation = Quaternion.AngleAxis(30.0f, new Vector3(0, 1.0f, 0));
        //         _originalVector = _yRotation * _originalVector;
        // 
        //         //Matrix4x4
        //         // ȸ�� ����� ���� ����.
        //         // ����� �̿��ϴ� ���
        //         Matrix4x4 _rotationMatrix = Matrix4x4.Rotate(_yRotation);

        //        Quaternion q = c.transform.rotation;

        //      c.transform.rotation = c.transform.rotation * outPortal.transform.rotation;

        /// ������ ���������ϱ� �ణ�� ���������ش� ������� �� ������ �ʿ䰡�ִ�.
        var zSize = outPortal.GetComponent<Collider>().bounds.size.z;
        Debug.Log(zSize);
        Vector3 outportalPos = outPortal.transform.position;
        outportalPos += outPortal.transform.forward * zSize;
        outportalPos.y += 0.01f;
        Debug.Log("�ƿ���Ż y�� = " + outPortal.transform.rotation.eulerAngles.y);
        _targetCollider.transform.position = outportalPos;

        //�ٽ� Ŀ�����Ѵ�.
        while (0 < repeatCountDownSide)
        {
            repeatCountDownSide--;
            _localScale.y += _shrinkSize;
            _targetCollider.transform.localScale = _localScale;

            yield return wfs;
        }

        while (0 < repeatCountUpper)
        {
            repeatCountUpper--;
            _localScale.y += 0.05f;
            _targetCollider.transform.localScale = _localScale;
            yield return wfs;
        }

        _targetCollider.enabled = true;
        _targetCollider.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        _targetCollider.transform.tag = originalTag;
    }
}

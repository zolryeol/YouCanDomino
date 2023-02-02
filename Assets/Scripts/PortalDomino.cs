using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 야매 포탈을 만들겠다.
/// 정면에 닿는 물체를 멈추게한뒤 반대편에서 밀어낸다.
/// </summary>

public class PortalDomino : MonoBehaviour
{
    public GameObject outPortal; // 아웃포탈 

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

    /// 포탈의 경우 놓으면 바로 Kinematic으로 설정한다.
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("DominoMakerGhost"))
        {
            rigidBody.isKinematic = true;   // 어딘가에 놓이면 굳힌다.
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Domino") && usedSkill == false && isIn == true && other.gameObject.layer != 6)
        {
            ForceRotateJetpack(other); // 제트팩일때 강제로 각도바꿔줌 임시방편코드

            usedSkill = true;

            if (other.gameObject.name == "ExplosionScale") return; /// 폭발스케일이 걸리지 않도록

            var targetRigidBody = other.GetComponent<Rigidbody>();

            targetRigidBody.constraints = RigidbodyConstraints.FreezeAll;

            /// 닿았을 때 이동하는 방향
            Vector3 distance = this.transform.position - other.transform.position; //거리를 구한다.
            var direction = distance.normalized; //타겟이 이동할 방향이다.
            direction.y = 0;

            /// 태그 저장
            originalTag = other.transform.tag;
            other.transform.tag = "Changing";

            // 아웃포탈에서 나올때 바로 작동하지 않도록 매시콜라이더를 잠시 꺼둔다.
            other.GetComponent<MeshCollider>().enabled = false;

            // 아웃포탈이 존재한다면 
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

    float GetTopVertice(Collider c) // 오브젝트의 버텍스 중 월드상에서 가장 높은 친구를 반환
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
        //     Debug.Log("버텍스 = " + v);
        //     if (topVertex.y < v.y)
        //         topVertex.y = v.y; // 가장 높이있는 버택스를 구한다.
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

        Vector3 _localScale = _targetCollider.transform.localScale; // 타겟의 로컬스케일
        //Debug.LogWarning("원래 높이" + _targetCollider.bounds.size.y);

        var mc = this.GetComponent<MeshCollider>(); // 인포탈의 매시콜라이더

        // 포탈도미노보다 높은부분은 일단 줄인다.
        while (mc.bounds.max.y < GetTopVertice(_targetCollider))
        {
            repeatCountUpper++;
            _localScale.y -= 0.05f;
            _targetCollider.transform.localScale = _localScale;

            yield return wfs;
        }

        Vector3 lengthdist = transform.position - _targetCollider.transform.position;
        //Debug.LogError("남은거리" + lengthdist.magnitude);

        /// renderer.bounds 는 월드 공간 좌표 에서 축 정렬 경계 상자를 반환한다.
        /// mesh.bounds 는 로컬 공간 좌표 에서 축 정렬 경계 상자를 반환한다.
        /// collider 는 월드 좌표 반환한다.

        var repCount = _localScale.y / _shrinkSize; // 코루틴을 돌 횟수
        var dist = lengthdist.magnitude / repCount;
        //Debug.LogError("한바퀴당 움질임 거리" + dist);


        // 타겟을 줄이면서 이동시킨다.
        while (0 <= _localScale.y)
        {
            repeatCountDownSide++;
            _localScale.y -= _shrinkSize;
            _targetCollider.transform.localScale = _localScale;

            _targetCollider.transform.position += _drt * dist;

            yield return wfs;
        }

        /// 들어가는 포탈과 나오는 포탈의 y회전각(world) 차이만큼을 !!!돌려준다 by lehide
        /// //오일러 앵글

        float _deltaYAngle = transform.rotation.eulerAngles.y - outPortal.transform.rotation.eulerAngles.y;

        _targetCollider.transform.Rotate(new Vector3(0, -1 * _deltaYAngle, 0), Space.World);

        /// 회전 상태를 대입하는 것이 맞다.
        ///c.transform.localRotation = outPortal.transform.localRotation;

        ///벡터를 회전시키는 방법 by lehide
        // 원본 벡터
        //         Vector3 _originalVector = Vector3.one;
        //         Quaternion _yRotation = Quaternion.AngleAxis(30.0f, new Vector3(0, 1.0f, 0));
        //         _originalVector = _yRotation * _originalVector;
        // 
        //         //Matrix4x4
        //         // 회전 행렬을 만들어서 곱함.
        //         // 행렬을 이용하는 방법
        //         Matrix4x4 _rotationMatrix = Matrix4x4.Rotate(_yRotation);

        //        Quaternion q = c.transform.rotation;

        //      c.transform.rotation = c.transform.rotation * outPortal.transform.rotation;

        /// 나갈때 겹쳐있으니까 약간의 오프셋을준다 사이즈는 좀 수정할 필요가있다.
        var zSize = outPortal.GetComponent<Collider>().bounds.size.z;
        Debug.Log(zSize);
        Vector3 outportalPos = outPortal.transform.position;
        outportalPos += outPortal.transform.forward * zSize;
        outportalPos.y += 0.01f;
        Debug.Log("아웃포탈 y각 = " + outPortal.transform.rotation.eulerAngles.y);
        _targetCollider.transform.position = outportalPos;

        //다시 커지게한다.
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

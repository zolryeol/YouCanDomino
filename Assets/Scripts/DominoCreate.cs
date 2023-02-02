using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DominoCreate : MonoBehaviour
{
    [SerializeField] public Bezier m_Bezier;
    [SerializeField] public GameObject m_Domino;

    private int m_BezierResolution = 300;
    public float m_DominoInterval = 0.1f;
    public LayerMask m_GroundLayer;

    [ContextMenu("Make Domino")]
    public void MakeDomino2()
    {
        GameObject parent = new GameObject();
        transform.parent = this.gameObject.transform;

        float _deltaInterval = 1.0f / (m_BezierResolution - 1);

        Vector3 _lastDominoPoint;
        Vector3 _nextDeltaPoint;
        Vector3 _dir;
        Quaternion _rotation;

        // Bezier의 첫 지점에 놓일 도미노를 설치합니다.

        _lastDominoPoint = m_Bezier.GetPoint(0).GetPosition();
        _nextDeltaPoint = m_Bezier.GetPoint(0.01f).GetPosition();
        _dir = (_nextDeltaPoint - _lastDominoPoint).normalized;
        _dir.y = 0;
        _rotation = Quaternion.LookRotation(_dir);

        // Bezier의 위에서 땅바닥을 향해 Raycast하여
        // 실제로 도미노가 설치될 위치를 구합니다.
        // 이는 도미노가 공중에 설치되지 않게, 또 지형을 뚫고 설치되지 않게 하기 위함입니다.

        Vector3 _rayOrigin = _lastDominoPoint + Vector3.up * 10;
        RaycastHit _hit;
        if (Physics.Raycast(_rayOrigin, Vector3.down, out _hit, 100.0f, m_GroundLayer))
        {
            _lastDominoPoint = _hit.point;
        }

        GameObject _go = GameObject.Instantiate(m_Domino, _lastDominoPoint, _rotation, parent.transform);
        _go.name = "1, 0";

        int _dominoCount = 1;
        for (int i = 1; i < m_BezierResolution; ++i)
        {
            // Bezier 위의 지점을 resolution 수만큼 검사하여
            // 도미노를 일정 간격마다 설치합니다.

            float _currentDelta = _deltaInterval * i;

            Vector3 _currentPoint = m_Bezier.GetPoint(_currentDelta).GetPosition();

            // Bezier의 위에서 땅바닥을 향해 Raycast하여
            // 실제로 도미노가 설치될 위치를 구합니다.
            // 이는 도미노가 공중에 설치되지 않게, 또 지형을 뚫고 설치되지 않게 하기 위함입니다.

            _rayOrigin = _currentPoint + Vector3.up * 10;
            if (Physics.Raycast(_rayOrigin, Vector3.down, out _hit, 100.0f, m_GroundLayer))
            {
                _currentPoint = _hit.point;
            }

            // 지난 번에 설치된 도미노와 일정 간격이 떨어진 위치인지 확인합니다.
            // 떨어져있지 않은 경우, 설치를 건너뜁니다.

            if (Vector3.Distance(_currentPoint, _lastDominoPoint) < m_DominoInterval)
                continue;

            _lastDominoPoint = _currentPoint;
            ++_dominoCount;

            _nextDeltaPoint = m_Bezier.GetPoint(_currentDelta + 0.01f).GetPosition();
            _dir = (_nextDeltaPoint - _currentPoint).normalized;
            _dir.y = 0;
            _rotation = Quaternion.LookRotation(_dir);

            _go = GameObject.Instantiate(m_Domino, _currentPoint, _rotation, parent.transform);
            _go.name = $"{_dominoCount}, {_currentDelta}";
        }
    }

}

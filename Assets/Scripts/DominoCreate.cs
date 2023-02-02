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

        // Bezier�� ù ������ ���� ���̳븦 ��ġ�մϴ�.

        _lastDominoPoint = m_Bezier.GetPoint(0).GetPosition();
        _nextDeltaPoint = m_Bezier.GetPoint(0.01f).GetPosition();
        _dir = (_nextDeltaPoint - _lastDominoPoint).normalized;
        _dir.y = 0;
        _rotation = Quaternion.LookRotation(_dir);

        // Bezier�� ������ ���ٴ��� ���� Raycast�Ͽ�
        // ������ ���̳밡 ��ġ�� ��ġ�� ���մϴ�.
        // �̴� ���̳밡 ���߿� ��ġ���� �ʰ�, �� ������ �հ� ��ġ���� �ʰ� �ϱ� �����Դϴ�.

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
            // Bezier ���� ������ resolution ����ŭ �˻��Ͽ�
            // ���̳븦 ���� ���ݸ��� ��ġ�մϴ�.

            float _currentDelta = _deltaInterval * i;

            Vector3 _currentPoint = m_Bezier.GetPoint(_currentDelta).GetPosition();

            // Bezier�� ������ ���ٴ��� ���� Raycast�Ͽ�
            // ������ ���̳밡 ��ġ�� ��ġ�� ���մϴ�.
            // �̴� ���̳밡 ���߿� ��ġ���� �ʰ�, �� ������ �հ� ��ġ���� �ʰ� �ϱ� �����Դϴ�.

            _rayOrigin = _currentPoint + Vector3.up * 10;
            if (Physics.Raycast(_rayOrigin, Vector3.down, out _hit, 100.0f, m_GroundLayer))
            {
                _currentPoint = _hit.point;
            }

            // ���� ���� ��ġ�� ���̳�� ���� ������ ������ ��ġ���� Ȯ���մϴ�.
            // ���������� ���� ���, ��ġ�� �ǳʶݴϴ�.

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

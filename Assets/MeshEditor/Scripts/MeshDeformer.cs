using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeshDeformer : MonoBehaviour
{
    private Mesh _deformingMesh;
    private Vector3[] _originalVertices, _displacedVertices;

    public float deformRadius = 0.5f; // ���� �ݰ�
    public float deformStrength = 0.2f; // ���� ����
    public float maxDeformAmount = 0.1f;


    private void Start()
    {
        // �Ž� ���ؽ� ���� ��������
        _deformingMesh = GetComponent<MeshFilter>().mesh;
        if(_deformingMesh == null)
        {
            Debug.Log("There's no mesh");
        }
        _originalVertices = _deformingMesh.vertices;
        _displacedVertices = new Vector3[_originalVertices.Length];
        for (int i = 0; i < _originalVertices.Length; i++)
        {
            _displacedVertices[i] = _originalVertices[i];
        }
    }
    public Vector3? GetNearVertex(Vector3 worldPosition)
    {
        Vector3 position = this.transform.position;
        Quaternion rotation = this.transform.rotation;

        Vector3 nearVectex = Vector3.zero;
        float minDistance = float.MaxValue;
        foreach (Vector3 vertex in _displacedVertices)
        {
            var pos = this.transform.TransformPoint(vertex);
            var distance = (pos - worldPosition).sqrMagnitude;
            if (minDistance > distance)
            {
                minDistance = distance;
                nearVectex = pos;
            }
        }
        return nearVectex;
    }

    public void DeformVertices(Vector3 worldPosition, Vector3 deformerPoint)
    {
        int[] nearbyVertexIndices = GetNearbyVertices(worldPosition);

        foreach (int index in nearbyVertexIndices)
        {
            // ���ؽ��� ���� ��ǥ -> ���� ��ǥ�� ��ȯ
            Vector3 vertexWorldPos = this.transform.TransformPoint(_displacedVertices[index]);
            Vector3 vertexLocalPos = this.transform.InverseTransformPoint(vertexWorldPos);

            // ���� x-z ��鿡�� �߽��� ���� ���� ���
            Vector3 radialDirection = new Vector3(vertexLocalPos.x, 0, vertexLocalPos.z).normalized;

            // �ݰ� ������ �ݴ�� �����Ͽ� �׻� ���η� ���ϵ��� ����
            Vector3 inwardDirection = -radialDirection;

            // �Ÿ� ��� ����ġ ���
            float distance = Vector3.Distance(vertexWorldPos, worldPosition);
            float weight = 1f - (distance / deformRadius);
            weight = Mathf.Clamp01(weight);

            // ���� ���� ���
            float deformFactor = deformStrength * weight;

            // ���� ���� ���� �� ���� ���
            Vector3 direction = inwardDirection * deformFactor;

            // �ִ� ������ ����
            if (direction.magnitude > maxDeformAmount)
            {
                direction = direction.normalized * maxDeformAmount;
            }

            // ���� ����
            _displacedVertices[index] += direction;

            // ����� �ð�ȭ
            Debug.DrawRay(vertexWorldPos, inwardDirection * 0.1f, Color.green, 0.1f); // ����
            Debug.DrawRay(vertexWorldPos, direction, Color.red, 0.1f); // ���� ����
        }

        // ������Ʈ�� ���ؽ��� ����
        _deformingMesh.vertices = _displacedVertices;
        _deformingMesh.RecalculateNormals();
    }








    private int[] GetNearbyVertices(Vector3 worldPosition)
    {
        List<int> nearbyIndices = new List<int>();
        float sqrRadius = deformRadius * deformRadius;

        for (int i = 0; i < _displacedVertices.Length; i++)
        {
            Vector3 vertexWorldPos = this.transform.TransformPoint(_displacedVertices[i]);
            float sqrDistance = (vertexWorldPos - worldPosition).sqrMagnitude;

            if (sqrDistance < sqrRadius) // ���� �ݰ� ���� �ִ� ���ؽ� �߰�
            {
                nearbyIndices.Add(i);
            }
        }
        return nearbyIndices.ToArray();
    }

    /// <summary>
    /// ���ؽ� ����
    /// </summary>
    /// <param name="targetVertex"></param>
    /// <param name="deformerPoint"></param>
    public void SetVertex(Vector3 targetVertex, Vector3 deformerPoint)
    {
        int[] vertexIndexs = GetVertexIndexs(targetVertex);
        for (int i = 0; i < vertexIndexs.Length; i++)
        {
            var index = vertexIndexs[i];
            if (index != -1)
            {
                _displacedVertices[index] = this.transform.InverseTransformPoint(deformerPoint);
            }
        }
        _deformingMesh.vertices = _displacedVertices;
        _deformingMesh.RecalculateNormals();
    }

    /// <summary>
    /// ���ؽ� ��������
    /// </summary>
    /// <param name="targetVertex"></param>
    /// <returns></returns>
    private int[] GetVertexIndexs(Vector3 targetVertex)
    {
        int[] indexs = { -1, -1, - 1 };
        int setIndex = 0;
        for (int i = 0; i < _displacedVertices.Length; i++)
        {
            if (this.transform.TransformPoint(_displacedVertices[i]) == targetVertex)
            {
                indexs[setIndex++] = i;
            }
        }
        return indexs;
    }
}
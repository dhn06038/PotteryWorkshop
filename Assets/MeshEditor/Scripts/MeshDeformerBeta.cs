using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeshDeformerBeta : MonoBehaviour
{
    private Mesh _deformingMesh;
    private Vector3[] _originalVertices, _displacedVertices;

    public float deformRadius = 0.5f; // 변형 반경
    public float deformStrength = 0.2f; // 변형 강도
    public float maxDeformAmount = 0.1f;

    private void Start()
    {
        // 매시 버텍스 정보 가져오기
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

    /// <summary>
    /// 월드 포지션에서 가장 근접한 버텍스 반환하기
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
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

        _deformingMesh.RecalculateNormals();
        Vector3[] normals = _deformingMesh.normals;

        foreach (int index in nearbyVertexIndices)
        {
            Vector3 vertexWorldPos = this.transform.TransformPoint(_displacedVertices[index]);

            float distance = Vector3.Distance(vertexWorldPos, worldPosition);

            // 거리에 따라 강도 감소
            //float deformFactor = Mathf.Lerp(deformStrength, 0, distance / deformRadius);
            float deformFactor = deformStrength * Mathf.Exp(-distance * distance / (2 * deformRadius * deformRadius));

            Vector3 vertexLocalPos = transform.InverseTransformPoint(vertexWorldPos);
            float angle = Mathf.Atan2(vertexLocalPos.z, vertexLocalPos.x);

            // 변형 방향 가중치 계산 (절댓값 사용)
            float directionMultiplier = Mathf.Abs(Mathf.Cos(angle));

            // 변형 방향을 버텍스의 법선으로 설정
            Vector3 normalWorld = this.transform.TransformDirection(normals[index]).normalized;
            Vector3 direction = normalWorld * directionMultiplier;

            // 변형 적용
            Vector3 displacement = direction * deformFactor;

            // 최대 변형량 제한
            if (displacement.magnitude > maxDeformAmount)
            {
                displacement = displacement.normalized * maxDeformAmount;
            }

            _displacedVertices[index] += displacement;
        }

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

            if (sqrDistance < sqrRadius) // 변형 반경 내에 있는 버텍스 추가
            {
                nearbyIndices.Add(i);
            }
        }
        return nearbyIndices.ToArray();
    }

    /// <summary>
    /// 버텍스 수정
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
    /// 버텍스 가져오기
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
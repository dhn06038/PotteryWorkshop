using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeshDeformer : MonoBehaviour
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
            // 버텍스의 월드 좌표 -> 로컬 좌표로 변환
            Vector3 vertexWorldPos = this.transform.TransformPoint(_displacedVertices[index]);
            Vector3 vertexLocalPos = this.transform.InverseTransformPoint(vertexWorldPos);

            // 로컬 x-z 평면에서 중심을 향한 방향 계산
            Vector3 radialDirection = new Vector3(vertexLocalPos.x, 0, vertexLocalPos.z).normalized;

            // 반경 방향을 반대로 설정하여 항상 내부로 향하도록 강제
            Vector3 inwardDirection = -radialDirection;

            // 거리 기반 가중치 계산
            float distance = Vector3.Distance(vertexWorldPos, worldPosition);
            float weight = 1f - (distance / deformRadius);
            weight = Mathf.Clamp01(weight);

            // 변형 강도 계산
            float deformFactor = deformStrength * weight;

            // 최종 변형 방향 및 강도 계산
            Vector3 direction = inwardDirection * deformFactor;

            // 최대 변형량 제한
            if (direction.magnitude > maxDeformAmount)
            {
                direction = direction.normalized * maxDeformAmount;
            }

            // 변형 적용
            _displacedVertices[index] += direction;

            // 디버그 시각화
            Debug.DrawRay(vertexWorldPos, inwardDirection * 0.1f, Color.green, 0.1f); // 방향
            Debug.DrawRay(vertexWorldPos, direction, Color.red, 0.1f); // 최종 변형
        }

        // 업데이트된 버텍스를 적용
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
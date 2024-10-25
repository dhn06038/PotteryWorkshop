using UnityEngine;

public class MeshDeformer : MonoBehaviour
{
    private Mesh _deformingMesh;
    private Vector3[] _originalVertices, _displacedVertices;

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
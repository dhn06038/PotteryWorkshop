using UnityEngine;

public class MeshEditor : MonoBehaviour
{
    public GameObject pointer;

    private Camera _camera;

    private MeshDeformer _targetMeshDeformer = null;
    private Vector3? _targetVertexPos;

    private void Start()
    {
        _camera = Camera.main;
        pointer.SetActive(false);
    }

    private void Update()
    {
        bool isPressLeft = Input.GetMouseButton(0);
        bool isPressRight = Input.GetMouseButton(1);

        pointer.SetActive(false);
        if (_targetMeshDeformer != null && _targetVertexPos != null)
        {
            if (isPressRight)
            {
                float speed = 2f;
                _targetMeshDeformer.transform.Rotate(0f, -Input.GetAxis("Mouse X") * speed, 0f, 0f);
                _targetMeshDeformer.transform.Rotate(Input.GetAxis("Mouse Y") * speed, 0f, 0f);
            }
            else if (isPressLeft)
            {
                float distanceZ = _camera.WorldToScreenPoint(_targetVertexPos.Value).z;
                Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceZ);
                Vector3 vertexPos = _camera.ScreenToWorldPoint(mousePos);
                Vector3 deformerPoint = vertexPos;
                _targetMeshDeformer.SetVertex(_targetVertexPos.Value, deformerPoint);
                _targetVertexPos = deformerPoint;
                pointer.transform.position = _targetVertexPos.Value;
                pointer.SetActive(true);
            }
        }
        else
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            Vector3 start = ray.origin;
            Vector3 dir = ray.direction;
            float distance = 10f;

            Debug.DrawRay(start, dir * distance);
            if (Physics.Raycast(start, dir, out RaycastHit hit, distance))
            {
                _targetMeshDeformer = hit.collider.gameObject.GetComponent<MeshDeformer>();
                _targetVertexPos = _targetMeshDeformer.GetNearVertex(hit.point);
                pointer.transform.position = _targetVertexPos.Value;
                pointer.SetActive(true);
            }
        }

        if (!isPressLeft && !isPressRight)
        {
            _targetMeshDeformer = null;
            _targetVertexPos = null;
        }
    }
}

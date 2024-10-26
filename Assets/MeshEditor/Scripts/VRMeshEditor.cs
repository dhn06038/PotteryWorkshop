using UnityEngine;

public class VRMeshEditor : MonoBehaviour
{
    public GameObject pointer;
    public GameObject controller; // ��Ʈ�ѷ� ������Ʈ, OVRControllerPrefab ��

    private MeshDeformer _targetMeshDeformer = null;
    private Vector3? _targetVertexPos;

    private void Start()
    {
        pointer.SetActive(false);
    }

    private void Update()
    {
        bool isPressLeft = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger); // Ʈ���� ��ư
        bool isPressRight = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger); // �׸� ��ư

        pointer.SetActive(false);

        if (_targetMeshDeformer != null && _targetVertexPos != null)
        {
            if (isPressRight)
            {
                // ȸ�� ����, ��Ʈ�ѷ��� ȸ������ ���� ����ص� ��
                float speed = 2f;
                _targetMeshDeformer.transform.Rotate(0f, -OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x * speed, 0f, Space.World);
                _targetMeshDeformer.transform.Rotate(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y * speed, 0f, 0f, Space.World);
            }
            else if (isPressLeft)
            {
                Vector3 vertexPos = controller.transform.position; // ��Ʈ�ѷ� ��ġ�� ���
                Vector3 deformerPoint = vertexPos;
                //_targetMeshDeformer.SetVertex(_targetVertexPos.Value, deformerPoint);
                _targetMeshDeformer.DeformVertices(_targetVertexPos.Value, deformerPoint);
                _targetVertexPos = deformerPoint;
                pointer.transform.position = _targetVertexPos.Value;
                pointer.SetActive(true);
            }
        }
        else
        {
            // ��Ʈ�ѷ����� ����ĳ��Ʈ�� �߻�
            Ray ray = new Ray(controller.transform.position, controller.transform.forward);
            float distance = 10f;

            Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);
            if (Physics.Raycast(ray, out RaycastHit hit, distance))
            {
                _targetMeshDeformer = hit.collider.gameObject.GetComponent<MeshDeformer>();
                if (_targetMeshDeformer != null)
                {
                    _targetVertexPos = _targetMeshDeformer.GetNearVertex(hit.point);
                    pointer.transform.position = _targetVertexPos.Value;
                    pointer.SetActive(true);
                }
            }
        }

        // ��ư�� ���� ���� ����
        if (!isPressLeft && !isPressRight)
        {
            _targetMeshDeformer = null;
            _targetVertexPos = null;
        }
    }
}
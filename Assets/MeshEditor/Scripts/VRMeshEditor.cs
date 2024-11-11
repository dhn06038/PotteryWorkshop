using UnityEngine;

public class VRMeshEditor : MonoBehaviour
{
    public GameObject pointer;
    //public GameObject controller; // 컨트롤러 오브젝트, OVRControllerPrefab 등

    private MeshDeformer _targetMeshDeformer = null;
    //private Vector3? _targetVertexPos;

    private void OnTriggerEnter(Collider other)
    {
        // Cylinder에 부착된 MeshDeformer를 가져옴
        MeshDeformer deformer = GetComponent<MeshDeformer>();
        if (deformer != null)
        {
            _targetMeshDeformer = deformer;
            Debug.Log("Trigger entered");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_targetMeshDeformer != null)
        {
            // 손끝의 공 위치를 기반으로 메쉬 변형
            Vector3 contactPoint = pointer.transform.position;
            Vector3 transformPoint = new Vector3(transform.position.x, contactPoint.y, transform.position.z);
            _targetMeshDeformer.DeformVertices(contactPoint, transformPoint);  // 현재 위치를 변형점으로 사용
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 충돌이 끝나면 타겟 초기화
        if (GetComponent<MeshDeformer>() == _targetMeshDeformer)
        {
            _targetMeshDeformer = null;
            Debug.Log("Trigger exit");
        }
    }

    /*private void Update()
    {
        bool isPressLeft = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger); // 트리거 버튼
        bool isPressRight = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger); // 그립 버튼

        pointer.SetActive(false);

        if (_targetMeshDeformer != null && _targetVertexPos != null)
        {
            if (isPressRight)
            {
                // 회전 조작, 컨트롤러의 회전값을 직접 사용해도 됨
                float speed = 2f;
                _targetMeshDeformer.transform.Rotate(0f, -OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x * speed, 0f, Space.World);
                _targetMeshDeformer.transform.Rotate(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y * speed, 0f, 0f, Space.World);
            }
            else if (isPressLeft)
            {
                Vector3 vertexPos = controller.transform.position; // 컨트롤러 위치를 사용
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
            // 컨트롤러에서 레이캐스트를 발사
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

        // 버튼을 떼면 선택 해제
        if (!isPressLeft && !isPressRight)
        {
            _targetMeshDeformer = null;
            _targetVertexPos = null;
        }
    }*/
}
using UnityEngine;

public class VRMeshEditor : MonoBehaviour
{
    public GameObject pointer;
    //public GameObject controller; // ��Ʈ�ѷ� ������Ʈ, OVRControllerPrefab ��
    public AudioClip touchSoundEffect; 
    private MeshDeformer _targetMeshDeformer = null;
    //private Vector3? _targetVertexPos;
    private AudioSource audioSource;  

    private void Start()
    {
        // AudioSource 초기화
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = touchSoundEffect;  // 효과음 연결
        audioSource.loop = true;              // 반복 재생 설정 (만지는 동안 소리 반복)
        audioSource.playOnAwake = false;      // 시작 시 자동 재생 방지

         audioSource.volume = 0.8f;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Cylinder�� ������ MeshDeformer�� ������
        MeshDeformer deformer = GetComponent<MeshDeformer>();
        if (deformer != null)
        {
            _targetMeshDeformer = deformer;
            Debug.Log("Trigger entered");

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_targetMeshDeformer != null)
        {
            // �ճ��� �� ��ġ�� ������� �޽� ����
            Vector3 contactPoint = pointer.transform.position;
            Vector3 transformPoint = new Vector3(transform.position.x, contactPoint.y, transform.position.z);
            _targetMeshDeformer.DeformVertices(contactPoint, transformPoint);  // ���� ��ġ�� ���������� ���
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �浹�� ������ Ÿ�� �ʱ�ȭ
        if (GetComponent<MeshDeformer>() == _targetMeshDeformer)
        {
            _targetMeshDeformer = null;
            Debug.Log("Trigger exit");

            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    /*private void Update()
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
    }*/
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintManager : MonoBehaviour
{
    public Transform tableTransform; // 책상의 Transform
    public float placementHeight = 0.8f; // 책상 위 배치 높이
    public AudioClip placeSoundEffect;
    public Transform spawnPoint; // 새로운 도자기 생성 위치

    private Collider triggerCollider;
    private GameObject fixedCylinder; // 고정된 실린더 참조
    private AudioSource audioSource;

    void Start()
    {
        triggerCollider = GetComponent<Collider>();

        // AudioSource 설정
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.clip = placeSoundEffect;
    }

    private void Update()
    {
        // 버튼 입력으로 고정된 도자기를 이동 가능한 상태로 전환
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            if (fixedCylinder != null)
            {
                CreateMovablePot(fixedCylinder);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("MovablePot")) return;
        
        // 그랩 가능한 실린더인지 확인
        var grabbable = other.GetComponent<Oculus.Interaction.Grabbable>();


        if (grabbable != null)
        {
            // 원본 실린더의 위치와 회전값 저장
            Vector3 position = other.transform.position;
            Quaternion rotation = other.transform.rotation;
            Vector3 scale = other.transform.localScale;

            // 원본 게임오브젝트에서 MeshFilter와 MeshRenderer 가져오기
            MeshFilter originalFilter = other.GetComponent<MeshFilter>();
            MeshRenderer originalRenderer = other.GetComponent<MeshRenderer>();

            // 새로운 고정된 실린더 생성
            fixedCylinder = new GameObject("Fixed Cylinder");

            // 책상 위에 위치 설정
            fixedCylinder.transform.position = new Vector3(
                tableTransform.position.x,
                tableTransform.position.y + placementHeight,
                tableTransform.position.z
            );
            fixedCylinder.transform.localScale = scale;

            // 메시와 렌더러 복사
            MeshFilter newFilter = fixedCylinder.AddComponent<MeshFilter>();
            MeshRenderer newRenderer = fixedCylinder.AddComponent<MeshRenderer>();
            newFilter.mesh = Instantiate(originalFilter.mesh);
            newRenderer.material = new Material(originalRenderer.material); // 기존 머티리얼 복사

            // 콜라이더 추가 (상호작용용)
            var collider = fixedCylinder.AddComponent<MeshCollider>();
            collider.sharedMesh = newFilter.mesh;
            collider.convex = true;
            collider.isTrigger = true;

            // 원본 실린더 제거
            Destroy(other.gameObject);

            triggerCollider.enabled = false;
        }
    }

    private void CreateMovablePot(GameObject originalPot)
{
    // 기존 도자기 제거
    Destroy(originalPot);

    // 새로운 게임오브젝트 생성
    GameObject movablePot = new GameObject("Movable Pottery");
    movablePot.tag = "MovablePot";

    // 메쉬 필터와 렌더러 복사
    MeshFilter originalFilter = originalPot.GetComponent<MeshFilter>();
    MeshRenderer originalRenderer = originalPot.GetComponent<MeshRenderer>();

    MeshFilter newFilter = movablePot.AddComponent<MeshFilter>();
    MeshRenderer newRenderer = movablePot.AddComponent<MeshRenderer>();

    // 메쉬와 머티리얼 복사
    newFilter.mesh = Instantiate(originalFilter.mesh);
    newRenderer.material = originalRenderer.material; // 기존 머티리얼 유지

    // 위치와 회전 설정
    movablePot.transform.position = spawnPoint.position;
    movablePot.transform.rotation = Quaternion.identity; // 초기 회전값 리셋
    movablePot.transform.localScale = originalPot.transform.localScale;

    // Rigidbody 추가
    Rigidbody rb = movablePot.AddComponent<Rigidbody>();
    rb.useGravity = true;
    rb.isKinematic = true;

    // 초기 물리 속도 설정 (정지 상태)
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    // 콜라이더 추가
    var collider = movablePot.AddComponent<MeshCollider>();
    collider.sharedMesh = newFilter.mesh;
    collider.convex = true;
    collider.isTrigger = false;

    // OVR 상호작용 컴포넌트 추가
    var grabbable = movablePot.AddComponent<Oculus.Interaction.Grabbable>();
    grabbable.enabled = true;

    // GrabInteractable을 추가하여 상호작용 가능 설정
    var grabInteractable = movablePot.AddComponent<Oculus.Interaction.GrabInteractable>();
    grabInteractable.InjectRigidbody(rb);
    grabInteractable.InjectOptionalPointableElement(grabbable);

    // 효과음 재생
    if (audioSource != null)
    {
        audioSource.Play();
    }
}
}
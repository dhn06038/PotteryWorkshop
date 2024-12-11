using UnityEngine;
using System.Collections;

public class KilnManager : MonoBehaviour
{
    public Material glazedMaterial; // 유광 갈색 머티리얼
    public float fadeTime = 1.0f;  // 페이드 시간
    public Transform spawnPoint;    // 새 도자기 생성 위치
    public AudioClip potterySoundEffect; 
    private Collider triggerCollider;
    private AudioSource audioSource;
    void Start()
    {
        triggerCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // 효과음 설정
        audioSource.clip = potterySoundEffect;
        audioSource.playOnAwake = false; // 자동 재생 방지
    }

    private void OnTriggerEnter(Collider other)
    {
        var grabbable = other.GetComponent<Oculus.Interaction.Grabbable>();
        if (grabbable != null)
        {
            StartCoroutine(ProcessPottery(other.gameObject));
            // 트리거 콜라이더 비활성화
            triggerCollider.enabled = false;
        }
    }

    private IEnumerator ProcessPottery(GameObject originalPot)
    {
        // 페이드 아웃
        yield return StartCoroutine(FadeScreen(true));

        // 원본 도자기 제거
        Destroy(originalPot);

        // 새로운 도자기 생성
        GameObject glazedPot = CreateGlazedPot(originalPot);
        // 효과음 재생
        if (audioSource != null && potterySoundEffect != null)
        {
            audioSource.Play();
        }
        
        // 페이드 인
        yield return StartCoroutine(FadeScreen(false));
    }

    private GameObject CreateGlazedPot(GameObject originalPot)
    {
        // 새로운 게임오브젝트 생성
        GameObject glazedPot = new GameObject("Glazed Pottery");
        
        // 메쉬 필터와 렌더러 복사
        MeshFilter originalFilter = originalPot.GetComponent<MeshFilter>();
        
        MeshFilter newFilter = glazedPot.AddComponent<MeshFilter>();
        MeshRenderer newRenderer = glazedPot.AddComponent<MeshRenderer>();
        
        // 메쉬 복사 및 유광 머티리얼 적용
        newFilter.mesh = Instantiate(originalFilter.mesh);
        newRenderer.material = glazedMaterial;
        
        // 위치와 회전 설정
        glazedPot.transform.position = spawnPoint.position;
        glazedPot.transform.rotation = spawnPoint.rotation;
        glazedPot.transform.localScale = originalPot.transform.localScale;

        // Rigidbody 추가
        Rigidbody rb = glazedPot.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = true;
        rb.drag = 0.3f;
        rb.angularDrag = 0.3f;
        rb.mass = 0.1f;

        // 콜라이더 추가
        var collider = glazedPot.AddComponent<MeshCollider>();
        collider.sharedMesh = newFilter.mesh;
        collider.convex = true;
        collider.isTrigger = true;

        // OVR 상호작용 컴포넌트 추가
        var grabbable = glazedPot.AddComponent<Oculus.Interaction.Grabbable>();
        grabbable.enabled = true;

        var grabInteractable = glazedPot.AddComponent<Oculus.Interaction.GrabInteractable>();
        grabInteractable.enabled = true;
        grabInteractable.InjectRigidbody(rb);
        grabInteractable.InjectOptionalPointableElement(grabbable);

        var distanceGrabInteractable = glazedPot.AddComponent<Oculus.Interaction.DistanceGrabInteractable>();
        distanceGrabInteractable.enabled = true;
        distanceGrabInteractable.InjectRigidbody(rb);
        distanceGrabInteractable.InjectOptionalPointableElement(grabbable);

        if (audioSource != null)
        {
            audioSource.Play(); // AudioSource에 설정된 클립을 재생
        }
        return glazedPot;
    }

    private IEnumerator FadeScreen(bool fadeOut)
    {
        // 페이드를 위한 캔버스와 이미지가 필요합니다
        GameObject fadeObject = GameObject.Find("FadeCanvas"); // 씬에 FadeCanvas가 있어야 합니다
        CanvasGroup canvasGroup = fadeObject.GetComponent<CanvasGroup>();
        
        float elapsed = 0f;
        float startAlpha = fadeOut ? 0f : 1f;
        float endAlpha = fadeOut ? 1f : 0f;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / fadeTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
    }
}

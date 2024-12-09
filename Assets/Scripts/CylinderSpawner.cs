using UnityEngine;
using Oculus.Interaction;

public class CylinderSpawner : MonoBehaviour
{
    public MeshCylinder originalCylinder;    // 원본 실린더 참조
    public OVRInput.Button completeButton = OVRInput.Button.One;  // 완료 버튼 (기본값: A 버튼)
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;  // 사용할 컨트롤러
    
    private void Update()
    {
        if (OVRInput.GetDown(completeButton, controller))
        {
            if (originalCylinder != null)
            {
                // 잡을 수 있는 복사본 생성
                GameObject copy = originalCylinder.CreateGrabbableCopy();
                
                // 플레이어 카메라 기준으로 앞쪽에 배치
                Vector3 spawnPosition = Camera.main.transform.position + 
                                     Camera.main.transform.forward * 0.3f + // 거리를 좀 더 가깝게
                                     Vector3.up * 0.2f; // 바닥보다 좀 더 높게
                copy.transform.position = spawnPosition;
                
                // 크기 조정
                copy.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                
                // 회전 초기화 (완전히 수평으로)
                copy.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
                
                // 원본 실린더 제거
                Destroy(originalCylinder.gameObject);
            }
        }
    }
} 
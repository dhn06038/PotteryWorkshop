using UnityEngine;
using Oculus.Interaction;

public class CylinderSpawner : MonoBehaviour
{
    public MeshCylinder originalCylinder;    // 원본 실린더 참조
    public OVRInput.Button completeButton = OVRInput.Button.One;  // 완료 버튼 (기본값: A 버튼)
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;  // 사용할 컨트롤러
    public Transform spawnPoint;
    
    private void Update()
    {
        if (OVRInput.GetDown(completeButton, controller))
        {
            if (originalCylinder != null)
            {
                // 잡을 수 있는 복사본 생성
                GameObject copy = originalCylinder.CreateGrabbableCopy();

                copy.transform.position = spawnPoint.transform.position;
                
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
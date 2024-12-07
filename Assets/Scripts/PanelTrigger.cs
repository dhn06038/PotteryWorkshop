using UnityEngine;
using UnityEngine.UI;

public class PanelTrigger : MonoBehaviour
{
    public GameObject yesNoPanel;  // 예/아니오 패널
    public Button yesButton;       // Yes 버튼
    public Button noButton;        // No 버튼
    public Transform cameraTransform;
    private bool playerInZone = false;
    private bool isPanelActive = false;

    void Start()
    {
        // 패널 비활성화
        yesNoPanel.SetActive(false);
    }

    void Update()
    {
        // 플레이어가 트리거 안에 있을 때 X 버튼으로 패널 표시
        if (playerInZone && !isPanelActive && OVRInput.GetDown(OVRInput.Button.One)) // X 버튼
        {
            ShowPanel();
            return; // 패널 활성화만 처리
        }

        // 패널 활성화 상태에서 Yes/No 동작 실행
        if (isPanelActive)
        {
            if (OVRInput.GetDown(OVRInput.Button.One)) // X 버튼 → Yes 동작
            {
                Debug.Log("Yes 버튼 클릭 시도");
                yesButton.onClick.Invoke();

                
            }

            if (OVRInput.GetDown(OVRInput.Button.Two)) // Y 버튼 → No 동작
            {
                Debug.Log("No 버튼 클릭");
                noButton.onClick.Invoke();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            // 패널 비활성화
            yesNoPanel.SetActive(false);
            isPanelActive = false;
        }
    }

    void ShowPanel()
    {
        yesNoPanel.SetActive(true);
        yesNoPanel.transform.position = cameraTransform.position + cameraTransform.forward * 0.4f;  // 카메라 앞 0.4미터
        yesNoPanel.transform.rotation = Quaternion.LookRotation(cameraTransform.forward);
        isPanelActive = true;
        Debug.Log("패널 활성화");
    }
}
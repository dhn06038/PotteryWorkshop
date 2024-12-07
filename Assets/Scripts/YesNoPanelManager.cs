using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class YesNoPanelManager : MonoBehaviour
{
     public Image fadePlane;         // 암전 효과를 위한 UI 이미지
    public Color fadeInColor = new Color(0, 0, 0, 0); // 투명
    public Color fadeOutColor = new Color(0, 0, 0, 1); // 완전히 검은색
    public float fadeDuration = 1f; // 암전 지속 시간
    public void OnYesClicked()
    {
        
       
        Debug.Log("Yes 버튼 클릭됨, 암전 시작");
        StartCoroutine(FadeSequence());
    }

    public void OnNoClicked()
    {
        Debug.Log("No 버튼 클릭됨");
        gameObject.SetActive(false); // 패널 닫기
    }
    

    private IEnumerator FadeSequence()
    
    {   
        
        // 1. 암전
        yield return StartCoroutine(Fade(fadeInColor, fadeOutColor, fadeDuration));

        // 2. 암전 상태 유지
        yield return new WaitForSeconds(1f); // 1초 유지

        // 3. 복귀
        yield return StartCoroutine(Fade(fadeOutColor, fadeInColor, fadeDuration));

         gameObject.SetActive(false);

        Debug.Log("암전 완료 및 복귀");
    }

    private IEnumerator Fade(Color from, Color to, float time, System.Action onComplete = null)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            fadePlane.color = Color.Lerp(from, to, elapsedTime / time);
            yield return null;
        }
        fadePlane.color = to;

        // 암전 완료 후 콜백 실행
        onComplete?.Invoke();
    }
} 
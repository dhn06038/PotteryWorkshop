using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBrush : MonoBehaviour
{
    private MeshRenderer brushRenderer;

    private void Start()
    {
        // 브러시의 메쉬 렌더러 가져오기
        brushRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 다른 오브젝트의 메쉬 렌더러 가져오기
        MeshRenderer otherRenderer = other.GetComponent<MeshRenderer>();
        
        // 두 오브젝트 모두 메쉬 렌더러를 가지고 있는지 확인
        if (brushRenderer != null && otherRenderer != null)
        {
            // 브러시의 머티리얼을 다른 오브젝트에 적용
            otherRenderer.material = new Material(brushRenderer.material);
        }
    }
}

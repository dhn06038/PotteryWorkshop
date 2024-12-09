using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintManager : MonoBehaviour
{
    public Transform tableTransform; // 책상의 Transform
    public float placementHeight = 0.8f; // 책상 위 배치 높이
    private Collider triggerCollider;

    void Start()
    {
        triggerCollider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
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
            GameObject fixedCylinder = new GameObject("Fixed Cylinder");
            
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
            newRenderer.material = new Material(originalRenderer.material);

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
}

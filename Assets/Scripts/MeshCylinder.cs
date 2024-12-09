using UnityEngine;

public class MeshCylinder : MonoBehaviour
{
    public Texture2D texture;
    [Min(3)] public int edges = 64; // 최소 삼각형 수 (더 많은 삼각형이 더 부드러움)
    public float height = 2.0f; // 실린더의 높이
    public int heightSegments = 10; // 실린더의 높이 세그먼트 수
    public string shaderName = "Unlit/Texture"; // Unlit 셰이더 사용

    private void Awake()
    {
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find(shaderName));
        renderer.material.mainTexture = texture;
        renderer.material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off); // 뒷면 컬링 비활성화

        Mesh mesh = new Mesh();

        // 정점 계산
        int vertexCount = (edges * (heightSegments + 1)) + 2; // 측면 삼각형 수 + 상단, 하단 원의 정점 수
        Vector3[] vertices = new Vector3[vertexCount]; // 정점 배열
        Vector3[] normals = new Vector3[vertexCount]; // 정점 배열 추가
        Vector2[] uv = new Vector2[vertexCount];
        int[] triangles = new int[edges * heightSegments * 6 + edges * 6]; // 삼각형 인덱스 배열

        // 측면 삼각형 계산
        float segmentHeight = height / heightSegments;

        // 정점 계산
        int vertIndex = 0;
        for (int y = 0; y <= heightSegments; y++) // 높이 세그먼트 수만큼 반복하여 정점 계산
        {
            float yPos = y * segmentHeight;
            for (int i = 0; i < edges; i++)
            {
                float rad = Mathf.PI * 2 * i / edges;
                float x = Mathf.Sin(rad);
                float z = Mathf.Cos(rad);
                vertices[vertIndex] = new Vector3(x, yPos, z);
                normals[vertIndex] = new Vector3(x, 0, z).normalized; // 측면 정점의 법선 벡터 계산
                uv[vertIndex] = new Vector2((x + 1) * 0.5f, (z + 1) * 0.5f);
                vertIndex++;
            }
        }

        // 하단 원의 중심 정점 추가
        vertices[vertIndex] = new Vector3(0, 0, 0); // 하단 원의 중심 정점
        normals[vertIndex] = Vector3.down; // 하단 원의 법선 벡터
        uv[vertIndex] = new Vector2(0.5f, 0.5f);
        int bottomCenterIndex = vertIndex++;

        vertices[vertIndex] = new Vector3(0, height, 0); // 상단 원의 중심 정점
        normals[vertIndex] = Vector3.up; // 상단 원의 법선 벡터
        uv[vertIndex] = new Vector2(0.5f, 0.5f);
        int topCenterIndex = vertIndex++;

        // 삼각형 인덱스 계산 (측면 삼각형)
        int triIndex = 0;
        for (int y = 0; y < heightSegments; y++)
        {
            for (int i = 0; i < edges; i++)
            {
                int current = i + y * edges;
                int next = (i + 1) % edges + y * edges;
                int upperCurrent = current + edges;
                int upperNext = next + edges;

                // 첫 번째 삼각형
                triangles[triIndex] = current;
                triangles[triIndex + 1] = next;
                triangles[triIndex + 2] = upperCurrent;

                // 두 번째 삼각형
                triangles[triIndex + 3] = next;
                triangles[triIndex + 4] = upperNext;
                triangles[triIndex + 5] = upperCurrent;

                triIndex += 6;
            }
        }

        // 하단 삼각형 계산 (원의 중심 정점 기준 삼각형)
        for (int i = 0; i < edges; i++)
        {
            int next = (i + 1) % edges;
            triangles[triIndex] = bottomCenterIndex; // 하단 원의 중심 정점
            triangles[triIndex + 1] = next;
            triangles[triIndex + 2] = i;
            triIndex += 3;
        }

        // 상단 삼각형 계산 (원의 중심 정점 기준 삼각형)
        for (int i = 0; i < edges; i++)
        {
            int next = (i + 1) % edges;
            triangles[triIndex] = topCenterIndex; // 상단 원의 중심 정점
            triangles[triIndex + 1] = i + edges * heightSegments;
            triangles[triIndex + 2] = next + edges * heightSegments;
            triIndex += 3;
        }

        // 메쉬 생성
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        filter.mesh = mesh;
    }

    public GameObject CreateGrabbableCopy()
    {
        // 새로운 게임오브젝트 생성
        GameObject copy = new GameObject("Grabbable Cylinder");
        
        // 메쉬 필터와 렌더러 가져오기
        MeshFilter originalFilter = GetComponent<MeshFilter>();
        MeshRenderer originalRenderer = GetComponent<MeshRenderer>();
        
        // 새 오브젝트에 메쉬 필터와 렌더러 추가
        MeshFilter newFilter = copy.AddComponent<MeshFilter>();
        MeshRenderer newRenderer = copy.AddComponent<MeshRenderer>();
        
        // 메쉬와 머티리얼 복사
        newFilter.mesh = Instantiate(originalFilter.mesh);
        newRenderer.material = new Material(originalRenderer.material);
        
        // 크기와 회전 복사
        copy.transform.localScale = transform.localScale;
        copy.transform.rotation = transform.rotation;
        
        // Rigidbody 추가 및 설정
        Rigidbody rb = copy.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.drag = 1f;         // 공기 저항 추가
        rb.angularDrag = 1f;  // 회전 저항 추가
        rb.mass = 1f;         // 질량 설정
        
        // OVR 컴포넌트들 추가
        var grabbable = copy.AddComponent<OVRGrabbable>();
    
        // 포크 인터랙션을 위한 컴포넌트 추가
        var pokeInteractable = copy.AddComponent<Oculus.Interaction.PokeInteractable>();
        var pokeInteractor = copy.AddComponent<Oculus.Interaction.PokeInteractor>();
    
        // 콜라이더 추가 (포크 인터랙션에 필요)
        var collider = copy.AddComponent<MeshCollider>();
        collider.sharedMesh = newFilter.mesh;
        collider.convex = true;
    
        return copy;
    }
}

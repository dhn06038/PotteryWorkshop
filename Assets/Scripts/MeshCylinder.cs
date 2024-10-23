using UnityEngine;

public class MeshCylinder : MonoBehaviour
{
    public Texture2D texture;
    [Min(3)] public int edges = 64; // 원의 엣지 수 (상하단의 원을 구성하는 엣지)
    public float height = 2.0f; // 원기둥의 높이
    public int heightSegments = 10; // 옆면을 수직으로 쪼개는 세그먼트 수
    public string shaderName = "Unlit/Texture"; // Unlit 셰이더 사용

    private void Awake()
    {
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find(shaderName));
        renderer.material.mainTexture = texture;
        renderer.material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off); // 양면 렌더링

        Mesh mesh = new Mesh();

        // 정점 계산
        int vertexCount = (edges * (heightSegments + 1)) + 2; // 각 높이 세그먼트마다 정점을 추가 + 상단, 하단 중심점
        Vector3[] vertices = new Vector3[vertexCount]; // 정점 배열
        Vector2[] uv = new Vector2[vertexCount];
        int[] triangles = new int[edges * heightSegments * 6 + edges * 6]; // 옆면과 상하단을 위한 삼각형 배열

        // 각 높이 세그먼트의 높이
        float segmentHeight = height / heightSegments;

        // 정점 생성
        int vertIndex = 0;
        for (int y = 0; y <= heightSegments; y++) // 각 세그먼트마다 높이를 계산하여 정점 생성
        {
            float yPos = y * segmentHeight;
            for (int i = 0; i < edges; i++)
            {
                float rad = Mathf.PI * 2 * i / edges;
                float x = Mathf.Sin(rad);
                float z = Mathf.Cos(rad);
                vertices[vertIndex] = new Vector3(x, yPos, z);
                uv[vertIndex] = new Vector2((x + 1) * 0.5f, (z + 1) * 0.5f);
                vertIndex++;
            }
        }

        // 상하단 원 중심점 추가
        vertices[vertIndex] = new Vector3(0, 0, 0); // 하단 원 중심
        uv[vertIndex] = new Vector2(0.5f, 0.5f);
        int bottomCenterIndex = vertIndex++;

        vertices[vertIndex] = new Vector3(0, height, 0); // 상단 원 중심
        uv[vertIndex] = new Vector2(0.5f, 0.5f);
        int topCenterIndex = vertIndex++;

        // 옆면 삼각형 생성 (정점 순서 유지)
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

        // 하단 삼각형 생성 (정점 순서 반시계 방향)
        for (int i = 0; i < edges; i++)
        {
            int next = (i + 1) % edges;
            triangles[triIndex] = bottomCenterIndex; // 하단 중심
            triangles[triIndex + 1] = next;
            triangles[triIndex + 2] = i;
            triIndex += 3;
        }

        // 상단 삼각형 생성 (정점 순서 반시계 방향)
        for (int i = 0; i < edges; i++)
        {
            int next = (i + 1) % edges;
            triangles[triIndex] = topCenterIndex; // 상단 중심
            triangles[triIndex + 1] = i + edges * heightSegments;
            triangles[triIndex + 2] = next + edges * heightSegments;
            triIndex += 3;
        }

        // 메쉬 설정
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        filter.mesh = mesh;
    }
}

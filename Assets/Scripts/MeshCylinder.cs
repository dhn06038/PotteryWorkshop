using UnityEngine;

public class MeshCylinder : MonoBehaviour
{
    public Texture2D texture;
    [Min(3)] public int edges = 64; // ���� ���� �� (���ϴ��� ���� �����ϴ� ����)
    public float height = 2.0f; // ������� ����
    public int heightSegments = 10; // ������ �������� �ɰ��� ���׸�Ʈ ��
    public string shaderName = "Unlit/Texture"; // Unlit ���̴� ���

    private void Awake()
    {
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find(shaderName));
        renderer.material.mainTexture = texture;
        renderer.material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off); // ��� ������

        Mesh mesh = new Mesh();

        // ���� ���
        int vertexCount = (edges * (heightSegments + 1)) + 2; // �� ���� ���׸�Ʈ���� ������ �߰� + ���, �ϴ� �߽���
        Vector3[] vertices = new Vector3[vertexCount]; // ���� �迭
        Vector2[] uv = new Vector2[vertexCount];
        int[] triangles = new int[edges * heightSegments * 6 + edges * 6]; // ����� ���ϴ��� ���� �ﰢ�� �迭

        // �� ���� ���׸�Ʈ�� ����
        float segmentHeight = height / heightSegments;

        // ���� ����
        int vertIndex = 0;
        for (int y = 0; y <= heightSegments; y++) // �� ���׸�Ʈ���� ���̸� ����Ͽ� ���� ����
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

        // ���ϴ� �� �߽��� �߰�
        vertices[vertIndex] = new Vector3(0, 0, 0); // �ϴ� �� �߽�
        uv[vertIndex] = new Vector2(0.5f, 0.5f);
        int bottomCenterIndex = vertIndex++;

        vertices[vertIndex] = new Vector3(0, height, 0); // ��� �� �߽�
        uv[vertIndex] = new Vector2(0.5f, 0.5f);
        int topCenterIndex = vertIndex++;

        // ���� �ﰢ�� ���� (���� ���� ����)
        int triIndex = 0;
        for (int y = 0; y < heightSegments; y++)
        {
            for (int i = 0; i < edges; i++)
            {
                int current = i + y * edges;
                int next = (i + 1) % edges + y * edges;
                int upperCurrent = current + edges;
                int upperNext = next + edges;

                // ù ��° �ﰢ��
                triangles[triIndex] = current;
                triangles[triIndex + 1] = next;
                triangles[triIndex + 2] = upperCurrent;

                // �� ��° �ﰢ��
                triangles[triIndex + 3] = next;
                triangles[triIndex + 4] = upperNext;
                triangles[triIndex + 5] = upperCurrent;

                triIndex += 6;
            }
        }

        // �ϴ� �ﰢ�� ���� (���� ���� �ݽð� ����)
        for (int i = 0; i < edges; i++)
        {
            int next = (i + 1) % edges;
            triangles[triIndex] = bottomCenterIndex; // �ϴ� �߽�
            triangles[triIndex + 1] = next;
            triangles[triIndex + 2] = i;
            triIndex += 3;
        }

        // ��� �ﰢ�� ���� (���� ���� �ݽð� ����)
        for (int i = 0; i < edges; i++)
        {
            int next = (i + 1) % edges;
            triangles[triIndex] = topCenterIndex; // ��� �߽�
            triangles[triIndex + 1] = i + edges * heightSegments;
            triangles[triIndex + 2] = next + edges * heightSegments;
            triIndex += 3;
        }

        // �޽� ����
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        filter.mesh = mesh;
    }
}

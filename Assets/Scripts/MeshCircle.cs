using UnityEngine;

public class MeshCircle : MonoBehaviour
{
    public Texture2D texture;
    [Min(3)] public int edges = 64;
    public string shaderName = "Standard";

    private void Awake()
    {
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find(shaderName));
        renderer.material.mainTexture = texture;

        Mesh mesh = new();

        /** Vertices **/
        Vector3[] vertices = new Vector3[edges + 1];
        for (int i = 0, l = edges; i < l; ++i)
        {
            float rad = Mathf.PI * 2 * i / edges;
            vertices[i] = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0f); //한 바퀴를 돌면서 정점 지정
        }
        vertices[edges] = Vector3.zero; //중앙 정점

        /** UV **/
        Vector2[] uv = new Vector2[edges + 1];
        for (int i = 0, l = edges + 1; i < l; ++i)
        {
            uv[i] = new Vector2((vertices[i].x + 1) * 0.5f, (vertices[i].y + 1) * 0.5f); //-1 ~ 1 범위를 0 ~ 1로 변경
        }

        /** Triangles **/
        int[] triangles = new int[edges * 3];
        for (int i = 0, l = edges; i < l; ++i)
        {
            triangles[i * 3] = edges;
            triangles[i * 3 + 1] = i;
            triangles[i * 3 + 2] = (i + 1) % edges;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        filter.mesh = mesh;
    }
}
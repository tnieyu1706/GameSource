using Sirenix.OdinInspector;
using UnityEngine;

public class InstancingExample : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    private Matrix4x4[] matrices;
    public int matricesCount = 1000;

    [MinMaxSlider(-100f, 100f)]
    public Vector2 xRange = Vector2.zero;
    [MinMaxSlider(-100f, 100f)]
    public Vector2 yRange = Vector2.zero;
    [MinMaxSlider(-100f, 100f)]
    public Vector2 zRange = Vector2.zero;

    void Start()
    {
        matrices = new Matrix4x4[matricesCount];
        for (int i = 0; i < matricesCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(xRange.x, xRange.y),
                Random.Range(yRange.x, yRange.y),
                Random.Range(zRange.x, zRange.y));
            matrices[i] = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
        }
    }

    void Update()
    {
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices);
    }
}
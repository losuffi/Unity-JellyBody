using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class JellyShaderModify : MonoBehaviour,IGUNReciever
{
    [SerializeField]
    private ComputeShader cs;
    [SerializeField]
    private Shader shader;
    [SerializeField]
    public float Spring;
    [SerializeField]
    public float Damping;
    private MeshRenderer render;
    private float f;
    private void OnEnable()
    {
        if (render == null)
        {
            render = GetComponent<MeshRenderer>();
            if (render == null)
            {
                render = gameObject.AddComponent<MeshRenderer>();
            }
            render.sharedMaterial = new Material(shader);
            render.hideFlags = HideFlags.HideInInspector;
        }
        f = 0;
    }
    public void AddForce(Vector3 pos, float force)
    {
        Debug.Log("Bingo");
        f = force;
        render.sharedMaterial.SetVector("_WorldForcePos", pos);
        render.sharedMaterial.SetFloat("_Force", f);
        render.sharedMaterial.SetFloat("_Spring", Spring);
        render.sharedMaterial.SetFloat("_Damping", Damping);
        render.sharedMaterial.SetFloat("_StartTime", Time.time);
    }
}

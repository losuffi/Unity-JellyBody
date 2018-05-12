using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class JellyShaderModify : MonoBehaviour,IGUNReciever
{
    public enum Type
    {
        Wave,
        Explore,
    }

    [SerializeField]
    private ComputeShader cs;
    [SerializeField]
    private Shader shader;
    [SerializeField]
    public float Spring;
    [SerializeField]
    public float Damping;
    [SerializeField]
    public float Namida;
    [SerializeField]
    public int CountMax;
    [SerializeField]
    public Type type;
    private MeshRenderer render;
    private int probe;
    private Vector4[] pAfs;
    private Vector4[] dAts;
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
        pAfs = new Vector4[CountMax];
        dAts = new Vector4[CountMax];
        probe = 0;
    }
    public void AddForce(Vector3 pos,Vector3 dir,float force)
    {
        Debug.Log("Bingo");
        Vector4 posAndForce = new Vector4(pos.x, pos.y, pos.z, force);
        Vector4 dirAndTime = new Vector4(dir.x, dir.y, dir.z, Time.time);
        EnQueue(posAndForce, dirAndTime);
        Transmit();
    }
    private void EnQueue(Vector4 a,Vector4 b)
    {
        pAfs[probe] = a;
        dAts[probe] = b;
        probe++;
        probe %= CountMax;
    }
    private void Transmit()
    {
        render.sharedMaterial.SetInt("_Count", CountMax);
        render.sharedMaterial.SetFloat("_Spring", Spring);
        render.sharedMaterial.SetFloat("_Damping", Damping);
        render.sharedMaterial.SetFloat("_Namida",Namida);
        render.sharedMaterial.SetVectorArray("_pAfs", pAfs);
        render.sharedMaterial.SetVectorArray("_dAts", dAts);
        switch(type)
        {
            case Type.Wave:
                render.sharedMaterial.EnableKeyword("_IsWave");
                render.sharedMaterial.DisableKeyword("_Explore");
                break;
            case Type.Explore:
                render.sharedMaterial.EnableKeyword("_Explore");
                render.sharedMaterial.DisableKeyword("_IsWave");
                break;
        }

    }
}

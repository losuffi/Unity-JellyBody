using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HuaJiao.JellyMiscNs
{
    [RequireComponent(typeof(MeshFilter))]
    public class JellyAgentCtr : MonoBehaviour
    {
        [SerializeField] private List<IGeometryAgent> agents;

        private MeshFilter meshFilter;
        private Mesh mesh;
        public void Init()
        {
            meshFilter = GetComponent<MeshFilter>();
            mesh = meshFilter.mesh;
        }
    }

    public abstract class IGeometryAgent:MonoBehaviour
    {
        
    }
}

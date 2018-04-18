using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HuaJiao.JellyMiscNs
{
    public class JellySphereAgent : IGeometryAgent
    {
        [SerializeField] public Vector3 Center;
        [SerializeField] public float Radius;


        private void OnDrawGizmos()
        {
            Gizmos.color=Color.yellow;
            Gizmos.DrawSphere(transform.position,Radius);
        }
    }
    
}


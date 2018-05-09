using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HuaJiao.JellyMiscNs
{
    public class JellySphereAgent : IGeometryAgent
    {
        [SerializeField] public float Radius;


        public override Vector3 worldPos
        {
            get
            {
                return transform.position;
            }
        }
        public override float GetPointDistance(Vector3 targetPoint)
        {
            return base.GetPointDistance(targetPoint) - (Radius * Radius);
        }
        private void OnDrawGizmos()
        {
            Gizmos.color=Color.yellow;
            Gizmos.DrawSphere(transform.position,Radius);
        }
    }
    
}


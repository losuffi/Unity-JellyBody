using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HuaJiao.JellyMiscNs
{
    [RequireComponent(typeof(MeshFilter))]
    public class JellyAgentCtr : MonoBehaviour
    {
        [SerializeField] private List<IGeometryAgent> agents;
        [SerializeField] private AgentProprity proprity;
        private EffectTimer.Task task;
        private MeshFilter meshFilter;
        private Mesh mesh;
        private Vector3[] vertexTemp;
        private Vector3[] vertexOrigin;
        private void Awake()
        {
            Init();
        }
        public void Init()
        {
            meshFilter = GetComponent<MeshFilter>();
            mesh = meshFilter.mesh;
            foreach(IGeometryAgent agent in agents)
            {
                agent.Wake();
            }
            for(int i = 0; i < mesh.vertexCount; i++)
            {
                FindAgent(mesh.vertices[i]).Reg(i,mesh.vertices[i]);
            }
            vertexOrigin = mesh.vertices;
            vertexTemp = new Vector3[mesh.vertexCount];
        }
        private void UpdateMesh()
        {
            for(int i = 0; i < agents.Count; i++)
            {
                agents[i].UpdatePoints(vertexTemp);
            }
            mesh.vertices = vertexTemp;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        }
        private void Update()
        {
            UpdateMesh();
        }
        private IGeometryAgent FindAgent(Vector3 pos)
        {
            IGeometryAgent a = null;
            float tempDistanceMin = 0;
            float tempDistance = 0;
            for(int i = 0; i < agents.Count; i++)
            {
                if (a == null)
                {
                    a = agents[i];
                    tempDistanceMin = agents[i].GetPointDistance(pos);
                    continue;
                }
                tempDistance= agents[i].GetPointDistance(pos);
                if (tempDistance < tempDistanceMin)
                {
                    a = agents[i];
                    tempDistanceMin = tempDistance;
                }
            }
            return a;
        }
        #region angent ctr
        private void UpdateAgents()
        {
            for(int i = 0; i < agents.Count; i++)
            {
                agents[i].AgentsTransform(proprity);
            }
            UpdateMesh();
        }

        public void AddForce(Vector3 pos,float force)
        {
            Debug.Log("Bingo");
            for(int i = 0; i < agents.Count; i++)
            {
                agents[i].AddForce(pos,force);
            }
            if (task == null)
            {
                task = EffectTimer.Instance.RunTimerTask(Time.deltaTime, CheckVelocity, ClamAgent);
            }
        }
        private bool CheckVelocity()
        {          
            for(int i = 0; i < agents.Count; i++)
            {
                agents[i].AgentsTransform(proprity);
                if (agents[i].CheckVelocity())
                {
                    return true;
                }
            }
            UpdateMesh();
            return false;
        }
        private void ClamAgent()
        {
            task = null;
        }
        #endregion
    }

    [System.Serializable]
    public class AgentProprity
    {
        [SerializeField]
        public float Spring;
        [SerializeField]
        public float Damping;
        [HideInInspector]
        public Vector3 Displacement;
    }

    public abstract class IGeometryAgent:MonoBehaviour
    {
        private Vector3 displacement;
        protected Vector3 velocity;
        protected Vector3 originPos;
        protected List<int> PointsIndex;
        protected List<Vector3> PointsLocalPos;
        public virtual void Wake()
        {
            originPos = transform.localPosition;
            PointsIndex = new List<int>();
            PointsLocalPos = new List<Vector3>();
            velocity = Vector3.zero;
        }
        public virtual float GetPointDistance(Vector3 targetPoint)
        {
            return (transform.position - targetPoint).sqrMagnitude;
        }
        public virtual void Reg(int index,Vector3 posWorld)
        {
            PointsIndex.Add(index);
            PointsLocalPos.Add(transform.worldToLocalMatrix.MultiplyPoint3x4(posWorld));
        }
        public virtual void UpdatePoints(Vector3[] output)
        {
            for(int i=0;i<PointsIndex.Count;i++)
            {
                int index= PointsIndex[i];
                output[index] = transform.localToWorldMatrix.MultiplyPoint3x4(PointsLocalPos[i]);
            }
        }
        public abstract Vector3 worldPos { get; }
        public virtual void AgentsTransform(AgentProprity Proprity)
        {
            Vector3 displacement = transform.localPosition - originPos;
            velocity -= displacement * Proprity.Spring * Time.deltaTime;
            velocity *= (1 - Proprity.Damping * Time.deltaTime);
            transform.localPosition += velocity * Time.deltaTime;
        }
        public virtual void AddForce(Vector3 pos,float force)
        {
            pos = transform.InverseTransformPoint(pos);
            Vector3 dir = transform.localPosition - pos;
            float velocitySqr = dir.sqrMagnitude;
            float singleForce = force / (1 + velocitySqr);
            velocity += dir.normalized * singleForce * Time.deltaTime;

        }
        public bool CheckVelocity()
        {
            return velocity.sqrMagnitude < 0.0000001f;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace EffectL.Support.NodeSupport
{
    public class NodeFlowInKnob : NodeKnob
    {
        public NodeFlowOutKnob connection=null;

        public static NodeFlowInKnob Create(Node nodeBody,string cotName,float offset)
        {
            NodeFlowInKnob flowIn = CreateInstance<NodeFlowInKnob>();
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(flowIn, "Create Node");
#endif
            flowIn.CotName =cotName;
            flowIn.Init(nodeBody, "FlowIn", Side.Left, offset);
            nodeBody.FlowIn = flowIn;
            return flowIn;
        }

        protected override void CheckColor()
        {
            texture2D = EffectUtility.Tex(Color.white);
        }

        public override void Linking(NodeKnob other)
        {
            base.Linking(other);
            if (other.GetType() != typeof(NodeFlowOutKnob))
            {
                return;
            }
            other.ClearConnection();
            ClearConnection();
            connection = other as NodeFlowOutKnob;
            (other as NodeFlowOutKnob).connection=this;
        }

        protected internal override void DrawConnection()
        {
            if (connection != null && connection.Body != null)
                EffectUtility.DrawLine(connection.rect.center, this.rect.center, Color.white);
        }
        protected internal override void ClearConnection()
        {
            if (connection == null)
                return;
            var temp = connection;
            connection = null;
            temp.ClearConnection();
        }
        protected internal override void Remove()
        {
#if UNITY_EDITOR

            if (connection != null)
            {
                Undo.RecordObject(connection, "Remove");
                connection.connection = null;
            }
            Undo.DestroyObjectImmediate(this);
#endif

        }
    }
}

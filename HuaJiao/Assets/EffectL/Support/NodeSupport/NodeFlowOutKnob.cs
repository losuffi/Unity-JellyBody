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
    public class NodeFlowOutKnob:NodeKnob
    {
        public NodeFlowInKnob connection;

        public static NodeFlowOutKnob Create(Node nodeBody,string cotName,float offset)
        {
            NodeFlowOutKnob flowOut = CreateInstance<NodeFlowOutKnob>();
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(flowOut, "Create Node");
#endif
            flowOut.CotName = cotName;
            flowOut.Init(nodeBody, "FlowOut", Side.Right, offset);
            nodeBody.FlowOut = flowOut;
            return flowOut;
        }

        protected override void CheckColor()
        {
            texture2D = EffectUtility.Tex(Color.white);
        }

        public override void Linking(NodeKnob other)
        {
            base.Linking(other);
            if (other.GetType() != typeof(NodeFlowInKnob))
            {
                return;
            }
            other.ClearConnection();
            ClearConnection();
            connection =other as NodeFlowInKnob;
            (other as NodeFlowInKnob).connection = this;
        }

        protected internal override void ClearConnection()
        {
            foreach (NodeOutput knob in Body.outputKnobs)
            {
                if(knob.connection==null)
                    continue;
                knob.ClearConnection();
            }
            if (connection==null)
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

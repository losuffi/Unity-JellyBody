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
    public class NodeDirectKnob:NodeKnob
    {
        public NodeDirectKnob connection;
        public static NodeDirectKnob Create(Node nodeBody,string name,Side side=Side.Left,float offset=30f)
        {
            var Direct = CreateInstance<NodeDirectKnob>();
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(Direct, "Create Node");
#endif
            offset += 20;
            Direct.Init(nodeBody, name, side, offset);
            Direct.texture2D = EffectUtility.Tex(Color.blue);
            return Direct;
        }


        protected override void CheckColor()
        {
            texture2D = EffectUtility.Tex(Color.blue);
        }

        public override void Linking(NodeKnob other)
        {
            base.Linking(other);
            if (other.GetType() != typeof(NodeDirectKnob))
            {
                return;
            }
            if (other.Name.Equals(Name))
            {
                connection=other as NodeDirectKnob;
                (other as NodeDirectKnob).connection = this;
            }
        }

        protected internal override void DrawConnection()
        {
            if (connection != null && connection.Body != null)
                EffectUtility.DrawLineDirect(rect.center,connection.rect.center,Color.yellow);
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

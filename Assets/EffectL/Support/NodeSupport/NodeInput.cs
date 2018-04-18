using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace EffectL.Support.NodeSupport
{
    public class NodeInput:NodeKnob
    {
        public string InputType;
        public NodeOutput connection=null;
        public static NodeInput Create(Node nodeBody, string inputName, string inputType,Side sd= Side.Left,float offset = 0,string fieldName="")
        {
            NodeInput input = CreateInstance<NodeInput>();
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(input, "Create Node");
#endif
            input.InputType = inputType;
            offset += 20;
            input.Init(nodeBody, inputName, sd, offset);
            SetKnobUI(input);
            nodeBody.inputKnobs.Add(input);
            input.Body = nodeBody;
            input.BindFieldName = fieldName;
            input.CotName = fieldName;
            return input;
        }


        static void SetKnobUI(NodeInput input)
        {
            input.texture2D = EffectUtility.Tex(Color.blue);
        }

        protected override void CheckColor()
        {
            base.CheckColor();
            SetKnobUI(this);
        }

        protected internal override void DrawConnection()
        {
            if(connection!=null&& connection.Body!=null)
                EffectUtility.DrawLine(connection.rect.center, this.rect.center,Color.blue);
        }

        #region TransmitValue
        public T GetValue<T>()
        {
            return connection != null ? connection.GetValue<T>() : NodeOutput.GetDefault<T>();
        }

        public object GetValue()
        {
            return connection != null ? connection.GetValue() : null;
        }
        public void SetValue<T>(T obj)
        {
            if (connection != null)
            {
                connection.SetValue<T>(obj);
            }
        }
        #endregion

        public override void Linking(NodeKnob other)
        {
            base.Linking(other);
            if (other.GetType() != typeof(NodeOutput))
            {
                return;
            }
            other.ClearConnection();
            ClearConnection();
            connection =other as NodeOutput;
            (other as NodeOutput).connection = this;
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

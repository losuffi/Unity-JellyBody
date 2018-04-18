using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace EffectL.Support.NodeSupport
{
    [Serializable]
    public class NodeOutput:NodeKnob
    {
        public string outputType;
        public NodeInput connection;
        public ConnectionTypeData ctd;
        public object Value = null;
        public static NodeOutput Create(Node nodeBody, string outputName, string outputType,Side sd= Side.Right, float offset = 0,string field="")
        {
            NodeOutput output = CreateInstance<NodeOutput>();
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(output, "Create Node");
#endif
            output.outputType = outputType;
            offset = offset + 20;
            output.Init(nodeBody, outputName, sd, offset);
            SetKnobUI(output);
            nodeBody.outputKnobs.Add(output);
            output.Body = nodeBody;
            output.BindFieldName = field;
            output.CotName = field;
            return output;
        }

        private static void SetKnobUI(NodeOutput output)
        {
            output.texture2D = EffectUtility.Tex(Color.blue);
        }

        protected override void CheckColor()
        {
            base.CheckColor();
            SetKnobUI(this);
        }

        protected internal override void DrawConnection()
        {
            if (connection != null)
            {
                EffectUtility.DrawLine(this.rect.center, connection.rect.center);
            }
        }

        void Check()
        {
            if(ctd!=null)
                return;
            if (ConnectionType.types.ContainsKey(outputType))
            {
                this.ctd = ConnectionType.types[outputType];
            }
            else
            {
                this.ctd = null;
                throw new UnityException("节点：" + Body.GetId + "-" + Body.Title + "的输出端口设置的类型，是未定义的类型！");
            }
        }

        #region TransmitValue

        public void SetValue<T>(T obj)
        {
            Check();
            if (!ctd.type.IsAssignableFrom(typeof(T)))
            {
                throw new UnityException(Body.GetId + "-" + Body.Title + "输出端口数据类型不匹配");
            }
            Value = obj;
        }

        public void SetValueDefault(object obj)
        {
            Value = obj;
        }

        public static T GetDefault<T>()
        {
            if (typeof(T).GetConstructor(Type.EmptyTypes) != null)
                return Activator.CreateInstance<T>();
            return default(T);
        }

        public T GetValue<T>()
        {
            Check();
            if (typeof(T).IsAssignableFrom(ctd.type))
                return (T)(Value ?? (Value = GetDefault<T>()));
            return GetDefault<T>();
        }

        public object GetValue()
        {
            return Value;
        }



        #endregion

        public override void Linking(NodeKnob other)
        {
            base.Linking(other);
            if (other.GetType() != typeof(NodeInput))
            {
                return;
            }
            other.ClearConnection();
            ClearConnection();
            connection =other as NodeInput;
            (other as NodeInput).connection = this;
        }
        protected internal override void ClearConnection()
        {
            if (Body.FlowOut!=null&& Body.FlowOut.connection != null)
            {
                Body.FlowOut.ClearConnection();
            }
            if (connection == null)
                return;
            var temp = connection;
            connection = null;
            temp.ClearConnection();
        }

        protected internal override void Remove()
        {
#if UNITY_EDITOR

            if (connection!=null)
            {
                Undo.RecordObject(connection, "Remove");
                connection.connection = null;
            }
            Undo.DestroyObjectImmediate(this);
#endif
        }
    }
}

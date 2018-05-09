using System;
using UnityEngine;
#if UNITY_EDITOR
using  UnityEditor;
#endif
namespace EffectL.Support.NodeSupport
{
    [Serializable]
    public enum Side
    {
        Left,
        Top,
        Right,
        Bottom,
    }
    [Serializable]
    public class NodeKnob:ScriptableObject
    {
        [HideInInspector] [SerializeField] protected internal string CotName;
        [SerializeField]
        protected internal Node Body=null;
        [SerializeField]
        protected internal string Name=string.Empty;
        [SerializeField]
        protected Side side;

        [SerializeField]
        protected internal string BindFieldName;

        [SerializeField]
        protected float sideOffset;
        [NonSerialized]
        protected Texture2D texture2D;
        [NonSerialized]
        public Rect rect;
        protected void Init(Node bodyNode, string knobName, Side sd,float offset)
        {
#if UNITY_EDITOR
            Body = bodyNode;
            Name = knobName;
            side = sd;
            sideOffset = offset;
            bodyNode.Knobs.Add(this);
            AssetDatabase.AddObjectToAsset(this, Body);
#endif
        }
        protected virtual void CheckColor()
        {
            
        }
        public virtual void Draw(Node bodyNode)
        {
#if UNITY_EDITOR
            Body = bodyNode;
            rect = GetGUIKnob();
            EffectUtility.RectConverting(ref rect,Body.curGraph.curNodeState);
            if (texture2D == null)
            {
                CheckColor();
                return;
            }
            DrawCotName();
            //GUI.DrawTexture(rect, texture2D);
            GUI.DrawTextureWithTexCoords(rect, texture2D, new Rect(0, 0, 1, 1));

#endif
        }

        protected internal virtual void DrawCotName()
        {
            if(Body.curGraph.curNodeState.GraphZoom<0.9f)
                return;
            Vector2 offset=Vector2.zero;
            var gUIStyle = Body.NodeSkin.GetStyle("nodeKnobName");
            switch (side)
            {
                case Side.Left:
                    offset=new Vector2(15,0);
                    gUIStyle = Body.NodeSkin.GetStyle("nodeKnobNameL");
                    break;
                case Side.Top:
                    offset = new Vector2(0, -10);
                    break;
                case Side.Right:
                    offset = new Vector2(-55, 0);
                    gUIStyle = Body.NodeSkin.GetStyle("nodeKnobNameR");
                    break;
                case Side.Bottom:
                    offset = new Vector2(0, 5);
                    break;
            }
            Rect drawRect = new Rect(rect.position + offset, new Vector2(50, 10));
            EffectUtility.FormatLabel(drawRect, CotName, gUIStyle);
        }

        protected internal virtual void DrawConnection()
        {
            
        }


        private Rect GetGUIKnob()
        {
            Vector2 pos = Body.rect.position;
            Vector2 size=new Vector2(10.9f,8.5f);
            //size *= Body.curGraph.curNodeState.GraphZoom;
            //sideOffset*= Body.curGraph.curNodeState.GraphZoom;
            if (side == Side.Left)
            {
                pos = new Vector2(pos.x - size.x, pos.y + sideOffset);
            }
            else if(side== Side.Top)
            {
                pos = new Vector2(pos.x + sideOffset, pos.y - size.y);
            }
            else if (side == Side.Right)
            {
                pos = new Vector2(pos.x+Body.rect.width, pos.y + sideOffset);
            }
            else if (side == Side.Bottom)
            {
                pos = new Vector2(pos.x + sideOffset, pos.y + +Body.rect.height);
            }
            return new Rect(pos, size);
        }

        public static void Linking(NodeOutput output, NodeInput input)
        {
            if(output.Body==input.Body)
                return;
            output.connection = input;
            input.connection = output;
        }

        public virtual void Linking(NodeKnob other)
        {
            if (other.Body == null || other.Body == Body)
            {
                return;
            }
        }

        protected internal virtual void ClearConnection()
        {
            
        }

        protected internal virtual void Remove()
        {
            
        }
    }
}

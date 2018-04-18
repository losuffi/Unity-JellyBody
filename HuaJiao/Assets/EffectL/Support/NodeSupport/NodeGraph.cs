using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;

#endif
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    [Serializable]
    public  class NodeGraph: ScriptableObject
    {
        public List<Node> nodes=new List<Node>();

        public string Name;
        public string Type;

        protected static Texture2D texture2D;
        public NodeEditorState curNodeState=new NodeEditorState();
        #region GUI

        protected internal void Draw()
        {
#if  UNITY_EDITOR

            if (texture2D == null)
            {
                texture2D = EditorDataMgr.GetTexture("WG");
                return;
            }
            DrawBackground();
            foreach (Node node in nodes)
            {
                node.Draw();
            }

#endif
        }

        public void DrawBackground()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            float width = curNodeState.GraphZoom / texture2D.width;
            float height = curNodeState.GraphZoom / texture2D.height;
            Vector2 offset = curNodeState.ZoomPos + curNodeState.PanOffset / curNodeState.GraphZoom;
            Rect rect = curNodeState.CurGraphRect;
            Rect uiRect=new Rect(
                -offset.x*width,
                (offset.y-rect.height)*height,
                rect.width*width,
                rect.height*height
                );
            rect.position = Vector2.zero;
            GUI.DrawTextureWithTexCoords(rect, texture2D,
                uiRect);
        }

        #endregion

        #region Node

        public void AddNode(string id,Vector2 pos)
        {
#if UNITY_EDITOR
            int gid = Undo.GetCurrentGroup();
            Undo.RecordObject(this,"Add");
            Node node = Node.CreateNode(pos, id, this);
            nodes.Add(node);
            Undo.CollapseUndoOperations(gid);

#endif
        }
        public void RemoveNode(Node selectedNode)
        {
#if UNITY_EDITOR
            int gid = Undo.GetCurrentGroup();
            Undo.RecordObject(this, "Remove");
            nodes.Remove(selectedNode);
            selectedNode.Remove();
            Undo.CollapseUndoOperations(gid);
#endif
        }
        public Node CheckFocus(Vector2 inputInfoInputPos)
        {
            foreach (Node node in nodes)
            {
                if (node.nodeRect.Contains(inputInfoInputPos))
                {
                    return node;
                }
            }
            return null;
        }
        public NodeKnob CheckFocusKnob(Vector2 inputInfoInputPos)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < nodes[i].Knobs.Count; j++)
                {
                    if (nodes[i].Knobs[j].rect.Contains(inputInfoInputPos))
                    {
                        return nodes[i].Knobs[j];
                    }
                }
            }
            return null;
        }
        #endregion


        public void Open(string path)
        {
            
        }

        #region Handle
        public void DrawSelectedNodeMenu()
        {
#if UNITY_EDITOR
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("删除节点"), false, SelectedNodeMenuCallBack, "RemoveNode");
            menu.ShowAsContext();
#endif
        }
        public void NodeGenericMenu()
        {
#if UNITY_EDITOR
            var menu = new GenericMenu();
            if (!NodeSelectPanel.selectedNode.Equals(string.Empty))
            {
                menu.AddItem(new GUIContent("添加节点"), false, NodeGenericMenuCallBack, "InsertNode");
                menu.ShowAsContext();
            }
#endif
        }

        void NodeGenericMenuCallBack(object type)
        {
            var identity = type.ToString();
            if (identity.Equals("InsertNode"))
            {
                AddNode(NodeSelectPanel.selectedNode, curNodeState.MousePos);
            }
        }
        void SelectedNodeMenuCallBack(object type)
        {
            var identity = type.ToString();
            if (identity.Equals("RemoveNode"))
            {
                RemoveNode(curNodeState.SelectedNode);
            }
        }
        #endregion
    }
}

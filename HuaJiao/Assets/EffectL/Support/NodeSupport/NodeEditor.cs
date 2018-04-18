using System.Collections.Generic;
using System.Text.RegularExpressions;



using UnityEngine;

namespace EffectL.Support.NodeSupport
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    public static class NodeEditor
    {
        private static Texture2D texture2D;
        public static NodeGraph curNodeGraph;
        public static NodeEditorState curNodeEditorState;
        public static NodeInputInfo CurNodeInputInfo;
        public static Node memoryNode;
        public static Stack<Node> selectNodes = new Stack<Node>();
        public static string Message = string.Empty;
        public static Rect Messagerect = new Rect(0, 0, 100, 20);

        public static void Refresh()
        {
            TriggerEditorUtility.Init();
            //if (EditorPrefs.HasKey("LastGraph"))
            //{
            //  Load(EditorPrefs.GetString("LastGraph"));
            //}
            //else
            //{
            //    curNodeGraph = null;
            //}
            curNodeGraph = null;
        }


        public static void DrawCanvas(Rect viewRect, GUISkin skin)
        {
            if (curNodeGraph == null)
                return;
            curNodeEditorState.CurGraphRect = viewRect;
            curNodeGraph.Draw();
            DrawLinkSelecting();
            DrawSelectedNode();
            NodeInputSystem.DynamicInvoke(CurNodeInputInfo);
        }
        public static void DrawLinkSelecting()
        {
            if (curNodeEditorState.IsLinkSetting)
            {
                Vector3 startPos = new Vector3(curNodeEditorState.SelectedKnob.rect.center.x,
                    curNodeEditorState.SelectedKnob.rect.center.y, 0);
                Vector3 endPos = new Vector3(CurNodeInputInfo.InputPos.x, CurNodeInputInfo.InputPos.y, 0);
                EffectUtility.DrawLine(startPos, endPos);
            }
        }

        static void DrawSelectedNode()
        {
            if (curNodeEditorState.SelectedNode == null)
            {
                return;
            }
            var nodeRect = curNodeEditorState.SelectedNode.nodeRect;
            EffectUtility.DrawLineA(Color.yellow, 
                nodeRect.position,
                nodeRect.position + new Vector2(nodeRect.size.x, 0),
                nodeRect.position + new Vector2(nodeRect.size.x, nodeRect.size.y),
                nodeRect.position + new Vector2(0, nodeRect.size.y),
                nodeRect.position
                );
        }
        #region ToolBar Func
        public static void Save()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(curNodeGraph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        public static void CreateNew()
        {
#if UNITY_EDITOR
            string path = EditorUtility.SaveFilePanel("Save graph", EditorDataMgr.RootPath,
                "defeault", "asset");
            path = Regex.Replace(path, @"^.+/Assets", "Assets");
            var graph = ScriptableObject.CreateInstance<NodeGraph>();
            AssetDatabase.CreateAsset(graph,path);
            graph.Name = Regex.Match(path, @"/.+?\.", RegexOptions.RightToLeft).Value.TrimStart('/').TrimEnd('.');
            graph.Type = "Logic";
            curNodeGraph = graph;
            graph.AddNode("Start", new Vector2(50f, 50f));
            graph.AddNode("Update", new Vector2(50f, 150f));
            curNodeEditorState = graph.curNodeState;
            CurNodeInputInfo=new NodeInputInfo("One",curNodeGraph);
            EditorPrefs.SetString("LastGraph", path);
#endif
        }
        public static void Load()
        {
#if UNITY_EDITOR
            string path = EditorUtility.OpenFilePanel("Open graph", EditorDataMgr.RootPath, "asset");
            path = Regex.Replace(path, @"^.+/Assets", "Assets");
            var graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(path);
            if(graph==null)
                return;
            curNodeEditorState = graph.curNodeState;
            curNodeGraph = graph;
            CurNodeInputInfo =new NodeInputInfo("Two", curNodeGraph);
            EditorPrefs.SetString("LastGraph", path);
#endif
        }
        public static void Load(string path)
        {
#if UNITY_EDITOR
            var graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(path);
            curNodeEditorState = graph.curNodeState;
            curNodeGraph = graph;
            CurNodeInputInfo = new NodeInputInfo("Two", curNodeGraph);
#endif
        }
        #endregion



        public static void Open(NodeGraph graph)
        {
#if UNITY_EDITOR
            TriggerEditorUtility.Init();
            Load(AssetDatabase.GetAssetPath(graph));
#endif
        }

    }
}

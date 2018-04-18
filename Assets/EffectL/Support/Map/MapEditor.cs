using System.Collections.Generic;
using System.Text.RegularExpressions;



using UnityEngine;

namespace EffectL.Support.NodeSupport
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    public static class MapEditor
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
            TriggerEditorUtility.MapInit();
            if (EditorPrefs.HasKey("LastMap"))
            {
                Load(EditorPrefs.GetString("LastMap"));
            }
            else
            {
                curNodeGraph = null;
            }
        }


        public static void DrawCanvas(Rect viewRect, GUISkin skin)
        {
            if (curNodeGraph == null)
                return;
            curNodeEditorState.CurGraphRect = viewRect;
            curNodeGraph.Draw();
            DrawLinkSelecting();
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
            string path = EditorUtility.SaveFilePanel("Save graph", Application.dataPath + "/EffectL/Output",
                "defeault", "asset");
            path = Regex.Replace(path, @"^.+/Assets", "Assets");
            var graph = ScriptableObject.CreateInstance<NodeGraph>();
            AssetDatabase.CreateAsset(graph, path);
            graph.Name = Regex.Match(path, @"/.+?\.", RegexOptions.RightToLeft).Value.TrimStart('/').TrimEnd('.');
            graph.Type = "Map";
            curNodeGraph = graph;
            curNodeEditorState = graph.curNodeState;
            CurNodeInputInfo = new NodeInputInfo("One", curNodeGraph);
            EditorPrefs.SetString("LastMap", path);
#endif
        }
        public static void Load()
        {
#if UNITY_EDITOR
            string path = EditorUtility.OpenFilePanel("Open graph", Application.dataPath + "/EffectL/Output", "asset");
            path = Regex.Replace(path, @"^.+/Assets", "Assets");
            var graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(path);
            if (graph == null)
                return;
            curNodeEditorState = graph.curNodeState;
            curNodeGraph = graph;
            CurNodeInputInfo = new NodeInputInfo("Two", curNodeGraph);
            EditorPrefs.SetString("LastMap", path);
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
            TriggerEditorUtility.MapInit();
            Load(AssetDatabase.GetAssetPath(graph));
#endif
        }

    }
}

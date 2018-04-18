#if UNITY_EDITOR
#endif
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    public static class NodeToolBarPanel
    {
        public static void DrawToolBar(Rect rect,GUISkin skin)
        {
#if UNITY_EDITOR

            GUILayout.BeginHorizontal();
            EffectUtility.FormatButton("", NodeEditor.Save,skin.GetStyle("toolbar-Save"));
            EffectUtility.FormatButton("", null, skin.GetStyle("toolbar-SaveAs"));
            EffectUtility.FormatButton("", NodeEditor.CreateNew, skin.GetStyle("toolbar-New"));
            EffectUtility.FormatButton("", NodeEditor.Load, skin.GetStyle("toolbar-Load"));
            //EffectUtility.FormatButton("Play", ()=> { EditorApplication.isPlaying = true; } , skin.GetStyle("ToolBarButton"));
            //EffectUtility.FormatButton("End", () => { EditorApplication.isPlaying = false; }, skin.GetStyle("ToolBarButton"));

            GUILayout.EndHorizontal();
#endif
        }
        public static void MapDrawToolBar(Rect rect, GUISkin skin)
        {
#if UNITY_EDITOR

            GUILayout.BeginHorizontal();
            EffectUtility.FormatButton("", MapEditor.Save, skin.GetStyle("toolbar-Save"));
            EffectUtility.FormatButton("", null, skin.GetStyle("toolbar-SaveAs"));
            EffectUtility.FormatButton("", MapEditor.CreateNew, skin.GetStyle("toolbar-New"));
            EffectUtility.FormatButton("", MapEditor.Load, skin.GetStyle("toolbar-Load"));
            EffectUtility.FormatButton("", MapGeneration.Generator, skin.GetStyle("toolbarMap-Generator"));
            //EffectUtility.FormatButton("Play", ()=> { EditorApplication.isPlaying = true; } , skin.GetStyle("ToolBarButton"));
            //EffectUtility.FormatButton("End", () => { EditorApplication.isPlaying = false; }, skin.GetStyle("ToolBarButton"));

            GUILayout.EndHorizontal();
#endif
        }
    }
}

using UnityEditor;
using effectL.Editor.Windows;
using UnityEditor.Callbacks;
using EffectL.Support.NodeSupport;

namespace EffectL.Editor.Windows
{
    public partial class OuMenu
    {
        [MenuItem("effectL/nodeEditor")]
        private static void Open()
        {
            EffectLWindow.Init();
        }
        [MenuItem("effectL/mapGenerator")]
        private static void OpenA()
        {
            MapGeneratorWindows.Init();
        }
        [OnOpenAsset(1)]
        public static bool AutoOpenLogic(int instanceId, int line)
        {
            if (AssetDatabase.Contains(instanceId))
            {
                string path = AssetDatabase.GetAssetPath(instanceId);
                var graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(path);
                if (graph != null&&graph.Type.Equals("Logic"))
                {
                    EffectLWindow.Init();
                    NodeEditor.Open(graph);
                    return true;
                }
            }
            return false;
        }
        [OnOpenAsset(2)]
        public static bool AutoOpenMap(int instanceId, int line)
        {
            if (AssetDatabase.Contains(instanceId))
            {
                string path = AssetDatabase.GetAssetPath(instanceId);
                var graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(path);
                if (graph != null && graph.Type.Equals("Map"))
                {
                    MapGeneratorWindows.Init();
                    MapEditor.Open(graph);
                    return true;
                }
            }
            return false;
        }
        //[OnOpenAsset(2)]
        //public static bool AutoOpenUnits(int instanceId, int line)
        //{
        //    if (AssetDatabase.Contains(instanceId))
        //    {
        //        string path = AssetDatabase.GetAssetPath(instanceId);
        //        if (AssetDatabase.LoadAssetAtPath<UnitBase>(path) != null)
        //        {
        //            UnitEditorWindows.Init();
        //            Support.UnitSupport.UnitEditor.Load(path);
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}

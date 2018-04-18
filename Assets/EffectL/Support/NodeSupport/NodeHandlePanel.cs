using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace EffectL.Support.NodeSupport
{
    public class NodeHandlePanel
    {
        public static Node target=null;
        public static List<FieldInfo> fields=new List<FieldInfo>();
        public static void Draw(Rect rect, GUISkin skin)
        {
#if UNITY_EDITOR
            if(NodeEditor.curNodeGraph==null)
                return;
            GameObject obj = Selection.activeGameObject;
            var tar = Selection.activeObject;
            if (tar!=null&& tar.GetType().IsSubclassOf(typeof(Node)))
            {
                DrawNodeHandle(tar as Node, skin);
            }
            else if (obj != null)
            {
                DrawBindProxy(obj,skin);
            }
#endif
        }

        public static void MapDraw(Rect rect, GUISkin skin)
        {
#if UNITY_EDITOR
            if (MapEditor.curNodeGraph == null)
                return;
            GameObject obj = Selection.activeGameObject;
            var tar = Selection.activeObject;
            if (tar != null && tar.GetType().IsSubclassOf(typeof(Cube)))
            {
                DrawCubeHandle(tar as Cube, skin);
            }
#endif
        }

        static void DrawCubeHandle(Cube cube, GUISkin skin)
        {
#if UNITY_EDITOR
            if (cube != target)
            {
                fields = NodeStack.NodeGetAsset(NodeStack.getDefaultNode(cube.GetId));
                target = cube;
                return;
            }
            EffectUtility.FormatLabel(cube.Title, skin.GetStyle("HandleLabel1"));
            EffectUtility.FormatLabel("节点名：", skin.GetStyle("HandleLabel2"));
            EffectUtility.FormatTextfield(ref cube.Title);
            EffectUtility.FormatButton("添加组件", () =>
            {
                cube.Component.Add(0);
            });
            for (int i = 0; i < cube.Component.Count; i++)
            {
                GUILayout.BeginHorizontal();
                cube.Component[i] = EffectUtility.FormatPopup(cube.Component[i],140f, NodeStack.MonoComponentsName.ToArray());
                int j = i;
                EffectUtility.FormatButton("除去", () =>
                {
                    cube.Component.RemoveAt(j);
                });
                GUILayout.EndHorizontal();
            }

            foreach (FieldInfo field in fields)
            {
                var value = field.GetValue(cube);
                EffectUtility.FormatLabel(field.Name, skin.GetStyle("HandleLabel2"));
                EffectUtility.FormatAssetField(ref value, field.FieldType);
                field.SetValue(cube, value);
            }
#endif
        }

        static void DrawNodeHandle(Node node, GUISkin skin)
        {
#if UNITY_EDITOR
            if (node != target)
            {
                fields = NodeStack.NodeGetAsset(NodeStack.getDefaultNode(node.GetId));
                target = node;
                return;
            }
            EffectUtility.FormatLabel(node.Title, skin.GetStyle("HandleLabel1"));
            EffectUtility.FormatLabel("节点名：", skin.GetStyle("HandleLabel2"));
            EffectUtility.FormatTextfield(ref node.Title);
            foreach (FieldInfo field in fields)
            {
                var value = field.GetValue(node);
                EffectUtility.FormatLabel(field.Name, skin.GetStyle("HandleLabel2"));
                EffectUtility.FormatAssetField(ref value, field.FieldType);
                field.SetValue(node, value);
            }
#endif
        }

        static void DrawBindProxy(GameObject obj,GUISkin skin)
        {
#if UNITY_EDITOR
            EffectUtility.FormatLabel(obj.name, skin.GetStyle("HandleLabel1"));
            if (obj.GetComponent<EffectLProxy>() == null)
            {
                EffectUtility.FormatButton("bind", () => { BindProxy(obj); });
            }
#endif
        }

        public static void BindProxy(GameObject obj)
        {
            var proxy = obj.AddComponent<EffectLProxy>();
            proxy.graph = NodeEditor.curNodeGraph;
        }
    }
}

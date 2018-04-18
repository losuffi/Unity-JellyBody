using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR

using UnityEditor;

#endif
namespace EffectL.Support.NodeSupport
{

    public static class EffectUtility
    {

        #region FuncLib
        public static string GetGameObjectPath(GameObject obj)
        {
            if (obj == null)
                return string.Empty;
            string Path = string.Empty;
            while (obj.transform.parent != null)
            {
                Path.Insert(0, "/" + obj.name);
                obj = obj.transform.parent.gameObject;
            }
            Path.Insert(0, obj.name);
            return Path;
        }
        public static string GetTransfromPath(Transform obj)
        {
            if (obj == null)
                return string.Empty;
            string Path = string.Empty;
            while (obj.parent != null)
            {
                Path = Path.Insert(0, "/" + obj.name);
                obj = obj.parent;
            }
            Path = Path.Insert(0, obj.name);

            return Path;
        }
        #endregion

        #region FuncNormal
        public static bool CheckRepaint(ref bool isDone)
        {
            if (Event.current.type == EventType.Repaint && !isDone)
            {
                return false;
            }
            if (Event.current.type == EventType.Layout && !isDone)
            {
                isDone = true;
            }
            return true;
        }
        public static GUISkin GetGUISkinStyle(string skinName)
        {
#if UNITY_EDITOR

            return EditorDataMgr.GetSkin(skinName);
#else
            return null;
#endif

        }

        public static Vector2 PanelScale(ref Rect rect, Vector2 zoomPivot, float zoom)
        {
            Rect screenRect;
            screenRect = rect;
            rect = Scale(screenRect, screenRect.position+zoomPivot, new Vector2(zoom, zoom));
            rect.position = Vector2.zero;
            Vector2 zoomPosAdjust = rect.center - screenRect.size / 2 + zoomPivot;
            return zoomPosAdjust;
        }

        public static Rect Scale(Rect rect, Vector2 pivot, Vector2 scale)
        {
            rect.position = Vector2.Scale(rect.position - pivot, scale) + pivot;
            rect.size = Vector2.Scale(rect.size, scale);
            return rect;
        }

        #endregion
        #region Low-Level UI

        public static Texture2D ColorToTex(int pxSize, Color col)
        {
            Texture2D tex = new Texture2D(pxSize, pxSize);
            for (int x = 0; x < pxSize; x++)
                for (int y = 0; y < pxSize; y++)
                    tex.SetPixel(x,y,col);
            tex.Apply();
            return tex;
        }

        public static Texture2D Tex(Color col)
        {
#if UNITY_EDITOR
            if (col == Color.blue)
            {
                return EditorDataMgr.GetTexture("C-Blue");
            }
            else if(col == Color.yellow)
            {
                return EditorDataMgr.GetTexture("C-Yellow");
            }
            else if (col == Color.red)
            {
                return EditorDataMgr.GetTexture("C-Red");
            }
            else if (col == Color.green)
            {
                return EditorDataMgr.GetTexture("C-Green");
            }
            else if (col == Color.white)
            {
                return EditorDataMgr.GetTexture("C-WhiteOut");
            }
            return ColorToTex(5, Color.clear);
#else
            return null;
#endif
        }

        #endregion

#region FunctionalUI


        public static void DrawLine(Vector3 startPos, Vector3 endPos)
        {
#if UNITY_EDITOR
            float distance = Vector3.Distance(startPos, endPos);
            Vector3 startTan = startPos + Vector3.right * distance/3;
            Vector3 endTan = endPos + Vector3.left * distance/3;
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 5);
#endif
        }
        public static void DrawLine(Vector3 startPos, Vector3 endPos,Color color)
        {
#if UNITY_EDITOR
            float distance = Vector3.Distance(startPos, endPos);
            Vector3 startTan = startPos + Vector3.right * distance / 3;
            Vector3 endTan = endPos + Vector3.left * distance / 3;
            var tempcolor = Handles.color;
            Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 5);
#endif
        }
        public static void DrawLineDirect(Vector3 startPos, Vector3 endPos, Color color)
        {
#if UNITY_EDITOR
            var tempcolor = Handles.color;
            Handles.color = color;
            Handles.DrawAAPolyLine(5,startPos, endPos);
            Handles.color = tempcolor;
#endif
        }
        public static void DrawLineA(Vector3 startPos, Vector3 endPos)
        {
#if UNITY_EDITOR
            Vector3[] Points=new Vector3[4];
            Points[0] = startPos;
            Points[3] = endPos;
            if (startPos.y < endPos.y)
            {
                Points[1] = new Vector3(startPos.x, startPos.y + 10);
                Points[2] = new Vector3(endPos.x, Points[1].y);
            }
            else
            {
                float xMins = startPos.x - endPos.x;
                Points[1] = new Vector3(startPos.x - 0.2f * xMins, startPos.y);
                Points[2] = new Vector3(Points[1].x, endPos.y);
            }
            Handles.DrawAAPolyLine(5,Points);
#endif
        }
        public static void DrawLineA(Color color,params Vector3[] Points)
        {
#if UNITY_EDITOR
            var tempcolor = Handles.color;
            Handles.color = color;
            Handles.DrawAAPolyLine(5, Points);
            Handles.color = tempcolor;
#endif
        }


        public static void FormatAssetField(ref object o)
        {
            string identity= ConnectionType.getTypeName(o.GetType());
            ConnectionType.types[identity].GUILayout(ref o);
        }
        public static void FormatAssetField(ref object o,Type type)
        {
            string identity = ConnectionType.getTypeName(type);
            ConnectionType.types[identity].GUILayout(ref o);
        }
        public static void FormatPopup(ref int index, params string[] strs)
        {
#if UNITY_EDITOR
            if (strs == null)
            {
                EditorGUILayout.Popup(index, new string[0],GUILayout.Width(60));
            }
            else
            {
                index = EditorGUILayout.Popup(index, strs, GUILayout.Width(60));
            }
#endif
        }
        public static int FormatPopup(int index, params string[] strs)
        {
#if UNITY_EDITOR
            if (strs == null)
            {
               return  EditorGUILayout.Popup(index, new string[0], GUILayout.Width(60));
            }
            else
            {
                return EditorGUILayout.Popup(index, strs, GUILayout.Width(60));
            }
#endif
        }
        public static int FormatPopup(int index,float width, params string[] strs)
        {
#if UNITY_EDITOR
            if (strs == null)
            {
                return EditorGUILayout.Popup(index, new string[0], GUILayout.Width(width));
            }
            else
            {
                return EditorGUILayout.Popup(index, strs, GUILayout.Width(width));
            }
#endif
        }
        public static void FormatLabel(string str)
        {
#if UNITY_EDITOR
            GUILayout.Label(str);
#endif
        }
        public static void FormatLabel(string str,GUIStyle style)
        {
#if UNITY_EDITOR
            GUILayout.Label(str,style);
#endif
        }

        public static void FormatLabel(Rect rect, string str)
        {
#if UNITY_EDITOR
            GUI.Label(rect,str);
#endif
        }
        public static void FormatLabel(Rect rect, string str, GUIStyle style)
        {
#if UNITY_EDITOR
            GUI.Label(rect, str,style);
#endif
        }
        public static void FormatTextfield(ref string str)
        {
#if UNITY_EDITOR
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("粘贴", GUILayout.Width(40)))
            {
                str = EditorGUIUtility.systemCopyBuffer;
            }
            str = GUILayout.TextField(str);
            GUILayout.EndHorizontal();
#endif
        }

        public static void FormatTextfield(ref string str, GUIStyle style)
        {
#if UNITY_EDITOR
            if (GUILayout.Button("粘贴", GUILayout.Width(40)))
            {
                str = EditorGUIUtility.systemCopyBuffer;
            }
            str = GUILayout.TextField(str,style);
#endif
        }

        public static string FormatTextfield(string str)
        {
#if UNITY_EDITOR
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("粘贴", GUILayout.Width(40)))
            {
                str = EditorGUIUtility.systemCopyBuffer;
            }
            str = GUILayout.TextField(str);
            GUILayout.EndHorizontal();
            return str;
#endif
        }


        public static void FormatIntfield(ref int val)
        {
#if UNITY_EDITOR
            val = EditorGUILayout.IntField(val);
#endif
        }
        public static void FormatTextArea(ref string str)
        {
#if UNITY_EDITOR
            if (GUILayout.Button("粘贴", GUILayout.Width(40)))
            {
                str = EditorGUIUtility.systemCopyBuffer;
            }
            str = GUILayout.TextArea(str);
#endif
        }

        public static void FormatButton(string label, Action method,GUIStyle style=null)
        {
#if UNITY_EDITOR
            if (style == null)
            {
                if (GUILayout.Button(label))
                {
                    method();
                }
            }
            else
            {
                if (GUILayout.Button(label, style))
                {
                    method();
                }
            }
#endif
        }
        public static void FormatButton(string label, Action method, Rect rect, GUIStyle style = null)
        {
#if UNITY_EDITOR
            if (style == null)
            {
                if (GUI.Button(rect, label))
                {
                    method();
                }
            }
            else
            {
                if (GUI.Button(rect, label, style))
                {
                    method();
                }
            }
#endif
        }
        public static void FormatButton(string label, Action method, Vector2 size, GUIStyle style = null)
        {
#if UNITY_EDITOR
            if (style == null)
            {
                if (GUILayout.Button(label, GUILayout.Width(size.x), GUILayout.Height(size.y)))
                {
                    method();
                }
            }
            else
            {
                if (GUILayout.Button(label, style, GUILayout.Width(size.x), GUILayout.Height(size.y)))
                {
                    method();
                }
            }
#endif
        }

        public static Vector3 FormatVector3(Vector3 v3)
        {
#if UNITY_EDITOR
            return EditorGUILayout.Vector3Field("", v3);
#endif
        }

        public static UnityEngine.Object FormatObject(UnityEngine.Object t,Type type)
        {
#if UNITY_EDITOR
            return EditorGUILayout.ObjectField(t, type,true);
#endif
        }
        public static bool FormatBool(bool tar)
        {
#if UNITY_EDITOR
            return EditorGUILayout.Toggle(tar);
#endif
        }
        public static int FormatInt(int tar)
        {
#if UNITY_EDITOR
            return EditorGUILayout.IntField(tar);
#endif
        }
        public static float FormatFloat(float tar)
        {
#if UNITY_EDITOR
            return EditorGUILayout.FloatField(tar);
#endif
        }
        public static double FormatDouble(double tar)
        {
#if UNITY_EDITOR
            return EditorGUILayout.DoubleField(tar);
#endif
        }
        public static Enum FormatEnum(Enum tar)
        {
#if UNITY_EDITOR
            return EditorGUILayout.EnumPopup(tar);
#endif
        }
        #endregion

        public static void RectConverting(ref Rect nodeRect, NodeEditorState state)
        {
            nodeRect.position *= state.GraphZoom;
            nodeRect.position += state.PanOffset + state.PanAdjust;
            nodeRect.size *= state.GraphZoom;
        }

        public static Vector2 VecConverting(Vector2 pos, NodeEditorState state)
        {
            return pos*state.GraphZoom+ state.PanOffset + state.PanAdjust;
        }

        public static Vector2 RectReConverting(Vector2 pos,NodeEditorState state)
        {
            var res = pos;
            res = (res - (state.PanOffset)-state.PanAdjust)/state.GraphZoom;
            return res;
        }
    }

    public static class TriggerEditorUtility
    {
        public static bool IsInit = false;
        public static bool IsLayout = false;
        public static event Action TrigInit; 

        public static Event e;
        public static void Init()
        {
            IsInit = false;
            EditorDataMgr.Init();
            NodeStack.FetchNode();
            NodeInputSystem.Fetch();
            ConnectionType.Fetch();
#if UNITY_EDITOR
#endif
            IsInit = true;
            if (TrigInit != null)
                TrigInit();
        }

        public static void MapInit()
        {
            IsInit = false;
            NodeStack.FetchCube();
            NodeInputSystem.Fetch();
            ConnectionType.Fetch();
#if UNITY_EDITOR
#endif
            IsInit = true;
            if (TrigInit != null)
                TrigInit();
        }

    }
}

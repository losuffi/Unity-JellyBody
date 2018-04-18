using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;

#endif
using UnityEngine;
using UnityEngine.UI;

namespace EffectL.Support.NodeSupport
{


    public static class ConnectionType
    {
        public static Dictionary<string, ConnectionTypeData> types;
        public static void Fetch()
        {
            types=new Dictionary<string, ConnectionTypeData>();
            foreach (Assembly scriptAssembly in AppDomain.CurrentDomain.GetAssemblies().Where(res=>res.FullName.Contains("Assembly-")))
            {
                foreach (Type type in scriptAssembly.GetTypes().Where(res=>!res.IsAbstract&&res.IsClass&&res.GetInterfaces().Contains(typeof(IConnectionDecorator))))
                {
                    IConnectionDecorator icd=scriptAssembly.CreateInstance(type.FullName) as IConnectionDecorator;
                    if (icd != null)
                    {
                        types.Add(icd.identity, new ConnectionTypeData(icd));
                    }
                }
            }
        }

        public static string getTypeName(Type type)
        {
            var t = types.Values.ToList().Find(res => res.type == type);
            if (t != null)
            {
                return t.identity;
            }
            else
            {
                if (type.BaseType != typeof(object))
                {
                    return getTypeName(type.BaseType);
                }
            }
            return string.Empty;
        }

        public static string[] identitys
        {
            get
            {
                string[] strs=new string[types.Count];
                int i = 0;
                foreach (string typesKey in types.Keys)
                {
                    if(types[typesKey].isGlobalVariable)
                        strs[i++] = typesKey;
                }
                return strs;
            }
        }

        public static string UnityUIObjectToString<T>(T obj) where T:Component
        {
            if (obj == null)
                return string.Empty;
            StringBuilder path = new StringBuilder();
            Transform vobj = obj.transform;
            if (vobj.parent == null)
                return "*noObj|" + vobj.name;
            while (true)
            {
                if (vobj.parent==null)
                {
                    path.Remove(0, 1);
                    path.Append("|" + vobj.name);
                    break;
                }
                else
                {
                    path.Insert(0, "/" + vobj.name);
                }
                vobj = vobj.parent;
            }
            return path.ToString();
        }

        public static string UnityUIObjectToString(GameObject obj)
        {
            if (obj == null)
                return string.Empty;
            StringBuilder path = new StringBuilder();
            Transform vobj = obj.transform;
            if (vobj.parent == null)
                return "*noObj|" + vobj.name;
            while (true)
            {
                if (vobj.parent == null)
                {
                    path.Remove(0, 1);
                    path.Append("|" + vobj.name);
                    break;
                }
                else
                {
                    path.Insert(0, "/" + vobj.name);
                }
                vobj = vobj.parent;
            }
            return path.ToString();
        }

        public static T UnityUIStringToObject<T>(string str) where T : Component
        {
            string[] strs = str.Split('|');
            var path = strs[0];
            var root = strs[1];
            if (path.Equals("*noObj"))
                return GameObject.Find(root).GetComponent<T>();
            var gobj = GameObject.Find(root);
            var target = gobj.transform.Find(path);
            if (target == null)
                return null;
            return target.GetComponent<T>();
        }
        public static GameObject UnityUIStringToObject(string str)
        {
            string[] strs = str.Split('|');
            var path = strs[0];
            var root = strs[1];
            if(path.Equals("*noObj"))
                return GameObject.Find(root);
            var gobj = GameObject.Find(root);
            var target = gobj.transform.Find(path);
            if (target == null)
                return null;
            return target.gameObject;
        }
    }

    public class ConnectionTypeData
    {
        public string identity;
        public Type type;
        public Color color;
        public bool isGlobalVariable;

        public delegate void DelLayout(ref object t);

        public DelLayout GUILayout;
        public Func<object,string> ObjtoString;
        public Func<string, object> StringtoObj;
        internal ConnectionTypeData(IConnectionDecorator icd)
        {
            this.identity = icd.identity;
            this.type = icd.type;
            this.color = icd.color;
            this.isGlobalVariable = icd.isGlobalType;
            this.GUILayout = icd.GUIFill;
            this.ObjtoString = icd.objTostring;
            this.StringtoObj = icd.stringtoobj;
        }
    }

    internal interface IConnectionDecorator
    {
        string identity { get; }
        Type type { get; }
        Color color { get; }
        bool isGlobalType { get; }
        void GUIFill(ref object obj);
        string objTostring(object obj);
        object stringtoobj(string str);
        string _Class { get; }
    }
    public class StringType : IConnectionDecorator
    {
        public string identity { get { return "字符串"; } }
        public Type type { get { return typeof(string); } }
        public Color color { get{return Color.cyan;} }
        public bool isGlobalType { get { return true; } }
        public string _Class { get { return "Normal"; } }

        public void GUIFill(ref object obj)
        {
#if UNITY_EDITOR
            if (obj==null||!obj.GetType().IsAssignableFrom(typeof(string)))
            {
                obj = string.Empty;
            }
            obj = EffectUtility.FormatTextfield((string)obj);
#endif
        }

        public string objTostring(object obj)
        {
            return obj.ToString();
        }

        public object stringtoobj(string str)
        {
            return str;
        }
    }
    public class IntValueType : IConnectionDecorator
    {
        public string identity { get { return "真值"; } }
        public Type type { get { return typeof(int); } }
        public Color color { get { return Color.cyan; } }
        public bool isGlobalType { get { return true; } }
        public string _Class { get { return "Normal"; } }

        public void GUIFill(ref object obj)
        {
#if UNITY_EDITOR
            if (obj == null || !obj.GetType().IsAssignableFrom(typeof(int)))
            {
                obj=new int();
                obj = 0;
            }
            obj = EditorGUILayout.IntField((int) obj);
#endif
        }

        public string objTostring(object obj)
        {
            return obj.ToString();
        }

        public object stringtoobj(string str)
        {
            int res;

            if (int.TryParse(str, out res))
            {
                return res;
            }
            return null;
        }
    }
    public class RealValueType : IConnectionDecorator
    {
        public string identity { get { return "实值"; } }
        public Type type { get { return typeof(float); } }
        public Color color { get { return Color.cyan; } }
        public bool isGlobalType { get { return true; } }
        public string _Class { get { return "Normal"; } }

        public void GUIFill(ref object obj)
        {
#if UNITY_EDITOR
            if (obj == null || !obj.GetType().IsAssignableFrom(typeof(float)))
            {
                obj = new float();
                obj = 0.1f;
            }
            obj = EditorGUILayout.FloatField((float)obj);
#endif
        }

        public string objTostring(object obj)
        {
            return obj.ToString();
        }

        public object stringtoobj(string str)
        {
            float res;
            if (float.TryParse(str, out res))
            {
                return res;
            }
            return null;
        }
    }
    public class BoolType : IConnectionDecorator
    {
        public string identity { get { return "布尔"; } }
        public Type type { get { return typeof(bool); } }
        public Color color { get { return Color.cyan; } }
        public bool isGlobalType { get { return true; } }
        public string _Class { get { return "Normal"; } }

        public void GUIFill(ref object obj)
        {
#if UNITY_EDITOR
            if (obj == null || !obj.GetType().IsAssignableFrom(typeof(bool)))
            {
                obj = false;
            }
            obj = EffectUtility.FormatBool((bool) obj);
#endif
        }

        public string objTostring(object obj)
        {
            return obj.ToString();
        }

        public object stringtoobj(string str)
        {
            float res;
            if (float.TryParse(str, out res))
            {
                return res;
            }
            return null;
        }
    }

    public class CommandType : IConnectionDecorator
    {
        public string identity { get { return "命令"; } }
        public Type type { get { return typeof(EffectLCommand); } }
        public Color color { get { return Color.cyan; } }
        public bool isGlobalType { get { return true; } }
        public string _Class { get { return "Normal"; } }

        public void GUIFill(ref object obj)
        {
#if UNITY_EDITOR
            if (obj == null || !obj.GetType().IsAssignableFrom(typeof(EffectLCommand)))
            {
                obj = new EffectLCommand(string.Empty);
                obj = 0.1f;
            }
            EffectUtility.FormatTextArea(ref (obj as EffectLCommand).Content);
#endif
        }

        public string objTostring(object obj)
        {
            return obj.ToString();
        }

        public object stringtoobj(string str)
        {
            return null;
        }
    }
    public class PopupType : IConnectionDecorator
    {
        public string identity { get { return "多选一"; } }
        public Type type { get { return typeof(EffectLPopup); } }
        public Color color { get { return Color.cyan; } }
        public bool isGlobalType { get { return true; } }
        public string _Class { get { return "Normal"; } }

        public void GUIFill(ref object obj)
        {
#if UNITY_EDITOR
            if (obj == null || !obj.GetType().IsAssignableFrom(typeof(EffectLPopup)))
            {
                obj = new EffectLPopup(0);
            }
            EffectUtility.FormatPopup(ref (obj as EffectLPopup).index, (obj as EffectLPopup).popup);
#endif
        }

        public string objTostring(object obj)
        {
            return obj.ToString();
        }

        public object stringtoobj(string str)
        {
            return null;
        }
    }

    public class NumericType : IConnectionDecorator
    {
        public string identity { get { return "数字"; } }
        public Type type { get { return typeof(EffectLNumeric); } }
        public Color color { get { return Color.cyan; } }
        public bool isGlobalType { get { return true; } }
        public string _Class { get { return "Normal"; } }

        public void GUIFill(ref object obj)
        {
#if UNITY_EDITOR
            if (obj == null || !obj.GetType().IsAssignableFrom(typeof(EffectLNumeric)))
            {
                obj=new EffectLNumeric();
            }
            (obj as EffectLNumeric).type =
                (EffectLNumeric.NumericType) EffectUtility.FormatEnum((obj as EffectLNumeric).type);
            switch ((obj as EffectLNumeric).type)
            {
                case EffectLNumeric.NumericType.真值:
                    (obj as EffectLNumeric).SetVal(EffectUtility.FormatInt((int)(obj as EffectLNumeric).Val()));
                    break;
                case EffectLNumeric.NumericType.实值:
                    (obj as EffectLNumeric).SetVal(EffectUtility.FormatFloat((float)(obj as EffectLNumeric).Val()));
                    break;
                case EffectLNumeric.NumericType.Double:
                    (obj as EffectLNumeric).SetVal(EffectUtility.FormatDouble((double)(obj as EffectLNumeric).Val()));
                    break;
            }
#endif
        }

        public string objTostring(object obj)
        {
            return obj.ToString();
        }

        public object stringtoobj(string str)
        {
            return null;
        }
    }
    public class EnumType:IConnectionDecorator
    {
        public string identity { get { return "枚举"; } }
        public Type type { get { return typeof(Enum); } }
        public Color color { get { return Color.cyan; } }
        public bool isGlobalType { get { return true; } }
        public string _Class { get { return "Normal"; } }

        public void GUIFill(ref object obj)
        {
#if UNITY_EDITOR
            obj =  EffectUtility.FormatEnum((Enum) obj);
#endif
        }

        public string objTostring(object obj)
        {
            return obj.ToString();
        }

        public object stringtoobj(string str)
        {
            return null;
        }
    }

    public class Vector3Type: IConnectionDecorator
    {
        public string identity { get { return "Vector3"; } }
        public Type type { get { return typeof(Vector3); } }
        public Color color { get { return Color.cyan; } }
        public bool isGlobalType { get { return true; } }
        public string _Class { get { return "Normal"; } }
        public void GUIFill(ref object obj)
        {
#if UNITY_EDITOR
            if (obj == null || !obj.GetType().IsAssignableFrom(typeof(Vector3)))
            {
                obj = Vector3.zero;
            }
            obj = EffectUtility.FormatVector3((Vector3) obj);
#endif
        }
        public string objTostring(object obj)
        {
            return obj.ToString();
        }

        public object stringtoobj(string str)
        {
            return null;
        }
    }
    public class TransformType : IConnectionDecorator
    {
        public string identity { get { return "Transform"; } }
        public Type type { get { return typeof(EffectLTransformObject); } }
        public Color color { get { return Color.cyan; } }
        public bool isGlobalType { get { return true; } }
        public string _Class { get { return "Normal"; } }
        public void GUIFill(ref object obj)
        {
#if UNITY_EDITOR
            (obj as EffectLTransformObject).value =
                (Transform) EffectUtility.FormatObject((obj as EffectLTransformObject).value, typeof(Transform));
#endif
        }

        public string objTostring(object obj)
        {
            return obj.ToString();
        }

        public object stringtoobj(string str)
        {
            return null;
        }
    }
}

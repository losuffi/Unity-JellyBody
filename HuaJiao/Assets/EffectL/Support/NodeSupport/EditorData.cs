using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    public class EditorData : ScriptableObject
    {
        private  const string Version = "2.0f";
        [Header("Skin集合")]
        public List<GUISkin> Skins;

        [Header("Texture2D集合")] public List<Texture2D> pics;

        [Header("程序集特征名称：")] public string Assmbly_ContainName;
        [Header("工具版本号：" + Version)] public string Diaglog;
    }
    public static class EditorDataMgr
    {
        private static EditorData curData;

        public static EditorData DATA
        {
            get
            {
                if (curData == null)
                {
                    var strOnly= AssetDatabase.FindAssets("effectl-initialization-data");
                    if (strOnly.Length <= 0)
                    {
                        Debug.LogError("初始配置文件丢失！已生成空配置文件，请先进行配置");
                        Debug.LogError("目标路径："+Application.dataPath+ "/effectl-initialization-data.asset");
                        curData = ScriptableObject.CreateInstance<EditorData>();
                        AssetDatabase.CreateAsset(curData, @"Assets/effectl-initialization-data.asset");
                        EditorUtility.SetDirty(curData);
                        AssetDatabase.SaveAssets();
                        return curData;
                    }
                    return AssetDatabase.LoadAssetAtPath<EditorData>(AssetDatabase.GUIDToAssetPath(strOnly[0]));
                }

                return curData;
            }
        }

        public static void ShowName()
        {
            foreach (GUISkin dataSkin in DATA.Skins)
            {
                Debug.Log(dataSkin.name);
            }
        }

        public static string RootPath
        {
            get { return AssetDatabase.GetAssetPath(DATA); }
        }

        public static GUISkin GetSkin(string name)
        {
            return DATA.Skins.Find(res => res.name.Equals(name));
        }

        public static Texture2D GetTexture(string name)
        {
            return DATA.pics.Find(res => res.name.Equals(name));
        }

        public static void Init()
        {
            curData = DATA;
        }
    }
}
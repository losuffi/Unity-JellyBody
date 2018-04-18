using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    [Node(false,"Transform-向量参数","Transform", NodeType.Handle)]
    public class TransformProperty:Node
    {
        [EffectLDisplayHandleAssets] public EffectLTransformObject target;
        [EffectLDisplayHandleAssets] public EffectLPopup type;
        [EffectLTransitAsset] public Vector3 setting;
        void Init()
        {
            var t = NodeStack.getProperty("Transform",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance,
                T => T.PropertyType == typeof(Vector3));
            string[] name=new string[t.Count];
            int index = 0;
            foreach (PropertyInfo info in t)
            {
                name[index] = info.Name;
                index++;
            }
            index = 0;
            type = new EffectLPopup(index, name);
        }

        Vector3 Evaluate()
        {
            var t = NodeStack.getProperty("Transform",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance,
                T => T.PropertyType == typeof(Vector3));
            return (Vector3) t[type.index].GetValue(target.value, null);
        }

        void Work()
        {
            var t = NodeStack.getProperty("Transform",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance,
                T => T.PropertyType == typeof(Vector3));
            t[type.index].SetValue(target.value, setting, null);
        }
    }
}

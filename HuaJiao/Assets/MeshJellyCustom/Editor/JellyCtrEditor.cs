using UnityEngine;
using UnityEditor;
namespace HuaJiao.JellyMiscNs {
    [CustomEditor(typeof(JellyAgentCtr))]
    public class JellyCtrEditor : Editor
    {
        private JellyAgentCtr ctr;
        private float radius;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (ctr == null)
            {
                ctr = (JellyAgentCtr)target;
            }
            radius = EditorGUILayout.FloatField(radius);
            if (GUILayout.Button("生成Agent"))
            {

            }
        }
    }
}
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    public static class NodeSelectPanel
    {
        public static string selectedNode = string.Empty;
        public static string selectedClan = string.Empty;
        static Vector2 scrollPos=Vector2.zero;
        public static void Draw(Rect vrect,GUISkin skin)
        {
#if UNITY_EDITOR
            if(NodeEditor.curNodeGraph==null)
                return;
            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (var clan in NodeStack.clans.Keys)
            {
                EffectUtility.FormatButton(clan, () => { SelectedClan(clan); },
                    selectedClan.Equals(clan) ? skin.GetStyle("ClanButtonDown") : skin.GetStyle("ClanButton"));
                if (selectedClan.Equals(clan))
                {
                    foreach (var node in NodeStack.clans[selectedClan].clanNode) 
                    {
                        EffectUtility.FormatButton(node.GetId, () => { SelectedNode(node.GetId); },
                            selectedNode.Equals(node.GetId)
                                ? skin.GetStyle("NodeButtonDown")
                                : skin.GetStyle("NodeButtonUp"));
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

#endif
        }

        public static void MapDraw(Rect vrect, GUISkin skin)
        {
#if UNITY_EDITOR
            if (MapEditor.curNodeGraph == null)
                return;
            GUILayout.BeginVertical();
            foreach (var clan in NodeStack.clans.Keys)
            {
                EffectUtility.FormatButton(clan, () => { SelectedClan(clan); },
                    selectedClan.Equals(clan) ? skin.GetStyle("ClanButtonDown") : skin.GetStyle("ClanButton"));
                if (selectedClan.Equals(clan))
                {
                    foreach (var node in NodeStack.clans[selectedClan].clanNode)
                    {
                        EffectUtility.FormatButton(node.GetId, () => { SelectedNode(node.GetId); },
                            selectedNode.Equals(node.GetId)
                                ? skin.GetStyle("NodeButtonDown")
                                : skin.GetStyle("NodeButtonUp"));
                    }
                }
            }
            GUILayout.EndVertical();
#endif
        }


        static void SelectedClan(string clanName)
        {
            if (clanName.Equals(selectedClan))
            {
                selectedClan = string.Empty;
                selectedNode = string.Empty;
            }
            else
            {
                selectedClan = clanName;
            }
        }

        static void SelectedNode(string nodeName)
        {
            if (nodeName.Equals(selectedNode))
            {
                selectedNode = string.Empty;
            }
            else
            {
                selectedNode = nodeName;
            }
        }
    }
}

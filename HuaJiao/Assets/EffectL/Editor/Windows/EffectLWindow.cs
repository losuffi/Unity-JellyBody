using System;
using effectL.Editor.EditorLib;
using EffectL.Support.NodeSupport;
using UnityEditor;
using UnityEngine;
namespace effectL.Editor.Windows
{
    public class EffectLWindow : EditorWindow
    {

        //Views:
        private ViewPanel toolbarView, nodeGraphView, nodeHandleView,nodeTypeView;

        public static EffectLWindow Instance;
        private bool IsDone=false;
        public static void Init()
        {
            Instance = GetWindow<EffectLWindow>(false);
            Instance.titleContent = new GUIContent("effectL");
            Instance.maxSize = new Vector2(1980, 660);
            Instance.minSize = new Vector2(1400, 460);
        }

        private void OnEnable()
        {
            NodeEditor.Refresh();
            IsDone = false;
        }
        private void OnGUI()
        {
            if (!CheckView())
            {
                return;
            }
            if (EffectUtility.CheckRepaint(ref IsDone))
            {
                try
                {
                    DrawViews();
                }
                catch (ArgumentException e)
                {
                    Repaint();
                }
                Repaint();
            }
        }

        private void DrawViews()
        {
            toolbarView.UpdateView(
                new Rect(position.size, position.size),
                new Rect(0,0,1,.049f), 
                "toolbarView"
                );
            nodeGraphView.UpdateView(
                new Rect(position.size, position.size),
                new Rect(.201f, .05f, .598f, .95f),
                "nodeGraphView"
            );
            nodeHandleView.UpdateView(
                new Rect(position.size, position.size),
                new Rect(0, .05f, .2f, .95f),
                "nodeHandleView"
            );
            nodeTypeView.UpdateView(
                new Rect(position.size, position.size),
                new Rect(.8f, .05f, .2f, .95f),
                "nodeTypeView"
            );
        }
        bool CheckView()
        {
            if (Instance == null)
            {
                Init();
                return false;
            }
            if (toolbarView == null)
            {
                toolbarView=new ViewPanel("tool bar",NodeToolBarPanel.DrawToolBar);
            }
            if (nodeHandleView == null)
            {
                nodeHandleView=new ViewPanel("handle",NodeHandlePanel.Draw);
            }
            if (nodeGraphView == null)
            {
                nodeGraphView=new ViewPanel("graph",NodeEditor.DrawCanvas);
            }
            if (nodeTypeView == null)
            {
                nodeTypeView = new ViewPanel("type", NodeSelectPanel.Draw);
            }
            return true;
        }
    }
}

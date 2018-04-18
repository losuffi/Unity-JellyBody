using System;
using EffectL.Support.NodeSupport;
using UnityEditor;
using UnityEngine;

namespace effectL.Editor.EditorLib
{
    public class ViewPanel
    {
        public string Title;
        public Rect ViewRect;
        protected GUISkin ViewSkin;
        private Action<Rect,GUISkin> OnDraw;
        public ViewPanel()
        {
            Title = String.Empty;
            ViewRect = new Rect();
            GetGUISkin();
        }

        private void GetGUISkin()
        {
            ViewSkin = EditorDataMgr.GetSkin("effectL Skin");
        }

        public ViewPanel(string title, Action<Rect, GUISkin> content)
        {
            Title = title;
            ViewRect = new Rect();
            GetGUISkin();
            OnDraw = content;
        }

        public ViewPanel(string title, Rect viewRect)
        {
            Title = title;
            ViewRect = viewRect;
            GetGUISkin();
        }

        protected internal virtual void UpdateView(Rect size, Rect percentageSize, string style)
        {
            if (ViewSkin == null)
                GetGUISkin();
            ViewRect = new Rect(size.x * percentageSize.x,
                size.y * percentageSize.y,
                size.width * percentageSize.width,
                size.height * percentageSize.height);
            drawView(style);
        }
        protected virtual void drawView(string style)
        {
            GUI.Box(ViewRect, Title, ViewSkin.GetStyle(style));
            using (var area=new ClipArea(ViewRect))
            {
                if (OnDraw != null)
                {
                    OnDraw(ViewRect, ViewSkin);
                }
            }

        }

    }
}

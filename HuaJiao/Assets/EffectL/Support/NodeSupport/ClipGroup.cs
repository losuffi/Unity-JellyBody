using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace EffectL.Support.NodeSupport
{
    public class ClipGroup : IDisposable
    {
        private bool dispoed = false;

        public ClipGroup(Rect area)
        {
            GUI.BeginGroup(area);
        }

        public ClipGroup(Rect area, GUIStyle style)
        {
            GUI.BeginGroup(area, style);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.dispoed)
            {
                if (disposing)
                    GUI.EndGroup();
                dispoed = true;
            }
        }
        ~ClipGroup() { Dispose(false); }
    }
}

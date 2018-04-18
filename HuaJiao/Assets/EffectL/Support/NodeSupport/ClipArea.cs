using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    public class ClipArea:IDisposable
    {
        private bool dispoed = false;

        public ClipArea(Rect area)
        {
            GUILayout.BeginArea(area);
        }

        public ClipArea(Rect area, GUIStyle style)
        {
            GUILayout.BeginArea(area,style);
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
                if(disposing)
                    GUILayout.EndArea();
                dispoed = true;
            }
        }
        ~ClipArea() { Dispose(false);}
    }
}

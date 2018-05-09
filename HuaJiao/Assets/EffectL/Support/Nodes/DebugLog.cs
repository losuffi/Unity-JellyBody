using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    [Node(false,"打印Log", "Calculate", NodeType.Handle)]
    public class DebugLog:Node
    {
        [EffectLTransitAsset] public string LogText;

        void Work()
        {
            Debug.Log(LogText);
        }
    }
}

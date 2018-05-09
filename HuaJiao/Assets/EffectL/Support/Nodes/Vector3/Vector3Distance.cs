using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    [Node(false, "坐标距离", "Vector", NodeType.Calc)]
    public class Vector3Distance : Node
    {
        public enum Direct
        {
            x,
            y,
            z,
        }
        [EffectLTransitAsset] public Vector3 A;
        [EffectLTransitAsset] public Vector3 B;
        float Evaluate()
        {
            return Vector3.Distance(A, B);
        }

    }
}

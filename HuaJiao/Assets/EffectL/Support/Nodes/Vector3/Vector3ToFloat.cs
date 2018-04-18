using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    [Node(false,"向量模读取","Vector", NodeType.Calc)]
    public class Vector3ToFloat:Node
    {
        public enum Direct
        {
            x,
            y,
            z,
        }
        [EffectLTransitAsset] public Vector3 tar;
        [EffectLDisplayHandleAssets] public Direct direct;

        float Evaluate()
        {
            if (direct == Direct.x)
                return tar.x;
            else if (direct == Direct.y)
                return tar.y;
            else
            {
                return tar.z;
            }
        }

    }
}

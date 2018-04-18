using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    [Node(false,"移动","Transform", NodeType.Handle)]
    public class TransformMove:Node
    {
        [EffectLTransitAsset] public float Speed;
        [EffectLTransitAsset] public Vector3 Direct;
        [EffectLDisplayHandleAssets]public EffectLTransformObject target;
        void Work()
        {
            target.value.position += Direct * Speed * Time.deltaTime;
        }
    }
}

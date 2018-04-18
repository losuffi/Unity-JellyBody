using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EffectL.Support.NodeSupport;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    [Node(false, "字符拼接", "Calculate",NodeType.Calc)]
    public class StringAdd : Node
    {
        [EffectLTransitAsset]
        public string Text;

        [EffectLTransitAsset] public string TextB;
        string Evaluate()
        {
            return (Text + TextB);
        }

    }
}

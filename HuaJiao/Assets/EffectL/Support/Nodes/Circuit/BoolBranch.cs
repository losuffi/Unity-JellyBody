using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    [Node(false,"布尔分支","Circuit", NodeType.Branch)]
    public class BoolBranch:Node
    {
        [EffectLTransitAsset] public bool Res;

        [SerializeField]
        [HideInInspector]
        private List<NodeFlowOutKnob> output;

        void Init()
        {
            output=new List<NodeFlowOutKnob>();
            output.Add(CreateFlowOutKnob("真",30));
            output.Add(CreateFlowOutKnob("假",60));
        }

        void Work()
        {
            if (Res)
            {
                FlowOut = output[0];
            }
            else
            {
                FlowOut = output[1];
            }
        }
    }
}

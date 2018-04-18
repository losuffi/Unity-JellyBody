using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    public class EffectLProxy:MonoBehaviour
    {
        public NodeGraph graph;
        void Awake()
        {
            if (!TriggerEditorUtility.IsInit)
            {
                TriggerEditorUtility.Init();
            }
        }

        void Start()
        {
            foreach (Node node in graph.nodes.Where(res => res.GetId.Equals("Start")))
            {
                NodeStack.Run(node);
            }
        }

        void Update()
        {
            foreach (Node node in graph.nodes.Where(res => res.GetId.Equals("Update")))
            {
                NodeStack.Run(node);
            }
            foreach (Node node in graph.nodes.Where(res=>res.Clan.Equals("Event")))
            {
                NodeStack.SingleRun(node);
            }
        }
    }
}

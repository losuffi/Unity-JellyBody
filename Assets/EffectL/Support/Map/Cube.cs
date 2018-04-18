using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace EffectL.Support.NodeSupport
{
    [Node(true,"Cube","Map",NodeType.Event,120,120)]
     public   class Cube:Node
    {
        [HideInInspector]
        [SerializeField] internal List<NodeDirectKnob> Direct=new List<NodeDirectKnob>();
        [HideInInspector]
        public List<int> Component = new List<int>();

        internal NodeDirectKnob CreateDirect(string name, Side side)
        {
            return NodeDirectKnob.Create(this, name, side);
        }


        protected internal override void DrawKnob()
        {
            foreach (NodeDirectKnob knob in Direct)
            {
                knob.Draw(this);
            }
        }

        protected internal override void DrawConnections()
        {
            foreach (NodeDirectKnob knob in Direct)
            {
                knob.DrawConnection();
            }
        }

        protected internal override void Remove()
        {
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(this);
#endif
            foreach (var knob in Direct)
            {
                knob.Remove();
            }

        }
    }
}

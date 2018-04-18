using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace EffectL.Support.NodeSupport
{
    public class Node:ScriptableObject
    {
        [HideInInspector]
        public string GetId;

        [HideInInspector]
        public string Clan;
        [SerializeField]
        [HideInInspector]
        protected internal GUISkin NodeSkin;
        public string Title;
        [HideInInspector]
        public Rect rect=new Rect();
        [HideInInspector]
        public Rect nodeRect;
        [HideInInspector]
        public NodeGraph curGraph;
        [HideInInspector]
        internal List<NodeKnob> Knobs = new List<NodeKnob>();
        [SerializeField]
        [HideInInspector]
        internal List<NodeInput> inputKnobs=new List<NodeInput>();
        [SerializeField]
        [HideInInspector]
        internal List<NodeOutput> outputKnobs=new List<NodeOutput>();

        [HideInInspector] [SerializeField] internal NodeFlowInKnob FlowIn;
        [HideInInspector] [SerializeField] internal NodeFlowOutKnob FlowOut;

        public bool isNoneUsefulNode
        {
            get
            {
                foreach (NodeInput input in inputKnobs)
                {
                    if (input.connection != null)
                        return false;
                }
                return true;
            }
        }


        protected virtual void Init(Vector2 pos, Node protoType)
        {
            NodeSkin = EffectUtility.GetGUISkinStyle("NormalSkin");
            var data = NodeStack.nodes[protoType];
            rect=new Rect(pos,data.Size);
            Title = data.Name;
            GetId = data.Name;
            Clan = protoType.Clan;
            NodeStack.NodeInit(this);
        }

        public static Node CreateNode(Vector2 pos,string nodeId,NodeGraph graph)
        {
#if UNITY_EDITOR

            Node node = NodeStack.getDefaultNode(nodeId);
            if (node == null)
            {
                Debug.Log("node editor running is warning!");
            }
            var npos = EffectUtility.RectReConverting(pos,graph.curNodeState);
            var nodeObject = node.Create();
            AssetDatabase.AddObjectToAsset(nodeObject, graph);
            Undo.RecordObject(nodeObject, "Add");
            nodeObject.curGraph = graph;
            nodeObject.Init(npos,node);
            return nodeObject;
#else
            return null;
#endif
        }
        protected internal virtual void Draw()
        {
#if UNITY_EDITOR
            if (NodeSkin==null)
                return;
            CheckKnobMigration();
            nodeRect = rect;
            EffectUtility.RectConverting(ref nodeRect,curGraph.curNodeState);
            Vector2 contentOffset = new Vector2(0, 15 * curGraph.curNodeState.GraphZoom);
            Rect nodeHead = new Rect(0, 0, nodeRect.width, contentOffset.y);
            Rect nodeBody = new Rect(nodeRect.x, nodeRect.y, nodeRect.width,
                nodeRect.height);
            using (var group=new ClipGroup(nodeBody))
            {
                nodeBody.position = Vector2.zero;
                using (var area = new ClipArea(nodeBody,NodeSkin.GetStyle("nodeBody")))
                {
                    var style = NodeSkin.GetStyle("nodeHead");
                    style.fontSize = (int)(10 * curGraph.curNodeState.GraphZoom);
                    GUI.Label(nodeHead, Title, style);
                    GUILayout.Space(contentOffset.y);
                    NodeGUI();
                }
            }
            //GUILayout.BeginArea(nodeBody, curGraph.curNodeState.SelectedNode == this
            //    ? NodeSkin.GetStyle("nodeBodySelected")
            //    : NodeSkin.GetStyle("nodeBody"));

           // GUILayout.EndArea();
            DrawKnob();
            DrawConnections();

#endif
        }

        protected internal virtual void DrawKnob()
        {
            CheckKnobMigration();
            foreach (var res in Knobs)
            {
                res.Draw(this);
            }
        }

        protected internal virtual void DrawConnections()
        {
            CheckKnobMigration();
            foreach (NodeInput input in inputKnobs)
            {
                input.DrawConnection();
            }
            if(FlowIn!=null)
                FlowIn.DrawConnection();
        }

        internal void CheckKnobMigration()
        {
            if (!Knobs.Any())
            {
                if (inputKnobs.Any() || outputKnobs.Any() || FlowIn != null || FlowOut != null)
                {
                    Knobs.AddRange(inputKnobs.Cast<NodeKnob>());
                    Knobs.AddRange(outputKnobs.Cast<NodeKnob>());
                    if(FlowIn != null)
                        Knobs.Add(FlowIn);
                    if(FlowOut!=null)
                        Knobs.Add(FlowOut);
                }
            }
        }

        protected internal virtual void NodeGUI()
        {
        }



        public virtual Node Create()
        {
#if UNITY_EDITOR
            var node= CreateInstance(GetType().Name) as Node;
            Undo.RegisterCreatedObjectUndo(node,"Create Node");
            return node;
#else
            return null;
#endif

        }


        #region Knops

        public NodeFlowInKnob CreateFlowInKnob(string cotName = "入",float offset=20)
        {
            return NodeFlowInKnob.Create(this,cotName,offset);
        }

        public NodeFlowOutKnob CreateFlowOutKnob(string cotName="出",float offset=20)
        {
            return NodeFlowOutKnob.Create(this, cotName,offset);
        }

        public NodeInput CreateNodeInput(string inputName, string inputType)
        {
            return NodeInput.Create(this, inputName, inputType);
        }
        public NodeInput CreateNodeInput(string inputName, string inputType,Side sd)
        {
            return NodeInput.Create(this, inputName, inputType, sd);
        }
        public NodeInput CreateNodeInput(string inputName, string inputType,Side sd,float offset)
        {
            return NodeInput.Create(this, inputName, inputType, sd,offset);
        }
        public NodeInput CreateNodeInput(string inputName, string inputType, Side sd, float offset,string fieldName)
        {
            return NodeInput.Create(this, inputName, inputType, sd, offset,fieldName);
        }
        public NodeOutput CreateNodeOutput(string outputName, string outputType)
        {
            return NodeOutput.Create(this, outputName, outputType);
        }
        public NodeOutput CreateNodeOutput(string outputName, string outputType, Side sd)
        {
            return NodeOutput.Create(this, outputName, outputType,sd);
        }
        public NodeOutput CreateNodeOutput(string outputName, string outputType, Side sd, float offset)
        {
            return NodeOutput.Create(this, outputName, outputType, sd, offset);
        }
        public NodeOutput CreateNodeOutput(string outputName, string outputType, Side sd, float offset, string fieldName)
        {
            return NodeOutput.Create(this, outputName, outputType, sd, offset, fieldName);
        }
        #endregion

        protected internal virtual void Remove()
        {
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(this);
            foreach (NodeInput input in inputKnobs)
            {
                input.Remove();
            }
            foreach (NodeOutput output in outputKnobs)
            {
                output.Remove();
            }
            if(FlowIn != null)
                FlowIn.Remove();
            if(FlowOut!=null)
                FlowOut.Remove();
            //DestroyImmediate(this,true);
#endif
        }
    }
}

#if UNITY_EDITOR
   
using UnityEditor;

#endif
using UnityEngine;

namespace EffectL.Support.NodeSupport { 
    public static class NodeInputControls
    {
#if UNITY_EDITOR
        #region WindowPanel
        [Handle(EventType.MouseDrag, 10)]
        private static void HandleWindowPineDrag(NodeInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.EdState;
            if (inputInfo.InputPos.x < 0 || inputInfo.InputPos.y < 0||state.SelectedNode!=null||state.SelectedKnob!=null)
            {
                return;
            }
            if (state.IsPineSetting)
            {
                Vector2 panChangeDragOffset = state.DragOffset;
                state.DragOffset = inputInfo.InputPos - state.DragStart;
                panChangeDragOffset = (state.DragOffset - panChangeDragOffset);
                state.PanOffset += (panChangeDragOffset*inputInfo.EdState.GraphZoom);
            }
        }

        [Handle(EventType.MouseDown)]
        private static void HandleWindowPineDown(NodeInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.EdState;
            if (state.FocusNode != null&&state.FocusKnob!=null||inputInfo.InputEvent.button!=0)
            {
                return;
            }
            else
            {
                state.SelectedNode = null;
            }
            if (!state.IsPineSetting)
            {
                state.IsPineSetting = true;
                state.DragStart = inputInfo.InputPos;
                state.DragOffset = Vector2.zero;

            }
            else
            {
                state.DragStart = inputInfo.InputPos;
                state.DragOffset = Vector2.zero;
            }

        }

        [Handle(EventType.MouseUp)]
        private static void HandleWindowPineUp(NodeInputInfo inputInfo)
        {
            if (inputInfo.InputPos.x < 0 || inputInfo.InputPos.y < 0)
            {
                return;
            }
            NodeEditorState state = inputInfo.EdState;
            state.IsPineSetting = false;
        }
        [Handle(EventType.scrollWheel)]
        private static void HandleWindowScroll(NodeInputInfo inputInfo)
        {
            if (inputInfo.InputPos.x < 0 || inputInfo.InputPos.y < 0)
            {
                return;
            }
            NodeEditorState state = inputInfo.EdState;
            float Scale = 0.01f * inputInfo.InputEvent.delta.y;
            state.GraphZoom += Scale;
            if (state.GraphZoom <= 0.2f)
            {
                state.GraphZoom = 0.2f;
            }
            else if (state.GraphZoom >= 1.2f)
            {
                state.GraphZoom = 1.2f;
            }
            state.ZoomPos = state.CurGraphRect.size / 2 ;
            state.PanAdjust = EffectUtility.PanelScale(ref state.CurGraphRect,state.ZoomPos,state.GraphZoom);
        }
        #endregion

        #region NodePanel

        [Handle(EventType.Repaint)]
        private static void HandleCheckFocus(NodeInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.EdState;
            state.FocusNode = inputInfo.Graph.CheckFocus(inputInfo.InputPos);
            state.FocusKnob = inputInfo.Graph.CheckFocusKnob(inputInfo.InputPos);
            state.MousePos = inputInfo.InputPos;
        }

        [Handle(EventType.MouseDown, 110)]
        private static void HandleNodePanelDown(NodeInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.EdState;
            if (inputInfo.InputEvent.button == 0 && state.FocusNode != null)
            {
                state.SelectedNode = state.FocusNode;
                Selection.activeObject = state.FocusNode;
                if (!state.IsPineSetting)
                {
                    state.IsPineSetting = true;
                    state.DragStart = inputInfo.InputPos;
                    state.DragOffset = Vector2.zero;
                }
                else
                {
                    state.DragStart = inputInfo.InputPos;
                    state.DragOffset = Vector2.zero;
                }
            }
        }

        [Handle(EventType.mouseDrag)]
        private static void HandleNodePanelDrag(NodeInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.EdState;
            if (inputInfo.InputPos.x < 0 || inputInfo.InputPos.y < 0||state.SelectedNode==null)
            {
                return;
            }
            if (state.IsPineSetting)
            {
                Vector2 panChangeDragOffset = state.DragOffset;
                state.DragOffset = inputInfo.InputPos - state.DragStart;
                panChangeDragOffset = (state.DragOffset - panChangeDragOffset);
                state.SelectedNode.rect.position += panChangeDragOffset / state.GraphZoom;
            }
        }
        [Handle(EventType.MouseUp)]
        private static void HandleNodePanelUp(NodeInputInfo inputInfo)
        {
            if (inputInfo.InputPos.x < 0 || inputInfo.InputPos.y < 0)
            {
                return;
            }
            NodeEditorState state = inputInfo.EdState;
            state.IsPineSetting = false;
        }
        #endregion

        #region KnopPanel

        [Handle(EventType.MouseDown, 120)]
        private static void HandleKnobMouseDown(NodeInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.EdState;
            if (inputInfo.InputEvent.button == 0 && state.FocusKnob != null)
            {
                state.SelectedKnob = state.FocusKnob;
                state.IsLinkSetting = true;
            }
        }

        [Handle(EventType.MouseDrag)]
        private static void HandleKnobMouseDrag(NodeInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.EdState;
            if (inputInfo.InputPos.x < 0 || inputInfo.InputPos.y < 0 || state.SelectedKnob == null)
            {
                return;
            }
        }

        [Handle(EventType.MouseUp)]
        private static void HandleKnobMouseUp(NodeInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.EdState;
            state.IsLinkSetting = false;
            if (state.SelectedKnob==null||inputInfo.InputPos.x < 0 || inputInfo.InputPos.y < 0||state.SelectedKnob==state.FocusKnob)
            {                
                return;
            }
            if (state.FocusKnob != null)
            {
                state.SelectedKnob.Linking(state.FocusKnob);

            }
            else
            {
                state.SelectedKnob = null;
            }
        }
        #endregion

        #region HandleSetting

        [Handle(EventType.MouseDown)]
        private static void HandleSettingMouseDown(NodeInputInfo inputInfo)
        {
            var state = inputInfo.EdState;
            if (inputInfo.InputEvent.button != 1)
            {
                return;
            }
            if (state.SelectedNode != null && state.FocusNode == state.SelectedNode)
            {
                inputInfo.Graph.DrawSelectedNodeMenu();
            }
            else
            {
                inputInfo.Graph.NodeGenericMenu();
            }
        }

        #endregion

        #region SelectedNodes

        [Handle(EventType.MouseDown)]
        private static void SelectedPanelMouseDown(NodeInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.EdState;
            if(inputInfo.InputEvent.modifiers== EventModifiers.Alt)
            {
                state.IsSelectedPanelNodes = true;
                state.SelectedNodes.Clear();
                state.selectedPanelNodesStartPos = inputInfo.InputPos;
            }
        }

        [Handle(EventType.MouseDrag)]
        private static void SelectedPanelMouseDrag(NodeInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.EdState;
            if (inputInfo.InputEvent.modifiers == EventModifiers.Alt)
            {
                state.selectedPanelNodesEndPos = inputInfo.InputPos;
            }
        }
        [Handle(EventType.MouseUp)]
        private static void SelectedPanelMouseUp(NodeInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.EdState;
            if (inputInfo.InputEvent.modifiers == EventModifiers.Alt)
            {
                state.selectedPanelNodesEndPos = inputInfo.InputPos;
                state.IsSelectedPanelNodes = false;
               // Rect rect=new Rect(state.selectedPanelNodesStartPos);
                for (int i = 0; i < NodeEditor.curNodeGraph.nodes.Count; i++)
                {
                    //if( state.CurGraph.nodes[i].rect)
                    state.SelectedNodes.Add(NodeEditor.curNodeGraph.nodes[i]);
                }
            }
        }
        [Handle(EventType.MouseDrag)]
        private static void SelectedPanelMouseDragCheck(NodeInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.EdState;
            if (inputInfo.InputEvent.modifiers != EventModifiers.Alt)
            {
                state.IsSelectedPanelNodes = false;
            }
        }
        #endregion

#endif
    }
}

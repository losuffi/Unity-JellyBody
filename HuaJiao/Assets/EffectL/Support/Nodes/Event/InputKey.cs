using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    [Node(false,"按键输入","Event", NodeType.Event)]
    public class InputKey:Node
    {
        public enum InputType
        {
            Down,
            Up,
            Press
        }
        [EffectLDisplayHandleAssets] public KeyCode keyCode;
        [EffectLDisplayHandleAssets] public InputType inputType;
        void Work()
        {
            if (inputType== InputType.Down&&Input.GetKeyDown(keyCode))
            {
                NodeStack.RunNext(this);
            }
            else if (inputType == InputType.Up && Input.GetKeyUp(keyCode))
            {
                NodeStack.RunNext(this);
            }
            else if (inputType == InputType.Press && Input.GetKey(keyCode))
            {
                NodeStack.RunNext(this);
            }
        }
    }
}

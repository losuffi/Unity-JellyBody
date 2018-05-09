using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    public static class NodeInputSystem
    {
        //TODO:Fill Function For Several
        private static List<KeyValuePair<HandleAttribute, Delegate>> _eventHandles;
        #region Setup Handles
        public static void Fetch()
        {
            _eventHandles=new List<KeyValuePair<HandleAttribute, Delegate>>();
            IEnumerable<Assembly> scriptAssemblies =
                AppDomain.CurrentDomain.GetAssemblies().Where(ar => ar.FullName.Contains("Assembly-"));
            foreach (Assembly assembly in scriptAssemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public|BindingFlags.FlattenHierarchy|BindingFlags.NonPublic|BindingFlags.Static))
                    {
                        foreach (object attribute in method.GetCustomAttributes(true))
                        {
                            Type attributeType = attribute.GetType();
                            if (attributeType == typeof(HandleAttribute))
                            {
                                _eventHandles.Add(new KeyValuePair<HandleAttribute, Delegate>(
                                    attribute as HandleAttribute,
                                    Delegate.CreateDelegate(typeof(Action<NodeInputInfo>), method)));
                            }
                        }
                    }
                }
            }
            _eventHandles.Sort((a, b) => a.Key.Priority.CompareTo(b.Key.Priority));
        }
        #endregion

        #region Dynamic Invoke Events

        internal static void DynamicInvoke(NodeInputInfo inputInfo)
        {
            if (_eventHandles == null)
            {
                return;
            }
            if (inputInfo.CheckEvent())
            {
                foreach (var keyValuePair in _eventHandles)
                {
                    if (keyValuePair.Key.eventType == inputInfo.InputEvent.type)
                    {
                        keyValuePair.Value.DynamicInvoke(inputInfo);
                    }
                }
            }
        }

        #endregion
    }

    public class NodeInputInfo
    {
        public string Message;
        public NodeEditorState EdState;
        public NodeGraph Graph;

        public Vector2 InputPos
        {
            get { return InputEvent.mousePosition; }
        }
        public Event InputEvent;
        public NodeInputInfo(string message,NodeGraph graph)
        {
            Message = message;
            EdState = graph.curNodeState;
            InputEvent=Event.current;
            Graph = graph;
        }

        public bool CheckEvent()
        {
            if (InputEvent == null)
            {
                InputEvent=Event.current;
                return false;
            }
            return true;
        }

        public void InfoUpdate(string message, NodeGraph graph)
        {
            Message = message;
            EdState = graph.curNodeState;
            InputEvent = Event.current;
            Graph = graph;
        }
    }
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = true)]
    public class HandleAttribute : Attribute
    {
        public EventType? eventType { get; private set; }
        public int Priority { get; private set; }
        public HandleAttribute(EventType type,int priority)
        {
            eventType = type;
            Priority = priority;
        }

        public HandleAttribute(EventType type)
        {
            eventType = type;
        }

        public HandleAttribute(int priority)
        {
            Priority = priority;
        }
        public HandleAttribute()
        {
            eventType = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    public static class NodeStack
    {
        #region NodeSelectPanel

        public static List<string> MonoComponentsName;
        public static List<Type> MonoComponents;
        public static Dictionary<Node, NodeData> nodes;
        public static Dictionary<string, Type> types;
        public static Dictionary<string, NodeClan> clans;
        public static Node getDefaultNode(string nodeName)
        {
            return nodes.Keys.ToList().Find(ar => ar.GetId.Equals(nodeName));
        }
        public static void FetchNode()
        {
            nodes=new Dictionary<Node, NodeData>();
            clans=new Dictionary<string, NodeClan>();
            IEnumerable<Assembly> scriptAssemblies =
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(ar => ar.FullName.Contains(EditorDataMgr.DATA.Assmbly_ContainName));
            foreach (var assembly in scriptAssemblies)
            {
                Debug.Log(assembly.FullName);
                foreach (Type type in assembly.GetTypes().Where(ar=> ar.Namespace!=null&& ar.Namespace.Contains("NodeSupport")&& ar.IsClass&&!ar.IsAbstract&& ar.IsSubclassOf(typeof(Node))&&ar!=typeof(Cube) &&!ar.IsSubclassOf(typeof(Cube))))
                {
                    Debug.Log(type.Name+"-----"+type.Namespace);
                    Node node=ScriptableObject.CreateInstance(type.Name) as  Node;
                    object[] attrs = type.GetCustomAttributes(typeof(NodeAttribute), false);
                    NodeAttribute attr=attrs[0] as NodeAttribute;
                    if (attr != null||!attr.IsHide)
                    {
                        node.GetId=attr.Name;
                        var data = new NodeData(attr.Name, attr.Clan, attr.Size);
                        data.methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public |
                                                       BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();
                        data.fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public |
                                                     BindingFlags.DeclaredOnly | BindingFlags.Instance).ToList();
                        data.nodeType = attr.Type;
                        nodes.Add(node,data
                            );
                        if (!clans.ContainsKey(attr.Clan)&&!attr.Clan.Equals("UnSelected"))
                        {
                            clans.Add(attr.Clan, new NodeClan(attr.Clan));
                        }
                        if (clans.ContainsKey(attr.Clan))
                        {
                            clans[attr.Clan].clanNode.Add(node);
                            node.Clan = attr.Clan;
                        }
                    }
                }
            }
            FetchMonos();
            FetchUnityType();
        }

        public static void FetchCube()
        {
            nodes = new Dictionary<Node, NodeData>();
            clans = new Dictionary<string, NodeClan>();
            IEnumerable<Assembly> scriptAssemblies =
                AppDomain.CurrentDomain.GetAssemblies().Where(ar => ar.FullName.Contains("Assembly-"));
            foreach (var assembly in scriptAssemblies)
            {
                foreach (Type type in assembly.GetTypes().Where(ar => ar.IsClass && !ar.IsAbstract &&ar.IsSubclassOf(typeof(Cube))))
                {
                    Node node = ScriptableObject.CreateInstance(type.Name) as Node;
                    object[] attrs = type.GetCustomAttributes(typeof(NodeAttribute), false);
                    NodeAttribute attr = attrs[0] as NodeAttribute;
                    if (attr != null || !attr.IsHide)
                    {
                        node.GetId = attr.Name;
                        var data = new NodeData(attr.Name, attr.Clan, attr.Size);
                        data.methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public |
                                                       BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();
                        data.fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public |
                                                     BindingFlags.DeclaredOnly | BindingFlags.Instance).ToList();
                        data.nodeType = attr.Type;
                        nodes.Add(node, data
                        );
                        if (!clans.ContainsKey(attr.Clan))
                        {
                            clans.Add(attr.Clan, new NodeClan(attr.Clan));
                        }
                        clans[attr.Clan].clanNode.Add(node);

                    }
                }
            }
            FetchMonos();
        }

        public static void FetchUnityType()
        {
            var assembly = typeof(GameObject).Assembly;
            types=new Dictionary<string, Type>();
            foreach (Type type in assembly.GetTypes().Where(T => !T.IsAbstract && !T.IsEnum && !T.IsContextful&&!T.IsInterface))
            {
                if (!types.ContainsKey(type.Name))
                {
                    types.Add(type.Name, type);
                }
            }
        }
        static void FetchMonos()
        {
            MonoComponentsName=new List<string>();
            MonoComponents=new List<Type>();
            IEnumerable<Assembly> scriptAssemblies =
                AppDomain.CurrentDomain.GetAssemblies().Where(ar => ar.FullName.Contains("Assembly-"));
            foreach (var assembly in scriptAssemblies)
            {
                foreach (Type type in assembly.GetTypes().Where(ar=>ar.IsClass&&!ar.IsAbstract&&ar.IsSubclassOf(typeof(Component))))
                {
                    MonoComponents.Add(type);
                    MonoComponentsName.Add(type.Name);
                }
            }
        }
        #endregion

        #region TypeStructSetting

        #endregion

        #region NodeWork


        public static void Run(Node node)
        {
            var data = nodes[getDefaultNode(node.GetId)];
            MethodInfo workMethod =
                data.methods.Find(res => res.Name.Equals("Work") && res.ReturnType.Equals(typeof(void)));
            if (workMethod != null)
            {
                AssetGetRun(node);
                workMethod.Invoke(node, null);
            }
            if (node.FlowOut != null && node.FlowOut.connection != null)
            {
                Run(node.FlowOut.connection.Body);
            }
        }

        public static void SingleRun(Node node)
        {
            var data = nodes[getDefaultNode(node.GetId)];
            MethodInfo workMethod =
                data.methods.Find(res => res.Name.Equals("Work") && res.ReturnType.Equals(typeof(void)));
            if (workMethod != null)
            {
                AssetGetRun(node);
                workMethod.Invoke(node, null);
            }
        }
        public static void RunNext(Node node)
        {
            var data = nodes[getDefaultNode(node.GetId)];
            if (node.FlowOut != null && node.FlowOut.connection != null)
            {
                Run(node.FlowOut.connection.Body);
            }
        }
        static void AssetGetRun(Node node)
        {
            foreach (NodeInput input in node.inputKnobs)
            {
                if (input.connection != null)
                {
                    AssetGetRun(input.connection.Body);
                    var data = nodes[getDefaultNode(node.GetId)];
                    var field= data.fields.Find(res => res.Name.Equals(input.BindFieldName));   //根据名称 对应field，如有可能同名 不同类，需要检测类型匹配

                    UpdateField(node,field,data,input.GetValue());
                }
            }
            AssetSetRun(node);
        }

        static void AssetSetRun(Node node)
        {
            var data = nodes[getDefaultNode(node.GetId)];
            MethodInfo workMethod =
                data.methods.Find(res => res.Name.Equals("Evaluate"));
            if (workMethod != null)
            {
                node.outputKnobs.Find(res => res.BindFieldName.Equals("Evaluate"))
                    .SetValueDefault(workMethod.Invoke(node, null));
            }
        }
        public static void NodeInit(Node node)
        {
            var data = nodes[getDefaultNode(node.GetId)];
            if (data.nodeType == NodeType.Event||data.nodeType==NodeType.Message)
            {
                node.FlowOut = node.CreateFlowOutKnob();
            }
            else if(data.nodeType == NodeType.Handle)
            {
                node.FlowIn = node.CreateFlowInKnob();
                node.FlowOut = node.CreateFlowOutKnob();
            }
            else if (data.nodeType == NodeType.End||data.nodeType==NodeType.Branch)
            {
                node.FlowIn = node.CreateFlowInKnob();
            }
            else if(data.nodeType==NodeType.Cube)
            {
                var cube = node as Cube;
                cube.Direct.Add(cube.CreateDirect("Direct-Y", Side.Top));
                cube.Direct.Add(cube.CreateDirect("Direct-Y", Side.Bottom));
                cube.Direct.Add(cube.CreateDirect("Direct-X", Side.Left));
                cube.Direct.Add(cube.CreateDirect("Direct-X", Side.Right));
            }

            NodeHandleAsset(node,data);
            NodeHandleOutput(node, data);
            NodeHandleInit(node, data);
        }

        static void NodeHandleOutput(Node node, NodeData data)
        {
            MethodInfo workMethod =
                data.methods.Find(res => res.Name.Equals("Evaluate"));
            if (workMethod != null)
            {
                node.CreateNodeOutput("AssetOut", ConnectionType.getTypeName(workMethod.ReturnType), Side.Right, 20,
                    "Evaluate");
            }
        }
        static void NodeHandleAsset(Node node,NodeData data)
        {
            int assetPort = 0;
            foreach (FieldInfo field in data.fields)
            {
                foreach (object attribute in field.GetCustomAttributes(false))
                {   
                    if (attribute.GetType() == typeof(EffectLTransitAssetAttribute))
                    {
                        assetPort++;
                        node.CreateNodeInput("AssetIn", ConnectionType.getTypeName(field.FieldType), Side.Left,
                            assetPort * 20, field.Name);
                    }
                }
            }
        }


        static void NodeHandleInit(Node node, NodeData data)
        {
            MethodInfo initMethod =
                data.methods.Find(res => res.Name.Equals("Init") && res.ReturnType.Equals(typeof(void)));
            if (initMethod != null)
            {
                initMethod.Invoke(node, null);
            }
        }
        public static List<FieldInfo> NodeGetAsset(Node node)
        {
            var type = node.GetType();
            List<FieldInfo> fields=new List<FieldInfo>();
            foreach (FieldInfo field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance))
            {
                foreach (object attribute in field.GetCustomAttributes(false))
                {
                    if (attribute.GetType() == typeof(EffectLTransitAssetAttribute)||attribute.GetType()==typeof(EffectLDisplayHandleAssetsAttribute))
                    {
                        fields.Add(field);
                    }
                }
            }
            return fields;
        }
        static void UpdateField(Node node, FieldInfo field,NodeData data, object value)
        {
            object[] os = field.GetCustomAttributes(typeof(EffectLTransitConvertAssetAttribute), false);
            if (!os.Any())
            {
                field.SetValue(node,value);
                return;
            }
            var conver = os[0] as EffectLTransitConvertAssetAttribute;
            if (conver == null)
            {
                field.SetValue(node,value);
            }
            else
            {
                var converMethod = data.methods.Find(ar => ar.Name.Equals(conver.MethodName));
                if (converMethod == null)
                {
                    field.SetValue(node, value);
                }
                else
                {
                    field.SetValue(node, converMethod.Invoke(node, new object[] {value}));
                }
            }
        }
        #endregion

        #region UnityTypeHandle 

        public static List<PropertyInfo> getProperty(string type, BindingFlags flags, Func<PropertyInfo, bool> pre =null)
        {
            List<PropertyInfo> tar=new List<PropertyInfo>();
            if (pre == null)
            {
                foreach (PropertyInfo info in types[type].GetProperties(flags))
                {
                    tar.Add(info);
                }
            }
            else
            {
                foreach (PropertyInfo info in types[type].GetProperties(flags).Where(pre))
                {
                    tar.Add(info);
                }
            }
            return tar;
        }

        

        #endregion
    }
    [Serializable]
    public class NodeClan
    {
        public string clanName;
        public List<Node> clanNode;

        public NodeClan(string clanName)
        {
            this.clanName = clanName;
            this.clanNode = new List<Node>();
        }
    }
    [Serializable]
    public enum NodeType
    {
        Message,
        Event,
        Handle,
        Calc,
        End,
        Cube,
        Branch,
    }
    public class NodeData
    {
        public string Name;
        public string Identity;
        public Vector2 Size;
        public NodeType nodeType;
        public List<MethodInfo> methods;
        public List<FieldInfo> fields;
        public NodeData(string name, string identity, Vector2 size)
        {
            Name = name;
            Identity = identity;
            Size = size;
        }
    }


    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method,AllowMultiple = true)]
    public class NodeAttribute : Attribute
    {
        public bool IsHide { get; private set; }
        public string Name { get; private set; }
        public string Clan { get; private set; }
        public Vector2 Size { get; private set; }
        public NodeType Type { get; private set; }
        public NodeAttribute(bool isHide, string name,string clan,NodeType type=NodeType.Event,float width= 60f, float height=120f)
        {
            IsHide = isHide;
            Name = name;
            Clan = clan;
            Type = type;
            Size=new Vector2(width,height);
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EffectLTransitAssetAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EffectLDisplayHandleAssetsAttribute:Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EffectLTransitConvertAssetAttribute:Attribute
    {
        public string MethodName;

        public EffectLTransitConvertAssetAttribute(string convert)
        {
            MethodName = convert;
        }
    }

}

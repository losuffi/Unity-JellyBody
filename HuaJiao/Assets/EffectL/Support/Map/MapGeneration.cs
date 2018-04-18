using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    public static class MapGeneration
    {
        public const float Distance = 2f;
        public static GameObject mapG = null;
        public static List<GameObject> map;
        private static HashSet<Cube> closeSet;
        public static void Generator()
        {
            if (MapEditor.curNodeGraph == null)
            {
                return;
            }
            mapG= GameObject.Find("mapGenerator");
            if (GameObject.Find("mapGenerator") != null)
            {
                Clear(mapG);
                Initialzation(mapG);
            }
            else
            {
                mapG = new GameObject("mapGenerator");
                Initialzation(mapG);
            }
        }

        static void Clear(GameObject o)
        {
            for (int i = o.transform.childCount-1; i >=0 ; i--)
            {
                MonoBehaviour.DestroyImmediate(o.transform.GetChild(i).gameObject,true);
            }
        }

        static void Initialzation(GameObject o)
        {
            map=new List<GameObject>();
            closeSet=new HashSet<Cube>();
            var nodes = MapEditor.curNodeGraph.nodes;
            if (nodes.Any())
            {
                DrawCube(Vector3.zero, nodes[0] as Cube);
            }
        }

        static void DrawCube(Vector3 Origin, Cube node)
        {
            if (!closeSet.Contains(node))
            {
                var obj = CreateCube(Origin);
                map.Add(obj);
                foreach (int Index in node.Component)
                {
                    obj.AddComponent(NodeStack.MonoComponents[Index]);
                }
                closeSet.Add(node);
            }
            if (node.Direct[0].connection!=null)
            {
                //Top
                if (!closeSet.Contains(node.Direct[0].connection.Body as Cube))
                    DrawCube(Origin + Vector3.forward * Distance, node.Direct[0].connection.Body as Cube);
            }
            if (node.Direct[1].connection!=null)
            {
                //Bottom
                if (!closeSet.Contains(node.Direct[1].connection.Body as Cube))
                    DrawCube(Origin - Vector3.forward * Distance, node.Direct[1].connection.Body as Cube);
            }
            if (node.Direct[2].connection!=null)
            {
                //Left
                if (!closeSet.Contains(node.Direct[2].connection.Body as Cube))
                    DrawCube(Origin - Vector3.right * Distance, node.Direct[2].connection.Body as Cube);
            }
            if (node.Direct[3].connection!=null)
            {
                //Right
                if (!closeSet.Contains(node.Direct[3].connection.Body as Cube))
                    DrawCube(Origin + Vector3.right * Distance, node.Direct[3].connection.Body as Cube);
            }
        }


        static GameObject CreateCube(Vector3 pos)
        {
            var cube=new GameObject("Cube");
            cube.AddComponent<MeshFilter>().mesh = CubeMesh();
            var mate = cube.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            mate.SetColor("_Color", Color.black);
            mate.SetFloat("_Glossiness", 0);
            cube.AddComponent<BoxCollider>();
            cube.transform.parent = mapG.transform;
            cube.transform.position = pos;
            return cube;
        }

        static Mesh CubeMesh()
        {
            var m=new Mesh();
            Vector3[] vertices = {
                new Vector3 (0, 0, 0),
                new Vector3 (1, 0, 0),
                new Vector3 (1, 1, 0),
                new Vector3 (0, 1, 0),
                new Vector3 (0, 1, 1),
                new Vector3 (1, 1, 1),
                new Vector3 (1, 0, 1),
                new Vector3 (0, 0, 1),
            };
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i]+=new Vector3(-0.5f,-0.5f,-0.5f);
            }
            int[] triangles = {
                0, 2, 1, //face front
                0, 3, 2,
                2, 3, 4, //face top
                2, 4, 5,
                1, 2, 5, //face right
                1, 5, 6,
                0, 7, 4, //face left
                0, 4, 3,
                5, 4, 7, //face back
                5, 7, 6,
                0, 6, 7, //face bottom
                0, 1, 6
            };
            m.Clear();
            m.vertices = vertices;
            m.triangles = triangles;
            m.RecalculateNormals();
            return m;
        }
    }
}

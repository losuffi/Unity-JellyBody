using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer),typeof(MeshFilter))]
public class TestMeshCtr : MonoBehaviour
{
    [SerializeField] private float m_Spring;
    [SerializeField] private float m_Damping;

    private Vector3[] velocity;
    private Vector3[] replaceVertices,originVertices;
    private Renderer r;
    private MeshFilter meshFilter;
    private Mesh mesh;

    void Awake()
    {
        r = GetComponent<Renderer>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        velocity=new Vector3[mesh.vertices.Length];
        originVertices = mesh.vertices;
        replaceVertices =new Vector3[originVertices.Length];
        for (int i = 0; i < originVertices.Length; i++)
        {
            replaceVertices[i] = originVertices[i];
        }
    }

    void Update()
    {
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            UpdateVertices(i);
        }
        mesh.vertices = replaceVertices;
        NormalSolver.RecalculateNormals(mesh,90);
    }

    #region InternalFunction

    private Vector3 displacement;
    private void UpdateVertices(int i)
    {

        displacement = replaceVertices[i] - originVertices[i];
        velocity[i] -= displacement * m_Spring * Time.deltaTime;
        velocity[i] *= (1f - m_Damping * Time.deltaTime);
        replaceVertices[i] += velocity[i] * Time.deltaTime;
    }
    #endregion

    #region Interface
    private Vector3 dir = Vector3.one;
    private float velocitySqr;
    private float singleForce;
    public void AddForcce(Vector3 pos, float force)
    { 
        Debug.Log("Bingo");
        pos=transform.worldToLocalMatrix.MultiplyPoint(pos);
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            dir = mesh.vertices[i]-pos;
            velocitySqr = dir.sqrMagnitude;
            singleForce = force / (1 + velocitySqr);
            velocity[i] += dir.normalized * singleForce;
        }
    }

    #endregion
}

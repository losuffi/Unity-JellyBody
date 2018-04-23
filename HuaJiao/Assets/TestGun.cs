using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGun : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float m_Force;
    private RaycastHit hit;
    private Ray ray;
	
	void Update () {
	    if (Input.GetMouseButtonDown(0))
	    {
	        ray = cam.ScreenPointToRay(Input.mousePosition);
	        if (Physics.Raycast(ray, out hit, 100))
	        {
	            if (hit.collider.GetComponent<HuaJiao.JellyMiscNs.JellyAgentCtr>() != null)
	            {
                    hit.collider.GetComponent<HuaJiao.JellyMiscNs.JellyAgentCtr>().AddForce(hit.point, m_Force);
	            }
	        }
	    }
	}
}

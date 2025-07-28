using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Camera_Model : MonoBehaviour
{
    public Transform target;
    public Vector3 distance;    

    void Start()
    {  
        GetComponent<PostProcess>().enabled = false;                
    }

    public void StartGrayScale(GameObject camera) {
        Debug.Log("start gray scale");
        GetComponent<PostProcess>().enabled = true;
    }

    public void ResetGrayScale(GameObject camera) {
        Debug.Log("reset gray scale");
        GetComponent<PostProcess>().enabled = false;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + distance;

            transform.LookAt(target);
        }
    }

    public void debug_camera_plus_x(int d)
    {
        distance += new Vector3(d, 0, 0);
    }
    public void debug_camera_plus_y(int d)
    {
        distance += new Vector3(0, d, 0);
    }
    public void debug_camera_plus_z(int d)
    {
        distance += new Vector3(0, 0, d);
    }

}
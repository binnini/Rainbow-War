using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraSetup : MonoBehaviourPun
{
    private void Start()
    {
        if (photonView.IsMine)
        {
            Camera_Model mainCamera = FindObjectOfType<Camera_Model>();

            mainCamera.target = transform;
        }
    }
}

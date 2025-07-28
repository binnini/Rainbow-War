using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UISetup : MonoBehaviourPun
{
    private void Start()
    {
        UIManager uIManager = GameObject.Find("HUD Canvas").GetComponent<UIManager>();

        if (photonView.IsMine)
        {
            uIManager.playerStatus = transform.GetComponent<Status>();
        }
        else
        {            
            uIManager.AddHpSlider(transform.gameObject);
        }
    }
}

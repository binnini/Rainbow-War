using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraillColl_Manager : MonoBehaviour
{    
    private void OnTriggerEnter(Collider other)    {        
        Debug.Log("trailcoll manager���� �浹 �߻� : "+other.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log(other.transform.parent.name + "�� trail�� ��ҽ��ϴ�.");                        
            transform.parent.GetComponent<TrailArea>().mainTrail.GetComponent<Trail_Action>().isTrailAttacked = true;
        }
    }
}

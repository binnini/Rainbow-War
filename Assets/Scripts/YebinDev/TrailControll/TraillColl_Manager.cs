using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraillColl_Manager : MonoBehaviour
{    
    private void OnTriggerEnter(Collider other)    {        
        Debug.Log("trailcoll manager에서 충돌 발생 : "+other.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log(other.transform.parent.name + "이 trail을 밟았습니다.");                        
            transform.parent.GetComponent<TrailArea>().mainTrail.GetComponent<Trail_Action>().isTrailAttacked = true;
        }
    }
}

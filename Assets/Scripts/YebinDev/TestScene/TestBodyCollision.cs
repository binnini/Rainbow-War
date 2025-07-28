using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class TestBodyCollision : MonoBehaviourPun
{
    [SerializeField] TestTrail_Action trail_Action;

    [PunRPC]
    // 총알이 몸에 닿았을 때
    private void OnTriggerEnter(Collider other)
    {        
        // 영역과 충돌시
        if (other.gameObject.layer == 9)
        {
            TestTrail_Action otherTrail_Action = other.GetComponent<TestTrailArea>().mainTrail.GetComponent<TestTrail_Action>();
            TestTrail_Action myTrail_Action = gameObject.transform.parent.GetComponentInChildren<TestTrail_Action>();

            if (otherTrail_Action && otherTrail_Action != myTrail_Action && !myTrail_Action.attackedTrail_Action.Contains(otherTrail_Action))
            {
                myTrail_Action.attackedTrail_Action.Add(otherTrail_Action);
            }
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        // 영역과 충돌시
        if (col.gameObject.layer == 9)
        {
            //Debug.Log("영역 침범 성공");
            TestTrail_Action otherTrail_Action = col.collider.GetComponent<TestTrailArea>().mainTrail.GetComponent<TestTrail_Action>();
            TestTrail_Action myTrail_Action = gameObject.transform.parent.GetComponentInChildren<TestTrail_Action>();

            if (otherTrail_Action && otherTrail_Action != myTrail_Action && !myTrail_Action.attackedTrail_Action.Contains(otherTrail_Action))
            {
                myTrail_Action.attackedTrail_Action.Add(otherTrail_Action);
            }
        }
    }
}



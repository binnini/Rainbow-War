using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class TestBodyCollision : MonoBehaviourPun
{
    [SerializeField] TestTrail_Action trail_Action;

    [PunRPC]
    // �Ѿ��� ���� ����� ��
    private void OnTriggerEnter(Collider other)
    {        
        // ������ �浹��
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
        // ������ �浹��
        if (col.gameObject.layer == 9)
        {
            //Debug.Log("���� ħ�� ����");
            TestTrail_Action otherTrail_Action = col.collider.GetComponent<TestTrailArea>().mainTrail.GetComponent<TestTrail_Action>();
            TestTrail_Action myTrail_Action = gameObject.transform.parent.GetComponentInChildren<TestTrail_Action>();

            if (otherTrail_Action && otherTrail_Action != myTrail_Action && !myTrail_Action.attackedTrail_Action.Contains(otherTrail_Action))
            {
                myTrail_Action.attackedTrail_Action.Add(otherTrail_Action);
            }
        }
    }
}



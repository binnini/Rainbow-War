using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Body_Collision : MonoBehaviourPun
{
    Status status_script;
    EffectManager effectManager;
    [SerializeField] Trail_Action trail_Action;
    [SerializeField] Transform hitEff_SpawnPoint;

    void Start()
    {
        status_script = GetComponentInParent<Status>();        
        effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();                
    }

    private void Update() {

    }

    [PunRPC]
    // �Ѿ��� ���� ����� ��
    private void OnTriggerEnter(Collider other)
    {        
        // �׾������� ���� ����
        if (status_script.isDead)
        {
            return;
        }

        // ȣ��Ʈ������ ����
        if (PhotonNetwork.IsMasterClient)
        {
            // ���� ������Ʈ�� �±װ� bullet�ΰ� �Ǻ�
            if (other.tag.Equals("bullet"))
            {
                // Bullet ������Ʈ ������
                status_script.bullet = other.GetComponent<Bullet>();

                // ������ ���� �÷��̾� �Ҵ�
                status_script.lastHitPlayer = other.GetComponent<PhotonView>().Owner;

                // �Ѿ� ��������ŭ ������ ����
                status_script.OnDamage(status_script.bullet.bullet_damage);
            }
        }

        if (photonView.IsMine)
        {
            if (other.tag.Equals("bullet"))
            {
                // �ǰ� ����Ʈ ���
                effectManager.playHit_Effect(hitEff_SpawnPoint);                
            }

            // ������ �浹��
            if (other.gameObject.layer == 9)
            {
                Debug.Log("���� ħ�� ����");
                Trail_Action otherTrail_Action = other.GetComponent<TrailArea>().mainTrail.GetComponent<Trail_Action>();
                Trail_Action myTrail_Action = gameObject.transform.parent.GetComponentInChildren<Trail_Action>();

                if (otherTrail_Action && otherTrail_Action != myTrail_Action && !myTrail_Action.attackedTrail_Action.Contains(otherTrail_Action))
                {
                    myTrail_Action.attackedTrail_Action.Add(otherTrail_Action);
                }
            }

        }
    }
    private void OnCollisionEnter(Collision col)
    {
        
        // �׾������� ���� ����
        if (status_script.isDead)
        {
            return;
        }

        // ȣ��Ʈ������ ����
        if (PhotonNetwork.IsMasterClient)
        {
            // ���� ������Ʈ�� �±װ� bullet�ΰ� �Ǻ�
            if (col.collider.tag.Equals("bullet"))
            {
                // Bullet ������Ʈ ������
                status_script.bullet = col.collider.GetComponent<Bullet>();

                // ������ ���� �÷��̾� �Ҵ�
                status_script.lastHitPlayer = col.collider.GetComponent<PhotonView>().Owner;

                // �Ѿ� ��������ŭ ������ ����
                status_script.OnDamage(status_script.bullet.bullet_damage);                    
            }            
        }

        if (photonView.IsMine)
        {
            if (col.collider.tag.Equals("bullet"))
            {
                // �ǰ� ����Ʈ ���
                effectManager.playHit_Effect(hitEff_SpawnPoint);
            }

            // ������ �浹��
            if (col.gameObject.layer == 9)
            {
                //Debug.Log("���� ħ�� ����");
                Trail_Action otherTrail_Action = col.collider.GetComponent<TrailArea>().mainTrail.GetComponent<Trail_Action>();
                Trail_Action myTrail_Action = gameObject.transform.parent.GetComponentInChildren<Trail_Action>();

                if (otherTrail_Action && otherTrail_Action != myTrail_Action && !myTrail_Action.attackedTrail_Action.Contains(otherTrail_Action))
                {
                    myTrail_Action.attackedTrail_Action.Add(otherTrail_Action);
                }
            }
        }
    }

}

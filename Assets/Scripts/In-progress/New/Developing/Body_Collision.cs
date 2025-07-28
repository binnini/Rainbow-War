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
    // 총알이 몸에 닿았을 때
    private void OnTriggerEnter(Collider other)
    {        
        // 죽어있으면 실행 안함
        if (status_script.isDead)
        {
            return;
        }

        // 호스트에서만 실행
        if (PhotonNetwork.IsMasterClient)
        {
            // 닿은 오브젝트의 태그가 bullet인가 판별
            if (other.tag.Equals("bullet"))
            {
                // Bullet 컴포넌트 가져옴
                status_script.bullet = other.GetComponent<Bullet>();

                // 마지막 공격 플레이어 할당
                status_script.lastHitPlayer = other.GetComponent<PhotonView>().Owner;

                // 총알 데미지만큼 데미지 받음
                status_script.OnDamage(status_script.bullet.bullet_damage);
            }
        }

        if (photonView.IsMine)
        {
            if (other.tag.Equals("bullet"))
            {
                // 피격 이펙트 재생
                effectManager.playHit_Effect(hitEff_SpawnPoint);                
            }

            // 영역과 충돌시
            if (other.gameObject.layer == 9)
            {
                Debug.Log("영역 침범 성공");
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
        
        // 죽어있으면 실행 안함
        if (status_script.isDead)
        {
            return;
        }

        // 호스트에서만 실행
        if (PhotonNetwork.IsMasterClient)
        {
            // 닿은 오브젝트의 태그가 bullet인가 판별
            if (col.collider.tag.Equals("bullet"))
            {
                // Bullet 컴포넌트 가져옴
                status_script.bullet = col.collider.GetComponent<Bullet>();

                // 마지막 공격 플레이어 할당
                status_script.lastHitPlayer = col.collider.GetComponent<PhotonView>().Owner;

                // 총알 데미지만큼 데미지 받음
                status_script.OnDamage(status_script.bullet.bullet_damage);                    
            }            
        }

        if (photonView.IsMine)
        {
            if (col.collider.tag.Equals("bullet"))
            {
                // 피격 이펙트 재생
                effectManager.playHit_Effect(hitEff_SpawnPoint);
            }

            // 영역과 충돌시
            if (col.gameObject.layer == 9)
            {
                //Debug.Log("영역 침범 성공");
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

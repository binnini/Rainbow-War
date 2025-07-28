using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public string bullet_name { get; private set; }
    public int bullet_effect { get; private set; }

    public float bullet_speed = 5;
    public float bullet_damage = 50;

    private Rigidbody rigidbody;

    private float DestroyTime = 2.0f;

    /*IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(DestroyTime);
        PhotonNetwork.Destroy(gameObject);
    }*/

    // 초기값 부여
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        bullet_name = "Normal Bullet";
        bullet_effect = 0;

        //Debug.Log(PhotonNetwork.IsMasterClient);
        //Debug.Log(photonView.IsMine);
        // DestroyTime 후 자동삭제
        Destroy(gameObject, DestroyTime);

        /*if (gameObject.GetComponent<PhotonView>().Owner == photonView.Owner)
        {
            StartCoroutine("DestroyDelay");
        }
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine("DestroyDelay");
        }*/
    }

    // 물리 관련 기능 구현
    private void FixedUpdate()
    {
        // 앞으로 총알 이동
        transform.Translate(Vector3.forward * bullet_speed);
    }

    // 총알이 다른 물체와 닿았을 때
    private void OnTriggerEnter(Collider other)
    {
        // 디버그
        Debug.Log("총알 충돌 : "+other.name + " " + other.tag + " " + other.gameObject.layer);

        //Debug.Log(other.tag);

        // 충돌체가 Ground, Area layer가 아니라면 오브젝트 삭제
        if (!other.tag.Equals("Ground") && other.gameObject.layer != 9 && other.gameObject.layer != 3)
        {
            GameObject.Destroy(gameObject);
        }        
    }

    private void OnCollisionEnter(Collision col)
    {
        // 디버그
        Debug.Log("총알 충돌 : " + col.collider.name + " " + col.collider.tag + " " + col.gameObject.layer);

        //Debug.Log(other.tag);

        // 충돌체가 Ground, Area layer가 아니라면 오브젝트 삭제
        if (!col.collider.tag.Equals("Ground") && col.gameObject.layer != 9 && col.gameObject.layer != 3)
        {
            GameObject.Destroy(gameObject);
        }
    }
}

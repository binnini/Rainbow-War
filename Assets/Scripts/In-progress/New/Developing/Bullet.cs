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

    // �ʱⰪ �ο�
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        bullet_name = "Normal Bullet";
        bullet_effect = 0;

        //Debug.Log(PhotonNetwork.IsMasterClient);
        //Debug.Log(photonView.IsMine);
        // DestroyTime �� �ڵ�����
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

    // ���� ���� ��� ����
    private void FixedUpdate()
    {
        // ������ �Ѿ� �̵�
        transform.Translate(Vector3.forward * bullet_speed);
    }

    // �Ѿ��� �ٸ� ��ü�� ����� ��
    private void OnTriggerEnter(Collider other)
    {
        // �����
        Debug.Log("�Ѿ� �浹 : "+other.name + " " + other.tag + " " + other.gameObject.layer);

        //Debug.Log(other.tag);

        // �浹ü�� Ground, Area layer�� �ƴ϶�� ������Ʈ ����
        if (!other.tag.Equals("Ground") && other.gameObject.layer != 9 && other.gameObject.layer != 3)
        {
            GameObject.Destroy(gameObject);
        }        
    }

    private void OnCollisionEnter(Collision col)
    {
        // �����
        Debug.Log("�Ѿ� �浹 : " + col.collider.name + " " + col.collider.tag + " " + col.gameObject.layer);

        //Debug.Log(other.tag);

        // �浹ü�� Ground, Area layer�� �ƴ϶�� ������Ʈ ����
        if (!col.collider.tag.Equals("Ground") && col.gameObject.layer != 9 && col.gameObject.layer != 3)
        {
            GameObject.Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestDodge : MonoBehaviourPun
{
    public float dodge_speed = 50;
    public float dodge_anim_cool = 0.5f;
    public float dodge_cool = 5;
    public float dodge_distance = 5;
    public bool is_dodging = false;

    private float dodge_time = 0;
    public float cooltime { get; private set; } = 5;
    public bool is_cooltime { get; private set; } = false;

    private Vector3 dodge_direction = Vector3.zero;

    private Rigidbody rigidBody;


    // ������Ʈ ȣ�� �� �ʱⰪ �ο�
    void Start()
    {
        rigidBody = GetComponentInChildren<Rigidbody>();
        dodge_time = dodge_anim_cool;
        cooltime = dodge_cool;
    }

    // �Ϲ����� ��� ����
    private void Update()
    {
        // ������ �޼ҵ�
        //startDodge();
        Teleport();
    }

    private void Teleport()
    {
        // Dodge �Է��� �ޱ� and Dodge ���°� �ƴ� and ���� �Է��� 0�� �ƴ�
        if (Input.GetAxis("Dodge") == 1.0 && !is_cooltime && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
        {
            dodge_direction = Vector3.zero;
            Vector3 to = transform.position;

            // ������ ������ �� ������ ������ �Է¹���
            dodge_direction.x = Input.GetAxis("Horizontal");
            dodge_direction.z = Input.GetAxis("Vertical");
            Quaternion rot = Quaternion.LookRotation(dodge_direction.normalized);
            
            dodge_direction = dodge_direction.normalized * dodge_distance;
            rigidBody.MovePosition(transform.position + dodge_direction);
            
            Vector3 from = transform.position;            
            is_cooltime = true;
        }
    }

    // ���� ���� ��� ����
    void FixedUpdate()
    {
        if (is_cooltime)
        {
            // ��Ÿ�� ����
            cooltime -= Time.fixedDeltaTime;


            // ��Ÿ���� 0�� �Ǿ��� ��
            if (cooltime < 0)
            {
                // �ʱ�ȭ
                cooltime = dodge_cool;
                is_cooltime = false;
            }
        }
    }

    // ������ ���� �Ǻ��ϴ� �޼ҵ�
    private void startDodge()
    {
        // Dodge �Է��� �ޱ� and Dodge ���°� �ƴ� and ���� �Է��� 0�� �ƴ�
        if (Input.GetAxis("Dodge") == 1.0 && !is_dodging && !is_cooltime && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
        {
            // Dodge ���� true�� ����
            is_dodging = true;

            // ������ ������ �� ������ ������ �Է¹���
            dodge_direction.x = Input.GetAxis("Horizontal");
            dodge_direction.z = Input.GetAxis("Vertical");
        }
    }
}


using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestMove : MonoBehaviourPun
{
    public float walk_speed = 2.5f;
    public float rotate_speed = 5f;
    public float dash_speed = 5f;

    private bool is_dashing = false;
    private bool is_walking = false;
    public bool enable_walking = true;
    private bool dash_input = false;

    private Vector3 walk_direction = Vector3.zero;
    private Rigidbody rigidBody;
    private TestDodge dodge_script;


    // ������Ʈ ȣ��
    void Start()
    {
        rigidBody = GetComponentInChildren<Rigidbody>();
        dodge_script = GetComponent<TestDodge>();
    }

    // �Ϲ����� ��� ����
    private void Update()
    {
        transform.position = rigidBody.transform.position;
        rigidBody.transform.position = transform.position;


        // GetComponent ���� ���� �����

        // �̵� ���� ���� �ʱ�ȭ
        walk_direction = Vector3.zero;

        // �̵� ���� �Է� ����
        walk_direction.x = Input.GetAxis("Horizontal");
        walk_direction.z = Input.GetAxis("Vertical");
        // �̵� ���� �Ϲ�ȭ
        walk_direction = walk_direction.normalized;

        // �뽬 �Է� ����
        dash_input = Input.GetAxis("Dash") > 0;

        if (!is_dashing && dash_input)
        {
            is_walking = false;


            is_dashing = true;

        }
        else if(is_dashing && !dash_input)
        {
            is_dashing = false;
        }

        if (walk_direction.Equals(Vector3.zero) && is_walking)
        {
            is_walking = false;
        }
        else if (!walk_direction.Equals(Vector3.zero) && !is_walking && !is_dashing)
        {
            is_walking = true;
        }
    }

    private void walking()
    {
        if (dodge_script.is_dodging)
        {
            return;
        }
        // �޸��� ����
        if (is_dashing)
        {
            walk_direction *= dash_speed * Time.fixedDeltaTime;
        }

        // �ȱ� ����
        else
        {
            walk_direction *= walk_speed * Time.fixedDeltaTime;
        }
        // ȸ�� �ִϸ��̼�
        TurnAnime();

        // �̵�
        transform.Translate(walk_direction);
    }

    // ���� ���� ��� ����
    private void FixedUpdate()
    {
        rigidBody.velocity = Vector3.zero;
        if (enable_walking)
        {
            walking();
        }
    }

    // ĳ���� ȸ��
    private void TurnAnime()
    {
        // ���� ���� ��� return��
        if (walk_direction.x == 0 && walk_direction.z == 0)
        {
            return;
        }

        // ���� �ٶ󺸴� ������ Quaternion ���·� ��ȯ��
        Quaternion new_rotation = Quaternion.LookRotation(walk_direction.normalized);

        // rigidBody�� ȸ�� ������ Slerp�� õõ�� ��ȭ��Ŵ
        rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, new_rotation, rotate_speed * Time.fixedDeltaTime);
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Move : MonoBehaviourPun
{
    public float walk_speed = 2.5f;
    public float rotate_speed = 5f;
    public float dash_speed = 5f;

    private bool is_dashing = false;
    private bool is_walking = false;
    public bool enable_walking = true;
    private bool dash_input = false;

    private Vector3 walk_direction = Vector3.zero;

    private Animator animator;
    private Rigidbody rigidBody;
    private Status status;
    private Sound_Manager sound_mng;
    private Gun_Model gun_object;
    private Dodge dodge_script;
    private JoyStick joystick;
    private Toggle paint_button;

    private GameManager gameManager;

    // ������Ʈ ȣ��
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rigidBody = GetComponentInChildren<Rigidbody>();
        status = GetComponent<Status>();
        gun_object = GetComponentInChildren<Gun_Model>();
        dodge_script = GetComponent<Dodge>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        sound_mng = GameObject.Find("Sound_Manager").GetComponent<Sound_Manager>();
        joystick = GameObject.Find("HUD Canvas").GetComponentsInChildren<JoyStick>()[0];
        paint_button = GameObject.Find("PaintButton").GetComponent<Toggle>();
    }

    // �Ϲ����� ��� ����
    private void Update()
    {
        // ������ �������� �ʾ����� �Է� ���� ����
        if (!gameManager.isStart) {
            return;
        }
        // ĳ���Ͱ� �׾������� �Է� ���� ����
        if (status.isDead) {
            return;
        }

        // ���� �÷��̾ �ƴ� ��� �Է��� ���� �ʴ´�.
        if (!photonView.IsMine)
        {
            return;
        }

        transform.position = rigidBody.transform.position;
        rigidBody.transform.position = transform.position;


        // GetComponent ���� ���� �����
        //Debug.Log(status.speed);

        // �̵� ���� ���� �ʱ�ȭ
        walk_direction = Vector3.zero;

        // �̵� ���� �Է� ����
        //walk_direction.x = Input.GetAxis("Horizontal");
        walk_direction.x = joystick.InputVector.x;
        //walk_direction.z = Input.GetAxis("Vertical");
        walk_direction.z = joystick.InputVector.z;



        // �̵� ���� �Ϲ�ȭ
        walk_direction = walk_direction.normalized;

        // �뽬 �Է� ����
        //dash_input = Input.GetAxis("Dash") > 0;
        dash_input = paint_button.isOn;

        if (!is_dashing && dash_input)
        {
            is_walking = false;

            sound_mng.SFXPlay(Sound_Manager.SFXName.Dash);
            sound_mng.SFXPlay(Sound_Manager.SFXName.Painting);

            gun_object.enable_shooting = false;
            is_dashing = true;

        }
        else if(is_dashing && !dash_input)
        {
            sound_mng.SFXPlay(Sound_Manager.SFXName.StopPainting);
            is_dashing = false;
            gun_object.enable_shooting = true;
        }

        if (walk_direction.Equals(Vector3.zero) && is_walking)
        {
            sound_mng.SFXPlay(Sound_Manager.SFXName.StopMove);
            is_walking = false;
        }
        else if (!walk_direction.Equals(Vector3.zero) && !is_walking && !is_dashing)
        {
            sound_mng.SFXPlay(Sound_Manager.SFXName.Walk);

            is_walking = true;
        }

        // is_dashing �����
        //Debug.Log(is_dashing);
        //Debug.Log(Input.GetAxis("Horizontal") + " " + Input.GetAxis("Vertical"));
        //Debug.Log(walk_direction.x + " " + walk_direction.z);

        // �ȱ� �ִϸ��̼� �۵�
        if (walk_direction != Vector3.zero)
        {
            animator.SetBool("is_walking", true);
        }
        else
        {
            animator.SetBool("is_walking", false);
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

        // �����
        //Debug.Log(walk_direction);

        // �̵�
        //rigidBody.velocity = walk_direction;
        transform.Translate(walk_direction);
        //rigidBody.MovePosition(transform.position + walk_direction);
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

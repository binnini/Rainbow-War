using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Dodge : MonoBehaviourPun
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

    private Animator animator;
    private Rigidbody rigidBody;
    private UIManager UI_Manager;
    private EffectManager effectManager;
    private Sound_Manager sound_mng;
    private InputManager inputManager;


    // ������Ʈ ȣ�� �� �ʱⰪ �ο�
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rigidBody = GetComponentInChildren<Rigidbody>();
        UI_Manager = GameObject.Find("HUD Canvas").GetComponent<UIManager>();
        effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        sound_mng = GameObject.Find("Sound_Manager").GetComponent<Sound_Manager>();
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();

        dodge_time = dodge_anim_cool;
        cooltime = dodge_cool;
    }

    // �Ϲ����� ��� ����
    private void Update()
    {
        // ���� �÷��̾ ������ ����
        if (!photonView.IsMine)
        {
            return;
        }

        // ������ �޼ҵ�
        //startDodge();
        inputManager.teleport_button.onClick.AddListener(Teleport);
    }

    private void Teleport()
    {// && (inputManager.joyStick.InputVector != Vector3.zero)
        Debug.Log("Enter Teleport");
        // Dodge �Է��� �ޱ� and Dodge ���°� �ƴ� and ���� �Է��� 0�� �ƴ�
        if (!is_cooltime)
        {
            dodge_direction = Vector3.zero;
            Vector3 to = transform.position;

            // ������ ������ �� ������ ������ �Է¹���
            // dodge_direction.x = Input.GetAxis("Horizontal");
            // dodge_direction.z = Input.GetAxis("Vertical");
            dodge_direction.x = inputManager.joyStick.InputVector.x;
            dodge_direction.z = inputManager.joyStick.InputVector.z;

            Quaternion rot = Quaternion.LookRotation(dodge_direction.normalized);

            dodge_direction = dodge_direction.normalized * dodge_distance;
            rigidBody.MovePosition(transform.position + dodge_direction);
            
            Vector3 from = transform.position;
            effectManager.playDash_Effect(Vector3.Lerp(to,from,0.1f),rot);
            UI_Manager.ToggleTeleportUI(false);

            sound_mng.SFXPlay(Sound_Manager.SFXName.Teleport);

            is_cooltime = true;
        }
    }

    // ���� ���� ��� ����
    void FixedUpdate()
    {
        // ���� �÷��̾ ������ ����
        if (!photonView.IsMine)
        {
            return;
        }

        if (is_cooltime)
        {
            // ��Ÿ�� ����
            cooltime -= Time.fixedDeltaTime;

            UI_Manager.UpdateTeleportUI(dodge_cool, cooltime);

            // ��Ÿ���� 0�� �Ǿ��� ��
            if (cooltime < 0)
            {
                // �ʱ�ȭ
                cooltime = dodge_cool;
                UI_Manager.ToggleTeleportUI(true);
                is_cooltime = false;
            }
        }

        /*
        if (is_dodging)
        {
            // ������ �ִϸ��̼� �۵�
            //animator.SetFloat("Vertical", dodge_direction.normalized.z);
            //animator.SetFloat("Horizontal", dodge_direction.normalized.x);

            // ��Ÿ�� ����
            dodge_time -= Time.fixedDeltaTime;

            // ��Ÿ���� 0�� �Ǿ��� ��
            if(dodge_time < 0)
            {
                // �ʱ�ȭ
                dodge_time = dodge_anim_cool;
                dodge_direction = Vector3.zero;
                is_dodging = false;
                animator.SetBool("is_dodging", false);
            }

            // ������ ���⿡ ũ�� ����
            dodge_direction = dodge_direction.normalized * dodge_speed * Time.fixedDeltaTime;
            //dodge_direction = dodge_direction.normalized * dodge_speed;

            //rigidBody.velocity = dodge_direction;

            // ������ ũ�� ��ŭ ��ġ�� �̵���
            //transform.Translate(dodge_direction);
            rigidBody.MovePosition(transform.position + dodge_direction);
        }
*/
    }

    // ������ ���� �Ǻ��ϴ� �޼ҵ�
    private void startDodge()
    {
        // Dodge �Է��� �ޱ� and Dodge ���°� �ƴ� and ���� �Է��� 0�� �ƴ�
        if (Input.GetAxis("Dodge") == 1.0 && !is_dodging && !is_cooltime && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
        {
            // Dodge ���� true�� ����
            is_dodging = true;

            // ������ �ִϸ��̼� ����
            animator.SetBool("is_dodging", true);

            // ������ ������ �� ������ ������ �Է¹���
            dodge_direction.x = Input.GetAxis("Horizontal");
            dodge_direction.z = Input.GetAxis("Vertical");
        }
    }
}


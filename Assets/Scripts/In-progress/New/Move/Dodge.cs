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


    // 컴포넌트 호출 및 초기값 부여
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

    // 일반적인 기능 구현
    private void Update()
    {
        // 로컬 플레이어만 구르기 가능
        if (!photonView.IsMine)
        {
            return;
        }

        // 구르기 메소드
        //startDodge();
        inputManager.teleport_button.onClick.AddListener(Teleport);
    }

    private void Teleport()
    {// && (inputManager.joyStick.InputVector != Vector3.zero)
        Debug.Log("Enter Teleport");
        // Dodge 입력을 받기 and Dodge 상태가 아님 and 방향 입력이 0이 아님
        if (!is_cooltime)
        {
            dodge_direction = Vector3.zero;
            Vector3 to = transform.position;

            // 구르기 시작할 때 구르는 방향을 입력받음
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

    // 물리 관련 기능 구현
    void FixedUpdate()
    {
        // 로컬 플레이어만 구르기 가능
        if (!photonView.IsMine)
        {
            return;
        }

        if (is_cooltime)
        {
            // 쿨타임 감소
            cooltime -= Time.fixedDeltaTime;

            UI_Manager.UpdateTeleportUI(dodge_cool, cooltime);

            // 쿨타임이 0이 되었을 때
            if (cooltime < 0)
            {
                // 초기화
                cooltime = dodge_cool;
                UI_Manager.ToggleTeleportUI(true);
                is_cooltime = false;
            }
        }

        /*
        if (is_dodging)
        {
            // 구르기 애니메이션 작동
            //animator.SetFloat("Vertical", dodge_direction.normalized.z);
            //animator.SetFloat("Horizontal", dodge_direction.normalized.x);

            // 쿨타임 감소
            dodge_time -= Time.fixedDeltaTime;

            // 쿨타임이 0이 되었을 때
            if(dodge_time < 0)
            {
                // 초기화
                dodge_time = dodge_anim_cool;
                dodge_direction = Vector3.zero;
                is_dodging = false;
                animator.SetBool("is_dodging", false);
            }

            // 구르기 방향에 크기 곱함
            dodge_direction = dodge_direction.normalized * dodge_speed * Time.fixedDeltaTime;
            //dodge_direction = dodge_direction.normalized * dodge_speed;

            //rigidBody.velocity = dodge_direction;

            // 구르는 크기 만큼 위치를 이동함
            //transform.Translate(dodge_direction);
            rigidBody.MovePosition(transform.position + dodge_direction);
        }
*/
    }

    // 구르기 상태 판별하는 메소드
    private void startDodge()
    {
        // Dodge 입력을 받기 and Dodge 상태가 아님 and 방향 입력이 0이 아님
        if (Input.GetAxis("Dodge") == 1.0 && !is_dodging && !is_cooltime && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
        {
            // Dodge 상태 true로 변경
            is_dodging = true;

            // 구르기 애니메이션 적용
            animator.SetBool("is_dodging", true);

            // 구르기 시작할 때 구르는 방향을 입력받음
            dodge_direction.x = Input.GetAxis("Horizontal");
            dodge_direction.z = Input.GetAxis("Vertical");
        }
    }
}


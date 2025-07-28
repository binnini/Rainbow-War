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

    // 컴포넌트 호출
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

    // 일반적인 기능 구현
    private void Update()
    {
        // 게임이 시작하지 않았으면 입력 받지 않음
        if (!gameManager.isStart) {
            return;
        }
        // 캐릭터가 죽어있으면 입력 받지 않음
        if (status.isDead) {
            return;
        }

        // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.
        if (!photonView.IsMine)
        {
            return;
        }

        transform.position = rigidBody.transform.position;
        rigidBody.transform.position = transform.position;


        // GetComponent 정보 전달 디버그
        //Debug.Log(status.speed);

        // 이동 방향 벡터 초기화
        walk_direction = Vector3.zero;

        // 이동 방향 입력 받음
        //walk_direction.x = Input.GetAxis("Horizontal");
        walk_direction.x = joystick.InputVector.x;
        //walk_direction.z = Input.GetAxis("Vertical");
        walk_direction.z = joystick.InputVector.z;



        // 이동 방향 일반화
        walk_direction = walk_direction.normalized;

        // 대쉬 입력 받음
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

        // is_dashing 디버그
        //Debug.Log(is_dashing);
        //Debug.Log(Input.GetAxis("Horizontal") + " " + Input.GetAxis("Vertical"));
        //Debug.Log(walk_direction.x + " " + walk_direction.z);

        // 걷기 애니메이션 작동
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
        // 달리기 적용
        if (is_dashing)
        {
            walk_direction *= dash_speed * Time.fixedDeltaTime;
        }

        // 걷기 적용
        else
        {
            walk_direction *= walk_speed * Time.fixedDeltaTime;
        }

        // 회전 애니메이션
        TurnAnime();

        // 디버그
        //Debug.Log(walk_direction);

        // 이동
        //rigidBody.velocity = walk_direction;
        transform.Translate(walk_direction);
        //rigidBody.MovePosition(transform.position + walk_direction);
    }

    // 물리 관련 기능 구현
    private void FixedUpdate()
    {
        rigidBody.velocity = Vector3.zero;
        if (enable_walking)
        {
            walking();
        }
    }

    // 캐릭터 회전
    private void TurnAnime()
    {
        // 걷지 않을 경우 return함
        if (walk_direction.x == 0 && walk_direction.z == 0)
        {
            return;
        }

        // 현재 바라보는 방향을 Quaternion 형태로 변환함
        Quaternion new_rotation = Quaternion.LookRotation(walk_direction.normalized);

        // rigidBody의 회전 방향을 Slerp로 천천히 변화시킴
        rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, new_rotation, rotate_speed * Time.fixedDeltaTime);
    }
}

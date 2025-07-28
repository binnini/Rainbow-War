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


    // 컴포넌트 호출
    void Start()
    {
        rigidBody = GetComponentInChildren<Rigidbody>();
        dodge_script = GetComponent<TestDodge>();
    }

    // 일반적인 기능 구현
    private void Update()
    {
        transform.position = rigidBody.transform.position;
        rigidBody.transform.position = transform.position;


        // GetComponent 정보 전달 디버그

        // 이동 방향 벡터 초기화
        walk_direction = Vector3.zero;

        // 이동 방향 입력 받음
        walk_direction.x = Input.GetAxis("Horizontal");
        walk_direction.z = Input.GetAxis("Vertical");
        // 이동 방향 일반화
        walk_direction = walk_direction.normalized;

        // 대쉬 입력 받음
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

        // 이동
        transform.Translate(walk_direction);
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

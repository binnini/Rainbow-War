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


    // 컴포넌트 호출 및 초기값 부여
    void Start()
    {
        rigidBody = GetComponentInChildren<Rigidbody>();
        dodge_time = dodge_anim_cool;
        cooltime = dodge_cool;
    }

    // 일반적인 기능 구현
    private void Update()
    {
        // 구르기 메소드
        //startDodge();
        Teleport();
    }

    private void Teleport()
    {
        // Dodge 입력을 받기 and Dodge 상태가 아님 and 방향 입력이 0이 아님
        if (Input.GetAxis("Dodge") == 1.0 && !is_cooltime && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
        {
            dodge_direction = Vector3.zero;
            Vector3 to = transform.position;

            // 구르기 시작할 때 구르는 방향을 입력받음
            dodge_direction.x = Input.GetAxis("Horizontal");
            dodge_direction.z = Input.GetAxis("Vertical");
            Quaternion rot = Quaternion.LookRotation(dodge_direction.normalized);
            
            dodge_direction = dodge_direction.normalized * dodge_distance;
            rigidBody.MovePosition(transform.position + dodge_direction);
            
            Vector3 from = transform.position;            
            is_cooltime = true;
        }
    }

    // 물리 관련 기능 구현
    void FixedUpdate()
    {
        if (is_cooltime)
        {
            // 쿨타임 감소
            cooltime -= Time.fixedDeltaTime;


            // 쿨타임이 0이 되었을 때
            if (cooltime < 0)
            {
                // 초기화
                cooltime = dodge_cool;
                is_cooltime = false;
            }
        }
    }

    // 구르기 상태 판별하는 메소드
    private void startDodge()
    {
        // Dodge 입력을 받기 and Dodge 상태가 아님 and 방향 입력이 0이 아님
        if (Input.GetAxis("Dodge") == 1.0 && !is_dodging && !is_cooltime && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
        {
            // Dodge 상태 true로 변경
            is_dodging = true;

            // 구르기 시작할 때 구르는 방향을 입력받음
            dodge_direction.x = Input.GetAxis("Horizontal");
            dodge_direction.z = Input.GetAxis("Vertical");
        }
    }
}


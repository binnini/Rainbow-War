using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Action                                                //입력키와 상관 없이 이동 방향만 나타내는 구조체
{
    public bool Forward, Back, Left, Right, Dash, Fire;
}

public class Move_Model : MonoBehaviour
{
    public float speed = 10f;
    public float dash_speed = 20f;
    public float dash_anim_cool = 0.5f;
    public float rotate_speed = 10f;

    private float cooltime;
    private bool is_dashing;
    /*
    private Player_MoveController moveController = new Player_MoveController();
    */
    private KeyInput_Manager key_mng = new KeyInput_Manager();

    private Vector3 move_direction = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    private Action action;

    private Vector3 dash_direction = Vector3.zero;

    private CharacterController charController;
    private Animator animator;
    private Rigidbody rigidBody;



    private void Start()
    {
        //charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();

        cooltime = dash_anim_cool;
    }



    private void Update()
    {
        startDash();

        //대쉬 할 때
        if (is_dashing)
        {
            move_direction = dash_direction * dash_speed * Time.deltaTime;              // 대쉬 스피드 만큼 이동 거리 곱함

            animator.SetBool("is_dashing", true);

            animator.SetFloat("Vertical", dash_direction.normalized.z);                 // 애니메이션 작동
            animator.SetFloat("Horizontal", dash_direction.normalized.x);

            cooltime -= Time.deltaTime;

            if (cooltime < 0)
            {
                cooltime = dash_anim_cool;
                dash_direction = Vector3.zero;
                is_dashing = false;
                animator.SetBool("is_dashing", false);

                animator.SetFloat("Vertical", 0);                                       // 애니메이션 초기화
                animator.SetFloat("Horizontal", 0);
            }
        }
        //대쉬 안할 때
        else
        {
            move_direction = Vector3.zero;                                              // 이동 방향 벡터 초기화
            move_direction = getMoveDirection();                                        // Controller으로부터 방향 정보 전달받음

            animator.SetFloat("Vertical", move_direction.normalized.z);                 // 애니메이션 작동
            animator.SetFloat("Horizontal", move_direction.normalized.x);

            if (move_direction != Vector3.zero)
            {
                animator.SetBool("is_moving", true);
            }
            else
            {
                animator.SetBool("is_moving", false);
            }

            move_direction *= speed * Time.deltaTime;                                   // 방향에 이동 거리 곱함
        }

        //TurnAnime();
        Debug.Log(move_direction);
        rigidBody.MovePosition(transform.position + move_direction);

        //charController.Move(move_direction);                                            // 실제로 이동시키는 메소드
    }

    private void TurnAnime()
    {
        if(move_direction.x == 0 && move_direction.z == 0)
        {
            return;
        }

        Quaternion new_rotation = Quaternion.LookRotation(move_direction);
        rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, new_rotation, rotate_speed * Time.deltaTime);

    }

    private void FixedUpdate()
    {

    }

    private void startDash()
    {
        keyToAction(out action);

        if (action.Dash && !is_dashing && (action.Forward || action.Back || action.Left || action.Right))
        {
            is_dashing = true;
            dash_direction = this.direction;
        }
    }

    private Vector3 getMoveDirection()                                                   //이동 방향 반환하는 public 메소드
    {
        keyToAction(out action);
        direction = Vector3.zero;

        if (action.Forward)                                                             //입력키 바뀌어도 수정 안하는 부분
        {
            direction += Vector3.forward;
        }
        if (action.Left)
        {
            direction += Vector3.left;
        }
        if (action.Back)
        {
            direction += Vector3.back;
        }
        if (action.Right)
        {
            direction += Vector3.right;
        }

        return direction;
    }

    private void keyToAction(out Action action)                     //입력키 데이터 -> 이동 방향 데이터
    {
        KeyInput key_state;
        key_mng.checkInput(out key_state);
        action = new Action();

        if (key_state.W)                                                                // 입력키 수정 시 이곳을 수정함
        {
            action.Forward = true;
        }
        if (key_state.A)
        {
            action.Left = true;
        }
        if (key_state.S)
        {
            action.Back = true;
        }
        if (key_state.D)
        {
            action.Right = true;
        }
        if (key_state.Space)
        {
            action.Dash = true;
        }

        return;
    }
}

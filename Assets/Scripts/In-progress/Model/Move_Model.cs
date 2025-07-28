using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Action                                                //�Է�Ű�� ��� ���� �̵� ���⸸ ��Ÿ���� ����ü
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

        //�뽬 �� ��
        if (is_dashing)
        {
            move_direction = dash_direction * dash_speed * Time.deltaTime;              // �뽬 ���ǵ� ��ŭ �̵� �Ÿ� ����

            animator.SetBool("is_dashing", true);

            animator.SetFloat("Vertical", dash_direction.normalized.z);                 // �ִϸ��̼� �۵�
            animator.SetFloat("Horizontal", dash_direction.normalized.x);

            cooltime -= Time.deltaTime;

            if (cooltime < 0)
            {
                cooltime = dash_anim_cool;
                dash_direction = Vector3.zero;
                is_dashing = false;
                animator.SetBool("is_dashing", false);

                animator.SetFloat("Vertical", 0);                                       // �ִϸ��̼� �ʱ�ȭ
                animator.SetFloat("Horizontal", 0);
            }
        }
        //�뽬 ���� ��
        else
        {
            move_direction = Vector3.zero;                                              // �̵� ���� ���� �ʱ�ȭ
            move_direction = getMoveDirection();                                        // Controller���κ��� ���� ���� ���޹���

            animator.SetFloat("Vertical", move_direction.normalized.z);                 // �ִϸ��̼� �۵�
            animator.SetFloat("Horizontal", move_direction.normalized.x);

            if (move_direction != Vector3.zero)
            {
                animator.SetBool("is_moving", true);
            }
            else
            {
                animator.SetBool("is_moving", false);
            }

            move_direction *= speed * Time.deltaTime;                                   // ���⿡ �̵� �Ÿ� ����
        }

        //TurnAnime();
        Debug.Log(move_direction);
        rigidBody.MovePosition(transform.position + move_direction);

        //charController.Move(move_direction);                                            // ������ �̵���Ű�� �޼ҵ�
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

    private Vector3 getMoveDirection()                                                   //�̵� ���� ��ȯ�ϴ� public �޼ҵ�
    {
        keyToAction(out action);
        direction = Vector3.zero;

        if (action.Forward)                                                             //�Է�Ű �ٲ� ���� ���ϴ� �κ�
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

    private void keyToAction(out Action action)                     //�Է�Ű ������ -> �̵� ���� ������
    {
        KeyInput key_state;
        key_mng.checkInput(out key_state);
        action = new Action();

        if (key_state.W)                                                                // �Է�Ű ���� �� �̰��� ������
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//입력받은 키 정보 보관하는 구조체
public struct KeyInput
{
    public bool W, A, S, D, Shift, Space, MB1;
}

public class KeyInput_Manager
{
    //입력받은 키 정보 반환하는 public 메소드
    public void checkInput(out KeyInput key_state)
    {
        key_state = new KeyInput();

        if (Input.GetKey(KeyCode.W))
        {
            key_state.W = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            key_state.A = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            key_state.S = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            key_state.D = true;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            key_state.Shift = true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            key_state.Space = true;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            key_state.MB1 = true;
        }

        return;
    }
    

}

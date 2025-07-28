using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Toggle paint_button;
    public Button teleport_button;

    public JoyStick joyStick;
    public JoyStick joyStick_shoot;


    // Start is called before the first frame update
    void Start()
    {
        joyStick = GameObject.Find("HUD Canvas").GetComponentsInChildren<JoyStick>()[0];
        joyStick_shoot = GameObject.Find("HUD Canvas").GetComponentsInChildren<JoyStick>()[1];
        paint_button = GameObject.Find("PaintButton").GetComponent<Toggle>();
        teleport_button = GameObject.Find("Teleport").GetComponentInChildren<Button>();
        //dash_button.onValueChanged.AddListener(DashButton_ValueChanged);
    }

    // Update is called once per frame
    void Update()
    {
        //teleport_button.onClick.AddListener(debug_log);
    }

    void debug_log()
    {
        Debug.Log("teleport onClick");
    }

    private void DashButton_ValueChanged(bool _bool) {        
        Debug.Log("Toggle Click!\n" + _bool);        
    }
}

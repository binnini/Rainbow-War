using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Test : MonoBehaviour
{
    public float speed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bool keyInput_Right;
        bool keyInput_Left;
        bool keyInput_Up;
        bool keyInput_Down;

        keyInput_Right = false;
        keyInput_Left = false;
        keyInput_Up = false;
        keyInput_Down = false;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            keyInput_Right = true;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            keyInput_Left = true;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            keyInput_Up = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            keyInput_Down = true;
        }


        if (keyInput_Right)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        if (keyInput_Left)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }
    }

}

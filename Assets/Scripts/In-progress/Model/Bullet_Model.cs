using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Model : MonoBehaviour
{
    public float DestroyTime = 2.0f;
    public float bullet_speed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, DestroyTime);       // DestroyTime �� �ڵ�����
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * bullet_speed);        // ������ �Ѿ� �̵�
    }

    
}

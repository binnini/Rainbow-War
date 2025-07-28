using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting_Data : MonoBehaviour
{
    public float master_volume = 1;
    public float bgm_volume = 0.5f;
    public float sfx_volume = 0.5f;

    private static Setting_Data m_instance;
    public static Setting_Data instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<Setting_Data>();
            }

            return m_instance;
        }
    }
    private void Awake()
    {
        // 씬에 싱글턴 오브젝트가 된 다른 Sound_Manager 오브젝트가 있다면
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        
    }
}

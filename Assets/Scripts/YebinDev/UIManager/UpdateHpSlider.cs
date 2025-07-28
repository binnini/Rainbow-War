using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHpSlider : MonoBehaviour
{
    public GameManager gameManager;

    // 체력바 업데이트
    private void FixedUpdate()
    {
        if (!gameManager.isGameover)
        {
            UIManager.instance.UpdatePlayerHpSlider();
            UIManager.instance.UpdateHpSliderList();            
        }
    }
}

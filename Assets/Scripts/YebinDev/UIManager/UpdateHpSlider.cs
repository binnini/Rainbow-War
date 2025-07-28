using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHpSlider : MonoBehaviour
{
    public GameManager gameManager;

    // ü�¹� ������Ʈ
    private void FixedUpdate()
    {
        if (!gameManager.isGameover)
        {
            UIManager.instance.UpdatePlayerHpSlider();
            UIManager.instance.UpdateHpSliderList();            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameManager : MonoBehaviour
{
    public List<HpUI> HpUIList = new List<HpUI>();
    
    // 체력 구조체
    public struct HpUI {
        public Slider hpSlider;
        public GameObject targetPlayer;
    }
}

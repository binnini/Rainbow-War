using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.EventSystems;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class UIManager : MonoBehaviourPun
{
    // �̱��� ���ٿ� ������Ƽ
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }
            return m_instance;
        }
    }

    private static UIManager m_instance;    // �̱����� �Ҵ�� ����
    private Sound_Manager sound_mng;
    
    [Header ("������ UI")]
    public GameObject scoreBoard;   // ������
    public Image Team_1_Board;  // �� 1 ������
    public Image Team_2_Board;  // �� 2 ������
    public Text Team_1_ScoreText;      // �� 1 ���� ǥ�ÿ� �ؽ�Ʈ
    public Text Team_2_ScoreText;      // �� 2 ���� ǥ�ÿ� �ؽ�Ʈ
    public Sprite[] teamScoreBoard;  // �� ������ �̹��� ��������Ʈ    
    public Text timerText;     // Ÿ�̸� �ؽ�Ʈ

    [Header ("���â UI")]
    public GameObject resultBoard;  // ���â
    public GameObject winnerBG;  // �¸� ���â
    public GameObject loseBG;  // �й� ���â
    public Text Team_1_ResultText;  // ������ ���
    public Text Team_2_ResultText; // ����� ���
    public Text winnerText;         // ���� �ؽ�Ʈ
    public Button exitButton;       // �ΰ��ӿ��� �κ�� ���ư��� ��ư
    public Image team_1_Image;  // �� 1 �̹���
    public Image team_2_Image;  // �� 2 �̹���
    public Sprite[] teamImage;  // �� ��� �̹��� ��������Ʈ
    

    [Header ("źâ UI")]
    public Text ammoText;      // ź�� ǥ�ÿ� �ؽ�Ʈ
    public Image[] ammoList;  // ź�� UI ����Ʈ  


    [Header ("ü�� UI")]
    public Slider HpSlider;    // ü�� ǥ�ÿ� �����̴�
    public GameObject HpSliderPrefab;   // ü�� �����̴� ������
    public List<HpUI> HpUIList = new List<HpUI>();
    
    [Header ("�̸�ǥ UI")]
    private Text PlayerName;    // �̸� ǥ�ÿ� �ؽ�Ʈ
    public GameObject PlayerNamePrefab;   // �÷��̾� �̸� ������
    
    // ü�� & �̸� ����ü
    public struct HpUI {
        public Slider hpSlider;
        public GameObject targetPlayer;
        public Text playerName;    
    }

    
    [Header ("ī��Ʈ�ٿ� UI")]
    public GameObject countDownBG;    // ī��Ʈ �ٿ�â ��׶���        
    public Sprite[] countDownSpriteList;    // ī��Ʈ �ٿ� ���� ��������Ʈ ����Ʈ
    public GameObject[] countDownBGList;


    public GameObject settingBG;    // ����â ��׶���
    
    public GameObject respawnBG;    // ������ UI ��׶���
    public GameObject[] respawnTextList;
    public Slider respawnSlider;    // ������ �ð� ǥ�ÿ� �����̴�
    public Image killerIcon;    // ���� �÷��̾��� ������
    public Sprite[] iconList;   // ������ ����Ʈ

    public Text teleportCoolText;  
    public Image[] teleportUI;  // ��ۿ� �ڷ���Ʈ UI


    public GameObject noticeBG; // ��� ǥ�ÿ� UI
    public GameObject noticeText_Prefab;


    
    [Header ("�ܺ� ������Ʈ ���۷���")]
    public GameObject player;
    public GameObject enemy;
    public Status playerStatus;     // Player �������ͽ�
    public Status enemyStatus;      // Enemy �������ͽ�
    

    [Header ("Paint ������ ��. ����ҰŸ� �̰� �ᵵ ��")]
    public Slider tempPaintSlider;
    public Slider MyHPSlider;

    bool flag;

    public Toggle teleport_toggle;
    

    private void Start()
    {
        PlayerName = Instantiate(PlayerNamePrefab,transform).GetComponentInChildren<Text>();
        PlayerName.text = playerStatus.photonView.Owner.NickName;
        sound_mng = GameObject.Find("Sound_Manager").GetComponent<Sound_Manager>();
        
    }

    
    public void ChangeCountDownNum(float num) {        
        if (num > 4)
        {
            countDownBGList[0].SetActive(true);
            return;
        }
        else if (num > 3) {
            //countDownBG.transform.GetChild(0).GetComponent<Image>().sprite = countDownSpriteList[0];
            countDownBGList[0].SetActive(false);
            countDownBGList[1].SetActive(true);
        }
        else if (num > 2)
        {
            //countDownBG.transform.GetChild(0).GetComponent<Image>().sprite = countDownSpriteList[1];
            countDownBGList[1].SetActive(false);
            countDownBGList[2].SetActive(true);
        }
        else if (num > 1) {
            //countDownBG.transform.GetChild(0).GetComponent<Image>().sprite = countDownSpriteList[2];
            countDownBGList[2].SetActive(false);
            countDownBGList[3].SetActive(true);
        }
        else if (num > 0) {
            //countDownBG.transform.GetChild(0).GetComponent<Image>().sprite = countDownSpriteList[3];
            countDownBGList[3].SetActive(false);
            countDownBGList[4].SetActive(true);
        }
    }
    
    private void Update()
    {   
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     ShowSettingUI();
        // }

        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     UpdateRespawnUI(5f,5f,"�׽�Ʈ",0);
        // }
    }
    
    // HpSlider ĵ������ �߰�
    public void AddHpSlider(GameObject newPlayer)
    {
        HpUI newHpUI;
        newHpUI.hpSlider = Instantiate(HpSliderPrefab, transform).GetComponent<Slider>();
        newHpUI.playerName = Instantiate(PlayerNamePrefab,transform).GetComponentInChildren<Text>();
        newHpUI.playerName.text = newPlayer.GetPhotonView().Owner.NickName;
        newHpUI.targetPlayer = newPlayer;
        HpUIList.Add(newHpUI);
    }

    private void ShowSettingUI()
    {
        if (settingBG.activeSelf)
        {
            HpSlider.gameObject.SetActive(true);
            PlayerName.gameObject.SetActive(true);
            SetActiveAllHpSlider(true);
            settingBG.SetActive(false);
        }
        else
        {
            HpSlider.gameObject.SetActive(false);
            PlayerName.gameObject.SetActive(false);
            SetActiveAllHpSlider(false);
            settingBG.SetActive(true);
        }       
    }
    public void ToggleSettingBG()
    {
        settingBG.SetActive(!settingBG.activeSelf);
    }

    public void UpdateAmmoText(int max_ammo, int ammo, bool is_reloading)
    {
        if (is_reloading)
        {
            ammoText.text = "��������...";
        }
        else
        {
            ammoText.text = ammo.ToString() + " / " + max_ammo.ToString();
        }
    }

    public void UpdateAmmoList(int max_ammo, int ammo) 
    {
        if (ammo == max_ammo) {
            foreach(Image image in ammoList) {
                image.enabled = true;
            }
        }        
        else {
            ammoList[ammo].enabled = false;
        }
    }

    // �÷��̾��� ü�� �����̴� ����
    public void RemoveHpSlider(GameObject gonePlayer)
    {
        foreach (var hpUI in HpUIList)
        {
            if (hpUI.targetPlayer == gonePlayer)
            {
                HpUIList.Remove(hpUI);
                return;
            }
        }
    }

    public void SetActiveAllHpSlider(bool value)
    {       
        foreach(HpUI hpui in HpUIList)
        {
            hpui.hpSlider.gameObject.SetActive(value);
            hpui.playerName.gameObject.SetActive(value);
        }
    }

 

    // ������ ������Ʈ
    public void UpdateScoreBoard(float team_1_score, float team_2_score)
    {
        if (team_1_score ==0 && team_2_score == 0)
            return;


        // Slider scoreBoard_slider = scoreBoard.GetComponent<Slider>();
        // scoreBoard_slider.value = redScore / (redScore + blueScore);
        
        Team_1_ScoreText.text = Mathf.Round((team_1_score / (team_2_score + team_1_score) *100)).ToString();
        Team_2_ScoreText.text = Mathf.Round((team_2_score / (team_2_score + team_1_score) *100)).ToString();
    }

    // ������ ���ھ� �ؽ�Ʈ ������Ʈ
    public void UpdateRedScoreText(float newScore)
    {
        Team_1_ScoreText.text = newScore.ToString();
    }

    // ����� ���ھ� �ؽ�Ʈ ������Ʈ
    public void UpdateBlueScoreText(float newScore)
    {
        Team_2_ScoreText.text = newScore.ToString();
    }

    // �ڱ� �ڽ��� ü�¹� ������Ʈ
    public void UpdatePlayerHpSlider()
    {
        HpSlider.transform.position = Camera.main.WorldToScreenPoint(player.transform.position + new Vector3(0, 1f, 1));
        PlayerName.transform.position = Camera.main.WorldToScreenPoint(player.transform.position + new Vector3(0, 1.5f, 1));
        MyHPSlider.value = playerStatus.health / playerStatus.max_health;
        HpSlider.value = playerStatus.health / playerStatus.max_health;
    }

    // �ڱ� �ڽ��� ������ �÷��̾���� ü�¹� & �̸�ǥ ������Ʈ
    public void UpdateHpSliderList()
    {
        foreach (HpUI hpUI in HpUIList)
        {
            hpUI.hpSlider.transform.position = Camera.main.WorldToScreenPoint(hpUI.targetPlayer.transform.position + new Vector3(0, 1f, 1));
            hpUI.playerName.transform.position = Camera.main.WorldToScreenPoint(hpUI.targetPlayer.transform.position + new Vector3(0, 1.5f, 1));
            hpUI.hpSlider.value = hpUI.targetPlayer.transform.GetComponent<Status>().health / hpUI.targetPlayer.transform.GetComponent<Status>().max_health;
        }
    }

    // 
    public void UpdateTempPaintSlider(float curPaint, float max_Paint)
    {               
        tempPaintSlider.value = curPaint / max_Paint;
    }

    public void UpdateRespawnUI(float respawn_maxTime, float respawn_remainTime, string killerName, int iconNum)
    {   
        killerIcon.sprite = iconList[iconNum];
        
        respawnSlider.value = respawn_remainTime/respawn_maxTime;
        respawnTextList[0].GetComponent<Text>().text = "<color=lime>" + Math.Round(respawn_remainTime, 1) + "</color>" + " �� �� ������������ ������ �˴ϴ�...";
        respawnTextList[1].GetComponent<Text>().text = "<color=aqua>" + killerName + "</color>" + "�� ���� " + "<color=fuchsia>" + "óġ " + "</color>"+ "���߽��ϴ�.";
    }  

    public void UpdateTeleportUI(float cooltime_max, float cooltime_now)
    {
        teleportCoolText.text = "" + Math.Round(cooltime_now, 2);
    }

    public void ToggleTeleportUI(bool value)
    {
        teleportUI[0].enabled = value;
        //teleportUI[1].enabled = !value;
        teleportCoolText.gameObject.SetActive(!value);
    }

    public void SetNoticeText(string content) {
        GameObject temp = Instantiate(noticeText_Prefab,noticeBG.transform.position,Quaternion.identity,noticeBG.transform);
        temp.GetComponent<Text>().text = content;
        
    }
    public void OnDeath(GameObject dead_object)
    {
        // �׾��� ���� UI ���� ����� �߰� �Ǿ����                
        HpSlider.gameObject.SetActive(false); 
        respawnBG.SetActive(true);        
        
        
        // ��Ƽ���� ü�� UI ��Ȱ��ȭ�ϱ�
        photonView.RPC("ToggleHpSlider",RpcTarget.Others,PhotonNetwork.LocalPlayer);
        return;
    }


    public void OnRespawn(GameObject respawn_object)
    {
        // ������ ���� UI ���� ����� �߰� �Ǿ����
        HpSlider.gameObject.SetActive(true);        
        respawnBG.SetActive(false);


        // ��Ƽ���� ü�� UI Ȱ��ȭ�ϱ�
        photonView.RPC("ToggleHpSlider",RpcTarget.Others,PhotonNetwork.LocalPlayer);
        return;
    }



    // ���â �����ֱ�
    public void ShowResult(float team_1_score, float team_2_score, bool win)
    {
        resultBoard.SetActive(true);
        Team_1_ResultText.text = "" + Math.Round(team_1_score * 1000)/1000;
        Team_2_ResultText.text = "" + Math.Round(team_2_score * 1000)/1000;
        

        if (win)
        {
            winnerBG.SetActive(true);
            playVictorySound();            
        }
        else
        {
            loseBG.SetActive(true);
            playDefeatSound();
        }
    }
    
    

    private void playVictorySound()
    {
        sound_mng.SFXPlay(Sound_Manager.SFXName.Victory);
    }
    private void playDefeatSound()
    {
        sound_mng.SFXPlay(Sound_Manager.SFXName.Defeat);
    }
    
    // ��Ƽ���� HpSlider ��Ȱ��ȭ�ϱ�
    [PunRPC]
    public void ToggleHpSlider(Player targetPlayer)
    {
        GameObject obj = SearchHpSliderByOwner(targetPlayer).hpSlider.gameObject;
        obj.SetActive(!obj.activeSelf);
    }

    // ��Ʈ��ũ �÷��̾�� HpSlider ã��
    public HpUI SearchHpSliderByOwner(Player targetPlayer)
    {
        foreach (var hpUI in HpUIList)
        {
            if (hpUI.targetPlayer.GetComponent<PhotonView>().Owner == targetPlayer)
            {                
                return hpUI;
            }
        }

        // ���н� �� UI ��ȯ
        HpUI temp = new HpUI();
        return temp;
    }

    public void UpdateTeamColor(GameObject playerObj) {

        Player player = playerObj.GetComponent<PhotonView>().Owner;
        Hashtable playerCP = player.CustomProperties;   
        Sprite tempScoreBoard_Sprite;
        Sprite tempResultImage_Sprite;            

        if (playerCP["Color"].ToString().Equals("#FF0000") ) {                //teamScoreBoard[0]
            Debug.Log("���� ����");
            tempResultImage_Sprite = teamImage[0];
            tempScoreBoard_Sprite = teamScoreBoard[0];
        }
        else if (playerCP["Color"].ToString().Equals("#FFA500") ) {
            Debug.Log("��Ȳ ����");
            tempResultImage_Sprite = teamImage[1];
            tempScoreBoard_Sprite = teamScoreBoard[1];
        }
        else if (playerCP["Color"].ToString().Equals("#FFFF00") ) {
            Debug.Log("��� ����");
            tempResultImage_Sprite = teamImage[2];
            tempScoreBoard_Sprite = teamScoreBoard[2];
        }
        else if (playerCP["Color"].ToString().Equals("#008000") ) {
            Debug.Log("�ʷ� ����");
            tempResultImage_Sprite = teamImage[3];
            tempScoreBoard_Sprite = teamScoreBoard[3];
        }
        else if (playerCP["Color"].ToString().Equals("#0000FF") ) {
            Debug.Log("�Ķ� ����");
            tempResultImage_Sprite = teamImage[4];
            tempScoreBoard_Sprite = teamScoreBoard[4];
        }   
        else if (playerCP["Color"].ToString().Equals("#000080") ) {
            Debug.Log("���� ����");
            tempResultImage_Sprite = teamImage[5];
            tempScoreBoard_Sprite = teamScoreBoard[5];
        }
        else if (playerCP["Color"].ToString().Equals("#800080") ) {
            Debug.Log("���� ����");
            tempResultImage_Sprite = teamImage[6];
            tempScoreBoard_Sprite = teamScoreBoard[6];
        }
        else {
            Debug.Log("else ����");
            tempResultImage_Sprite = teamImage[0];
            tempScoreBoard_Sprite = teamScoreBoard[0];
        }

        if (player.GetPhotonTeam().Name.Equals("Red"))
        {
            team_1_Image.sprite = tempResultImage_Sprite;
            Team_1_Board.sprite = tempScoreBoard_Sprite;
        }
        else if (player.GetPhotonTeam().Name.Equals("Blue"))
        {
            team_2_Image.sprite = tempResultImage_Sprite;
            Team_2_Board.sprite = tempScoreBoard_Sprite;
        }
        else 
        {
            return;
        }
    }
}

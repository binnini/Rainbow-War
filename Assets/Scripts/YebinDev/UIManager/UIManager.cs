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
    // 싱글턴 접근용 프로퍼티
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

    private static UIManager m_instance;    // 싱글턴이 할당될 변수
    private Sound_Manager sound_mng;
    
    [Header ("점수판 UI")]
    public GameObject scoreBoard;   // 점수판
    public Image Team_1_Board;  // 팀 1 점수판
    public Image Team_2_Board;  // 팀 2 점수판
    public Text Team_1_ScoreText;      // 팀 1 점수 표시용 텍스트
    public Text Team_2_ScoreText;      // 팀 2 점수 표시용 텍스트
    public Sprite[] teamScoreBoard;  // 팀 점수판 이미지 스프라이트    
    public Text timerText;     // 타이머 텍스트

    [Header ("결과창 UI")]
    public GameObject resultBoard;  // 결과창
    public GameObject winnerBG;  // 승리 결과창
    public GameObject loseBG;  // 패배 결과창
    public Text Team_1_ResultText;  // 레드팀 결과
    public Text Team_2_ResultText; // 블루팀 결과
    public Text winnerText;         // 승자 텍스트
    public Button exitButton;       // 인게임에서 로비로 돌아가기 버튼
    public Image team_1_Image;  // 팀 1 이미지
    public Image team_2_Image;  // 팀 2 이미지
    public Sprite[] teamImage;  // 팀 결과 이미지 스프라이트
    

    [Header ("탄창 UI")]
    public Text ammoText;      // 탄알 표시용 텍스트
    public Image[] ammoList;  // 탄알 UI 리스트  


    [Header ("체력 UI")]
    public Slider HpSlider;    // 체력 표시용 슬라이더
    public GameObject HpSliderPrefab;   // 체력 슬라이더 프리팹
    public List<HpUI> HpUIList = new List<HpUI>();
    
    [Header ("이름표 UI")]
    private Text PlayerName;    // 이름 표시용 텍스트
    public GameObject PlayerNamePrefab;   // 플레이어 이름 프리팹
    
    // 체력 & 이름 구조체
    public struct HpUI {
        public Slider hpSlider;
        public GameObject targetPlayer;
        public Text playerName;    
    }

    
    [Header ("카운트다운 UI")]
    public GameObject countDownBG;    // 카운트 다운창 백그라운드        
    public Sprite[] countDownSpriteList;    // 카운트 다운 숫자 스프라이트 리스트
    public GameObject[] countDownBGList;


    public GameObject settingBG;    // 설정창 백그라운드
    
    public GameObject respawnBG;    // 리스폰 UI 백그라운드
    public GameObject[] respawnTextList;
    public Slider respawnSlider;    // 리스폰 시간 표시용 슬라이더
    public Image killerIcon;    // 죽인 플레이어의 아이콘
    public Sprite[] iconList;   // 아이콘 리스트

    public Text teleportCoolText;  
    public Image[] teleportUI;  // 토글용 텔레포트 UI


    public GameObject noticeBG; // 경고 표시용 UI
    public GameObject noticeText_Prefab;


    
    [Header ("외부 오브젝트 레퍼런스")]
    public GameObject player;
    public GameObject enemy;
    public Status playerStatus;     // Player 스테이터스
    public Status enemyStatus;      // Enemy 스테이터스
    

    [Header ("Paint 게이지 바. 사용할거면 이거 써도 됨")]
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
        //     UpdateRespawnUI(5f,5f,"테스트",0);
        // }
    }
    
    // HpSlider 캔버스에 추가
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
            ammoText.text = "재장전중...";
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

    // 플레이어의 체력 슬라이더 제거
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

 

    // 점수판 업데이트
    public void UpdateScoreBoard(float team_1_score, float team_2_score)
    {
        if (team_1_score ==0 && team_2_score == 0)
            return;


        // Slider scoreBoard_slider = scoreBoard.GetComponent<Slider>();
        // scoreBoard_slider.value = redScore / (redScore + blueScore);
        
        Team_1_ScoreText.text = Mathf.Round((team_1_score / (team_2_score + team_1_score) *100)).ToString();
        Team_2_ScoreText.text = Mathf.Round((team_2_score / (team_2_score + team_1_score) *100)).ToString();
    }

    // 레드팀 스코어 텍스트 업데이트
    public void UpdateRedScoreText(float newScore)
    {
        Team_1_ScoreText.text = newScore.ToString();
    }

    // 블루팀 스코어 텍스트 업데이트
    public void UpdateBlueScoreText(float newScore)
    {
        Team_2_ScoreText.text = newScore.ToString();
    }

    // 자기 자신의 체력바 업데이트
    public void UpdatePlayerHpSlider()
    {
        HpSlider.transform.position = Camera.main.WorldToScreenPoint(player.transform.position + new Vector3(0, 1f, 1));
        PlayerName.transform.position = Camera.main.WorldToScreenPoint(player.transform.position + new Vector3(0, 1.5f, 1));
        MyHPSlider.value = playerStatus.health / playerStatus.max_health;
        HpSlider.value = playerStatus.health / playerStatus.max_health;
    }

    // 자기 자신을 제외한 플레이어들의 체력바 & 이름표 업데이트
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
        respawnTextList[0].GetComponent<Text>().text = "<color=lime>" + Math.Round(respawn_remainTime, 1) + "</color>" + " 초 후 시작지점에서 리스폰 됩니다...";
        respawnTextList[1].GetComponent<Text>().text = "<color=aqua>" + killerName + "</color>" + "님 에게 " + "<color=fuchsia>" + "처치 " + "</color>"+ "당했습니다.";
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
        // 죽었을 때의 UI 관련 기능이 추가 되어야함                
        HpSlider.gameObject.SetActive(false); 
        respawnBG.SetActive(true);        
        
        
        // 멀티에서 체력 UI 비활성화하기
        photonView.RPC("ToggleHpSlider",RpcTarget.Others,PhotonNetwork.LocalPlayer);
        return;
    }


    public void OnRespawn(GameObject respawn_object)
    {
        // 리스폰 때의 UI 관련 기능이 추가 되어야함
        HpSlider.gameObject.SetActive(true);        
        respawnBG.SetActive(false);


        // 멀티에서 체력 UI 활성화하기
        photonView.RPC("ToggleHpSlider",RpcTarget.Others,PhotonNetwork.LocalPlayer);
        return;
    }



    // 결과창 보여주기
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
    
    // 멀티에서 HpSlider 비활성화하기
    [PunRPC]
    public void ToggleHpSlider(Player targetPlayer)
    {
        GameObject obj = SearchHpSliderByOwner(targetPlayer).hpSlider.gameObject;
        obj.SetActive(!obj.activeSelf);
    }

    // 네트워크 플레이어로 HpSlider 찾기
    public HpUI SearchHpSliderByOwner(Player targetPlayer)
    {
        foreach (var hpUI in HpUIList)
        {
            if (hpUI.targetPlayer.GetComponent<PhotonView>().Owner == targetPlayer)
            {                
                return hpUI;
            }
        }

        // 실패시 빈 UI 반환
        HpUI temp = new HpUI();
        return temp;
    }

    public void UpdateTeamColor(GameObject playerObj) {

        Player player = playerObj.GetComponent<PhotonView>().Owner;
        Hashtable playerCP = player.CustomProperties;   
        Sprite tempScoreBoard_Sprite;
        Sprite tempResultImage_Sprite;            

        if (playerCP["Color"].ToString().Equals("#FF0000") ) {                //teamScoreBoard[0]
            Debug.Log("빨강 실행");
            tempResultImage_Sprite = teamImage[0];
            tempScoreBoard_Sprite = teamScoreBoard[0];
        }
        else if (playerCP["Color"].ToString().Equals("#FFA500") ) {
            Debug.Log("주황 실행");
            tempResultImage_Sprite = teamImage[1];
            tempScoreBoard_Sprite = teamScoreBoard[1];
        }
        else if (playerCP["Color"].ToString().Equals("#FFFF00") ) {
            Debug.Log("노랑 실행");
            tempResultImage_Sprite = teamImage[2];
            tempScoreBoard_Sprite = teamScoreBoard[2];
        }
        else if (playerCP["Color"].ToString().Equals("#008000") ) {
            Debug.Log("초록 실행");
            tempResultImage_Sprite = teamImage[3];
            tempScoreBoard_Sprite = teamScoreBoard[3];
        }
        else if (playerCP["Color"].ToString().Equals("#0000FF") ) {
            Debug.Log("파랑 실행");
            tempResultImage_Sprite = teamImage[4];
            tempScoreBoard_Sprite = teamScoreBoard[4];
        }   
        else if (playerCP["Color"].ToString().Equals("#000080") ) {
            Debug.Log("남색 실행");
            tempResultImage_Sprite = teamImage[5];
            tempScoreBoard_Sprite = teamScoreBoard[5];
        }
        else if (playerCP["Color"].ToString().Equals("#800080") ) {
            Debug.Log("보라 실행");
            tempResultImage_Sprite = teamImage[6];
            tempScoreBoard_Sprite = teamScoreBoard[6];
        }
        else {
            Debug.Log("else 실행");
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

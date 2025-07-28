using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    // 싱글턴 접근용 프로퍼티
    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }

            return m_instance;
        }
    }

    // 속성
    private static GameManager m_instance;
    public GameObject playerPrefab;
    public GameObject redTeamplayerPrefab;
    public GameObject orangeTeamplayerPrefab;
    public GameObject yellowTeamplayerPrefab;
    public GameObject greenTeamplayerPrefab;
    public GameObject blueTeamplayerPrefab;
    public GameObject navyTeamplayerPrefab;
    public GameObject purpleTeamplayerPrefab;
    public GameObject TeamManager;
    public GameObject team_1_rewpawn_point;
    public GameObject team_2_rewpawn_point;
    public GameObject team_3_rewpawn_point;
    public UIManager UI_Manager;

    public List<GameObject> playerList;
    public bool isGameover { get; private set; } = false;
    private GameObject localPlayer;
    public Camera_Model mainCamera;

    // 시간
    public float limitTime = 60;

    private float play_time = 0;
    private float score = 0;

    // 점수 프로퍼티
    private float team_1_score = 0;
    private float team_2_score = 0;

    public Sound_Manager sound_mng;
    
    // 카운트 다운        
    public bool isStart = false;
    public bool countDownSoundPlayed = false;
    public float countDownTime = 4f;       

    bool startTimer = false;
    double timerIncrementValue;
    double startTime = 0;

    [SerializeField] double waitTime = 10;
    ExitGames.Client.Photon.Hashtable CustomeValue;        
    
    private void Awake()
    {
        // 씬에 싱글턴 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    bool flag = true;
    bool startComplete = false;
    bool isColorSet = false;
    private void Start()
    {
        //Debug.Log(photonView.Owner);
        TeamManager = GameObject.Find("TeamManager");

        sound_mng = GameObject.Find("Sound_Manager").GetComponent<Sound_Manager>();
        sound_mng.BGMPlay(Sound_Manager.BGMName.Ingame);

        CreatePlayer(); 
        // SetMyColor();


        // localPlayer.GetComponent<Status>().SetColorOnServer();        
              
        // photonView.RPC("RPCSetOthersTeam",RpcTarget.Others);
        // photonView.RPC("RPCSetOthersColorOnServer",RpcTarget.Others);
        photonView.RPC("SetAreaOnServer", RpcTarget.Others);     
        
        if (PhotonNetwork.IsMasterClient)
        {            
            startTime = PhotonNetwork.Time;            
            startTimer = true;                      
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable{{"StartTime",startTime}});            
        }

        startComplete = true;
    }
 
 
    private void Update()
    {           
        // 리모트에서 시작 시간 받아오기        
        if(!PhotonNetwork.IsMasterClient && startTime == 0 && PhotonNetwork.CurrentRoom.CustomProperties.Count>1) {
            
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            startTimer = true;            
        }

        if (!startTimer) return;
        timerIncrementValue = PhotonNetwork.Time - startTime;
        
        if (timerIncrementValue >= waitTime)
        {
            // 색 설정
            if (!isColorSet) {
                GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
                if (objs.Length == PhotonNetwork.PlayerList.Length) {
                    foreach (var obj in objs) {                    
                        obj.GetComponent<Status>().SetPlayerColor();
                        UI_Manager.UpdateTeamColor(obj);
                    }          
                    isColorSet = true;
                }                
            }

            // 카운트 다운
            if (!isStart) {           
                countDownTime -= Time.deltaTime;    
                UI_Manager.ChangeCountDownNum(countDownTime);  

                // 시작 카운트 다운 효과음 한번만 재생
                if (!countDownSoundPlayed && countDownTime > 3 && countDownTime < 4) 
                {
                    Debug.Log("시작 대기");
                    sound_mng.SFXPlay(Sound_Manager.SFXName.CountDown); 
                    countDownSoundPlayed = true;
                }
                
                // 카운트 다운 완료 후 게임 시작
                if (countDownTime < 0) {
                    isStart = true;
                    UI_Manager.countDownBG.SetActive(false);   
                }                                     
                return;                        
            }
        }       

        
        // // esc를 누르면 설정 창 열기
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     UI_Manager.ToggleSettingBG();
        //     //LeaveRoom();
        // }
 
        
        // // 탭 눌러서 점수판 확인        
        // if (Input.GetKey(KeyCode.Tab))
        // {
        //     if (PhotonNetwork.IsMasterClient)
        //     {
        //         photonView.RPC("RPCShowResult", RpcTarget.All,team_1_score, team_2_score);
        //     }            
        // }        
        

        // 우선 Update에서 점수판 동기화. 비효율적이니 나중에 수정할 것.
        if (!isGameover && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPCUpdateScoreBoard", RpcTarget.All, team_1_score, team_2_score);
        }
        
        // 제한시간 지나면 결과창 출력
        if (!isGameover && isStart)
        {
            play_time += Time.deltaTime;
            UIManager.instance.timerText.text = "" + Mathf.Round(limitTime - play_time);
        }
        if (play_time >= limitTime && !isGameover)
        {
            isGameover = true;
            UIManager.instance.timerText.text = "시간종료!";
            sound_mng.BGMPlay(Sound_Manager.BGMName.Stop);

            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPCShowResult", RpcTarget.All,team_1_score, team_2_score);
            }
        }
    }    

    [PunRPC]
    private void RPCUpdateScoreBoard(float team_1_score, float team_2_score)
    {
        UIManager.instance.UpdateScoreBoard(team_1_score, team_2_score);
    }


    [PunRPC]
    private void RPCShowResult(float team_1_score, float team_2_score)
    {
        int team = TeamManager.GetComponent<TeamManager>().localPlayerTeam;
        bool win = false;
        if (team == 1 && team_1_score > team_2_score) {
            win = true;
        }
        else if (team == 2 && team_2_score > team_1_score) {
            win = true;
        }

        UIManager.instance.ShowResult(team_1_score, team_2_score, win);
        //UIManager.instance.InactiveHpSliders();
    }


    // 디버그용 더미생성 메소드
    private void CreateDummy(Vector3 spawnPoint)
    {
        GameObject new_dummy = PhotonNetwork.Instantiate("Dummy", spawnPoint, Quaternion.identity);

        new_dummy.GetComponent<Status>().Respawn += Respawn;

        UI_Manager.enemyStatus = new_dummy.GetComponent<Status>();
        UI_Manager.enemy = new_dummy;

        UI_Manager.AddHpSlider(new_dummy);
        return;
    }

    private void Respawn(GameObject game_object)
    {
        Status status = game_object.GetComponent<Status>();

        // 1팀일 때
        if (status.team == 1)
        {
            // 체력 회복
            game_object.SendMessage("FullRestoreHealth");

            // 리스폰 지점으로 오브젝트 이동
            game_object.transform.position = team_1_rewpawn_point.transform.position;
        }
        // 2팀일 때
        else if (status.team == 2)
        {
            // 체력 회복
            game_object.SendMessage("FullRestoreHealth");

            // 리스폰 지점으로 오브젝트 이동
            game_object.transform.position = team_2_rewpawn_point.transform.position;
        }

        return;
    }
 
    [PunRPC]
    public void SetScore(float newScore, int team)
    {
        // 게임오버가 아닌 상태에서만 점수 추가 가능
        if (isGameover)
        {
            return;
        }
        // 호스트에서만 점수 추가 가능
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetScore", RpcTarget.MasterClient, newScore, team);
            return;
        }

        if (team == 1)
        {
            // 점수 추가
            team_1_score = newScore;
        }
        else
        {
            team_2_score = newScore;
        }

        photonView.RPC("RPCUpdateScoreText", RpcTarget.All, team_1_score, team_2_score);
    }

    [PunRPC]
    private void RPCUpdateScoreText(float team_1_score, float team_2_score)
    {
        UIManager.instance.UpdateBlueScoreText(team_2_score);
        UIManager.instance.UpdateRedScoreText(team_1_score);
    }

    public void CreatePlayer()
    {
        // player 생성
        //int team = SelectTeam();
        int team = TeamManager.GetComponent<TeamManager>().localPlayerTeam;

        Vector3 spawnPoint;

        // 처음 스폰 포인트
        if (team == 1)
        {
            spawnPoint = team_1_rewpawn_point.transform.position;
        }
        else if (team == 2)
        {
            spawnPoint = team_2_rewpawn_point.transform.position;
        }
        else
        {
            Debug.Log("팀 설정이 안되어있습니다.");
            return;
        }
        //Vector3 spawnPoint = Random.insideUnitSphere * 5f;
        //spawnPoint.y = 9.26f;
        
        // Photon player custom property 사용해서 팀 색깔 할당하기
        
        Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;        
        if (playerCP["Color"].ToString().Equals("#FF0000") ) {
            localPlayer = PhotonNetwork.Instantiate(redTeamplayerPrefab.name, spawnPoint, Quaternion.identity);
        }
        else if (playerCP["Color"].ToString().Equals("#FFA500") ) {
            localPlayer = PhotonNetwork.Instantiate(orangeTeamplayerPrefab.name, spawnPoint, Quaternion.identity);
        }
        else if (playerCP["Color"].ToString().Equals("#FFFF00") ) {
            localPlayer = PhotonNetwork.Instantiate(yellowTeamplayerPrefab.name, spawnPoint, Quaternion.identity);
        }
        else if (playerCP["Color"].ToString().Equals("#008000") ) {
            localPlayer = PhotonNetwork.Instantiate(greenTeamplayerPrefab.name, spawnPoint, Quaternion.identity);
        }
        else if (playerCP["Color"].ToString().Equals("#0000FF") ) {
            localPlayer = PhotonNetwork.Instantiate(blueTeamplayerPrefab.name, spawnPoint, Quaternion.identity);
        }   
        else if (playerCP["Color"].ToString().Equals("#000080") ) {
            localPlayer = PhotonNetwork.Instantiate(navyTeamplayerPrefab.name, spawnPoint, Quaternion.identity);
        }
        else if (playerCP["Color"].ToString().Equals("#800080") ) {
            localPlayer = PhotonNetwork.Instantiate(purpleTeamplayerPrefab.name, spawnPoint, Quaternion.identity);
        }
    

        // // 블루팀 플레이어 생성
        // if (team == 1) {
        //     localPlayer = PhotonNetwork.Instantiate(blueTeamplayerPrefab.name, spawnPoint, Quaternion.identity);
        // }
        // // 레드팀 플레이어 생성
        // else {
        //     localPlayer = PhotonNetwork.Instantiate(redTeamplayerPrefab.name, spawnPoint, Quaternion.identity);
        // }


        //localPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity);
        localPlayer.name = TeamManager.GetComponent<TeamManager>().localPlayerName;
        Status status = localPlayer.GetComponent<Status>();
        status.Respawn += Respawn;
        status.Respawn += UI_Manager.OnRespawn;
        status.Respawn += mainCamera.ResetGrayScale;
        status.Dead += UI_Manager.OnDeath;
        status.Dead += mainCamera.StartGrayScale;

        UI_Manager.playerStatus = status;
        status.team = team;
        //status.SetPlayerColor();

        //TrailArea.team = team;
        UI_Manager.player = localPlayer;
        // SetColor();
        // status.SetColorOnServer();
        // photonView.RPC("SetAreaOnServer", RpcTarget.Others);

        // 디버그용 더미 생성 코드
        if (PhotonNetwork.IsMasterClient)
        {
            //CreateDummy(team_3_rewpawn_point.transform.position + new Vector3(0,0,0));
        }

        //AddPlayerToList(new_player);
    }


    // 임시 방편. 룸 씬을 따로 만들어 플레이어 별 색을 세팅한 후 동시에 플레이어 객체를 만들면 해결가능할 듯.
    //https://answers.unity.com/questions/898534/networking-colors-with-pun.html
    // player 생성, 팀 설정
    public void SetMyColor()
    {
        // 팀 별 플레이어 색 할당
        if (TeamManager.GetComponent<TeamManager>().localPlayerTeam == 1)
        {
            localPlayer.GetComponentInChildren<Trail_Action>().color = Color.blue;
        }
        else if (TeamManager.GetComponent<TeamManager>().localPlayerTeam == 2)
        {
            localPlayer.GetComponentInChildren<Trail_Action>().color = Color.red;
        }

        // Hashtable playerCP = photonView.Owner.CustomProperties;  
        // Color tempColor;
        // ColorUtility.TryParseHtmlString(playerCP["Color"].ToString(),out tempColor);
        // localPlayer.GetComponentInChildren<Trail_Action>().color = tempColor;
    }

    // 접속해있던 플레이어들의 팀 다시 설정
    [PunRPC]
    public void RPCSetOthersTeam()
    {
        localPlayer.GetComponent<Status>().SetOthersTeam();
    }

    // 접속해있던 플레이어들의 색깔 다시 설정
    [PunRPC]
    public void RPCSetOthersColorOnServer()
    {
        localPlayer.GetComponent<Status>().SetColorOnServer();
    }

    // 접속해있던 플레이어들의 영역 동기화
    [PunRPC]
    public void SetAreaOnServer()

    {
        localPlayer.GetComponentInChildren<Trail_Action>().UpdateAreaOnServer(localPlayer.GetComponentInChildren<Trail_Action>().areaVertices.ToArray());        
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
        base.OnLeftRoom();
    }
}


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
    // �̱��� ���ٿ� ������Ƽ
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

    // �Ӽ�
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

    // �ð�
    public float limitTime = 60;

    private float play_time = 0;
    private float score = 0;

    // ���� ������Ƽ
    private float team_1_score = 0;
    private float team_2_score = 0;

    public Sound_Manager sound_mng;
    
    // ī��Ʈ �ٿ�        
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
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
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
        // ����Ʈ���� ���� �ð� �޾ƿ���        
        if(!PhotonNetwork.IsMasterClient && startTime == 0 && PhotonNetwork.CurrentRoom.CustomProperties.Count>1) {
            
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            startTimer = true;            
        }

        if (!startTimer) return;
        timerIncrementValue = PhotonNetwork.Time - startTime;
        
        if (timerIncrementValue >= waitTime)
        {
            // �� ����
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

            // ī��Ʈ �ٿ�
            if (!isStart) {           
                countDownTime -= Time.deltaTime;    
                UI_Manager.ChangeCountDownNum(countDownTime);  

                // ���� ī��Ʈ �ٿ� ȿ���� �ѹ��� ���
                if (!countDownSoundPlayed && countDownTime > 3 && countDownTime < 4) 
                {
                    Debug.Log("���� ���");
                    sound_mng.SFXPlay(Sound_Manager.SFXName.CountDown); 
                    countDownSoundPlayed = true;
                }
                
                // ī��Ʈ �ٿ� �Ϸ� �� ���� ����
                if (countDownTime < 0) {
                    isStart = true;
                    UI_Manager.countDownBG.SetActive(false);   
                }                                     
                return;                        
            }
        }       

        
        // // esc�� ������ ���� â ����
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     UI_Manager.ToggleSettingBG();
        //     //LeaveRoom();
        // }
 
        
        // // �� ������ ������ Ȯ��        
        // if (Input.GetKey(KeyCode.Tab))
        // {
        //     if (PhotonNetwork.IsMasterClient)
        //     {
        //         photonView.RPC("RPCShowResult", RpcTarget.All,team_1_score, team_2_score);
        //     }            
        // }        
        

        // �켱 Update���� ������ ����ȭ. ��ȿ�����̴� ���߿� ������ ��.
        if (!isGameover && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPCUpdateScoreBoard", RpcTarget.All, team_1_score, team_2_score);
        }
        
        // ���ѽð� ������ ���â ���
        if (!isGameover && isStart)
        {
            play_time += Time.deltaTime;
            UIManager.instance.timerText.text = "" + Mathf.Round(limitTime - play_time);
        }
        if (play_time >= limitTime && !isGameover)
        {
            isGameover = true;
            UIManager.instance.timerText.text = "�ð�����!";
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


    // ����׿� ���̻��� �޼ҵ�
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

        // 1���� ��
        if (status.team == 1)
        {
            // ü�� ȸ��
            game_object.SendMessage("FullRestoreHealth");

            // ������ �������� ������Ʈ �̵�
            game_object.transform.position = team_1_rewpawn_point.transform.position;
        }
        // 2���� ��
        else if (status.team == 2)
        {
            // ü�� ȸ��
            game_object.SendMessage("FullRestoreHealth");

            // ������ �������� ������Ʈ �̵�
            game_object.transform.position = team_2_rewpawn_point.transform.position;
        }

        return;
    }
 
    [PunRPC]
    public void SetScore(float newScore, int team)
    {
        // ���ӿ����� �ƴ� ���¿����� ���� �߰� ����
        if (isGameover)
        {
            return;
        }
        // ȣ��Ʈ������ ���� �߰� ����
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetScore", RpcTarget.MasterClient, newScore, team);
            return;
        }

        if (team == 1)
        {
            // ���� �߰�
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
        // player ����
        //int team = SelectTeam();
        int team = TeamManager.GetComponent<TeamManager>().localPlayerTeam;

        Vector3 spawnPoint;

        // ó�� ���� ����Ʈ
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
            Debug.Log("�� ������ �ȵǾ��ֽ��ϴ�.");
            return;
        }
        //Vector3 spawnPoint = Random.insideUnitSphere * 5f;
        //spawnPoint.y = 9.26f;
        
        // Photon player custom property ����ؼ� �� ���� �Ҵ��ϱ�
        
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
    

        // // ����� �÷��̾� ����
        // if (team == 1) {
        //     localPlayer = PhotonNetwork.Instantiate(blueTeamplayerPrefab.name, spawnPoint, Quaternion.identity);
        // }
        // // ������ �÷��̾� ����
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

        // ����׿� ���� ���� �ڵ�
        if (PhotonNetwork.IsMasterClient)
        {
            //CreateDummy(team_3_rewpawn_point.transform.position + new Vector3(0,0,0));
        }

        //AddPlayerToList(new_player);
    }


    // �ӽ� ����. �� ���� ���� ����� �÷��̾� �� ���� ������ �� ���ÿ� �÷��̾� ��ü�� ����� �ذᰡ���� ��.
    //https://answers.unity.com/questions/898534/networking-colors-with-pun.html
    // player ����, �� ����
    public void SetMyColor()
    {
        // �� �� �÷��̾� �� �Ҵ�
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

    // �������ִ� �÷��̾���� �� �ٽ� ����
    [PunRPC]
    public void RPCSetOthersTeam()
    {
        localPlayer.GetComponent<Status>().SetOthersTeam();
    }

    // �������ִ� �÷��̾���� ���� �ٽ� ����
    [PunRPC]
    public void RPCSetOthersColorOnServer()
    {
        localPlayer.GetComponent<Status>().SetColorOnServer();
    }

    // �������ִ� �÷��̾���� ���� ����ȭ
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


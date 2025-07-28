using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using UnityEngine.UI;

public class TeamManager : MonoBehaviourPunCallbacks
{
    // 싱글턴 접근용 프로퍼티
    public static TeamManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<TeamManager>();
            }

            return m_instance;
        }
    }


    private static TeamManager m_instance;
    public ScrollViewController SVC;
    public string localPlayerName;
    public Player[] playerList;
    public int localPlayerTeam;
    public int[] teamList;

    public Text noticeContent;
    public ColorManager colorManager;
    public Button colorSelectButton;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 씬에 싱글턴 오브젝트가 된 다른 SceneManager 오브젝트가 있다면
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        localPlayerName = PhotonNetwork.LocalPlayer.NickName;

        //PhotonNetwork.LocalPlayer.GetTeam
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log(PhotonNetwork.LocalPlayer.GetPhotonTeam()); // Get Null

            Player[] players;

            if (PhotonTeamsManager.Instance.TryGetTeamMembers("Red", out players))
            {
                foreach (var p in players)
                {
                    Debug.Log(p.NickName);
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(PhotonNetwork.LocalPlayer.GetPhotonTeam().Name);
        }
    }
    public void JoinRedTeam()
    {
        colorManager.Initialize_Color(localPlayerTeam);
        colorManager.SetColor("#FFFFFF");
        
        // 원래 블루 팀인 경우
        if (localPlayerTeam == 2)
        {
            PhotonNetwork.LocalPlayer.SwitchTeam("Red");
            localPlayerTeam = 1;
        }
        // 원래 팀이 없는 경우
        else if (localPlayerTeam == 0)
        {
            PhotonNetwork.LocalPlayer.JoinTeam("Red");
            colorSelectButton.gameObject.SetActive(true);
            localPlayerTeam = 1;
        }
        Invoke("UpdateTeamList",0.1f);
        photonView.RPC("RPCUpdateTeamList",RpcTarget.Others);
    }

    public void JoinBlueTeam()
    {
        colorManager.Initialize_Color(localPlayerTeam);
        colorManager.SetColor("#FFFFFF");
        
        // 원래 레드 팀인 경우
        if (localPlayerTeam == 1)
        {
            PhotonNetwork.LocalPlayer.SwitchTeam("Blue");
            localPlayerTeam = 2;
        }
        // 원래 팀이 없는 경우
        else if (localPlayerTeam == 0)
        {
            PhotonNetwork.LocalPlayer.JoinTeam("Blue");
            colorSelectButton.gameObject.SetActive(true);
            localPlayerTeam = 2;
        }
        
        Invoke("UpdateTeamList",0.1f);
        photonView.RPC("RPCUpdateTeamList",RpcTarget.Others);
    }
    public bool CheckTeamCounts() {
        Player[] players_red;
        Player[] players_blue;        
        PhotonTeamsManager.Instance.TryGetTeamMembers("Red", out players_red);
        PhotonTeamsManager.Instance.TryGetTeamMembers("Blue", out players_blue);
        Debug.Log("Red Team : " + players_red.Length);
        Debug.Log("Blue Team : " + players_blue.Length);
        noticeContent.text = "(레드 : "+players_red.Length+" 블루 : "+players_blue.Length+")";
        
        if (players_red.Length != 0 && players_red.Length == players_blue.Length) {
            return true;
        }
        else {
            return false;
        }
    }
    public override void OnLeftRoom()
    {
        
        Destroy(gameObject);
    }

    public void UpdateTeamList() {
        SVC.DeleteUiObjectAll(1);
        SVC.DeleteUiObjectAll(2);

        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            PhotonTeam team = players[i].GetPhotonTeam();
            Debug.Log(players[i].NickName + "의 팀 :  " + team.Name);
            if (team == null) {
                continue;
            }
            else if (team.Name.Equals("Red")) {
                SVC.AddNewUiObject_(players[i],1);
            }
            else if (team.Name.Equals("Blue")) {
                SVC.AddNewUiObject_(players[i],2);
            }
        }
    }

    [PunRPC]
    public void RPCUpdateTeamList() {
        SVC.DeleteUiObjectAll(1);
        SVC.DeleteUiObjectAll(2);

        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            PhotonTeam team = players[i].GetPhotonTeam();
            if (team == null) {
                continue;
            }
            else if (team.Name.Equals("Red")) {
                SVC.AddNewUiObject_(players[i],1);
            }
            else if (team.Name.Equals("Blue")) {
                SVC.AddNewUiObject_(players[i],2);
            }
        }
    }


}

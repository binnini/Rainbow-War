using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;//Path사용위에 사용

public class RoomManager : MonoBehaviourPunCallbacks//다른 포톤 반응 받아들이기
{
//    public static RoomManager Instance;//Room Manager 스크립트를 메서드로 사용하기 위해 선언
    public GameObject StartButton;
    public GameObject ScrollView;
    public ScrollViewController scrollViewController;


    //
    bool flag = true;
    private ScrollRect scrollRect;
    
    public GameObject noticeBG;

    public float space = 10f;
    public GameObject playerPanelPrefab;
    public List<RectTransform> uiObjects = new List<RectTransform>();

    public TeamManager teamManager;
    public ColorManager colorManager;
    

    void Awake()
    {
        // 마스터 클라이언트와 씬 동기화
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartButton.SetActive(true);
            //StartButton.GetComponent<Button>().enabled = false;
        }
    }

    private void Update()
    {
        
        // 왜 start 에서는 안되는지 모르겠음
        if (flag)
        {
            Player[] players = PhotonNetwork.PlayerList;
            for (int i = 0; i < players.Length; i++)
            {
                //scrollViewController.AddNewUiObject(players[i]);
                scrollViewController.AddNewUiObject_(players[i],0);

                PhotonTeam team = players[i].GetPhotonTeam();
                if (team == null) {
                    continue;
                }
                else if (team.Name.Equals("Red")) {
                    scrollViewController.AddNewUiObject_(players[i],1);
                }
                else if (team.Name.Equals("Blue")) {
                    scrollViewController.AddNewUiObject_(players[i],2);
                }
            }

            
            flag = false;
        }

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     Debug.Log("현재 룸 개수 : " + PhotonNetwork.CountOfRooms);
        //     Debug.Log("현재 룸 이름 : " + PhotonNetwork.CurrentRoom.Name);
        //     Debug.Log("현재 룸 플레이어들 : " + PhotonNetwork.CurrentRoom.GetPlayer(0).NickName);
        // }
    }
    
    public void OnClickStart()
    {
        bool teamCountCheck = TeamManager.instance.CheckTeamCounts();
        bool colorCheck = colorManager.CheckColorSetting();

        if (teamCountCheck && colorCheck) {
            Debug.Log("실행 완료");
            StartButton.SetActive(false);
            PhotonNetwork.LoadLevel("Playing");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            return;
        }        
        else if (!teamCountCheck) {
            noticeBG.GetComponentInChildren<Text>().text = "팀 인원 수가 맞지 않습니다.";
            noticeBG.SetActive(true);
            return;
        }
        else if (!colorCheck) {
            noticeBG.GetComponentInChildren<Text>().text = "팀 색 설정이 완료되지 않았습니다.";
            noticeBG.SetActive(true);
            return;
        }        

        
    }
    public void OnClickStartDebug()
    {
        PhotonNetwork.LoadLevel("Playing");
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

  
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //scrollViewController.AddNewUiObject(newPlayer);
        scrollViewController.AddNewUiObject_(newPlayer, 0);

        /*if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                StartButton.GetComponent<Button>().enabled = true;
            }
        }*/
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Debug.Log(otherPlayer.NickName + " Left");
        scrollViewController.DeleteUiObjectAll(0);
        
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            //scrollViewController.AddNewUiObject(players[i]);
            scrollViewController.AddNewUiObject_(players[i], 0);
        }

        // 
        PhotonTeam team = otherPlayer.GetPhotonTeam();
        if (team == null) {
            
        }
        else if (team.Name.Equals("Red")) {
            scrollViewController.DeleteUiObjectAll(1);
            PhotonTeamsManager.Instance.TryGetTeamMatesOfPlayer(otherPlayer,out players);
            foreach (var player in players) {
                scrollViewController.AddNewUiObject_(player,1);
            }
        }
        else if (team.Name.Equals("Blue")) {
            scrollViewController.DeleteUiObjectAll(2);
            PhotonTeamsManager.Instance.TryGetTeamMatesOfPlayer(otherPlayer,out players);
            foreach (var player in players) {
                scrollViewController.AddNewUiObject_(player,2);
            }

        }


        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Debug.Log("마스터 클라이언트가 변경되었습니다.");
            StartButton.SetActive(true);
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");

        base.OnLeftRoom();
    }
}
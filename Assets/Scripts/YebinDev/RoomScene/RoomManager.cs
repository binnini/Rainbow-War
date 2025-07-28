using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;//Path������� ���

public class RoomManager : MonoBehaviourPunCallbacks//�ٸ� ���� ���� �޾Ƶ��̱�
{
//    public static RoomManager Instance;//Room Manager ��ũ��Ʈ�� �޼���� ����ϱ� ���� ����
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
        // ������ Ŭ���̾�Ʈ�� �� ����ȭ
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
        
        // �� start ������ �ȵǴ��� �𸣰���
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
        //     Debug.Log("���� �� ���� : " + PhotonNetwork.CountOfRooms);
        //     Debug.Log("���� �� �̸� : " + PhotonNetwork.CurrentRoom.Name);
        //     Debug.Log("���� �� �÷��̾�� : " + PhotonNetwork.CurrentRoom.GetPlayer(0).NickName);
        // }
    }
    
    public void OnClickStart()
    {
        bool teamCountCheck = TeamManager.instance.CheckTeamCounts();
        bool colorCheck = colorManager.CheckColorSetting();

        if (teamCountCheck && colorCheck) {
            Debug.Log("���� �Ϸ�");
            StartButton.SetActive(false);
            PhotonNetwork.LoadLevel("Playing");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            return;
        }        
        else if (!teamCountCheck) {
            noticeBG.GetComponentInChildren<Text>().text = "�� �ο� ���� ���� �ʽ��ϴ�.";
            noticeBG.SetActive(true);
            return;
        }
        else if (!colorCheck) {
            noticeBG.GetComponentInChildren<Text>().text = "�� �� ������ �Ϸ���� �ʾҽ��ϴ�.";
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
            Debug.Log("������ Ŭ���̾�Ʈ�� ����Ǿ����ϴ�.");
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
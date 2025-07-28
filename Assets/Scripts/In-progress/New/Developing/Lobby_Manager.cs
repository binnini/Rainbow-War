using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby_Manager : MonoBehaviourPunCallbacks
{
    private string game_version = "1";

    public Text connection_info_text;
    public Button join_button;
    Player player;

    private Sound_Manager sound_mng;

    // �˸�â UI �Ŵ���
    public NoticeUIManager noticeUIManager;


    void Start()
    {        
        // �����ϱ� ���� ���� ���� ����
        PhotonNetwork.GameVersion = game_version;

        // ������ ���� ����
        PhotonNetwork.ConnectUsingSettings();

        // ���� ��ư ��Ȱ��ȭ
        join_button.interactable = false;

        // ���� �� ���� ǥ��
        connection_info_text.text = "������ ������ ������...";

        // �÷��̾� ���� �̸��Ҵ�
        // PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");

        sound_mng = GameObject.Find("Sound_Manager").GetComponent<Sound_Manager>();
        if(sound_mng != null)
            sound_mng.BGMPlay(Sound_Manager.BGMName.Lobby);
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("���� �� ���� : " + PhotonNetwork.CountOfRooms);            
        }
    }

    // �г��� ����
    public void SetNickName(string nickName)
    {
        PhotonNetwork.NickName = nickName;        
    }
    // �г��� ����
    public string GetNickName()
    {
        return PhotonNetwork.NickName;
    }


    public override void OnConnectedToMaster()
    {
        join_button.interactable = true;
        connection_info_text.text = "�¶��� : ������ ������ �����";
        
        

        // �г����� ������ �˸�â �˾�
        if (PhotonNetwork.NickName == "")
        {
            noticeUIManager.PopUpNicknamePanel();
        }
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        join_button.interactable = false;
        connection_info_text.text = "�������� : ������ ������ ������� ����\n ���� ��õ� ��...";

        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        join_button.interactable = false;

        if (PhotonNetwork.IsConnected)
        {
            connection_info_text.text = "�뿡 ����...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connection_info_text.text = "�������� : ������ ������ ������� ����\n ���� ��õ���...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connection_info_text.text = "�� ���� ����, ���ο� �� ���� ��...";

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;        

        PhotonNetwork.CreateRoom(null, roomOptions);
        
    }

    public override void OnJoinedRoom()
    {
        connection_info_text.text = "�� ���� ����";

        PhotonNetwork.LoadLevel("Room");        
    }

 
}

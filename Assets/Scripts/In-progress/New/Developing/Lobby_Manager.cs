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

    // 알림창 UI 매니저
    public NoticeUIManager noticeUIManager;


    void Start()
    {        
        // 접속하기 위한 게임 버전 조건
        PhotonNetwork.GameVersion = game_version;

        // 마스터 서버 접속
        PhotonNetwork.ConnectUsingSettings();

        // 접속 버튼 비활성화
        join_button.interactable = false;

        // 접속 중 상태 표시
        connection_info_text.text = "마스터 서버에 접속중...";

        // 플레이어 랜덤 이름할당
        // PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");

        sound_mng = GameObject.Find("Sound_Manager").GetComponent<Sound_Manager>();
        if(sound_mng != null)
            sound_mng.BGMPlay(Sound_Manager.BGMName.Lobby);
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("현재 룸 개수 : " + PhotonNetwork.CountOfRooms);            
        }
    }

    // 닉네임 설정
    public void SetNickName(string nickName)
    {
        PhotonNetwork.NickName = nickName;        
    }
    // 닉네임 리턴
    public string GetNickName()
    {
        return PhotonNetwork.NickName;
    }


    public override void OnConnectedToMaster()
    {
        join_button.interactable = true;
        connection_info_text.text = "온라인 : 마스터 서버와 연결됨";
        
        

        // 닉네임이 없으면 알림창 팝업
        if (PhotonNetwork.NickName == "")
        {
            noticeUIManager.PopUpNicknamePanel();
        }
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        join_button.interactable = false;
        connection_info_text.text = "오프라인 : 마스터 서버와 연결되지 않음\n 접속 재시도 중...";

        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        join_button.interactable = false;

        if (PhotonNetwork.IsConnected)
        {
            connection_info_text.text = "룸에 접속...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connection_info_text.text = "오프라인 : 마스터 서버와 연결되지 않음\n 접속 재시도중...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connection_info_text.text = "빈 방이 없음, 새로운 방 생성 중...";

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;        

        PhotonNetwork.CreateRoom(null, roomOptions);
        
    }

    public override void OnJoinedRoom()
    {
        connection_info_text.text = "방 참가 성공";

        PhotonNetwork.LoadLevel("Room");        
    }

 
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class AccountUIManager : MonoBehaviourPun
{
    public Lobby_Manager lobby_Manager;    
    public NoticeUIManager noticeUIManager;
    public InputField nickNameInputField;  
    
    public Image icon_Image;
    public Sprite[] iconList;

    // Start is called before the first frame update
    void Start()
    {
        // 디폴트 아이콘 설정
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable{{"Icon",0}});   
    }

    void Update() {
        nickNameInputField.placeholder.GetComponent<Text>().text = lobby_Manager.GetNickName();
    }

    public void SetNickNameField()
    {

    }
    public void OnClickEditButton()
    {
        Debug.Log("edit");
        lobby_Manager.SetNickName(nickNameInputField.text); 
        //PopUpNoticePanel
        noticeUIManager.PopUpNoticePanel("닉네임 변경",nickNameInputField.text+"으로 변경되었습니다");
    }


    public void OnClickIconButton(int num)
    {
        icon_Image.sprite = iconList[num];
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable{{"Icon",num}});                
    }
}

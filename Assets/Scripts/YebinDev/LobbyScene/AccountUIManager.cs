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
        // ����Ʈ ������ ����
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
        noticeUIManager.PopUpNoticePanel("�г��� ����",nickNameInputField.text+"���� ����Ǿ����ϴ�");
    }


    public void OnClickIconButton(int num)
    {
        icon_Image.sprite = iconList[num];
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable{{"Icon",num}});                
    }
}

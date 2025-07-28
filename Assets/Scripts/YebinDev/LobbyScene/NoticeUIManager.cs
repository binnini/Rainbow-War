using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeUIManager : MonoBehaviour
{
    public Lobby_Manager lobby_Manager;

    // 팝업창
    public GameObject nickNamePanel;

    // 팝업창 배경
    public GameObject nickNamePanel_BG;

    // 입력, 취소 버튼
    private Button inputButton;
    private Button cancelButton;

    // 입력 공간
    private InputField nickNameInputField;  
    // 알림창
    public GameObject noticePanel;
    public Text noticePanel_Title;
    public Text noticePanel_Content;
    private float noticePanel_lifeTime = 2f;
    
    private void Awake() {
        inputButton = nickNamePanel.transform.Find("InputButton").GetComponent<Button>();
        cancelButton = nickNamePanel.transform.Find("CancelButton").GetComponent<Button>();
        nickNameInputField = nickNamePanel.transform.Find("InputField").GetComponent<InputField>();
        inputButton.onClick.AddListener(OnClickInputButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
    }
    
    // 알림창 사라질때까지 기다리기
    IEnumerator NoticePanelWait()
    {
        yield return new WaitForSeconds(noticePanel_lifeTime); 
        noticePanel.SetActive(false);  
    }

    // 닉네임 입력 팝업 띄우기
    public void PopUpNicknamePanel()
    {
        nickNamePanel_BG.SetActive(true);
        nickNamePanel.SetActive(true);
    }

    // 닉네임 입력 && 팝업창 닫기
    public void OnClickInputButton()
    {
        lobby_Manager.SetNickName(nickNameInputField.text);
        nickNamePanel_BG.SetActive(false);
        nickNamePanel.SetActive(false);
    }

    // 팝업창 닫기
    public void OnClickCancelButton()
    {
        // 등록된 닉네임이 공백이면 다시 입력
        if (lobby_Manager.GetNickName() == "")
        {
            nickNameInputField.placeholder.GetComponent<Text>().text = "닉네임을 설정하셔야 합니다.";
            return;
        }
        else
        {
            nickNamePanel_BG.SetActive(false);
            nickNamePanel.SetActive(false);
        }
    }

    // 알림창 띄우기
    public void PopUpNoticePanel(string title, string content)
    {
        noticePanel_Title.text = title;
        noticePanel_Content.text = content;
        noticePanel.SetActive(true);
        StartCoroutine(NoticePanelWait());        
    }
}

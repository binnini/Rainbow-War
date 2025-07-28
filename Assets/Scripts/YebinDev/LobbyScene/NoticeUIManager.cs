using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeUIManager : MonoBehaviour
{
    public Lobby_Manager lobby_Manager;

    // �˾�â
    public GameObject nickNamePanel;

    // �˾�â ���
    public GameObject nickNamePanel_BG;

    // �Է�, ��� ��ư
    private Button inputButton;
    private Button cancelButton;

    // �Է� ����
    private InputField nickNameInputField;  
    // �˸�â
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
    
    // �˸�â ����������� ��ٸ���
    IEnumerator NoticePanelWait()
    {
        yield return new WaitForSeconds(noticePanel_lifeTime); 
        noticePanel.SetActive(false);  
    }

    // �г��� �Է� �˾� ����
    public void PopUpNicknamePanel()
    {
        nickNamePanel_BG.SetActive(true);
        nickNamePanel.SetActive(true);
    }

    // �г��� �Է� && �˾�â �ݱ�
    public void OnClickInputButton()
    {
        lobby_Manager.SetNickName(nickNameInputField.text);
        nickNamePanel_BG.SetActive(false);
        nickNamePanel.SetActive(false);
    }

    // �˾�â �ݱ�
    public void OnClickCancelButton()
    {
        // ��ϵ� �г����� �����̸� �ٽ� �Է�
        if (lobby_Manager.GetNickName() == "")
        {
            nickNameInputField.placeholder.GetComponent<Text>().text = "�г����� �����ϼž� �մϴ�.";
            return;
        }
        else
        {
            nickNamePanel_BG.SetActive(false);
            nickNamePanel.SetActive(false);
        }
    }

    // �˸�â ����
    public void PopUpNoticePanel(string title, string content)
    {
        noticePanel_Title.text = title;
        noticePanel_Content.text = content;
        noticePanel.SetActive(true);
        StartCoroutine(NoticePanelWait());        
    }
}

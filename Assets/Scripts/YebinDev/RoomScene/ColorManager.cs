using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using System.Linq;
using System.Text;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
     // �̱��� ���ٿ� ������Ƽ
    public static ColorManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<ColorManager>();
            }

            return m_instance;
        }
    }
    private static ColorManager m_instance;
    public Color localColor;
    public GameObject team1_Color;
    public GameObject team2_Color;

    private void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    // ���ֳ����ĳ��� hex
    // #FF0000
    // #FFA500
    // #FFFF00
    // #008000
    // #0000FF
    // #000080
    // #800080
     
    private void Start() {
        SetColor("#FFFFFF");        
        team1_Color.GetComponentInChildren<Image>().color = Color.white;
        team2_Color.GetComponentInChildren<Image>().color = Color.white;
    }

    private void Update() {     
        PhotonTeam[] teams = PhotonTeamsManager.Instance.GetAvailableTeams();
        foreach (var team in teams) {
            // ���� �ƹ��� ���� ���
            if (PhotonTeamsManager.Instance.GetTeamMembersCount(team.Code) == 0) {
                // �� 1�� ���
                if (team.Name.Equals("Red")) {
                    team1_Color.GetComponentInChildren<Image>().color = Color.white;
                }
                // �� 2�� ���
                else if (team.Name.Equals("Blue")){
                    team2_Color.GetComponentInChildren<Image>().color = Color.white;
                }
            }
            // ���� ������ �ִ� ���
            else {
                Player[] players;
                PhotonTeamsManager.Instance.TryGetTeamMembers(team.Code,out players);
                
                // ù��° �÷��̾��� �� �Ҵ�
                Color tempColor; 
                Hashtable playerCP = players[0].CustomProperties;   
                ColorUtility.TryParseHtmlString(playerCP["Color"].ToString(),out tempColor); 

                // �� 1�� ���
                if (team.Name.Equals("Red")) {                    
                    team1_Color.GetComponentInChildren<Image>().color = tempColor;
                }
                // �� 2�� ���
                else if (team.Name.Equals("Blue")){                    
                    team2_Color.GetComponentInChildren<Image>().color = tempColor;
                }
            }
        }
    }

    public void SetColor(string color) {                
        ColorUtility.TryParseHtmlString(color,out localColor);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable{{"Color",color}});        
    }        

    public void Initialize_Color(int originTeam) {
        SetColor("FFFFFF");    
    }

   
    public bool CheckColorSetting() {
        // ���� �ȵ��ִ� ���
        if (team1_Color.GetComponentInChildren<Image>().color == Color.white || team2_Color.GetComponentInChildren<Image>().color == Color.white) {
            return false;
        }
        // ���� �Ǿ��ִ� ���
        else {
            return true;
        }   
    }
}

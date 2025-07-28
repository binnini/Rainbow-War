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
     // 싱글턴 접근용 프로퍼티
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
        // 씬에 싱글턴 오브젝트가 된 다른 오브젝트가 있다면
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    // 빨주노초파남보 hex
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
            // 팀에 아무도 없는 경우
            if (PhotonTeamsManager.Instance.GetTeamMembersCount(team.Code) == 0) {
                // 팀 1인 경우
                if (team.Name.Equals("Red")) {
                    team1_Color.GetComponentInChildren<Image>().color = Color.white;
                }
                // 팀 2인 경우
                else if (team.Name.Equals("Blue")){
                    team2_Color.GetComponentInChildren<Image>().color = Color.white;
                }
            }
            // 팀에 누군가 있는 경우
            else {
                Player[] players;
                PhotonTeamsManager.Instance.TryGetTeamMembers(team.Code,out players);
                
                // 첫번째 플레이어의 색 할당
                Color tempColor; 
                Hashtable playerCP = players[0].CustomProperties;   
                ColorUtility.TryParseHtmlString(playerCP["Color"].ToString(),out tempColor); 

                // 팀 1인 경우
                if (team.Name.Equals("Red")) {                    
                    team1_Color.GetComponentInChildren<Image>().color = tempColor;
                }
                // 팀 2인 경우
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
        // 세팅 안되있는 경우
        if (team1_Color.GetComponentInChildren<Image>().color == Color.white || team2_Color.GetComponentInChildren<Image>().color == Color.white) {
            return false;
        }
        // 세팅 되어있는 경우
        else {
            return true;
        }   
    }
}

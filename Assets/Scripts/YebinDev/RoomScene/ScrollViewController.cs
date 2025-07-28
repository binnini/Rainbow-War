using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScrollViewController : MonoBehaviour
{
    private ScrollRect scrollRect;
    public ScrollRect team1_ScrollRect;
    public ScrollRect team2_ScrollRect;


    public float space = 10f;
    public GameObject playerPanelPrefab;
    /*public GameObject redPlayerPanelPrefab;
    public GameObject bluePlayerPanelPrefab;*/
    public List<RectTransform> uiObjects = new List<RectTransform>();
    public List<RectTransform> team1_uiObjects = new List<RectTransform>();
    public List<RectTransform> team2_uiObjects = new List<RectTransform>();
    public Sprite[] iconList;

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();   
                
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNewUiObject_(Player player, int team)
    {
        Hashtable playerCP = player.CustomProperties;   
        int iconNum = (int)playerCP["Icon"];

        var uiObjects = this.uiObjects;
        var scrollRect = this.scrollRect;
        if (team == 2)
        {
            uiObjects = team2_uiObjects;
            scrollRect = team2_ScrollRect;
        }
        else if (team == 1)
        {
            uiObjects = team1_uiObjects;
            scrollRect = team1_ScrollRect;
        }

        GameObject panel = Instantiate(playerPanelPrefab, scrollRect.content.transform);
        //panel.transform.position = new Vector3(194,0,0);
        var newUi = panel.GetComponent<RectTransform>();        
        newUi.GetChild(newUi.transform.childCount-1).GetComponent<Image>().sprite = iconList[iconNum];
        newUi.GetComponentInChildren<Text>().text = player.NickName;
        uiObjects.Add(newUi);

        float y = 0f;
        for (int i = 0; i < uiObjects.Count; i++)
        {
            uiObjects[i].anchoredPosition = new Vector2(2f, -y);
            y += uiObjects[i].sizeDelta.y + space;
        }
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);
    }

    public void DeleteUiObjectAll(int team)
    {
        var uiObjects = this.uiObjects;
        var scrollRect = this.scrollRect;
        if (team == 2)
        {
            uiObjects = team2_uiObjects;
            scrollRect = team2_ScrollRect;
        }
        else if (team == 1)
        {
            uiObjects = team1_uiObjects;
            scrollRect = team1_ScrollRect;
        }

        uiObjects.Clear();
                
        Transform[] childList = scrollRect.content.GetComponentsInChildren<Transform>(true);
        if (childList != null)
        {
            for (int i = 0; i < childList.Length; i++)
            {
                if (childList[i] != scrollRect.content)
                    Destroy(childList[i].gameObject);
            }
        }        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIManager : MonoBehaviour
{
    private int pageNum = 1;
    public GameObject[] tutorialPageContent;
    public Image[] tutorialPageList;
    public Sprite currentPageIcon;
    public Sprite othersPageIcon;


    public void NextPageButtonOnClick()
    {
        if (pageNum < 6)
        {
            // 현재 페이지 비활성화
            tutorialPageContent[pageNum-1].SetActive(false);
            tutorialPageList[pageNum-1].sprite = othersPageIcon;
            // 현재 페이지 다음 페이지로 갱신 및 활성화
            pageNum++;
            tutorialPageContent[pageNum-1].SetActive(true);
            tutorialPageList[pageNum-1].sprite = currentPageIcon;
        }        
    }
    public void PrevPageButtonOnClick()
    {
        if (pageNum > 1)
        {
            // 현재 페이지 비활성화
            tutorialPageContent[pageNum-1].SetActive(false);
            tutorialPageList[pageNum-1].sprite = othersPageIcon;
            // 현재 페이지 이전 페이지로 갱신 및 활성화
            pageNum--;
            tutorialPageContent[pageNum-1].SetActive(true);
            tutorialPageList[pageNum-1].sprite = currentPageIcon;
        }        
    }
}

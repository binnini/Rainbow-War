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
            // ���� ������ ��Ȱ��ȭ
            tutorialPageContent[pageNum-1].SetActive(false);
            tutorialPageList[pageNum-1].sprite = othersPageIcon;
            // ���� ������ ���� �������� ���� �� Ȱ��ȭ
            pageNum++;
            tutorialPageContent[pageNum-1].SetActive(true);
            tutorialPageList[pageNum-1].sprite = currentPageIcon;
        }        
    }
    public void PrevPageButtonOnClick()
    {
        if (pageNum > 1)
        {
            // ���� ������ ��Ȱ��ȭ
            tutorialPageContent[pageNum-1].SetActive(false);
            tutorialPageList[pageNum-1].sprite = othersPageIcon;
            // ���� ������ ���� �������� ���� �� Ȱ��ȭ
            pageNum--;
            tutorialPageContent[pageNum-1].SetActive(true);
            tutorialPageList[pageNum-1].sprite = currentPageIcon;
        }        
    }
}

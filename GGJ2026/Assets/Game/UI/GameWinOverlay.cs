using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameWinOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private Text dayText;
    [SerializeField] private Text statsText;
    [SerializeField] private Text titleText;

    [SerializeField] public Button peopleListButton;
    [SerializeField] public Button endButton;
    [SerializeField] private List<Title> titles;

    private IngameStateManager ingameStateManager;

    private void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        peopleListButton.gameObject.SetActive(false);
        endButton.gameObject.SetActive(false);
        titles.Sort((a, b) => a.alivePercentage.CompareTo(b.alivePercentage));
    }

    public void Setup(IngameStateManager ingameStateManager, int dayCount, int alive, int dead)
    {
        this.ingameStateManager = ingameStateManager;
        dayText.text = $"Day {dayCount}";
        statsText.text = $"{alive} out of {alive + dead} people survived.";
        titleText.text = "GRIM REAPER";
        foreach (Title title in titles)
        {
            if ((float)alive * 100 / (alive + dead) >= title.alivePercentage)
            {
                titleText.text = title.title;
            }
        }
    }

    public void ClosePeopleList()
    {
        peopleListButton.gameObject.SetActive(false);
        ingameStateManager.TogglePeopleList(true);
        StartCoroutine(ShowThisOverlay());
    }

    private IEnumerator ShowThisOverlay()
    {
        yield return new WaitForSeconds(1f);
        canvasGroup.DOFade(1, 0.5f).SetEase(Ease.Linear);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        statsText.color = new Color(1,1,1,0);
        titleText.color = new Color(1, 1, 1, 0);
        statsText.DOFade(1, 0.5f).SetDelay(1f).SetEase(Ease.Linear);
        titleText.DOFade(1, 0.5f).SetDelay(1.5f).SetEase(Ease.Linear).OnComplete((() =>
        {
            endButton.gameObject.SetActive(true);
        }));
    }

    public void EndGame()
    {
        ingameStateManager.EndGame();
    }

    [Serializable]
    public class Title
    {
        public float alivePercentage;
        public string title;
    }
}

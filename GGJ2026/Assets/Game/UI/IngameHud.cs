using DG.Tweening;
using UnityEngine;

public class IngameHud : MonoBehaviour
{
    private Player player;
    [SerializeField]
    private PlayerUi playerUi;
    [SerializeField]
    private PeopleList list;
    [SerializeField]
    private NextDayOverlay nextDay;
    [SerializeField]
    private GameWinOverlay gameWin;
    [SerializeField] 
    private GameLossOverlay gameLoss;
    [SerializeField]
    private CountIcon pouchIcon;

    [SerializeField]
    private int barSegmentSize = 15;

    public NextDayOverlay NextDay => nextDay;
    public GameWinOverlay GameWin => gameWin;

    public void Setup(IngameStateManager ingameStateManager)
    {
        player = ingameStateManager.Player;
        list.Populate(ingameStateManager.People);

        RectTransform listTransform = list.transform as RectTransform;
        {
            Vector2 anchorMin = listTransform.anchorMin;
            anchorMin.y = -1;
            listTransform.anchorMin = anchorMin;
            Vector2 anchorMax = listTransform.anchorMax;
            anchorMax.y = 0;
            listTransform.anchorMax = anchorMax;
        }

        playerUi.SetMaxPlayerAir(player.TotalMaskPoints / barSegmentSize);
        nextDay.Setup();
    }

    private void Update()
    {
        if (player)
        {
            if (playerUi)
            {
                playerUi.SetPlayerAir(player.MaskPoints / barSegmentSize);
                playerUi.SetPlayerHealth(player.HitPoints);
            }

            if (pouchIcon)
            {
                pouchIcon.SetCount(player.PouchCount);
            }
        }
    }

    public void ShowWinOverlay()
    {
        TogglePeopleList();
        gameWin.peopleListButton.gameObject.SetActive(true);
    }

    public void ShowLossOverlay()
    {

    }

    public bool TogglePeopleList()
    {
        list.IsVisible = !list.IsVisible;

        if (list.IsVisible)
            list.Refresh();

        RectTransform listTransform = list.transform as RectTransform;
        Vector2 anchorMin = listTransform.anchorMin;
        Vector2 anchorMax = listTransform.anchorMax;
        anchorMin.y = list.IsVisible ? 0 : -1;
        anchorMax.y = list.IsVisible ? 1 : 0;

        DOTween.Kill(listTransform);
        listTransform.DOAnchorMin(anchorMin, 1).SetEase(Ease.OutCubic).Play();
        listTransform.DOAnchorMax(anchorMax, 1).SetEase(Ease.OutCubic).Play();

        return list.IsVisible;
    }
}

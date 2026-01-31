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
    private int barSegmentSize = 15;

    public NextDayOverlay NextDay => nextDay;

    public void Setup(IngameStateManager ingameStateManager)
    {
        player = ingameStateManager.Player;
        list.Populate(ingameStateManager.People);
        list.gameObject.SetActive(false);
        playerUi.SetMaxPlayerAir(player.TotalMaskPoints / barSegmentSize);
        nextDay.Setup();
    }

    private void Update()
    {
        if (player && playerUi)
        {
            playerUi.SetPlayerAir(player.MaskPoints / barSegmentSize);
            playerUi.SetPlayerHealth(player.HitPoints);
        }
    }

    public void ShowPeopleList(bool show)
    {
        if (show)
            list.Refresh();

        RectTransform listTransform = list.transform as RectTransform;
        Vector2 anchorMin = listTransform.anchorMin;
        Vector2 anchorMax = listTransform.anchorMax;
        anchorMin.y = show ? 0 : 1;
        anchorMax.y = show ? 1 : 2;

        listTransform.DOAnchorMin(anchorMin, 1).SetEase(Ease.OutCubic).Play();
        listTransform.DOAnchorMax(anchorMax, 1).SetEase(Ease.OutCubic).Play();
    }
}

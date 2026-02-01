using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameLossOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;

    private void Awake()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
        titleText.color = new Color(1, 1, 1, 0);
        descriptionText.color = new Color(1, 1, 1, 0);
    }

    public void ShowOverlay()
    {
        Sequence sequence = DOTween.Sequence(this);
        sequence.Append(canvasGroup.DOFade(1, 0.5f).SetEase(Ease.Linear));
        sequence.Append(titleText.DOFade(1, 0.5f).SetEase(Ease.Linear));
        sequence.Append(descriptionText.DOFade(1, 0.5f).SetEase(Ease.Linear));
        sequence.Append(canvasGroup.DOFade(0, 0.5f).SetEase(Ease.Linear).SetDelay(4));
        sequence.OnComplete(SequenceComplete);
        sequence.Play();
    }

    private void SequenceComplete()
    {
        Game.Instance.SwitchState(Game.GameState.Menu);
    }
}

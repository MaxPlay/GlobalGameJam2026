using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NextDayOverlay : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup root;

    [SerializeField]
    private Text heading;

    [SerializeField]
    private Text description;

    [SerializeField]
    private Text firstDayText;

    [SerializeField]
    private Text firstDayText2;

    public void SetData(int day, int dead, int total)
    {
        heading.text = $"Day {day}";
        description.text = $"{dead} out of {total} people are dead.";
        description.gameObject.SetActive(true);
    }

    public IEnumerator Show(bool firstDay)
    {
        root.blocksRaycasts = true;
        firstDayText.gameObject.SetActive(firstDay);
        firstDayText2.gameObject.SetActive(firstDay);
        {
            Color color = firstDayText2.color;
            color.a = 0;
            firstDayText2.color = color;
        }
        description.gameObject.SetActive(!firstDay);

        yield return root.DOFade(1, 2).Play().WaitForCompletion();
        if (firstDay)
        {
            yield return new WaitForSeconds(1);
            yield return firstDayText2.DOFade(1, 1).Play().WaitForCompletion();
        }
    }

    public IEnumerator Hide()
    {
        yield return root.DOFade(0, 2).Play().WaitForCompletion();
        root.blocksRaycasts = false;
    }

    public void Setup()
    {
        root.alpha = 0;
        root.blocksRaycasts = false;
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class CountIcon : MonoBehaviour
{
    [SerializeField]
    private Text countText;

    [SerializeField]
    private Image iconImage;

    [SerializeField]
    private Sprite fullIcon;
    [SerializeField]
    private Sprite emptyIcon;

    public int Count { get; private set; }

    private void Start()
    {
        Debug.Assert(iconImage, "iconImage not assigned");

        Refresh();
    }

    public void SetCount(int count)
    {
        if (count != Count)
        {
            Count = count;
            Refresh();
        }
    }

    private void Refresh()
    {
        if (iconImage)
            iconImage.sprite = Count == 0 ? emptyIcon : fullIcon;
        if (countText)
            countText.text = Count.ToString();
    }
}

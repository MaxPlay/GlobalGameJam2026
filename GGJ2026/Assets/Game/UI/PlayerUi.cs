using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    [SerializeField] private Image maskRenderer;
    [SerializeField] private RectTransform airRenderer;
    [SerializeField] private RectTransform backgroundRenderer;
    [SerializeField] private List<PlayerMaskThreshold> healthMasks;
    
    private int airBarSpriteWidth;
    private int minimumBarWidth;
    private float airRightSideOffset;
    private float currentMaxAir;
    private float currentAir;

    private void Awake()
    {
        airRightSideOffset = airRenderer.offsetMax.x;
        airBarSpriteWidth = Mathf.RoundToInt(airRenderer.GetComponent<Image>().sprite.rect.width);
        minimumBarWidth = Mathf.RoundToInt(backgroundRenderer.rect.width - airRenderer.rect.width) - 1;
        healthMasks.Sort((a,b) => a.minimumHealth.CompareTo(b.minimumHealth));
    }

    public void SetPlayerHealth(float health)
    {
        int index = 0;
        for (int i = 0; i < healthMasks.Count; i++)
        {
            PlayerMaskThreshold threshold = healthMasks[i];
            if (health < threshold.minimumHealth)
            {
                break;
            }
            index = i;
        }

        if (healthMasks.TryGetEntry(index, out PlayerMaskThreshold output))
        {
            maskRenderer.sprite = output.sprite;
        }
    }

    public void SetPlayerAir(float air)
    {
        if (!airRenderer)
            return;

        currentAir = Mathf.CeilToInt(air);
        UpdateAir();
    }

    public void SetMaxPlayerAir(float maxAir)
    {
        if (!backgroundRenderer)
            return;

        currentMaxAir = Mathf.CeilToInt(maxAir);
        backgroundRenderer.sizeDelta = new Vector2(minimumBarWidth + currentMaxAir * airBarSpriteWidth, backgroundRenderer.sizeDelta.y);
        UpdateAir();
    }

    private void UpdateAir()
    {
        airRenderer.offsetMax = new Vector2(airRightSideOffset - (currentMaxAir - currentAir) * airBarSpriteWidth, airRenderer.offsetMax.y);
    }

    [System.Serializable]
    public class PlayerMaskThreshold
    {
        [SerializeField] public float minimumHealth;
        [SerializeField] public Sprite sprite;
    }
}

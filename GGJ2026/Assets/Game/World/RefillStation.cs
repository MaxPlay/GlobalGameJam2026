using UnityEngine;

public class RefillStation : MonoBehaviour, IInteractable
{
    [SerializeField]
    private int refillAmount;

    [SerializeField]
    private bool available = false;

    public Vector3 Position => transform.position;

    public void HideInfo()
    {
        // TODO: Hide Interaction Info
    }

    public void ShowInfo()
    {
        // TODO: Show Interaction Info
    }

    public void Interact(Player player)
    {
        if (available)
        {
            player.RefillMask(refillAmount);
            available = false;
        }
    }

    public void NextDay()
    {
        available = true;
    }
}

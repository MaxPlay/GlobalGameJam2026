using UnityEngine;

public class RefillStation : MonoBehaviour, IInteractable
{
    [SerializeField]
    private int refillAmount;

    [SerializeField]
    private bool available = false;

    public Vector3 Position => transform.position;

    public void Interact(Player player)
    {
        if (available)
        {
            player.RefillMask(refillAmount);
            available = false;
        }
    }

    public string InfoText => "Refill your air once.";

    public void NextDay()
    {
        available = true;
    }
}

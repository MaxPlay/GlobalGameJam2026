using UnityEngine;

public class OfficeDoor : MonoBehaviour, IInteractable
{
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
        Game.Instance.GetStateManager<IngameStateManager>().NextDay();
    }
}

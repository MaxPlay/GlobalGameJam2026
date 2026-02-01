using UnityEngine;

public class OfficeDoor : MonoBehaviour, IInteractable
{
    public Vector3 Position => transform.position;

    public void Interact(Player player)
    {
        Game.Instance.GetStateManager<IngameStateManager>().NextDay();
    }

    public string InfoText => "End Day";
}

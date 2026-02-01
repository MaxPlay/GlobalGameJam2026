using UnityEngine;

public interface IInteractable
{
    Vector3 Position { get; }

    void Interact(Player player);

    public string InfoText { get; }
}
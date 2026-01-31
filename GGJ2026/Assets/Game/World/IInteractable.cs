using UnityEngine;

public interface IInteractable
{
    Vector3 Position { get; }

    void Interact(Player player);

    void ShowInfo();
    void HideInfo();
}
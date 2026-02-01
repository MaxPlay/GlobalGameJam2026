using UnityEngine;

public class HintComponent : MonoBehaviour, IInteractable
{
    [SerializeField] private string hintText;

    public Vector3 Position => transform.position;
    public void Interact(Player player)
    {
        
    }

    public string InfoText => hintText;
}

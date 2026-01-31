using Alchemy.Inspector;
using UnityEngine;

public class Person : MonoBehaviour, IInteractable
{
    public enum PersonState
    {
        Unchecked,
        Alive,
        Unknown,
        Dead
    }

    [SerializeField]
    private string displayName;
    [SerializeField, Range(0, 100)]
    private int hitPoints = 100;

    public string DisplayName => displayName;
    public int HitPoints => hitPoints;

    [ShowInInspector]
    public PersonState State { get; private set; } = PersonState.Unchecked;

    public Vector3 Position => transform.position;

    public bool IsRecorded => State == PersonState.Alive || State == PersonState.Dead;

    public void Record()
    {
        State = hitPoints > 0 ? PersonState.Alive : PersonState.Dead;
        Game.Instance.GetStateManager<IngameStateManager>().ValidatePeople();
    }

    public void Heal(int amount)
    {
        if (hitPoints > 0)
        {
            hitPoints += amount;
            // TODO: Update Visuals
        }
    }

    public void NextDay()
    {
        if (State == PersonState.Alive)
            State = PersonState.Unknown;

        hitPoints -= 25;
        // TODO: Update Visuals
    }

    public void Interact(Player player)
    {
        switch (State)
        {
            case PersonState.Unchecked:
            case PersonState.Unknown:
                player.RecordPerson(this);
                break;
            case PersonState.Alive:
                player.HealPerson(this);
                break;
            case PersonState.Dead:
                // TODO: We could add a funny animation here
                break;
        }
    }

    public void HideInfo()
    {
        // TODO: Hide Interaction Info
    }

    public void ShowInfo()
    {
        // TODO: Show Interaction Info
    }
}

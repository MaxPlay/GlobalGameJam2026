using Alchemy.Inspector;
using UnityEngine;

public class Person : MonoBehaviour
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
    [SerializeField, Range(0, 100)]
    private int healthyThreshold = 50;

    [SerializeField] private Animatable animations;

    public string DisplayName => displayName;
    public int HitPoints => hitPoints;

    [ShowInInspector]
    public PersonState State { get; private set; } = PersonState.Unchecked;

    public void Record()
    {
        State = hitPoints > 0 ? PersonState.Alive : PersonState.Dead;
    }

    public void Heal(int amount)
    {
        if (hitPoints > 0)
        {
            hitPoints += amount;
            animations.PlayAnimation("Heal", 5, () =>
            {
                animations.PlayAnimation("Sick");
            });
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
}

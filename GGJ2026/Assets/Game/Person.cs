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

    private void Start()
    {
        EnterIdleAnimation();
    }

    public void Record()
    {
        State = hitPoints > 0 ? PersonState.Alive : PersonState.Dead;
    }

    private void EnterIdleAnimation()
    {
        if (hitPoints >= healthyThreshold)
        {
            animations.PlayAnimation("Healthy");
        }
        else if (hitPoints > 0)
        {
            animations.PlayAnimation("Sick");
        }
        else
        {
            animations.PlayAnimation("Dead");
        }
    }

    public void Heal(int amount)
    {
        if (hitPoints > 0)
        {
            animations.PlayAnimation(hitPoints >= healthyThreshold ? "Heal_Healthy" : "Heal", 5, EnterIdleAnimation);
            hitPoints += amount;
            // TODO: Update Visuals
        }
    }

    public void NextDay()
    {
        if (State == PersonState.Alive)
            State = PersonState.Unknown;

        hitPoints -= 25;
        EnterIdleAnimation();
        // TODO: Update Visuals
    }
}

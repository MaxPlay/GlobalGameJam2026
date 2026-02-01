using System.Collections;
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
    [SerializeField, Range(0, 100)]
    private int healthyThreshold = 50;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] coughClips;
    [SerializeField] private Vector2 coughDelayRange;

    [SerializeField] private Animatable animations;

    public string DisplayName => displayName;
    public int HitPoints => hitPoints;

    [ShowInInspector]
    public PersonState State { get; private set; } = PersonState.Unchecked;

    public Vector3 Position => transform.position;

    public bool IsRecorded => State == PersonState.Alive || State == PersonState.Dead;

    private void Start()
    {
        EnterIdleAnimation();
        StartCoroutine(CoughRoutine());
    }

    private IEnumerator CoughRoutine()
    {
        while (hitPoints > 0)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(coughDelayRange.x, coughDelayRange.y));
            if (hitPoints >= healthyThreshold)
            {
                audioSource.clip = coughClips[Random.Range(0, coughClips.Length - 1)];
                audioSource.Play();
            }
        }
    }

    public void Record()
    {
        State = hitPoints > 0 ? PersonState.Alive : PersonState.Dead;
        Game.Instance.GetStateManager<IngameStateManager>().ValidatePeople();
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

    public string InfoText => "Heal Me!";
}

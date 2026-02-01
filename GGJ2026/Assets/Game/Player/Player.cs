using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private IngameStateManager gameState;

    [SerializeField, BoxGroup("Rendering")] private SpriteRenderer spriteRenderer;
    [SerializeField, BoxGroup("Rendering")] private AnimationStateMachine<PlayerAnimationStates> animations;

    [SerializeField, BoxGroup("Points")]
    private float totalHitPoints;

    [SerializeField, BoxGroup("Points")]
    private float totalMaskPoints;
    [SerializeField, BoxGroup("Points")]
    private float maskPoints;
    [SerializeField, BoxGroup("Points")]
    private float hitPoints;

    public float TotalMaskPoints => totalMaskPoints;
    public float TotalHitPoints => totalHitPoints;
    public float HitPoints => hitPoints;
    public float MaskPoints => maskPoints;

    [SerializeField, BoxGroup("Reductions")]
    private float distanceMaskReductionMultiplier = 1;
    [SerializeField, BoxGroup("Reductions")]
    private float sprintingMaskReductionMultiplier = 2.5f;
    [SerializeField, BoxGroup("Reductions")]
    private float recordMaskReduction;
    [SerializeField, BoxGroup("Reductions")]
    private float healMaskReduction;

    [SerializeField, BoxGroup("Effects")]
    private Volume postEffectVolume;
    [SerializeField, BoxGroup("Effects")]
    private Color baseVignetteColor;
    [SerializeField, BoxGroup("Effects")]
    private Color damageVignetteColor;

    [SerializeField, BoxGroup("Effects")] private ParticleSystem damagingParticleEffects;

    [SerializeField]
    private int healAmount;

    private bool isLookingUp;
    private PlayerController controller;
    private Color currentVignetteColor;
    private bool isDoingVignetteEffect;

    public enum PlayerAnimationStates
    {
        Idle, IdleUp, Walk, WalkUp, Injection, Record, 
    }

    private readonly List<IInteractable> interactablesInRange = new();

    private void Awake()
    {
        animations.Setup(PlayerAnimationStates.Idle, new(PlayerAnimationStates, string)[]
        {
            (PlayerAnimationStates.Idle, "Idle"),
            (PlayerAnimationStates.Walk, "Walk"),
            (PlayerAnimationStates.IdleUp, "Idle_Back"),
            (PlayerAnimationStates.WalkUp, "Walk_Back"),
            (PlayerAnimationStates.Injection, "Cure_Injection"),
            (PlayerAnimationStates.Record, "Note")
        });
        controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        gameState = Game.Instance.GetStateManager<IngameStateManager>();
        maskPoints = TotalMaskPoints;
        hitPoints = totalHitPoints;
    }

    public void MoveDistance(float distance, bool sprinting, Vector2 movement)
    {
        if (movement.x < 0 && spriteRenderer)
            spriteRenderer.flipX = true;
        else if (movement.x > 0 && spriteRenderer)
            spriteRenderer.flipX = false;

        if (movement.y > 0)
            isLookingUp = true;
        else if (movement.y < 0)
            isLookingUp = false;

        if (distance > 0)
        {
            animations.TryPlayState(isLookingUp ? PlayerAnimationStates.WalkUp : PlayerAnimationStates.Walk);
        }
        else
        {
            animations.TryPlayState(isLookingUp ? PlayerAnimationStates.IdleUp : PlayerAnimationStates.Idle);
        }
        
        float multiplier = sprinting ? sprintingMaskReductionMultiplier : distanceMaskReductionMultiplier;
        float reduction = distance * multiplier;
        ReduceMask(reduction);
    }

    private void ReduceMask(float reduction)
    {
        if (reduction >= maskPoints)
        {
            if (maskPoints > 0)
            {
                reduction -= maskPoints;
                maskPoints = 0;
            }
            hitPoints -= reduction;
            damagingParticleEffects.Play();
            if (!isDoingVignetteEffect)
            {
                isDoingVignetteEffect = true;
                Sequence sequence = DOTween.Sequence(this);
                sequence.Append(DOTween.To(() => currentVignetteColor, SetCurrentVignetteColor, damageVignetteColor,
                    0.5f));
                sequence.Append(DOTween.To(() => currentVignetteColor, SetCurrentVignetteColor, baseVignetteColor,
                    2f).SetDelay(0.5f));
                sequence.OnComplete((() => { isDoingVignetteEffect = false; }));
                sequence.Play();


                Sequence spriteSequence = DOTween.Sequence(this);
                spriteSequence.Append(spriteRenderer.DOColor(damageVignetteColor, 1f));
                spriteSequence.Append(spriteRenderer.DOColor(Color.white, 1f).SetDelay(0.5f));
                spriteSequence.Play();

                spriteRenderer.transform.DOShakePosition(1f,0.1f, 20);
            }

            if (hitPoints <= 0)
                gameState.GameLost();
        }
        else
        {
            maskPoints -= reduction;
            damagingParticleEffects.Pause();
        }
    }

    private void SetCurrentVignetteColor(Color color)
    {
        currentVignetteColor = color;
        foreach (VolumeComponent component in postEffectVolume.profile.components)
        {
            if (component is Vignette vignette)
            {
                vignette.color.value = currentVignetteColor;
            }
        }
    }

    public void Interact()
    {
        if (GetClosestInteractable(out IInteractable interactable))
        {
            interactable.Interact(this);
        }
    }

    public void RecordPerson(Person person)
    {
        person.Record();
        ReduceMask(recordMaskReduction);

        spriteRenderer.flipX = person.transform.position.x < transform.position.x;
        isLookingUp = false;
        controller.enabled = false;
        animations.TryPlayState(PlayerAnimationStates.Record, 0,
            () =>
            {
                animations.locked = false;
                controller.enabled = true;
                animations.TryPlayState(PlayerAnimationStates.Idle);
            }, true);
        // TODO: Visualization
    }

    public void HealPerson(Person person)
    {
        ReduceMask(healMaskReduction);
        person.Heal(healAmount); 

        spriteRenderer.flipX = person.transform.position.x < transform.position.x;
        isLookingUp = false;
        controller.enabled = false;
        animations.TryPlayState(PlayerAnimationStates.Injection, 0,
            () =>
            {
                animations.locked = false;
                controller.enabled = true;
                animations.TryPlayState(PlayerAnimationStates.Idle);
            }, true);

        // TODO: Visualization
    }

    public void RefillMask(int amount)
    {
        maskPoints = Mathf.Min(maskPoints + amount, TotalMaskPoints);
        // TODO: Visualization
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            interactablesInRange.Add(interactable);
            interactable.ShowInfo();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            interactablesInRange.Remove(interactable);
            interactable.HideInfo();
        }
    }

    public bool GetClosestInteractable(out IInteractable result)
    {
        result = null;
        if (interactablesInRange.Count == 0)
            return false;

        Vector3 position = transform.position;
        result = interactablesInRange[0];
        if (interactablesInRange.Count > 1)
        {
            float closest = (position - result.Position).sqrMagnitude;
            for (int i = 1; i < interactablesInRange.Count; i++)
            {
                IInteractable candidate = interactablesInRange[i];
                float distance = (position - candidate.Position).sqrMagnitude;
                if (distance < closest)
                {
                    result = candidate;
                    closest = distance;
                }
            }
        }

        return true;
    }
}

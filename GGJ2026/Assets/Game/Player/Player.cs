using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Random = System.Random;

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

    [SerializeField, BoxGroup("SFX")] private AudioSource audioSource;
    [SerializeField, BoxGroup("SFX")] private AudioClip[] walkClips;
    [SerializeField, BoxGroup("SFX")] private AudioClip injectionClip;
    [SerializeField, BoxGroup("SFX")] private AudioClip refillClip;

    [SerializeField, BoxGroup("Effects")] private ParticleSystem damagingParticleEffects;

    [SerializeField] private Canvas infoCanvas;
    private TMP_Text infoText;

    [SerializeField]
    private int healAmount;
    [SerializeField]
    private int pouchFillAmount;
    [SerializeField]
    private int pouchCount;

    public int PouchCount => pouchCount;

    private bool isLookingUp;
    private PlayerController controller;
    private Color currentVignetteColor;
    private bool isDoingVignetteEffect;

    public enum PlayerAnimationStates
    {
        Idle, IdleUp, Walk, WalkUp, Injection, Record,
        Death
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
            (PlayerAnimationStates.Record, "Note"),
            (PlayerAnimationStates.Death, "Death")
        });
        controller = GetComponent<PlayerController>();

        infoText = infoCanvas.GetComponentInChildren<TMP_Text>();
        infoCanvas.gameObject.SetActive(false);
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
            animations.TryPlayState(isLookingUp ? PlayerAnimationStates.WalkUp : PlayerAnimationStates.Walk, 0, null, false, new Animatable.AnimationEvent(PlayStepSound, 3), new Animatable.AnimationEvent(PlayStepSound, 7));
        }
        else
        {
            animations.TryPlayState(isLookingUp ? PlayerAnimationStates.IdleUp : PlayerAnimationStates.Idle);
        }
        
        float multiplier = sprinting ? sprintingMaskReductionMultiplier : distanceMaskReductionMultiplier;
        float reduction = distance * multiplier;
        ReduceMask(reduction);
    }

    private void PlayStepSound()
    {
        audioSource.clip = walkClips[UnityEngine.Random.Range(0, walkClips.Length - 1)];
        audioSource.Play();
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
            {
                animations.TryPlayState(PlayerAnimationStates.Death, 0, null, true);
                gameState.GameLost();
            }
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
            infoCanvas.gameObject.SetActive(false);
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
            }, true, new Animatable.AnimationEvent(PlayInjectionSound, 5));

        // TODO: Visualization
    }

    private void PlayInjectionSound()
    {
        audioSource.clip = injectionClip;
        audioSource.Play();
    }

    public void RefillMask(int amount)
    {
        maskPoints = Mathf.Min(maskPoints + amount, TotalMaskPoints);
        audioSource.clip = refillClip;
        audioSource.Play();
        // TODO: Visualization
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            interactablesInRange.Add(interactable);
            infoCanvas.gameObject.SetActive(true);
            infoText.text = interactable.InfoText;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            interactablesInRange.Remove(interactable);
            infoCanvas.gameObject.SetActive(false);
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

    public void UsePouch()
    {
        if (pouchCount > 0 && maskPoints < TotalMaskPoints)
        {
            pouchCount--;
            RefillMask(pouchFillAmount);
        }
    }
}

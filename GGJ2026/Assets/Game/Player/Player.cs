using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private IngameStateManager gameState;

    [SerializeField, BoxGroup("Points")]
    private float hitPoints;
    [SerializeField, BoxGroup("Points")]
    private float maskPoints;

    [SerializeField, BoxGroup("Reductions")]
    private float distanceMaskReductionMultiplier = 1;
    [SerializeField, BoxGroup("Reductions")]
    private float sprintingMaskReductionMultiplier = 2.5f;
    [SerializeField, BoxGroup("Reductions")]
    private float recordMaskReduction;
    [SerializeField, BoxGroup("Reductions")]
    private float healMaskReduction;

    [SerializeField]
    private int healAmount;

    private readonly List<IInteractable> interactablesInRange = new();

    private void Start()
    {
        gameState = Game.Instance.GetStateManager<IngameStateManager>();
    }

    public void MoveDistance(float distance, bool sprinting)
    {
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

            if (hitPoints < 0)
                gameState.GameLost();
        }
        else
        {
            maskPoints -= reduction;
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
        // TODO: Visualization
    }

    public void HealPerson(Person person)
    {
        ReduceMask(healMaskReduction);
        person.Heal(healAmount);
        // TODO: Visualization
    }

    public void RefillMask(int amount)
    {
        maskPoints += amount;
        // TODO: Visualization
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            interactablesInRange.Add(interactable);
            interactable.ShowInfo();
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            interactablesInRange.Remove(interactable);
            interactable.HideInfo();
            return;
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

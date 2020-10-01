using UnityEngine;
using System.Collections.Generic;

public class AvoidSensor : MonoBehaviour
{
    HashSet<AIRigidbody> _targets = new HashSet<AIRigidbody>();

    public HashSet<AIRigidbody> targets
    {
        get
        {
            /* Remove any MovementAIRigidbodies that have been destroyed */
            _targets.RemoveWhere(IsNull);
            return _targets;
        }
    }

    static bool IsNull(AIRigidbody r)
    {
        return (r == null || r.Equals(null));
    }

    void TryToAdd(Component other)
    {
        AIRigidbody rb = other.GetComponent<AIRigidbody>();
        if (rb != null)
        {
            _targets.Add(rb);
        }
    }

    void TryToRemove(Component other)
    {
        AIRigidbody rb = other.GetComponent<AIRigidbody>();
        if (rb != null)
        {
            _targets.Remove(rb);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryToAdd(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        TryToRemove(other);
    }
}
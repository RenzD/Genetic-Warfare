using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    HashSet<GameObject> _targets = new HashSet<GameObject>();

    private void Awake()
    {
    }

    private void FixedUpdate()
    {
        if (transform.localPosition.x != 0 || transform.localPosition.y != 0)
        {
            transform.localPosition = Vector3.zero;
        }
    }

    public HashSet<GameObject> targets
    {
        get
        {
            /* Remove any AIRigidbodies that have been destroyed */
            _targets.RemoveWhere(IsNull);
            return _targets;
        }
    }

    static bool IsNull(GameObject r)
    {
        return (r == null || r.Equals(null));
    }

    void TryToAdd(GameObject other)
    {
        /*
        GameObject rb = other.GetComponent<GameObject>();
        if (rb != null)
        {
            _targets.Add(rb);
        }
        */
        if (other.GetComponent<Faction1>())
            _targets.Add(other);
    }

    void TryToRemove(GameObject other)
    {
        /*
        GameObject rb = other.GetComponent<GameObject>();
        if (rb != null)
        {
            _targets.Remove(rb);
        }
        */
        if (other.GetComponent<Faction1>())
            _targets.Remove(other);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryToAdd(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        TryToRemove(other.gameObject);
    }
}

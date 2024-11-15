using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField]
    private float damage;

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        Human h = other.GetComponent<Human>();
        if (h != null && !h.CompareTag("Player"))
        {
            Debug.Log($"DamageTrigger hit with damage: {damage}");
            h.DealDamage(damage);
        }
    }
}

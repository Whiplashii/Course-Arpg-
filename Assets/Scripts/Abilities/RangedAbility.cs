using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ranged")]
public class RangedAbility : Ability
{
    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private float speed = 15;   

    [SerializeField]
    private int amount = 1;

    public override void Execute(Human user)
{
    float damage = user.baseDamage; 
    float damageFI = user.additionalDamageFromItems; 
    

    
    GameObject inst = Instantiate(projectile, user.transform.position + user.transform.forward, user.transform.rotation);
    
    var damageable = inst.GetComponent<DamageTrigger>();
    if (damageable != null)
    {
        damageable.SetDamage(damage+damageFI);  // Устанавливаем урон напрямую в DamageTrigger
    }
    
    Destroy(inst, 3);
    Rigidbody rb = inst.AddComponent<Rigidbody>();
    rb.useGravity = false;
    rb.velocity = user.transform.forward * speed;

    // Если damage равно 0, можно добавить дополнительные логи для диагностики
    Debug.Log($"Executing ranged ability with damage: {damage}");
}
}
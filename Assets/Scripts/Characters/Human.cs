using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    private Vector3 destination;
    public int moveSpeed = 3;
    public float health = 100;
    public float mana = 100;

    [Header("Stats")]
    [SerializeField]
    public float manaRegenSpeed = 25;
    [SerializeField]
    private float healthRegenSpeed = 25;

    private float baseHealth;
    private float baseMana;
    private float baseManaRegenSpeed;
    private float baseHealthRegenSpeed;
    public float baseDamage = 10;
    public float additionalDamageFromItems = 0;

    private float SecondSpellcooldown = 2f; // Кулдаун в секундах
    private float SecondSpelllastUseTime; // Время последнего использования

    public Ability[] abilities;

    [SerializeField]
    private Animator animator;

    public event Action<Human> OnHealthChanged = delegate { };
    public event Action<Human> OnManaChanged = delegate { };
    public event Action<Human> OnDied = delegate { };

    private void Awake()
    {
        Move(transform.position);

        baseHealth = health;
        baseMana = mana;
        baseManaRegenSpeed = manaRegenSpeed;
        baseHealthRegenSpeed = healthRegenSpeed;
    }

    public void Move(Vector3 destination)
    {
        this.destination = destination;
    }

    public void DealDamage(float v)
    {
        
        health -= v;
        OnHealthChanged(this);

        if(health <= 0)
        {
            OnDied(this);
            Destroy(gameObject);
        }
    }

    public void Attack(Human player)
    {
        animator.SetTrigger("attack");
        player.DealDamage(10);
        Rotate(player.transform.position - transform.position);
    }

    public float GetCurrentDamage()
    {
        return baseDamage + additionalDamageFromItems;
    }


    public void Rotate(Vector3 forwad, float lerp = 1)
    {
        Vector3 rot = forwad;
        rot.y = 0;
        if(lerp == 1)
        {
            transform.rotation = Quaternion.LookRotation(rot);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rot), lerp);
        }
    }

    public void UseAbility(int abilitySlot)
    {
        Ability abilityData = abilities[abilitySlot];
        if (mana >= abilityData.Mana && abilitySlot==0)
        {
            AddMana(-15);
            
            animator?.SetTrigger("skill");
            abilityData.Execute(this);
        }
        else if(mana >= abilityData.Mana && abilitySlot == 1)
        {   
            if (Time.time - SecondSpelllastUseTime >= SecondSpellcooldown)
            {
                animator?.SetTrigger("skill");
                abilityData.Execute(this);
                AddMana(-50);
                SecondSpelllastUseTime = Time.time;
            }         
            
        }
    }

    public void UseDash(int abilitySlot)
    {
        Ability abilityData = abilities[abilitySlot];
        if (mana >= abilityData.Mana)
        {
            AddMana(-15);
            
            
        }
    }

    private void Update()
    {
        Vector3 flatDestination = destination;
        flatDestination.y = transform.position.y;

        if (Vector3.Distance(transform.position, flatDestination) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, flatDestination, moveSpeed * Time.deltaTime);

            if (animator != null)
            {
                animator?.SetInteger("run", 1);
            }
        }
        else
        {
            if (animator != null)
            {
                animator?.SetInteger("run", 0);
            }
        }


        if(mana < 1000)
        {
            AddMana(Time.deltaTime * manaRegenSpeed);
        }

        if (health < 1000)
        {
            AddHealth(Time.deltaTime * healthRegenSpeed);
        }
    }
    private void AddHealth(float value)
    {
        health = Mathf.Clamp(health + value, 0, 100);
        OnHealthChanged(this);
    }

    private void AddMana(float value)
    {
        mana = Mathf.Clamp(mana + value, 0, 100);
        OnManaChanged(this);
    }

    public void ApplyItemStats(ItemSO item)
{
    health += item.healthBonus;
    mana += item.manaBonus;
    healthRegenSpeed += item.healthRegenBonus;
    manaRegenSpeed += item.manaRegenBonus;
    additionalDamageFromItems += item.damageModifier;
}

public void RemoveItemStats(ItemSO item)
{
    health -= item.healthBonus;
    mana -= item.manaBonus;
    healthRegenSpeed -= item.healthRegenBonus;
    manaRegenSpeed -= item.manaRegenBonus;
    additionalDamageFromItems -= item.damageModifier;
    Debug.Log($"Item stats applied: {item.damageModifier} damage. New BaseDamage: {baseDamage}");
}


}

using UnityEngine;

[System.Serializable]
public class CharacterStats : IDamagable
{
    public string Name;

    [Header("Movement")]
    public Stat walkSpeed;
    public Stat jumpForce;

    [Header("Combat")]
    public Stat maxHealth;
    [HideInInspector] public Stat currentHealth;
    public Stat damageOutput;
    public Stat attackSpeed;
    // private bool damagable = true;


    // Interface
    // bool IDamagable.Damagable 
    // { 
    //     get => damagable;
    //     set => damagable = value; 
    // }

    public void TakeDamage(float incomingDamage) 
    {
        // if(!damagable) return;

        // Logic for damage reduction

        // Apply damage to health
        currentHealth.SetValue(currentHealth.GetValue() - incomingDamage);

        // Update UI. Should have a method that updates the UI which I can invoke in here
        
        if(currentHealth.GetValue() <= 0f) 
        {
            currentHealth.SetValue(0);
            // damagable = false;
            Death();
        }
    }

    public void Death() 
    {
        // Play animation

        // Some other logic

        Debug.Log($"{Name} is death");
    }
}

public interface IDamagable 
{
    abstract void TakeDamage(float damage);
    abstract void Death();
    
    // bool Damagable { get; set; }
}
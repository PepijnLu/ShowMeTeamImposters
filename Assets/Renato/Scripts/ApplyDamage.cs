using UnityEngine;

public class ApplyDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider) 
    {
        PlayerManager player = gameObject.GetComponentInParent<PlayerManager>();
        
        // If player is attacking
        if(!player.isAttacking) return; 

        if(collider.CompareTag("Player")) 
        {
            if(collider.TryGetComponent<PlayerManager>(out var opponent)) 
            {
                // Fetch the player from this script
                if(player != null) 
                {
                    if(opponent.name != player.name) 
                    {
                        opponent.stats.TakeDamage(player.stats.damageOutput.GetValue());
                        Debug.Log($"{player.name} applied damage to {opponent.name}");
                    }
                }
            }
        }
    }
}

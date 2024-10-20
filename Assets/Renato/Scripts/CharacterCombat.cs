public class CharacterCombat
{
    private PlayerManager playerManager;
    public CharacterAnimator animator;
    // public bool isAttacking = false;

    public CharacterCombat(PlayerManager playerManager) 
    {
        this.playerManager = playerManager;
        animator = new();
    }
    
    public void Punch() 
    {
        if(!playerManager.isAttacking) 
        {
            playerManager.isAttacking = true;

            animator.PunchAnim();

            // Other logic...
            
            // Do Damage (done in another script: ApplyDamage, might change this)

            // Apply knockback if condition is met
        }
    }

    public void ResetAttack()
    {
        playerManager.isAttacking = false;
    }
}

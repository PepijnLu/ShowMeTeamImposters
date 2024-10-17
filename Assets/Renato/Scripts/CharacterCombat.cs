public class CharacterCombat
{
    public CharacterAnimator animator;

    public CharacterCombat() 
    {
        animator = new();
    }
    
    public void Punch() 
    {
        animator.PunchAnim();

        // Other logic...
    }
}

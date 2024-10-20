using UnityEngine;

public class CharacterAnimator
{
    private Animator animator;

    public void Start(Transform transform)
    {
        // animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
        animator = transform.GetComponent<Animator>();
        if(animator != null) 
        {
            Debug.Log($"Animator: {animator.name} found!");
        }
        else 
        {
            Debug.LogWarning("Animator not found!");
        }
    }

    public void Idle() 
    {
        animator.SetBool("Walk Forward", false);
        animator.SetBool("Walk Backward", false);
    }

    public void WalkForwardAnim() 
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Walk Backward", false);
        animator.SetBool("Walk Forward", true);
    }

    public void WalkBackwardAnim() 
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Walk Forward", false);
        animator.SetBool("Walk Backward", true);
    }

    public void PunchAnim() 
    {
        animator.SetBool("Idle", false);
        animator.SetTrigger("PunchTrigger");
    }
}

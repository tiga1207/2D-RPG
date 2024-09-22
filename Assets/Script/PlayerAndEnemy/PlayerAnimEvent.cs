using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{   
    private Player player;
    
    void Start()
    {
        player =GetComponentInParent<Player>();
    }

    private void AnimationTrigger()
    {
        if (!player.isTakeDamage)
        {
            player.AttackOver();
        }
        // player.AttackOver();
    }

    private void CheckFinishAnim()
    {
        player.PlayerDieAfter();
    }

    private void DestoryEffect()
    {
        Destroy(gameObject);
    }
    
}

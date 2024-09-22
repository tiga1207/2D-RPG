using UnityEngine;

public class SkeletonAnimEvent : MonoBehaviour
{   
    private Enemy_Skeleton enemy_Skeleton;
    
    void Start()
    {
        enemy_Skeleton =GetComponentInParent<Enemy_Skeleton>();
    }


    private void CheckFinishAnim()
    {
        enemy_Skeleton.Enemy_DieAfter();
    }

}
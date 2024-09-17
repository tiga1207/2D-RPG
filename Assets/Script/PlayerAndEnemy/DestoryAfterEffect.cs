using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryAfterEffect : MonoBehaviour
{    
    public void Start()
    {
        Destroy(gameObject,GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
    }
}

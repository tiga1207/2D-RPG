// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Inventory : MonoBehaviour
// {   
//     #region 
//     public static Inventory instace;
//     private void Awake() {
//         if(instace != null)
//         {
//             Destroy(instace);
//             return;
//         }
//         instace = this;
//         DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
//     }
//     #endregion

//     private int slotCnt;

//     public delegate void OnSlotCountChange(int val);//대리자 정의
//     public OnSlotCountChange onSlotCountChange;//대리자 인스턴스화

//     public int SlotCnt 
//     { 
//         get => slotCnt; 
//         set 
//         {
//             slotCnt = value; 
//             // onSlotCountChange.Invoke(slotCnt);
//             onSlotCountChange?.Invoke(slotCnt);
//         } 
            
//     }

    

//     void Start()
//     {
//         slotCnt = 4;
//     }

//     void Update()
//     {
        
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private int slotCnt;

    public delegate void OnSlotCountChange(int val);
    public OnSlotCountChange onSlotCountChange;

    public int SlotCnt
    {
        get => slotCnt;
        set
        {
            slotCnt = value;
            onSlotCountChange?.Invoke(slotCnt); // Null 체크 후 호출
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
    }

    public void Initialize()
    {
        slotCnt = 4;
        // 초기화에 필요한 다른 로직 추가
    }
}


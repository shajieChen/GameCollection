#if !__TIMER__
#define __TIMER__
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class CTimerList : ITimerList
{
    #region  Field   
        private static readonly int nhash_count = 1024 ;  //通过hash 列表来进行管理 我们所有的timer 如果我们足够大的时候 我们通过使用mod 的方法 从而算法复杂度就是为 O(n) 不会
       public static int HASH_COUNT {get {return nhash_count ; }} 
        CTimer[] m_ArrayTimerList ;
        public CTimer[] ArrayTimerList {// 进行简单的封装 
            get{return m_ArrayTimerList; }
            set{m_ArrayTimerList = value  ; } 
        }
        int m_nGlobalTimerId ;
        public int nGlobalTimerId{// 进行简单的封装 
            get {return m_nGlobalTimerId ; }
            set {m_nGlobalTimerId= value ;  } 
        } 
   #endregion

    #region SingleTon 
        private static CTimerList instance;
        public static CTimerList Instance { get{
            if(instance == null)
                    instance = CreateTimerList();
                return instance ;    
        } }
 
   #endregion



   private static CTimerList CreateTimerList()
    {
        instance = new CTimerList() ;
        instance.m_ArrayTimerList = new CTimer[HASH_COUNT] ; 
        instance.m_nGlobalTimerId = 0 ;  
        return instance;  
    }

    public void Release()
    {
        for(int i = 0 ; i < m_ArrayTimerList.Length;  i ++)
        {
            CTimer tmpTimer = m_ArrayTimerList[i].CHashNextTimer;    
            for( ; tmpTimer !=null ; )
            {
                CTimer nextTimer = tmpTimer;  
                tmpTimer = tmpTimer.CHashNextTimer ; 
                nextTimer.Release(); 
            }  
        }
        m_nGlobalTimerId = 0 ; 
    }

}
//暴露给用户使用的
public class CTimer
{ 
    #region  Field
        int m_nEndTimeStamp; // 结束的时间戳 毫秒表示
        int m_nTimerId ; // 这个timer 对应的timer id 
        int m_nRepeatTime ;  // 重复次数 如果是-1 表示一直重复使用 
        int m_nDuration ; 
        CTimer m_CHashNextTimer; // 指向下一个hash 的节点， 也就是避免了当切hash table 不够的时候 被堆叠的情况管理 
        public Action On_Timer ; 
        public Action<object> On_Timer2; // 允许一个参数 的回调函数
        public Action <object,object> On_Timer3 ; //允许两个参数的回调函数
        public object Data ;  //第一个参数
        public object Data2 ; //第二个参数 
   #endregion

   #region  Encap 
        public int NEndTimeStamp { get => m_nEndTimeStamp; set => m_nEndTimeStamp = value; }
        public int NDuration { get => m_nDuration ; set => m_nDuration = value ;   }
        public int NTimerId { get => m_nTimerId; set => m_nTimerId = value; }
        public int NRepeatTime { get => m_nRepeatTime; set => m_nRepeatTime = value; }
        public CTimer CHashNextTimer { get => m_CHashNextTimer; set => m_CHashNextTimer = value; }
   #endregion

        public void Release()
        {
            this.m_nEndTimeStamp = 0 ; 
            this.m_nTimerId = 0 ; 
            this.m_nRepeatTime = 0 ; 
            this.m_CHashNextTimer = null ; 
        }

}

#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using System;  
public static class CTimerMgr
{ 
    public static int AddTimer(CTimerList In_cTimerL, Action In_eCallBack, float In_fAfterSeconds, object In_objPara1 = null , object In_objPara2 = null  ) // 添加timer 到timerlist 中 
    {
        if (In_cTimerL is null)
        {
            return -1 ; 
        }

        int nTimerId = In_cTimerL.nGlobalTimerId ++  ; 
   
        CTimer cNewTimer = new CTimer ();
          
        cNewTimer.NRepeatTime = 1  ; //默认情况下 为不重复

        cNewTimer.NDuration = (int)(In_fAfterSeconds * 1000) ; 

        cNewTimer.NEndTimeStamp = System.DateTimeOffset.Now.Millisecond  + cNewTimer.NDuration ; //结束时间戳 获取当前的时间 最后在加上上面声明的时间
       
        cNewTimer.NTimerId = nTimerId ;  

        cNewTimer.Data = In_objPara1 ; 
        
        cNewTimer.Data2 = In_objPara2 ; 


        if(In_objPara1 == null && In_objPara2 == null)
            cNewTimer.On_Timer = In_eCallBack ; 
        else
        {
            cNewTimer.On_Timer = null ; 
        }

        
        int tmp_nIndex =  nTimerId % CTimerList.HASH_COUNT; //将当前的Global Timer id 复制给这个新的timer  
        
        CTimer tmp_cTimer = In_cTimerL.ArrayTimerList[tmp_nIndex] ; //加入到尾巴去 
        CTimer lastTimer  = null ; 

        while(tmp_cTimer != null)
        {
            lastTimer = tmp_cTimer ; 
            tmp_cTimer = tmp_cTimer.CHashNextTimer ;  
        }
        if(lastTimer != null )
            lastTimer.CHashNextTimer = cNewTimer; 

        tmp_cTimer = cNewTimer ; 
        // cNewTimer.CHashNextTimer = In_cTimerL.ArrayTimerList[tmp_nIndex+1] ;  //这里容易嵌套加入
       


        // In_cTimerL.ArrayTimerList[tmp_nIndex] = cNewTimer ; //  

        return nTimerId ; 
    }
    public static void CancelTimer(CTimerList In_cTimerL, int In_nTimeId) //删除 list 里面的内容
    {
        int tmp_nIndex = In_nTimeId % CTimerList.HASH_COUNT ; 

        for(int i = 0 ;  i < In_cTimerL.ArrayTimerList.Length ; i ++)
        {
            if(In_cTimerL.ArrayTimerList[i].NTimerId == In_nTimeId ) // 遍历列表找到相关id 的对象
            {
                CTimer tmpTimer = In_cTimerL.ArrayTimerList[i]; 

                In_cTimerL.ArrayTimerList[i] = In_cTimerL.ArrayTimerList[i].CHashNextTimer ;

                tmpTimer.Release(); // 释放

                return ; 
            }
        }
    } 
    public static int ScheduleTimer(CTimerList In_cTimerL, Action In_eCallBack, float In_fAfterSeconds) // 调节timer
    {
         if (In_cTimerL is null)
        {
            return -1 ; 
        }

        int nTimerId = In_cTimerL.nGlobalTimerId ++  ; 
   
        CTimer cNewTimer = new CTimer ();
          
        cNewTimer.NRepeatTime = -1  ; //默认情况下 为不重复

        cNewTimer.NDuration = (int)(In_fAfterSeconds * 1000) ; 

        cNewTimer.NEndTimeStamp = System.DateTimeOffset.Now.Millisecond  + cNewTimer.NDuration ; //结束时间戳 获取当前的时间 最后在加上上面声明的时间
       
        cNewTimer.NTimerId = nTimerId ;  
        
        int tmp_nIndex =  nTimerId % CTimerList.HASH_COUNT; //将当前的Global Timer id 复制给这个新的timer 
        
        CTimer tmp_cTimer = In_cTimerL.ArrayTimerList[tmp_nIndex].CHashNextTimer ; //加入到尾巴去 
        while(tmp_cTimer != null)
        {
            tmp_cTimer = tmp_cTimer.CHashNextTimer ;  
        }
        tmp_cTimer = cNewTimer ; 
        // cNewTimer.CHashNextTimer = In_cTimerL.ArrayTimerList[tmp_nIndex] ;  //这里容易嵌套加入
       
        // In_cTimerL.ArrayTimerList[tmp_nIndex] = cNewTimer ; //  

        return nTimerId ; 
    }
    public static void DestroyTimerList(CTimerList In_cTimerL)
    {
        In_cTimerL.Release(); 
    }

    public static int UpdateTimerList(CTimerList In_cTimerL)
    {
        int nMin_Sec = Int32.MaxValue ;  // 最紧迫的timer 运行
        CTimer cMin_Timer = null ; 

        int nStartMSec = System.DateTimeOffset.Now.Millisecond ;

        for(int i = 0 ; i < In_cTimerL.ArrayTimerList.Length ; i ++)
        {
            if( In_cTimerL.ArrayTimerList[i].CHashNextTimer.Equals(null))
                break ; 
            CTimer tmpTimer = In_cTimerL.ArrayTimerList[i].CHashNextTimer;    

            for( ; tmpTimer !=null ; )
            {
                int nCurTime = System.DateTimeOffset.Now.Millisecond  ; // 获取当前时间

                if(tmpTimer.NEndTimeStamp <= nCurTime )  // 需要触发回调函数
                {
                    if(tmpTimer.On_Timer != null ) // 需要触发回调函数
                    {
                        if(tmpTimer.Data == null && tmpTimer.Data2 == null )
                            tmpTimer.On_Timer() ;
                             
                        if(tmpTimer.NRepeatTime > 0 )
                         {
                            tmpTimer.NRepeatTime -- ;
                             if(tmpTimer.NRepeatTime == 0 )
                            {
                                CTimer tmpTimer2 = tmpTimer  ; 
                                tmpTimer = tmpTimer.CHashNextTimer ; 
                                tmpTimer2.Release() ;  
                            }
                            else
                            {
                                tmpTimer.NEndTimeStamp =  System.DateTimeOffset.Now.Millisecond  + tmpTimer.NDuration;  
                                
                                if((tmpTimer.NEndTimeStamp - nStartMSec ) < nMin_Sec )
                                {
                                    cMin_Timer = tmpTimer ; 
                                    nMin_Sec = tmpTimer.NEndTimeStamp - nStartMSec ; 
                                }
                                    
                                tmpTimer = tmpTimer.CHashNextTimer ; 
                            }
                         }
                         else
                         {
                            tmpTimer.NEndTimeStamp =  System.DateTimeOffset.Now.Millisecond  + tmpTimer.NDuration;

                            if((tmpTimer.NEndTimeStamp - nStartMSec ) < nMin_Sec )
                            {
                                cMin_Timer = tmpTimer ; 
                                nMin_Sec = tmpTimer.NEndTimeStamp - nStartMSec ; 
                            } 

                            tmpTimer = tmpTimer.CHashNextTimer ; 
                         } 
                    }
                    else //不需要触发的时候
                    { 
                        if((tmpTimer.NEndTimeStamp - nStartMSec ) < nMin_Sec )
                        {
                            cMin_Timer = tmpTimer ; 
                            nMin_Sec = tmpTimer.NEndTimeStamp - nStartMSec ; 
                        }

                        tmpTimer = tmpTimer.CHashNextTimer ; 
                    }
                }
            }  
        }
        if(cMin_Timer != null) // 存在一个时间紧迫的timer
        {
            return ( cMin_Timer.NEndTimeStamp - System.DateTimeOffset.Now.Millisecond); 
        }
        return -1 ; 
    }
}

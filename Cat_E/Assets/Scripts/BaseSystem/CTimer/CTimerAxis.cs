using System.Collections.Generic;
using System; 
 
    /// <summary>
    /// 始终结构体
    /// </summary>
class sTimer
{
    public int dwTimerID = 0;               // 定时器ID
    public ITimerHandler handler = null;    // 回调句柄
    public int dnDuration = 0;              // 回调间隔, 向外暴露的时候是秒为主 ，但是我们要转换成毫秒
    public int dwCallTimes = 0;             // 总共需要回调多少次
    public int dwLastCallTick = 0;          // 最后一次调用的时间
    public int dwGridIndex = 0;             // 所在的时间刻度
    public string debugInfo = "";           // 调试信息

    public bool bRemove = false;
    private int m_nLocked = 0;

    // 上锁
    public void AddLock()
    {
        m_nLocked++;
    }
    // 解锁
    public void SubLock()
    {
        m_nLocked--;
        if (m_nLocked < 0)
        {
            m_nLocked = 0;
        }
    }
    // 是否上锁
    public bool isLock()
    {
        return (m_nLocked > 0);
    }
};


class CTimerAxis : ITimerAxis
{
    
    public const int CHECK_FREQUENCY = 16;      // 推荐检查频率	:	16(ms) , 也就是要求游戏频率要低于60FPS下
    public const int TIME_GRID = 16;            // 推荐时间刻度 :   16(ms)

    public const int TIME_AXIS_LENGTH = 720000;
    public const int INVALID_TIMER = int.MaxValue;
    public const int INFINITY_CALL = int.MaxValue;
    public const int ARRAY_LENGTH = (TIME_AXIS_LENGTH + TIME_GRID - 1) / TIME_GRID;

    // 数据管理
    private Dictionary<ITimerHandler, List<sTimer>> m_DicTimer = new Dictionary<ITimerHandler, List<sTimer>>();
    // 刻度管理
    private LinkedList<sTimer>[] m_TimerAxis = new LinkedList<sTimer>[CTimerAxis.ARRAY_LENGTH];


    // 最后一次检查的时间
    private int m_dwLastCheckTick = 0;
    // 时间轴初始时间   
    private int m_dwCurTime = 0;


    /// <summary>
    /// 创建
    /// </summary>
    /// <returns></returns>
    public bool Create()
    {
        for (int i = 0; i < CTimerAxis.ARRAY_LENGTH; i++)
        {
            m_TimerAxis[i] = new LinkedList<sTimer>();
        }
        return true;
    }


    /// <summary>
    /// 释放
    /// </summary>
    public void Release()
    {
        
        for (int i = 0; i < CTimerAxis.ARRAY_LENGTH; i++)
        {
            LinkedList<sTimer> node = m_TimerAxis[i];
            if (node != null)
            {
                node.Clear();
            }
            m_TimerAxis[i] = null;
        }

        m_DicTimer.Clear();

    }



    /// <summary>
    /// 设置一个定时器
    /// </summary>
    /// <param name="nID">定时器ID</param>
    /// <param name="fTime">定时器调用间隔</param>
    /// <param name="handler">处理接口</param>
    /// <param name="callTimes">调用次数,默认调用无穷次</param>
    /// <param name="debugInfo">调试信息</param>
    /// <returns>成功/失败</returns>
    public bool AddTimer(int nID, float fTime, ITimerHandler handler, int nCallTimes, string szLog)
    {
        
        if( handler == null )
        {
            return false;
        }

        // 进行最小时间安全判断
        if(fTime < CTimerAxis.CHECK_FREQUENCY / 1000.0f)
        {
            fTime = CTimerAxis.CHECK_FREQUENCY / 1000.0f;
        }
        
        // 诞生结构体
        sTimer timer = new sTimer();
        timer.dwTimerID = nID;
        timer.handler = handler;
        timer.dnDuration= (int)(fTime * 1000);
        timer.dwCallTimes = nCallTimes;
        timer.dwLastCallTick = m_dwLastCheckTick;
        timer.debugInfo = szLog;

        // 检查相同的回调函数 是否已经有TimerList 了， 如果没有创建一个新的 ， 如果已经有了在旧的上面进行添加
        List<sTimer> list = null;
        if (false == m_DicTimer.TryGetValue(handler, out list))
        {
            list = new List<sTimer>();
            m_DicTimer.Add(handler, list);
        }

        // 进行查重
        foreach(var node in list)
        {
            if(node.dwTimerID == timer.dwTimerID)
            {
                return false;
            }
        }
        list.Add(timer);


        // 将节点插入到时间轴上
        timer.dwGridIndex = ((timer.dwLastCallTick + timer.dnDuration) / CTimerAxis.TIME_GRID) % CTimerAxis.ARRAY_LENGTH;
        LinkedList<sTimer> GridPos = m_TimerAxis[timer.dwGridIndex]; /*注意这里的计算方法 ，如果开始为0 1s 对应的gridId 为62 ，由于浮点数计算的时候无法精确， 我们hashtable会很大，1/0.016（一帧的时间）也就是说我们运行1s 需要在62帧之后才能运行 */
        GridPos.AddFirst(timer);

        return true;
    }

    /// <summary>
    /// 杀掉一个定时器
    /// </summary>
    /// <param name="timerID">定时器ID</param>
    /// <param name="handler">处理接口</param>
    /// <returns>成功失败</returns>
    public bool DeleteTimer(int nID, ITimerHandler handler)
    {
        List<sTimer> list = null;
        if (false == m_DicTimer.TryGetValue(handler, out list))
        {
            return false;
        }

        // 遍历
        foreach(var node in list)
        {
            if(node.dwTimerID == nID)
            {
                if(node.isLock() == true)
                {
                    node.bRemove = true;
                }
                else
                {
                    // 删除刻度
                    LinkedList<sTimer> GridPos = m_TimerAxis[node.dwGridIndex];
                    GridPos.Remove(node);
                    // 删除管理
                    list.Remove(node);
                    break;
                }
            }
        }

        return true;
    }




    /// <summary>
    /// 检测时间
    /// </summary>
    public void UpdateTimers(float fDt)
    {
        m_dwCurTime += (int)(fDt * 1000);
        // 时间小于最小精度 则返回
        if (m_dwCurTime - m_dwLastCheckTick < CTimerAxis.CHECK_FREQUENCY)
        {
            return;
        }
        // 计算出开始步长和结束步长
        int start_grid = (m_dwLastCheckTick / CTimerAxis.TIME_GRID) % CTimerAxis.ARRAY_LENGTH;
        int end_grid = (m_dwCurTime / CTimerAxis.TIME_GRID) % CTimerAxis.ARRAY_LENGTH;

        m_dwLastCheckTick = m_dwCurTime;


        int i = start_grid;
        while(i != end_grid)
        {
            LinkedList<sTimer> list = m_TimerAxis[i];
            LinkedListNode<sTimer> pNode = list.First;
            while (pNode != null)
            {
                sTimer timer = pNode.Value;
                if (m_dwCurTime - timer.dwLastCallTick >= timer.dnDuration)
                {
                    try
                    {
                        timer.AddLock();
                        timer.handler.OnTimer(timer.dwTimerID);
                        timer.SubLock();
                    }
                    catch (Exception e)
                    {
                        timer.SubLock(); 
                        throw e;
                    }
                    
                    LinkedListNode<sTimer> pTempNode = pNode;
                    pNode = pNode.Next;
                    

                    // 如果要执行删除
                    if (timer.bRemove == true && timer.isLock() == false)
                    {
                        DeleteTimer(timer.dwTimerID, timer.handler);
                        continue;
                    }

                    // 如果调用次数已经用完 执行删除
                    if (timer.dwCallTimes > 0)
                    {
                        timer.dwCallTimes -= 1;
                        if (timer.dwCallTimes <= 0)
                        {
                            DeleteTimer(timer.dwTimerID, timer.handler);
                            continue;
                        }
                    }

                    // 搬迁到下一次触发的位置
                    timer.dwLastCallTick = m_dwCurTime;
                    timer.dwGridIndex = ((timer.dwLastCallTick + timer.dnDuration) / CTimerAxis.TIME_GRID) % CTimerAxis.ARRAY_LENGTH;
                    LinkedList<sTimer> GridPos = m_TimerAxis[timer.dwGridIndex];
                    //GridPos.AddFirst(timer);
                    GridPos.AddLast(timer);
                    list.Remove(pTempNode);
                }

            }

            i = (i + 1) % CTimerAxis.ARRAY_LENGTH;
        } 
    }
}

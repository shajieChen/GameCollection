public interface ITimerHandler
{
    /// <summary>
    /// 定时检查时间轴
    /// </summary>
    /// <param name="dwEventID">事件ID</param>
    void OnTimer(int dwEventID);
}


public interface ITimerAxis
{
    /// <summary>
    /// 创建
    /// </summary>
    /// <returns>成功/失败</returns>
    bool Create();

    /// <summary>
    /// 释放
    /// </summary>
    void Release();


    /// <summary>
    /// 设置一个定时器
    /// </summary>
    /// <param name="nID">定时器ID</param>
    /// <param name="fTime">定时器调用间隔 秒为单位</param>
    /// <param name="handler">处理接口</param>
    /// <param name="callTimes">调用次数,0为调用无穷次</param>
    /// <param name="debugInfo">调试信息</param>
    /// <returns>成功/失败</returns>
    bool AddTimer(int nID, float fTime, ITimerHandler handler, int nCallTimes, string szLog);


    /// <summary>
    /// 杀掉一个定时器
    /// </summary>
    /// <param name="nID">定时器ID</param>
    /// <param name="handler">处理接口</param>
    /// <returns>成功失败</returns>
    bool DeleteTimer(int nID, ITimerHandler handler);

    /// <summary>
    /// 检测时间
    /// </summary>
    /// <param name="fDt">间隔时间</param>
    void UpdateTimers(float fDt);

}

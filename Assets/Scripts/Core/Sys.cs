using System;

/// <summary>
/// 系统相关类
/// </summary>
public class Sys
{
    /// <summary>
    /// 获取一个时间戳
    /// 记录时间的方法有很多种，时间戳是其中一种，时间戳是指从1970年1月1号零点到现在的秒数，
    /// 比方说1970年1月1日一点的时间戳是3600，我们可以中一个long的类型数据保存这个时间戳
    /// </summary>
    /// <returns>返回一个long数据</returns>
    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责客户端消息的分发功能
/// </summary>
public class MsgDistribution
{
    /// <summary>
    /// 每帧处理的消息数量
    /// </summary>
    public int msgNum;
    /// <summary>
    /// 一个消息列表，储存接收到的协议，处理完成后移除  这里也可以使用队列来实现
    /// </summary>
    public List<ProtocolBase> msgList = new List<ProtocolBase>();
    /// <summary>
    /// 同上 只是队列的实现
    /// </summary>
    public Queue<ProtocolBase> msgQueue = new Queue<ProtocolBase>();
    /// <summary>
    /// 一个处理消息的委托
    /// </summary>
    public delegate void Delegate(ProtocolBase protocol);
    ///// <summary>
    ///// 一个处理消息的委托
    ///// </summary>
    //public Action<ProtocolBase> MsgHandler;
    /// <summary>
    /// 事件监听表，重复调用
    /// </summary>
    private Dictionary<string, Action<ProtocolBase>> eventDict = new Dictionary<string, Action<ProtocolBase>>();
    /// <summary>
    /// 只调用一次的事件监听表
    /// </summary>
    private Dictionary<string, Action<ProtocolBase>> onceDict = new Dictionary<string, Action<ProtocolBase>>();

    /// <summary>
    /// 负责消息的分发方法
    /// </summary>
    /// <param name="protocolBase"></param>
    public void DispatchMsgEvent(ProtocolBase protocolBase)
    {
        string protoName = protocolBase.GetName();
        Debug.Log($"分发处理消息--{protoName}");
        
        ///获取协议名称 然后遍历列表 交由指定的委托对象处理该消息
        if(eventDict.ContainsKey(protoName))
        {
            eventDict[protoName](protocolBase);
        }
        //只执行一次的消息
        if(onceDict.ContainsKey(protoName))
        {
            onceDict[protoName](protocolBase);
            //从列表中移除
            onceDict[protoName] = null;
            onceDict.Remove(protoName);
        }
    }

    /// <summary>
    /// Update函数  负责需要一直更新的逻辑
    /// </summary>
    public void Update()
    {
        ///最多每帧处理前msgNum条消息
        for (int i = 0; i < msgNum; i++)
        {
            if(msgList.Count>0)
            {

                ///类似于一个消息队列 每次只分发第一个 然后再删除
                DispatchMsgEvent(msgList[0]);
                //加锁 防止线程竞争
                lock (msgList)
                {
                    msgList.RemoveAt(0);
                }
            }
            else
            {
                break;
            }
        }

        /////使用队列的实现
        //for (int i = 0; i < msgNum; i++)
        //{
        //    if(msgQueue.Count>0)
        //    {
        //        lock (msgQueue)
        //        {
        //            DispatchMsgEvent(msgQueue.Dequeue());
        //        }
        //    }
        //}
    }

    /// <summary>
    /// 向eventDict中添加协议的监听方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void AddListener(string name,Action<ProtocolBase> action)
    {
        if(eventDict.ContainsKey(name))
        {
            eventDict[name] += action;
        }
        else
        {
            eventDict.Add(name,action);
        }
    }


    /// <summary>
    /// 向onceDict中添加协议监听方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void AddOnceListener(string name, Action<ProtocolBase> action)
    {
        if (onceDict.ContainsKey(name))
        {
            onceDict[name] += action;
        }
        else
        {
            onceDict.Add(name, action);
        }
    }

    /// <summary>
    /// 移除协议对应监听方法 eventDict
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void DeleteListener(string name,Action<ProtocolBase> action)
    {
        if(eventDict.ContainsKey(name))
        {
            eventDict[name] -= action;
            if(eventDict[name]==null)
            {
                eventDict.Remove(name);
            }
        }
    }


    /// <summary>
    /// 移除协议对应监听方法onceDict
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void DeleteOnceListener(string name,Action<ProtocolBase> action)
    {
        if(onceDict.ContainsKey(name))
        {
            onceDict[name] -= action;
            if(onceDict[name]==null)
            {
                onceDict.Remove(name);
            }
        }
    }
}

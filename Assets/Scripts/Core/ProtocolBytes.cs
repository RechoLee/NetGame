using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 字节流协议
/// 例如：0003|POS|0001|0002|0004  
/// 字符长度|协议名称|X|Y|Z
/// </summary>
public class ProtocolBytes : ProtocolBase
{
    //传输的字节流
    public byte[] bytes;

    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="readBuff"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public override ProtocolBase Decode(byte[] readBuff, int start, int length)
    {
        //ProtocolBytes protocol = new ProtocolBytes();
        //protocol.bytes = new byte[length];
        //Array.Copy(readBuff,start,protocol.bytes,0,length);
        //return protocol as ProtocolBase;

        bytes = new byte[length];
        Array.Copy(readBuff, start, bytes, 0, length);
        return (this as ProtocolBase);
    }

    /// <summary>
    /// 编码
    /// </summary>
    /// <returns></returns>
    public override byte[] Encode()
    {
        return this.bytes;
    }

    /// <summary>
    /// 提取每一个字节 组装成一个字符串
    /// </summary>
    /// <returns></returns>
    public override string GetDesc()
    {
        if (bytes == null)
            return "";
        string str = "";
        for (int i = 0; i < bytes.Length; i++)
        {
            int b = (int)bytes[i];
            str += b.ToString()+" ";
        }

        return str;
    }

    /// <summary>
    /// 获取协议名称
    /// </summary>
    /// <returns></returns>
    public override string GetName()
    {
        return GetString(0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    private string GetString(int start)
    {
        int end = 0;
        return GetString(start,ref end);
    }

    /// <summary>
    /// 添加字符串,将字符串添加到协议的bytes中
    /// </summary>
    /// <param name="str"></param>
    public void AddString(string str)
    {
        Int32 len = str.Length;
        byte[] lenBytes = BitConverter.GetBytes(len);
        byte[] strBytes = Encoding.UTF8.GetBytes(str);

        if (bytes == null)
            bytes = lenBytes.Concat(strBytes).ToArray();
        else
            bytes = bytes.Concat(lenBytes).Concat(strBytes).ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public string GetString(int start,ref int end)
    {
        if (bytes == null || bytes.Length < start + sizeof(Int32))
            return "";
        //判断是否够一个完整消息长度
        Int32 strLen = BitConverter.ToInt32(bytes, start);
        if (bytes.Length < start + sizeof(Int32) + strLen)
            return "";

        //下一条消息起点
        end = start + sizeof(Int32) + strLen;
        return Encoding.UTF8.GetString(bytes, start + sizeof(Int32), strLen);

    }

    /// <summary>
    /// 向协议体中添加int数据
    /// </summary>
    /// <param name="num"></param>
    public void AddInt(int num)
    {
        byte[] numBytes = BitConverter.GetBytes(num);
        if (bytes == null)
            bytes = numBytes;
        else
            bytes = bytes.Concat(numBytes).ToArray();
    }

    /// <summary>
    /// 获取协议体中的Int数据
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public int? GetInt(int start,ref int end)
    {
        if (bytes == null)
            return null;
        //不足一条完整消息的长度
        if (bytes.Length < sizeof(Int32)+start)
            return null;
        end = start + sizeof(Int32);
        return BitConverter.ToInt32(bytes, start);
    }

    public int? GetIndex(int start)
    {
        int end=0;
        return GetInt(start, ref end);
    }

    /// <summary>
    /// 添加float数据
    /// </summary>
    /// <param name="num"></param>
    public void AddFloat(float num)
    {
        byte[] numBytes = BitConverter.GetBytes(num);
        if (bytes == null)
            bytes = numBytes;
        else
            bytes = bytes.Concat(numBytes).ToArray();
    }

    /// <summary>
    /// 获取Float数据
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns>float? 为Single结构体</returns>
    public float? GetFloat(int start,ref int end)
    {
        if (bytes == null)
            return null;
        if (bytes.Length < start + sizeof(float))
            return null;
        end = start + sizeof(Single);
        return BitConverter.ToSingle(bytes, start);
    }

    /// <summary>
    /// 获取float重载函数
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    public float? GetFloat(int start)
    {
        int end=0;
        return GetFloat(start, ref end);
    }
}


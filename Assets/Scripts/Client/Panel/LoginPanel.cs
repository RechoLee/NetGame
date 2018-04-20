using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{

    private Button btnLogin;
    private Button btnReg;

    private InputField inputId;
    private InputField inputPw;

    private void Awake()
    {

        btnLogin = GameObject.Find("Canvas/BG/FG/Login_btn").GetComponent<Button>();
        btnReg = GameObject.Find("Canvas/BG/FG/Reg_btn").GetComponent<Button>();

        inputId = GameObject.Find("Canvas/BG/FG/InputField_id").GetComponent<InputField>();
        inputPw = GameObject.Find("Canvas/BG/FG/InputField_pw").GetComponent<InputField>();
    }
    // Use this for initialization
    void Start()
    {
        btnLogin.onClick.AddListener(OnLoginClick);
        btnReg.onClick.AddListener(OnRegisterClick);
    }

    /// <summary>
    /// 响应登陆按钮
    /// </summary>
    public void OnLoginClick()
    {
        if (string.IsNullOrEmpty(inputId.text) || string.IsNullOrEmpty(inputPw.text))
        {
            Debug.LogWarning("用户名和密码不能为空");
            return;
        }

        if (NetMgr.serverConn.status != ConnStatus.Connected)
        {
            //连接
            string host = "127.0.0.1";
            int port = 1999;
            if (!NetMgr.serverConn.Connect(host, port))
            {
                Debug.LogError("连接异常");
                return;
            }

        }

        //发送登陆请求协议
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Login");
        protocol.AddString(inputId.text);
        protocol.AddString(inputId.text);

        NetMgr.serverConn.Send(protocol, OnLoginCallBack);
        Debug.Log("login");
    }

    private void OnLoginCallBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = protocol as ProtocolBytes;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int? result = proto.GetInt(start, ref start);
        if (result == 0)
        {
            Debug.Log("登陆成功");
        }
        else
        {
            Debug.Log("登陆失败");
        }
    }

    /// <summary>
    /// 响应注册按钮
    /// </summary>
    public void OnRegisterClick()
    {


        Debug.Log("register");
    }
}

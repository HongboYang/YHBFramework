using System;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetSample : MonoBehaviour
{
    private Socket m_socket;
    [SerializeField] private InputField m_inputField = null;
    [SerializeField] private Text m_text = null;
    private byte[] m_readBuff = new byte[1024];
    private string m_recvStr = "";
    public void Connection()
    {
        m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_socket.BeginConnect("127.0.0.1", 8888, ConnCallback, m_socket);
    }

    private void ConnCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ");
            socket.BeginReceive(m_readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        } 
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket) ar.AsyncState;
            int count = socket.EndReceive(ar);
            m_recvStr = System.Text.Encoding.UTF8.GetString(m_readBuff, 0, count);
            socket.BeginReceive(m_readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    public void Send()
    {
        string sendStr = m_inputField.text;
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
        m_socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, m_socket);
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket) ar.AsyncState;
            int count = socket.EndSend(ar);
            Debug.Log("Socket Send succ" + count);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

    }

    private void Update()
    {
        m_text.text = m_recvStr;
    }

    private void Start()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.fullScreen = false;
    }
}

//�Ʒ� �� ���� �׳� �⺻�̶�� �����ϸ� ����.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;//�� ���� ���̺귯���� ����Ѵ�


public class Node : MonoBehaviour
{
    Text data;
    private WebSocket ws;//���� ����

    void Start()

    {
        data = GetComponentInParent<Text>();
        ws = new WebSocket("ws://localhost:3333/chat"); // ���� ���
                                                      // 127.0.0.1�� ������ ������ �ּ��̴�. 3333��Ʈ�� �����Ѵٴ� �ǹ��̴�.

        ws.OnMessage += ws_OnMessage; //�������� ����Ƽ ������ �޼����� �� ��� ������ �Լ��� ����Ѵ�.

        ws.OnOpen += ws_OnOpen;//������ ����� ��� ������ �Լ��� ����Ѵ�

        ws.OnClose += ws_OnClose;//������ ���� ��� ������ �Լ��� ����Ѵ�.

        ws.Connect();//������ �����Ѵ�.
        ws.Send("�ȳ��ϼ���.");

    }

    void ws_OnMessage(object sender, MessageEventArgs e)

    {
        Debug.Log(e.Data);//���� �޼����� ����� �ֿܼ� ����Ѵ�.
    }

    void ws_OnOpen(object sender, System.EventArgs e)

    {
        Debug.Log("open"); //����� �ֿܼ� "open"�̶�� ��´�.
    }

    void ws_OnClose(object sender, CloseEventArgs e)

    {
        Debug.Log("close"); //����� �ֿܼ� "close"�̶�� ��´�.
    }

}

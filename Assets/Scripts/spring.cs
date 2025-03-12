using UnityEngine;
using SocketIOClient;

public class spring : MonoBehaviour
{
    private SocketIOUnity socket;

    void Start()
    {
        var uri = new System.Uri("http://localhost:3333");
        socket = new SocketIOUnity(uri);

        socket.OnConnected += (sender, e) => {
            Debug.Log("���� �����");
            socket.Emit("greeting", "�ȳ��ϼ���!");
        };

        socket.OnUnityThread("server_response", (response) => {
            string data = response.GetValue<string>();
            Debug.Log("���� ����: " + data);
        });

        socket.OnError += (sender, e) => {
            Debug.LogError("���� ����: " + e);
        };

        socket.Connect();
    }

    void OnDestroy()
    {
        if (socket != null) {
            socket.Disconnect();
            socket.Dispose();
        }
    }
}
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
            Debug.Log("서버 연결됨");
            socket.Emit("greeting", "안녕하세요!");
        };

        socket.OnUnityThread("server_response", (response) => {
            string data = response.GetValue<string>();
            Debug.Log("서버 응답: " + data);
        });

        socket.OnError += (sender, e) => {
            Debug.LogError("소켓 오류: " + e);
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
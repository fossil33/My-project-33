using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using SocketIOClient; // 추가
using SocketIOClient.Newtonsoft.Json; // 추가
public class GameManager : MonoBehaviour {
    public static GameManager instance; // 싱글톤을 할당할 전역 변수
     private SocketIOUnity socket;
    public bool isGameover = false; // 게임 오버 상태
    public bool isPaused = false; // 게임 일시 정지 상태
    public Text scoreText; // 점수를 출력할 UI 텍스트
    public GameObject gameoverUI; // 게임 오버시 활성화 할 UI 게임 오브젝트

    public int score = 0; // 게임 점수

    // 채팅 관련 변수
    public GameObject chatPanel;       // Scroll View Content
    public InputField chatInput;       // 입력 필드
    public Button sendButton;          // 전송 버튼
    public GameObject chatMessagePrefab; // 메시지 프리팹 (Text UI)
    private List<GameObject> chatMessages = new List<GameObject>();

    // 게임 시작과 동시에 싱글톤을 구성
    void Awake() { // MonoBehaviour의 Awake 메서드, 게임 오브젝트가 활성화될 때 호출됨
        if (instance == null) { // 인스턴스가 null인 경우
            instance = this; // 현재 인스턴스를 할당
        } else {
            Debug.LogWarning("씬에 두개 이상의 게임 매니저가 존재합니다!"); // 경고 메시지 출력
            Destroy(gameObject); // 현재 게임 오브젝트 삭제
        }

    // 채팅 이벤트 연결
    sendButton.onClick.AddListener(SendChatMessage); // 전송 버튼 클릭 시 SendChatMessage 호출
    chatInput.onEndEdit.AddListener((text) => { // 입력 필드에서 편집 종료 시 이벤트 리스너 추가
        if (Input.GetKeyDown(KeyCode.Return)) SendChatMessage(); // 엔터 키가 눌리면 SendChatMessage 호출
    });

    var uri = new System.Uri("http://localhost:3333"); // Socket.IO 서버 URI 설정
    socket = new SocketIOUnity(uri); // SocketIOUnity 인스턴스 생성
    socket.JsonSerializer = new NewtonsoftJsonSerializer(); // JSON 직렬화기 설정

    // 연결 이벤트 핸들러
    socket.OnConnected += (sender, e) => { // Socket.IO 연결 성공 시 이벤트 핸들러
        Debug.Log("Socket.IO 연결 성공"); // 연결 성공 메시지 출력
    };

    // 채팅 메시지 수신
    socket.OnUnityThread("chat_message", (response) => { // 'chat_message' 이벤트 수신
        string message = response.GetValue<string>(); // 수신된 메시지를 문자열로 변환
        AddChatMessage(message); // 채팅 메시지를 추가하는 함수 호출
    });

    socket.Connect(); // Socket.IO 서버에 연결 시작
}


    void Update() {
        // 게임 오버 상태에서 게임을 재시작할 수 있게 하는 처리
        if (isGameover == true && Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // ESC 버튼으로 게임 일시 정지 및 재개
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isPaused) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }
    }

    // 점수를 증가시키는 메서드
    public void AddScore(int newScore) {
        if (!isGameover) {
            score += newScore;
            scoreText.text = "Score: " + score;
        }
    }

    // 플레이어 캐릭터가 사망시 게임 오버를 실행하는 메서드
    public void OnPlayerDead() {
        isGameover = true;
        gameoverUI.SetActive(true);
    }

    // 게임 일시 정지 메서드
    void PauseGame() {
        isPaused = true;
        Time.timeScale = 0; // 게임 시간 정지
    }

    // 게임 재개 메서드
    void ResumeGame() {
        isPaused = false;
        Time.timeScale = 1; // 게임 시간 재개
    }

    // 채팅 메시지 전송
    public void SendChatMessage() {
        if (string.IsNullOrEmpty(chatInput.text)) return;

        // 서버에 JSON 형식으로 메시지 전송
        socket.Emit("chat", new { 
            sender = "Player", 
            content = chatInput.text 
        });

        // 로컬 채팅 표시
        AddChatMessage("나: " + chatInput.text);
        chatInput.text = "";
    }

    // 채팅 메시지 추가
    // 채팅 메시지 추가
    public void AddChatMessage(string message) {
        GameObject newMessage = Instantiate(chatMessagePrefab, chatPanel.transform);
        newMessage.GetComponent<Text>().text = message;
        chatMessages.Add(newMessage);

        // 메시지가 너무 많으면 삭제
        if (chatMessages.Count > 20) {
            Destroy(chatMessages[0]);
            chatMessages.RemoveAt(0);
        }

        // 스크롤을 최신 메시지로 이동
        ScrollToBottom();
    }

    // 스크롤을 최신 메시지로 이동하는 메서드
    private void ScrollToBottom() {
        Canvas.ForceUpdateCanvases(); // UI 업데이트 강제 실행
        chatPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0); // 스크롤 위치를 최하단으로 설정
    }
}
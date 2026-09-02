using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; //파일 읽는 라이브러리

[System.Serializable] //JSON을 받을 값들입니다.
public class PlayerSettings
{
    [Header("이동 및 시선 속도 설정")]      // Using JSON
    public float moveSpeed = 0f;           // 캐릭터 이동 속도
    public float mouseSensitivity =0f;    // 마우스 회전 민감도
}

public class PlayerMovement : MonoBehaviour
{
    // [C언어 비교] 구조체의 멤버 변수를 선언하는 것과 같습니다.
    // [SerializeField]나 public을 붙이면 유니티 인스펙터(우측 창)에서 마우스로 수치를 조절할 수 있습니다.


    [Header("연결할 유니티 컴포넌트")]        //Don't Need to Using JSON
    public CharacterController controller;   // 부모(PlayerBody)의 캐릭터 컨트롤러
    public Transform cameraTransform;       // 자식(Head)의 위치/회전 정보
    [Header("JSON 바구니")]
    public PlayerSettings settings;

    private float xRotation = 0f;            // 마우스 위아래(상하) 회전값을 누적 저장할 변수
    private string jsonFilePath;               

    // [유니티 기초] 게임이 시작될 때 딱 한 번 실행되는 함수입니다 (C언어의 main 초기화 부분)
    void Start()
    {
        // 게임 화면을 클릭하면 마우스 커서가 사라지고 게임 창 안에 고정됩니다. (ESC 누르면 다시 풀림)
        Cursor.lockState = CursorLockMode.Locked;

        // 혹시 인스펙터 창에서 깜빡하고 Controller를 연결 안 했을 때를 대비한 '자동 찾기' 안전장치입니다.
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }
        // =================================================================
        // [JSON 불러오기 핵심] 
        // Application.persistentDataPath는 유니티가 보장하는 안전한 저장 경로입니다.
        // C:\Users\유저이름\AppData\LocalLow\회사이름\프로젝트이름 폴더에 저장됩니다.
        // =================================================================
        jsonFilePath = Path.Combine(Application.persistentDataPath, "StreamingAssets", "PlayerConfig.json");

        LoadSettings();
    }

    // [유니티 핵심] 매 프레임마다 화면이 갱신될 때 계속 실행되는 함수입니다 (무한 루프 while문 느낌)
    void Update()
    {
        // =================================================================
        // 1. 마우스 입력 처리 (자식 'Head'는 위아래로, 부모 'PlayerBody'는 좌우로 회전)
        // =================================================================

        // 마우스의 좌우(X), 상하(Y) 움직임 값을 가져옵니다.
        // Time.deltaTime은 컴퓨터 성능(프레임)에 상관없이 회전 속도를 일정하게 맞춰주는 보정값입니다.
        float mouseX = Input.GetAxis("Mouse X") * settings.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * settings.mouseSensitivity * Time.deltaTime;

        // 마우스를 위로 올리면 화면이 위를 봐야 하므로, 누적된 상하 회전값에서 mouseY를 '뺍니다'.
        xRotation -= mouseY;
        // [C# 문법] Mathf.Clamp(값, 최소, 최대) -> 목이 뒤로 꺾이지 않게 -90도에서 90도 사이로 회전을 제한합니다.
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 자식 오브젝트인 'Head'(카메라)만 위아래(X축)로 회전시킵니다.
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 부모 오브젝트인 'PlayerBody' 자체를 좌우(Y축)로 회전시킵니다. (몸통이 통째로 도는 것)
        transform.Rotate(Vector3.up * mouseX);


        // =================================================================
        // 2. 키보드 입력 처리 (W, A, S, D 이동)
        // =================================================================

        // 유니티가 제공하는 기본 입력 세팅입니다. (-1.0에서 1.0 사이의 값이 들어옵니다)
        float x = Input.GetAxis("Horizontal"); // A(좌/-1), D(우/+1) 입력
        float z = Input.GetAxis("Vertical");   // S(후/-1), W(전/+1) 입력

        // 중요: 절대적인 동서남북 기준이 아니라, 내 몸통(PlayerBody)이 바라보는 정면과 오른쪽 기준 벡터를 계산합니다.
        // C언어의 3차원 공간 좌표 계산과 원리가 같습니다.
        Vector3 moveDirection = (transform.forward * z) + (transform.right * x);

        // Character Controller 컴포넌트의 Move 함수를 이용해 실제로 물리 이동을 시킵니다.
        controller.Move(moveDirection * settings.moveSpeed * Time.deltaTime);
    }
    void LoadSettings()
    {
        // 만약 지정된 경로에 JSON 파일이 이미 존재한다면? 읽어오기!
        if (File.Exists(jsonFilePath))
        {
            string jsonText = File.ReadAllText(jsonFilePath);
            settings = JsonUtility.FromJson<PlayerSettings>(jsonText);
            Debug.Log($"[JSON 성공] 세팅을 파일에서 불러왔습니다! 경로: {jsonFilePath}");
        }
        else
        {
            // 만약 처음 실행해서 파일이 없다면? 기본값으로 새 파일을 만들어버립니다.
            settings = new PlayerSettings();
            string jsonText = JsonUtility.ToJson(settings, true); // true를 주면 메모장처럼 예쁘게 정렬됨
            File.WriteAllText(jsonFilePath, jsonText);
            Debug.Log($"[JSON 안내] 파일이 없어서 기본값으로 새로 만들었습니다! 경로: {jsonFilePath}");
        }
    }
}

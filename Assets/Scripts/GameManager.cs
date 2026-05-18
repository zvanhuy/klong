using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Biến static này vẫn giữ giá trị sau khi load lại Scene
    // Dùng để biết người chơi vừa bấm RETRY
    private static bool tuDongChoiLaiSauRetry = false;

    [Header("UI Menu Chính")]
    [InspectorName("Bảng menu bắt đầu")]
    public GameObject bangMenuBatDau;

    [Header("UI Game Over")]
    [InspectorName("Bảng Game Over")]
    public GameObject bangGameOver;

    [Header("UI Khung điểm")]
    [InspectorName("Khung điểm hiện tại")]
    public GameObject khungDiemHienTai;

    [InspectorName("Khung điểm cao nhất")]
    public GameObject khungDiemCaoNhat;

    [Header("UI Điểm số")]
    [InspectorName("Chữ điểm hiện tại")]
    public TextMeshProUGUI chuDiemHienTai;

    [InspectorName("Chữ điểm cao")]
    public TextMeshProUGUI chuDiemCaoNhat;

    [Header("Cài đặt điểm")]
    [InspectorName("Tốc độ tăng điểm")]
    public float tocDoTangDiem = 10f;

    [Header("Cài đặt độ khó")]
    [InspectorName("Mức tăng độ khó theo điểm")]
    public float mucTangDoKhoTheoDiem = 0.002f;

    [InspectorName("Giới hạn độ khó tối đa")]
    public float gioiHanDoKhoToiDa = 1.6f;

    private float diemHienTai = 0f;
    private int diemCaoNhat = 0;

    private bool daGameOver = false;
    private bool daBatDauGame = false;

    private float heSoDoKho = 1f;

    private const string KhoaLuuDiemCaoNhat = "BEST_SCORE";

    public bool IsGameOver
    {
        get { return daGameOver; }
    }

    public bool DaBatDauGame
    {
        get { return daBatDauGame; }
    }

    public float CurrentScore
    {
        get { return diemHienTai; }
    }

    public float DifficultyMultiplier
    {
        get { return heSoDoKho; }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        diemHienTai = 0f;
        heSoDoKho = 1f;
        daGameOver = false;
        daBatDauGame = false;

        diemCaoNhat = PlayerPrefs.GetInt(KhoaLuuDiemCaoNhat, 0);

        if (bangGameOver != null)
        {
            bangGameOver.SetActive(false);
        }

        CapNhatGiaoDienDiem();

        // Nếu vừa bấm RETRY thì vào chơi luôn
        if (tuDongChoiLaiSauRetry)
        {
            tuDongChoiLaiSauRetry = false;
            BatDauGame();
        }
        else
        {
            // Nếu mở game bình thường thì hiện menu chính
            HienMenuBatDau();
        }
    }

    private void Update()
    {
        if (!daBatDauGame || daGameOver)
        {
            return;
        }

        diemHienTai += tocDoTangDiem * Time.deltaTime;

        CapNhatDoKho();
        CapNhatGiaoDienDiem();
    }

    private void HienMenuBatDau()
    {
        Time.timeScale = 0f;

        if (bangMenuBatDau != null)
        {
            bangMenuBatDau.SetActive(true);
        }

        AnHienDiem(false);
    }

    public void BatDauGame()
    {
        Time.timeScale = 1f;

        daBatDauGame = true;
        daGameOver = false;

        diemHienTai = 0f;
        heSoDoKho = 1f;

        if (bangMenuBatDau != null)
        {
            bangMenuBatDau.SetActive(false);
        }

        if (bangGameOver != null)
        {
            bangGameOver.SetActive(false);
        }

        AnHienDiem(true);
        CapNhatGiaoDienDiem();

        Debug.Log("Bắt đầu game.");
    }

    private void CapNhatDoKho()
    {
        heSoDoKho = 1f + diemHienTai * mucTangDoKhoTheoDiem;

        if (heSoDoKho > gioiHanDoKhoToiDa)
        {
            heSoDoKho = gioiHanDoKhoToiDa;
        }
    }

    public void GameOver()
    {
        if (daGameOver)
        {
            return;
        }

        Debug.Log("GAME OVER! KHỦNG LONG ĐÃ CHẾT!");

        daGameOver = true;

        int diemLamTron = Mathf.FloorToInt(diemHienTai);

        if (diemLamTron > diemCaoNhat)
        {
            diemCaoNhat = diemLamTron;
            PlayerPrefs.SetInt(KhoaLuuDiemCaoNhat, diemCaoNhat);
            PlayerPrefs.Save();
        }

        CapNhatGiaoDienDiem();

        Time.timeScale = 0f;

        if (bangGameOver != null)
        {
            bangGameOver.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        // Đánh dấu rằng lần load Scene tiếp theo là do bấm RETRY
        // Sau khi Scene load xong, Start() sẽ tự gọi BatDauGame()
        tuDongChoiLaiSauRetry = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        Debug.Log("Thoát game.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void CapNhatGiaoDienDiem()
    {
        int diemLamTron = Mathf.FloorToInt(diemHienTai);

        if (chuDiemHienTai != null)
        {
            chuDiemHienTai.text = "Điểm: " + diemLamTron;
        }

        if (chuDiemCaoNhat != null)
        {
            chuDiemCaoNhat.text = "Điểm cao: " + diemCaoNhat;
        }
    }

    private void AnHienDiem(bool hien)
    {
        // Ẩn/hiện cả 2 khung xanh chứa điểm
        if (khungDiemHienTai != null)
        {
            khungDiemHienTai.SetActive(hien);
        }

        if (khungDiemCaoNhat != null)
        {
            khungDiemCaoNhat.SetActive(hien);
        }

        // Ẩn/hiện chữ điểm bên trong khung
        if (chuDiemHienTai != null)
        {
            chuDiemHienTai.gameObject.SetActive(hien);
        }

        if (chuDiemCaoNhat != null)
        {
            chuDiemCaoNhat.gameObject.SetActive(hien);
        }
    }

    public void ResetBestScore()
    {
        diemCaoNhat = 0;
        PlayerPrefs.DeleteKey(KhoaLuuDiemCaoNhat);
        PlayerPrefs.Save();

        CapNhatGiaoDienDiem();
    }
}
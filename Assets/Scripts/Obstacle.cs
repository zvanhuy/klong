using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    [InspectorName("Tốc độ gốc")]
    public float tocDoGoc = 5f;

    [InspectorName("Tốc độ tối đa")]
    public float tocDoToiDa = 12f;

    [InspectorName("Vị trí X để tự xóa")]
    public float viTriXDeTuXoa = -15f;

    private void Update()
    {
        // Nếu game đã kết thúc thì vật cản đứng yên
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        float heSoDoKho = 1f;

        // Lấy hệ số độ khó từ GameManager
        if (GameManager.Instance != null)
        {
            heSoDoKho = GameManager.Instance.DifficultyMultiplier;
        }

        // Tốc độ thực tế = tốc độ gốc nhân với hệ số độ khó
        float tocDoThucTe = tocDoGoc * heSoDoKho;

        // Không cho tốc độ vượt quá giới hạn tối đa
        tocDoThucTe = Mathf.Min(tocDoThucTe, tocDoToiDa);

        // Di chuyển vật cản sang trái
        transform.Translate(Vector3.left * tocDoThucTe * Time.deltaTime);

        // Nếu vật cản đi quá xa bên trái thì tự xóa để tránh nặng game
        if (transform.position.x < viTriXDeTuXoa)
        {
            Destroy(gameObject);
        }
    }
}
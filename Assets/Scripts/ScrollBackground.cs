using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    [Header("Cài đặt tốc độ")]
    [InspectorName("Tốc độ gốc")]
    public float speed = 5f;

    [InspectorName("Tốc độ tối đa")]
    public float maxSpeed = 12f;

    [InspectorName("Tăng theo độ khó")]
    public bool tangTheoDoKho = false;

    private float width;

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            width = spriteRenderer.bounds.size.x;
        }
        else
        {
            Debug.LogWarning(gameObject.name + " chưa có SpriteRenderer.");
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        float currentSpeed = speed;

        if (tangTheoDoKho && GameManager.Instance != null)
        {
            currentSpeed = speed * GameManager.Instance.DifficultyMultiplier;
        }

        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

        if (transform.position.x <= -width)
        {
            transform.position = new Vector3(
                transform.position.x + width * 2f,
                transform.position.y,
                transform.position.z
            );
        }
    }
}
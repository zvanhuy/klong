using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefab chướng ngại mặt đất")]
    [InspectorName("Danh sách xương rồng / đá")]
    public GameObject[] danhSachXuongRong;

    [Header("Prefab chim bay")]
    [InspectorName("Danh sách chim")]
    public GameObject[] danhSachChim;

    [Header("Vị trí sinh vật cản")]
    [InspectorName("Điểm sinh vật cản")]
    public Transform diemSinhVatCan;

    [Header("Cài đặt thời gian sinh")]
    [InspectorName("Thời gian sinh ngắn nhất")]
    public float thoiGianSinhNganNhat = 2.2f;

    [InspectorName("Thời gian sinh dài nhất")]
    public float thoiGianSinhDaiNhat = 3.8f;

    [Header("Cài đặt chim")]
    [InspectorName("Điểm bắt đầu cho chim xuất hiện")]
    public int diemBatDauCoChim = 200;

    [Range(0f, 1f)]
    [InspectorName("Tỉ lệ sinh chim sau khi đủ điểm")]
    public float tiLeSinhChim = 0.3f;

    [InspectorName("Độ cao chim thấp nhất")]
    public float doCaoChimThapNhat = -0.4f;

    [InspectorName("Độ cao chim cao nhất")]
    public float doCaoChimCaoNhat = 1.0f;

    private void Start()
    {
        StartCoroutine(VongLapSinhVatCan());
    }

    private IEnumerator VongLapSinhVatCan()
    {
        // Chờ một chút lúc mới bắt đầu để người chơi kịp chuẩn bị
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            {
                yield break;
            }

            SinhMotVatCan();

            float thoiGianCho = Random.Range(thoiGianSinhNganNhat, thoiGianSinhDaiNhat);

            // Không cho spawn quá dày, kể cả về sau
            thoiGianCho = Mathf.Max(1.8f, thoiGianCho);

            yield return new WaitForSeconds(thoiGianCho);
        }
    }

    private void SinhMotVatCan()
    {
        float diemHienTai = 0f;

        if (GameManager.Instance != null)
        {
            diemHienTai = GameManager.Instance.CurrentScore;
        }

        bool duDiemDeCoChim = diemHienTai >= diemBatDauCoChim;
        bool coPrefabChim = danhSachChim != null && danhSachChim.Length > 0;

        bool seSinhChim = false;

        if (duDiemDeCoChim && coPrefabChim)
        {
            seSinhChim = Random.value < tiLeSinhChim;
        }

        // Quan trọng:
        // Mỗi lần chỉ gọi 1 trong 2 hàm này
        // Không bao giờ sinh xương rồng và chim cùng lúc
        if (seSinhChim)
        {
            SinhChim();
        }
        else
        {
            SinhXuongRong();
        }
    }

    private void SinhXuongRong()
    {
        if (danhSachXuongRong == null || danhSachXuongRong.Length == 0)
        {
            Debug.LogWarning("Chưa gán danh sách xương rồng / đá trong Spawner.");
            return;
        }

        int chiSo = Random.Range(0, danhSachXuongRong.Length);
        GameObject prefab = danhSachXuongRong[chiSo];

        Vector3 viTriSinh = LayViTriSinhMacDinh();

        Instantiate(prefab, viTriSinh, Quaternion.identity);
    }

    private void SinhChim()
    {
        if (danhSachChim == null || danhSachChim.Length == 0)
        {
            SinhXuongRong();
            return;
        }

        int chiSo = Random.Range(0, danhSachChim.Length);
        GameObject prefab = danhSachChim[chiSo];

        Vector3 viTriSinh = LayViTriSinhMacDinh();

        viTriSinh.y = Random.Range(doCaoChimThapNhat, doCaoChimCaoNhat);

        Instantiate(prefab, viTriSinh, Quaternion.identity);
    }

    private Vector3 LayViTriSinhMacDinh()
    {
        if (diemSinhVatCan != null)
        {
            return diemSinhVatCan.position;
        }

        return transform.position;
    }
}
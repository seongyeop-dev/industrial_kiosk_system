using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//영수증 UI 캡처 및 PNG 저장
//주문 데이터를 영수증 UI에 반영
//지정 RectTransform 영역을 PNG로 저장
//KioskData/Orders/Receipts 구조 사용
public class scr_ReceiptCapture : MonoBehaviour
{
    [Header("Receipt UI Root")]
    public RectTransform receiptRoot;   //캡처할 영수증 UI 영역

    [Header("Receipt Text References")]
    public Text txtReceiptTitle;
    public Text txtOrderNo;
    public Text txtProductName;
    public Text txtQuantity;
    public Text txtDate;
    public Text txtStatus;

    [Header("Save Settings")]
    public string defaultRootFolderName = "KioskData";
    public string defaultOrderFolderName = "Orders";
    public string defaultReceiptFolderName = "Receipts";
    public string defaultFilePrefix = "Receipt_";

    // 주문 데이터를 영수증 UI 텍스트에 반영
    public void ApplyReceiptData(scr_OrderController.OrderData orderData)
    {
        if (orderData == null)
        {
            return;
        }

        if (txtReceiptTitle != null)
        {
            txtReceiptTitle.text = "RECEIPT";
        }

        if (txtOrderNo != null)
        {
            txtOrderNo.text = orderData.orderNo;
        }

        if (txtProductName != null)
        {
            txtProductName.text = orderData.productName;
        }

        if (txtQuantity != null)
        {
            txtQuantity.text = orderData.quantity;
        }

        if (txtDate != null)
        {
            txtDate.text = orderData.date;
        }

        if (txtStatus != null)
        {
            txtStatus.text = orderData.status;
        }
    }

    //영수증 PNG 저장 요청
    //savePath가 비어 있으면 기본 경로 사용
    //코루틴 시작 요청 성공 시 true 반환
    public bool SaveReceiptPng(scr_OrderController.OrderData orderData, string savePath)
    {
        if (orderData == null)
        {
            Debug.LogWarning("[scr_ReceiptCapture] SaveReceiptPng failed. orderData is null.");
            return false;
        }

        if (receiptRoot == null)
        {
            Debug.LogWarning("[scr_ReceiptCapture] SaveReceiptPng failed. receiptRoot is not assigned.");
            return false;
        }

        ApplyReceiptData(orderData);

        string finalSavePath = savePath;

        if (string.IsNullOrEmpty(finalSavePath))
        {
            string folderPath = Path.Combine(
                Application.persistentDataPath,
                defaultRootFolderName,
                defaultOrderFolderName,
                defaultReceiptFolderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            finalSavePath = Path.Combine(
                folderPath,
                defaultFilePrefix + SanitizeFileName(orderData.orderNo) + ".png");
        }
        else
        {
            string directoryPath = Path.GetDirectoryName(finalSavePath);

            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        StartCoroutine(CaptureReceiptRoutine(finalSavePath, orderData.orderNo));
        return true;
    }

    //영수증 UI 영역을 화면에서 읽어서 PNG 저장
    private IEnumerator CaptureReceiptRoutine(string savePath, string orderNo)
    {
        yield return new WaitForEndOfFrame();

        Vector3[] worldCorners = new Vector3[4];
        receiptRoot.GetWorldCorners(worldCorners);

        float x = worldCorners[0].x;
        float y = worldCorners[0].y;
        float width = worldCorners[2].x - worldCorners[0].x;
        float height = worldCorners[2].y - worldCorners[0].y;

        x = Mathf.Clamp(x, 0f, Screen.width - 1f);
        y = Mathf.Clamp(y, 0f, Screen.height - 1f);
        width = Mathf.Clamp(width, 1f, Screen.width - x);
        height = Mathf.Clamp(height, 1f, Screen.height - y);

        Texture2D texture = new Texture2D(
            Mathf.RoundToInt(width),
            Mathf.RoundToInt(height),
            TextureFormat.RGB24,
            false);

        texture.ReadPixels(
            new Rect(x, y, width, height),
            0,
            0);

        texture.Apply();

        byte[] pngBytes = texture.EncodeToPNG();
        File.WriteAllBytes(savePath, pngBytes);

        Destroy(texture);

        Debug.Log("[scr_ReceiptCapture] Receipt PNG saved : " + savePath);

        if (scr_PageLogController.instance != null)
        {
            scr_PageLogController.instance.AddLogRecord(
                "Order",
                "Receipt PNG Saved",
                "OrderNo : " + orderNo + "\nPath : " + savePath);
        }
    }

    //파일명에 사용할 수 없는 문자 제거
    private string SanitizeFileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Unknown";
        }

        string safeName = value.Trim();

        foreach (char c in Path.GetInvalidFileNameChars())
        {
            safeName = safeName.Replace(c.ToString(), "_");
        }

        return safeName;
    }
}
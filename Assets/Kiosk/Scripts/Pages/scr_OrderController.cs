using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

//문 데이터 전용 컨트롤러
//주문 리스트 메모리 관리
//Master TXT 저장 / 불러오기
//주문 개별 주문서 TXT 출력
//영수증 PNG 저장 경로 관리
//주문 추가 / 수정 / 삭제
public class scr_OrderController : MonoBehaviour
{
    [Header("Save Settings")]
    public string rootFolderName = "KioskData";   
    public string orderFolderName = "Orders";

    public string saveFolderName = "OrderData";            //루트 저장 폴더
    public string saveFileName = "orders.txt";             //전체 주문 Master TXT 파일명
    public string orderSheetFolderName = "OrderSheets";    //주문서 TXT 폴더명
    public string receiptFolderName = "Receipts";          //영수증 PNG 폴더명

    private readonly List<OrderData> orderList = new List<OrderData>();   //메모리상 주문 데이터 목록

    //주문 1건 데이터 구조
    [Serializable]
    public class OrderData
    {
        public string orderNo;
        public string productName;
        public string quantity;
        public string date;
        public string status;

        public OrderData(
            string orderNo,
            string productName,
            string quantity,
            string date,
            string status)
        {
            this.orderNo = orderNo;
            this.productName = productName;
            this.quantity = quantity;
            this.date = date;
            this.status = status;
        }
    }

    //저장 루트 폴더 경로 반환
    public string GetSaveFolderPath()
    {
        return Path.Combine(
            Application.persistentDataPath,
            rootFolderName,
            orderFolderName);
    }

    //전체 주문 Master TXT 경로 반환
    public string GetSaveFilePath()
    {
        return Path.Combine(GetSaveFolderPath(), saveFileName);
    }

    //주문서 TXT 폴더 경로 반환
    public string GetOrderSheetFolderPath()
    {
        return Path.Combine(GetSaveFolderPath(), orderSheetFolderName);
    }

    //영수증 PNG 폴더 경로 반환
    public string GetReceiptFolderPath()
    {
        return Path.Combine(GetSaveFolderPath(), receiptFolderName);
    }

    //특정 주문번호의 주문서 TXT 경로 반환
    public string GetOrderSheetPath(string orderNo)
    {
        return Path.Combine(
            GetOrderSheetFolderPath(),
            "OrderSheet_" + SanitizeFileName(orderNo) + ".txt");
    }

    //특정 주문번호의 영수증 PNG 경로 반환
    public string GetReceiptPath(string orderNo)
    {
        return Path.Combine(
            GetReceiptFolderPath(),
            "Receipt_" + SanitizeFileName(orderNo) + ".png");
    }

    //현재 주문 목록 반환
    //외부 수정 방지를 위해 복사본 반환
    public List<OrderData> GetAllOrders()
    {
        return new List<OrderData>(orderList);
    }

    //저장용 폴더 없으면 생성
    private void EnsureSaveFoldersExist()
    {
        string rootPath = GetSaveFolderPath();
        string orderSheetPath = GetOrderSheetFolderPath();
        string receiptPath = GetReceiptFolderPath();

        if (!Directory.Exists(rootPath))
        {
            Directory.CreateDirectory(rootPath);
        }

        if (!Directory.Exists(orderSheetPath))
        {
            Directory.CreateDirectory(orderSheetPath);
        }

        if (!Directory.Exists(receiptPath))
        {
            Directory.CreateDirectory(receiptPath);
        }
    }

    //시작 시 Master TXT 파일에서 주문 목록 불러오기
    public void LoadOrdersFromTxt()
    {
        orderList.Clear();

        string filePath = GetSaveFilePath();

        if (!File.Exists(filePath))
        {
            Debug.Log("[scr_OrderController] Save file not found. New file will be created later.");
            return;
        }

        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            OrderData parsedData = ParseOrderLine(line);

            if (parsedData != null)
            {
                orderList.Add(parsedData);
            }
        }

        Debug.Log("[scr_OrderController] Load complete. Count = " + orderList.Count);
    }

    //현재 주문 목록을 Master TXT 파일로 저장
    public void SaveOrdersToTxt()
    {
        EnsureSaveFoldersExist();

        string filePath = GetSaveFilePath();
        List<string> lines = new List<string>();

        foreach (OrderData order in orderList)
        {
            lines.Add(ConvertOrderToLine(order));
        }

        File.WriteAllLines(filePath, lines.ToArray(), Encoding.UTF8);

        Debug.Log("[scr_OrderController] Save complete.");
        Debug.Log("[scr_OrderController] Path = " + filePath);
    }

    //주문 추가
    //성공 시 true 반환
    //실패 시 false 반환
    //추가 후 Master TXT 저장 + 주문서 TXT 출력
    public bool AddOrder(
        string orderNo,
        string productName,
        string quantity,
        string date,
        string status)
    {
        if (ContainsOrderNo(orderNo))
        {
            return false;
        }

        OrderData newOrder = new OrderData(orderNo, productName, quantity, date, status);
        orderList.Add(newOrder);

        SaveOrdersToTxt();
        ExportOrderSheetTxt(newOrder);

        return true;
    }

    //targetOrderNo를 기준으로 해당 주문을 찾아 전체 데이터 갱신
    //수정 후 Master TXT 저장 + 주문서 TXT 재출력
    public bool UpdateOrder(
        string targetOrderNo,
        string newOrderNo,
        string newProductName,
        string newQuantity,
        string newDate,
        string newStatus)
    {
        OrderData targetData = FindOrderByOrderNo(targetOrderNo);

        if (targetData == null)
        {
            return false;
        }

        //주문번호가 바뀌는 경우 중복 검사
        if (!string.Equals(targetOrderNo, newOrderNo, StringComparison.OrdinalIgnoreCase) &&
            ContainsOrderNo(newOrderNo))
        {
            return false;
        }

        string oldOrderNo = targetData.orderNo;

        targetData.orderNo = newOrderNo;
        targetData.productName = newProductName;
        targetData.quantity = newQuantity;
        targetData.date = newDate;
        targetData.status = newStatus;

        //주문번호가 변경되면 이전 개별 파일 정리
        if (!string.Equals(oldOrderNo, newOrderNo, StringComparison.OrdinalIgnoreCase))
        {
            DeleteOrderSheetFile(oldOrderNo);
            DeleteReceiptFile(oldOrderNo);
        }

        SaveOrdersToTxt();
        ExportOrderSheetTxt(targetData);

        return true;
    }

    //주문번호 기준 삭제
    //Master TXT 저장
    //주문서 TXT / 영수증 PNG 파일 삭제
    public bool DeleteOrder(string orderNo)
    {
        OrderData targetData = FindOrderByOrderNo(orderNo);

        if (targetData == null)
        {
            return false;
        }

        orderList.Remove(targetData);

        DeleteOrderSheetFile(orderNo);
        DeleteReceiptFile(orderNo);
        SaveOrdersToTxt();

        return true;
    }

    //마지막 주문 삭제
    public bool DeleteLastOrder()
    {
        if (orderList.Count == 0)
        {
            return false;
        }

        int lastIndex = orderList.Count - 1;
        string deletedOrderNo = orderList[lastIndex].orderNo;

        orderList.RemoveAt(lastIndex);

        DeleteOrderSheetFile(deletedOrderNo);
        DeleteReceiptFile(deletedOrderNo);
        SaveOrdersToTxt();

        return true;
    }

    //전체 주문 삭제
    public void ClearAllOrders()
    {
        orderList.Clear();
        SaveOrdersToTxt();

        ClearDirectory(GetOrderSheetFolderPath());
        ClearDirectory(GetReceiptFolderPath());
    }

    //주문번호 존재 여부 확인
    public bool ContainsOrderNo(string orderNo)
    {
        return FindOrderByOrderNo(orderNo) != null;
    }

    //주문번호 기준 데이터 찾기
    public OrderData FindOrderByOrderNo(string orderNo)
    {
        if (string.IsNullOrWhiteSpace(orderNo))
        {
            return null;
        }

        string trimmedOrderNo = orderNo.Trim();

        foreach (OrderData order in orderList)
        {
            if (string.Equals(order.orderNo, trimmedOrderNo, StringComparison.OrdinalIgnoreCase))
            {
                return order;
            }
        }

        return null;
    }

    //다음 주문번호 생성
    //현재 저장된 데이터 기준 가장 큰 숫자 + 1
    //숫자가 아닌 주문번호는 무시
    public string GenerateNextOrderNo(int digits)
    {
        int maxNumber = 0;

        foreach (OrderData order in orderList)
        {
            int parsedNumber;

            if (int.TryParse(order.orderNo, out parsedNumber))
            {
                if (parsedNumber > maxNumber)
                {
                    maxNumber = parsedNumber;
                }
            }
        }

        int nextNumber = maxNumber + 1;
        return nextNumber.ToString("D" + digits);
    }

    //특정 주문의 주문서 TXT 출력
    public void ExportOrderSheetTxt(OrderData order)
    {
        if (order == null)
        {
            Debug.LogWarning("[scr_OrderController] ExportOrderSheetTxt failed. order is null.");
            return;
        }

        EnsureSaveFoldersExist();

        string filePath = GetOrderSheetPath(order.orderNo);
        string content = BuildOrderSheetText(order);

        File.WriteAllText(filePath, content, Encoding.UTF8);

        Debug.Log("[scr_OrderController] OrderSheet TXT saved : " + filePath);
    }

    //전체 주문 주문서 TXT 일괄 출력
    public void ExportAllOrderSheets()
    {
        EnsureSaveFoldersExist();

        foreach (OrderData order in orderList)
        {
            ExportOrderSheetTxt(order);
        }
    }

    //주문서 TXT 내용 생성
    private string BuildOrderSheetText(OrderData order)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("======================================");
        sb.AppendLine("              ORDER SHEET             ");
        sb.AppendLine("======================================");
        sb.AppendLine("Order No   : " + order.orderNo);
        sb.AppendLine("Product    : " + order.productName);
        sb.AppendLine("Quantity   : " + order.quantity);
        sb.AppendLine("Date       : " + order.date);
        sb.AppendLine("Status     : " + order.status);
        sb.AppendLine("======================================");
        sb.AppendLine("Generated  : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        sb.AppendLine("Path Root  : " + GetSaveFolderPath());

        return sb.ToString();
    }

    //주문서 TXT 파일 삭제
    private void DeleteOrderSheetFile(string orderNo)
    {
        if (string.IsNullOrWhiteSpace(orderNo))
        {
            return;
        }

        string path = GetOrderSheetPath(orderNo);

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    //영수증 PNG 파일 삭제
    private void DeleteReceiptFile(string orderNo)
    {
        if (string.IsNullOrWhiteSpace(orderNo))
        {
            return;
        }

        string path = GetReceiptPath(orderNo);

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    //폴더 내부 파일 전체 삭제
    private void ClearDirectory(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            return;
        }

        string[] files = Directory.GetFiles(folderPath);

        foreach (string file in files)
        {
            File.Delete(file);
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

    //OrderData → Master TXT 한 줄 문자열 변환
    //형식: orderNo | productName | quantity|date | status
    private string ConvertOrderToLine(OrderData order)
    {
        return EscapeField(order.orderNo) + "|" +
               EscapeField(order.productName) + "|" +
               EscapeField(order.quantity) + "|" +
               EscapeField(order.date) + "|" +
               EscapeField(order.status);
    }

    //Master TXT 한 줄 → OrderData 변환
    private OrderData ParseOrderLine(string line)
    {
        string[] parts = line.Split('|');

        if (parts.Length < 5)
        {
            Debug.LogWarning("[scr_OrderController] Invalid line skipped: " + line);
            return null;
        }

        string orderNo = UnescapeField(parts[0]);
        string productName = UnescapeField(parts[1]);
        string quantity = UnescapeField(parts[2]);
        string date = UnescapeField(parts[3]);
        string status = UnescapeField(parts[4]);

        return new OrderData(orderNo, productName, quantity, date, status);
    }

    //TXT 저장 시 구분자 충돌 방지용 이스케이프 처리
    private string EscapeField(string value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        return value.Replace("\\", "\\\\").Replace("|", "\\p");
    }

    //TXT 불러오기 시 원래 문자열로 복원
    private string UnescapeField(string value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        return value.Replace("\\p", "|").Replace("\\\\", "\\");
    }
}
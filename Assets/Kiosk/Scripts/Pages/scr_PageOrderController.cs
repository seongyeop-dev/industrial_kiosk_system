using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Order 페이지 컨트롤러
//주문 추가 / 삭제 / 선택 / 수정
//주문번호 자동 생성
//주문번호 중복 검사
//입력값 검증
//리스트 UI 관리
//OrderController 저장 연동
//주문서 TXT 출력 연동
//영수증 PNG 출력 연동
//Log 기록 연동
//하단 Status 메시지 갱신
public class scr_PageOrderController : MonoBehaviour
{
    [Header("Input References")]
    public InputField inputOrderNo;
    public InputField inputProductName;
    public InputField inputQuantity;
    public InputField inputDate;
    public Dropdown dropdownStatus;

    [Header("Button References")]
    public Button btnSave;
    public Button btnReset;
    public Button btnDeleteSelected;
    public Button btnDeleteLast;
    public Button btnEditSelected;
    public Button btnDateToday;
    public Button btnClearSelection;

    [Header("List References")]
    public Transform listContent;
    public GameObject orderListItemPrefab;

    [Header("Order Status UI")]
    public Text txtStatusMessage;   //Order 페이지 하단 Status 박스 텍스트

    [Header("Controller References")]
    public scr_OrderController orderController;
    public scr_ReceiptCapture receiptCapture;
    public scr_UIActionLogger actionLogger;

    [Header("Order Number Options")]
    public int orderNumberDigits = 3;     //001, 002, 003 주문번호 
    public bool autoFillOrderNo = true;   //Reset / Start 시 다음 주문번호 자동 표시

    [Header("Receipt Options")]
    public bool autoSaveReceiptPngOnAdd = true;
    public bool autoSaveReceiptPngOnEdit = true;

    private scr_OrderListItemUI selectedItem;

    private void Start()
    {
        BindButtons();
        LoadSavedOrdersToUI();
        ResetInputs();
        SetStatusMessage("Ready");
    }

    //버튼 이벤트 연결
    private void BindButtons()
    {
        if (btnSave != null) btnSave.onClick.AddListener(AddOrder);
        if (btnReset != null) btnReset.onClick.AddListener(ResetInputs);
        if (btnDeleteSelected != null) btnDeleteSelected.onClick.AddListener(DeleteSelected);
        if (btnDeleteLast != null) btnDeleteLast.onClick.AddListener(DeleteLast);
        if (btnEditSelected != null) btnEditSelected.onClick.AddListener(EditSelected);
        if (btnDateToday != null) btnDateToday.onClick.AddListener(SetTodayDate);
        if (btnClearSelection != null) btnClearSelection.onClick.AddListener(ClearSelection);
    }

    //저장된 주문 데이터 로드 후 UI 리스트 재구성
    private void LoadSavedOrdersToUI()
    {
        ClearAllListItemsOnly();

        if (orderController == null)
        {
            Debug.LogWarning("[scr_PageOrderController] orderController is not assigned.");
            SetStatusMessage("OrderController Missing");
            return;
        }

        orderController.LoadOrdersFromTxt();

        List<scr_OrderController.OrderData> loadedOrders = orderController.GetAllOrders();

        for (int i = 0; i < loadedOrders.Count; i++)
        {
            CreateOrderListItem(loadedOrders[i], false);
        }

        SetStatusMessage("Order List Loaded");
    }

    //주문 추가
    //주문번호가 비어 있으면 자동 생성
    //제품명 / 수량 기본 검증
    //중복 주문번호 방지
    //저장 후 주문서 TXT 출력
    //영수증 PNG 출력
    public void AddOrder()
    {
        if (orderListItemPrefab == null || listContent == null)
        {
            Debug.LogWarning("[scr_PageOrderController] Order Prefab or Content not assigned.");
            SetStatusMessage("Order List Reference Missing");
            return;
        }

        string orderNo = GetInputText(inputOrderNo);
        string product = GetInputText(inputProductName);
        string qty = GetInputText(inputQuantity);
        string date = GetInputText(inputDate);
        string status = GetSelectedStatusText();

        if (!ValidateOrderInput(ref orderNo, product, qty, ref date, null))
        {
            return;
        }

        bool addSuccess = true;

        if (orderController != null)
        {
            addSuccess = orderController.AddOrder(orderNo, product, qty, date, status);
        }
        else
        {
            Debug.LogWarning("[scr_PageOrderController] orderController is null. Data file save will not run.");
        }

        if (!addSuccess)
        {
            Debug.LogWarning("[scr_PageOrderController] Duplicate OrderNo : " + orderNo);
            SetStatusMessage("Save Failed : Duplicate Order No");
            return;
        }

        scr_OrderController.OrderData addedOrderData = null;

        if (orderController != null)
        {
            addedOrderData = orderController.FindOrderByOrderNo(orderNo);
        }

        if (addedOrderData == null)
        {
            addedOrderData = new scr_OrderController.OrderData(orderNo, product, qty, date, status);
        }

        scr_OrderListItemUI createdItem = CreateOrderListItem(addedOrderData, true);

        if (createdItem != null)
        {
            SelectItem(createdItem);
        }

        SaveReceiptIfPossible(addedOrderData, autoSaveReceiptPngOnAdd);     //테스트

        WriteOrderLog(
            "Order Added",
            "OrderNo : " + orderNo +
            "\nProduct : " + product +
            "\nQuantity : " + qty +
            "\nDate : " + date +
            "\nStatus : " + status);

        if (actionLogger != null)
        {
            actionLogger.LogAction("Order Saved : " + orderNo);
        }

        SetStatusMessage("Saved : " + orderNo);
        ResetInputs(false);
    }

    
    //입력 초기화
    //선택 해제
    //날짜는 오늘 날짜 자동 입력
    //주문번호는 다음 번호 자동 표시
    //기본적으로 Status 문구도 갱신
    public void ResetInputs()
    {
        ResetInputs(true);
    }

    //입력 초기화 내부 처리
    private void ResetInputs(bool updateStatusText)
    {
        ClearSelection(false);

        if (inputProductName != null)
        {
            inputProductName.text = "";
        }

        if (inputQuantity != null)
        {
            inputQuantity.text = "";
        }

        if (dropdownStatus != null)
        {
            dropdownStatus.value = 0;
        }

        SetTodayDate();

        if (inputOrderNo != null)
        {
            inputOrderNo.text = autoFillOrderNo ? GenerateNextOrderNo() : "";
        }

        if (updateStatusText)
        {
            SetStatusMessage("Reset Complete");
        }
    }

    //선택된 주문 삭제
    public void DeleteSelected()
    {
        if (selectedItem == null)
        {
            Debug.LogWarning("[scr_PageOrderController] No selected order item.");
            SetStatusMessage("Delete Failed : No Row Selected");
            return;
        }

        string orderNo = selectedItem.GetOrderNo();
        string product = selectedItem.GetProduct();

        if (orderController != null)
        {
            orderController.DeleteOrder(orderNo);
        }
        
        Destroy(selectedItem.gameObject);
        selectedItem = null;

        WriteOrderLog(
            "Order Deleted",
            "Deleted OrderNo : " + orderNo +
            "\nProduct : " + product);

        if (actionLogger != null)
        {
            actionLogger.LogAction("Order Deleted : " + orderNo);
        }

        SetStatusMessage("Deleted : " + orderNo);
        ResetInputs(false);
    }

    //마지막 주문 삭제
    public void DeleteLast()
    {
        if (listContent == null || listContent.childCount == 0)
        {
            Debug.LogWarning("[scr_PageOrderController] No order item in list.");
            SetStatusMessage("Delete Failed : No Order In List");
            return;
        }

        Transform lastTransform = listContent.GetChild(listContent.childCount - 1);
        scr_OrderListItemUI lastItem = lastTransform.GetComponent<scr_OrderListItemUI>();

        if (lastItem == null)
        {
            Destroy(lastTransform.gameObject);
            SetStatusMessage("Last Row Removed");
            ResetInputs(false);
            return;
        }

        string deletedOrderNo = lastItem.GetOrderNo();
        string deletedProduct = lastItem.GetProduct();

        if (orderController != null)
        {
            orderController.DeleteOrder(deletedOrderNo);
        }

        if (selectedItem == lastItem)
        {
            selectedItem = null;
        }

        Destroy(lastTransform.gameObject);

        WriteOrderLog(
            "Order Deleted",
            "Deleted Last Order" +
            "\nOrderNo : " + deletedOrderNo +
            "\nProduct : " + deletedProduct);

        if (actionLogger != null)
        {
            actionLogger.LogAction("Last Order Deleted : " + deletedOrderNo);
        }

        SetStatusMessage("Deleted Last : " + deletedOrderNo);
        ResetInputs(false);
    }

    //선택된 주문 수정
    //선택된 Row 데이터 변경
    //저장 데이터 변경
    //주문서 TXT 재출력
    //영수증 PNG 재출력
    public void EditSelected()
    {
        if (selectedItem == null)
        {
            Debug.LogWarning("[scr_PageOrderController] No selected order item.");
            SetStatusMessage("Edit Failed : No Row Selected");
            return;
        }

        string oldOrderNo = selectedItem.GetOrderNo();

        string orderNo = GetInputText(inputOrderNo);
        string product = GetInputText(inputProductName);
        string qty = GetInputText(inputQuantity);
        string date = GetInputText(inputDate);
        string status = GetSelectedStatusText();

        if (!ValidateOrderInput(ref orderNo, product, qty, ref date, selectedItem))
        {
            return;
        }

        bool updateSuccess = true;

        if (orderController != null)
        {
            updateSuccess = orderController.UpdateOrder(
                oldOrderNo,
                orderNo,
                product,
                qty,
                date,
                status);
        }
        else
        {
            Debug.LogWarning("[scr_PageOrderController] orderController is null. Data file save will not run.");
        }

        if (!updateSuccess)
        {
            Debug.LogWarning("[scr_PageOrderController] Update failed. Duplicate or target not found.");
            SetStatusMessage("Edit Failed");
            return;
        }

        selectedItem.UpdateData(orderNo, product, qty, date, status);
        selectedItem.SetSelected(true);

        scr_OrderController.OrderData updatedOrderData = null;

        if (orderController != null)
        {
            updatedOrderData = orderController.FindOrderByOrderNo(orderNo);
        }

        if (updatedOrderData == null)
        {
            updatedOrderData = new scr_OrderController.OrderData(orderNo, product, qty, date, status);
        }

        SaveReceiptIfPossible(updatedOrderData, autoSaveReceiptPngOnEdit);

        WriteOrderLog(
            "Order Edited",
            "Old OrderNo : " + oldOrderNo +
            "\nNew OrderNo : " + orderNo +
            "\nProduct : " + product +
            "\nQuantity : " + qty +
            "\nDate : " + date +
            "\nStatus : " + status);

        if (actionLogger != null)
        {
            actionLogger.LogAction("Order Edited : " + orderNo);
        }

        SetStatusMessage("Edited : " + oldOrderNo + " -> " + orderNo);
    }

    //오늘 날짜 자동 입력
    public void SetTodayDate()
    {
        if (inputDate != null)
        {
            inputDate.text = System.DateTime.Now.ToString("yyyy-MM-dd");
        }
    }

    //선택 해제
    public void ClearSelection()
    {
        ClearSelection(true);
    }

    //선택 해제 내부 처리
    private void ClearSelection(bool updateStatusText)
    {
        if (selectedItem != null)
        {
            selectedItem.SetSelected(false);
            selectedItem = null;
        }

        if (updateStatusText)
        {
            SetStatusMessage("Selection Cleared");
        }
    }

    //Row에서 호출 → 선택 처리
    public void SelectItem(scr_OrderListItemUI item)
    {
        if (selectedItem != null)
        {
            selectedItem.SetSelected(false);
        }

        selectedItem = item;

        if (selectedItem != null)
        {
            selectedItem.SetSelected(true);

            if (inputOrderNo != null)
            {
                inputOrderNo.text = item.GetOrderNo();
            }

            if (inputProductName != null)
            {
                inputProductName.text = item.GetProduct();
            }

            if (inputQuantity != null)
            {
                inputQuantity.text = item.GetQty();
            }

            if (inputDate != null)
            {
                inputDate.text = item.GetDate();
            }

            SetDropdownStatus(item.GetStatus());
            SetStatusMessage("Selected : " + item.GetOrderNo());
        }
    }

    //주문 리스트 Row 1개 생성
    private scr_OrderListItemUI CreateOrderListItem(scr_OrderController.OrderData data, bool autoSelect)
    {
        if (data == null || orderListItemPrefab == null || listContent == null)
        {
            return null;
        }

        GameObject item = Instantiate(orderListItemPrefab, listContent);
        scr_OrderListItemUI itemUI = item.GetComponent<scr_OrderListItemUI>();

        if (itemUI != null)
        {
            itemUI.Setup(
                data.orderNo,
                data.productName,
                data.quantity,
                data.date,
                data.status,
                this);

            if (autoSelect)
            {
                itemUI.SetSelected(true);
            }
        }

        return itemUI;
    }

    //저장 가능한 경우 영수증 PNG 저장
    private void SaveReceiptIfPossible(scr_OrderController.OrderData orderData, bool shouldSave)
    {
        if (!shouldSave)
        {
            return;
        }

        if (receiptCapture == null)
        {
            Debug.LogWarning("[scr_PageOrderController] receiptCapture is not assigned.");
            SetStatusMessage("Receipt Capture Missing");
            return;
        }

        if (orderController == null)
        {
            Debug.LogWarning("[scr_PageOrderController] orderController is not assigned.");
            SetStatusMessage("OrderController Missing");
            return;
        }

        string savePath = orderController.GetReceiptPath(orderData.orderNo);

        bool requestSuccess = receiptCapture.SaveReceiptPng(orderData, savePath);

        if (requestSuccess)
        {
            WriteOrderLog(
                "Receipt PNG Requested",
                "OrderNo : " + orderData.orderNo +
                "\nPath : " + savePath);

            SetStatusMessage("Receipt Requested : " + orderData.orderNo);
        }
    }

    //저장용 입력값 검증
    private bool ValidateOrderInput(
        ref string orderNo,
        string product,
        string qty,
        ref string date,
        scr_OrderListItemUI ignoreItem)
    {
        if (string.IsNullOrEmpty(product))
        {
            Debug.LogWarning("[scr_PageOrderController] Product name is empty.");
            SetStatusMessage("Input Error : Product Name Is Empty");
            return false;
        }

        if (string.IsNullOrEmpty(qty))
        {
            Debug.LogWarning("[scr_PageOrderController] Quantity is empty.");
            SetStatusMessage("Input Error : Quantity Is Empty");
            return false;
        }

        if (!IsPositiveInteger(qty))
        {
            Debug.LogWarning("[scr_PageOrderController] Quantity must be a positive integer.");
            SetStatusMessage("Input Error : Quantity Must Be Positive Integer");
            return false;
        }

        if (string.IsNullOrEmpty(date))
        {
            date = System.DateTime.Now.ToString("yyyy-MM-dd");

            if (inputDate != null)
            {
                inputDate.text = date;
            }
        }

        if (string.IsNullOrEmpty(orderNo))
        {
            orderNo = GenerateNextOrderNo();

            if (inputOrderNo != null)
            {
                inputOrderNo.text = orderNo;
            }
        }

        if (IsDuplicateOrderNo(orderNo, ignoreItem))
        {
            Debug.LogWarning("[scr_PageOrderController] Duplicate OrderNo : " + orderNo);
            SetStatusMessage("Input Error : Duplicate Order No");
            return false;
        }

        return true;
    }

    //저장 데이터 기준 다음 주문번호 생성
    private string GenerateNextOrderNo()
    {
        if (orderController != null)
        {
            return orderController.GenerateNextOrderNo(orderNumberDigits);
        }

        //orderController 미연결 시 UI 기준 fallback
        int maxNumber = 0;

        if (listContent != null)
        {
            for (int i = 0; i < listContent.childCount; i++)
            {
                scr_OrderListItemUI item = listContent.GetChild(i).GetComponent<scr_OrderListItemUI>();

                if (item == null)
                {
                    continue;
                }

                int parsedNumber;
                if (int.TryParse(item.GetOrderNo(), out parsedNumber))
                {
                    if (parsedNumber > maxNumber)
                    {
                        maxNumber = parsedNumber;
                    }
                }
            }
        }

        int nextNumber = maxNumber + 1;
        return nextNumber.ToString("D" + orderNumberDigits);
    }

    //주문번호 중복 검사
    //저장 데이터 기준 우선 검사
    //수정 시 자기 자신 제외
    private bool IsDuplicateOrderNo(string targetOrderNo, scr_OrderListItemUI ignoreItem)
    {
        if (string.IsNullOrEmpty(targetOrderNo))
        {
            return false;
        }

        if (ignoreItem != null && ignoreItem.GetOrderNo() == targetOrderNo)
        {
            return false;
        }

        if (orderController != null)
        {
            return orderController.ContainsOrderNo(targetOrderNo);
        }

        if (listContent == null)
        {
            return false;
        }

        for (int i = 0; i < listContent.childCount; i++)
        {
            scr_OrderListItemUI item = listContent.GetChild(i).GetComponent<scr_OrderListItemUI>();

            if (item == null)
            {
                continue;
            }

            if (item == ignoreItem)
            {
                continue;
            }

            if (item.GetOrderNo() == targetOrderNo)
            {
                return true;
            }
        }

        return false;
    }

    //상태 드롭다운 값 설정
    private void SetDropdownStatus(string status)
    {
        if (dropdownStatus == null)
        {
            return;
        }

        for (int i = 0; i < dropdownStatus.options.Count; i++)
        {
            if (dropdownStatus.options[i].text == status)
            {
                dropdownStatus.value = i;
                return;
            }
        }
    }

    //InputField 텍스트 가져오기
    private string GetInputText(InputField inputField)
    {
        if (inputField == null)
        {
            return "";
        }

        return inputField.text.Trim();
    }

    //현재 선택된 Status 텍스트 반환
    private string GetSelectedStatusText()
    {
        if (dropdownStatus == null || dropdownStatus.options == null || dropdownStatus.options.Count == 0)
        {
            return "";
        }

        return dropdownStatus.options[dropdownStatus.value].text;
    }

    //양이 정수인지 확인
    private bool IsPositiveInteger(string value)
    {
        int parsedValue;
        return int.TryParse(value, out parsedValue) && parsedValue > 0;
    }

    //현재 리스트 Row만 제거
    //저장 데이터는 건드리지 않음
    private void ClearAllListItemsOnly()
    {
        if (listContent == null)
        {
            return;
        }

        for (int i = listContent.childCount - 1; i >= 0; i--)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }
    }

    //Order 카테고리 로그 기록 공통 함수
    private void WriteOrderLog(string title, string detail)
    {
        if (scr_PageLogController.instance != null)
        {
            scr_PageLogController.instance.AddLogRecord("Order", title, detail);
        }
    }

    //Order 페이지 하단 Status 메시지 갱신
    private void SetStatusMessage(string message)
    {
        if (txtStatusMessage != null)
        {
            txtStatusMessage.text = message;
        }
    }
}
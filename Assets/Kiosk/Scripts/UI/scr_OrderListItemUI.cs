using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Order Row UI
//주문 데이터 표시
//Row 클릭 시 선택 처리
//Hover / Selected 배경 강조
//Status 텍스트 색상 표시
public class scr_OrderListItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Text References")]
    public Text txtOrderNo;
    public Text txtProduct;
    public Text txtQty;
    public Text txtDate;
    public Text txtStatus;

    [Header("UI References")]
    public Button btnRow;            //Row 전체 클릭 버튼
    public Image imgBackground;      //Row 배경 이미지

    [Header("Row Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.93f, 0.96f, 1f);
    public Color selectedColor = new Color(0.75f, 0.9f, 1f);

    [Header("Status Text Colors")]
    public Color readyStatusColor = new Color(0.35f, 0.8f, 1f);
    public Color waitStatusColor = new Color(1f, 0.55f, 0.2f);
    public Color doneStatusColor = new Color(0.2f, 0.9f, 0.35f);
    public Color defaultStatusColor = Color.white;

    private string currentOrderNo;
    private string currentProduct;
    private string currentQty;
    private string currentDate;
    private string currentStatus;

    private bool isSelected = false;
    private bool isHovered = false;

    private scr_PageOrderController pageController;

    //Row 데이터 초기화
    public void Setup(
        string orderNo,
        string product,
        string qty,
        string date,
        string status,
        scr_PageOrderController controller)
    {
        pageController = controller;
        UpdateData(orderNo, product, qty, date, status);

        isSelected = false;
        isHovered = false;
        UpdateBackgroundVisual();

        if (btnRow != null)
        {
            btnRow.onClick.RemoveAllListeners();
            btnRow.onClick.AddListener(OnClickRow);
        }
    }

    //선택된 Row 데이터 갱신
    public void UpdateData(
        string orderNo,
        string product,
        string qty,
        string date,
        string status)
    {
        currentOrderNo = orderNo;
        currentProduct = product;
        currentQty = qty;
        currentDate = date;
        currentStatus = status;

        if (txtOrderNo != null)
        {
            txtOrderNo.text = orderNo;
        }

        if (txtProduct != null)
        {
            txtProduct.text = product;
        }

        if (txtQty != null)
        {
            txtQty.text = qty;
        }

        if (txtDate != null)
        {
            txtDate.text = date;
        }

        if (txtStatus != null)
        {
            txtStatus.text = status;
        }

        ApplyStatusColor(status);
    }

    //Row 클릭 처리
    private void OnClickRow()
    {
        Debug.Log("Order Row Clicked : " + currentOrderNo);  //테스트

        if (pageController != null)
        {
            pageController.SelectItem(this);
        }
    }

    //마우스 진입 시 Hover 표시
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateBackgroundVisual();
    }

    //마우스 이탈 시 Hover 해제
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateBackgroundVisual();
    }

    // 선택 상태 표시
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateBackgroundVisual();
    }

    //현재 상태에 따라 Row 배경색 갱신
    //우선순위: Selected > Hover > Normal
    private void UpdateBackgroundVisual()
    {
        if (imgBackground == null)
        {
            return;
        }

        if (isSelected)
        {
            imgBackground.color = selectedColor;
        }
        else if (isHovered)
        {
            imgBackground.color = hoverColor;
        }
        else
        {
            imgBackground.color = normalColor;
        }
    }

    //Status 텍스트 색상 적용
    private void ApplyStatusColor(string status)
    {
        if (txtStatus == null)
        {
            return;
        }

        txtStatus.color = GetStatusColor(status);
    }

    //Status 문자열에 따라 색상 반환
    private Color GetStatusColor(string status)
    {
        if (string.Equals(status, "Ready", System.StringComparison.OrdinalIgnoreCase))
        {
            return readyStatusColor;
        }

        if (string.Equals(status, "Wait", System.StringComparison.OrdinalIgnoreCase))
        {
            return waitStatusColor;
        }

        if (string.Equals(status, "Done", System.StringComparison.OrdinalIgnoreCase))
        {
            return doneStatusColor;
        }

        return defaultStatusColor;
    }

    public string GetOrderNo()
    {
        return currentOrderNo;
    }

    public string GetProduct()
    {
        return currentProduct;
    }

    public string GetQty()
    {
        return currentQty;
    }

    public string GetDate()
    {
        return currentDate;
    }

    public string GetStatus()
    {
        return currentStatus;
    }

    public GameObject GetItemObject()
    {
        return gameObject;
    }
}
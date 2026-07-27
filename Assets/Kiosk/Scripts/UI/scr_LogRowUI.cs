using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Log Row 1개 표시 + 클릭 처리 + Hover / 선택 하이라이트
//시간 / 아이콘 / 카테고리 / 내용 분리 표시
//클릭 시 PageLogController로 선택 이벤트 전달
//선택 상태에 따라 배경 색상 변경
//Summary / Detail 동시 전달
public class scr_LogRowUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Text txtIcon;
    public Text txtTime;
    public Text txtCategory;
    public Text txtMessage;
    public Button btnRow;
    public Image background;

    [Header("Row Colors")]
    public Color normalColor = new Color(0.18f, 0.25f, 0.34f);
    public Color hoverColor = new Color(0.22f, 0.31f, 0.41f);
    public Color selectedColor = new Color(0.08f, 0.26f, 0.49f);

    [Header("Text Colors")]
    public Color normalTimeColor = new Color(0.78f, 0.84f, 0.92f);
    public Color selectedTimeColor = Color.white;

    public Color normalMessageColor = Color.white;
    public Color selectedMessageColor = Color.white;

    public Color normalCategoryTextColor = Color.white;
    public Color selectedCategoryTextColor = Color.white;

    [Header("Category Colors")]
    public Color logColor = new Color(0.85f, 0.85f, 0.85f);
    public Color alarmColor = new Color(1.00f, 0.45f, 0.45f);
    public Color statusColor = new Color(0.40f, 0.85f, 1.00f);
    public Color orderColor = new Color(0.55f, 1.00f, 0.55f);
    public Color defaultCategoryColor = Color.white;

    [Header("Selected Category Accent")]
    public bool keepCategoryAccentWhenSelected = true;

    private string currentSummary;
    private string currentDetail;
    private string currentCategory = "Log";

    private scr_PageLogController pageController;

    private bool isSelected = false;
    private bool isHovered = false;

    //Log Row 데이터 초기화
    public void Setup(
        string timeText,
        string categoryText,
        string messageText,
        string summaryText,
        string detailMessage,
        scr_PageLogController controller)
    {
        currentSummary = summaryText;
        currentDetail = detailMessage;
        currentCategory = categoryText;
        pageController = controller;

        if (txtTime != null)
        {
            txtTime.text = timeText;
        }

        if (txtCategory != null)
        {
            txtCategory.text = categoryText;
        }

        if (txtMessage != null)
        {
            txtMessage.text = messageText;
        }

        if (txtIcon != null)
        {
            txtIcon.text = GetCategoryIcon(categoryText);
        }

        if (btnRow != null)
        {
            btnRow.onClick.RemoveAllListeners();
            btnRow.onClick.AddListener(OnClick);
        }

        isSelected = false;
        isHovered = false;

        UpdateVisualState();
    }

    //Row 클릭 처리
    public void OnClick()
    {
        if (pageController != null)
        {
            pageController.SelectLogRow(this, currentSummary, currentDetail);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateVisualState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateVisualState();
    }

    //선택 상태 변경
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState();
    }

    //현재 Row 시각 상태 갱신
    private void UpdateVisualState()
    {
        UpdateBackground();
        UpdateTexts();
    }

    //배경 색상 상태 갱신
    //Selected > Hover > Normal 우선순위
    private void UpdateBackground()
    {
        if (background == null)
        {
            return;
        }

        if (isSelected)
        {
            background.color = selectedColor;
        }
        else if (isHovered)
        {
            background.color = hoverColor;
        }
        else
        {
            background.color = normalColor;
        }
    }

    //텍스트 / 아이콘 색상 상태 갱신
    private void UpdateTexts()
    {
        if (txtTime != null)
        {
            txtTime.color = isSelected ? selectedTimeColor : normalTimeColor;
        }

        if (txtMessage != null)
        {
            txtMessage.color = isSelected ? selectedMessageColor : normalMessageColor;
        }

        UpdateCategoryVisual();
    }

    //카테고리 텍스트 / 아이콘 색상 갱신
    private void UpdateCategoryVisual()
    {
        Color accentColor = GetCategoryColor(currentCategory);

        if (txtCategory != null)
        {
            if (isSelected)
            {
                txtCategory.color = keepCategoryAccentWhenSelected ? accentColor : selectedCategoryTextColor;
            }
            else
            {
                txtCategory.color = accentColor;
            }
        }

        if (txtIcon != null)
        {
            if (isSelected)
            {
                txtIcon.color = keepCategoryAccentWhenSelected ? accentColor : selectedCategoryTextColor;
            }
            else
            {
                txtIcon.color = accentColor;
            }
        }
    }

    //카테고리별 아이콘 반환
    private string GetCategoryIcon(string category)
    {
        switch (category)
        {
            case "Log":
                return "●";

            case "Alarm":
                return "▲";

            case "Status":
                return "■";

            case "Order":
                return "◆";

            default:
                return "•";
        }
    }

    //카테고리별 색상 반환
    private Color GetCategoryColor(string category)
    {
        switch (category)
        {
            case "Log":
                return logColor;

            case "Alarm":
                return alarmColor;

            case "Status":
                return statusColor;

            case "Order":
                return orderColor;

            default:
                return defaultCategoryColor;
        }
    }

    public string GetCategory()
    {
        return currentCategory;
    }

    public string GetSummary()
    {
        return currentSummary;
    }

    public string GetDetail()
    {
        return currentDetail;
    }
}
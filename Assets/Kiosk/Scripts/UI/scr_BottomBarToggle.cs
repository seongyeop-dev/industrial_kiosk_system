using UnityEngine;
using UnityEngine.UI;

//BottomBar 확장 / 축소 토글
//버튼 클릭 시 BottomBar 높이 변경
//버튼 텍스트를 EXPAND / COLLAPSE로 변경
public class scr_BottomBarToggle : MonoBehaviour
{
    [Header("Target References")]
    public RectTransform bottomBarRect;     // BottomBar RectTransform
    public Text btnLabel;                   // Btn_ExpandToggle 내부 Text

    [Header("Height Settings")]
    public float collapsedHeight = 60f;     // 기본 높이
    public float expandedHeight = 180f;     // 확장 높이

    [Header("Start State")]
    public bool startExpanded = false;      // 시작 시 펼침 여부

    public RectTransform mainFrameRect;     // 본문 영역 RectTransform

    private bool isExpanded;                //현재 확장 상태

    //시작 시 초기 상태 적용
    private void Start()
    {
        isExpanded = startExpanded;
        ApplyState();
    }

    //버튼 클릭 시 확장 / 축소 토글
    public void ToggleBottomBar()
    {
        isExpanded = !isExpanded;
        ApplyState();
    }

    //현재 상태를 BottomBar와 버튼 텍스트에 반영
    private void ApplyState()
    {
        float targetHeight = isExpanded ? expandedHeight : collapsedHeight;

        if (bottomBarRect != null)
        {
            Vector2 size = bottomBarRect.sizeDelta;
            size.y = targetHeight;
            bottomBarRect.sizeDelta = size;
        }

        if (mainFrameRect != null)
        {
            Vector2 offsetMin = mainFrameRect.offsetMin;
            offsetMin.y = targetHeight;
            mainFrameRect.offsetMin = offsetMin;
        }

        if (btnLabel != null)
        {
            btnLabel.text = isExpanded ? "COLLAPSE" : "EXPAND";
        }
    }
}
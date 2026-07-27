using UnityEngine;
using UnityEngine.UI;

//Alarm Row UI (클릭 / 선택 처리)
public class scr_AlarmRowUI : MonoBehaviour
{
    public Text txtMessage;
    public Image background;

    private scr_AlarmController controller;
    private string alarmText;

    private Color normalColor = new Color(0.16f, 0.22f, 0.28f);
    private Color selectedColor = new Color(0.23f, 0.34f, 0.43f);

    //초기 설정
    public void Setup(string message, scr_AlarmController parentController)
    {
        alarmText = message;
        controller = parentController;

        if (txtMessage != null)
            txtMessage.text = message;
    }

    //클릭 시 호출
    public void OnClick()
    {
        if (controller != null)
        {
            controller.SelectRow(this, alarmText);
        }
    }

    //선택 상태 표시
    public void SetSelected(bool isSelected)
    {
        if (background != null)
        {
            background.color = isSelected ? selectedColor : normalColor;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

//Status 페이지 상태 제어
//READY / RUNNING / ERROR 상태 변경
//Status 페이지 텍스트 및 BottomBar 시스템 상태 갱신
//Detail / Status Log 미리보기 갱신
//LastAction 및 Log 기록 연동
public class scr_StatusController : MonoBehaviour
{
    [Header("UI References")]
    public Text txtStatusValue;
    public Text txtSystemStatus;
    public Text txtDetail;
    public Text txtStatusLogPreview;

    [Header("Status Colors")]
    public Color readyColor = Color.green;
    public Color runningColor = Color.yellow;
    public Color errorColor = Color.red;

    [Header("Logger")]
    public scr_UIActionLogger actionLogger;

    private string currentStatus = "READY";
    private string statusLogText = "";

    private void Start()
    {
        RefreshDetailText();
        RefreshLogPreview("Status Changed : READY");
    }

    public void SetReady()
    {
        UpdateStatus("READY", readyColor);
    }

    public void SetRunning()
    {
        UpdateStatus("RUNNING", runningColor);
    }

    public void SetError()
    {
        UpdateStatus("ERROR", errorColor);
    }

    //상태 텍스트 + 색상 업데이트
    private void UpdateStatus(string statusText, Color color)
    {
        string previousStatus = currentStatus;

        if (txtStatusValue != null)
        {
            txtStatusValue.text = statusText;
            txtStatusValue.color = color;
        }

        if (txtSystemStatus != null)
        {
            txtSystemStatus.text = statusText;
            txtSystemStatus.color = color;
        }

        if (actionLogger != null)
        {
            actionLogger.LogAction("Status Changed : " + statusText);
        }

        if (scr_PageLogController.instance != null && previousStatus != statusText)
        {
            scr_PageLogController.instance.AddLogRecord(
                "Status",
                "Status Changed",
                previousStatus + " -> " + statusText);
        }

        currentStatus = statusText;

        RefreshDetailText();
        RefreshLogPreview("Status Changed : " + statusText);
    }

    //Detail 영역 갱신
    private void RefreshDetailText()
    {
        if (txtDetail == null)
        {
            return;
        }

        string lastActionText = "None";

        if (actionLogger != null && actionLogger.txtLastAction != null)
        {
            lastActionText = actionLogger.txtLastAction.text;
        }

        txtDetail.text =
            "Current Status : " + currentStatus + "\n" +
            "Last Action : " + lastActionText + "\n" +
            "Updated Time : " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    //Status Log 미리보기 갱신
    //최근 3개까지만 표시
    private void RefreshLogPreview(string newLog)
    {
        if (txtStatusLogPreview == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(statusLogText))
        {
            statusLogText = newLog;
        }
        else
        {
            statusLogText = newLog + "\n" + statusLogText;
        }

        string[] lines = statusLogText.Split('\n');

        if (lines.Length > 3)
        {
            statusLogText = lines[0] + "\n" + lines[1] + "\n" + lines[2];
        }

        txtStatusLogPreview.text = statusLogText;
    }

    public string GetCurrentStatus()
    {
        return currentStatus;
    }
}
using UnityEngine;
using UnityEngine.UI;

//Alarm 페이지 더미 알람 리스트 관리
//알람 추가
//전체 알람 삭제
//알람 개수 표시
//Dashboard 조회용 데이터 제공
//선택 로그는 저장하지 않음
public class scr_AlarmController : MonoBehaviour
{
    [Header("UI References")]
    public Text txtAlarmCount;
    public Text txtAlarmDetail;

    [Header("List References")]
    public Transform alarmListContent;
    public GameObject alarmRowPrefab;

    [Header("Dashboard / Summary References")]
    public Text txtLastAlarm;

    [Header("Logger")]
    public scr_UIActionLogger actionLogger;

    private int alarmIndex = 0;
    private scr_AlarmRowUI selectedRow;

    //더미 알람 1개 추가
    public void AddDummyAlarm()
    {
        if (alarmListContent == null || alarmRowPrefab == null)
        {
            Debug.LogWarning("[scr_AlarmController] Alarm list reference is missing.");
            return;
        }

        alarmIndex++;

        GameObject rowObject = Instantiate(alarmRowPrefab, alarmListContent);

        string message = "ALARM_" + alarmIndex.ToString("000") + " : Check system status";

        scr_AlarmRowUI rowUI = rowObject.GetComponent<scr_AlarmRowUI>();
        if (rowUI != null)
        {
            rowUI.Setup(message, this);
        }

        UpdateAlarmCount();

        if (txtAlarmDetail != null)
        {
            txtAlarmDetail.text = "Last Alarm : ALARM_" + alarmIndex.ToString("000");
        }

        if (txtLastAlarm != null)
        {
            txtLastAlarm.text = message;
        }

        if (actionLogger != null)
        {
            actionLogger.LogAction("Alarm Added : " + message);
        }

        if (scr_PageLogController.instance != null)
        {
            scr_PageLogController.instance.AddLogRecord(
                "Alarm",
                "Alarm Added",
                message);
        }
    }

    //전체 알람 삭제
    public void ClearAllAlarms()
    {
        if (alarmListContent == null)
        {
            Debug.LogWarning("[scr_AlarmController] alarmListContent is null.");
            return;
        }

        for (int i = alarmListContent.childCount - 1; i >= 0; i--)
        {
            Destroy(alarmListContent.GetChild(i).gameObject);
        }

        alarmIndex = 0;
        selectedRow = null;
        UpdateAlarmCount();

        if (txtAlarmDetail != null)
        {
            txtAlarmDetail.text = "No alarm selected";
        }

        if (txtLastAlarm != null)
        {
            txtLastAlarm.text = "No alarm";
        }

        if (actionLogger != null)
        {
            actionLogger.LogAction("Alarm Cleared");
        }

        if (scr_PageLogController.instance != null)
        {
            scr_PageLogController.instance.AddLogRecord(
                "Alarm",
                "Alarm Cleared",
                "All alarms were removed.");
        }
    }

    //알람 개수 텍스트 갱신
    private void UpdateAlarmCount()
    {
        if (txtAlarmCount != null)
        {
            int count = alarmListContent != null ? alarmListContent.childCount : 0;
            txtAlarmCount.text = "Alarm Count : " + count;
        }
    }

    //Row 선택 처리
    //선택은 UI 반응만 처리하고 로그는 남기지 않음
    public void SelectRow(scr_AlarmRowUI row, string message)
    {
        if (selectedRow != null)
        {
            selectedRow.SetSelected(false);
        }

        selectedRow = row;

        if (selectedRow != null)
        {
            selectedRow.SetSelected(true);
        }

        if (txtAlarmDetail != null)
        {
            txtAlarmDetail.text = message;
        }

        if (txtLastAlarm != null)
        {
            txtLastAlarm.text = message;
        }

        if (actionLogger != null)
        {
            actionLogger.LogAction("Alarm Selected : " + message);
        }
    }

    //현재 알람 개수 반환
    public int GetAlarmCount()
    {
        if (alarmListContent == null)
        {
            return 0;
        }

        return alarmListContent.childCount;
    }

    //마지막 알람 텍스트 반환
    public string GetLastAlarmText()
    {
        if (txtLastAlarm == null)
        {
            return "No alarm";
        }

        if (string.IsNullOrEmpty(txtLastAlarm.text))
        {
            return "No alarm";
        }

        return txtLastAlarm.text;
    }
}
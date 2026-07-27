using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yeop;

//Log 페이지 자동 기록 컨트롤러
//전체 로그 데이터는 메모리에 유지
//UI 최근 로그만 표시
//JSON 자동 저장 / 불러오기
//CSV Export 지원
//Category 필터
//필터 버튼 선택 강조
//로그 개수 제한 적용
[Singleton(Name = "PageLogController", Persistent = false, Automatic = false)]
public class scr_PageLogController : MonoBehaviour
{
    public static scr_PageLogController instance
    {
        get { return Singleton<scr_PageLogController>.instance; }
    }

    [Header("UI References")]
    public Text txtLogSummary;
    public Text txtLogDetail;
    public Text txtLogCount;

    [Header("List References")]
    public Transform logListContent;
    public GameObject logRowPrefab;

    [Header("Filter Buttons")]
    public Button btnFilterAll;
    public Button btnFilterLog;
    public Button btnFilterAlarm;
    public Button btnFilterStatus;
    public Button btnFilterOrder;

    [Header("Filter Button Colors")]
    public Color filterNormalColor = new Color(0.28f, 0.40f, 0.52f);
    public Color filterSelectedColor = new Color(0.12f, 0.38f, 0.72f);
    public Color filterNormalTextColor = Color.white;
    public Color filterSelectedTextColor = Color.white;

    [Header("Options")]
    public int maxVisibleLogCount = 15;     //화면에 보이는 최대 로그 수
    public int maxStoredLogCount = 30;      //메모리 / 저장 파일 유지 최대 로그 수

    private scr_LogRowUI selectedRow;
    private readonly List<LogData> logDataList = new List<LogData>();
    private string currentFilter = "All";

    private void Awake()
    {
        Singleton<scr_PageLogController>.Awake(this);
    }

    private void Start()
    {
        Debug.Log("Persistent Path : " + Application.persistentDataPath);

        LoadLogsFromJson();
        ApplyFilterButtonState();
        RefreshLogList();
    }

    private void OnApplicationQuit()
    {
        SaveLogsToJson();
    }

    private void OnDestroy()
    {
        SaveLogsToJson();
        Singleton<scr_PageLogController>.OnDestroy(this);
    }

    //외부 스크립트에서 호출하는 Log 기록 함수
    //전체 로그는 logDataList 에 유지
    //UI에는 최근 maxVisibleLogCount 개만 표시
    //오래된 로그는 자동 삭제
    public void AddLogRecord(string category, string title, string detail)
    {
        if (string.IsNullOrEmpty(category))
        {
            category = "Log";
        }

        string timeText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string messageText = title;

        string summaryText =
            "[ " + category + " ] " + title + "\n" +
            detail;

        string detailText =
            "[Time] " + timeText + "\n" +
            "[Category] " + category + "\n" +
            "[Title] " + title + "\n\n" +
            detail;

        LogData newData = new LogData
        {
            timeText = timeText,
            category = category,
            title = title,
            message = messageText,
            summary = summaryText,
            detail = detailText
        };

        logDataList.Add(newData);

        TrimOldLogs();       //오래된 로그 자동 정리

        //현재 필터에 포함되는 카테고리면 UI 상단에 즉시 반영
        if (currentFilter == "All" || currentFilter == category)
        {
            AddVisibleRowAtTop(newData);
        }

        UpdateLogCount();

        SaveLogsToJson();    //의미 있는 변화만 로그로 남김
    }

    //오래된 로그 자동 정리
    //최대 보관 수 초과 시 가장 오래된 로그부터 삭제
    private void TrimOldLogs()
    {
        while (logDataList.Count > maxStoredLogCount)
        {
            logDataList.RemoveAt(0);
        }
    }

    //현재 필터 기준으로 최근 로그만 다시 그린다.
    private void RefreshLogList()
    {
        if (logListContent == null || logRowPrefab == null)
        {
            return;
        }

        ClearVisibleRows();

        selectedRow = null;
        int visibleCount = 0;

        for (int i = logDataList.Count - 1; i >= 0; i--)
        {
            LogData data = logDataList[i];

            if (!IsMatchedFilter(data.category))
            {
                continue;
            }

            CreateLogRow(data, false);
            visibleCount++;

            if (visibleCount >= maxVisibleLogCount)
            {
                break;
            }
        }

        if (visibleCount == 0)
        {
            SetEmptyTexts();
        }

        UpdateLogCount();
    }

    //새 로그를 UI 맨 위에 1개만 추가
    //최근 표시 개수 초과 시 마지막 Row 제거
    private void AddVisibleRowAtTop(LogData data)
    {
        if (logListContent == null || logRowPrefab == null)
        {
            return;
        }

        scr_LogRowUI newRow = CreateLogRow(data, true);

        if (newRow != null)
        {
            SelectLogRow(newRow, data.summary, data.detail);
        }

        while (logListContent.childCount > maxVisibleLogCount)
        {
            Transform lastChild = logListContent.GetChild(logListContent.childCount - 1);
            Destroy(lastChild.gameObject);
        }
    }

    //Log Row 1개 생성
    private scr_LogRowUI CreateLogRow(LogData data, bool insertAsFirst)
    {
        GameObject rowObject = Instantiate(logRowPrefab, logListContent);

        if (insertAsFirst)
        {
            rowObject.transform.SetAsFirstSibling();
        }

        scr_LogRowUI rowUI = rowObject.GetComponent<scr_LogRowUI>();
        if (rowUI != null)
        {
            rowUI.Setup(
                data.timeText,
                data.category,
                data.message,
                data.summary,
                data.detail,
                this
            );
        }

        return rowUI;
    }

    //현재 필터에 맞는지 확인
    private bool IsMatchedFilter(string category)
    {
        return currentFilter == "All" || currentFilter == category;
    }

    //화면에 표시 중인 Row만 제거
    private void ClearVisibleRows()
    {
        if (logListContent == null)
        {
            return;
        }

        for (int i = logListContent.childCount - 1; i >= 0; i--)
        {
            Destroy(logListContent.GetChild(i).gameObject);
        }
    }

    //현재 필터 기준 총 로그 수 계산
    private int GetFilteredLogCount()
    {
        if (currentFilter == "All")
        {
            return logDataList.Count;
        }

        int count = 0;

        for (int i = 0; i < logDataList.Count; i++)
        {
            if (logDataList[i].category == currentFilter)
            {
                count++;
            }
        }

        return count;
    }

    //Count 텍스트 갱신
    private void UpdateLogCount()
    {
        if (txtLogCount == null)
        {
            return;
        }

        int visibleCount = logListContent != null ? logListContent.childCount : 0;
        int filteredTotalCount = GetFilteredLogCount();

        txtLogCount.text = "LOG Count : " + visibleCount + " / " + filteredTotalCount;
    }

    //Log Row 선택 처리
    public void SelectLogRow(scr_LogRowUI row, string summary, string detail)
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

        if (txtLogSummary != null)
        {
            txtLogSummary.text = summary;
        }

        if (txtLogDetail != null)
        {
            txtLogDetail.text = detail;
        }
    }

    //외부 버튼에서 필터 변경
    public void SetFilter(string category)
    {
        currentFilter = category;
        selectedRow = null;

        ApplyFilterButtonState();
        RefreshLogList();
    }

    public void ShowAll()
    {
        SetFilter("All");
    }

    public void ShowLog()
    {
        SetFilter("Log");
    }

    public void ShowAlarm()
    {
        SetFilter("Alarm");
    }

    public void ShowStatus()
    {
        SetFilter("Status");
    }

    public void ShowOrder()
    {
        SetFilter("Order");
    }

    //필터 버튼 선택 상태 적용
    private void ApplyFilterButtonState()
    {
        SetFilterButtonVisual(btnFilterAll, currentFilter == "All");
        SetFilterButtonVisual(btnFilterLog, currentFilter == "Log");
        SetFilterButtonVisual(btnFilterAlarm, currentFilter == "Alarm");
        SetFilterButtonVisual(btnFilterStatus, currentFilter == "Status");
        SetFilterButtonVisual(btnFilterOrder, currentFilter == "Order");
    }

    //버튼 배경색 / 텍스트색 적용
    private void SetFilterButtonVisual(Button targetButton, bool isSelected)
    {
        if (targetButton == null)
        {
            return;
        }

        Image buttonImage = targetButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = isSelected ? filterSelectedColor : filterNormalColor;
        }

        Text buttonText = targetButton.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.color = isSelected ? filterSelectedTextColor : filterNormalTextColor;
        }
    }

    //Summary / Detail 기본 문구 설정
    private void SetEmptyTexts()
    {
        if (txtLogSummary != null)
        {
            txtLogSummary.text = "No Log selected";
        }

        if (txtLogDetail != null)
        {
            txtLogDetail.text = "Select a log row to view detail.";
        }
    }

    //전체 로그 삭제
    public void ClearAllLogs()
    {
        logDataList.Clear();
        selectedRow = null;

        SaveLogsToJson();
        RefreshLogList();
        ApplyFilterButtonState();
        SetEmptyTexts();
    }

    //JSON 파일로 전체 로그 저장
    //저장 전 오래된 로그 자동 정리
    public void SaveLogsToJson()
    {
        TrimOldLogs();
        scr_LogFileSave.SaveLogs(logDataList);
    }

    //JSON 파일에서 전체 로그 불러오기
    public void LoadLogsFromJson()
    {
        List<LogData> loadedLogs = scr_LogFileSave.LoadLogs();

        logDataList.Clear();

        if (loadedLogs != null)
        {
            logDataList.AddRange(loadedLogs);
        }

        TrimOldLogs();
    }

    //CSV 파일로 전체 로그 Export
    public void ExportLogsToCsv()
    {
        TrimOldLogs();
        scr_LogFileSave.ExportLogsToCsv(logDataList);
    }

    //외부에서 전체 로그 목록 참조 시 복사본 반환
    public List<LogData> GetAllLogs()
    {
        return new List<LogData>(logDataList);
    }
}
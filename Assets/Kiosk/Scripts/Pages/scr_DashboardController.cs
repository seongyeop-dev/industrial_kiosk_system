using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Dashboard 페이지 요약 정보 표시
//Order / Status / Alarm / Log 데이터를 읽어와 요약 표시
//상세 Dashboard + Home 메인 카드 둘 다 갱신
public class scr_DashboardController : MonoBehaviour
{
    [Header("References")]
    public scr_OrderController orderController;
    public scr_PageOrderController pageOrderController;
    public scr_StatusController statusController;
    public scr_AlarmController alarmController;
    public scr_UIActionLogger actionLogger;

    [Header("Dashboard - Order Summary UI")]
    public Text txtTotalOrders;
    public Text txtReadyCount;
    public Text txtRunningCount;
    public Text txtErrorCount;

    [Header("Dashboard - System Status UI")]
    public Text txtCurrentStatus;
    public Text txtLastAction;
    public Text txtCurrentTime;

    [Header("Dashboard - Alarm Summary UI")]
    public Text txtAlarmCount;
    public Text txtLastAlarm;

    [Header("Dashboard - Log Summary UI")]
    public Text txtTotalLogs;
    public Text txtLatestLog;

    [Header("Home Main Card UI")]
    public Text txtHomeTotalOrders;
    public Text txtHomeRunning;
    public Text txtHomeCompleted;

    public Text txtHomeSystemStatus;

    public Text txtHomeReady;
    public Text txtHomeWait;
    public Text txtHomeDone;

    public Text txtHomeLatestLog;
    public Text txtHomeLastAction;
    public Text txtHomeAlarm;

    public Text txtHomeDate;
    public Text txtHomeTime;

    public Text txtHomeOrder;   //Home Order 카드 텍스트 추가

    [Header("Refresh Options")]
    public float refreshInterval = 1.0f;

    private void Start()
    {
        RefreshDashboard();
        InvokeRepeating(nameof(RefreshDashboard), refreshInterval, refreshInterval);
    }

    private void OnEnable()
    {
        RefreshDashboard();
        CancelInvoke(nameof(RefreshDashboard));
        InvokeRepeating(nameof(RefreshDashboard), refreshInterval, refreshInterval);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(RefreshDashboard));
    }

    public void RefreshDashboard()
    {
        UpdateOrderSummary();
        UpdateSystemStatus();
        UpdateAlarmSummary();
        UpdateLogSummary();
        UpdateCurrentTime();
        UpdateHomeOrderCard();
    }

    private void UpdateOrderSummary()
    {
        List<scr_OrderController.OrderData> orders = new List<scr_OrderController.OrderData>();

        if (pageOrderController != null && pageOrderController.orderController != null)
        {
            orders = pageOrderController.orderController.GetAllOrders();
        }
        else if (orderController != null)
        {
            orders = orderController.GetAllOrders();
        }
        else
        {
            SetOrderSummaryTexts(0, 0, 0, 0);
            UpdateHomeOrderSummary(0, 0, 0, 0);
            return;
        }

        int total = orders.Count;
        int ready = 0;
        int running = 0;
        int error = 0;

        for (int i = 0; i < orders.Count; i++)
        {
            string status = orders[i].status;

            if (string.Equals(status, "READY", System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Ready", System.StringComparison.OrdinalIgnoreCase))
            {
                ready++;
            }
            else if (string.Equals(status, "RUNNING", System.StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(status, "Running", System.StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(status, "WAIT", System.StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(status, "Wait", System.StringComparison.OrdinalIgnoreCase))
            {
                running++;
            }
            else if (string.Equals(status, "ERROR", System.StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(status, "Error", System.StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(status, "DONE", System.StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(status, "Done", System.StringComparison.OrdinalIgnoreCase))
            {
                error++;
            }
        }

        SetOrderSummaryTexts(total, ready, running, error);
        UpdateHomeOrderSummary(total, ready, running, error);
    }

    private void SetOrderSummaryTexts(int total, int ready, int running, int error)
    {
        if (txtTotalOrders != null) txtTotalOrders.text = "Total : " + total;
        if (txtReadyCount != null) txtReadyCount.text = "Ready : " + ready;
        if (txtRunningCount != null) txtRunningCount.text = "Running : " + running;
        if (txtErrorCount != null) txtErrorCount.text = "Error : " + error;
    }

    private void UpdateHomeOrderSummary(int total, int ready, int running, int error)
    {
        if (txtHomeTotalOrders != null) txtHomeTotalOrders.text = "Total Orders: " + total;
        if (txtHomeRunning != null) txtHomeRunning.text = "Running: " + running;
        if (txtHomeCompleted != null) txtHomeCompleted.text = "Completed: " + error;

        if (txtHomeReady != null) txtHomeReady.text = "Ready: " + ready;
        if (txtHomeWait != null) txtHomeWait.text = "Wait: " + running;
        if (txtHomeDone != null) txtHomeDone.text = "Done: " + error;
    }

    private void UpdateSystemStatus()
    {
        string currentStatusText = "READY";

        if (statusController != null)
        {
            currentStatusText = statusController.GetCurrentStatus();
        }

        if (txtCurrentStatus != null) txtCurrentStatus.text = currentStatusText;
        if (txtHomeSystemStatus != null) txtHomeSystemStatus.text = "SYSTEM " + currentStatusText;

        string lastActionText = "No Action";

        if (actionLogger != null)
        {
            string action = actionLogger.GetLastAction();
            if (!string.IsNullOrEmpty(action))
            {
                lastActionText = action;
            }
        }

        if (txtLastAction != null) txtLastAction.text = lastActionText;
        if (txtHomeLastAction != null) txtHomeLastAction.text = lastActionText;
    }

    private void UpdateAlarmSummary()
    {
        int alarmCountValue = 0;
        string lastAlarmText = "No Active Alarm";

        if (alarmController != null)
        {
            alarmCountValue = alarmController.GetAlarmCount();
            lastAlarmText = alarmController.GetLastAlarmText();
        }

        if (txtAlarmCount != null) txtAlarmCount.text = "Alarm Count : " + alarmCountValue;
        if (txtLastAlarm != null) txtLastAlarm.text = lastAlarmText;
        if (txtHomeAlarm != null) txtHomeAlarm.text = lastAlarmText;
    }

    private void UpdateLogSummary()
    {
        int logCount = 0;
        string latestLogTitle = "No recent logs";

        if (scr_PageLogController.instance != null)
        {
            List<LogData> logs = scr_PageLogController.instance.GetAllLogs();
            logCount = logs.Count;

            if (logs.Count > 0)
            {
                latestLogTitle = logs[logs.Count - 1].title;
            }
        }

        if (txtTotalLogs != null) txtTotalLogs.text = "Logs : " + logCount;
        if (txtLatestLog != null) txtLatestLog.text = latestLogTitle;
        if (txtHomeLatestLog != null) txtHomeLatestLog.text = latestLogTitle;
    }

    private void UpdateCurrentTime()
    {
        if (txtCurrentTime != null)
        {
            txtCurrentTime.text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        if (txtHomeDate != null)
        {
            txtHomeDate.text = System.DateTime.Now.ToString("yyyy-MM-dd");
        }

        if (txtHomeTime != null)
        {
            txtHomeTime.text = System.DateTime.Now.ToString("HH:mm");
        }
    }

    private void UpdateHomeOrderCard()
    {
        if (txtHomeOrder == null)
        {
            return;
        }

        List<scr_OrderController.OrderData> orders = new List<scr_OrderController.OrderData>();

        if (pageOrderController != null && pageOrderController.orderController != null)
        {
            orders = pageOrderController.orderController.GetAllOrders();
        }
        else if (orderController != null)
        {
            orders = orderController.GetAllOrders();
        }

        if (orders == null || orders.Count == 0)
        {
            txtHomeOrder.text = "No order selected";
            return;
        }

        scr_OrderController.OrderData lastOrder = orders[orders.Count - 1];

        string orderNo = lastOrder.orderNo;
        string product = lastOrder.productName;

        if (string.IsNullOrEmpty(orderNo) && string.IsNullOrEmpty(product))
        {
            txtHomeOrder.text = "No order selected";
            return;
        }

        if (string.IsNullOrEmpty(product))
        {
            txtHomeOrder.text = orderNo;
            return;
        }

        txtHomeOrder.text = orderNo + " - " + product;
    }
}
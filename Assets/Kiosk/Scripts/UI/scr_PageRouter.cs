using UnityEngine;
using UnityEngine.UI;
using System.Text;

//전체 페이지 전환 관리
//Main_Home 시작 화면 관리
//Dashboard / Order / Status / Alarm / System Log 상세 페이지 전환
//Home 모드 / Detail 모드 전환
//현재 페이지에 맞는 Side Menu 버튼 하이라이트 처리
public class scr_PageRouter : MonoBehaviour
{
    [Header("Page Panels")]
    public GameObject pageHome;
    public GameObject pageDashboard;
    public GameObject pageOrder;
    public GameObject pageStatus;
    public GameObject pageAlarm;
    public GameObject pageSystemLog;

    [Header("Main UI Containers")]
    public GameObject mainHome;
    public GameObject topBar;
    public GameObject sideMenu;
    public GameObject bottomBar;
    public GameObject mainFrame;

    [Header("Side Menu Buttons")]
    public Button btnDashboard;
    public Button btnOrder;
    public Button btnStatus;
    public Button btnAlarm;
    public Button btnSystemLog;

    [Header("Top Bar Button")]
    public Button btnHome;

    [Header("Home Page Buttons")]
    public Button btnHomeDashboard;
    public Button btnHomeOrder;
    public Button btnHomeStatus;
    public Button btnHomeAlarm;
    public Button btnHomeSystemLog;

    [Header("Button Highlight Colors")]
    public Color normalButtonColor = new Color(0.27f, 0.47f, 0.62f);
    public Color selectedButtonColor = new Color(0.08f, 0.26f, 0.49f);

    [Header("Bottom Bar Text")]
    public Text txtLastAction;

    [Header("Start Page")]
    public GameObject defaultPage;

    [Header("Logger")]
    public scr_UIActionLogger actionLogger;

    private GameObject currentPage;

    private void Start()
    {
        BindButtons();
        ShowDefaultPage();
    }

    //각 버튼 클릭 이벤트 연결
    private void BindButtons()
    {
        if (btnDashboard != null)
        {
            btnDashboard.onClick.RemoveAllListeners();
            btnDashboard.onClick.AddListener(() => ShowPage(pageDashboard));
        }

        if (btnOrder != null)
        {
            btnOrder.onClick.RemoveAllListeners();
            btnOrder.onClick.AddListener(() => ShowPage(pageOrder));
        }

        if (btnStatus != null)
        {
            btnStatus.onClick.RemoveAllListeners();
            btnStatus.onClick.AddListener(() => ShowPage(pageStatus));
        }

        if (btnAlarm != null)
        {
            btnAlarm.onClick.RemoveAllListeners();
            btnAlarm.onClick.AddListener(() => ShowPage(pageAlarm));
        }

        if (btnSystemLog != null)
        {
            btnSystemLog.onClick.RemoveAllListeners();
            btnSystemLog.onClick.AddListener(() => ShowPage(pageSystemLog));
        }

        if (btnHome != null)
        {
            btnHome.onClick.RemoveAllListeners();
            btnHome.onClick.AddListener(GoToHome);
        }

        if (btnHomeDashboard != null)
        {
            btnHomeDashboard.onClick.RemoveAllListeners();
            btnHomeDashboard.onClick.AddListener(() => ShowPage(pageDashboard));
        }

        if (btnHomeOrder != null)
        {
            btnHomeOrder.onClick.RemoveAllListeners();
            btnHomeOrder.onClick.AddListener(() => ShowPage(pageOrder));
        }

        if (btnHomeStatus != null)
        {
            btnHomeStatus.onClick.RemoveAllListeners();
            btnHomeStatus.onClick.AddListener(() => ShowPage(pageStatus));
        }

        if (btnHomeAlarm != null)
        {
            btnHomeAlarm.onClick.RemoveAllListeners();
            btnHomeAlarm.onClick.AddListener(() => ShowPage(pageAlarm));
        }

        if (btnHomeSystemLog != null)
        {
            btnHomeSystemLog.onClick.RemoveAllListeners();
            btnHomeSystemLog.onClick.AddListener(() => ShowPage(pageSystemLog));
        }
    }

    //시작 시 기본 화면 표시
    private void ShowDefaultPage()
    {
        ShowHomeMode();
        currentPage = pageHome;

        UpdateBottomBarTexts("Go to Home");

        if (actionLogger != null)
        {
            actionLogger.LogAction("Go to Home");
        }
    }

    //지정한 페이지 표시
    public void ShowPage(GameObject targetPage)
    {
        if (targetPage == null)
        {
            Debug.LogWarning("[scr_PageRouter] targetPage is null.");
            return;
        }

        if (targetPage == pageHome)
        {
            ShowHomeMode();
            currentPage = pageHome;

            UpdateBottomBarTexts("Go to Home");

            if (actionLogger != null)
            {
                actionLogger.LogAction("Go to Home");
            }

            Debug.Log("[scr_PageRouter] Current Mode : Home");
            return;
        }

        string pageName = GetPageDisplayName(targetPage);

        ShowDetailMode(targetPage);

        if (actionLogger != null)
        {
            actionLogger.LogAction("Go to " + pageName);
        }

        UpdateBottomBarTexts("Go to " + pageName);

        Debug.Log("[scr_PageRouter] Current Page : " + targetPage.name);
    }

    //Home 화면 모드 표시
    private void ShowHomeMode()
    {
        if (mainHome != null)
            mainHome.SetActive(true);

        if (topBar != null)
            topBar.SetActive(false);

        if (sideMenu != null)
            sideMenu.SetActive(false);

        if (bottomBar != null)
            bottomBar.SetActive(false);

        if (mainFrame != null)
            mainFrame.SetActive(false);

        HideAllPages();
        UpdateSideMenuHighlight();
    }

    //상세 페이지 모드 표시
    private void ShowDetailMode(GameObject targetPage)
    {
        if (targetPage == null)
            return;

        if (mainHome != null)
            mainHome.SetActive(false);

        if (topBar != null)
            topBar.SetActive(true);

        if (sideMenu != null)
            sideMenu.SetActive(true);

        if (bottomBar != null)
            bottomBar.SetActive(true);

        if (mainFrame != null)
            mainFrame.SetActive(true);

        HideAllPages();

        targetPage.SetActive(true);
        currentPage = targetPage;

        UpdateSideMenuHighlight();
    }

    //모든 상세 페이지 숨김
    private void HideAllPages()
    {
        if (pageDashboard != null)
            pageDashboard.SetActive(false);

        if (pageOrder != null)
            pageOrder.SetActive(false);

        if (pageStatus != null)
            pageStatus.SetActive(false);

        if (pageAlarm != null)
            pageAlarm.SetActive(false);

        if (pageSystemLog != null)
            pageSystemLog.SetActive(false);
    }

    //현재 페이지 기준으로 Side Menu 버튼 색상 갱신
    private void UpdateSideMenuHighlight()
    {
        SetButtonColor(btnDashboard, normalButtonColor);
        SetButtonColor(btnOrder, normalButtonColor);
        SetButtonColor(btnStatus, normalButtonColor);
        SetButtonColor(btnAlarm, normalButtonColor);
        SetButtonColor(btnSystemLog, normalButtonColor);

        if (currentPage == pageDashboard)
            SetButtonColor(btnDashboard, selectedButtonColor);
        else if (currentPage == pageOrder)
            SetButtonColor(btnOrder, selectedButtonColor);
        else if (currentPage == pageStatus)
            SetButtonColor(btnStatus, selectedButtonColor);
        else if (currentPage == pageAlarm)
            SetButtonColor(btnAlarm, selectedButtonColor);
        else if (currentPage == pageSystemLog)
            SetButtonColor(btnSystemLog, selectedButtonColor);
    }

    //Bottom Bar 텍스트 갱신
    private void UpdateBottomBarTexts(string actionText)
    {
        if (txtLastAction != null)
            txtLastAction.text = actionText;
    }

    //현재 페이지 기준 경로 문자열 생성
    private string BuildPathText()
    {
        StringBuilder pathBuilder = new StringBuilder();
        pathBuilder.Append("HOME");

        if (currentPage == pageDashboard)
            pathBuilder.Append(" > DASHBOARD");
        else if (currentPage == pageOrder)
            pathBuilder.Append(" > ORDER");
        else if (currentPage == pageStatus)
            pathBuilder.Append(" > STATUS");
        else if (currentPage == pageAlarm)
            pathBuilder.Append(" > ALARM");
        else if (currentPage == pageSystemLog)
            pathBuilder.Append(" > SYSTEM LOG");

        return pathBuilder.ToString();
    }

    //페이지 오브젝트를 사용자 표시 이름으로 변환
    private string GetPageDisplayName(GameObject targetPage)
    {
        if (targetPage == pageDashboard)
            return "Dashboard";
        if (targetPage == pageOrder)
            return "Order";
        if (targetPage == pageStatus)
            return "Status";
        if (targetPage == pageAlarm)
            return "Alarm";
        if (targetPage == pageSystemLog)
            return "System Log";
        if (targetPage == pageHome)
            return "Home";

        return targetPage.name;
    }

    //버튼 배경 색상 변경
    private void SetButtonColor(Button targetButton, Color targetColor)
    {
        if (targetButton == null)
            return;

        Image buttonImage = targetButton.GetComponent<Image>();
        if (buttonImage != null)
            buttonImage.color = targetColor;
    }

    //현재 활성 상세 페이지 반환
    public GameObject GetCurrentPage()
    {
        return currentPage;
    }

    //Home 화면으로 이동
    public void GoToHome()
    {
        ShowHomeMode();
        currentPage = pageHome;

        UpdateBottomBarTexts("Go to Home");

        if (actionLogger != null)
        {
            actionLogger.LogAction("Go to Home");
        }

        Debug.Log("[scr_PageRouter] Current Mode : Home");
    }

    public void GoToDashboard()
    {
        ShowPage(pageDashboard);
    }

    public void GoToOrder()
    {
        ShowPage(pageOrder);
    }

    public void GoToStatus()
    {
        ShowPage(pageStatus);
    }

    public void GoToAlarm()
    {
        ShowPage(pageAlarm);
    }

    public void GoToSystemLog()
    {
        ShowPage(pageSystemLog);
    }
}
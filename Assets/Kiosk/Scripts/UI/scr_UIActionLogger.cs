using UnityEngine;
using UnityEngine.UI;

//하단 LastAction 텍스트 공통 갱신
//기존 BottomBar 표시 유지
//Dashboard 등 다른 스크립트 마지막 액션 문자열 조회
public class scr_UIActionLogger : MonoBehaviour
{
    [Header("UI References")]
    public Text txtLastAction;

    //마지막 동작 메시지 저장
    private string lastActionMessage = "";      

    // 마지막 동작 문구 갱신
    public void LogAction(string message)       
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        lastActionMessage = message;

        if (txtLastAction != null)
        {
            txtLastAction.text = message;
        }
    }
    
    // 마지막 동작 문자열 반환
    public string GetLastAction()
    {
        return lastActionMessage;
    }
}
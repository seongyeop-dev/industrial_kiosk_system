using UnityEngine;
using UnityEngine.UI;

//시간 표시 전용
//매 프레임 갱신 대신 1초 간격으로 갱신
public class scr_TimeDisplay : MonoBehaviour
{
    public Text txtTime;
    public float refreshInterval = 1.0f;

    private void Start()
    {
        RefreshTime();
        InvokeRepeating(nameof(RefreshTime), refreshInterval, refreshInterval);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(RefreshTime));
    }

    //현재시간 갱신
    private void RefreshTime()
    {
        if (txtTime != null)
        {
            txtTime.text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
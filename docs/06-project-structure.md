# 06. 프로젝트 구조

```text
Assets/Kiosk
├─ Docs
│  └─ 프로젝트 참고 문서
├─ Prefab
│  ├─ Background
│  ├─ Buttons
│  ├─ Card
│  ├─ Common
│  ├─ Panels
│  └─ Txt
├─ Scenes
│  └─ Kiosk_Industrial
└─ Scripts
   ├─ Data
   │  ├─ NST_CSV.cs
   │  ├─ NST_Json.cs
   │  ├─ scr_LogFileSave.cs
   │  ├─ Singleton.cs
   │  └─ SingletonAttribute.cs
   ├─ Pages
   │  ├─ scr_AlarmController.cs
   │  ├─ scr_DashboardController.cs
   │  ├─ scr_OrderController.cs
   │  ├─ scr_PageLogController.cs
   │  ├─ scr_PageOrderController.cs
   │  └─ scr_StatusController.cs
   └─ UI
      ├─ scr_AlarmRowUI.cs
      ├─ scr_BottomBarToggle.cs
      ├─ scr_LogRowUI.cs
      ├─ scr_OrderListItemUI.cs
      ├─ scr_PageRouter.cs
      ├─ scr_ReceiptCapture.cs
      ├─ scr_TimeDisplay.cs
      └─ scr_UIActionLogger.cs
```

## Prefab 구성

- `PF_BG_Base`
- `PF_BG_Panel`
- `PF_ActionButton_Large`
- `PF_ActionButton_Small`
- `PF_MenuButton`
- `Card_Template`
- `PF_Button_Main`
- `PF_Input_Row`
- `PF_OrderRow`
- `PF_Panel_Base`
- `PF_ScrollList_Base`
- `PF_ContentPanel`
- `PF_PageHeader`
- `PF_AlarmRow`
- `PF_LogRow`
- `PF_Receipt`

## 정리 원칙

- 반복 UI만 Prefab화
- 전체 UI_Root 및 Page 단위 프리팹화는 하지 않음
- 빌드 직전 대규모 Hierarchy 변경은 하지 않음
- 안정적으로 동작하는 Scene 구조를 최종 기준으로 유지

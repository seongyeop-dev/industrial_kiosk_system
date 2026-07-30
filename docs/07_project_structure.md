# 07. Project Structure

## 저장소 구조

```text
industrial-kiosk-system
├─ Assets
│  └─ Kiosk
│     ├─ Docs
│     ├─ Prefab
│     ├─ Scenes
│     └─ Scripts
├─ Packages
│  ├─ manifest.json
│  └─ packages-lock.json
├─ ProjectSettings
├─ docs
│  ├─ README.md
│  ├─ 01_overview.md
│  ├─ 02_architecture.md
│  ├─ 03_features.md
│  ├─ 04_data_flow.md
│  ├─ 05_validation.md
│  ├─ 06_project_scope.md
│  ├─ 07_project_structure.md
│  └─ images
└─ README.md
```

## Unity 프로젝트 구조

```text
Assets/Kiosk
├─ Docs
│  └─ 프로젝트 참고 문서
├─ Prefab
│  ├─ Background
│  ├─ Canvas_Main
│  ├─ Common
│  ├─ MainFrame
│  ├─ Main_UI
│  ├─ Page
│  └─ UI_Root
├─ Scenes
│  └─ Kiosk_Industrial.unity
└─ Scripts
   ├─ Data
   ├─ Pages
   └─ UI
```

## 주요 Prefab

| 폴더 | 주요 Prefab | 역할 |
|:---|:---|:---|
| `Background` | `Background.prefab` | 전체 배경 |
| `Canvas_Main` | `Canvas_Main`, `OrderController`, `PageManager` | Scene 상위 UI와 Controller |
| `Common` | `PF_OrderRow`, `PF_AlarmRow`, `PF_LogRow`, `PF_Receipt` | 반복 Row와 공통 UI |
| `MainFrame` | `MainFrame.prefab` | Detail 화면 표시 영역 |
| `Main_UI` | `TopBar`, `SideMenu`, `Home`, `BottomBar`, `Main_UI` | 공통 네비게이션과 Home |
| `Page` | `Page_Dashboard`, `Page_Order`, `Page_Status`, `Page_Alarm`, `Page_SystemLog` | 기능별 Detail 화면 |
| `UI_Root` | `UI_Root.prefab` | 전체 UI Root |

## Data Scripts

| 파일 | 역할 |
|:---|:---|
| `NST_CSV.cs` | Dictionary 목록을 CSV로 저장하거나 불러오는 공통 기능 |
| `NST_Json.cs` | Newtonsoft Json 기반 JSON 저장·복원 |
| `scr_LogFileSave.cs` | `KioskData/Logs` 경로와 로그 파일 관리 |
| `Singleton.cs` | Singleton 인스턴스 관리 |
| `SingletonAttribute.cs` | Singleton 생성·유지 옵션 정의 |

## Page Scripts

| 파일 | 역할 |
|:---|:---|
| `scr_AlarmController.cs` | 테스트 알람 생성, 선택, 전체 삭제 |
| `scr_DashboardController.cs` | 주문·상태·알람·로그 요약 표시 |
| `scr_OrderController.cs` | 주문 데이터, TXT 저장·복원, 검색과 번호 생성 |
| `scr_PageLogController.cs` | 로그 Row, 필터, JSON 저장과 CSV Export |
| `scr_PageOrderController.cs` | 주문 입력, 검증, CRUD와 선택 상태 |
| `scr_StatusController.cs` | 시스템 상태 변경과 관련 UI 갱신 |

## UI Scripts

| 파일 | 역할 |
|:---|:---|
| `scr_AlarmRowUI.cs` | 알람 Row 표시와 선택 이벤트 |
| `scr_BottomBarToggle.cs` | Bottom Bar 상태 표시 |
| `scr_LogRowUI.cs` | 로그 Row 표시 |
| `scr_OrderListItemUI.cs` | 주문 Row 표시와 선택 이벤트 |
| `scr_PageRouter.cs` | Home·Detail 화면과 Side Menu 전환 |
| `scr_ReceiptCapture.cs` | 영수증 UI 반영, 화면 캡처와 PNG 저장 |
| `scr_TimeDisplay.cs` | 날짜·시간 1초 주기 갱신 |
| `scr_UIActionLogger.cs` | UI 동작 로그 연결 |

## 주요 패키지

| 패키지 | 버전 | 용도 |
|:---|---:|:---|
| Unity uGUI | 2.0.0 | UI 구성 |
| Newtonsoft Json | 3.2.2 | 로그 JSON 직렬화 |

---

[문서 목차](README.md) · [프로젝트 README](../README.md)

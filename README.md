# Industrial Kiosk System

Unity 기반 산업용 키오스크 UI 프로젝트입니다.  
주문 관리, 시스템 상태 표시, 알람, 로그 기록, 파일 저장 및 영수증 이미지 출력 기능을 하나의 키오스크 화면에 통합했습니다.

## 프로젝트 개요

- **프로젝트명**: Industrial Kiosk System
- **개발 환경**: Unity / C#
- **목표**: 산업 현장에서 사용할 수 있는 주문·상태·알람·로그 통합 UI 구현
- **핵심 방향**: UI와 데이터 처리 분리, 파일 기반 데이터 영속성, 반복 UI 프리팹화, 제출 환경에서도 안정적으로 동작하는 경량 구조

## 주요 기능

- 주문 추가 / 수정 / 삭제
- 주문번호 자동 생성 및 중복 방지
- 제품명 / 수량 / 날짜 입력 검증
- 주문 상태 관리
- 주문 데이터 TXT 저장 및 자동 불러오기
- 주문서 TXT 개별 출력
- 영수증 PNG 저장
- 시스템 상태 변경 및 Dashboard 반영
- 알람 생성 / 선택 / 전체 삭제
- 의미 있는 데이터 변경 중심의 시스템 로그
- 로그 카테고리 필터
- 로그 JSON 저장 및 CSV Export
- Home / Detail 모드 기반 페이지 전환
- Dashboard / Home 카드 실시간 요약 표시

## 화면 구성

- Home
- Dashboard
- Order
- Status
- Alarm
- System Log

공통 UI는 `TopBar`, `SideMenu`, `MainFrame`, `BottomBar` 구조로 구성했습니다.

## 아키텍처

```text
UI Layer
├─ scr_PageOrderController
├─ scr_PageLogController
└─ scr_PageRouter

Controller Layer
├─ scr_OrderController
├─ scr_DashboardController
├─ scr_StatusController
├─ scr_AlarmController
└─ scr_ReceiptCapture

Data / File Layer
├─ OrderData
├─ LogData
├─ scr_LogFileSave
├─ NST_Json
└─ NST_CSV

UI Component Layer
├─ scr_OrderListItemUI
├─ scr_LogRowUI
├─ scr_AlarmRowUI
├─ scr_UIActionLogger
├─ scr_BottomBarToggle
└─ scr_TimeDisplay
```

## 데이터 저장 구조

저장 기준 경로는 `Application.persistentDataPath`입니다.

```text
KioskData
├─ Orders
│  ├─ orders.txt
│  ├─ OrderSheets
│  │  └─ OrderSheet_{OrderNo}.txt
│  └─ Receipts
│     └─ Receipt_{OrderNo}.png
└─ Logs
   ├─ system_log.json
   └─ system_log_export_YYYYMMDD_HHMMSS.csv
```

## 성능 및 안정화

- 페이지 이동 및 단순 클릭 로그 제거
- 주문 / 상태 / 알람처럼 실제 데이터가 변경된 경우만 기록
- Log UI 표시 개수 제한
- 메모리 및 저장 로그 개수 제한
- 새 로그 1건만 UI 상단에 추가하는 방식 적용
- 시간 표시를 매 프레임 갱신하지 않고 1초 주기로 변경
- 영수증 PNG 저장 시 임시 Texture 메모리 해제
- 반복 Row UI는 Prefab 기반 생성

## 테스트 결과

| 구분 | 테스트 항목 | 결과 |
|---|---|---|
| Order | 추가 / 수정 / 삭제 | PASS |
| Order | 주문번호 자동 증가 / 중복 방지 | PASS |
| Order | TXT 저장 / 재실행 후 복원 | PASS |
| Receipt | PNG 생성 | PASS |
| Status | READY / RUNNING / ERROR 변경 | PASS |
| Alarm | 생성 / 선택 / 전체 삭제 | PASS |
| Log | 기록 / 필터 / JSON 저장 | PASS |
| Log | CSV Export | PASS |
| Dashboard | 주문 / 상태 / 알람 / 로그 요약 | PASS |
| Navigation | Home / Detail 페이지 전환 | PASS |
| Build | Windows 64-bit 빌드 | PASS |

## 실행 방법

1. Unity에서 `Assets/Kiosk/Scenes/Kiosk_Industrial` Scene을 엽니다.
2. Console Error가 없는지 확인합니다.
3. `File > Build Profiles > Windows`에서 `Intel 64-bit`로 빌드합니다.
4. 생성된 `Industrial Kiosk System.exe`를 실행합니다.

## 저장 파일 위치

Windows 환경에서는 일반적으로 다음 위치에 저장됩니다.

```text
C:\Users\<UserName>\AppData\LocalLow\<CompanyName>\<ProductName>\KioskData
```

## 프로젝트 구조

```text
Assets/Kiosk
├─ Docs
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
   ├─ Pages
   └─ UI
```

## 기술 포인트

- Page Router 기반 UI 전환 구조
- UI / 데이터 / 파일 저장 책임 분리
- `Application.persistentDataPath` 기반 데이터 영속성
- Prefab 기반 동적 리스트 UI
- 입력 검증과 중복 방지
- 제한형 로그 보관 정책
- TXT / JSON / CSV / PNG 혼합 출력 구조
- 빌드 환경에서 실제 저장과 재실행 테스트 완료

## 문서

- [프로젝트 개요](docs/01-project-overview.md)
- [시스템 아키텍처](docs/02-architecture.md)
- [기능 설명](docs/03-features.md)
- [데이터 저장 구조](docs/04-data-and-storage.md)
- [테스트 결과](docs/05-test-report.md)
- [프로젝트 구조](docs/06-project-structure.md)
- [개발 이력](docs/07-development-history.md)
- [포트폴리오 설명](docs/08-portfolio.md)

## 포트폴리오 한 줄 소개

> Unity 기반 산업용 키오스크 시스템을 설계하고, 주문·상태·알람·로그 기능과 파일 기반 데이터 영속성, 영수증 이미지 출력을 구현한 프로젝트입니다.

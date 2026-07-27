# 02. 시스템 아키텍처

## 전체 구조

```text
UI Layer
├─ Page Router
├─ Page Controllers
└─ Row UI Components

Controller Layer
├─ Order Controller
├─ Dashboard Controller
├─ Status Controller
├─ Alarm Controller
└─ Receipt Capture

Data / File Layer
├─ OrderData
├─ LogData
├─ TXT
├─ JSON
├─ CSV
└─ PNG
```

## 주요 클래스 역할

### scr_PageRouter

- Home / Detail 모드 전환
- Dashboard / Order / Status / Alarm / System Log 페이지 전환
- SideMenu 버튼 하이라이트
- 공통 네비게이션 처리

### scr_PageOrderController

- 주문 입력값 수집
- 입력 검증
- 주문 추가 / 수정 / 삭제
- Row 선택 및 InputField 재로딩
- 저장 및 영수증 출력 흐름 연결

### scr_OrderController

- 주문 데이터 메모리 관리
- Master TXT 저장 / 불러오기
- 개별 주문서 TXT 출력
- 영수증 저장 경로 생성
- 주문번호 중복 검사 및 검색

### scr_PageLogController

- 로그 데이터 관리
- 최신 로그 UI 표시
- 카테고리 필터
- JSON 저장 / 불러오기
- CSV Export
- 로그 개수 제한 및 오래된 로그 정리

### scr_ReceiptCapture

- 선택 주문 데이터를 영수증 UI에 반영
- 지정 RectTransform 영역 캡처
- PNG 인코딩 및 저장
- 저장 완료 로그 기록

## 설계 특징

- UI와 데이터 처리 책임 분리
- Controller 중심 구조
- 반복 Row UI Prefab화
- 저장 경로 통일
- 실제 데이터 변경 중심 로그 정책

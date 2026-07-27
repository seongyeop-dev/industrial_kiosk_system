# 04. 데이터 및 저장 구조

## 기준 경로

```csharp
Application.persistentDataPath
```

운영체제와 실행 위치가 달라도 Unity가 쓰기 가능한 사용자 데이터 경로를 제공하므로 빌드 환경에서도 안정적으로 저장할 수 있습니다.

## 저장 구조

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

## 저장 규칙

### Order

- 전체 주문 데이터: `orders.txt`
- 주문 추가 / 수정 / 삭제 직후 저장
- 앱 시작 시 자동 불러오기
- 주문번호 변경 시 이전 주문서 및 영수증 파일 정리

### Order Sheet

- 주문별 개별 TXT 파일
- 파일명: `OrderSheet_{OrderNo}.txt`
- 주문번호, 제품명, 수량, 날짜, 상태, 생성 시간 기록

### Receipt

- 주문별 PNG 이미지
- 파일명: `Receipt_{OrderNo}.png`
- UI 영역을 캡처하여 저장
- 저장 후 Texture 메모리 해제

### Log

- 기본 저장: `system_log.json`
- 수동 Export: CSV
- 저장 개수 제한을 통해 파일 크기 증가 방지

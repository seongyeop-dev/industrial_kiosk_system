# 01. 프로젝트 개요

## 프로젝트명

Industrial Kiosk System

## 개발 목표

Unity 기반으로 산업용 키오스크 UI를 구현하고 주문, 상태, 알람, 로그, 파일 저장 기능을 하나의 시스템으로 통합하는 것을 목표로 했습니다.

## 개발 범위

- Home 및 Dashboard UI
- Order 관리
- Status 관리
- Alarm 관리
- System Log 관리
- TXT / JSON / CSV / PNG 저장
- Windows 실행 파일 빌드 및 최종 기능 테스트

## 개발 방향

단순히 화면을 구성하는 데 그치지 않고, 실제 운영형 시스템처럼 데이터가 저장되고 다시 불러와지도록 설계했습니다. 또한 UI와 데이터 처리, 파일 저장 책임을 분리하여 유지보수성과 확장성을 높였습니다.

## 최종 결과

- 전체 화면 및 기능 연동 완료
- Windows 64-bit 빌드 성공
- 주문 저장 및 재실행 복원 확인
- 영수증 PNG 생성 확인
- 로그 저장 및 CSV Export 확인
- Dashboard 요약 정보 정상 연동

# 01. Overview

## 개발 배경

산업 현장에서 주문, 시스템 상태, 알람과 작업 로그를 각각 확인하는 흐름을 하나의 데스크톱 UI로 통합하기 위해 제작했습니다. 단순 화면 시안이 아니라 데이터를 로컬 파일로 저장하고 프로그램 재실행 후 다시 불러오는 운영 흐름까지 구현하는 것을 목표로 했습니다.

## 프로젝트 목적

- Home과 Dashboard에서 주요 운영 정보를 요약
- 주문 추가·수정·삭제와 입력값 검증
- 시스템 상태와 테스트 알람 관리
- 주문·상태·알람 변경 이력 기록
- TXT·JSON·CSV·PNG 파일 출력
- Windows 독립 실행형 빌드 검증과 데이터 복원 검증

## 구현 범위

| 구분 | 구현 내용 |
|:---|:---|
| Navigation | Home·Detail 화면 전환, Side Menu와 바로가기 |
| Order | CRUD, 자동 주문번호, 중복 방지, 입력 검증 |
| Status | READY·RUNNING·ERROR 상태 변경과 공통 UI 반영 |
| Alarm | 테스트 알람 생성, 목록·상세 조회, 전체 삭제 |
| Log | 최신순 표시, 카테고리 필터, JSON 저장, CSV Export |
| Receipt | 선택 주문 기반 영수증 UI와 PNG 출력 |
| Persistence | 주문 TXT 저장과 프로그램 시작 시 복원 |
| Build | Windows Intel 64-bit Standalone 실행 |

## 개발 단계

### UI 골격

- `Canvas_Main`, `TopBar`, `SideMenu`, `MainFrame`, `BottomBar` 구성
- Home과 Detail 화면 구조 정리
- 공통 버튼, Panel과 반복 Row Prefab 구성

### 주문 및 데이터 구조

- 주문 입력, Row 생성, 선택, 수정과 삭제 구현
- 주문번호 자동 생성과 중복 검사
- `OrderData`와 `scr_OrderController`를 통한 데이터 책임 분리

### 파일 저장과 로그

- Master 주문 TXT와 개별 주문서 TXT 저장
- 영수증 PNG 출력
- 로그 JSON 저장과 CSV Export
- 실제 데이터 변경 중심으로 로그 기록 범위 정리

### 안정화 및 빌드

- 화면 로그 15건, 메모리·파일 로그 30건으로 보관 범위 제한
- 새 로그만 Row로 추가하는 방식 적용
- 시간 표시를 1초 주기로 갱신
- Windows Intel 64-bit 빌드와 재실행 복원 확인

## 최종 결과

- 주문·상태·테스트 알람·로그 화면 연동 완료
- TXT·JSON·CSV·PNG 파일 생성 확인
- Windows 독립 실행형 빌드 환경 실행 확인
- 프로그램 재실행 후 저장된 주문 복원 확인

---

[문서 목차](README.md) · [프로젝트 README](../README.md)

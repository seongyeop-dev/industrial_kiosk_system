# 07. 개발 이력

## Phase 0 — 기획

- 산업용 키오스크 방향 확정
- Dashboard + 주문 / 출력 혼합형 UI 구조 채택
- 문서 및 Unity Hierarchy 초안 작성

## Phase 1 — UI 골격

- Canvas / TopBar / SideMenu / MainFrame / BottomBar 구성
- 공통 색상 및 버튼 스타일 적용
- 반복 UI Prefab 기준 정리
- Home / Detail UI 구조 완성

## Phase 2 — Order UI 및 검증

- Order 입력 UI 구현
- Row 생성 / 선택 / 수정 / 삭제 구현
- 날짜 및 상태 데이터 확장
- 주문번호 자동 생성 및 중복 방지
- 제품명 / 수량 / 날짜 입력 검증
- 상태별 텍스트 색상 적용

## Phase 3 — 데이터 중심 구조

- OrderData 구조 도입
- `scr_OrderController`를 통한 데이터 및 저장 책임 분리
- UI 직접 관리 구조에서 Controller 연동 구조로 전환

## Phase 4 — 저장 및 로그

- Master TXT 저장 / 불러오기
- 개별 주문서 TXT 출력
- Receipt PNG 저장
- System Log JSON 저장
- CSV Export
- History 명칭을 Log로 통일
- 카테고리 필터, Summary / Detail, Hover / Selected 적용

## Final Optimization

- 단순 페이지 이동 로그 제거
- 단순 Row 선택 로그 제거
- 실제 데이터 변화 중심 로그 정책 적용
- Log UI 표시 개수 제한
- 메모리 / 저장 로그 개수 제한
- 새 로그 1건만 UI에 추가하는 경량 구조 적용
- TimeDisplay를 1초 주기로 변경
- Windows 64-bit 빌드 성공

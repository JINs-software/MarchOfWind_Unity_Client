### \[개요\]
유니티를 통한 클라이언트 개발을 통해 클라이언트 개발 분야에 대한 학습과 유니티 개발 실습을 수행.
MMO_Fighter(미니 격투 형식 mmo 게임)과 같이 RPC 코드를 통해 메시지 송신을 추상화, 메시지 수신 및 로직 코드 수행을 자동화.

더미 프로그램을 만들기 이전에 2~4명의 소수 플레이어로 그 보다 많은 수의 세션을 하나의 필드에 생성하여 서버의 부하 및 로직을 검증하고자 RTS 장르와 같은 다중 유닛을 컨트롤 할 수 있는 게임을 선택하였음. 플레이어 자신이 생성하고 컨트롤하는 유닛마다 서버와 연결된 세션이 부착되며, 이동/공격/위치 동기화 패킷은 이 유닛 별 세션을 통해 송신됨.

### \[RPC, Proxy, Stub 클래스\]

클라이언트의 RPC, Proxy, Stub 클래스에 대한 설명: https://github.com/JINs-software/MMO_Fighter_Client 참고

### 간단 시연 영상

<p align="center">
<img src="https://github.com/user-attachments/assets/60b5345a-e0ec-4e63-a920-9549a03c3d97" width="800">
https://www.youtube.com/watch?v=5YbUqW7MBsM
</p>

### \[HubScene 씬\]

1. 클라이언트에 접속하면 서버의 IP와 Port 번호, 그리고 사용할 플레이어 이름(닉네임)을 입력하고, 'Connect to Server' 버튼을 클릭하여 서버 연결을 요청.
서버에서 TCP 연결 Accept 이후 세션을 생성한 후 전달된 플레이어 이름의 유효성과 중복을 확인한 후 응답 패킷을 전송함.
![접속](https://github.com/user-attachments/assets/d1f40258-00a6-4f83-a03d-6af30d553f42)

	
 3. 서버와 연결에 성공한 클라이언트는 '매치룸 생성', '매치룸 입장'을 할 수 있으며, 매치룸을 생성하기 위해 'Create a Match'를 클릭하고, '방 이름', '제한 인원'을 설정하여 매치룸 생성 요청 패킷을 서버에 송신. 서버에서는 이름의 유효성과 중복을 확인한 후 응답 패킷을 전송함.
![방생성](https://github.com/user-attachments/assets/3676639f-c790-4c24-adcf-a41aede42063)


4. 매치룸 입장을 하기 위해선 'Join a Match' 버튼을 클릭하여 로비로 입장. 입장 후 미리 개설된 매치룸들을 확인할 수 있으며, 원하는 매치룸을 클릭하여 입장. 서버는 로비 입장 시 기존 개설 매치룸들의 정보를 전송하며, 매치룸 입장 요청 시 유효한 방인지 수용 인원을 넘어가는지 등을 확인하여 응답 패킷 전송.
![방입장](https://github.com/user-attachments/assets/1d0f20c1-d682-4bef-bd27-3e44bf4ea270)
(호스트가 아닌 플레이어는 'Ready' 버튼을 통해 게임 시작을 준비함. 호스트의 경우 모든 플레이어가 준비될 때 'Start' 버튼을 통해 게임을 시작함. 서버는 매치룸 내 플레이어들의 준비 상태를 확인하여 게임 시작 요청에 응답.)


### \[Select/BattleField 씬\]
	
 1. 매치룸에서 게임이 시작되고, 로딩 씬을 거쳐 먼저 'SeletField' 씬으로 전환된다.
    셀렉터(유닛 선택 모형)이 텔레포터 입장을 통해 유닛을 생성한다. 유닛은 세션이 연결되어 서버에 요청이 보내어 진다.
    각 팀마다의 리스폰 포인트 기준으로 나선형 방향으로 유닛들이 생성된다.
    ![생성](https://github.com/user-attachments/assets/56a86794-16eb-41ca-ae9f-510a2ed4c995)

2. 좌클릭으로 단일 유닛을 선택할 수 있으며, 드래그를 통해 다중 유닛을 선택할 수도 있다.
   ![컨트롤](https://github.com/user-attachments/assets/9f390712-7215-4f9e-8a39-09a88d31808e)

3. 플레이어 유닛에는 적군 유닛을 탐지할 수 있는 추적 콜라이더가 부착된다. 콜라이더를 통해 타 팀의 유닛이 감지되면 추격 대상이 된다.
   ![기즈모](https://github.com/user-attachments/assets/f70a8c09-24fd-47a2-926a-9de0b6b669a0)

4. 배틀필드 내 두 플레이어 유닛들의 전투 장면
   ![전투](https://github.com/user-attachments/assets/e5ec6a47-290a-4635-b011-53444c1351ba)

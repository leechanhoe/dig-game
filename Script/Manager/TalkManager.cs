using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkManager : MonoBehaviour
{
    static public TalkManager instance;

    public Dictionary<int, string[]> talkData { get; set; }
    //Dictionary<int, Sprite> portraitData;
    //public Sprite[] portraitArr;
    //public Sprite[] BackGroundArr;

    [SerializeField]
    GameObject darkEffect;
    public TypeEffect typeEffect;
    public Animator talkPanel;
    public Text npcName;

    //public Image portrait;
    //public Animator portraitAnim;
    //public Sprite prevPortrait;

    //public Text questTalk;

    public bool isTalking; // 대화중인가?
    public int talkIndex;

    Player player;
    QuestManager questManager;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        questManager = FindObjectOfType<QuestManager>();
        talkData = new Dictionary<int, string[]>();
        //portraitData = new Dictionary<int, Sprite>();
        GenerateData();

        if (instance == null) // 파괴방지
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }


    void GenerateData() // 10101 >> 만의자리 : 엔피씨아이디
        // 백의자리 : 퀘스트아이디

        // 일의자리 : 퀘스트 처음 받을때 대화 0 
        // 퀘스트를 받고 난후 퀘스트를 주는 npc한테 다시 말걸떄 1(퀘스트 주는애랑 완료하는애랑 같은애면 2로 통일)
        // 퀘스트의 완료npc한테 말걸었을 떄 완료조건을 충족 못시켰으면 2
        // 퀘스트의 완료조건을 채우고 퀘스트 완료하는애한테 가면 3 
    {
        talkData.Add(10000, new string[] { "하루라도 광질을 하지 않으면 광손실이난다고!! 어서가서 땅파"});
        talkData.Add(20000, new string[] { "황수정..황수정.. 어디있는거야.." });
        talkData.Add(40000, new string[] { "돌덩어리보다 단단한 얼음이다. 일반적인 방법으론 부수기 힘들어보인다." });

        talkData.Add(10000 + 100 + 0, new string[] { "안녕? 네가 이번에 새로 들어온 전학생이구나? 지하고등학교에 온걸 환영해!", "나는 네 담임을 맡은 베테랑 광부다.", "그럼 바로 시작해보지, 우선 땅을 파려면 드릴이 있어야겠지? 드릴이 없으면 땅을 팔 수가 없단다.", "옆에 있는 상점에서 구리 드릴을 하나 가져와보렴." });
        talkData.Add(10000 + 100 + 2, new string[] { "드릴이 없으면 땅을 팔 수 없어.","만약 드릴을 장착중이면 장착을 해제해봐"});
        talkData.Add(10000 + 100 + 3, new string[] { "잘했다. 이 드릴은 이제 네 것이란다. 항상 가지고 다니렴." });

        talkData.Add(10000 + 200 + 0, new string[] { "이번에는 구매했던 드릴을 장착해볼거야.", "오른쪽 위의 메뉴버튼을 누르고 가방에 들어가서 드릴을 장착해보렴." });
        talkData.Add(10000 + 200 + 2, new string[] { "오른쪽 위의 메뉴버튼을 누르고 가방에 들어가서 드릴을 장착해보렴." });
        talkData.Add(10000 + 200 + 3, new string[] { "좋아, 좀 무겁지? 꽉 잡고 사용해야해.","이제 땅을 팔 준비가 어느정도 된 것 같네, 너에게 광부의 증표를 줄게." });

        talkData.Add(10000 + 300 + 0, new string[] { "지하에서는 맘 편히 땅만 팔수는 없을거야. 여러 몬스터가 출몰하거든.\n지하 깊은 곳에는 원거리 공격을 하는 몬스터가 있다는 소문이 있지", "하지만 너무 걱정하지마. 너무 깊은 곳만 안들어가면 그렇게 강한 몬스터는 없을거야.", "지하 25m 즈음 달팽이몬스터가 보일거야. 상점에서 폭탄을 구매해서 달팽이 몬스터를 잡고 달팽이의 껍질을 가져와보렴." });
        talkData.Add(10000 + 300 + 2, new string[] { "너무 무서워 할 필요 없어. 달팽이는 가장 약한 몬스터야" });
        talkData.Add(10000 + 300 + 3, new string[] { "좋아, 이제 어디가서 안 맞고 다니겠군." });

        talkData.Add(10000 + 400 + 0, new string[] { "땅을 파다보면 바위가 많이 보일거야. 이 바위는 상점에서 파는 다이너마이트로 부술 수 있어.\n그런데 가끔 다이너마이트로도 부셔지지 않는 바위도 있으니 주의하렴.", "지하 40m 부근에 상자가 하나 묻혀있는데, 그 안에 들은걸 가져와보렴. 주변이 바위로 둘러쌓여있어서 바위를 부숴야 할거야." });
        talkData.Add(10000 + 400 + 2, new string[] { "내가 아끼는 물건이니 조심히 가져와야해." });
        talkData.Add(10000 + 400 + 3, new string[] { "잘했어. 이 장갑으로 말할 것 같으면 내 인생이 담긴 장갑이지" });

        talkData.Add(10000 + 500 + 0, new string[] { "음? 왜 교복을 입고 땅을 파야하냐고? 우리 교복은 특수재질로 만들어져서 몸을 보호해주는 기능이 있지. 잘 입고다니렴.", "저번에 받았던 광부의 증표는 잘 가지고있지?", "광부의 증표는 대장간에서 아이템을 만들때 재료로 쓰이지. 이번엔 그걸 사용해보자.","지하 60~70m 부근에 상자가 하나 묻혀있는데, 거기에 내가 제조법을 넣어놨어. 그걸 가지고 대장간에 가서 아이템을 제조해보렴." });
        talkData.Add(10000 + 500 + 2, new string[] { "대장간에 들어가서 제조법을 누르면 제조법을 등록할 수 있어. 재료를 모아 아이템을 만들어보렴." });
        talkData.Add(10000 + 500 + 3, new string[] { "꽤 괜찮아보이네. 이것 뿐만이 아니라 땅속엔 여러 상자들이 묻혀있으니 잘 찾아봐! 어쩌면 엄청난 것이 숨겨져있을지도 모르지?" });
        
        talkData.Add(10000 + 600 + 0, new string[] { "오늘은 같은 반 친구 한명을 소개시켜줄게. \n그 친구는 핑크색을 엄청 좋아하는 친구지. 네가 자수정을 15개정도 들고가면 금방 친해질 수 있을거야.", "지금쯤이면 150 ~ 160m 부근 동굴 안 어딘가에 있으려나?"});
        talkData.Add(10000 + 600 + 1, new string[] { "동굴의 입구는 가끔 땅에 가려져서 안보일 때도 있으니까 유심히 살펴봐야해." });
        talkData.Add(20000 + 600 + 2, new string[] { "누구세요?" });
        talkData.Add(20000 + 600 + 3, new string[] { "안녕? 그 교복을 보니 너도 지하고등학교 학생이구나. 못보던 얼굴인데 전학왔나보네?","어? 혹시 손에 들고있는 그건 혹시 황수정 아니니? 내가 제일 좋아하는 보석인데..","뭐? 선물이라고? 고마워! 보답으로 유용한 아이템을 하나 줄게." });

        talkData.Add(10000 + 700 + 0, new string[] { "오늘은 기말고사가 있는 날인건 까먹지 않았겠지? 긴장하지 말라구.", "지하 깊은곳 어딘가에 있는 몬스터를 잡아와보렴. 너라면 할수있을거야!\n 아마 깊숙한 동굴 안에 숨어있을거야." });
        talkData.Add(10000 + 700 + 2, new string[] { "회복아이템도 든든히 챙겨가라구." });
        talkData.Add(10000 + 700 + 3, new string[] { "음? 너 뭘 가져온거니?", "이건.. 돌이 무언가로 오염된 것 같은데.. 지하에서 어떤 몬스터를 잡은거니?", "뭐? 그런 몬스터는 처음듣는데.. 내가 준비한 몬스터는 그렇게까지 강한 몬스터가 아니였어.","일단 시험은 합격이다. 난 이 돌조각을 조사해보마.","그리고 시험을 통과한 너에게 400m이하로 내려갈 수 있는 자격을 줄게. 한번 지하390m 부근으로 내려가보렴, 길이 열렸을거야.\n난 먼저 아래로 가있으마." });
      
        talkData.Add(10000 + 800 + 0, new string[] { "어떤 이유에서인지 땅이 점점 오염되고 있어. 오염된 땅은 연보라색의 반점같은걸 가지고있다고 한다는구나. 지하로 내려갈수록 오염은 더 심해지는 것 같아.", "조사결과 오염된 땅을 파면 오염된 가스가 위쪽으로 분출된다는구나. \n그렇기 때문에 유독가스를 피하기 위해선 오염된 땅을 위쪽에서 파지말고 양 옆에서 파야하는 것 같다.", "오른쪽으로 가면 전에 파놓은 구덩이가 하나 있는데, 거기 누군가가 마스크를 숨겨놨다는 소문이 있어.\n마스크만으로는 역부족이겠지만 일단 그거라도 가져와야할 것 같아","오염된 땅을 조심하면서 마스크를 찾아와보렴." });
        talkData.Add(10000 + 800 + 2, new string[] { "유독가스는 위쪽으로 올라가는 성질이 있다는구나. 양 옆에서 접근해보렴" });
        talkData.Add(10000 + 800 + 3, new string[] { "상태는 꽤 양호하군. 마스크가 필수인 세상이 올 줄이야.. 마스크는 항상 끼고 다니렴." });

        talkData.Add(10000 + 900 + 0, new string[] { "조사 결과 오염물질은 평범한 돌조각을 강력한 몬스터로 변이 시키기도 한다는구나. 전에 네가 잡았던 그 몬스터도 아마 그 영향이겠지.","그런데 특이하게도 오염물질에 내성을 가지고 있어서 깨끗한 땅이 있다는구나.","나도 가본적은 없지만 지하 600m 부근 어딘가에 그 땅으로 가는 길이 있다는 소문이 있지. 가서 그 땅을 조사해주겠니?" });
        talkData.Add(10000 + 900 + 1, new string[] { "지하 600m 부근 어딘가에 길이 있다는 소문이 있어. 그런데 무슨 이유에서인지 그 땅에 가본사람은 찾기가 힘들구나." });
        talkData.Add(40000 + 900 + 3, new string[] { "(돌덩어리보다 단단한 얼음이 길을 막고있어 지나갈 수 없을 것 같다. 돌아가서 보고하자.)" });

        talkData.Add(10000 + 1000 + 0, new string[] { "단단한 얼음이 길을 막고 있다고? 아무래도 부수려고 하지 말고 녹여야 할 것 같구나.","예전 기록을 보면 '불의 원석' 으로 추위를 견뎌냈다는 기록이 있다. 그런데 불의 원석 제조법이 어느샌가 사라졌다는구나.","아마 땅속 어딘가에 묻혀있을 확률이 클거야. 얼음지역 사람들이 자주 썼으니 얼음지역으로 가는 길 근처에 묻혀있지 않을까? 그걸 찾아서 불의 원석을 제조해와보렴." });
        talkData.Add(10000 + 1000 + 2, new string[] { "제조법은 얼음지역으로 가는 길 근처에 묻혀있지 않을까?" });
        talkData.Add(10000 + 1000 + 3, new string[] { "이것이 불의원석.. 나도 처음보는구나. 화상을 입을수도 있으니 조심히 다루렴.","불의 원석을 가지고 있으면 그 열기로 얼음도 부술 수 있을거야. 이제 얼음지역으로 가서 조사를 부탁해."});

        talkData.Add(30000 + 1100 + 0, new string[] { "(게다가 이상한 진동이 아래에서 느껴진다. 오른쪽 땅을 파서 아래를 확인해보자.)" });
        talkData.Add(30000 + 1100 + 2, new string[] { "(게다가 이상한 진동이 아래에서 느껴진다. 오른쪽 땅을 파서 아래를 확인해보자.)" });
        talkData.Add(30000 + 1100 + 3, new string[] { "(설마 저 괴물녀석이 레버의 주변을 건드려서 레버가 작동하지 않는건가.. 돌아가서 이 사실을 알리자.)" });

    }

    public string GetTalk(int id, int talkIndex)
    {
        if (!talkData.ContainsKey(id))//대화 없을 때 예외처리
        {
            if (!talkData.ContainsKey(id - id % 100))
                return GetTalk(id - id % 10000, talkIndex); //퀘스트 맨 처음 대사가 없을 때 기본 대사 가져오기
            //else
                //return GetTalk(id - id % 100, talkIndex);//해당 퀘스트 진행 순서 대사가 없을 때 퀘스트 맨 처음 대사 가져오기
        }
        if (talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex];
    }

    public void Talk(ObData obData)
    {
        int questTalkIndex = 0;
        int questState = -1; // 퀘스트토크인덱스의 일의자리만 가져와서 현재 상태가 퀘스트 맨첨 받는상탠지 완료하는상탠지등 저장
        string talkData = "";

        //대화 데이터 셋
        if (typeEffect.isAnim)
        {
            typeEffect.SetMsg("");
            return;
        }
        else
        {
            if(obData.isHiddenNpc)
                questTalkIndex = questManager.CheckHiddenQuest(obData.id);
            else
                questTalkIndex = questManager.CheckQuest(obData.id);
            questState = questTalkIndex % 10;
            talkData = GetTalk(obData.id + questTalkIndex, talkIndex);//토크매니저에있던 대사 가져옴
        }

        //대화 끝내기
        if (talkData == null)
        {
            isTalking = false;
            talkPanel.SetBool("isShow", isTalking);
            darkEffect.SetActive(false);
            npcName.text = "";
            talkIndex = 0;
            player.CanMove();

            if (!obData.isHiddenNpc)
            {
                if (questManager.isConditionFilled() && questState == 3) // 퀘스트를 완료했으면 인덱스 증가
                    questManager.QuestComplete();
                else if (questState == 0) // 퀘스트를 시작
                {
                    questManager.QuestStart();
                    obData.ExclamationMarkOff();
                }
            }
            else
            {
                if (questManager.isConditionFilled(true) && questState == 3) // 퀘스트를 완료했으면 인덱스 증가
                    questManager.QuestComplete(true);
                else if (questState == 0) // 퀘스트를 시작
                    questManager.QuestStart(true);
            }
            return;
        }

        player.CantMove();

        typeEffect.SetMsg(talkData);

        //다음 대화
        isTalking = true;
        talkPanel.SetBool("isShow", isTalking);
        darkEffect.SetActive(true);
        npcName.text = obData.npcName;
        talkIndex++;
    }
}

using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//答案面板的控制脚本 附属控制下属的选项界面
public class ans_ques : all_panel {
    public Text title, quest, correct, countdown; //答题界面的各种信息
    public RawImage pic; //图片型题目的图片放置地点
    private bool random; //记录是否使用随机顺序
    protected int num_done; //已经答过的题数
    private List<QuestData> ques; //全部题目
    public int correctNum = 0; //记录已答对的题数
    public int numOfChoices = 4; //选项个数
    public AnsBtnId[] ansBtns; //引用答案按钮
    public int noOfQuest = 0; //题目总数
    private bool[] que_useful; //随机出题时记录这一问题是否出过
    public int quiz_type;//当前进入的答题模式
    //各模式的值
    public const int ty_no = 0,
        ty_com = 1,
        ty_cla = 2,
        ty_tl = 3;
    private float remainTime; //剩余时间
    public float limit_time; //总的时间

    public Vector3 circle_begin; //确定圆形初始位置
    public float genhao2 = Mathf.Sqrt (2); //存的根号2的值 后面计算选项面板摇杆位置时要用

    // Use this for initialization
    void OnEnable () {
        //在每次进入答题界面时记录circle的初始位置
        if (circle || circle_begin != circle.transform.localPosition) {
            circle_begin = circle.transform.localPosition;
        }
    }

    // Update is called once per frame
    void Update () {
        //检测手柄控制摇杆、选项界面
        float h, v; //h、v 读取摇杆的水平、竖直轴值
        h = Input.GetAxis ("Horizontal");
        v = Input.GetAxis ("Vertical");
        Vector3 hand;
//不同平台对摇杆的值进行不同处理
#if UNITY_ANDROID
        float ttt = h;
        h = v;
        v = -ttt;
        hand = new Vector3 (h * 340, v * 200, 0);
#else
        hand = new Vector3 (-h * 170, v * 100, 0);
#endif
        //呼出/隐去 选项界面
        if (Input.GetKeyDown (now_key[4])) {
            ans.SetActive (!ans.activeInHierarchy);
        }
        //用摇杆控制circle
        if (ans.activeInHierarchy) circle.transform.localPosition = circle_begin + hand;
        //按下选择按键时 识别那个选项被选择
        if (Input.GetKeyDown (now_key[5])) {
            if (v >= genhao2 / 2 && Mathf.Abs (h) <= genhao2 / 2) {
                if (ansBtns[0].gameObject.activeInHierarchy) { ansBtns[0].OnPressed (); }
            } else if (h > genhao2 / 2 && Mathf.Abs (v) < genhao2 / 2) {
                if (ansBtns[1].gameObject.activeInHierarchy) { ansBtns[1].OnPressed (); }
            } else if (h < -genhao2 / 2 && Mathf.Abs (v) < genhao2 / 2) {
                if (ansBtns[2].gameObject.activeInHierarchy) { ansBtns[2].OnPressed (); }
            } else if (v <= -genhao2 / 2 && Mathf.Abs (h) <= genhao2 / 2) {
                if (ansBtns[3].gameObject.activeInHierarchy) { ansBtns[3].OnPressed (); }
            }
        }
    }

    //将无关panel设置为未激活 并做初始准备
    public void quiz_begin (int now_type) {
        //开始时将其他panel隐去
        quiz.SetActive (true);
        welcome.SetActive (false);
        ans.SetActive (false);
        score.SetActive (false);

        //记录当前答题的模式 不同模式进行不同的准备
        quiz_type = now_type;
        switch (quiz_type) {
            case ty_tl:
                ques = quesOfOther; //ques指向要使用的题库
                que_useful = new bool[ques.Count]; //记录使用情况的数组 同时初始化 为true时说明这个问题可以使用
                for (int i = 0; i < ques.Count; ++i) {
                    que_useful[i] = true;
                }
                random = true; //开启随机模式
                countdown.gameObject.SetActive (true); //打开计分
                remainTime = limit_time; //设定剩余时间
                InvokeRepeating ("Timing", 0, 1); //计时
                noOfQuest = (int) (UnityEngine.Random.value * ques.Count); //题目的数量
                break;
            case ty_cla:
                ques = quesOfOther;
                que_useful = new bool[ques.Count];
                for (int i = 0; i < ques.Count; ++i) {
                    que_useful[i] = true;
                }
                random = true;
                countdown.gameObject.SetActive (false);
                noOfQuest = (int) (UnityEngine.Random.value * ques.Count);
                break;
            case ty_com:
                ques = quesOfCompe;
                random = false;
                noOfQuest = 0;
                countdown.gameObject.SetActive (false);
                break;
        }
        num_done = 0; //已答0题
        correctNum = 0; //已答对0题
        StartCoroutine (UpdateQuest (noOfQuest)); //开始更新题目
    }

    //正式答题并更新
    public IEnumerator UpdateQuest (int no) {
        //关闭选项面板
        ans.SetActive (false);
        //设置一些文本信息
        title.text = "第" + (num_done + 1) + "/" + ques.Count + "题";
        quest.text = ques[no].quest;
        correct.text = "已对" + correctNum + "题";
        //根据题目类型来设置题目
        switch (ques[no].type) {
            case QuestData.ty_choose:
                //设置按钮
                for (int i = 0; i < ques[no].op_num; i++) {
                    ansBtns[i].gameObject.SetActive (true);
                    ansBtns[i].Init ((char) (i + 'A') + " " + ques[no].answer[i]);
                }
                //将没有使用的按钮隐去
                for (int i = ques[no].op_num; i < numOfChoices; ++i) {
                    ansBtns[i].gameObject.SetActive (false);
                }
                //关闭图片
                pic.gameObject.gameObject.SetActive (false);
                break;
            case QuestData.ty_judge:
                ansBtns[0].gameObject.SetActive (false);
                ansBtns[3].gameObject.SetActive (false);
                ansBtns[1].gameObject.SetActive (true);
                ansBtns[2].gameObject.SetActive (true);
                for (int i = 1; i <= 2; ++i) {
                    ansBtns[i].Init (ques[no].answer[i]);
                }
                pic.gameObject.SetActive (false);
                break;
            case QuestData.ty_pic:
                pic.gameObject.SetActive (true);
                WWW new_pic;
//不同平台加载图片方式不一样
#if UNITY_ANDROID
                new_pic = new WWW (Application.persistentDataPath + "/" + ques[no].src);
#else
                new_pic = new WWW ("file://" + Application.persistentDataPath + "/" + ques[no].src);
#endif
                yield return new_pic;
                pic.texture = new_pic.texture;
                for (int i = 0; i < ques[no].op_num; i++) {
                    ansBtns[i].gameObject.SetActive (true);
                    ansBtns[i].Init ((char) (i + 'A') + " " + ques[no].answer[i]);
                }
                for (int i = ques[no].op_num; i < numOfChoices; ++i) {
                    ansBtns[i].gameObject.SetActive (false);
                }
                break;
        }
    }

    //判断是否做对
    public void Judge (AnsBtnId a) //判断答案是否正确 并改变noOfQuest
    {
        if (a.ID != ques[noOfQuest].ans) {
            a.img.color = Color.red;
        } else {
            correctNum++;
        }
        ansBtns[ques[noOfQuest].ans].img.color = Color.green;
        foreach (AnsBtnId ab in ansBtns) {
            ab.btn.interactable = false;
        }
        if (num_done < ques.Count - 1) {
            ++num_done;
            if (random) {
                que_useful[noOfQuest] = false;
                noOfQuest = (int) (UnityEngine.Random.value * ques.Count);
                while (!que_useful[noOfQuest]) {
                    ++noOfQuest;
                    noOfQuest %= ques.Count;
                }
            } else {
                ++noOfQuest;
            }
            StartCoroutine (DelayedUpdateQuest ());
        } else {
            StartCoroutine (DelayASec ());

        }
    }

    IEnumerator DelayedUpdateQuest () {
        yield return new WaitForSeconds (1);
        UpdateQuest (noOfQuest);
    }
    IEnumerator DelayASec () {
        yield return new WaitForSeconds (1);
        GotoFinal ();
    }

    //游戏结束 进入积分面板
    public void GotoFinal () {
        //        is_what = nothing;
        CancelInvoke ();
        score.SetActive (true);
    }

    public void Timing () {
        if (remainTime > 0) {
            countdown.text = "还剩" + remainTime + "秒";
            remainTime--;
        } else {
            GotoFinal ();
        }
    }

    public int getCor () {
        return correctNum;
    }

    //设定答题模式
    public void setType (int now) {
        quiz_type = now;
    }
    //获得当前模式
    public int getType () {
        return quiz_type;
    }
    
    //获得各种模式值
    public int getTypeNo () {
        return ty_no;
    }
    public int getTypeCom () {
        return ty_com;
    }
    public int getTypeCla () {
        return ty_cla;
    }
    public int getTypeTl () {
        return ty_tl;
    }
    //设定问题
    public void setQues (List<QuestData> q) {
        ques = q;
    }
}
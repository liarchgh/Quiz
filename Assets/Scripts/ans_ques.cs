using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ans_ques : all_panel {
    public Text title, quest, correct, countdown; //, finalScore; //答题界面的各种信息
    public RawImage pic;
    private bool random; //记录是否使用随机顺序
    protected int num_done; //已经答过的题数
    private List<QuestData> ques; //记录当前模式的题目信息
    public int correctNum = 0; //记录已答对的题数
    public int numOfChoices = 4; //选项个数
    public AnsBtnId[] ansBtns; //引用答案按钮
    public int noOfQuest = 0; //题目总数
    private bool[] que_useful; //记录这一问题是否出过
    public int quiz_type;
    public const int ty_no = 0,
        ty_com = 1,
        ty_cla = 2,
        ty_tl = 3;
    private float remainTime; //剩余时间
    public float limit_time; //要求时间

    public Vector3 circle_begin; //确定圆形初始位置
    public float genhao2 = Mathf.Sqrt (2);

    // Use this for initialization
    void OnEnable () {
        if (circle || circle_begin != circle.transform.localPosition) {
            circle_begin = circle.transform.localPosition;
        }
    }

    // Update is called once per frame
    void Update () {

        float h, v;
        //手柄控制
        h = Input.GetAxis ("Horizontal");
        v = Input.GetAxis ("Vertical");
        Vector3 hand;
#if UNITY_ANDROID
        float ttt = h;
        h = v;
        v = -ttt;
        hand = new Vector3 (h * 340, v * 200, 0);
#else
        hand = new Vector3 (-h * 170, v * 100, 0);
#endif

        if (Input.GetKeyDown (now_key[4])) {
            ans.SetActive (!ans.activeInHierarchy);
        }
        if (ans.activeInHierarchy) circle.transform.localPosition = circle_begin + hand;
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
        quiz.SetActive (true);
        welcome.SetActive (false);
        ans.SetActive (false);
        score.SetActive (false);

        quiz_type = now_type;
        switch (quiz_type) {
            case ty_tl:
                ques = quesOfOther;
                que_useful = new bool[ques.Count];
                for (int i = 0; i < ques.Count; ++i) {
                    que_useful[i] = true;
                }
                random = true;
                countdown.gameObject.SetActive (true);
                remainTime = limit_time;
                InvokeRepeating ("Timing", 0, 1);
                noOfQuest = (int) (UnityEngine.Random.value * ques.Count);
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
        num_done = 0;
        correctNum = 0;
        StartCoroutine (UpdateQuest (noOfQuest));
    }

    public IEnumerator UpdateQuest (int no) {
        ans.SetActive (false);
        title.text = "第" + (num_done + 1) + "/" + ques.Count + "题";
        quest.text = ques[no].quest;
        correct.text = "已对" + correctNum + "题";
        Debug.Log (ques[no].type);
        switch (ques[no].type) {
            case QuestData.ty_choose:
                for (int i = 0; i < ques[no].op_num; i++) {
                    ansBtns[i].gameObject.SetActive (true);
                    ansBtns[i].Init ((char) (i + 'A') + " " + ques[no].answer[i]);
                }
                for (int i = ques[no].op_num; i < numOfChoices; ++i) {
                    ansBtns[i].gameObject.SetActive (false);
                }
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

    public void setType (int now) {
        quiz_type = now;
    }
    public int getType () {
        return quiz_type;
    }
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
    public void setQues (List<QuestData> q) {
        ques = q;
    }
}
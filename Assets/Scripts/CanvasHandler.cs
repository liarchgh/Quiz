using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class CanvasHandler : all_panel {
    public string ans_but; //手柄确定按钮
    public GameObject wel, qui, an, scor, circl, cho;
    public CanvasHandler ch;
    // public Text title, quest, correct, countdown, finalScore;
    public List<string> quizData;
    public const string nameOfCompe = "quiz_competition.txt"; //competition模式文件名
    public const string nameOfOther = "quiz_other.txt"; //其他模式文件名
    public string urlUpdate; //更新competition模式文件的链接
    private string src;//, now_file; //src为原文件位置 now为最新文件位置（均无文件名）
    public ans_ques newaq;
    public int numOfChoices = 4;
    private const string ques_pic = "图片题",
        ques_judge = "判断题",
        ques_choose = "选择题";

    // Use this for initialization
    //根据路径和平台加载题目
    void Start () {
        scr_ch = ch;
        scr_aq = newaq;
        welcome = wel;
        quiz = qui;
        ans = an;
        score = scor;
        circle = circl;
        choose = cho;
#if UNITY_ANDROID
        src = Application.streamingAssetsPath;
        // now_file = "jar:file://" + Application.persistentDataPath;
#else
        src = "file://" + Application.streamingAssetsPath;
        // now_file = "file://" + Application.persistentDataPath;
#endif
        load_all ();
        choose.SetActive (true);
    }

    //根据按键不同进入不同模式
    void Update () { }

    //将字符串形式的题目转化为更方便的QuesData结构体
    List<QuestData> ScanData (List<string> raw) {
        List<QuestData> tmpList = new List<QuestData> ();
        QuestData tmp = new QuestData (numOfChoices);
        int counter = 0; //counter记录选项个数
        string str_type = "2121"; //题目类型
        for (int th = 0; th < raw.Count; ++th) { //每一轮从每一题的第一行开始识别 将整道题识别完 下一轮开始时别选项
            string r = raw[th];
            int i = 0; //记录整个题目的正式开始位置 即‘、’符号的位置
            for (i = 0; i < r.Length; i++) {
                if (r[i] == '、') {
                    break;
                }
            }
            if (i < r.Length - 1) { //如果有题目的话
                string tmp1 = r.Substring (i + 1); //'、'符号后的字符串
                // string tmp2 = "asdsa";
                // char ans = r[r.Length - 1]; //题目这一行的最后一个字符 一般为答案
                if (!char.IsLetter(r[0])) { //将这一题的第一行转化为题目和答案
                    string tmp2 = tmp1.Substring(5, tmp1.Length - 6); // 正式题目或者路径信息
                    char ans = tmp1[tmp1.Length - 1]; //最后一个字母  表示答案
                    if ((str_type == ques_choose || str_type == ques_pic) && th > 0) {
                        tmp.op_num = counter;
                        tmpList.Add (tmp);
                    }
                    str_type = tmp1.Substring (1, 3); //题目类型
                    switch (str_type) {
                        case ques_choose:
                            tmp = new QuestData(numOfChoices);
                            tmp.type = QuestData.ty_choose;
                            tmp.quest = tmp2; //tmp2作为题目
                            tmp.ans = ans - 'A'; //ans代表的字符作为答案
                            counter = 0;
                            break;
                        case ques_judge:
                            tmp = new QuestData(numOfChoices);
                            tmp.type = QuestData.ty_judge;
                            tmp.answer[1] = "正确";
                            tmp.answer[2] = "错误";
                            tmp.quest = tmp2; //tmp2最后一个字符之前的字符串作为题目
                            if (ans == 'F') {
                                tmp.ans = 2;
                            } //tmp2最后一个字符指出答案
                            else {
                                tmp.ans = 1;
                            }
                            tmp.op_num = 2;
                            tmpList.Add (tmp);
                            break;
                        case ques_pic:
                            tmp = new QuestData(numOfChoices);
                            tmp.type = QuestData.ty_choose;
                            tmp.quest = tmp2; //tmp2作为题目
                            tmp.ans = ans - 'A'; //ans代表的字符作为答案
                            tmp.src = raw[++i];
                            counter = 0;
                            break;
                    }
                } else {
                    tmp.answer[counter++] = tmp1;
                }
            }
        }
        return tmpList;
    }

    //用流方式将文件读为字符串
    List<string> LoadFile (string name) {
        List<string> arrlist = new List<string> ();
        try {
            using (StreamReader sr = new StreamReader (name)) {
                String line;
                while ((line = sr.ReadLine ()) != null) {
                    arrlist.Add (line);
                    //Debug.Log(line);
                }
            }
        } catch (Exception e) {
            Debug.Log ("NONONO" + e.Message);
        }
        return arrlist;
    }

    //退出按钮
    public void Exit () {
        Application.Quit ();
    }

    //将所有给出的文件转化为后面使用的QuesData格式
    void load_all () {
        StartCoroutine (load_file (nameOfCompe));
        StartCoroutine (load_file (nameOfOther));
    }

    //将name文件转化为QuesData结构体形式
    //name为文件名 路径均为src和now
    IEnumerator load_file (string name) {

        WWW w = new WWW (src + "/" + name);
        yield return w;
        CreateFile (Application.persistentDataPath + "/" + name, w.text); //ok
        //StartCoroutine(up_file());
        quizData = LoadFile (Application.persistentDataPath + "/" + name); //ok
        switch (name) {
            case nameOfCompe:
                scr_aq.quesOfCompe = ScanData (quizData);
                break;
            case nameOfOther:
                scr_aq.quesOfOther = ScanData (quizData);
                break;
        }
        // que_useful = new bool[aq.quesOfCompe.Count];
    }

    //用来给其他脚本文件调用使用
    public void up () {
        StartCoroutine (up_file ());
    }

    IEnumerator up_file () {
        WWW new_file = new WWW (urlUpdate + "/" + nameOfCompe);
        yield return new_file;
        if (new_file.text.Length > 5) {
            File.Delete (Application.persistentDataPath + "/" + nameOfCompe);
            CreateFile (Application.persistentDataPath + "/" + nameOfCompe, new_file.text);
        }
        new_file = new WWW (urlUpdate + "/" + nameOfOther);
        yield return new_file;
        if (new_file.text.Length > 5) {
            File.Delete (Application.persistentDataPath + "/" + nameOfOther);
            CreateFile (Application.persistentDataPath + "/" + nameOfOther, new_file.text);
        }
        load_all ();
    }

    //在路径path（包含文件名）创建文件并写入info字符串
    void CreateFile (string path, string info) {
        StreamWriter sw;
        FileInfo file = new FileInfo (path);
        if (file.Exists) {
            return;
        }
        sw = file.CreateText ();
        sw.WriteLine (info);
        sw.Close ();
        sw.Dispose ();
    }

    public GameObject getWel () {
        return welcome;
    }
    public GameObject getScor () {
        return score;
    }
    public GameObject getQuiz () {
        return quiz;
    }
    public GameObject getAns () {
        return ans;
    }
    public GameObject getCir () {
        return circle;
    }

    // public List<QuestData> aq.quesOfCompe;//记录当前模式的题目信息
    // public List<QuestData> quesOfOther;//记录当前模式的题目信息

    // private int num_done;//已经答过的题数
    // public int is_what;//记录当前进入的是哪种模式
    // private const int nothing = 0;
    // private const int competition = 1;
    // private const int classic = 2;
    // private const int time = 3;

    /*
#if UNITY_IPHONE
            string filepath = Application.dataPath +"/Raw
            quizData = LoadFile(filepath + "/Quiz.txt");
    #elif UNITY_ANDROID
            //string filepath = "jar:file://" + Application.dataPath + "!/assets/";
            quizData = LoadFile_Android ("Quiz");
#else
            string filepath = Application.dataPath + "/StreamingAssets";
            quizData = LoadFile (filepath + "/Quiz.txt");
#endif
            //quizData=LoadFile(fileName);
            //LoadFile(Application.streamingAssetsPath+"/position.txt");
            questData = ScanData (quizData);
            que_useful = new bool[questData.Count];
    */

    // List<string> LoadFile_Android(string name) {
    //     List<string> arrlist = new List<string>();
    //     try {
    //         TextAsset TXTFile = Resources.Load(name) as TextAsset;
    //         if (TXTFile != null) {
    //             Debug.Log(TXTFile.text);
    //             string[] tmp = TXTFile.text.Split(new char[] { '\r', '\n' });
    //             bool isBlank = false;
    //             foreach (string t in tmp) {
    //                 if (isBlank) {
    //                     isBlank = false;
    //                 }
    //                 else {
    //                     isBlank = true;
    //                     arrlist.Add(t);
    //                 }
    //             }
    //             //l.setInfo("success");
    //         }
    //         else {
    //             Debug.Log("Failed");
    //             //l.setInfo("failed to load map");
    //             return null;
    //         }

    //     }
    //     catch (Exception e) {
    //         Debug.Log(e.Message);
    //     }
    //     return arrlist;
    // }

}
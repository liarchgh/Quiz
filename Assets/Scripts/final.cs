using UnityEngine;
using UnityEngine.UI;

public class final : all_panel {
    public Text finalScore;
    public ans_ques aq;

	// Use this for initialization
	void OnEnable () {
        quiz.SetActive(false);
        ans.SetActive(false);
        finalScore.text = "你答对了" + aq.getCor() + "题";
	}
	
	// Update is called once per frame
	void Update () {
            //按键进入 重新开始/返回主菜单
            if (Input.GetKeyDown(now_key[5])) {
                Retry();
            }
            if (Input.GetKeyDown(now_key[4])) {
                ReturnToMenu();
            }
	}

    //重新开始
    public void Retry() {
		aq.quiz_begin(aq.quiz_type);
    }

    public void ReturnToMenu() {
        welcome.SetActive(true);
    }
}

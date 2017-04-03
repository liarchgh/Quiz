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
            if (Input.GetKeyDown(now_key[5])) {
                Retry();
            }
            if (Input.GetKeyDown(now_key[4])) {
                ReturnToMenu();
            }
	}

    public void Retry() {
        //ActivitatePanel(score, false);
        //switch (what) {
        //case competition:
        //    OnCompetition();
        //    break;
        //case classic:
        //    OnClassic();
        //    break;
        //case time:
        //    OnTimeChallenge();
        //    break;
        //}
		aq.quiz_begin(aq.quiz_type);
    }

    public void ReturnToMenu() {
        welcome.SetActive(true);
        //ActivitatePanel(welcome, true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wel : all_panel {
	public Button up_file;
	// Use this for initialization
	void OnEnable () {
		score.SetActive(false);
		quiz.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(now_key[0])) {
			//Debug.Log(scr_aq.quesOfOther);
            scr_aq.quiz_begin(scr_aq.getTypeCom());
        }
        if (Input.GetKeyDown(now_key[1])) {
            scr_aq.quiz_begin(scr_aq.getTypeCla());
            // OnClassic();
        }
        if (Input.GetKeyDown(now_key[2])) {
            scr_aq.quiz_begin(scr_aq.getTypeTl());
            // OnTimeChallenge();
        }
        if (Input.GetKeyDown(now_key[3])) {
            scr_ch.up();
            // OnTimeChallenge();
        }
	}
}

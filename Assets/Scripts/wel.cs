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
        //不同按键进入不同答题模式
        if (Input.GetKeyDown(now_key[0])) {
            scr_aq.quiz_begin(scr_aq.getTypeCom());
        }
        if (Input.GetKeyDown(now_key[1])) {
            scr_aq.quiz_begin(scr_aq.getTypeCla());
        }
        if (Input.GetKeyDown(now_key[2])) {
            scr_aq.quiz_begin(scr_aq.getTypeTl());
        }
        if (Input.GetKeyDown(now_key[3])) {
            scr_ch.up();
        }
	}
}

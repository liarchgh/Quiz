﻿using UnityEngine;

public class choose_control : all_panel {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//不同按键进入不同手柄操作模式
		if(Input.GetKeyDown(KeyCode.Joystick1Button3)){
			now_key = key_vrbox;
			next();
		}
		if(Input.GetKeyDown(KeyCode.Joystick1Button10)){
			now_key = key_gamepad;
			next();
		}
		if(Input.GetKeyDown(KeyCode.Joystick1Button0)){
			now_key = key_remote;
			next();
		}
		if(Input.GetKeyDown(KeyCode.A)){
			now_key = key_kb;
			next();
		}
	}

	//开始 关闭本panel 打开答题面板
	public void next(){
		welcome.SetActive(true);
		choose.SetActive(false);
	}
}

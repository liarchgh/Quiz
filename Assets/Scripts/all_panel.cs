using UnityEngine;
using Assets.Scripts;
using System.Collections.Generic;
public class all_panel : MonoBehaviour {
    public static GameObject welcome, quiz, ans, score, circle, choose;
    public static CanvasHandler scr_ch;
    public static ans_ques scr_aq;
    public List<QuestData> quesOfCompe, quesOfOther ;
    public static KeyCode[] key_vrbox = {KeyCode.Joystick1Button3, KeyCode.Joystick1Button0, KeyCode.Joystick1Button2, KeyCode.Joystick1Button1, KeyCode.Joystick1Button4, KeyCode.Joystick1Button5},//A, B, C, D, UP, DOWN
    key_gamepad = {KeyCode.Joystick1Button10, KeyCode.Joystick1Button5, KeyCode.Joystick1Button2, KeyCode.Joystick1Button3, KeyCode.Joystick1Button7, KeyCode.Joystick1Button11},//A, B, X, Y, OK, III
    key_remote = {KeyCode.Joystick1Button0, KeyCode.Joystick1Button1, KeyCode.Joystick1Button2, KeyCode.Joystick1Button3, KeyCode.Joystick1Button11, KeyCode.Joystick1Button10},//A, B, X, Y, Select, Start
    key_kb = {KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Q, KeyCode.W}, //键盘的 调试用
    now_key;
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using UnityEngine;
using UnityEngine.UI;

public class AnsBtnId : MonoBehaviour {
    public int ID;
    public ans_ques c;
    public Text txt; //按钮的文本
    public string t2;
    public Button btn;
    public Image img;

    public void Init (string t) { //按钮的初始化函数
        t2 = t;
        txt.text = t;
        img.color = Color.white;
        btn.interactable = true;
    }

    public void OnPressed () { //选项按钮按下时触发的函数
        c.Judge (this);
    }
}
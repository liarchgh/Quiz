using UnityEngine;
using UnityEngine.UI;

public class AnsBtnId : MonoBehaviour {
    public int ID;
    public ans_ques c;
    public Text txt;
    public string t2;
    public Button btn;
    public Image img;

    void Awake () { //其在实例化是调用 是在start()之前的
	}

    public void Init (string t) {
        t2 = t;
        txt.text = t;
        img.color = Color.white;
        btn.interactable = true;
    }

    public void OnPressed () {
        c.Judge (this);
    }
}
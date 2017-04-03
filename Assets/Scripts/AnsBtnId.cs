using UnityEngine;
using UnityEngine.UI;

public class AnsBtnId : MonoBehaviour {
    public int ID;
    public ans_ques c;
    public Text txt;
    public string t2;
    public Button btn;
    public Image img;
	private Vector3 begin;
	private Vector3 pos_long = new Vector3 (1000, 1000, 0);

    void Awake () { //其在实例化是调用 是在start()之前的
		begin = btn.transform.localPosition;
	}

    public void Init (string t) {
        t2 = t;
        txt.text = t;
        img.color = Color.white;
        btn.interactable = true;
    }

    public void setActive (bool state) {
        if (!state) { btn.transform.localPosition = pos_long; }
		else {btn.transform.localPosition = begin;}
    }

    public void OnPressed () {
        c.Judge (this);
    }
}
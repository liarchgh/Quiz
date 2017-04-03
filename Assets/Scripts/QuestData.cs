namespace Assets.Scripts {
    public class QuestData {
        public const int ty_choose = 0, ty_pic = 1, ty_judge = 2;
        public int type, op_num;
        public string quest; //题目
        public string[] answer; //存所有的选项
        public int ans; //正确答案
        public QuestData(int l) {
            answer = new string[l];
        }
    }
}

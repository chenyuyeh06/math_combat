using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace math_combat
{
    public static class GameUnits
    {
        public static HomePage homePage { get; private set; }

        public static void Initialize()
        {
            InitializeAllCards();       // 先初始化資料
            homePage = new HomePage();  // 再建立 Form
        }

        // 讓其他頁面可以直接存取彼此（不需要在建構子裡傳來傳去）
        public static RoomPage roomPage = new RoomPage(homePage);
        public static GamePage gamePage = new GamePage(homePage);
        public static ResultPage resultPage = new ResultPage(homePage);


        // 玩家輸入
        public static int vulume_sfx = 60;
        public static int volume_bgm = 60;
        public static string player_name;
        public static string room_number;
        
        //遊戲控制
        public static int rounds = 3;
        public static int secs = 5;

        public static void MakeRoundedControl(Control ctrl, int radius)
        {
            ctrl.Paint += (sender, e) =>
            {
                Control c = (Control)sender;
                int r = radius;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                GraphicsPath path = new GraphicsPath();
                path.StartFigure();
                path.AddArc(0, 0, 2 * r, 2 * r, 180, 90);
                path.AddArc(c.Width - 2 * r, 0, 2 * r, 2 * r, 270, 90);
                path.AddArc(c.Width - 2 * r, c.Height - 2 * r, 2 * r, 2 * r, 0, 90);
                path.AddArc(0, c.Height - 2 * r, 2 * r, 2 * r, 90, 90);
                path.CloseFigure();

                c.Region = new Region(path);
            };
        }

        // 滑鼠懸停變色
        public static void MakeFancyControl(Control ctrl, int radius, Color normalColor, Color hoverColor)
        {
            // 先套用圓角效果
            MakeRoundedControl(ctrl, radius);

            // 設定預設字體顏色
            ctrl.ForeColor = normalColor;

            // 滑鼠滑入
            ctrl.MouseEnter += (sender, e) =>
            {
                ctrl.ForeColor = hoverColor;
            };

            // 滑鼠離開
            ctrl.MouseLeave += (sender, e) =>
            {
                ctrl.ForeColor = normalColor;
            };
        }

        //字體載入


        //視窗切換
        public static void SwitchToForm(Form currentForm, Form targetForm)
        {
            if (currentForm == null || targetForm == null) return;

            // 設定新視窗的位置與大小同步
            targetForm.StartPosition = FormStartPosition.Manual;
            targetForm.Location = currentForm.Location;
            targetForm.Size = currentForm.Size;

            // 讓新視窗記住原本的owner是誰
            // 如果目前視窗本身就有owner就把owner傳承給新視窗
            if (currentForm.Owner != null)
            {
                targetForm.Owner = currentForm.Owner;
            }
            else
            {
                // 如果目前視窗沒有owner那自己就是新視窗的owner
                targetForm.Owner = currentForm;
            }

            targetForm.Show();
            currentForm.Hide();
        }

        // 1. 定義卡牌類型（全域可用）
        public enum CardType { Number, Operator }

        // 2. 定義卡牌資料結構（全域可用）
        public class Card
        {
            public int Id { get; set; }
            public CardType Type { get; set; }
            public string Value { get; set; }
            public Image CardImage { get; set; }
        }

        // 牌庫
        public static List<Card> CardDatabase { get; private set; } = new List<Card>();


        // 初始化這 14 張標準卡牌，在 HomePage 啟動時呼叫一次即可
        public static void InitializeAllCards()
        {
            CardDatabase.Clear();

            // 數字牌：card_0, card_1 ... card_9
            for (int i = 0; i <= 9; i++)
            {
                CardDatabase.Add(new Card
                {
                    Id = i,
                    Type = CardType.Number,
                    Value = i.ToString(),
                    CardImage = (Image)Properties.Resources.ResourceManager.GetObject($"card_{i}")
                });
            }

            // 運算子牌：名稱對應你 Resource 裡的實際命名
            var operators = new[]
            {
                (value: "+", name: "card_plus"),
                (value: "-", name: "card_sub"),
                (value: "*", name: "card_mul"),
                (value: "/", name: "card_div"),
            };
            int id = 10;
            foreach (var op in operators)
            {
                CardDatabase.Add(new Card
                {
                    Id = id++,
                    Type = CardType.Operator,
                    Value = op.value,
                    CardImage = (Image)Properties.Resources.ResourceManager.GetObject(op.name)
                });
            }
        }

        // TODO: 從牌庫隨機抽取 5 張牌組成玩家手牌，確保不重複且符合規則（3 數字 + 2 運算子）
        public static List<Card> CreateGameHand()
        {
            var random = new Random();
            var hand = new List<Card>();

            // 從數字牌（Id 0-9）隨機抽 3 張，不重複
            var numberCards = CardDatabase
                .Where(c => c.Type == CardType.Number)
                .OrderBy(_ => random.Next())
                .Take(3)
                .ToList();

            // 從運算子牌（+−×÷）隨機抽 2 張，不重複
            var operatorCards = CardDatabase
                .Where(c => c.Type == CardType.Operator)
                .OrderBy(_ => random.Next())
                .Take(2)
                .ToList();

            // 合併後再洗牌，讓手牌順序不固定
            hand.AddRange(numberCards);
            hand.AddRange(operatorCards);
            hand = hand.OrderBy(_ => random.Next()).ToList();

            return hand;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace math_combat
{
    public static class GameUnits
    {
        // ── Pages ──────────────────────────────────────────────────────────────
        public static HomePage   homePage   { get; private set; }
        public static RoomPage   roomPage   = null;   // 在 Initialize() 後建立
        public static GamePage   gamePage   = null;
        public static ResultPage resultPage = null;

        public enum PageState
        {
            Home,
            Room,
            Game,
            Result
        }
        public static PageState currentPage = PageState.Home;

        public static void Initialize()
        {
            InitializeAllCards();
            homePage   = new HomePage();
            roomPage   = new RoomPage(homePage);
            gamePage   = new GamePage(homePage);
            resultPage = new ResultPage(homePage);
        }

        // ── JSON 序列化工具 ────────────────────────────────────────────────────
        private static readonly JavaScriptSerializer _json = new JavaScriptSerializer();

        // ── 玩家輸入 ───────────────────────────────────────────────────────────
        public static int    vulume_sfx  = 60;
        public static int    volume_bgm  = 60;
        public static string player_name;
        public static string room_number;   // 房間號碼（字串，即 roomId）

        // ── Server 連線 ────────────────────────────────────────────────────────
        private const  string SERVER_IP   = "127.0.0.1";
        private const  int    SERVER_PORT = 9000;

        private static TcpClient    _tcp;
        private static NetworkStream _netStream;
        private static StreamReader  _reader;
        private static StreamWriter  _writer;
        public  static bool isConnected = false;

        // ── 身分 ───────────────────────────────────────────────────────────────
        /// <summary>true = 建立房間的 Player1（房主）</summary>
        public static bool isHost      = false;
        /// <summary>true = 觀戰者</summary>
        public static bool isSpectator = false;

        // ── 房間狀態 ───────────────────────────────────────────────────────────
        public static string       hostName       = "";
        public static string       player2Name    = "";
        public static bool         hostReady      = false;  // Player1 是否已送 START_GAME
        public static bool         clientReady    = false;  // Player2 是否已送 READY
        public static List<string> spectatorNames = new List<string>();

        // ── 遊戲設定 ───────────────────────────────────────────────────────────
        public static int rounds = 3;
        public static int secs   = 30;

        // ── 遊戲進行狀態 ───────────────────────────────────────────────────────
        public static int    currentRound = 0;
        public static int    wins1        = 0;
        public static int    wins2        = 0;
        public static bool   opponentDisconnected = false;

        // ── 本回合牌組（Server 下發） ──────────────────────────────────────────
        public static List<string> currentCards = new List<string>();

        // ══════════════════════════════════════════════════════════════════════
        //  連線 / 斷線
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// 連到中央 Server（127.0.0.1:9000），然後送 CREATE_ROOM 或 JOIN_ROOM。
        /// roomId = 使用者輸入的房間號碼（數字字串）。
        /// 回傳 true 表示 TCP 連線成功（不代表房間操作成功，結果透過回呼通知 UI）。
        /// </summary>
        public static async Task<bool> StartNetwork(string roomId, bool create)
        {
            room_number = roomId;
            try
            {
                _tcp = new TcpClient();
                var connectTask = _tcp.ConnectAsync(SERVER_IP, SERVER_PORT);

                if (await Task.WhenAny(connectTask, Task.Delay(3000)) != connectTask)
                    throw new TimeoutException("無法連到 Server（逾時）");

                await connectTask; // 拋出連線失敗的例外

                _netStream = _tcp.GetStream();
                _reader    = new StreamReader(_netStream, Encoding.UTF8);
                _writer    = new StreamWriter(_netStream, Encoding.UTF8) { AutoFlush = true };
                isConnected = true;

                // 啟動背景接收迴圈
                var currentTcp = _tcp;
                var currentReader = _reader;
                _ = Task.Run(() => ReceiveLoop(currentTcp, currentReader));

                // 送出建立 / 加入訊息
                if (create)
                {
                    isHost = true;
                    SendJson(new CreateRoomMsg
                    {
                        name    = player_name,
                        roomId  = roomId,
                        rounds  = rounds,
                        seconds = secs
                    });
                }
                else
                {
                    isHost = false;
                    SendJson(new JoinRoomMsg
                    {
                        name   = player_name,
                        roomId = roomId
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                CleanupNetwork();
                ShowError("連線失敗：" + ex.Message);
                return false;
            }
        }

        /// <summary>送出任意可序列化物件（加換行）</summary>
        public static void SendJson(object msg)
        {
            if (_writer == null) return;
            try
            {
                string json = _json.Serialize(msg);
                _writer.WriteLine(json);
            }
            catch { }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  接收迴圈
        // ══════════════════════════════════════════════════════════════════════

        private static async Task ReceiveLoop(TcpClient client, StreamReader reader)
        {
            try
            {
                while (isConnected && _tcp == client && reader != null)
                {
                    string line = await reader.ReadLineAsync();
                    if (line == null) break; // Server 斷線
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    ProcessMessage(line);
                }
            }
            catch { }
            finally
            {
                HandleDisconnect(client);
            }
        }

        private static void ProcessMessage(string json)
        {
            try
            {
                // 先只解 type 欄位，再根據 type 完整反序列化
                var baseMsg = _json.Deserialize<NetworkMessage>(json);
                if (baseMsg == null || string.IsNullOrEmpty(baseMsg.type)) return;

                switch (baseMsg.type)
                {
                    // ── 房間建立 / 加入 ──────────────────────────────────────
                    case "ROOM_CREATED":
                    {
                        var m = _json.Deserialize<RoomCreatedMsg>(json);
                        room_number = m.roomId;
                        // 切換到 RoomPage（用 homePage.BeginInvoke，它是目前可見視窗）
                        homePage?.BeginInvoke(new Action(() =>
                            SwitchToForm(homePage, roomPage)));
                        break;
                    }

                    case "ROOM_JOINED":
                    {
                        var m = _json.Deserialize<RoomJoinedMsg>(json);
                        isSpectator = (m.role == "spectator");

                        if (m.role == "player")
                        {
                            // 對手名稱 = opponent
                            if (isHost)
                            {
                                player2Name = m.opponent ?? "";
                            }
                            else
                            {
                                hostName    = m.opponent ?? "";
                                player2Name = player_name;
                            }
                            rounds = m.rounds;
                            secs   = m.seconds;
                        }
                        else // spectator
                        {
                            hostName    = m.player1 ?? "";
                            player2Name = m.player2 ?? "";
                            currentRound = m.currentRound;
                            wins1        = m.wins1;
                            wins2        = m.wins2;
                        }

                        // 如果已經在 RoomPage，直接更新 UI；否則切換視窗
                        if (currentPage == PageState.Room)
                        {
                            UpdateRoomUI();
                        }
                        else
                        {
                            Form activeForm = GetActiveForm();
                            activeForm?.BeginInvoke(new Action(() =>
                                SwitchToForm(activeForm, roomPage)));
                        }
                        break;
                    }

                    // ── 房間資訊更新 ─────────────────────────────────────────
                    case "ROOM_INFO":
                    {
                        var m = _json.Deserialize<RoomInfoMsg>(json);
                        hostName       = m.player1 ?? "";
                        player2Name    = m.player2 ?? "";
                        spectatorNames = m.spectators ?? new List<string>();
                        
                        // 動態更新是否為觀戰者與房主的角色狀態
                        isSpectator = spectatorNames.Contains(player_name);
                        isHost      = (!isSpectator && hostName == player_name);

                        UpdateRoomUI();
                        break;
                    }

                    case "ROOM_UPDATED":
                    {
                        var m = _json.Deserialize<RoomUpdatedMsg>(json);
                        rounds = m.rounds;
                        secs   = m.seconds;
                        UpdateRoomUI();
                        break;
                    }

                    // ── 準備狀態 ─────────────────────────────────────────────
                    case "READY_STATE":
                    {
                        var m = _json.Deserialize<ReadyStateMsg>(json);
                        hostReady   = m.player1Ready;
                        clientReady = m.player2Ready;
                        UpdateRoomUI();
                        break;
                    }

                    // ── 遊戲開始 ─────────────────────────────────────────────
                    case "GAME_START":
                    {
                        var m = _json.Deserialize<GameStartMsg>(json);
                        rounds = m.rounds;
                        secs   = m.seconds;
                        wins1  = 0;
                        wins2  = 0;
                        roomPage?.BeginInvoke(new Action(() =>
                            SwitchToForm(roomPage, gamePage)));
                        break;
                    }

                    // ── 回合開始（Server 下發牌組）──────────────────────────
                    case "ROUND_START":
                    {
                        if (currentPage == PageState.Result || currentPage == PageState.Home) break;
                        var m = _json.Deserialize<RoundStartMsg>(json);
                        currentRound = m.round;
                        currentCards = m.cards ?? new List<string>();
                        gamePage?.BeginInvoke(new Action(() =>
                            gamePage.OnRoundStart(m.round, m.seconds, currentCards)));
                        break;
                    }

                    // ── 回合結算 ─────────────────────────────────────────────
                    case "ROUND_RESULT":
                    {
                        if (currentPage == PageState.Result || currentPage == PageState.Home) break;
                        var m = _json.Deserialize<RoundResultMsg>(json);
                        wins1 = m.wins1;
                        wins2 = m.wins2;
                        gamePage?.BeginInvoke(new Action(() =>
                            gamePage.OnRoundResult(m)));
                        break;
                    }

                    // ── 進入下一回合 ─────────────────────────────────────────
                    case "NEXT_ROUND":
                    {
                        // ROUND_START 會緊接著來，不需額外動作
                        break;
                    }

                    // ── 遊戲結束 ─────────────────────────────────────────────
                    case "GAME_OVER":
                    {
                        var m = _json.Deserialize<GameOverMsg>(json);
                        wins1 = m.wins1;
                        wins2 = m.wins2;
                        gamePage?.BeginInvoke(new Action(() =>
                            gamePage.OnGameOver(m)));
                        break;
                    }

                    // ── 回到大廳後的房間重置 ─────────────────────────────────
                    case "ROOM_RESET":
                    {
                        var m = _json.Deserialize<RoomResetMsg>(json);
                        hostName       = m.player1 ?? "";
                        player2Name    = m.player2 ?? "";
                        spectatorNames = m.spectators ?? new List<string>();
                        rounds         = m.rounds;
                        secs           = m.seconds;
                        hostReady      = false;
                        clientReady    = false;

                        // 重置回到等待室時，動態更新是否為觀戰者與房主的角色狀態
                        isSpectator = spectatorNames.Contains(player_name);
                        isHost      = (!isSpectator && hostName == player_name);

                        if (currentPage == PageState.Game)
                        {
                            // 如果仍在遊戲進行中（例如遊戲被中斷或強行重置），則返回房間
                            gamePage?.BeginInvoke(new Action(() =>
                                SwitchToForm(gamePage, roomPage)));
                        }
                        else if (currentPage == PageState.Room)
                        {
                            UpdateRoomUI();
                        }
                        // 如果目前已經在 ResultPage 結算畫面，則不主動切換視窗！
                        // 讓玩家在結算頁面各自點選「回到房間」或「回到首頁」！
                        break;
                    }

                    // ── 玩家離開 ─────────────────────────────────────────────
                    case "PLAYER_LEFT":
                    {
                        var m = _json.Deserialize<PlayerLeftMsg>(json);
                        if (currentPage == PageState.Game)
                        {
                            opponentDisconnected = true;
                        }
                        else
                        {
                            ShowInfo($"玩家「{m.name}」已離開房間。");
                            UpdateRoomUI();
                        }
                        break;
                    }

                    // ── 觀戰者人數更新 ───────────────────────────────────────
                    case "SPECTATOR_JOINED":
                    {
                        var m = _json.Deserialize<SpectatorJoinedMsg>(json);
                        if (!spectatorNames.Contains(m.name))
                            spectatorNames.Add(m.name);
                        UpdateRoomUI();
                        break;
                    }

                    case "SPECTATOR_LEFT":
                    {
                        var m = _json.Deserialize<SpectatorLeftMsg>(json);
                        spectatorNames.Remove(m.name);
                        UpdateRoomUI();
                        break;
                    }

                    // ── 遊戲中止 ─────────────────────────────────────────────
                    case "GAME_ABORTED":
                    {
                        var m = _json.Deserialize<GameAbortedMsg>(json);
                        Form activeForm = GetActiveForm();
                        activeForm?.BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show(m.message ?? "對戰已中止。", "對戰中止",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            SwitchToForm(activeForm, roomPage);
                        }));
                        break;
                    }

                    // ── 錯誤 ─────────────────────────────────────────────────
                    case "ERROR":
                    {
                        var m = _json.Deserialize<ErrorMsg>(json);
                        HandleServerError(m);
                        break;
                    }
                }
            }
            catch { /* 忽略格式不良的訊息 */ }
        }

        private static void HandleServerError(ErrorMsg m)
        {
            string text = m.message ?? m.code;

            switch (m.code)
            {
                case "ROOM_NOT_FOUND":
                    // 找不到房間 → 詢問是否要建立
                    homePage?.BeginInvoke(new Action(async () =>
                    {
                        var dlg = MessageBox.Show(
                            $"找不到房間「{room_number}」。\n要建立這個房間嗎？",
                            "房間不存在",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (dlg == DialogResult.Yes)
                        {
                            // 重新連線，這次送 CREATE_ROOM
                            CleanupNetwork();
                            await StartNetwork(room_number, create: true);
                        }
                        else
                        {
                            CleanupNetwork();
                        }
                    }));
                    break;

                case "ROOM_ID_IN_USE":
                case "ROOM_FULL":
                    homePage?.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show(text, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (currentPage == PageState.Room)
                            SwitchToForm(roomPage, homePage);
                        homePage.ResetToRoomInput();
                    }));
                    CleanupNetwork();
                    break;

                case "NAME_IN_USE":
                    homePage?.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show(text, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (currentPage == PageState.Room)
                            SwitchToForm(roomPage, homePage);
                        homePage.ResetToNameInput();
                    }));
                    CleanupNetwork();
                    break;

                default:
                    ShowError(text);
                    break;
            }
        }

        private static void HandleDisconnect(TcpClient client)
        {
            // 如果斷線的 client 不是目前正活躍的 client，代表這是舊連線的結束，直接忽略
            if (_tcp != client) return;

            if (!isConnected) return;
            isConnected = false;

            if (currentPage == PageState.Game)
            {
                opponentDisconnected = true;
            }
            else
            {
                ShowInfo("與 Server 的連線已中斷。");
                Form activeForm = GetActiveForm();
                activeForm?.BeginInvoke(new Action(() =>
                    SwitchToForm(activeForm, homePage)));
            }
            CleanupNetwork();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  公開動作 API（由 UI 呼叫）
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>房主送出 START_GAME</summary>
        public static void SendStartGame()
        {
            if (!isHost || !isConnected) return;
            SendJson(new StartGameMsg());
        }

        /// <summary>Player2 切換準備狀態，並通知 Server</summary>
        public static void ToggleClientReady()
        {
            if (isSpectator || !isConnected) return;
            clientReady = !clientReady;
            SendJson(clientReady ? (object)new ReadyMsg() : new UnreadyMsg());
        }

        /// <summary>送出本回合得分給 Server</summary>
        public static void SendScore(int round, double score)
        {
            if (!isConnected) return;
            SendJson(new SubmitScoreMsg { round = round, score = score });
        }

        /// <summary>遊戲結束後回到大廳</summary>
        public static void SendReturnRoom()
        {
            SendJson(new ReturnRoomMsg());
        }

        /// <summary>遊戲結束後直接離開</summary>
        public static void SendLeaveAfterGame()
        {
            isConnected = false; // 避免背景接收執行緒誤判為非自願斷線而跳出中斷對話框
            try { SendJson(new LeaveAfterGameMsg()); } catch { }
            CleanupNetwork();
        }

        /// <summary>更新房間設定（只有 Player1 可送）</summary>
        public static void SendUpdateRoom()
        {
            if (!isHost || !isConnected) return;
            SendJson(new UpdateRoomMsg { rounds = rounds, seconds = secs });
        }

        /// <summary>通知 Server 離開，然後清理本機連線</summary>
        public static void SendLeaveAndCleanup()
        {
            isConnected = false; // 避免背景接收執行緒誤判為非自願斷線而跳出中斷對話框
            try { SendJson(new LeaveMsg()); } catch { }
            CleanupNetwork();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  清理
        // ══════════════════════════════════════════════════════════════════════

        public static void CleanupNetwork()
        {
            isConnected          = false;
            isHost               = false;
            isSpectator          = false;
            hostReady            = false;
            clientReady          = false;
            opponentDisconnected = false;
            hostName             = "";
            player2Name          = "";
            spectatorNames.Clear();
            currentCards.Clear();
            wins1        = 0;
            wins2        = 0;
            currentRound = 0;

            try { _writer?.Close(); }    catch { }
            try { _reader?.Close(); }    catch { }
            try { _netStream?.Close(); } catch { }
            try { _tcp?.Close(); }       catch { }

            _writer    = null;
            _reader    = null;
            _netStream = null;
            _tcp       = null;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  UI 輔助
        // ══════════════════════════════════════════════════════════════════════

        private static void UpdateRoomUI()
        {
            roomPage?.BeginInvoke(new Action(() => roomPage.RefreshRoomInfo()));
        }

        private static void ShowError(string msg)
        {
            Form target = currentPage == PageState.Game ? (Form)gamePage :
                          currentPage == PageState.Room ? (Form)roomPage : homePage;
            target?.BeginInvoke(new Action(() =>
                MessageBox.Show(msg, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error)));
        }

        private static void ShowInfo(string msg)
        {
            Form target = currentPage == PageState.Game ? (Form)gamePage :
                          currentPage == PageState.Room ? (Form)roomPage : homePage;
            target?.BeginInvoke(new Action(() =>
                MessageBox.Show(msg, "通知", MessageBoxButtons.OK, MessageBoxIcon.Information)));
        }

        // ══════════════════════════════════════════════════════════════════════
        //  視窗切換
        // ══════════════════════════════════════════════════════════════════════

        public static Form GetActiveForm()
        {
            if (currentPage == PageState.Game) return gamePage;
            if (currentPage == PageState.Result) return resultPage;
            if (currentPage == PageState.Room) return roomPage;
            return homePage;
        }

        public static void SwitchToForm(Form currentForm, Form targetForm)
        {
            if (currentForm == null || targetForm == null) return;
            targetForm.StartPosition = FormStartPosition.Manual;
            targetForm.Location      = currentForm.Location;
            targetForm.Size          = currentForm.Size;

            // 避免視窗擁有權限產生循環引用（例如主視窗擁有自己），這會導致 WinForms 內部佈局當機或死循環
            if (targetForm == homePage)
            {
                targetForm.Owner = null;
            }
            else
            {
                if (currentForm.Owner != null && currentForm.Owner != targetForm)
                {
                    targetForm.Owner = currentForm.Owner;
                }
                else
                {
                    targetForm.Owner = currentForm;
                }
            }

            if (targetForm is HomePage)        currentPage = PageState.Home;
            else if (targetForm is RoomPage)   currentPage = PageState.Room;
            else if (targetForm is GamePage)   currentPage = PageState.Game;
            else if (targetForm is ResultPage) currentPage = PageState.Result;

            targetForm.Show();
            currentForm.Hide();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  UI 控制元件裝飾
        // ══════════════════════════════════════════════════════════════════════

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

        public static void MakeFancyControl(Control ctrl, int radius, Color normalColor, Color hoverColor)
        {
            MakeRoundedControl(ctrl, radius);
            ctrl.ForeColor = normalColor;
            ctrl.MouseEnter += (s, e) => ctrl.ForeColor = hoverColor;
            ctrl.MouseLeave += (s, e) => ctrl.ForeColor = normalColor;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  卡牌系統
        // ══════════════════════════════════════════════════════════════════════

        public enum CardType { Number, Operator }

        public class Card
        {
            public int      Id        { get; set; }
            public CardType Type      { get; set; }
            public string   Value     { get; set; }
            public Image    CardImage { get; set; }
        }

        public static List<Card> CardDatabase { get; private set; } = new List<Card>();

        public static void InitializeAllCards()
        {
            CardDatabase.Clear();

            for (int i = 0; i <= 9; i++)
            {
                CardDatabase.Add(new Card
                {
                    Id        = i,
                    Type      = CardType.Number,
                    Value     = i.ToString(),
                    CardImage = (Image)Properties.Resources.ResourceManager.GetObject($"card_{i}")
                });
            }

            var operators = new[]
            {
                (value: "+",  name: "card_plus"),
                (value: "-",  name: "card_sub"),
                (value: "*",  name: "card_mul"),
                (value: "/",  name: "card_div"),
            };
            int id = 10;
            foreach (var op in operators)
            {
                CardDatabase.Add(new Card
                {
                    Id        = id++,
                    Type      = CardType.Operator,
                    Value     = op.value,
                    CardImage = (Image)Properties.Resources.ResourceManager.GetObject(op.name)
                });
            }
        }

        /// <summary>
        /// 根據 Server 下發的 cards 字串列表，建立對應的 Card 物件列表。
        /// </summary>
        public static List<Card> BuildHandFromCards(List<string> cardValues)
        {
            var hand = new List<Card>();
            if (cardValues == null) return hand;
            foreach (var val in cardValues)
            {
                Card found = CardDatabase.FirstOrDefault(c => c.Value == val);
                if (found != null)
                    hand.Add(found);
            }
            return hand;
        }

        /// <summary>
        /// 本機隨機產生手牌（供沒有 Server 時測試用）。
        /// </summary>
        public static List<Card> CreateGameHand()
        {
            var rng   = new Random();
            var nums  = CardDatabase.Where(c => c.Type == CardType.Number)
                                    .OrderBy(_ => rng.Next()).Take(3).ToList();
            var ops   = CardDatabase.Where(c => c.Type == CardType.Operator)
                                    .OrderBy(_ => rng.Next()).Take(2).ToList();
            var hand  = nums.Concat(ops).OrderBy(_ => rng.Next()).ToList();
            return hand;
        }

        // 音量設定
        public static int vulume_sfx_ref => vulume_sfx;
        public static int volume_bgm_ref => volume_bgm;
    }
}

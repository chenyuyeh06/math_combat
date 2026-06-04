using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        
        // 網路通訊變數
        public static int port;
        public static bool isHost;
        public static TcpListener listener;
        
        // Host 端管理連線
        public static List<TcpClient> clientList = new List<TcpClient>();
        public static List<string> clientNames = new List<string>();
        
        // Client 端連線
        public static TcpClient client;
        public static NetworkStream stream;
        
        // 暫存資料與狀態
        public static string hostName = "";
        public static string player2Name = "";
        public static bool isSpectator = false;
        public static bool isConnected = false;
        
        // 準備狀態
        public static bool hostReady = false;
        public static bool clientReady = false;
        
        // 斷線延遲處理與遞補變數
        public static bool opponentDisconnected = false;
        public static bool needHandoff = false;
        public static List<string> lastRoomInfoNames = new List<string>();
        
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

        // 啟動網路連線：優先嘗試連線為 Client，失敗則作為 Host 開啟監聽
        public static async Task<bool> StartNetwork(int portVal)
        {
            port = portVal;
            try
            {
                client = new TcpClient();
                var connectTask = client.ConnectAsync("127.0.0.1", portVal);
                if (await Task.WhenAny(connectTask, Task.Delay(1500)) == connectTask)
                {
                    await connectTask;
                    isHost = false;
                    stream = client.GetStream();
                    isConnected = true;
                    isSpectator = false;
                    
                    _ = Task.Run(() => ReceiveDataLoop());
                    
                    byte[] nameBytes = Encoding.UTF8.GetBytes("NAME:" + player_name);
                    await stream.WriteAsync(nameBytes, 0, nameBytes.Length);
                    
                    return true;
                }
                else
                {
                    client.Close();
                    throw new TimeoutException();
                }
            }
            catch
            {
                try
                {
                    if (client != null) client.Close();
                    
                    isHost = true;
                    isConnected = false;
                    clientList.Clear();
                    clientNames.Clear();
                    
                    listener = new TcpListener(IPAddress.Any, portVal);
                    listener.Start();
                    
                    _ = Task.Run(() => AcceptClientLoop());
                    
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("無法在 Port " + portVal + " 開啟監聽: " + ex.Message, "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CleanupNetwork();
                    return false;
                }
            }
        }

        private static async Task AcceptClientLoop()
        {
            try
            {
                while (listener != null)
                {
                    var acceptedClient = await listener.AcceptTcpClientAsync();
                    bool roomFull = false;
                    lock (clientList)
                    {
                        if (clientList.Count >= 7) // 1 Host + 7 Clients = 8 players max
                        {
                            roomFull = true;
                        }
                    }

                    if (roomFull)
                    {
                        try
                        {
                            byte[] fullBytes = Encoding.UTF8.GetBytes("ROOM_FULL");
                            var s = acceptedClient.GetStream();
                            s.Write(fullBytes, 0, fullBytes.Length);
                        }
                        catch {}
                        acceptedClient.Close();
                    }
                    else
                    {
                        lock (clientList)
                        {
                            clientList.Add(acceptedClient);
                        }
                        _ = Task.Run(() => ReceiveClientDataLoop(acceptedClient));
                    }
                }
            }
            catch {}
        }

        private static async Task ReceiveClientDataLoop(TcpClient c)
        {
            byte[] buffer = new byte[1024];
            var s = c.GetStream();
            try
            {
                while (listener != null && c.Connected)
                {
                    int bytesRead = await s.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;
                    
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    ProcessHostReceivedData(c, data);
                }
            }
            catch {}
            finally
            {
                HandleClientDisconnect(c);
            }
        }

        private static void ProcessHostReceivedData(TcpClient c, string data)
        {
            if (data.StartsWith("NAME:"))
            {
                string name = data.Substring(5);
                lock (clientList)
                {
                    int idx = clientList.IndexOf(c);
                    if (idx >= 0)
                    {
                        while (clientNames.Count <= idx)
                        {
                            clientNames.Add("");
                        }
                        clientNames[idx] = name;
                    }
                }
                BroadcastRoomInfo();
                BroadcastReadyState();
            }
            else if (data == "READY")
            {
                lock (clientList)
                {
                    int idx = clientList.IndexOf(c);
                    if (idx == 0)
                    {
                        clientReady = true;
                        CheckStartGame();
                    }
                }
            }
            else if (data == "UNREADY")
            {
                lock (clientList)
                {
                    int idx = clientList.IndexOf(c);
                    if (idx == 0)
                    {
                        clientReady = false;
                        BroadcastReadyState();
                    }
                }
            }
        }

        public static void Broadcast(string message)
        {
            if (!isHost) return;
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            lock (clientList)
            {
                foreach (var c in clientList)
                {
                    try
                    {
                        if (c.Connected)
                        {
                            var s = c.GetStream();
                            s.Write(bytes, 0, bytes.Length);
                        }
                    }
                    catch {}
                }
            }
        }

        public static void BroadcastRoomInfo()
        {
            if (!isHost) return;
            StringBuilder sb = new StringBuilder();
            sb.Append("ROOM_INFO:");
            sb.Append(player_name);
            
            lock (lastRoomInfoNames)
            {
                lastRoomInfoNames.Clear();
                lastRoomInfoNames.Add(player_name);
                lock (clientList)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        sb.Append("|");
                        if (i < clientNames.Count)
                        {
                            sb.Append(clientNames[i]);
                            lastRoomInfoNames.Add(clientNames[i]);
                        }
                    }
                }
            }
            Broadcast(sb.ToString());
            UpdateRoomPageUIFromInfo(player_name, clientNames);
        }

        public static void BroadcastReadyState()
        {
            if (!isHost) return;
            string msg = "READY_STATE:" + (hostReady ? "true" : "false") + "|" + (clientReady ? "true" : "false");
            Broadcast(msg);
            UpdateRoomPageUI();
        }

        public static void CheckStartGame()
        {
            if (!isHost) return;
            if (hostReady && clientReady)
            {
                Broadcast("START_GAME");
                if (roomPage != null)
                {
                    roomPage.BeginInvoke(new Action(() => {
                        SwitchToForm(roomPage, gamePage);
                    }));
                }
            }
            else
            {
                BroadcastReadyState();
            }
        }

        private static async Task ReceiveDataLoop()
        {
            byte[] buffer = new byte[1024];
            try
            {
                while (isConnected && stream != null)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        HandleDisconnect();
                        break;
                    }
                    
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    ProcessClientReceivedData(data);
                }
            }
            catch
            {
                HandleDisconnect();
            }
        }

        private static void ProcessClientReceivedData(string data)
        {
            if (data == "ROOM_FULL")
            {
                isConnected = false;
                if (client != null)
                {
                    try { client.Close(); } catch {}
                    client = null;
                }
                stream = null;
                
                if (roomPage != null)
                {
                    roomPage.BeginInvoke(new Action(() => {
                        MessageBox.Show("房間已滿員，無法進入！", "房間滿員", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        SwitchToForm(roomPage, homePage);
                    }));
                }
            }
            else if (data.StartsWith("ROOM_INFO:"))
            {
                string infoStr = data.Substring(10);
                string[] parts = infoStr.Split('|');
                
                lock (lastRoomInfoNames)
                {
                    lastRoomInfoNames.Clear();
                    foreach (var p in parts)
                    {
                        if (!string.IsNullOrEmpty(p))
                        {
                            lastRoomInfoNames.Add(p);
                        }
                    }
                }
                
                string p1 = parts[0];
                List<string> guests = new List<string>();
                for (int i = 1; i < parts.Length; i++)
                {
                    guests.Add(parts[i]);
                }
                
                int myIdx = -1;
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i] == player_name)
                    {
                        myIdx = i;
                        break;
                    }
                }
                
                if (myIdx >= 2)
                {
                    isSpectator = true;
                }
                else
                {
                    isSpectator = false;
                }
                
                UpdateRoomPageUIFromInfo(p1, guests);
            }
            else if (data.StartsWith("READY_STATE:"))
            {
                string[] parts = data.Substring(12).Split('|');
                hostReady = parts[0] == "true";
                clientReady = parts[1] == "true";
                UpdateRoomPageUI();
            }
            else if (data == "START_GAME")
            {
                if (roomPage != null)
                {
                    roomPage.BeginInvoke(new Action(() => {
                        SwitchToForm(roomPage, gamePage);
                    }));
                }
            }
            else if (data == "EXIT_GAME")
            {
                if (gamePage != null && gamePage.Visible)
                {
                    gamePage.BeginInvoke(new Action(() => {
                        SwitchToForm(gamePage, roomPage);
                    }));
                }
            }
        }

        private static void HandleClientDisconnect(TcpClient c)
        {
            lock (clientList)
            {
                int idx = clientList.IndexOf(c);
                if (idx >= 0)
                {
                    clientList.RemoveAt(idx);
                    if (idx < clientNames.Count)
                    {
                        clientNames.RemoveAt(idx);
                    }
                    
                    if (idx == 0)
                    {
                        clientReady = false;
                        if (gamePage != null && gamePage.Visible)
                        {
                            opponentDisconnected = true;
                        }
                        else
                        {
                            if (homePage != null)
                            {
                                homePage.BeginInvoke(new Action(() => MessageBox.Show("對手已中斷連線。", "連線中斷", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                            }
                        }
                    }
                }
            }
            BroadcastRoomInfo();
            BroadcastReadyState();
        }

        private static void HandleDisconnect()
        {
            isConnected = false;
            isSpectator = false;
            hostReady = false;
            clientReady = false;
            
            if (client != null)
            {
                try { client.Close(); } catch {}
                client = null;
            }
            stream = null;
            
            if (gamePage != null && gamePage.Visible)
            {
                opponentDisconnected = true;
                needHandoff = true;
            }
            else
            {
                PerformLobbyHandoff();
            }
        }

        private static void UpdateRoomPageUI()
        {
            if (roomPage != null)
            {
                roomPage.UpdateNames();
            }
        }

        private static void UpdateRoomPageUIFromInfo(string p1, List<string> guests)
        {
            if (roomPage != null)
            {
                roomPage.UpdateNamesFromInfo(p1, guests);
            }
        }

        public static void PerformLobbyHandoff()
        {
            needHandoff = false;
            if (lastRoomInfoNames.Count > 1)
            {
                string oldHost = lastRoomInfoNames[0];
                lastRoomInfoNames.RemoveAt(0); // 移除舊房主
                string nextHost = lastRoomInfoNames[0]; // 新房主名字
                
                if (player_name == nextHost)
                {
                    // 我們自動成為新房主
                    if (roomPage != null)
                    {
                        roomPage.BeginInvoke(new Action(async () => {
                            CleanupNetwork();
                            bool success = await StartNetwork(port);
                            if (success)
                            {
                                UpdateRoomPageUI();
                            }
                        }));
                    }
                }
                else
                {
                    // 重新連線新房主，靜默重試
                    if (roomPage != null)
                    {
                        roomPage.BeginInvoke(new Action(() => {
                            _ = Task.Run(async () => {
                                CleanupNetwork();
                                await Task.Delay(1500); // 稍等讓新房主先開啟 Port
                                if (roomPage != null)
                                {
                                    roomPage.BeginInvoke(new Action(async () => {
                                        bool success = await StartNetwork(port);
                                        if (success)
                                        {
                                            UpdateRoomPageUI();
                                        }
                                    }));
                                }
                            });
                        }));
                    }
                }
            }
            else
            {
                if (roomPage != null)
                {
                    roomPage.BeginInvoke(new Action(() => {
                        roomPage.UpdateNamesFromInfo("房主", new List<string>());
                    }));
                }
                if (homePage != null)
                {
                    homePage.BeginInvoke(new Action(() => MessageBox.Show("已中斷與房主的連線。", "連線中斷", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                }
            }
        }

        public static void CleanupNetwork()
        {
            isConnected = false;
            isSpectator = false;
            hostReady = false;
            clientReady = false;
            opponentDisconnected = false;
            needHandoff = false;
            hostName = "";
            player2Name = "";
            
            lock (clientList)
            {
                foreach (var c in clientList)
                {
                    try { c.Close(); } catch {}
                }
                clientList.Clear();
                clientNames.Clear();
            }
            
            if (stream != null)
            {
                try { stream.Close(); } catch {}
                stream = null;
            }
            if (client != null)
            {
                try { client.Close(); } catch {}
                client = null;
            }
            if (listener != null)
            {
                try { listener.Stop(); } catch {}
                listener = null;
            }
        }
    }
}

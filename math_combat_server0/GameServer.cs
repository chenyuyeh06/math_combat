using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace math_combat_server
{
    // ════════════════════════════════════════
    // 房間狀態
    // ════════════════════════════════════════
    public enum RoomState { WAITING, PLAYING, ROUND_RESULT, GAME_OVER, CLOSED }

    // ════════════════════════════════════════
    // 房間
    // ════════════════════════════════════════
    public class Room
    {
        public string RoomId { get; set; }
        public int MaxRounds { get; set; } = 3;
        public int Seconds { get; set; } = 30;
        public int CurrentRound { get; set; } = 1;
        public RoomState State { get; set; } = RoomState.WAITING;

        public ClientHandler Player1 { get; set; }
        public ClientHandler Player2 { get; set; }
        public List<ClientHandler> Spectators { get; set; } = new List<ClientHandler>();

        public bool Player1Ready { get; set; } = false;
        public bool Player2Ready { get; set; } = false;

        // 用於保護 Player1/Player2/Spectators 狀態變更的鎖對象
        public readonly object RoomLock = new object();

        public double? Score1 { get; set; }
        public double? Score2 { get; set; }
        public int Wins1 { get; set; } = 0;
        public int Wins2 { get; set; } = 0;

        // 儲存本回合雙方手牌，觀戰者用
        public List<string> LastHand1 { get; set; } = new List<string>();
        public List<string> LastHand2 { get; set; } = new List<string>();

        // 儲存雙方出牌算式，回合結算後廣播給觀戰者
        public string Expression1 { get; set; } = "";
        public string Expression2 { get; set; } = "";

        public DateTime RoundStartTime { get; set; }
        public CancellationTokenSource RoundCts { get; set; }

        public const int MaxTotal = 8;
        public bool PlayersFull { get { return Player1 != null && Player2 != null; } }
        public int TotalCount
        {
            get
            {
                return (Player1 != null ? 1 : 0) + (Player2 != null ? 1 : 0) + Spectators.Count;
            }
        }
        public bool IsFull { get { return TotalCount >= MaxTotal; } }

        public void Broadcast(object msg)
        {
            string json = Serialize(msg);
            if (Player1 != null) Player1.SendRaw(json);
            if (Player2 != null) Player2.SendRaw(json);
            lock (Spectators)
            {
                foreach (var s in Spectators) s.SendRaw(json);
            }
        }

        public void BroadcastSpectators(object msg)
        {
            string json = Serialize(msg);
            lock (Spectators)
            {
                foreach (var s in Spectators) s.SendRaw(json);
            }
        }

        private string Serialize(object msg)
        {
            return JsonConvert.SerializeObject(msg, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }

    // ════════════════════════════════════════
    // 主 Server
    // ════════════════════════════════════════
    public static class GameServer
    {
        public static ConcurrentDictionary<string, Room> Rooms = new ConcurrentDictionary<string, Room>();
        public static ConcurrentDictionary<string, bool> UsedNames = new ConcurrentDictionary<string, bool>();
        public static int TotalConnections = 0;

        private static TcpListener _listener;
        private static CancellationTokenSource _cts;

        public static void Start(int port = 9000)
        {
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            Form1.Instance.Log("Server 啟動，監聽 Port " + port, LogType.Success);
            Task.Run(() => AcceptLoop(_cts.Token));
        }

        public static void Stop()
        {
            if (_cts != null) _cts.Cancel();
            if (_listener != null) _listener.Stop();
            Form1.Instance.Log("Server 已停止", LogType.Warning);
        }

        private static async Task AcceptLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var tcp = await _listener.AcceptTcpClientAsync();
                    Interlocked.Increment(ref TotalConnections);
                    Form1.Instance.Log("新連線 (" + TotalConnections + " 人在線)", LogType.Success);
                    var handler = new ClientHandler(tcp);
                    Task.Run(() => handler.Run());
                }
                catch { break; }
            }
        }

        public static void UpdateUI()
        {
            Form1.Instance.UpdateStatus();
        }
    }

    // ════════════════════════════════════════
    // 單一 Client 處理器
    // ════════════════════════════════════════
    public class ClientHandler
    {
        private readonly TcpClient _tcp;
        private readonly NetworkStream _stream;
        public string Name { get; private set; } = "";
        private Room _room;
        private int _playerIndex; // 1=房主, 2=玩家2, 0=觀戰

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public ClientHandler(TcpClient tcp)
        {
            _tcp = tcp;
            _stream = tcp.GetStream();
        }

        public async Task Run()
        {
            try
            {
                var reader = new System.IO.StreamReader(_stream, Encoding.UTF8);
                while (true)
                {
                    string line = await reader.ReadLineAsync();
                    if (line == null) break;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    JObject msg;
                    try
                    {
                        msg = JObject.Parse(line);
                    }
                    catch
                    {
                        SendError("INVALID_JSON", "訊息格式錯誤");
                        continue;
                    }

                    string type = msg["type"] != null ? msg["type"].ToString() : "";
                    if (string.IsNullOrEmpty(type))
                    {
                        SendError("INVALID_MESSAGE_FORMAT", "缺少必要欄位");
                        continue;
                    }

                    await HandleMessage(type, msg);
                }
            }
            catch { }
            finally
            {
                await HandleDisconnect();
            }
        }

        private async Task HandleMessage(string type, JObject msg)
        {
            Form1.Instance.Log("[收] " + Name + " → " + type, LogType.Info);

            switch (type)
            {
                case "CREATE_ROOM": await OnCreateRoom(msg); break;
                case "JOIN_ROOM": await OnJoinRoom(msg); break;
                case "UPDATE_ROOM": OnUpdateRoom(msg); break;
                case "READY": OnReady(true); break;
                case "UNREADY": OnReady(false); break;
                case "START_GAME": await OnStartGame(); break;
                case "SUBMIT_SCORE": OnSubmitScore(msg); break;
                case "RETURN_ROOM": await OnReturnRoom(); break;
                case "LEAVE_AFTER_GAME": await OnLeaveAfterGame(); break;
                case "LEAVE": await HandleDisconnect(); break;
                default:
                    SendError("INVALID_MESSAGE_FORMAT", "未知的 type：" + type);
                    break;
            }
        }

        // ────────────────────────────────────
        // CREATE_ROOM
        // ────────────────────────────────────
        private async Task OnCreateRoom(JObject msg)
        {
            string name = msg["name"] != null ? msg["name"].ToString() : "";
            string roomId = msg["roomId"] != null ? msg["roomId"].ToString() : "";
            int rounds = msg["rounds"] != null ? (int)msg["rounds"] : 3;
            int seconds = msg["seconds"] != null ? (int)msg["seconds"] : 30;

            if (!GameServer.UsedNames.TryAdd(name, true))
            {
                SendError("NAME_IN_USE", "名字已被使用");
                return;
            }
            if (GameServer.Rooms.ContainsKey(roomId))
            {
                GameServer.UsedNames.TryRemove(name, out bool _);
                SendError("ROOM_ID_IN_USE", "房間號碼已被使用");
                return;
            }

            var room = new Room
            {
                RoomId = roomId,
                MaxRounds = rounds > 0 ? rounds : 3,
                Seconds = seconds > 0 ? seconds : 30,
                Player1 = this
            };
            GameServer.Rooms[roomId] = room;
            Name = name;
            _room = room;
            _playerIndex = 1;

            Send(new { type = "ROOM_CREATED", roomId = roomId });
            Form1.Instance.Log("[ROOM] " + roomId + " 建立 by " + name, LogType.Success);
            GameServer.UpdateUI();

            await Task.CompletedTask;
        }

        // ────────────────────────────────────
        // JOIN_ROOM
        // ────────────────────────────────────
        private async Task OnJoinRoom(JObject msg)
        {
            string name = msg["name"] != null ? msg["name"].ToString() : "";
            string roomId = msg["roomId"] != null ? msg["roomId"].ToString() : "";

            if (!GameServer.UsedNames.TryAdd(name, true))
            {
                SendError("NAME_IN_USE", "名字已被使用");
                return;
            }

            Room room;
            if (!GameServer.Rooms.TryGetValue(roomId, out room))
            {
                GameServer.UsedNames.TryRemove(name, out bool _);
                SendError("ROOM_NOT_FOUND", "房間不存在");
                return;
            }
            if (room.IsFull)
            {
                GameServer.UsedNames.TryRemove(name, out bool _);
                SendError("ROOM_FULL", "房間已滿");
                return;
            }

            Name = name;
            _room = room;

            if (!room.PlayersFull)
            {
                if (room.Player1 == null)
                {
                    // 防禦性處理：P1 不存在（理論上應由 PromoteSpectator 補，但做雙重保障）
                    room.Player1 = this;
                    _playerIndex = 1;
                    room.Player1Ready = false;
                    room.Player2Ready = false;

                    Send(new
                    {
                        type = "ROOM_JOINED",
                        role = "host",
                        opponent = room.Player2 != null ? room.Player2.Name : "",
                        rounds = room.MaxRounds,
                        seconds = room.Seconds
                    });

                    BroadcastRoomInfo();
                    BroadcastReadyState();
                    Form1.Instance.Log("[JOIN] " + name + " 加入 " + roomId + "（遞補為房主）", LogType.Warning);
                }
                else
                {
                    // 正常情況：P2 空位，新玩家加入
                    room.Player2 = this;
                    _playerIndex = 2;
                    room.Player1Ready = false;
                    room.Player2Ready = false;

                    Send(new
                    {
                        type = "ROOM_JOINED",
                        role = "player",
                        opponent = room.Player1 != null ? room.Player1.Name : "",
                        rounds = room.MaxRounds,
                        seconds = room.Seconds
                    });

                    BroadcastRoomInfo();
                    BroadcastReadyState();
                    Form1.Instance.Log("[JOIN] " + name + " 加入 " + roomId + "（玩家2）", LogType.Success);
                }
            }
            else
            {
                lock (room.Spectators) { room.Spectators.Add(this); }
                _playerIndex = 0;

                Send(new
                {
                    type = "ROOM_JOINED",
                    role = "spectator",
                    player1 = room.Player1 != null ? room.Player1.Name : "",
                    player2 = room.Player2 != null ? room.Player2.Name : "",
                    currentRound = room.CurrentRound,
                    wins1 = room.Wins1,
                    wins2 = room.Wins2,
                    spectatorCount = room.Spectators.Count,
                    state = room.State.ToString()
                });

                // 如果遊戲已經在進行中，額外送一個 ROUND_START 讓新觀戰者切換畫面並載入牌組
                if (room.State == RoomState.PLAYING || room.State == RoomState.ROUND_RESULT)
                {
                    int elapsed = (int)(DateTime.Now - room.RoundStartTime).TotalSeconds;
                    int remaining = Math.Max(0, room.Seconds - elapsed);
                    Send(new
                    {
                        type = "ROUND_START",
                        round = room.CurrentRound,
                        seconds = remaining,
                        cards = room.LastHand1
                    });
                }

                var joinMsg = new
                {
                    type = "SPECTATOR_JOINED",
                    name = name,
                    spectatorCount = room.Spectators.Count
                };
                if (room.Player1 != null) room.Player1.Send(joinMsg);
                if (room.Player2 != null) room.Player2.Send(joinMsg);
                lock (room.Spectators)
                {
                    foreach (var s in room.Spectators)
                        if (s != this) s.Send(joinMsg);
                }

                BroadcastRoomInfo();
                Form1.Instance.Log("[JOIN] " + name + " 加入 " + roomId + "（觀戰）", LogType.Info);
            }
            GameServer.UpdateUI();

            await Task.CompletedTask;
        }

        // ────────────────────────────────────
        // UPDATE_ROOM
        // ────────────────────────────────────
        private void OnUpdateRoom(JObject msg)
        {
            if (_room == null || _playerIndex != 1) { SendError("PERMISSION_DENIED", "無權限操作"); return; }
            if (_room.State != RoomState.WAITING) { SendError("GAME_ALREADY_STARTED", "遊戲已開始，無法更改設定"); return; }

            int rounds = msg["rounds"] != null ? (int)msg["rounds"] : _room.MaxRounds;
            int seconds = msg["seconds"] != null ? (int)msg["seconds"] : _room.Seconds;

            _room.MaxRounds = rounds > 0 ? rounds : _room.MaxRounds;
            _room.Seconds = seconds > 0 ? seconds : _room.Seconds;
            _room.Player1Ready = false;
            _room.Player2Ready = false;

            _room.Broadcast(new { type = "ROOM_UPDATED", rounds = _room.MaxRounds, seconds = _room.Seconds });
            BroadcastReadyState();
            Form1.Instance.Log("[UPDATE] " + _room.RoomId + " 設定：" + _room.MaxRounds + "回合 " + _room.Seconds + "秒", LogType.Info);
        }

        // ────────────────────────────────────
        // READY / UNREADY
        // ────────────────────────────────────
        private void OnReady(bool ready)
        {
            if (_room == null || _playerIndex == 0) return;
            if (_room.State != RoomState.WAITING) return;

            if (_playerIndex == 1) _room.Player1Ready = ready;
            else _room.Player2Ready = ready;

            BroadcastReadyState();
            Form1.Instance.Log("[READY] " + Name + " " + (ready ? "準備" : "取消準備"), LogType.Info);
        }

        // ────────────────────────────────────
        // START_GAME
        // ────────────────────────────────────
        private async Task OnStartGame()
        {
            if (_room == null || _playerIndex != 1) { SendError("PERMISSION_DENIED", "無權限操作"); return; }
            if (_room.State != RoomState.WAITING) { SendError("INVALID_STATE", "目前狀態不可執行此操作"); return; }
            if (!_room.PlayersFull) { SendError("INVALID_STATE", "尚未有對手"); return; }
            if (!_room.Player1Ready || !_room.Player2Ready) { SendError("INVALID_STATE", "雙方尚未都準備好"); return; }

            _room.State = RoomState.PLAYING;
            _room.CurrentRound = 1;
            _room.Wins1 = 0;
            _room.Wins2 = 0;

            _room.Broadcast(new { type = "GAME_START", rounds = _room.MaxRounds, seconds = _room.Seconds });
            Form1.Instance.Log("[START] " + _room.RoomId + " 遊戲開始！" + _room.MaxRounds + "回合 " + _room.Seconds + "秒", LogType.Success);

            await Task.Delay(500);
            await StartRound();
        }

        // ────────────────────────────────────
        // 發牌
        // ────────────────────────────────────
        private async Task StartRound()
        {
            if (_room == null) return;

            _room.Score1 = null;
            _room.Score2 = null;
            _room.State = RoomState.PLAYING;

            var hands = DealCards();
            var hand1 = hands.Item1;
            var hand2 = hands.Item2;

            // 儲存房主手牌供觀戰者使用
            _room.LastHand1 = hand1;

            if (_room.Player1 != null)
                _room.Player1.Send(new { type = "ROUND_START", round = _room.CurrentRound, seconds = _room.Seconds, cards = hand1 });

            if (_room.Player2 != null)
                _room.Player2.Send(new { type = "ROUND_START", round = _room.CurrentRound, seconds = _room.Seconds, cards = hand2 });

            // 觀戰者收到房主手牌
            _room.BroadcastSpectators(new { type = "ROUND_START", round = _room.CurrentRound, seconds = _room.Seconds, cards = hand1 });

            _room.RoundStartTime = DateTime.Now;
            Form1.Instance.Log("[DEAL] " + _room.RoomId + " 第" + _room.CurrentRound + "回合發牌", LogType.Info);
            Form1.Instance.Log("[DEAL] " + ((_room.Player1 != null) ? _room.Player1.Name : "P1") + ": " + string.Join(",", hand1), LogType.Info);
            Form1.Instance.Log("[DEAL] " + ((_room.Player2 != null) ? _room.Player2.Name : "P2") + ": " + string.Join(",", hand2), LogType.Info);

            // 逾時保底
            if (_room.RoundCts != null) _room.RoundCts.Cancel();
            _room.RoundCts = new CancellationTokenSource();
            var cts = _room.RoundCts;
            var room = _room;

            Task.Run(async () =>
            {
                try { await Task.Delay((room.Seconds + 2) * 1000, cts.Token); }
                catch { return; }

                lock (room)
                {
                    if (room.State != RoomState.PLAYING) return;
                    if (!room.Score1.HasValue)
                    {
                        room.Score1 = -999;
                        Form1.Instance.Log("[TIMEOUT] " + (room.Player1 != null ? room.Player1.Name : "P1") + " 逾時", LogType.Warning);
                    }
                    if (!room.Score2.HasValue)
                    {
                        room.Score2 = -999;
                        Form1.Instance.Log("[TIMEOUT] " + (room.Player2 != null ? room.Player2.Name : "P2") + " 逾時", LogType.Warning);
                    }
                    if (room.Score1.HasValue && room.Score2.HasValue)
                        JudgeRound(room);
                }
            });

            await Task.CompletedTask;
        }

        // 洗牌
        private Tuple<List<string>, List<string>> DealCards()
        {
            var numbers = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            var operators = new List<string> { "+", "-", "*", "/" };
            var deck = new List<string>();
            deck.AddRange(numbers);
            deck.AddRange(operators);

            List<string> h1, h2;
            var rng = new Random();
            do
            {
                var shuffled = deck.OrderBy(_ => rng.Next()).ToList();
                h1 = shuffled.Take(5).ToList();
                h2 = shuffled.Skip(5).Take(5).ToList();
            }
            while (!h1.Any(c => operators.Contains(c)) || !h2.Any(c => operators.Contains(c)));

            return Tuple.Create(h1, h2);
        }

        // ────────────────────────────────────
        // SUBMIT_SCORE
        // ────────────────────────────────────
        private void OnSubmitScore(JObject msg)
        {
            if (_room == null || _playerIndex == 0) { SendError("SPECTATOR_CANNOT_SUBMIT", "觀戰者不能提交分數"); return; }
            if (_room.State != RoomState.PLAYING) { SendError("INVALID_STATE", "目前狀態不可執行此操作"); return; }

            int round = msg["round"] != null ? (int)msg["round"] : 0;
            double score = msg["score"] != null ? (double)msg["score"] : -999;

            if (round != _room.CurrentRound) { SendError("INVALID_SCORE", "已超過本回合截止時間"); return; }

            lock (_room)
            {
                if (_playerIndex == 1) { _room.Score1 = score; _room.Expression1 = msg["expression"] != null ? msg["expression"].ToString() : ""; }
                else { _room.Score2 = score; _room.Expression2 = msg["expression"] != null ? msg["expression"].ToString() : ""; }

                Form1.Instance.Log("[SCORE] " + Name + " 送出 " + score + "（回合" + round + "）", LogType.Score);

                if (_room.Score1.HasValue && _room.Score2.HasValue)
                {
                    if (_room.RoundCts != null) _room.RoundCts.Cancel();
                    JudgeRound(_room);
                }
            }
        }

        // 判斷勝負
        private static void JudgeRound(Room room)
        {
            room.State = RoomState.ROUND_RESULT;

            double s1 = room.Score1.Value;
            double s2 = room.Score2.Value;

            string r1, r2;
            if (s1 > s2) { r1 = "WIN"; r2 = "LOSE"; room.Wins1++; }
            else if (s2 > s1) { r1 = "LOSE"; r2 = "WIN"; room.Wins2++; }
            else { r1 = "DRAW"; r2 = "DRAW"; }

            Form1.Instance.Log("[RESULT] 第" + room.CurrentRound + "回合：" + (room.Player1 != null ? room.Player1.Name : "P1") + " " + r1 + "（" + s1 + " vs " + s2 + "）", LogType.Score);

            var p1Msg = new { type = "ROUND_RESULT", round = room.CurrentRound, yourScore = s1, opponentScore = s2, result = r1, wins1 = room.Wins1, wins2 = room.Wins2 };
            var p2Msg = new { type = "ROUND_RESULT", round = room.CurrentRound, yourScore = s2, opponentScore = s1, result = r2, wins1 = room.Wins1, wins2 = room.Wins2 };

            if (room.Player1 != null) room.Player1.Send(p1Msg);
            if (room.Player2 != null) room.Player2.Send(p2Msg);
            // 觀戰者收到結算時附帶雙方出牌算式
            var spectatorResultMsg = new { type = "ROUND_RESULT", round = room.CurrentRound, yourScore = s1, opponentScore = s2, result = r1, wins1 = room.Wins1, wins2 = room.Wins2, expression1 = room.Expression1, expression2 = room.Expression2 };
            room.BroadcastSpectators(spectatorResultMsg);

            room.Score1 = null;
            room.Score2 = null;
            room.Expression1 = "";
            room.Expression2 = "";

            // 提前結算：超過半數勝場就結束
            int halfRounds = (int)Math.Ceiling(room.MaxRounds / 2.0);
            if (room.Wins1 >= halfRounds || room.Wins2 >= halfRounds || room.CurrentRound >= room.MaxRounds)
            {
                Task.Run(() => EndGame(room));
            }
            else
            {
                room.CurrentRound++;
                room.Broadcast(new { type = "NEXT_ROUND", round = room.CurrentRound });
                Task.Run(async () =>
                {
                    await Task.Delay(2000);
                    var handler = room.Player1 ?? room.Player2;
                    if (handler != null) await handler.StartRound();
                });
            }
        }

        // 遊戲結束
        private static void EndGame(Room room)
        {
            room.State = RoomState.GAME_OVER;

            string finalR1, finalR2;
            if (room.Wins1 > room.Wins2) { finalR1 = "WIN"; finalR2 = "LOSE"; }
            else if (room.Wins2 > room.Wins1) { finalR1 = "LOSE"; finalR2 = "WIN"; }
            else { finalR1 = "DRAW"; finalR2 = "DRAW"; }

            Form1.Instance.Log("[OVER] " + room.RoomId + " 遊戲結束：" + finalR1 + "（" + room.Wins1 + ":" + room.Wins2 + "）", LogType.Score);

            if (room.Player1 != null) room.Player1.Send(new { type = "GAME_OVER", result = finalR1, wins1 = room.Wins1, wins2 = room.Wins2 });
            if (room.Player2 != null) room.Player2.Send(new { type = "GAME_OVER", result = finalR2, wins1 = room.Wins1, wins2 = room.Wins2 });
            room.BroadcastSpectators(new { type = "GAME_OVER", result = finalR1, wins1 = room.Wins1, wins2 = room.Wins2 });

            // 30秒逾時自動清除
            if (room.RoundCts != null) room.RoundCts.Cancel();
            room.RoundCts = new CancellationTokenSource();
            var cts = room.RoundCts;
            Task.Run(async () =>
            {
                try { await Task.Delay(30000, cts.Token); }
                catch { return; }
                if (room.State != RoomState.GAME_OVER) return;
                Form1.Instance.Log("[TIMEOUT] " + room.RoomId + " 結算逾時，強制清除", LogType.Warning);
                CleanupRoom(room);
            });
        }

        // ────────────────────────────────────
        // RETURN_ROOM / LEAVE_AFTER_GAME
        // ────────────────────────────────────
        private async Task OnReturnRoom()
        {
            if (_room == null || _room.State != RoomState.GAME_OVER) return;
            Form1.Instance.Log("[RETURN] " + Name + " 選擇回到房間", LogType.Info);
            await CheckRoomReset(_room);
        }

        private async Task OnLeaveAfterGame()
        {
            if (_room == null) return;
            Form1.Instance.Log("[LEAVE] " + Name + " 選擇回到首頁", LogType.Info);
            await HandleLeave();
        }

        private static async Task CheckRoomReset(Room room)
        {
            if (room.Player1 != null && room.Player2 != null && room.State == RoomState.GAME_OVER)
            {
                if (room.RoundCts != null) room.RoundCts.Cancel();
                room.State = RoomState.WAITING;
                room.CurrentRound = 1;
                room.Wins1 = 0;
                room.Wins2 = 0;
                room.Score1 = null;
                room.Score2 = null;
                room.Player1Ready = false;
                room.Player2Ready = false;

                room.Broadcast(new
                {
                    type = "ROOM_RESET",
                    player1 = room.Player1 != null ? room.Player1.Name : "",
                    player2 = room.Player2 != null ? room.Player2.Name : "",
                    spectators = room.Spectators.Select(s => s.Name).ToList(),
                    rounds = room.MaxRounds,
                    seconds = room.Seconds
                });
                Form1.Instance.Log("[RESET] " + room.RoomId + " 房間重置", LogType.Success);
            }
            await Task.CompletedTask;
        }

        // ────────────────────────────────────
        // 斷線 / 離開
        // ────────────────────────────────────
        private async Task HandleDisconnect()
        {
            Interlocked.Decrement(ref GameServer.TotalConnections);
            await HandleLeave();
            try { _tcp.Close(); } catch { }
        }

        private async Task HandleLeave()
        {
            if (_room == null) return;
            var room = _room;
            _room = null;

            if (_playerIndex == 0)
            {
                lock (room.Spectators) { room.Spectators.Remove(this); }
                room.Broadcast(new { type = "SPECTATOR_LEFT", name = Name, spectatorCount = room.Spectators.Count });
                Form1.Instance.Log("[LEAVE] 觀戰者 " + Name + " 離開 " + room.RoomId, LogType.Info);
            }
            else
            {
                Form1.Instance.Log("[LEAVE] 玩家 " + Name + " 離開 " + room.RoomId, LogType.Warning);

                if (room.State == RoomState.PLAYING)
                {
                    if (room.RoundCts != null) room.RoundCts.Cancel();
                    room.State = RoomState.GAME_OVER;
                    room.Broadcast(new { type = "GAME_ABORTED", message = Name + " 已斷線，遊戲已中止" });
                    Form1.Instance.Log("[ABORT] " + room.RoomId + " 遊戲中止", LogType.Error);
                }

                room.Broadcast(new { type = "PLAYER_LEFT", name = Name });

                // 把「清空位置」和「遞補」封裝在同一個原子操作裡
                await PromoteSpectator(room, _playerIndex);

                if (room.Player1 == null && room.Player2 == null && room.Spectators.Count == 0)
                    CleanupRoom(room);
                else
                    BroadcastRoomInfoFor(room);
            }

            if (!string.IsNullOrEmpty(Name))
                GameServer.UsedNames.TryRemove(Name, out bool _);

            GameServer.UpdateUI();
        }

        private static async Task PromoteSpectator(Room room, int leavingPlayerIndex)
        {
            lock (room.RoomLock)
            {
                // Step 1: 清空離開玩家的位置（和下面的遞補在同一個 lock 裡，不會被其他 thread 切中）
                if (leavingPlayerIndex == 1) room.Player1 = null;
                else room.Player2 = null;

                // Step 2: 優先 1：玩家2 升為房主
                if (room.Player1 == null && room.Player2 != null)
                {
                    room.Player1 = room.Player2;
                    room.Player1._playerIndex = 1;
                    room.Player2 = null;
                    room.Player1Ready = false;
                    room.Player2Ready = false;
                    Form1.Instance.Log("[PROMOTE] " + room.Player1.Name + " 由玩家2升為房主", LogType.Warning);
                    room.Player1.Send(new { type = "PROMOTED", role = "host" });
                }

                // Step 3: 優先 2：觀戰者遞補房主（P1 仍空）
                if (room.Player1 == null && room.Spectators.Count > 0)
                {
                    var newHost = room.Spectators[0];
                    room.Spectators.RemoveAt(0);
                    room.Player1 = newHost;
                    newHost._playerIndex = 1;
                    room.Player1Ready = false;
                    room.Player2Ready = false;
                    Form1.Instance.Log("[PROMOTE] " + newHost.Name + " 由觀戰遞補為房主", LogType.Warning);
                    newHost.Send(new { type = "PROMOTED", role = "host" });
                }

                // Step 4: 優先 3：觀戰者遞補玩家2（P2 空）
                if (room.Player2 == null && room.Spectators.Count > 0)
                {
                    var newP2 = room.Spectators[0];
                    room.Spectators.RemoveAt(0);
                    room.Player2 = newP2;
                    newP2._playerIndex = 2;
                    room.Player2Ready = false;
                    Form1.Instance.Log("[PROMOTE] " + newP2.Name + " 由觀戰遞補為玩家2", LogType.Warning);
                    newP2.Send(new { type = "PROMOTED", role = "player" });
                }
            }
            room.State = RoomState.WAITING;
            BroadcastRoomInfoFor(room);
            await Task.CompletedTask;
        }

        private static void CleanupRoom(Room room)
        {
            room.State = RoomState.CLOSED;
            if (room.RoundCts != null) room.RoundCts.Cancel();
            GameServer.Rooms.TryRemove(room.RoomId, out Room _);
            Form1.Instance.Log("[CLOSE] 房間 " + room.RoomId + " 已移除", LogType.Warning);
            GameServer.UpdateUI();
        }

        // ────────────────────────────────────
        // 廣播工具
        // ────────────────────────────────────
        private void BroadcastRoomInfo() => BroadcastRoomInfoFor(_room);

        private static void BroadcastRoomInfoFor(Room room)
        {
            if (room == null) return;
            room.Broadcast(new
            {
                type = "ROOM_INFO",
                player1 = room.Player1 != null ? room.Player1.Name : "",
                player2 = room.Player2 != null ? room.Player2.Name : "",
                spectators = room.Spectators.Select(s => s.Name).ToList()
            });
        }

        private void BroadcastReadyState()
        {
            if (_room == null) return;
            _room.Broadcast(new
            {
                type = "READY_STATE",
                player1Ready = _room.Player1Ready,
                player2Ready = _room.Player2Ready
            });
        }

        // ────────────────────────────────────
        // 傳送工具
        // ────────────────────────────────────
        public void Send(object msg)
        {
            SendRaw(JsonConvert.SerializeObject(msg, _jsonSettings));
        }

        public void SendRaw(string json)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(json + "\n");
                lock (_stream) { _stream.Write(data, 0, data.Length); }
            }
            catch { }
        }

        private void SendError(string code, string message)
        {
            Send(new { type = "ERROR", code = code, message = message });
        }
    }
}
using System.Collections.Generic;

namespace math_combat
{
    // ─── 所有訊息的共同基底（只含 type 欄位）───────────────────────────────
    public class NetworkMessage
    {
        public string type { get; set; }
    }

    // ═══════════════════════════════════════════════════════
    //  Client → Server 訊息
    // ═══════════════════════════════════════════════════════

    /// <summary>建立房間（Player1 使用）</summary>
    public class CreateRoomMsg : NetworkMessage
    {
        public CreateRoomMsg() { type = "CREATE_ROOM"; }
        public string name    { get; set; }
        public string roomId  { get; set; }
        public int    rounds  { get; set; }
        public int    seconds { get; set; }
    }

    /// <summary>加入房間（Player2 / 觀戰者使用）</summary>
    public class JoinRoomMsg : NetworkMessage
    {
        public JoinRoomMsg() { type = "JOIN_ROOM"; }
        public string name   { get; set; }
        public string roomId { get; set; }
    }

    /// <summary>更新房間設定（回合數 / 秒數），只有 Player1 可送</summary>
    public class UpdateRoomMsg : NetworkMessage
    {
        public UpdateRoomMsg() { type = "UPDATE_ROOM"; }
        public int rounds  { get; set; }
        public int seconds { get; set; }
    }

    /// <summary>Player2 表示準備</summary>
    public class ReadyMsg : NetworkMessage
    {
        public ReadyMsg() { type = "READY"; }
    }

    /// <summary>Player2 取消準備</summary>
    public class UnreadyMsg : NetworkMessage
    {
        public UnreadyMsg() { type = "UNREADY"; }
    }

    /// <summary>Player1（房主）按下開始遊戲</summary>
    public class StartGameMsg : NetworkMessage
    {
        public StartGameMsg() { type = "START_GAME"; }
    }

    /// <summary>送出本回合得分</summary>
    public class SubmitScoreMsg : NetworkMessage
    {
        public SubmitScoreMsg() { type = "SUBMIT_SCORE"; }
        public int    round { get; set; }
        public double score { get; set; }
    }

    /// <summary>遊戲結束後回到大廳（送到 RoomPage）</summary>
    public class ReturnRoomMsg : NetworkMessage
    {
        public ReturnRoomMsg() { type = "RETURN_ROOM"; }
    }

    /// <summary>遊戲結束後直接離開</summary>
    public class LeaveAfterGameMsg : NetworkMessage
    {
        public LeaveAfterGameMsg() { type = "LEAVE_AFTER_GAME"; }
    }

    /// <summary>完全離開房間 / 斷線前通知</summary>
    public class LeaveMsg : NetworkMessage
    {
        public LeaveMsg() { type = "LEAVE"; }
    }

    // ═══════════════════════════════════════════════════════
    //  Server → Client 訊息
    // ═══════════════════════════════════════════════════════

    /// <summary>房間建立成功</summary>
    public class RoomCreatedMsg : NetworkMessage
    {
        public string roomId { get; set; }
    }

    /// <summary>成功加入房間（role = "player" 或 "spectator"）</summary>
    public class RoomJoinedMsg : NetworkMessage
    {
        // 共用
        public string role { get; set; }
        // role == "player"
        public string opponent { get; set; }
        public int    rounds   { get; set; }
        public int    seconds  { get; set; }
        // role == "spectator"
        public string player1        { get; set; }
        public string player2        { get; set; }
        public int    currentRound   { get; set; }
        public int    wins1          { get; set; }
        public int    wins2          { get; set; }
        public int    spectatorCount { get; set; }
    }

    /// <summary>房間資訊（玩家名稱、觀戰者列表）</summary>
    public class RoomInfoMsg : NetworkMessage
    {
        public string       player1    { get; set; }
        public string       player2    { get; set; }
        public List<string> spectators { get; set; }
    }

    /// <summary>房間設定已更新</summary>
    public class RoomUpdatedMsg : NetworkMessage
    {
        public int rounds  { get; set; }
        public int seconds { get; set; }
    }

    /// <summary>雙方準備狀態</summary>
    public class ReadyStateMsg : NetworkMessage
    {
        public bool player1Ready { get; set; }
        public bool player2Ready { get; set; }
    }

    /// <summary>遊戲正式開始（Server 廣播給所有人）</summary>
    public class GameStartMsg : NetworkMessage
    {
        public int rounds  { get; set; }
        public int seconds { get; set; }
    }

    /// <summary>新回合開始，附帶 Server 分配的牌組</summary>
    public class RoundStartMsg : NetworkMessage
    {
        public int          round   { get; set; }
        public int          seconds { get; set; }
        public List<string> cards   { get; set; }
    }

    /// <summary>回合結算結果</summary>
    public class RoundResultMsg : NetworkMessage
    {
        public int    round         { get; set; }
        public double yourScore     { get; set; }
        public double opponentScore { get; set; }
        public string result        { get; set; } // "WIN" / "LOSE" / "DRAW"
        public int    wins1         { get; set; }
        public int    wins2         { get; set; }
    }

    /// <summary>通知進入下一回合</summary>
    public class NextRoundMsg : NetworkMessage
    {
        public int round { get; set; }
    }

    /// <summary>整場遊戲結束</summary>
    public class GameOverMsg : NetworkMessage
    {
        public string result { get; set; } // "WIN" / "LOSE" / "DRAW"
        public int    wins1  { get; set; }
        public int    wins2  { get; set; }
    }

    /// <summary>回到大廳後的房間重置資訊</summary>
    public class RoomResetMsg : NetworkMessage
    {
        public string       player1    { get; set; }
        public string       player2    { get; set; }
        public List<string> spectators { get; set; }
        public int          rounds     { get; set; }
        public int          seconds    { get; set; }
    }

    /// <summary>某玩家離開</summary>
    public class PlayerLeftMsg : NetworkMessage
    {
        public string name { get; set; }
    }

    /// <summary>新觀戰者加入</summary>
    public class SpectatorJoinedMsg : NetworkMessage
    {
        public string name           { get; set; }
        public int    spectatorCount { get; set; }
    }

    /// <summary>觀戰者離開</summary>
    public class SpectatorLeftMsg : NetworkMessage
    {
        public string name           { get; set; }
        public int    spectatorCount { get; set; }
    }

    /// <summary>遊戲因異常中止</summary>
    public class GameAbortedMsg : NetworkMessage
    {
        public string message { get; set; }
    }

    /// <summary>Server 回報錯誤</summary>
    public class ErrorMsg : NetworkMessage
    {
        public string code    { get; set; }
        public string message { get; set; }
    }
}

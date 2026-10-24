using System;
using Cysharp.Threading.Tasks;
using GameFrameY.Bridge.Event;
using GameFrameY.Core;
using GameFrameY.Game.Modules.Match3.Board;

namespace GameFrameY.Game.Modules.Match3
{
    /// <summary>消消乐对外服务接口（登记进 IServiceRegistry，供其他模块/调试发现）。</summary>
    public interface IMatch3Service
    {
        int LevelId { get; }

        int Score { get; }

        void StartLevel(int levelId);

        void Restart();
    }

    internal static class Match3EventIds
    {
        public const string LevelStart = "GameFrameY.Match3.LevelStart";
        public const string ScoreChanged = "GameFrameY.Match3.ScoreChanged";
        public const string LevelComplete = "GameFrameY.Match3.LevelComplete";
        public const string LevelFailed = "GameFrameY.Match3.LevelFailed";
    }

    /// <summary>
    /// 玩法控制器：关卡状态机（目标分/步数/胜负）、把棋盘逻辑步骤翻译为 UI 表现并计分。
    /// 计分规则：每轮消除的宝石数 × 基础分（Luban TbMatch3Const）× 级联序号。
    /// </summary>
    public sealed class Match3GameController : IMatch3Service
    {
        private readonly cfg.Tables _tables;
        private readonly Match3UI _ui;
        private readonly IEventService _events;
        private Match3Board _board;
        private bool _busy;
        private bool _over;

        public Match3GameController(cfg.Tables tables, Match3UI ui, IEventService events)
        {
            _tables = tables;
            _ui = ui;
            _events = events;
            _ui.SwapRequested += OnSwapRequested;
            _ui.RestartRequested += Restart;
        }

        public int LevelId { get; private set; } = 1;

        public int Score { get; private set; }

        public int TargetScore { get; private set; }

        public int MovesLeft { get; private set; }

        public void StartLevel(int levelId)
        {
            var level = _tables.TbMatch3Level.GetOrDefault(levelId);
            if (level == null)
            {
                GfyLog.Error($"[Match3] 关卡 {levelId} 不存在（TbMatch3Level）");
                return;
            }

            LevelId = levelId;
            TargetScore = level.TargetScore;
            Score = 0;
            MovesLeft = level.MoveCount;
            _over = false;
            _busy = false;

            _board = new Match3Board(level.RowCount, level.ColumnCount, level.ColorCount);
            _board.Reset();

            _ui.HideResult();
            _ui.RebuildBoard(_board);
            _ui.RefreshHud(LevelId, Score, TargetScore, MovesLeft);
            _events?.Fire(Match3EventIds.LevelStart, this);
            GfyLog.Info($"[Match3] 第 {LevelId} 关开始 {level.RowCount}x{level.ColumnCount} 颜色 {level.ColorCount}，" +
                        $"目标 {TargetScore} 分 / {MovesLeft} 步");
        }

        public void Restart() => StartLevel(LevelId);

        private void OnSwapRequested(CellPos a, CellPos b)
        {
            UniTask.Void(async () =>
            {
                try
                {
                    await TrySwapAsync(a, b);
                }
                catch (OperationCanceledException)
                {
                    // 模块关闭时动画补间被取消，属正常流程
                }
                catch (Exception e)
                {
                    GfyLog.Error($"[Match3] 交换处理异常: {e}");
                }
            });
        }

        private async UniTask TrySwapAsync(CellPos a, CellPos b)
        {
            if (_busy || _over || _board == null || !a.IsAdjacentTo(b))
            {
                return;
            }

            _busy = true;
            try
            {
                var result = _board.TrySwap(a, b);
                if (!result.Valid)
                {
                    await _ui.PlayInvalidSwap(a, b);
                    return;
                }

                await _ui.PlaySwap(a, b);
                MovesLeft--;

                var baseScore = _tables.TbMatch3Const.BaseScore;
                await _ui.PlaySteps(result.Steps, step =>
                {
                    var gained = step.Removed.Count * baseScore * step.CascadeIndex;
                    Score += gained;
                    _ui.RefreshHud(LevelId, Score, TargetScore, MovesLeft);
                    return gained;
                });

                _events?.Fire(Match3EventIds.ScoreChanged, this);

                if (Score >= TargetScore)
                {
                    _over = true;
                    _events?.Fire(Match3EventIds.LevelComplete, this);
                    GfyLog.Info($"[Match3] 第 {LevelId} 关过关，得分 {Score}");
                    var next = _tables.TbMatch3Level.GetOrDefault(LevelId + 1) != null ? LevelId + 1 : 1;
                    _ui.ShowResult(true, Score, "下一关", () => StartLevel(next));
                }
                else if (MovesLeft <= 0)
                {
                    _over = true;
                    _events?.Fire(Match3EventIds.LevelFailed, this);
                    GfyLog.Info($"[Match3] 第 {LevelId} 关失败（步数用完），得分 {Score}/{TargetScore}");
                    _ui.ShowResult(false, Score, "再试一次", Restart);
                }
            }
            finally
            {
                _busy = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace GameFrameY.Game.Modules.Match3.Board
{
    /// <summary>宝石颜色（1..6 对应 gem_red..gem_purple 贴图；None=空）。</summary>
    public enum GemKind
    {
        None = 0,
        Red = 1,
        Orange = 2,
        Yellow = 3,
        Green = 4,
        Blue = 5,
        Purple = 6,
    }

    /// <summary>宝石实例：Id 全局唯一，渲染层靠它跟踪同一颗宝石的移动/消除。</summary>
    public sealed class Gem
    {
        public Gem(int id, GemKind kind)
        {
            Id = id;
            Kind = kind;
        }

        public int Id { get; }
        public GemKind Kind { get; set; }
    }

    /// <summary>格子坐标：Row 0 是最底行（重力方向），Col 0 在左。</summary>
    public readonly struct CellPos : IEquatable<CellPos>
    {
        public CellPos(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public int Row { get; }
        public int Col { get; }

        public bool Equals(CellPos other) => Row == other.Row && Col == other.Col;

        public override bool Equals(object obj) => obj is CellPos other && Equals(other);

        public override int GetHashCode() => (Row * 397) ^ Col;

        public override string ToString() => $"({Row},{Col})";

        public static bool operator ==(CellPos left, CellPos right) => left.Equals(right);

        public static bool operator !=(CellPos left, CellPos right) => !left.Equals(right);

        public bool IsAdjacentTo(CellPos other) =>
            Math.Abs(Row - other.Row) + Math.Abs(Col - other.Col) == 1;
    }

    public enum BoardStepKind
    {
        /// <summary>消除（Removed + 级联序号 CascadeIndex，1 起）。</summary>
        Match = 0,

        /// <summary>重力下落（Moved：From→To，宝石对象不变）。</summary>
        Fall = 1,

        /// <summary>顶部补充（Added）。</summary>
        Refill = 2,

        /// <summary>洗牌（Added = 洗牌后的全量棋盘，渲染层整体重建）。</summary>
        Shuffle = 3,
    }

    public sealed class RemovedGem
    {
        public RemovedGem(CellPos pos, Gem gem)
        {
            Pos = pos;
            Gem = gem;
        }

        public CellPos Pos { get; }
        public Gem Gem { get; }
    }

    public sealed class MovedGem
    {
        public MovedGem(CellPos from, CellPos to, Gem gem)
        {
            From = from;
            To = to;
            Gem = gem;
        }

        public CellPos From { get; }
        public CellPos To { get; }
        public Gem Gem { get; }
    }

    public sealed class NewGem
    {
        public NewGem(CellPos pos, Gem gem)
        {
            Pos = pos;
            Gem = gem;
        }

        public CellPos Pos { get; }
        public Gem Gem { get; }
    }

    /// <summary>一次棋盘演进步骤，渲染层按顺序播放。</summary>
    public sealed class BoardStep
    {
        public BoardStepKind Kind { get; set; }

        /// <summary>Match 专属：第几轮级联（1 起，用于分数倍率与 UI 表现）。</summary>
        public int CascadeIndex { get; set; }

        public List<RemovedGem> Removed { get; } = new();

        public List<MovedGem> Moved { get; } = new();

        public List<NewGem> Added { get; } = new();
    }

    /// <summary>交换结果：Valid=false 表示不产生消除（棋盘已还原）；Steps 为后续演进序列。</summary>
    public sealed class SwapResult
    {
        public bool Valid { get; set; }
        public List<BoardStep> Steps { get; } = new();
    }
}

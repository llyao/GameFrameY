using System;
using System.Collections.Generic;

namespace GameFrameY.Game.Modules.Match3.Board
{
    /// <summary>
    /// 消消乐棋盘纯逻辑（无 Unity 依赖）：
    /// 初始棋盘无现成消除且有解；交换仅当产生消除时生效；消除→重力→补充循环演进为步骤序列；
    /// 结算后无可消组合时自动洗牌。随机源可注入（seed）以便测试/回放。
    /// </summary>
    public sealed class Match3Board
    {
        private readonly int _rows;
        private readonly int _cols;
        private readonly int _colorCount;
        private readonly Random _rng;
        private Gem[,] _grid;
        private int _nextGemId = 1;

        public Match3Board(int rows, int cols, int colorCount, int seed = 0)
        {
            if (rows < 4 || cols < 4)
            {
                throw new ArgumentException($"棋盘尺寸过小: {rows}x{cols}");
            }

            _rows = rows;
            _cols = cols;
            _colorCount = Math.Clamp(colorCount, 3, 6);
            _rng = seed != 0 ? new Random(seed) : new Random(Environment.TickCount);
            _grid = new Gem[rows, cols];
        }

        public int Rows => _rows;

        public int Columns => _cols;

        public GemKind KindAt(int row, int col) =>
            row >= 0 && row < _rows && col >= 0 && col < _cols
                ? (_grid[row, col]?.Kind ?? GemKind.None)
                : GemKind.None;

        public Gem GetGem(int row, int col) => InBounds(row, col) ? _grid[row, col] : null;

        private bool InBounds(int row, int col) => row >= 0 && row < _rows && col >= 0 && col < _cols;

        /// <summary>重置棋盘：无初始消除、保证有解。</summary>
        public void Reset()
        {
            for (var attempt = 0; attempt < 50; attempt++)
            {
                FillWithoutMatches();
                if (HasPossibleMove())
                {
                    return;
                }
            }

            // 兜底：极端情况下也给出一个无匹配棋盘
            FillWithoutMatches();
        }

        private void FillWithoutMatches()
        {
            _grid = new Gem[_rows, _cols];
            for (var r = 0; r < _rows; r++)
            {
                for (var c = 0; c < _cols; c++)
                {
                    var kind = RollKindWithoutMatchAt(r, c);
                    _grid[r, c] = new Gem(_nextGemId++, kind);
                }
            }
        }

        private GemKind RollKindWithoutMatchAt(int row, int col)
        {
            while (true)
            {
                var kind = (GemKind)_rng.Next(1, _colorCount + 1);
                if (!WouldCompleteMatchAt(row, col, kind))
                {
                    return kind;
                }
            }
        }

        /// <summary>假设 (row,col) 放 kind 是否与左侧/下侧已有宝石形成三连。</summary>
        private bool WouldCompleteMatchAt(int row, int col, GemKind kind)
        {
            var left1 = KindAt(row, col - 1);
            var left2 = KindAt(row, col - 2);
            if (kind != GemKind.None && kind == left1 && kind == left2)
            {
                return true;
            }

            var down1 = KindAt(row - 1, col);
            var down2 = KindAt(row - 2, col);
            return kind != GemKind.None && kind == down1 && kind == down2;
        }

        /// <summary>
        /// 尝试交换相邻两格：不产生消除则还原并返回 Valid=false；
        /// 产生消除则逐步结算（消除/下落/补充/级联），末尾视需要追加 Shuffle 步骤。
        /// </summary>
        public SwapResult TrySwap(CellPos a, CellPos b)
        {
            var result = new SwapResult();
            if (!a.IsAdjacentTo(b))
            {
                return result;
            }

            var gemA = _grid[a.Row, a.Col];
            var gemB = _grid[b.Row, b.Col];
            if (gemA == null || gemB == null)
            {
                return result;
            }

            _grid[a.Row, a.Col] = gemB;
            _grid[b.Row, b.Col] = gemA;

            if (FindMatchedPositions().Count == 0)
            {
                _grid[a.Row, a.Col] = gemA;
                _grid[b.Row, b.Col] = gemB;
                return result;
            }

            result.Valid = true;
            ResolveCascades(result.Steps);

            if (!HasPossibleMove())
            {
                var shuffleStep = Reshuffle();
                result.Steps.Add(shuffleStep);
            }

            return result;
        }

        private void ResolveCascades(List<BoardStep> steps)
        {
            var cascade = 0;
            while (true)
            {
                var matched = FindMatchedPositions();
                if (matched.Count == 0)
                {
                    return;
                }

                cascade++;
                var matchStep = new BoardStep { Kind = BoardStepKind.Match, CascadeIndex = cascade };
                foreach (var pos in matched)
                {
                    matchStep.Removed.Add(new RemovedGem(pos, _grid[pos.Row, pos.Col]));
                    _grid[pos.Row, pos.Col] = null;
                }

                steps.Add(matchStep);
                ApplyGravity(steps);
                Refill(steps);
            }
        }

        private void ApplyGravity(List<BoardStep> steps)
        {
            var fallStep = new BoardStep { Kind = BoardStepKind.Fall };
            for (var c = 0; c < _cols; c++)
            {
                var writeRow = 0;
                for (var readRow = 0; readRow < _rows; readRow++)
                {
                    var gem = _grid[readRow, c];
                    if (gem == null)
                    {
                        continue;
                    }

                    if (writeRow != readRow)
                    {
                        fallStep.Moved.Add(new MovedGem(new CellPos(readRow, c), new CellPos(writeRow, c), gem));
                        _grid[writeRow, c] = gem;
                        _grid[readRow, c] = null;
                    }

                    writeRow++;
                }
            }

            if (fallStep.Moved.Count > 0)
            {
                steps.Add(fallStep);
            }
        }

        private void Refill(List<BoardStep> steps)
        {
            var refillStep = new BoardStep { Kind = BoardStepKind.Refill };
            for (var c = 0; c < _cols; c++)
            {
                for (var r = _rows - 1; r >= 0; r--)
                {
                    if (_grid[r, c] != null)
                    {
                        continue;
                    }

                    var kind = (GemKind)_rng.Next(1, _colorCount + 1);
                    var gem = new Gem(_nextGemId++, kind);
                    _grid[r, c] = gem;
                    refillStep.Added.Add(new NewGem(new CellPos(r, c), gem));
                }
            }

            if (refillStep.Added.Count > 0)
            {
                steps.Add(refillStep);
            }
        }

        /// <summary>洗牌：保持宝石多重集合不变重排；无解/有现成消除则重试，仍失败则重新生成。返回 Shuffle 步骤。</summary>
        private BoardStep Reshuffle()
        {
            var kinds = new List<GemKind>();
            foreach (var gem in _grid)
            {
                if (gem != null)
                {
                    kinds.Add(gem.Kind);
                }
            }

            for (var attempt = 0; attempt < 60; attempt++)
            {
                // Fisher-Yates 打乱后回填
                for (var i = kinds.Count - 1; i > 0; i--)
                {
                    var j = _rng.Next(i + 1);
                    (kinds[i], kinds[j]) = (kinds[j], kinds[i]);
                }

                var index = 0;
                for (var r = 0; r < _rows; r++)
                {
                    for (var c = 0; c < _cols; c++)
                    {
                        _grid[r, c].Kind = kinds[index++];
                    }
                }

                if (FindMatchedPositions().Count == 0 && HasPossibleMove())
                {
                    return BuildShuffleStep();
                }
            }

            Reset();
            return BuildShuffleStep();
        }

        private BoardStep BuildShuffleStep()
        {
            var step = new BoardStep { Kind = BoardStepKind.Shuffle };
            for (var r = 0; r < _rows; r++)
            {
                for (var c = 0; c < _cols; c++)
                {
                    step.Added.Add(new NewGem(new CellPos(r, c), _grid[r, c]));
                }
            }

            return step;
        }

        /// <summary>全部三连（横/竖 run>=3）位置集合，按行序稳定输出。</summary>
        public List<CellPos> FindMatchedPositions()
        {
            var matched = new HashSet<CellPos>();

            for (var r = 0; r < _rows; r++)
            {
                var runStart = 0;
                for (var c = 1; c <= _cols; c++)
                {
                    var prev = KindAt(r, c - 1);
                    var cur = c < _cols ? KindAt(r, c) : GemKind.None;
                    if (cur == prev && cur != GemKind.None)
                    {
                        continue;
                    }

                    if (prev != GemKind.None && c - runStart >= 3)
                    {
                        for (var k = runStart; k < c; k++)
                        {
                            matched.Add(new CellPos(r, k));
                        }
                    }

                    runStart = c;
                }
            }

            for (var c = 0; c < _cols; c++)
            {
                var runStart = 0;
                for (var r = 1; r <= _rows; r++)
                {
                    var prev = KindAt(r - 1, c);
                    var cur = r < _rows ? KindAt(r, c) : GemKind.None;
                    if (cur == prev && cur != GemKind.None)
                    {
                        continue;
                    }

                    if (prev != GemKind.None && r - runStart >= 3)
                    {
                        for (var k = runStart; k < r; k++)
                        {
                            matched.Add(new CellPos(k, c));
                        }
                    }

                    runStart = r;
                }
            }

            var ordered = new List<CellPos>(matched);
            ordered.Sort((x, y) => x.Row != y.Row ? x.Row.CompareTo(y.Row) : x.Col.CompareTo(y.Col));
            return ordered;
        }

        /// <summary>是否存在能产生消除的相邻交换。</summary>
        public bool HasPossibleMove()
        {
            for (var r = 0; r < _rows; r++)
            {
                for (var c = 0; c < _cols; c++)
                {
                    if (c + 1 < _cols && SwapCreatesMatch(r, c, r, c + 1))
                    {
                        return true;
                    }

                    if (r + 1 < _rows && SwapCreatesMatch(r, c, r + 1, c))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool SwapCreatesMatch(int r1, int c1, int r2, int c2)
        {
            (_grid[r1, c1], _grid[r2, c2]) = (_grid[r2, c2], _grid[r1, c1]);
            var creates = FindMatchedPositions().Count > 0;
            (_grid[r1, c1], _grid[r2, c2]) = (_grid[r2, c2], _grid[r1, c1]);
            return creates;
        }
    }
}

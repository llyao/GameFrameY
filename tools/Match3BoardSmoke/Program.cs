using System;
using System.Collections.Generic;
using GameFrameY.Game.Modules.Match3.Board;

// 消消乐棋盘不变量冒烟测试：dotnet run
// 1. Reset 后无现成消除且有解
// 2. 无效交换不改变棋盘
// 3. 有效交换结算后：棋盘满、无残余消除、有解、步骤合法
internal static class Program
{
    private static int _failures;

    private static int Main()
    {
        const int rows = 8, cols = 8;
        var rng = new Random(42);
        long validSwaps = 0, cascades = 0, shuffles = 0, maxCascade = 0;

        for (var trial = 0; trial < 300; trial++)
        {
            var board = new Match3Board(rows, cols, 3 + trial % 4, seed: 1000 + trial);
            board.Reset();
            Check(board.FindMatchedPositions().Count == 0, $"trial{trial}: Reset 后存在现成消除");
            Check(board.HasPossibleMove(), $"trial{trial}: Reset 后无可消组合");

            for (var attempt = 0; attempt < 400; attempt++)
            {
                var a = new CellPos(rng.Next(rows), rng.Next(cols));
                var d = new[] { (1, 0), (-1, 0), (0, 1), (0, -1) }[rng.Next(4)];
                var b = new CellPos(a.Row + d.Item1, a.Col + d.Item2);
                if (b.Row < 0 || b.Row >= rows || b.Col < 0 || b.Col >= cols)
                {
                    continue;
                }

                var before = Snapshot(board);
                var result = board.TrySwap(a, b);
                if (!result.Valid)
                {
                    Check(SameAs(before, board), $"trial{trial}: 无效交换改变了棋盘 {a}<->{b}");
                    continue;
                }

                validSwaps++;
                var seenRemoved = new HashSet<int>();
                foreach (var step in result.Steps)
                {
                    switch (step.Kind)
                    {
                        case BoardStepKind.Match:
                            Check(step.Removed.Count >= 3, $"trial{trial}: 单次消除少于 3");
                            Check(step.CascadeIndex >= 1, $"trial{trial}: 级联序号非法");
                            maxCascade = Math.Max(maxCascade, step.CascadeIndex);
                            cascades += step.CascadeIndex > 1 ? 1 : 0;
                            foreach (var removed in step.Removed)
                            {
                                Check(seenRemoved.Add(removed.Gem.Id), $"trial{trial}: 同一宝石被消除两次 id={removed.Gem.Id}");
                            }

                            break;
                        case BoardStepKind.Fall:
                            foreach (var moved in step.Moved)
                            {
                                Check(moved.From != moved.To, $"trial{trial}: Fall 位置相同");
                            }

                            break;
                        case BoardStepKind.Refill:
                            foreach (var added in step.Added)
                            {
                                Check(!seenRemoved.Contains(added.Gem.Id), $"trial{trial}: 补充复用了已消除宝石 id");
                            }

                            break;
                        case BoardStepKind.Shuffle:
                            shuffles++;
                            Check(step.Added.Count == rows * cols, $"trial{trial}: 洗牌步骤未覆盖全盘");
                            break;
                    }
                }

                Check(GridFull(board), $"trial{trial}: 结算后棋盘有空位");
                Check(board.FindMatchedPositions().Count == 0, $"trial{trial}: 结算后有残余消除");
                Check(board.HasPossibleMove(), $"trial{trial}: 结算后无可消组合");
            }
        }

        Console.WriteLine($"有效交换 {validSwaps} 次，级联 {cascades} 次，最大级联 x{maxCascade}，自动洗牌 {shuffles} 次");
        if (_failures > 0)
        {
            Console.WriteLine($"FAILED: {_failures} 个断言失败");
            return 1;
        }

        Console.WriteLine("ALL OK");
        return 0;
    }

    private static void Check(bool condition, string message)
    {
        if (condition)
        {
            return;
        }

        _failures++;
        Console.Error.WriteLine("FAIL: " + message);
    }

    private static GemKind[] Snapshot(Match3Board board)
    {
        var kinds = new GemKind[board.Rows * board.Columns];
        for (var r = 0; r < board.Rows; r++)
        {
            for (var c = 0; c < board.Columns; c++)
            {
                kinds[r * board.Columns + c] = board.KindAt(r, c);
            }
        }

        return kinds;
    }

    private static bool SameAs(GemKind[] snapshot, Match3Board board)
    {
        var current = Snapshot(board);
        for (var i = 0; i < snapshot.Length; i++)
        {
            if (snapshot[i] != current[i])
            {
                return false;
            }
        }

        return true;
    }

    private static bool GridFull(Match3Board board)
    {
        for (var r = 0; r < board.Rows; r++)
        {
            for (var c = 0; c < board.Columns; c++)
            {
                if (board.GetGem(r, c) == null)
                {
                    return false;
                }
            }
        }

        return true;
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFrameY.Game.Modules.Match3.Board;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFrameY.Game.Modules.Match3
{
    /// <summary>
    /// 消消乐 UI：全部运行时构建（Canvas/HUD/棋盘/结算面板），无 prefab 依赖。
    /// 动画用 UniTask 补间（UiKit.Tween），播放期间 Busy=true 屏蔽输入。
    /// </summary>
    public sealed class Match3UI : IDisposable
    {
        private readonly Match3Assets _assets;
        private readonly CancellationTokenSource _cts = new();

        private GameObject _root;
        private RectTransform _rootRect;
        private GameObject _eventSystem;
        private RectTransform _boardRoot;
        private RectTransform _cellLayer;
        private RectTransform _gemLayer;
        private RectTransform _popupLayer;
        private Text _levelText;
        private Text _movesText;
        private Text _scoreText;
        private Text _bannerText;
        private RectTransform _scoreFill;
        private RectTransform _overlayRoot;
        private Text _overlayTitle;
        private Text _overlaySubtitle;

        private readonly Dictionary<CellPos, GemView> _views = new();
        private CellPos? _selected;
        private GemView _selectedView;
        private int _rows;
        private int _cols;
        private float _cellSize = 120f;

        /// <summary>逻辑层（控制器）订阅：请求交换两格。</summary>
        public Action<CellPos, CellPos> SwapRequested;

        public Action RestartRequested;

        public bool Busy { get; private set; }

        public float DragThreshold => _cellSize * 0.35f;

        public Match3UI(Match3Assets assets)
        {
            _assets = assets;
        }

        public void Build()
        {
            _root = new GameObject("Match3Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            _rootRect = _root.GetComponent<RectTransform>();
            var canvas = _root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = _root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            if (EventSystem.current == null)
            {
                _eventSystem = new GameObject("Match3EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }

            BuildHud();
            BuildBoardRoot();
            BuildOverlay();
        }

        private void BuildHud()
        {
            var hud = UiKit.Rect(_rootRect, "HUD",
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -300), Vector2.zero);

            _levelText = UiKit.Text(hud, "Level", "第 1 关", 46, Color.white, TextAnchor.MiddleLeft, FontStyle.Bold);
            _levelText.rectTransform.anchoredPosition = new Vector2(-280, -70);
            _levelText.rectTransform.sizeDelta = new Vector2(520, 80);

            _movesText = UiKit.Text(hud, "Moves", "步数 20", 52, Color.white, TextAnchor.MiddleRight, FontStyle.Bold);
            _movesText.rectTransform.anchoredPosition = new Vector2(280, -70);
            _movesText.rectTransform.sizeDelta = new Vector2(520, 80);

            var bar = UiKit.Centered(hud, "ScoreBar", new Vector2(0, -165), new Vector2(960, 34));
            var barBg = bar.gameObject.AddComponent<Image>();
            barBg.color = new Color32(20, 24, 38, 200);
            var fill = UiKit.Rect(bar, "Fill",
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(3, -3), new Vector2(-3, 3));
            var fillImage = fill.gameObject.AddComponent<Image>();
            fillImage.color = new Color32(96, 220, 120, 255);
            _scoreFill = fill;

            _scoreText = UiKit.Text(hud, "Score", "0 / 0", 40, Color.white);
            _scoreText.rectTransform.anchoredPosition = new Vector2(0, -232);
            _scoreText.rectTransform.sizeDelta = new Vector2(800, 64);

            _bannerText = UiKit.Text(_rootRect, "Banner", string.Empty, 64,
                new Color32(255, 224, 96, 255), TextAnchor.MiddleCenter, FontStyle.Bold);
            _bannerText.rectTransform.anchoredPosition = new Vector2(0, 500);
            _bannerText.rectTransform.sizeDelta = new Vector2(900, 120);
            _bannerText.gameObject.SetActive(false);

            UiKit.Button(_rootRect, "Restart", "重新开始", _assets.ButtonBg,
                new Vector2(0, -880), new Vector2(360, 110), () => RestartRequested?.Invoke());
        }

        private void BuildBoardRoot()
        {
            _boardRoot = UiKit.Centered(_root.transform, "Board", new Vector2(0, -80), new Vector2(1000, 1000));
            _cellLayer = UiKit.Rect(_boardRoot, "Cells", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            _gemLayer = UiKit.Rect(_boardRoot, "Gems", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            _popupLayer = UiKit.Rect(_boardRoot, "Popups", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        }

        private void BuildOverlay()
        {
            _overlayRoot = UiKit.Rect(_rootRect, "ResultOverlay",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            var dim = _overlayRoot.gameObject.AddComponent<Image>();
            dim.color = new Color32(0, 0, 0, 158);
            dim.raycastTarget = true;

            var panel = UiKit.Centered(_overlayRoot, "Panel", Vector2.zero, new Vector2(720, 620));
            var panelImage = panel.gameObject.AddComponent<Image>();
            panelImage.sprite = _assets.PanelBg;
            panelImage.color = Color.white;

            _overlayTitle = UiKit.Text(panel, "Title", "过关！", 68, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
            _overlayTitle.rectTransform.anchoredPosition = new Vector2(0, 180);

            _overlaySubtitle = UiKit.Text(panel, "Subtitle", string.Empty, 44, new Color32(220, 228, 255, 255));
            _overlaySubtitle.rectTransform.anchoredPosition = new Vector2(0, 40);

            UiKit.Button(panel, "Action", "继续", _assets.ButtonBg,
                new Vector2(0, -190), new Vector2(380, 120), HideResultThenAction);
            _actionCallback = null;
            _overlayRoot.gameObject.SetActive(false);
        }

        private Action _actionCallback;

        private void HideResultThenAction()
        {
            _overlayRoot.gameObject.SetActive(false);
            var callback = _actionCallback;
            _actionCallback = null;
            callback?.Invoke();
        }

        /// <summary>显示结算面板（win=过关/失败），按钮文案与回调由调用方给。</summary>
        public void ShowResult(bool win, int score, string buttonLabel, Action onButton)
        {
            _overlayTitle.text = win ? "过关！" : "步数用完";
            _overlayTitle.color = win ? new Color32(120, 235, 140, 255) : new Color32(255, 130, 120, 255);
            _overlaySubtitle.text = $"得分 {score}";
            _actionCallback = onButton;
            _overlayRoot.gameObject.SetActive(true);
        }

        public void HideResult()
        {
            _actionCallback = null;
            if (_overlayRoot != null)
            {
                _overlayRoot.gameObject.SetActive(false);
            }
        }

        public void RefreshHud(int levelId, int score, int targetScore, int moves)
        {
            if (_levelText == null)
            {
                return;
            }

            _levelText.text = $"第 {levelId} 关";
            _movesText.text = $"步数 {moves}";
            _scoreText.text = $"{score} / {targetScore}";
            var progress = targetScore > 0 ? Mathf.Clamp01(score / (float)targetScore) : 0f;
            _scoreFill.anchorMax = new Vector2(Mathf.Max(progress, 0.001f), 1f);
        }

        /// <summary>按棋盘当前状态整体重建视图（开局/洗牌/换关）。</summary>
        public void RebuildBoard(Match3Board board)
        {
            _rows = board.Rows;
            _cols = board.Columns;
            const float boardPx = 1000f;
            _cellSize = boardPx / Mathf.Max(_rows, _cols);
            _boardRoot.sizeDelta = new Vector2(_cellSize * _cols, _cellSize * _rows);

            UiKit.DestroyAllChildren(_cellLayer);
            UiKit.DestroyAllChildren(_gemLayer);
            UiKit.DestroyAllChildren(_popupLayer);
            _views.Clear();
            Deselect();

            for (var r = 0; r < _rows; r++)
            {
                for (var c = 0; c < _cols; c++)
                {
                    var cell = UiKit.Image(_cellLayer, $"cell_{r}_{c}", _assets.CellBg, Color.white);
                    cell.rectTransform.anchoredPosition = CellToLocal(r, c);
                    var size = _cellSize * 0.96f;
                    cell.rectTransform.sizeDelta = new Vector2(size, size);
                }
            }

            for (var r = 0; r < _rows; r++)
            {
                for (var c = 0; c < _cols; c++)
                {
                    var gem = board.GetGem(r, c);
                    if (gem != null)
                    {
                        CreateGemView(gem, r, c, spawnAnim: false);
                    }
                }
            }
        }

        private GemView CreateGemView(Gem gem, int row, int col, bool spawnAnim)
        {
            var pos = new CellPos(row, col);
            var image = UiKit.Image(_gemLayer, $"gem_{gem.Id}",
                _assets.GemSprites[(int)gem.Kind], Color.white);
            var size = _cellSize * 0.84f;
            image.rectTransform.sizeDelta = new Vector2(size, size);
            image.rectTransform.anchoredPosition = CellToLocal(row, col);
            image.raycastTarget = true;

            var view = image.gameObject.AddComponent<GemView>();
            view.Owner = this;
            view.Pos = pos;
            view.Image = image;
            _views[pos] = view;

            if (spawnAnim)
            {
                image.rectTransform.localScale = Vector3.zero;
                UiKit.Tween(image.gameObject, 0.15f,
                        t => { if (image) image.rectTransform.localScale = Vector3.one * t; }, _cts.Token)
                    .Forget();
            }

            return view;
        }

        private Vector2 CellToLocal(int row, int col)
        {
            var x = (col - (_cols - 1) * 0.5f) * _cellSize;
            var y = (row - (_rows - 1) * 0.5f) * _cellSize;
            return new Vector2(x, y);
        }

        // ===== 输入 =====

        internal void OnGemTapped(GemView view)
        {
            if (Busy)
            {
                return;
            }

            if (_selected == null || _selectedView == null)
            {
                Select(view);
                return;
            }

            if (_selected.Value == view.Pos)
            {
                Deselect();
                return;
            }

            if (_selected.Value.IsAdjacentTo(view.Pos))
            {
                var from = _selected.Value;
                Deselect();
                SwapRequested?.Invoke(from, view.Pos);
            }
            else
            {
                Select(view);
            }
        }

        internal void RequestSwapByDrag(GemView view, Vector2Int dir)
        {
            if (Busy)
            {
                return;
            }

            var target = new CellPos(view.Pos.Row + dir.y, view.Pos.Col + dir.x);
            if (target.Row < 0 || target.Row >= _rows || target.Col < 0 || target.Col >= _cols)
            {
                return;
            }

            Deselect();
            SwapRequested?.Invoke(view.Pos, target);
        }

        private void Select(GemView view)
        {
            Deselect();
            _selected = view.Pos;
            _selectedView = view;
            if (view != null)
            {
                view.transform.localScale = Vector3.one * 1.18f;
            }
        }

        private void Deselect()
        {
            if (_selectedView != null)
            {
                _selectedView.transform.localScale = Vector3.one;
            }

            _selected = null;
            _selectedView = null;
        }

        // ===== 动画播放 =====

        /// <summary>有效交换：两颗宝石互换位置动画（逻辑棋盘已交换，视图只做表现）。</summary>
        public async UniTask PlaySwap(CellPos a, CellPos b)
        {
            var viewA = _views.GetValueOrDefault(a);
            var viewB = _views.GetValueOrDefault(b);
            _views[a] = viewB;
            _views[b] = viewA;
            if (viewA != null)
            {
                viewA.Pos = b;
            }

            if (viewB != null)
            {
                viewB.Pos = a;
            }

            await UniTask.WhenAll(
                MoveTo(viewA, CellToLocal(b.Row, b.Col), 0.12f),
                MoveTo(viewB, CellToLocal(a.Row, a.Col), 0.12f));
        }

        /// <summary>无效交换：换过去再弹回来的否定反馈。</summary>
        public async UniTask PlayInvalidSwap(CellPos a, CellPos b)
        {
            await PlaySwap(a, b);
            await UniTask.Delay(TimeSpan.FromMilliseconds(70), cancellationToken: _cts.Token);
            await PlaySwap(a, b);
        }

        public async UniTask PlaySteps(List<BoardStep> steps, Func<BoardStep, int> scoreOf)
        {
            Busy = true;
            try
            {
                foreach (var step in steps)
                {
                    switch (step.Kind)
                    {
                        case BoardStepKind.Match:
                            await PlayMatch(step, scoreOf(step));
                            break;
                        case BoardStepKind.Fall:
                            await PlayFall(step);
                            break;
                        case BoardStepKind.Refill:
                            await PlayRefill(step);
                            break;
                        case BoardStepKind.Shuffle:
                            await PlayShuffle(step);
                            break;
                    }
                }
            }
            finally
            {
                Busy = false;
            }
        }

        private async UniTask PlayMatch(BoardStep step, int gained)
        {
            var centroid = Vector2.zero;
            var tasks = new List<UniTask>();
            foreach (var removed in step.Removed)
            {
                centroid += CellToLocal(removed.Pos.Row, removed.Pos.Col);
                if (_views.Remove(removed.Pos, out var view))
                {
                    tasks.Add(PopOut(view));
                }
            }

            if (step.Removed.Count > 0)
            {
                centroid /= step.Removed.Count;
            }

            if (step.CascadeIndex >= 2)
            {
                ShowBanner($"Combo x{step.CascadeIndex}", 0.55f);
            }

            if (gained > 0)
            {
                SpawnScorePopup(centroid, gained, step.CascadeIndex);
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask PlayFall(BoardStep step)
        {
            var tasks = new List<UniTask>();
            foreach (var moved in step.Moved)
            {
                if (!_views.TryGetValue(moved.From, out var view))
                {
                    continue;
                }

                _views.Remove(moved.From);
                _views[moved.To] = view;
                view.Pos = moved.To;
                tasks.Add(MoveTo(view, CellToLocal(moved.To.Row, moved.To.Col), 0.18f));
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask PlayRefill(BoardStep step)
        {
            var tasks = new List<UniTask>();
            foreach (var added in step.Added)
            {
                var view = CreateGemView(added.Gem, added.Pos.Row, added.Pos.Col, spawnAnim: false);
                var target = CellToLocal(added.Pos.Row, added.Pos.Col);
                var start = target + new Vector2(0, (_rows - added.Pos.Row) * _cellSize);
                view.Image.rectTransform.anchoredPosition = start;
                var image = view.Image;
                tasks.Add(UiKit.Tween(view.gameObject, 0.22f,
                    t => { if (image) image.rectTransform.anchoredPosition = Vector2.Lerp(start, target, t * t); },
                    _cts.Token));
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask PlayShuffle(BoardStep step)
        {
            ShowBanner("无可消除，重新洗牌", 0.9f);
            await UniTask.Delay(TimeSpan.FromMilliseconds(350), cancellationToken: _cts.Token);

            UiKit.DestroyAllChildren(_gemLayer);
            _views.Clear();
            Deselect();
            foreach (var added in step.Added)
            {
                CreateGemView(added.Gem, added.Pos.Row, added.Pos.Col, spawnAnim: true);
            }

            await UniTask.Delay(TimeSpan.FromMilliseconds(200), cancellationToken: _cts.Token);
        }

        private async UniTask PopOut(GemView view)
        {
            var go = view.gameObject;
            await UiKit.Tween(go, 0.18f, t =>
            {
                if (!go)
                {
                    return;
                }

                var s = t < 0.35f
                    ? Mathf.Lerp(1f, 1.3f, t / 0.35f)
                    : Mathf.Lerp(1.3f, 0f, (t - 0.35f) / 0.65f);
                go.transform.localScale = new Vector3(s, s, 1f);
            }, _cts.Token);
            if (go)
            {
                UnityEngine.Object.Destroy(go);
            }
        }

        private async UniTask MoveTo(GemView view, Vector2 target, float duration)
        {
            if (view == null)
            {
                return;
            }

            var image = view.Image;
            var start = image.rectTransform.anchoredPosition;
            await UiKit.Tween(view.gameObject, duration,
                t => { if (image) image.rectTransform.anchoredPosition = Vector2.Lerp(start, target, t); },
                _cts.Token);
            if (image)
            {
                image.rectTransform.anchoredPosition = target;
            }
        }

        private void SpawnScorePopup(Vector2 localPos, int gained, int cascade)
        {
            var text = UiKit.Text(_popupLayer, "ScorePopup", $"+{gained}",
                40 + Mathf.Min(cascade, 4) * 10,
                cascade >= 2 ? new Color32(255, 214, 72, 255) : Color.white,
                TextAnchor.MiddleCenter, FontStyle.Bold);
            text.rectTransform.anchoredPosition = localPos;
            text.rectTransform.sizeDelta = new Vector2(320, 90);
            var group = text.gameObject.AddComponent<CanvasGroup>();

            UiKit.Tween(text.gameObject, 0.7f, t =>
            {
                if (!text)
                {
                    return;
                }

                text.rectTransform.anchoredPosition = localPos + new Vector2(0, 130f * t);
                group.alpha = 1f - t * t;
            }, _cts.Token).ContinueWith(() => { if (text) UnityEngine.Object.Destroy(text.gameObject); }).Forget();
        }

        private void ShowBanner(string message, float seconds)
        {
            if (_bannerText == null)
            {
                return;
            }

            _bannerText.text = message;
            _bannerText.gameObject.SetActive(true);
            HideBannerAsync(seconds).Forget();
        }

        private async UniTask HideBannerAsync(float seconds)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(seconds * 1000), cancellationToken: _cts.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (_bannerText != null)
            {
                _bannerText.gameObject.SetActive(false);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            if (_root != null)
            {
                UnityEngine.Object.Destroy(_root);
            }

            if (_eventSystem != null)
            {
                UnityEngine.Object.Destroy(_eventSystem);
            }
        }
    }
}

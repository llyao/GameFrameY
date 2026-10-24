using GameFrameY.Game.Modules.Match3.Board;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFrameY.Game.Modules.Match3
{
    /// <summary>宝石视图：承载拖拽换向与点选输入，逻辑交还 Match3UI/控制器。</summary>
    internal sealed class GemView : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public Match3UI Owner;
        public CellPos Pos;
        public Image Image;

        private Vector2 _downPosition;
        private bool _dragConsumed;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _downPosition = eventData.position;
            _dragConsumed = false;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (Owner == null || Owner.Busy || _dragConsumed)
            {
                return;
            }

            var delta = eventData.position - _downPosition;
            if (delta.magnitude < Owner.DragThreshold)
            {
                return;
            }

            _dragConsumed = true;
            var dir = Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
                ? new Vector2Int(Mathf.RoundToInt(Mathf.Sign(delta.x)), 0)
                : new Vector2Int(0, Mathf.RoundToInt(Mathf.Sign(delta.y)));
            Owner.RequestSwapByDrag(this, dir);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (_dragConsumed)
            {
                return;
            }

            Owner?.OnGemTapped(this);
        }
    }
}

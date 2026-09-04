using DG.Tweening;
using KBCore.Refs;
using UnityEngine;

namespace Project.Assets._Project._Scripts.UI
{
    public class AppearAnimationOnEnable : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private RectTransform _rectTransform;
        [SerializeField] private float _startingDelay = 0.5f;
        [SerializeField] private bool _timeScaleIndependent = true;
        [SerializeField] private float _duration = 0.5f;

        [SerializeField] private Vector3 _offsetFromOriginalPos = Vector3.zero;
        [SerializeField] private bool _scaleFromZero = false;
        private bool ScaleFromZero => _scaleFromZero;
        [Alchemy.Inspector.ShowIf(nameof(ScaleFromZero))]
        [SerializeField] private float _scalingDuration = 0.3f;
        [Alchemy.Inspector.ShowIf(nameof(ScaleFromZero))]

        [SerializeField] private Ease _scalingEase = Ease.OutBack;
        [SerializeField] private Ease _animationEase = Ease.OutCubic;
        private Tween _scalingTween;
        private Tween _tween;
        private Tween _startingDelayTween;
        private Vector2 _startingPos;
        private Vector3 _startingScale;
        public bool IsInTween => _scalingTween.IsPlaying() || _tween.IsPlaying() || _startingDelayTween.IsPlaying();

        private void Awake()
        {
            _startingPos = _rectTransform.anchoredPosition;
            _startingScale = _rectTransform.localScale;
        }

        private void OnEnable()
        {
            if (_scaleFromZero)
            {
                _rectTransform.localScale = Vector3.zero;
            }
            _startingDelayTween?.Kill();
            _rectTransform.anchoredPosition =
                        _startingPos + (Vector2)_offsetFromOriginalPos;
            _rectTransform.localScale = Vector3.zero;
            _startingDelayTween = DOVirtual.DelayedCall(_startingDelay,
                () =>
                {
                    _tween?.Kill();
                    _scalingTween?.Kill();
                    _scalingTween = _rectTransform.DOScale(_startingScale, _scalingDuration)
                        .From(Vector3.zero)
                        .SetEase(_scalingEase)
                        .SetUpdate(_timeScaleIndependent);
                    _tween = _rectTransform.DOAnchorPos(_startingPos, _duration)
                        .SetEase(_animationEase)
                        .SetUpdate(_timeScaleIndependent);
                }
                )
                .SetUpdate(_timeScaleIndependent);
            
        }
    }
}

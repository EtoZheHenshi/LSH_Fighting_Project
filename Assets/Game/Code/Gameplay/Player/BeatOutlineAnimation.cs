using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Code.Gameplay.UI.HUD
{
    public sealed class BeatOutlineAnimation : MonoBehaviour
    {
        public bool IsPlaying => _sequence is { active: true };
        
        private enum AnimationState
        {
            Idle,
            WaitingForBeat
        }

        [Header("Players")]
        [SerializeField] private SpriteRenderer playerOneRenderer;
        [SerializeField] private SpriteRenderer playerTwoRenderer;

        [Header("Shader properties")]
        [SerializeField] private string outlineColorProperty = "_OutlineColor";
        [SerializeField] private string outlineSizeProperty = "_OutlineSize";

        [Header("Animation")]
        [Min(0.01f)]
        [SerializeField] private float duration = 0.35f;

        [Min(1f)]
        [SerializeField] private float pulseSizeMultiplier = 1.75f;

        [SerializeField] private Ease pulseInEase = Ease.OutQuad;
        [SerializeField] private Ease pulseOutEase = Ease.InQuad;
        [SerializeField] private Ease colorSwapEase = Ease.InOutSine;

        private MaterialPropertyBlock _playerOneBlock;
        private MaterialPropertyBlock _playerTwoBlock;

        private int _outlineColorId;
        private int _outlineSizeId;

        private Color _playerOneColor;
        private Color _playerTwoColor;

        private float _playerOneBaseSize;
        private float _playerTwoBaseSize;

        private float _playerOneCurrentSize;
        private float _playerTwoCurrentSize;

        private AnimationState _state;
        private Sequence _sequence;
        private UniTaskCompletionSource _completionSource;

        private void Awake()
        {
            _outlineColorId = Shader.PropertyToID(outlineColorProperty);
            _outlineSizeId = Shader.PropertyToID(outlineSizeProperty);

            _playerOneBlock = new MaterialPropertyBlock();
            _playerTwoBlock = new MaterialPropertyBlock();

            ValidateRenderer(playerOneRenderer);
            ValidateRenderer(playerTwoRenderer);

            ReadCurrentValues();

            _state = AnimationState.Idle;
        }

        /// <summary>
        /// Вызывается при начале смены ролей.
        /// Анимация произойдёт на следующем событии бита.
        /// </summary>
        public void PrepareRoleSwap()
        {
            _sequence?.Kill();

            _completionSource?.TrySetCanceled();
            _completionSource = new UniTaskCompletionSource();

            ReadCurrentValues();
            ResetSizes();

            _state = AnimationState.WaitingForBeat;
        }

        public UniTask WaitForCompletion()
        {
            return _completionSource?.Task ?? UniTask.CompletedTask;
        }

        public void OnBeat()
        {
            if (_state != AnimationState.WaitingForBeat)
                return;

            _state = AnimationState.Idle;

            PlayPulseAndSwap();
        }

private void PlayPulseAndSwap()
    {
        _sequence?.Kill();

        Color playerOneTargetColor = _playerTwoColor;
        Color playerTwoTargetColor = _playerOneColor;

        float halfDuration = duration * 0.5f;

        float playerOnePulseSize =
            _playerOneBaseSize * pulseSizeMultiplier;

        float playerTwoPulseSize =
            _playerTwoBaseSize * pulseSizeMultiplier;

        _sequence = DOTween.Sequence()
            .SetUpdate(UpdateType.Normal, true)
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        _sequence.Join(
            DOTween.To(
                    () => _playerOneColor,
                    value =>
                    {
                        _playerOneColor = value;
                        ApplyPlayerOne();
                    },
                    playerOneTargetColor,
                    duration)
                .SetEase(colorSwapEase));

        _sequence.Join(
            DOTween.To(
                    () => _playerTwoColor,
                    value =>
                    {
                        _playerTwoColor = value;
                        ApplyPlayerTwo();
                    },
                    playerTwoTargetColor,
                    duration)
                .SetEase(colorSwapEase));

        _sequence.Join(
            DOTween.To(
                    () => _playerOneCurrentSize,
                    value =>
                    {
                        _playerOneCurrentSize = value;
                        ApplyPlayerOne();
                    },
                    playerOnePulseSize,
                    halfDuration)
                .SetEase(pulseInEase));

        _sequence.Join(
            DOTween.To(
                    () => _playerTwoCurrentSize,
                    value =>
                    {
                        _playerTwoCurrentSize = value;
                        ApplyPlayerTwo();
                    },
                    playerTwoPulseSize,
                    halfDuration)
                .SetEase(pulseInEase));

        _sequence.Insert(
            halfDuration,
            DOTween.To(
                    () => _playerOneCurrentSize,
                    value =>
                    {
                        _playerOneCurrentSize = value;
                        ApplyPlayerOne();
                    },
                    _playerOneBaseSize,
                    halfDuration)
                .SetEase(pulseOutEase));

        _sequence.Insert(
            halfDuration,
            DOTween.To(
                    () => _playerTwoCurrentSize,
                    value =>
                    {
                        _playerTwoCurrentSize = value;
                        ApplyPlayerTwo();
                    },
                    _playerTwoBaseSize,
                    halfDuration)
                .SetEase(pulseOutEase));

        _sequence.OnComplete(() =>
        {
            _playerOneColor = playerOneTargetColor;
            _playerTwoColor = playerTwoTargetColor;

            _playerOneCurrentSize = _playerOneBaseSize;
            _playerTwoCurrentSize = _playerTwoBaseSize;

            ApplyPlayerOne();
            ApplyPlayerTwo();

            _sequence = null;
            _completionSource?.TrySetResult();
        });
    }

    // Остальные ReadCurrentValues, ApplyPlayerOne и т. д.
    // остаются такими же.

        private void ReadCurrentValues()
        {
            _playerOneColor = ReadColor(
                playerOneRenderer,
                _playerOneBlock,
                _outlineColorId);

            _playerTwoColor = ReadColor(
                playerTwoRenderer,
                _playerTwoBlock,
                _outlineColorId);

            _playerOneBaseSize = ReadFloat(
                playerOneRenderer,
                _playerOneBlock,
                _outlineSizeId);

            _playerTwoBaseSize = ReadFloat(
                playerTwoRenderer,
                _playerTwoBlock,
                _outlineSizeId);

            ResetSizes();
        }

        private void ResetSizes()
        {
            _playerOneCurrentSize = _playerOneBaseSize;
            _playerTwoCurrentSize = _playerTwoBaseSize;
        }

        private static Color ReadColor(
            SpriteRenderer renderer,
            MaterialPropertyBlock block,
            int propertyId)
        {
            renderer.GetPropertyBlock(block);

            if (block.HasColor(propertyId))
                return block.GetColor(propertyId);

            return renderer.sharedMaterial.GetColor(propertyId);
        }

        private static float ReadFloat(
            SpriteRenderer renderer,
            MaterialPropertyBlock block,
            int propertyId)
        {
            renderer.GetPropertyBlock(block);

            if (block.HasFloat(propertyId))
                return block.GetFloat(propertyId);

            return renderer.sharedMaterial.GetFloat(propertyId);
        }

        private void ApplyPlayerOne()
        {
            ApplyProperties(
                playerOneRenderer,
                _playerOneBlock,
                _playerOneColor,
                _playerOneCurrentSize);
        }

        private void ApplyPlayerTwo()
        {
            ApplyProperties(
                playerTwoRenderer,
                _playerTwoBlock,
                _playerTwoColor,
                _playerTwoCurrentSize);
        }

        private void ApplyProperties(
            SpriteRenderer renderer,
            MaterialPropertyBlock block,
            Color color,
            float size)
        {
            renderer.GetPropertyBlock(block);

            block.SetColor(_outlineColorId, color);
            block.SetFloat(_outlineSizeId, size);

            renderer.SetPropertyBlock(block);
        }

        private void ValidateRenderer(SpriteRenderer renderer)
        {
            if (renderer == null)
            {
                Debug.LogError(
                    $"{nameof(BeatOutlineAnimation)}: SpriteRenderer не назначен.",
                    this);

                return;
            }

            Material material = renderer.sharedMaterial;

            if (material == null)
            {
                Debug.LogError(
                    $"У {renderer.name} отсутствует материал.",
                    renderer);

                return;
            }

            if (!material.HasProperty(_outlineColorId))
            {
                Debug.LogError(
                    $"Материал {material.name} не содержит параметр " +
                    $"{outlineColorProperty}.",
                    renderer);
            }

            if (!material.HasProperty(_outlineSizeId))
            {
                Debug.LogError(
                    $"Материал {material.name} не содержит параметр " +
                    $"{outlineSizeProperty}.",
                    renderer);
            }
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
            _completionSource?.TrySetCanceled();
        }

    }
}
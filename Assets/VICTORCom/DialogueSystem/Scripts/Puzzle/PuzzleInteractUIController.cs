using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VICTORCom
{
    /// <summary>
    /// 解谜/环境交互 UI：标题、可选插图、正文（打字机）；点击跳过或下一句。
    /// 需在场景中有 EventSystem，且本对象带 Graphic（如 Image）以接收射线点击。
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class PuzzleInteractUIController : MonoBehaviour, IPointerClickHandler
    {
        public static PuzzleInteractUIController Instance { get; private set; }

        [Header("UI 引用")]
        [SerializeField] private GameObject rootPanel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private Image illustrationImage;
        [Tooltip("可选；FrameCustomAspect 时会启用并设置宽高比。不填则在插图物体上自动 Get/Add。")]
        [SerializeField] private AspectRatioFitter illustrationAspectFitter;

        [Header("打字机")]
        [SerializeField] private float secondsPerChar = 0.04f;
        [SerializeField] private bool playSoundOnWhitespace;
        [Header("打字音效")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip defaultTypeSound;
        [Range(0f, 1f)]
        [SerializeField] private float typeSoundVolume = 0.7f;

        [Header("事件")]
        public UnityEvent onInteractionEnded;

        private PuzzleInteractData _data;
        private int _index;
        private bool _playing;
        private bool _lineComplete;
        private Coroutine _typewriterRoutine;
        private float _typeSoundEndTime;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            if (rootPanel != null)
                rootPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void OnDisable()
        {
            StopTypewriter();
        }

        public void StartInteraction(PuzzleInteractData data)
        {
            if (data == null || data.lines == null || data.lines.Length == 0)
            {
                Debug.LogWarning("PuzzleInteractUIController: 交互数据为空。", this);
                return;
            }

            StopTypewriter();

            _data = data;
            _index = 0;
            _playing = true;

            if (titleText != null)
                titleText.text = data.title ?? string.Empty;

            if (illustrationImage != null)
            {
                illustrationImage.sprite = data.illustration;
                illustrationImage.enabled = data.illustration != null;
                ApplyIllustrationLayout(data);
            }

            if (rootPanel != null)
                rootPanel.SetActive(true);

            PuzzleInteractRuntimeEvents.Raise(data.interactStartEventId, data, -1);

            ApplyLine();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ProcessLine();
        }

        private void ProcessLine()
        {
            if (!_playing || _data == null)
                return;

            if (!_lineComplete)
            {
                FinishCurrentLineInstant();
                return;
            }

            _index++;
            if (_index >= _data.lines.Length)
                EndInteraction();
            else
                ApplyLine();
        }

        private void ApplyLine()
        {
            PuzzleInteractLine line = _data.lines[_index];
            PuzzleInteractRuntimeEvents.Raise(line.lineBeginEventId, _data, _index);

            _lineComplete = false;
            _typeSoundEndTime = -999f;

            UpdateHintForTyping();

            if (bodyText != null && !string.IsNullOrEmpty(line.text))
            {
                bodyText.text = string.Empty;
                StopTypewriter();
                _typewriterRoutine = StartCoroutine(TypewriterRoutine(line.text));
            }
            else
            {
                if (bodyText != null)
                    bodyText.text = line.text ?? string.Empty;
                _lineComplete = true;
                UpdateHintForComplete();
                PuzzleInteractRuntimeEvents.Raise(line.lineCompleteEventId, _data, _index);
            }
        }

        private IEnumerator TypewriterRoutine(string full)
        {
            var wait = secondsPerChar > 0f
                ? new WaitForSeconds(secondsPerChar)
                : null;

            for (int i = 1; i <= full.Length; i++)
            {
                if (bodyText != null)
                    bodyText.text = full.Substring(0, i);

                char c = full[i - 1];
                if (ShouldPlaySoundForChar(c))
                    TryPlayTypeSound();

                if (wait != null)
                    yield return wait;
                else
                    yield return null;
            }

            _lineComplete = true;
            _typewriterRoutine = null;
            UpdateHintForComplete();
            if (_data != null && _index >= 0 && _index < _data.lines.Length)
                PuzzleInteractRuntimeEvents.Raise(_data.lines[_index].lineCompleteEventId, _data, _index);
        }

        private bool ShouldPlaySoundForChar(char c)
        {
            if (char.IsWhiteSpace(c) && !playSoundOnWhitespace)
                return false;
            return true;
        }

        private void TryPlayTypeSound()
        {
            if (audioSource == null)
                return;

            AudioClip clip = GetActiveTypeClip();
            if (clip == null)
                return;

            float t = Time.unscaledTime;
            if (t < _typeSoundEndTime)
                return;

            audioSource.PlayOneShot(clip, typeSoundVolume);

            float pitch = Mathf.Abs(audioSource.pitch);
            if (pitch < 0.0001f)
                pitch = 1f;
            _typeSoundEndTime = t + clip.length / pitch;
        }

        private AudioClip GetActiveTypeClip()
        {
            if (_data != null && _data.typingSoundOverride != null)
                return _data.typingSoundOverride;
            return defaultTypeSound;
        }

        private void FinishCurrentLineInstant()
        {
            if (_data == null || _index < 0 || _index >= _data.lines.Length)
                return;

            StopTypewriter();

            string full = _data.lines[_index].text ?? string.Empty;
            if (bodyText != null)
                bodyText.text = full;

            _lineComplete = true;
            UpdateHintForComplete();
            if (_data != null && _index >= 0 && _index < _data.lines.Length)
                PuzzleInteractRuntimeEvents.Raise(_data.lines[_index].lineCompleteEventId, _data, _index);
        }

        private void StopTypewriter()
        {
            if (_typewriterRoutine != null)
            {
                StopCoroutine(_typewriterRoutine);
                _typewriterRoutine = null;
            }
        }

        private void UpdateHintForTyping()
        {
            if (hintText == null)
                return;
            hintText.text = "点击跳过";
        }

        private void UpdateHintForComplete()
        {
            if (hintText == null)
                return;
            hintText.text = _index < _data.lines.Length - 1 ? "点击继续" : "点击结束";
        }

        private void EndInteraction()
        {
            StopTypewriter();
            _playing = false;
            PuzzleInteractData ended = _data;
            _data = null;
            if (rootPanel != null)
                rootPanel.SetActive(false);
            if (ended != null)
                PuzzleInteractRuntimeEvents.Raise(ended.interactEndEventId, ended, -1);
            onInteractionEnded?.Invoke();
        }

        public bool IsPlaying => _playing;

        private void ApplyIllustrationLayout(PuzzleInteractData data)
        {
            if (illustrationImage == null || data == null)
                return;

            AspectRatioFitter arf = illustrationAspectFitter;
            if (arf == null)
                illustrationImage.TryGetComponent(out arf);

            if (data.illustration == null)
            {
                if (arf != null)
                    arf.enabled = false;
                return;
            }

            switch (data.illustrationLayout)
            {
                case IllustrationLayoutMode.Stretch:
                    illustrationImage.preserveAspect = false;
                    if (arf != null)
                        arf.enabled = false;
                    break;

                case IllustrationLayoutMode.ContainSpriteAspect:
                    illustrationImage.preserveAspect = true;
                    if (arf != null)
                        arf.enabled = false;
                    break;

                case IllustrationLayoutMode.FrameCustomAspect:
                    illustrationImage.preserveAspect = true;
                    if (arf == null)
                        arf = illustrationImage.gameObject.AddComponent<AspectRatioFitter>();
                    arf.enabled = true;
                    arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                    arf.aspectRatio = Mathf.Max(0.01f, data.illustrationCustomAspectRatio);
                    break;
            }
        }
    }
}

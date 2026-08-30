using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject _stepPanel;
    [SerializeField] private GameObject _healthExplanationPanel;
    [SerializeField] private GameObject _waveExplanationPanel;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _instructionText;
    [SerializeField] private TMP_Text _bindingText;
    [SerializeField, Min(0f)] private float _bossPopupDuration = 3f;

    public event Action OnAdvanceRequested;

    private InputAction _advanceAction;
    private bool _canAdvanceWithInput;
    private Coroutine _hideBossPopupCoroutine;

    private void Awake()
    {
        _advanceAction = new InputAction("AdvanceTutorial", InputActionType.Button);
        _advanceAction.AddBinding("<Keyboard>/enter");
        _advanceAction.AddBinding("<Keyboard>/numpadEnter");
    }

    private void OnEnable()
    {
        _advanceAction.performed += HandleAdvanceAction;
        _advanceAction.Enable();
    }

    private void OnDisable()
    {
        _advanceAction.performed -= HandleAdvanceAction;
        _advanceAction.Disable();
        _hideBossPopupCoroutine = null;
    }

    private void OnDestroy()
    {
        _advanceAction.Dispose();
    }

    public void ShowStep(TutorialStep step)
    {
        if (_hideBossPopupCoroutine != null)
        {
            StopCoroutine(_hideBossPopupCoroutine);
            _hideBossPopupCoroutine = null;
        }

        _canAdvanceWithInput = step == TutorialStep.HealthExplanation ||
                               step == TutorialStep.WaveExplanation ||
                               step == TutorialStep.AttackExplanation ||
                               step == TutorialStep.SummonAlly;
        SetPanelVisibility(step);

        switch (step)
        {
            case TutorialStep.HealthExplanation:
            case TutorialStep.WaveExplanation:
            case TutorialStep.Complete:
            case TutorialStep.Attack:
                break;

            default:
                TutorialStepContent content = GetContent(step);
                _titleText.text = content.Title;
                _instructionText.text = content.Instruction;
                _bindingText.text = content.BindingText;

                if (step == TutorialStep.Boss)
                    _hideBossPopupCoroutine = StartCoroutine(HideBossPopupAfterDelay());
                break;
        }
    }

    private IEnumerator HideBossPopupAfterDelay()
    {
        yield return new WaitForSeconds(_bossPopupDuration);
        _stepPanel.SetActive(false);
        _hideBossPopupCoroutine = null;
    }

    private void SetPanelVisibility(TutorialStep step)
    {
        bool showHealth = step == TutorialStep.HealthExplanation;
        bool showWave = step == TutorialStep.WaveExplanation;
        bool showStep = !showHealth &&
                        !showWave &&
                        step != TutorialStep.Attack &&
                        step != TutorialStep.Complete;

        _stepPanel.SetActive(showStep);
        _healthExplanationPanel.SetActive(showHealth);
        _waveExplanationPanel.SetActive(showWave);
    }

    private void HandleAdvanceAction(InputAction.CallbackContext context)
    {
        if (_canAdvanceWithInput)
            OnAdvanceRequested?.Invoke();
    }

    private TutorialStepContent GetContent(TutorialStep step)
    {
        return step switch
        {
            TutorialStep.Move => new TutorialStepContent(
                "移動してみよう！", "左右に移動してください。", "A / D または ← / →"),
            TutorialStep.Jump => new TutorialStepContent(
                "ジャンプしてみよう！", "ジャンプしてください。", "Space または W または ↑"),
            TutorialStep.AttackExplanation => new TutorialStepContent(
                "攻撃してみよう！", "Enterを押して敵を出現させ、倒してください。", "Enterで開始 / 左クリックで攻撃"),
            TutorialStep.ChangeMode => new TutorialStepContent(
                "モードを切り替えよう！", "建築モードに切り替えてください。", "建築スロットのキー（1～4）"),
            TutorialStep.PlaceStructure => new TutorialStepContent(
                "建築物を設置しよう！", "建築可能な場所に建物を設置してください。", "左クリック"),
            TutorialStep.SummonAlly => new TutorialStepContent(
                "味方を召喚しよう！", "味方を召喚してください。", "Ctrl（召喚済みの場合は Enter）"),
            TutorialStep.Boss => new TutorialStepContent(
                "ボス戦！", "ボスを倒してください。", string.Empty),
            _ => TutorialStepContent.Empty
        };
    }

    private readonly struct TutorialStepContent
    {
        public static readonly TutorialStepContent Empty = new(
            string.Empty, string.Empty, string.Empty);

        public string Title { get; }
        public string Instruction { get; }
        public string BindingText { get; }

        public TutorialStepContent(string title, string instruction, string bindingText)
        {
            Title = title;
            Instruction = instruction;
            BindingText = bindingText;
        }
    }
}

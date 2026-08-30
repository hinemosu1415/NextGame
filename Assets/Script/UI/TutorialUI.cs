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

        switch (step)
        {
            case TutorialStep.HealthExplanation:
                SetPanelVisibility(false, true, false);
                break;

            case TutorialStep.WaveExplanation:
                SetPanelVisibility(false, false, true);
                break;

            case TutorialStep.Complete:
                SetPanelVisibility(false, false, false);
                break;

            case TutorialStep.Attack:
                SetPanelVisibility(false, false, false);
                break;

            default:
                SetPanelVisibility(true, false, false);
                _titleText.text = GetTitle(step);
                _instructionText.text = GetInstruction(step);
                _bindingText.text = GetBindingText(step);

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

    private void SetPanelVisibility(bool showStep, bool showHealth, bool showWave)
    {
        _stepPanel.SetActive(showStep);
        _healthExplanationPanel.SetActive(showHealth);
        _waveExplanationPanel.SetActive(showWave);
    }

    private void HandleAdvanceAction(InputAction.CallbackContext context)
    {
        if (_canAdvanceWithInput)
            OnAdvanceRequested?.Invoke();
    }

    private string GetTitle(TutorialStep step)
    {
        return step switch
        {
            TutorialStep.Move => "移動してみよう！",
            TutorialStep.Jump => "ジャンプしてみよう！",
            TutorialStep.AttackExplanation => "攻撃してみよう！",
            TutorialStep.ChangeMode => "モードを切り替えよう！",
            TutorialStep.PlaceStructure => "建築物を設置しよう！",
            TutorialStep.SummonAlly => "味方を召喚しよう！",
            TutorialStep.Boss => "ボス戦！",
            _ => string.Empty
        };
    }

    private string GetInstruction(TutorialStep step)
    {
        return step switch
        {
            TutorialStep.Move => "左右に移動してください。",
            TutorialStep.Jump => "ジャンプしてください。",
            TutorialStep.AttackExplanation => "Enterを押して敵を出現させ、倒してください。",
            TutorialStep.ChangeMode => "建築モードに切り替えてください。",
            TutorialStep.PlaceStructure => "建築可能な場所に建物を設置してください。",
            TutorialStep.SummonAlly => "味方を召喚してください。",
            TutorialStep.Boss => "ボスを倒してください。",
            _ => string.Empty
        };
    }

    private string GetBindingText(TutorialStep step)
    {
        return step switch
        {
            TutorialStep.Move => "A / D または ← / →",
            TutorialStep.Jump => "Space または W または ↑",
            TutorialStep.AttackExplanation => "Enterで開始 / 左クリックで攻撃",
            TutorialStep.ChangeMode => "建築スロットのキー（1～4）",
            TutorialStep.PlaceStructure => "左クリック",
            TutorialStep.SummonAlly => "Ctrl（召喚済みの場合は Enter）",
            TutorialStep.Boss => "",
            _ => string.Empty
        };
    }
}

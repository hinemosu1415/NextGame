using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject _stepPanel;
    [SerializeField] private GameObject _healthExplanationPanel;
    [SerializeField] private GameObject _waveExplanationPanel;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _instructionText;
    [SerializeField] private TMP_Text _bindingText;

    public void ShowStep(TutorialStep step)
    {
        bool isComplete = step == TutorialStep.Complete;
        bool isHealthExplanation = step == TutorialStep.HealthExplanation;
        bool isWaveExplanation = step == TutorialStep.WaveExplanation;
        bool isExplanation = isHealthExplanation || isWaveExplanation;

        if (_stepPanel != null) _stepPanel.SetActive(!isComplete && !isExplanation);
        if (_healthExplanationPanel != null) _healthExplanationPanel.SetActive(isHealthExplanation);
        if (_waveExplanationPanel != null) _waveExplanationPanel.SetActive(isWaveExplanation);
        if (isComplete || isExplanation) return;

        if (_titleText != null) _titleText.text = GetTitle(step);
        if (_instructionText != null) _instructionText.text = GetInstruction(step);
        if (_bindingText != null) _bindingText.text = GetBindingText(step);
    }

    private string GetTitle(TutorialStep step)
    {
        return step switch
        {
            TutorialStep.Move => "移動してみよう！",
            TutorialStep.Jump => "ジャンプしてみよう！",
            TutorialStep.Attack => "攻撃してみよう！",
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
            TutorialStep.Attack => "目の前のダミー敵を攻撃してください。",
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
            TutorialStep.Attack => "左クリック",
            TutorialStep.ChangeMode => "建築スロットのキー（1～4）",
            TutorialStep.PlaceStructure => "左クリック",
            TutorialStep.SummonAlly => "Ctrl",
            TutorialStep.Boss => "",
            _ => string.Empty
        };
    }
}

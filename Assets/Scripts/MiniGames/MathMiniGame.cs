using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MathMiniGame : MiniGame
{
    [SerializeField] private TextMeshProUGUI _equationText;
    [SerializeField] private Slider _timerSlider;
    [SerializeField] private Image _aPrompt;
    [SerializeField] private Image _bPrompt;
    [SerializeField] private Image _xPrompt;
    [SerializeField] private Image _yPrompt;
    [SerializeField] private CanvasGroup _choicesPanel;
    [SerializeField] private Sprite _correctSprite;
    [SerializeField] private Sprite _wrongSprite;
    [SerializeField] private Image[] _playerCorrectnessImages;

    public override string Name => "Mathematicar";

    private const int _coinsPerQuestion = 15;
    private const int _earlyCoins = 5;
    private const int _maxQuestionCount = 7;
    private const float _questionTimer = 10;

    public override void OnCalled()
    {
        _timerSlider.gameObject.SetActive(false);
        _choicesPanel.DOFade(0.0f, 0.0f);

        _equationText.SetText(string.Empty);

        for (int i = 0; i < 4; i++) GetPromptText(i).SetText(string.Empty);
        for (int i = 0; i < _playerCorrectnessImages.Length; i++) _playerCorrectnessImages[i].gameObject.SetActive(false);
    }

    private Image GetPrompt(int index)
    {
        return index switch
        {
            0 => _aPrompt,
            1 => _bPrompt,
            2 => _xPrompt,
            3 => _yPrompt,
            _ => null,
        };
    }

    private TextMeshProUGUI GetPromptText(int index)
    {
        return GetPrompt(index).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public override async void OnBegin()
    {
        List<int> scores = new();
        List<bool> playerAnswered = new();
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            scores.Add(0);
            playerAnswered.Add(false);
        }

        int currentSolutionIndex = 0;
        bool AllPlayersAnswered()
        {
            foreach (var answered in playerAnswered)
            {
                if (!answered) return false;
            }
            return true;
        }

        bool HasNobodyAnswered()
        {
            foreach (var answer in playerAnswered)
            {
                if (answer) return false;
            }
            return true;
        }

        await Awaitable.WaitForSecondsAsync(0.5f);
        await _choicesPanel.DOFade(1.0f, 0.25f).AsyncWaitForCompletion();
        await Awaitable.WaitForSecondsAsync(0.5f);

        AudioManager.Instance.Play(SoundName.MathTheme);

        _timerSlider.gameObject.SetActive(true);

        PlayerManager.Instance.EnableInput();
        PlayerManager.Instance.GiveOwnershipToAll();

        PlayerManager.Instance.OnAnyPlayerPromptSouthPerformed += OnInputSouth;
        PlayerManager.Instance.OnAnyPlayerPromptEastPerformed += OnInputEast;
        PlayerManager.Instance.OnAnyPlayerPromptWestPerformed += OnInputWest;
        PlayerManager.Instance.OnAnyPlayerPromptNorthPerformed += OnInputNorth;

        void OnInputSouth(PlayerController controller) => AnswerQuestion(controller, 0);
        void OnInputEast(PlayerController controller) => AnswerQuestion(controller, 1);
        void OnInputWest(PlayerController controller) => AnswerQuestion(controller, 2);
        void OnInputNorth(PlayerController controller) => AnswerQuestion(controller, 3);

        void AnswerQuestion(PlayerController controller, int index)
        {
            if (playerAnswered[controller.Index]) return;

            playerAnswered[controller.Index] = true;
            AudioManager.Instance.Play(SoundName.Pop);

            var prompt = GetPrompt(index);
            prompt.transform.DOKill();
            prompt.transform.DOScale(1.2f, 0.0f);
            prompt.transform.DOScale(1.0f, 0.2f);

            if (index == currentSolutionIndex)
            {
                scores[controller.Index] += _coinsPerQuestion;
                if (HasNobodyAnswered()) scores[controller.Index] += _earlyCoins;
                _playerCorrectnessImages[controller.Index].sprite = _correctSprite;
            }
            else
            {
                _playerCorrectnessImages[controller.Index].sprite = _wrongSprite;
            }
        }

        void HighlightAnswerText(int index)
        {
            var tmp = GetPromptText(index);
            tmp.transform.DOScale(1.2f, 0.0f);
            tmp.transform.DOScale(1.0f, 0.2f);
            tmp.SetText($"<color=green>{tmp.text}");
        }

        for (int i = 0; i < _maxQuestionCount; i++)
        {
            for (int j = 0; j < playerAnswered.Count; j++)
            {
                playerAnswered[j] = false;
                _playerCorrectnessImages[j].sprite = _wrongSprite;
                _playerCorrectnessImages[j].gameObject.SetActive(false);
            }

            var (equation, answers, solutionIndex) = GenerateQuestion();
            currentSolutionIndex = solutionIndex;

            _equationText.SetText(equation);
            for (int j = 0; j < 4; j++) GetPromptText(j).SetText(answers[j].ToString());

            _timerSlider.maxValue = _questionTimer;
            _timerSlider.value = _questionTimer;

            while (_timerSlider.value > 0)
            {
                await Awaitable.EndOfFrameAsync();
                _timerSlider.value -= Time.deltaTime;
                if (AllPlayersAnswered())
                {
                    break;
                }
            }

            for (int j = 0; j < playerAnswered.Count; j++)
            {
                playerAnswered[j] = true;
                _playerCorrectnessImages[j].gameObject.SetActive(true);
            }

            AudioManager.Instance.Play(SoundName.GameStart);

            HighlightAnswerText(solutionIndex);

            await Awaitable.WaitForSecondsAsync(1.5f);
        }

        PlayerManager.Instance.DisableInput();

        PlayerManager.Instance.OnAnyPlayerPromptSouthPerformed -= OnInputSouth;
        PlayerManager.Instance.OnAnyPlayerPromptEastPerformed -= OnInputEast;
        PlayerManager.Instance.OnAnyPlayerPromptWestPerformed -= OnInputWest;
        PlayerManager.Instance.OnAnyPlayerPromptNorthPerformed -= OnInputNorth;

        AudioManager.Instance.Stop(SoundName.MathTheme);

        End(new Dictionary<int, int>
        {
            {0, scores.InRange(0) ? scores[0] : 0},
            {1, scores.InRange(1) ? scores[1] : 0},
            {2, scores.InRange(2) ? scores[2] : 0},
            {3, scores.InRange(3) ? scores[3] : 0},
        });
    }

    private static (string, double[], int) GenerateQuestion()
    {
        string equation = GenerateEquation();
        double solution = System.Math.Round(EvaluateEquation(equation), 2);

        double[] answers = new double[4];
        answers[0] = solution;

        bool isWholeNumber = solution % 1 == 0;

        for (int i = 1; i < answers.Length; i++)
        {
            double answer;
            do
            {
                double randomOffset = Random.Range(-5.0f, 5.0f);
                answer = solution + randomOffset;

                answer = isWholeNumber
                    ? System.Math.Round(answer)
                    : System.Math.Round(answer, 2);
            } while (System.Math.Abs(answer - solution) < 0.01 || System.Array.Exists(answers, a => System.Math.Abs(a - answer) < 0.01));

            answers[i] = answer;
        }

        for (int i = 0; i < answers.Length; i++)
        {
            int randomIndex = Random.Range(0, answers.Length);
            (answers[i], answers[randomIndex]) = (answers[randomIndex], answers[i]);
        }

        int solutionIndex = System.Array.IndexOf(answers, solution);
        return (equation, answers, solutionIndex);
    }

    private static string GenerateEquation()
    {
        string[] operators = { "+", "-", "*", "/" };

        int numCount = Random.Range(3, 5);
        string[] tokens = new string[numCount * 2 - 1];

        for (int i = 0; i < numCount; i++)
        {
            tokens[i * 2] = Random.Range(1, 12).ToString();
        }

        for (int i = 1; i < tokens.Length; i += 2)
        {
            string op;
            do
            {
                op = operators[Random.Range(0, operators.Length)];
                if (op == "/")
                {
                    int leftOperand = int.Parse(tokens[i - 1]);
                    int rightOperand;
                    do
                    {
                        rightOperand = Random.Range(1, 12);
                    } while (leftOperand % rightOperand != 0); // Ensure division results in a whole number
                    tokens[i + 1] = rightOperand.ToString();
                }
            } while (op == "/" && tokens[i + 1] == null); // Ensure valid division
            tokens[i] = op;
        }

        string expression = string.Join(" ", tokens);

        if (numCount >= 4 && Random.Range(0, 1.0f) > 0.5)
        {
            int start = Random.Range(0, numCount - 2) * 2;
            int end = start + 4;
            if (end < tokens.Length)
            {
                tokens[start] = "(" + tokens[start];
                tokens[end] = tokens[end] + ")";
                expression = string.Join(" ", tokens);
            }
        }

        return expression;
    }

    private static double EvaluateEquation(string expression)
    {
        DataTable table = new();
        return System.Convert.ToDouble(table.Compute(expression, ""));
    }
}

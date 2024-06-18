using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

public class MasterScript : MonoBehaviour
{
    [SerializeField]
    Canvas ErrorCanvas;
    [SerializeField]
    TextMeshProUGUI ErrorMessage;
    [SerializeField]
    Canvas WeightCanvas;
    [SerializeField]
    Canvas ResultCanvas;
    [SerializeField]
    TextMeshProUGUI ResultHeader;
    [SerializeField]
    TextMeshProUGUI ResultMessage;
    [SerializeField]
    Canvas InfoCanvas;
    [SerializeField]
    Canvas MainCanvas;

    [SerializeField]
    Button WeighButton;
    [SerializeField]
    TextMeshProUGUI WeighCount;

    [SerializeField]
    PanCollisionScript LeftPan;
    [SerializeField]
    PanCollisionScript RightPan;

    [SerializeField]
    GameObject Scale;
    [SerializeField]
    GameObject AnswerObject;

    [SerializeField]
    CoinPossibility[] coinPossibilities = new CoinPossibility[24];

    [SerializeField]
    int TimesWeighed = 0;

    [SerializeField]
    Animator ScaleAnimator;
    [SerializeField]
    Animator CameraAnimator;

    [SerializeField]
    GameObject ResultPfb;
    [SerializeField]
    GameObject HorizontalGroup;

    float timer = 0;

    // Start is called before the first frame update
    void Awake()
    {
        ResetCoinPossibilities();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
    }

    void DisplayError(string Text)
    {
        MainCanvas.gameObject.SetActive(false);
        ErrorCanvas.gameObject.SetActive(true);
        ErrorMessage.text = Text;
    }

    public void DismissError()
    {
        MainCanvas.gameObject.SetActive(true);
        ErrorCanvas.gameObject.SetActive(false);
    }

    public void ResetCoinPosition()
    {
        CoinScript[] coins = FindObjectsOfType<CoinScript>();
        foreach (CoinScript coin in coins)
        {
            coin.resetTransform();
        }
    }

    public void StartWeighing()
    {
        StartCoroutine(Weigh());
    }

    IEnumerator Weigh()
    {
        List<int> LeftPanCoins = LeftPan.GetCoins();
        List<int> RightPanCoins = RightPan.GetCoins();
        if (LeftPanCoins.Count == 0 && RightPanCoins.Count == 0)
        {
            DisplayError("There's nothing to weigh!");
            yield break;
        }
        if (LeftPanCoins.Count != RightPanCoins.Count)
        {
            DisplayError("There must be an identical number of coins in each pan, otherwise the pan with more coins will always be heavier");
            yield break;
        }
        timer = 1.5f;
        CameraAnimator.SetTrigger("FocusScale");

        int Outcome = GetHeavierSide(LeftPanCoins, RightPanCoins);
        MainCanvas.gameObject.SetActive(false);
        WeightCanvas.gameObject.SetActive(true);

        TimesWeighed++;
        WeighCount.text = "Times weighed: " + TimesWeighed;

        yield return new WaitForSeconds(timer);

        switch (Outcome)
        {
            case 1:
                ScaleAnimator.SetTrigger("LeftSide");
                break;
            case 2:
                ScaleAnimator.SetTrigger("RightSide");
                break;
        }

        yield return new WaitForSeconds(5f);

        MainCanvas.gameObject.SetActive(true);
        WeightCanvas.gameObject.SetActive(false);

        if (TimesWeighed == 3)
        {
            PrepareAnswers();
        }

    }

    private int GetHeavierSide(List<int> LeftPanCoins, List<int> RightPanCoins)
    {
        CoinPossibility[] NoneOutcome = CountPossibitiesNone(LeftPanCoins, RightPanCoins);
        int NonePossibilities = NoneOutcome.Count(c => c.Possible);
        CoinPossibility[] LeftOutcome = CountPossibitiesLeft(LeftPanCoins, RightPanCoins);
        int LeftPossibilities = LeftOutcome.Count(c => c.Possible);
        CoinPossibility[] RightOutcome = CountPossibitiesRight(LeftPanCoins, RightPanCoins);
        int RightPossibilities = RightOutcome.Count(c => c.Possible);

        int MaximumPossibilities = Mathf.Max(RightPossibilities, LeftPossibilities, NonePossibilities);

        List<int> PossibleOutcomes = new List<int>();

        if (NonePossibilities == MaximumPossibilities || (NonePossibilities == 2 && MaximumPossibilities == 3))
            PossibleOutcomes.Add(0);
        if (LeftPossibilities == MaximumPossibilities || (LeftPossibilities == 2 && MaximumPossibilities == 3))
            PossibleOutcomes.Add(1);
        if (RightPossibilities == MaximumPossibilities || (RightPossibilities == 2 && MaximumPossibilities == 3))
            PossibleOutcomes.Add(2);

        int Outcome = PossibleOutcomes[UnityEngine.Random.Range(0, PossibleOutcomes.Count)];
        GameObject ResultObject = Instantiate(ResultPfb, HorizontalGroup.transform);

        ResultObject.transform.Find("WeighNumber").GetComponent<TextMeshProUGUI>().text = "Weigh Number " + (TimesWeighed + 1) + ":";
        ResultObject.transform.Find("LeftPanText").GetComponent<TextMeshProUGUI>().text = "Left Pan: " + string.Join(", ", LeftPanCoins);
        ResultObject.transform.Find("RightPanText").GetComponent<TextMeshProUGUI>().text = "Right Pan: " + string.Join(", ", RightPanCoins);

        switch (Outcome)
        {
            case 0:
                coinPossibilities = NoneOutcome;
                ResultObject.transform.Find("ResultText").GetComponent<TextMeshProUGUI>().text = "Result: Both sides were equal";
                break;
            case 1:
                coinPossibilities = LeftOutcome;
                ResultObject.transform.Find("ResultText").GetComponent<TextMeshProUGUI>().text = "Result: Left side was heavier";
                break;
            case 2:
                coinPossibilities = RightOutcome;
                ResultObject.transform.Find("ResultText").GetComponent<TextMeshProUGUI>().text = "Result: Right side was heavier";
                break;
        }
        return Outcome;
    }

    private CoinPossibility[] CountPossibitiesRight(List<int> LeftPanCoins, List<int> RightPanCoins)
    {
        CoinPossibility[] TheoreticalPossibilities = coinPossibilities.Select(c => (CoinPossibility)c.Clone()).ToArray();
        List<int> NotWeighed = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        foreach (int coin in LeftPanCoins)
        {
            TheoreticalPossibilities.FirstOrDefault(c => c.CoinNumber == coin && c.Weight == true).Possible = false;
            NotWeighed.Remove(coin);
        }

        foreach (int coin in RightPanCoins)
        {
            TheoreticalPossibilities.FirstOrDefault(c => c.CoinNumber == coin && c.Weight == false).Possible = false;
            NotWeighed.Remove(coin);
        }

        foreach (int coin in NotWeighed)
        {
            CoinPossibility[] NotPossible = TheoreticalPossibilities.Where(c => c.CoinNumber == coin).ToArray();
            foreach (CoinPossibility possibility in NotPossible)
                possibility.Possible = false;
        }

        return TheoreticalPossibilities;
    }

    private CoinPossibility[] CountPossibitiesLeft(List<int> LeftPanCoins, List<int> RightPanCoins)
    {
        CoinPossibility[] TheoreticalPossibilities = coinPossibilities.Select(c => (CoinPossibility)c.Clone()).ToArray();
        List<int> NotWeighed = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        foreach (int coin in LeftPanCoins)
        {
            TheoreticalPossibilities.FirstOrDefault(c => c.CoinNumber == coin && c.Weight == false).Possible = false;
            NotWeighed.Remove(coin);
        }

        foreach (int coin in RightPanCoins)
        {
            TheoreticalPossibilities.FirstOrDefault(c => c.CoinNumber == coin && c.Weight == true).Possible = false;
            NotWeighed.Remove(coin);
        }

        foreach (int coin in NotWeighed)
        {
            CoinPossibility[] NotPossible = TheoreticalPossibilities.Where(c => c.CoinNumber == coin).ToArray();
            foreach (CoinPossibility possibility in NotPossible)
                possibility.Possible = false;
        }

        return TheoreticalPossibilities;
    }

    private CoinPossibility[] CountPossibitiesNone(List<int> LeftPanCoins, List<int> RightPanCoins)
    {
        CoinPossibility[] TheoreticalPossibilities = coinPossibilities.Select(c => (CoinPossibility)c.Clone()).ToArray();

        foreach (int coin in LeftPanCoins)
        {
            CoinPossibility[] NotPossible = TheoreticalPossibilities.Where(c => c.CoinNumber == coin).ToArray();
            foreach (CoinPossibility possibility in NotPossible)
                possibility.Possible = false;
        }

        foreach (int coin in RightPanCoins)
        {
            CoinPossibility[] NotPossible = TheoreticalPossibilities.Where(c => c.CoinNumber == coin).ToArray();
            foreach (CoinPossibility possibility in NotPossible)
                possibility.Possible = false;
        }

        return TheoreticalPossibilities;
    }

    private void PrepareAnswers()
    {
        ResetCoinPosition();
        LeftPan.ClearCoins();
        RightPan.ClearCoins();
        WeighButton.gameObject.SetActive(false);
        Scale.SetActive(false);
        AnswerObject.SetActive(true);
    }

    public void ResetAll()
    {
        ResetCoinPosition();
        ResetCoinPossibilities();
        Scale.SetActive(true);
        AnswerObject.SetActive(false);
        TimesWeighed = 0;
        MainCanvas.gameObject.SetActive(true);
        ResultCanvas.gameObject.SetActive(false);
        WeighButton.gameObject.SetActive(true);
        WeighCount.text = "Times weighed: 0";
        foreach (Transform child in HorizontalGroup.transform)
        {
            Destroy(child.gameObject);
        }

    }

    private void ResetCoinPossibilities()
    {
        for (int i = 1, j = 0; i < 13; i++)
        {
            CoinPossibility coinPossibility = new CoinPossibility
            {
                CoinNumber = i,
                Possible = true,
                Weight = false
            };
            coinPossibilities[j] = coinPossibility;
            j++;

            coinPossibility = new CoinPossibility
            {
                CoinNumber = i,
                Possible = true,
                Weight = true
            };
            coinPossibilities[j] = coinPossibility;
            j++;
        }
    }

    public void EvaluateAnswer(int Coin, bool Weight)
    {
        CoinPossibility[] PossibleIncorrectAnswers = coinPossibilities.Where(c => c.Possible && !(c.CoinNumber == Coin && c.Weight == Weight)).ToArray();
        if (PossibleIncorrectAnswers.Length > 0)
        {
            //incorrect
            ResultHeader.text = "Oops!";
            CoinPossibility result = PossibleIncorrectAnswers[UnityEngine.Random.Range(0, PossibleIncorrectAnswers.Length)];
            ResultMessage.text = "The correct answer was actually coin " + result.CoinNumber + ", " + (result.Weight ? "Heavier" : "Lighter");
        }
        else
        {
            //correct
            ResultHeader.text = "Congratulations!";
            ResultMessage.text = "The correct answer is indeed coin " + Coin + ", " + (Weight ? "Heavier" : "Lighter");
        }
        MainCanvas.gameObject.SetActive(false);
        ResultCanvas.gameObject.SetActive(true);
    }

    public void DisplayInfoPanel()
    {
        InfoCanvas.gameObject.SetActive(true);
        MainCanvas.gameObject.SetActive(false);
    }

    public void HideInfoPanel()
    {
        InfoCanvas.gameObject.SetActive(false);
        MainCanvas.gameObject.SetActive(true);
    }

    private class CoinPossibility : ICloneable
    {
        public int CoinNumber;
        public bool Possible;
        //false = light, true = heavy;
        public bool Weight;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}


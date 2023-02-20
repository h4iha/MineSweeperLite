using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public enum TypeButton
{
    Squares, Mines
}
public class GameDisplay : MonoBehaviour, IObserver
{
    private GameManager gameManager;
    [SerializeField] private GameObject statusGameObject;
    [SerializeField] private Image iconDisplay;
    [SerializeField] private Sprite smileIcon;
    [SerializeField] private Sprite sadIcon;
    [SerializeField] private Text numberOfMinesDisplay;
    [SerializeField] private Text timerDisplay;
    [SerializeField] private Button squaresPlus;
    [SerializeField] private Button squaresMinus;
    [SerializeField] private Button minesPlus;
    [SerializeField] private Button minesMinus;
    private int timer = 0;
    private void Start()
    {
        timer = 300;
        gameManager = GameManager.Instance;
        AddSelfToObservers();
    }
    private void AddSelfToObservers()
    {
        gameManager.AddObserver(this);
    }
    private void StartCountDown()
    {
        StopAllCoroutines();
        timer = 300;
        DisplayTimer();
        StartCoroutine(CountDown());
    }
    private IEnumerator CountDown()
    {
        yield return new WaitForSecondsRealtime(1f);
        timer -= 1;
        DisplayTimer();
        if (timer <= 0)
        {
            gameManager.onMassageLostGame?.Invoke();
            yield break;
        }
        else
        {
            StartCoroutine(CountDown());
        }
    }
    private void DisplayTimer()
    {
        timerDisplay.text = DisplayZeros(timer) + timer.ToString();
    }
    private void DisplayStatusGameObject(EnumGameStatus status)
    {
        statusGameObject.SetActive(true);
        switch (status)
        {
            case EnumGameStatus.Won:
                iconDisplay.sprite = smileIcon;
                DisplayTimer();
                StopAllCoroutines();
                break;
            case EnumGameStatus.Lost:
                iconDisplay.sprite = sadIcon;
                DisplayTimer();
                StopAllCoroutines();
                break;
        }
    }
    private void DisplayNumberOfMines(int currentNumberOfMines)
    {
        numberOfMinesDisplay.text = DisplayZeros(currentNumberOfMines) + currentNumberOfMines.ToString();
    }
    private string DisplayZeros(int number)
    {
        string zeros = "";
        if (number < 10)
        {
            zeros = "00";
        }
        else if (number < 100)
        {
            zeros = "0";
        }
        return zeros;
    }
    private void SetActivedButton(int current, int max, int min, TypeButton type)
    {
        Image squaresPlusImage = squaresPlus.GetComponent<Image>();
        Image squaresMinusImage = squaresMinus.GetComponent<Image>();
        Image minesPlusImage = minesPlus.GetComponent<Image>();
        Image minesMinusImage = minesMinus.GetComponent<Image>();
        switch (type)
        {
            case TypeButton.Squares:
                {
                    if (current >= max)
                    {
                        squaresPlus.enabled = false;
                        squaresPlusImage.color = Color.yellow;
                        
                    }
                    else if (current <= min)
                    {
                        squaresMinus.enabled = false;
                        squaresMinusImage.color = Color.yellow;
                    }
                    else
                    {
                        squaresPlus.enabled = true;
                        squaresPlusImage.color = Color.black;
                        squaresMinus.enabled = true;
                        squaresMinusImage.color = Color.black;
                    }
                }
                break;
            case TypeButton.Mines:
                {
                    if (current >= max)
                    {
                        minesPlus.enabled = false;
                        minesPlusImage.color = Color.yellow;

                    }
                    else if (current <= min)
                    {
                        minesMinus.enabled = false;
                        minesMinusImage.color = Color.yellow;
                    }
                    else
                    {
                        minesPlus.enabled = true;
                        minesPlusImage.color = Color.black;
                        minesMinus.enabled = true;
                        minesMinusImage.color = Color.black;
                    }
                }
                break;
        }
    }
    public void OnRestartDataNotify(int currentNumberOfSquaresInRow, int currentNumberOfMines)
    {
        SetActivedButton(currentNumberOfSquaresInRow, gameManager.MaxNumberOfSquaresInRow, gameManager.MinNumberOfSquaresInRow, TypeButton.Squares);
        SetActivedButton(currentNumberOfMines, gameManager.MaxNumberOfMines, gameManager.MinNumberOfMines, TypeButton.Mines);
        statusGameObject.SetActive(false);
        DisplayNumberOfMines(currentNumberOfMines);
        StartCountDown();
    }
    public void OnStatusNotify(EnumGameStatus status)
    {
        DisplayStatusGameObject(status);
    }
}

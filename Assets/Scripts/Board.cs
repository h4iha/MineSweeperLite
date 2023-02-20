using UnityEngine;
using UnityEngine.UI;
public class Board : MonoBehaviour, IObserver
{
    private GameManager gameManager = null;
    [SerializeField] private GridLayoutGroup gridLayoutGroup = null;
    private void Start()
    {
        gameManager = GameManager.Instance;
        AddSelfToObservers();
    }
    private void AddSelfToObservers()
    {
        gameManager.AddObserver(this);
    }
    private void FixGridLayoutGroup(int currentNumberOfSquaresInRow)
    {
        int width = 900;
        int length = currentNumberOfSquaresInRow;
        int spacing = width / (length * 10  + length + 1);
        gridLayoutGroup.constraintCount = length;
        gridLayoutGroup.cellSize = new Vector2(spacing * 10, spacing * 10);
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
    }
    public void OnRestartDataNotify(int currentNumberOfSquaresInRow, int currentNumberOfMines)
    {
        FixGridLayoutGroup(currentNumberOfSquaresInRow);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum EnumDirection
{
    UpperLeft, UpperCenter, UpperRight, MiddleLeft, MiddleRight, LowerLeft, LowerCenter, LowerRight
}
public enum EnumGameStatus
{
    Nothing, Won, Lost
}
public enum EnumTypeNotifier
{
    Data, Status
}
public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance { get { return instance; } }
    [SerializeField] private GridLayoutGroup gridLayoutGroup = null;
    [SerializeField] private Cell cellPrefab = null;
    [SerializeField] private int maxNumberOfSquaresInRow = 0;
    [SerializeField] private int minNumberOfSquaresInRow = 0;
    private int maxNumberOfMines = 0;
    private int minNumberOfMines = 0;
    private int currentNumberOfSquaresInRow = 0;
    private int currentNumberOfMines = 0;
    private Cell[,] boardData = null;
    private List<IObserver> observers = new List<IObserver>();
    List<EnumDirection> enumDirections = new List<EnumDirection>();
    private EnumGameStatus gameStatus;
    public Action<int, int, int>  onCheckAndRevealBlankCells;
    public Action onMassageLostGame;
    public Action onCheckWon;
    
    private void Awake()
    {
        instance = this;
        AddEnumDirections();
        // Action
        onCheckAndRevealBlankCells = HandleRevealBlankCells;
        onCheckWon = HandleCheckWon;
        onMassageLostGame = HandleMassageLostGame;
        // data
        currentNumberOfSquaresInRow = minNumberOfSquaresInRow;
        FixLimitNumberOfMines();
        currentNumberOfMines = minNumberOfMines;
        boardData = new Cell[maxNumberOfSquaresInRow, maxNumberOfSquaresInRow];
        // Cell
        CreateHiddenCells();
    }
    private void Start()
    {
        GenerateBoardData();
    }
    protected void NotifyObservers(EnumTypeNotifier type)
    {
        observers.ForEach(delegate (IObserver observer)
        {
            switch (type)
            {
                case EnumTypeNotifier.Data:
                    observer.OnRestartDataNotify(currentNumberOfSquaresInRow, currentNumberOfMines);
                    break;
                case EnumTypeNotifier.Status:
                    
                    observer.OnStatusNotify(gameStatus);
                    break;
            }
        });
    }
    private void HandleCheckWon()
    {
        EnumGameStatus tempStatus = EnumGameStatus.Won;
        for (int indexRow = 0; indexRow < currentNumberOfSquaresInRow; indexRow++)
        {
            for (int indexColumn = 0; indexColumn < currentNumberOfSquaresInRow; indexColumn++)
            {
                Cell cellIsChecked = boardData[indexRow, indexColumn];
                if (cellIsChecked.Value != 9 && cellIsChecked.IsClicked == false)
                {
                    tempStatus = EnumGameStatus.Nothing;
                }
            }
        }
        MassageGameStatus(tempStatus);
    }
    private void HandleMassageLostGame()
    {
        for (int indexRow = 0; indexRow < maxNumberOfSquaresInRow; indexRow++)
        {
            for (int indexColumn = 0; indexColumn < maxNumberOfSquaresInRow; indexColumn++)
            {
                if (boardData[indexColumn, indexRow].Value == 9)
                {
                    boardData[indexColumn, indexRow].onReveal?.Invoke(false);
                }
            }
        }
        MassageGameStatus(EnumGameStatus.Lost);
    }
    private void HandleRevealBlankCells(int indexRow, int indexColumn, int valueBlank = 0)
    {
        CheckTheValueOfTheNearCell(indexRow, indexColumn, EnumDirection.UpperCenter, valueBlank, true);
        CheckTheValueOfTheNearCell(indexRow, indexColumn, EnumDirection.MiddleRight, valueBlank, true);
        CheckTheValueOfTheNearCell(indexRow, indexColumn, EnumDirection.LowerCenter, valueBlank, true);
        CheckTheValueOfTheNearCell(indexRow, indexColumn, EnumDirection.MiddleLeft, valueBlank, true);
    }
    private void CreateHiddenCells()
    {
        for (int indexRow = 0; indexRow < maxNumberOfSquaresInRow; indexRow++)
        {
            for (int indexColumn = 0; indexColumn < maxNumberOfSquaresInRow; indexColumn++)
            {
                Cell newHiddenCell = Instantiate(cellPrefab, gridLayoutGroup.transform);
                newHiddenCell.Value = 0;
                newHiddenCell.IndexRow = indexRow;
                newHiddenCell.IndexColumn = indexColumn;
                boardData[indexRow, indexColumn] = newHiddenCell;
                newHiddenCell.gameObject.SetActive(false);
            }
        }
    }
    private void MassageGameStatus(EnumGameStatus gameStatus)
    {
        switch(gameStatus)
        {
            case EnumGameStatus.Nothing:
                {
                    this.gameStatus = EnumGameStatus.Nothing;
                }
                break;
            case EnumGameStatus.Won:
                {
                    this.gameStatus = EnumGameStatus.Won;
                    NotifyObservers(EnumTypeNotifier.Status);
                }
                break;
            case EnumGameStatus.Lost:
                {
                    this.gameStatus = EnumGameStatus.Lost;
                    NotifyObservers(EnumTypeNotifier.Status);
                }
                break;
        }
    }
    private void GenerateBoardData()
    {
        MassageGameStatus(EnumGameStatus.Nothing);
        ResetBoardData();
        DoEnableCells();
        SetMines();
        SetValueToBoardData();
        NotifyObservers(EnumTypeNotifier.Data);
    }
    private void ResetBoardData()
    {
        for (int indexRow = 0; indexRow < maxNumberOfSquaresInRow; indexRow++)
        {
            for (int indexColumn = 0; indexColumn < maxNumberOfSquaresInRow; indexColumn++)
            {
                boardData[indexRow, indexColumn].onReset?.Invoke();
            }
        }
    }
    private void DoEnableCells()
    {
        for (int indexRow = 0; indexRow < maxNumberOfSquaresInRow; indexRow++)
        {
            for (int indexColumn = 0; indexColumn < maxNumberOfSquaresInRow; indexColumn++)
            {
                bool actived = indexRow >= currentNumberOfSquaresInRow || indexColumn >= currentNumberOfSquaresInRow ? false : true;
                boardData[indexRow, indexColumn].gameObject.SetActive(actived);
            }
        }
    }
    private void SetMines( int valueMines = 9)
    {
        List<int> randomList = new List<int>();
        randomList = CreateRandomList();
        for (int indexMines = 0; indexMines < currentNumberOfMines; indexMines++)
        {
            int indexRandom = UnityEngine.Random.Range(0, randomList.Count);
            int indexRow = randomList[indexRandom] / currentNumberOfSquaresInRow;
            int indexColumn = randomList[indexRandom] % currentNumberOfSquaresInRow;
            boardData[indexRow, indexColumn].Value = valueMines;
            randomList.RemoveAt(indexRandom);
        }
    }
    private List<int> CreateRandomList()
    {
        List<int> list = new List<int>();
        for (int index = 0; index < currentNumberOfSquaresInRow * currentNumberOfSquaresInRow; index++)
        {
            list.Add(index);
        }
        return list;
    }
    private void SetValueToBoardData()
    {
        for (int indexRow = 0; indexRow < currentNumberOfSquaresInRow; indexRow++)
        {
            for (int indexColumn = 0; indexColumn < currentNumberOfSquaresInRow; indexColumn++)
            {
                SetValueToCell(indexRow, indexColumn);
            }
        }
    }
    private void SetValueToCell(int indexRow, int indexColumn, int valueMines = 9)
    {
        int result = 0;
        if (boardData[indexRow, indexColumn].Value != valueMines)
        {
            for (int indexList = 0; indexList < enumDirections.Count; indexList++)
            {
                result += CheckTheValueOfTheNearCell(indexRow, indexColumn, enumDirections[indexList], valueMines) ? 1 : 0;
            }
            boardData[indexRow, indexColumn].Value = result;
        }
    }
    private bool CheckTheValueOfTheNearCell(int indexRow, int indexColumn, EnumDirection enumDirection, int valueCheck, bool revelationIsAllowed = false)
    {
        bool result = false;
        // exclusion = -1 : khong co truong hop nao khong hop le
        // exclusion = 0 : index dang nho nhat => left, upper
        // exclusion = maxNumberOfSquaresInRow - 1 : index dang lon nhat   => right, lower
        int exclusionIndexRow = -1; 
        int exclusionIndexColumn = -1;
        int indexRowToCheck = indexRow;
        int indexColumnToCheck = indexColumn;
        switch(enumDirection)
        {
            case EnumDirection.UpperLeft:
                {
                    exclusionIndexRow = 0;
                    exclusionIndexColumn = 0;
                    indexRowToCheck -= 1;
                    indexColumnToCheck -= 1;
                }
                break;
            case EnumDirection.UpperCenter:
                {
                    exclusionIndexRow = 0;
                    indexRowToCheck -= 1;
                }
                break;
            case EnumDirection.UpperRight:
                {
                    exclusionIndexRow = 0;
                    exclusionIndexColumn = currentNumberOfSquaresInRow - 1;
                    indexRowToCheck -= 1;
                    indexColumnToCheck += 1;
                }
                break;
            case EnumDirection.MiddleLeft:
                {
                    exclusionIndexColumn = 0;
                    indexColumnToCheck -= 1;
                }
                break;
            case EnumDirection.MiddleRight:
                {
                    exclusionIndexColumn = currentNumberOfSquaresInRow - 1;
                    indexColumnToCheck += 1;
                }
                break;
            case EnumDirection.LowerLeft:
                {
                    exclusionIndexRow = currentNumberOfSquaresInRow - 1;
                    exclusionIndexColumn = 0;
                    indexRowToCheck += 1;
                    indexColumnToCheck -= 1;
                }
                break;
            case EnumDirection.LowerCenter:
                {
                    exclusionIndexRow = currentNumberOfSquaresInRow - 1;
                    indexRowToCheck += 1;
                }
                break;
            case EnumDirection.LowerRight:
                {
                    exclusionIndexRow = currentNumberOfSquaresInRow - 1;
                    exclusionIndexColumn = currentNumberOfSquaresInRow - 1;
                    indexRowToCheck += 1;
                    indexColumnToCheck += 1;
                }
                break;
        }
        if (indexRow == exclusionIndexRow || indexColumn == exclusionIndexColumn)
        {
            result = false;
        }
        else
        {
            Cell cellIsChecked = boardData[indexRowToCheck, indexColumnToCheck];
            if (cellIsChecked.Value == valueCheck)
            {
                if(revelationIsAllowed && !cellIsChecked.IsClicked)
                {
                    cellIsChecked.onReveal?.Invoke(true);
                }
                result = true;
            }
            else
            {
                result = false;
            }
        }
        return result;
    }
    private void FixLimitNumberOfMines()
    {
        minNumberOfMines = currentNumberOfSquaresInRow;
        maxNumberOfMines = currentNumberOfSquaresInRow * 2;
    }
    private void AddEnumDirections()
    {
        enumDirections.Add(EnumDirection.MiddleLeft);
        enumDirections.Add(EnumDirection.UpperCenter);
        enumDirections.Add(EnumDirection.MiddleRight);
        enumDirections.Add(EnumDirection.LowerCenter);
        enumDirections.Add(EnumDirection.UpperLeft);
        enumDirections.Add(EnumDirection.UpperRight);
        enumDirections.Add(EnumDirection.LowerLeft);
        enumDirections.Add(EnumDirection.LowerRight);
    }
    public void AddObserver(IObserver observer)
    {
        observers.Add(observer);
    }
    public void RemoveObserver(IObserver observer) 
    { 
        observers.Remove(observer);
    }
    public void RestartGame()
    {
        GenerateBoardData();
    }
    public void ChangeNumberOfSquaresInRow(int amount)
    {
        currentNumberOfSquaresInRow += amount;
        FixLimitNumberOfMines();
        currentNumberOfMines = currentNumberOfSquaresInRow;
        GenerateBoardData();
    }
    public void ChangeNumberOfMines(int amount)
    {
        currentNumberOfMines += amount;
        GenerateBoardData();
    }
    public int MaxNumberOfSquaresInRow { get { return maxNumberOfSquaresInRow; } }
    public int MinNumberOfSquaresInRow { get { return minNumberOfSquaresInRow; } }
    public int MaxNumberOfMines { get { return maxNumberOfMines; } }
    public int MinNumberOfMines { get { return minNumberOfMines; } }
}

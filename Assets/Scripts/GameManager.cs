using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private Cell cellPrefab;
    private Cell[,] data;
    private int cellsInRow = 6;
    private int amountOfMine = 6;
    private void Start()
    {
        ResetData();
    }
    private void ChangeCellsInRow(int amount)
    {
        cellsInRow += amount;
        FixedAmountOfMine();
        EditSizeBoard();
        ResetData();
    }
    private void FixedAmountOfMine()
    {
        if (amountOfMine < cellsInRow)
        {
            amountOfMine = cellsInRow;
        }
        if (amountOfMine > cellsInRow * 2)
        {
            amountOfMine = cellsInRow * 2;
        }

    }
    private void ChangeAmountOfMines(int amount)
    {
        amountOfMine += amount;
        ResetData();
    }
    private void ResetData()
    {
        data = new Cell[cellsInRow, cellsInRow];
        if(gridLayoutGroup.transform.childCount != 0)
        {
            RemoveOldCells();
        }
        InstantiateCells();
        AddMines(CreateRandomList(data.Length));
        AddValueInCells();
    }
    private void RemoveOldCells()
    {
        foreach (GameObject child in gridLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void EditSizeBoard()
    {
        int widthOfGridLayoutGroup = 840;
        float lengthOfSpacing = widthOfGridLayoutGroup / cellsInRow * 10 + cellsInRow + 1;
        float lengthOfCellSize = lengthOfSpacing * 10;
        gridLayoutGroup.constraintCount = cellsInRow;
        gridLayoutGroup.cellSize = new Vector2(lengthOfCellSize, lengthOfCellSize);
        gridLayoutGroup.spacing = new Vector2(lengthOfSpacing, lengthOfSpacing);
    }
    private void InstantiateCells()
    {
        for (int indexRow = 0; indexRow < cellsInRow; indexRow++)
        {
            for (int indexColumn = 0; indexColumn < cellsInRow; indexColumn++)
            {
                data[indexRow, indexColumn] = Instantiate(cellPrefab, gridLayoutGroup.transform);
            }
        }
    }
    private void AddMines(List<int> randomList)
    {
        for (int index = 0; index < amountOfMine; index++)
        {
            int indexRandom = UnityEngine.Random.Range(0, randomList.Count);
            int number = randomList[indexRandom];
            int rowOfData = number / cellsInRow;
            int columnOfData = number % cellsInRow;
            randomList.RemoveAt(indexRandom);
            data[rowOfData, columnOfData].Value = -1;
        }
    }
    private void AddValueInCells()
    {
        for (int indexOfRow = 0; indexOfRow < cellsInRow; indexOfRow++)
        {
            for (int indexOfColumn = 0; indexOfColumn < cellsInRow; indexOfColumn++)
            {
                if (data[indexOfRow, indexOfColumn].Value != -1)
                {
                    data[indexOfRow, indexOfColumn].Value = CheckMines(indexOfRow, indexOfColumn);
                }
            }
        }
    }
    private List<int> CreateRandomList(int length)
    {
        List<int> randomList = new List<int>();
        for (int index = 0; index < length; index++)
        {
            randomList.Add(index);
        }
        return randomList;
    }
    #region Check Mine 
    private int CheckMines(int indexOfRow, int indexOfColumn)
    {
        return CheckMineOnUpperLeft(indexOfRow, indexOfColumn)
            + CheckMineOnUpperCenter(indexOfRow, indexOfColumn)
            + CheckMineOnUpperRight(indexOfRow, indexOfColumn)
            + CheckMineOnMiddleLeft(indexOfRow, indexOfColumn)
            + CheckMineOnMiddleRight(indexOfRow, indexOfColumn)
            + CheckMineOnLowerLeft(indexOfRow, indexOfColumn)
            + CheckMineOnLowerCenter(indexOfRow, indexOfColumn)
            + CheckMineOnLowerRight(indexOfRow, indexOfColumn);
    }
    private int CheckMineOnUpperLeft(int indexOfRow, int indexOfColumn)
    {
        if (indexOfRow == 0 || indexOfColumn == 0)
        {
            return 0;
        }
        if (data[indexOfRow - 1, indexOfColumn - 1].Value != -1)
        {
            return 0;
        }
        return 1;
    }
    private int CheckMineOnUpperCenter(int indexOfRow, int indexOfColumn)
    {
        if (indexOfRow == 0)
        {
            return 0;
        }
        if (data[indexOfRow - 1, indexOfColumn].Value != -1)
        {
            return 0;
        }
        return 1;
    }
    private int CheckMineOnUpperRight(int indexOfRow, int indexOfColumn)
    {
        if (indexOfRow == 0 || indexOfColumn == Mathf.Sqrt(data.Length) - 1)
        {
            return 0;
        }
        if (data[indexOfRow - 1, indexOfColumn + 1].Value != -1)
        {
            return 0;
        }
        return 1;
    }
    private int CheckMineOnMiddleLeft(int indexOfRow, int indexOfColumn)
    {
        if (indexOfColumn == 0)
        {
            return 0;
        }
        if (data[indexOfRow, indexOfColumn - 1].Value != -1)
        {
            return 0;
        }
        return 1;
    }
    private int CheckMineOnMiddleRight(int indexOfRow, int indexOfColumn)
    {
        if (indexOfColumn == Mathf.Sqrt(data.Length) - 1)
        {
            return 0;
        }
        if (data[indexOfRow, indexOfColumn + 1].Value != -1)
        {
            return 0;
        }
        return 1;
    }
    private int CheckMineOnLowerLeft(int indexOfRow, int indexOfColumn)
    {
        if (indexOfRow == Mathf.Sqrt(data.Length) - 1 || indexOfColumn == 0)
        {
            return 0;
        }
        if (data[indexOfRow + 1, indexOfColumn - 1].Value != -1)
        {
            return 0;
        }
        return 1;
    }
    private int CheckMineOnLowerCenter(int indexOfRow, int indexOfColumn)
    {
        if (indexOfRow == Mathf.Sqrt(data.Length) - 1)
        {
            return 0;
        }
        if (data[indexOfRow + 1, indexOfColumn].Value != -1)
        {
            return 0;
        }
        return 1;
    }
    private int CheckMineOnLowerRight(int indexOfRow, int indexOfColumn)
    {
        if (indexOfRow == Mathf.Sqrt(data.Length) - 1 || indexOfColumn == Mathf.Sqrt(data.Length) - 1)
        {
            return 0;
        }
        if (data[indexOfRow + 1, indexOfColumn + 1].Value != -1)
        {
            return 0;
        }
        return 1;
    }
    #endregion
    public void IncreaseRow()
    {
        int maxRow = 10;
        if (cellsInRow < maxRow)
        {
            ChangeCellsInRow(1);
        }
    }
    public void DecreaseRow()
    {
        int minRow = 5;
        if (cellsInRow > minRow)
        {
            ChangeCellsInRow(-1);
        }
    }
    public void IncreaseMines()
    {
        int maxMine = cellsInRow * 2;
        if (amountOfMine < maxMine)
        {
            ChangeAmountOfMines(1);
        }
    }
    public void DecreaseMines()
    {
        int minMine = cellsInRow;
        if (amountOfMine > minMine)
        {
            ChangeAmountOfMines(-1);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;

public class SudokuBoardController : MonoBehaviour
{
    public static SudokuBoardController Instance;
    [SerializeField] private Transform table;
    [SerializeField] private bool printVals = false;
    [SerializeField] private bool saveToDisk;
    [SerializeField] private bool loadFromDisk;
    [SerializeField] private bool solveSudoku = false;
    [SerializeField] private bool solveOneStep = false;

    private int[,] fullGridArray = new int[9, 9];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (printVals)
        {
            printVals = false;
            PrintAllValuesInTable();
        }

        if (solveSudoku)
        {
            solveSudoku = false;
            HandleSolveSudoku(false);
        }

        if (solveOneStep)
        {
            solveOneStep = false;
            HandleSolveSudoku(true);
        }

        if (saveToDisk)
        {
            saveToDisk = false;
            SaveGridToDisk();
        }

        if (loadFromDisk)
        {
            loadFromDisk = false;
            LoadGridFromDisk();
        }
    }

    private int GetGridFillCount()
    {
        int count = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (fullGridArray[i, j] != 0)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void HandleSolveSudoku(bool solveOneStep)
    {
        int gridCount = GetGridFillCount();
        for (int k = 0; k < 1000; k++)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (solveOneStep && GetGridFillCount() > gridCount)
                    {
                        break;
                    }
                    fullGridArray[i, j] = TryFillValue(i, j);
                }
            }
        }

        int count = 0;
        foreach (var cellVal in fullGridArray)
        {
            string resultCell = cellVal.ToString();
            if (cellVal == 0)
            {
                resultCell = "";
            }
            table.GetChild(count).GetComponent<SetTextForCell>().SetText(resultCell);
            count++;
        }
    }

    private int TryFillValue(int row, int col)
    {
        if (fullGridArray[row, col] != 0)
        {
            return fullGridArray[row, col];
        }

        HashSet<int> myHash = new HashSet<int>();
        for (int i = 0; i < 9; i++) //1 to 9 added to hash set
        {
            myHash.Add(i + 1);
        }

        //column trim
        for (int i = 0; i < 9; i++)
        {
            if (fullGridArray[row, i] != 0)
            {
                myHash.Remove(fullGridArray[row, i]);
            }
        }

        //row trim
        for (int i = 0; i < 9; i++)
        {
            if (fullGridArray[i, col] != 0)
            {
                myHash.Remove(fullGridArray[i, col]);
            }
        }

        List<int> elementsInQuad = GetElementsInQuad(GetQuadrantNumberFromRowAndCol(row, col), true, new List<int> { -1 });
        foreach (var element in elementsInQuad)
        {
            myHash.Remove(element);
        }

        if (myHash.Count == 1)
        {
            return myHash.ToArray()[0];
        }

        //Now check for all possible fits 
        foreach (var possibleCandidate in myHash)
        {
            if (CheckIfColumnFit(possibleCandidate, row, col))
            {
                return possibleCandidate;
            }
        }
        
        foreach (var possibleCandidate in myHash)
        {
            if (CheckIfRowFit(possibleCandidate, row, col))
            {
                return possibleCandidate;
            }
        }
        

        return fullGridArray[row, col];
    }

    private bool CheckIfColumnFit(int possibleCandidate, int row, int col)
    {
        int resultCount = 0;
        int quadNum = GetQuadrantNumberFromRowAndCol(row, col);
        List<int> adjacentQuads = GetQuadrantsInSameColumn(quadNum);
        if (adjacentQuads.Count != 2)
        {
            Debug.LogError("OH FUCKKKKKKK");
            return false;
        }

        if ((CheckIfNumExistsInQuad(possibleCandidate, adjacentQuads[0]) || CheckIfColumnOfQuadrantIsFull(adjacentQuads[0],col))
            && (CheckIfNumExistsInQuad(possibleCandidate, adjacentQuads[1]) || CheckIfColumnOfQuadrantIsFull(adjacentQuads[1],col)))
        {
            List<int> rowsToCheck = GetRowIndicesFromQuad(quadNum);
            foreach (var rowToCheck in rowsToCheck)
            {
                if (rowToCheck == row)
                {
                    continue;
                }

                if (fullGridArray[rowToCheck, col] != 0 || CheckIfNumberExistsInRow(possibleCandidate,rowToCheck))
                {
                    resultCount++;
                }
            }
        }

        return resultCount == 2;
    }

    private bool CheckIfColumnOfQuadrantIsFull(int adjacentQuad, int col)
    {
        int startRow = Mathf.FloorToInt(adjacentQuad / 3) * 3;
        if (fullGridArray[startRow, col] != 0
            && fullGridArray[startRow + 1, col] != 0
            && fullGridArray[startRow + 2, col] != 0)
        {
            return true;
        }

        return false;
    }
    
    private bool CheckIfRowOfQuadrantIsFull(int adjacentQuad, int row)
    {
        int startCol = adjacentQuad % 3 * 3;
        if (fullGridArray[row, startCol] != 0
            && fullGridArray[row, startCol + 1] != 0
            && fullGridArray[row, startCol + 2] != 0)
        {
            return true;
        }

        return false;
    }

    private bool CheckIfRowFit(int possibleCandidate, int row, int col)
    {
        int resultCount = 0;
        int quadNum = GetQuadrantNumberFromRowAndCol(row, col);
        List<int> adjacentQuads = GetQuadrantsInSameRow(quadNum);
        if (adjacentQuads.Count != 2)
        {
            Debug.LogError("OH FUCKKKKKKK");
            return false;
        }
    
        if ((CheckIfNumExistsInQuad(possibleCandidate, adjacentQuads[0]) || CheckIfRowOfQuadrantIsFull(adjacentQuads[0],row))
            && (CheckIfNumExistsInQuad(possibleCandidate, adjacentQuads[1]) ||CheckIfRowOfQuadrantIsFull(adjacentQuads[1],row)))
        {
            List<int> colsToCheck = GetColIndicesFromQuad(quadNum);
            foreach (var colToCheck in colsToCheck)
            {
                if (colToCheck == col)
                {
                    continue;
                }
    
                if (fullGridArray[row, colToCheck] != 0 || CheckIfNumberExistsInCol(possibleCandidate,colToCheck))
                {
                    resultCount++;
                }
            }
        }
    
        return resultCount == 2;
    }

    private bool CheckIfNumberExistsInCol(int possibleCandidate, int col)
    {
        for (int i = 0; i < 9; i++)
        {
            if (fullGridArray[i,col] == possibleCandidate)
            {
                return true;
            }
        }

        return false;
    }
    
    private bool CheckIfNumberExistsInRow(int possibleCandidate, int row)
    {
        for (int i = 0; i < 9; i++)
        {
            if (fullGridArray[row, i] == possibleCandidate)
            {
                return true;
            }
        }

        return false;
    }

    private List<int> GetColIndicesFromQuad(int quadNum)
    {
        List<int> retList = new List<int>();
        int startCol = quadNum % 3 * 3;
        retList.Add(startCol);
        retList.Add(startCol + 1);
        retList.Add(startCol + 2);
        return retList;
    }

    private List<int> GetRowIndicesFromQuad(int quadNum)
    {
        List<int> retList = new List<int>();
        int startRow = Mathf.FloorToInt(quadNum / 3) * 3;
        retList.Add(startRow);
        retList.Add(startRow + 1);
        retList.Add(startRow + 2);
        return retList;
    }

    private bool CheckIfNumExistsInQuad(int possibleCandidate, int adjacentQuad)
    {
        List<int> numsInQuad = GetElementsInQuad(adjacentQuad, true, new List<int> { -1 });
        if (numsInQuad.Contains(possibleCandidate))
        {
            return true;
        }
    
        return false;
    }
    
    private List<int> GetQuadrantsInSameColumn(int quadNum)
    {
        List<int> retNums = new List<int>();
        for (int i = quadNum + 3; i < 9; i += 3)
        {
            retNums.Add(i);
        }

        for (int i = quadNum - 3; i >= 0; i -= 3)
        {
            retNums.Add(i);
        }

        return retNums;
    }
    
    private List<int> GetQuadrantsInSameRow(int quadNum)
    {
        List<int> retNums = new List<int>();
        int startQuad = Mathf.FloorToInt(quadNum / 3) * 3;
        for (int i = 0; i < 3; i++)
        {
            if (startQuad + i != quadNum)
            {
                retNums.Add(startQuad + i);    
            }
        }

        return retNums;
    }


    private int GetQuadrantNumberFromRowAndCol(int row, int col)
    {
        return (Mathf.FloorToInt(row / 3) * 3) + Mathf.FloorToInt(col / 3);
    }

    private List<int> GetElementsInQuad(int quadNum, bool shouldExclude, List<int> excludeCols)
    {
        List<int> retList = new List<int>();

        int startRow = Mathf.FloorToInt(quadNum / 3) * 3;
        int startCol = quadNum % 3 * 3;

        for (int i = startRow; i < startRow + 3; i++)
        {
            for (int j = startCol; j < startCol + 3; j++)
            {
                if (shouldExclude)
                {
                    if (!excludeCols.Contains(j))
                    {
                        retList.Add(fullGridArray[i, j]);
                    }
                }
                else
                {
                    if (excludeCols.Contains(j))
                    {
                        retList.Add(fullGridArray[i, j]);
                    }
                }
            }
        }

        return retList;
    }

    public void SetCellInArray(int row, int col, int val)
    {
        fullGridArray[row, col] = val;
    }

    private void PrintAllValuesInTable()
    {
        foreach (var cell in fullGridArray)
        {
            Debug.Log(cell);
        }
    }

    private void SaveGridToDisk()
    {
        int count = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                PlayerPrefs.SetInt(count.ToString(), fullGridArray[i, j]);
                count++;
            }
        }
    }

    private void LoadGridFromDisk()
    {
        int count = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                fullGridArray[i, j] = PlayerPrefs.GetInt(count.ToString());
                count++;
            }
        }

        count = 0;
        foreach (var cellVal in fullGridArray)
        {
            string resultCell = cellVal.ToString();
            if (cellVal == 0)
            {
                resultCell = "";
            }
            table.GetChild(count).GetComponent<SetTextForCell>().SetText(resultCell);
            count++;
        }
    }
}
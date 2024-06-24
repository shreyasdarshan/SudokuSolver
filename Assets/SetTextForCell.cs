using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetTextForCell : MonoBehaviour
{
    private bool shouldAcceptInput = false;
    private TextMeshProUGUI textMeshPro;
    private int cellID = 0;
    private int rowNum;
    private int colNum;

    private void Awake()
    {
        textMeshPro = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        int count = 0;
        foreach (var childObj in transform.parent)
        {
            if ((Transform)childObj == transform)
            {
                break;
            }
            count++;
        }

        cellID = count;
        colNum = cellID % 9;
        rowNum = Mathf.FloorToInt(cellID / 9);
    }

    public void AcceptInput()
    {
        shouldAcceptInput = true;
    }

    public void SetText(string text)
    {
        textMeshPro.text = text;
    }

    private void Update()
    {
        if (!shouldAcceptInput)
        {
            return;
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            textMeshPro.text = "1";
            shouldAcceptInput = false;
            SudokuBoardController.Instance.SetCellInArray(rowNum,colNum,1);
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            textMeshPro.text = "2";
            shouldAcceptInput = false;
            SudokuBoardController.Instance.SetCellInArray(rowNum,colNum,2);
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            textMeshPro.text = "3";
            shouldAcceptInput = false;
            SudokuBoardController.Instance.SetCellInArray(rowNum,colNum,3);
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            textMeshPro.text = "4";
            shouldAcceptInput = false;
            SudokuBoardController.Instance.SetCellInArray(rowNum,colNum,4);
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            textMeshPro.text = "5";
            shouldAcceptInput = false;
            SudokuBoardController.Instance.SetCellInArray(rowNum,colNum,5);
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            textMeshPro.text = "6";
            shouldAcceptInput = false;
            SudokuBoardController.Instance.SetCellInArray(rowNum,colNum,6);
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha7))
        {
            textMeshPro.text = "7";
            shouldAcceptInput = false;
            SudokuBoardController.Instance.SetCellInArray(rowNum,colNum,7);
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            textMeshPro.text = "8";
            shouldAcceptInput = false;
            SudokuBoardController.Instance.SetCellInArray(rowNum,colNum,8);
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha9))
        {
            textMeshPro.text = "9";
            shouldAcceptInput = false;
            SudokuBoardController.Instance.SetCellInArray(rowNum,colNum,9);
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            textMeshPro.text = "";
            shouldAcceptInput = false;
            SudokuBoardController.Instance.SetCellInArray(rowNum,colNum,0);
        }
    }
}

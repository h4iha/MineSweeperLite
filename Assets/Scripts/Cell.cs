using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    GameManager gameManager;
    [Header("Index")]
    [SerializeField] private int indexRow = 0;
    [SerializeField] private int indexColumn = 0;
    [Header("Manual")]
    [SerializeField] private Sprite[] icons = null;
    [SerializeField] private Image background = null;
    [SerializeField] private GameObject unrevealedGameObject = null;
    [SerializeField] private GameObject flagGameObject = null;
    [SerializeField] private Image icon = null;
    [Header("Detail")]
    [SerializeField] private int valueOfCell = 0;
    private bool isClicked = false;
    private bool isFlagged = false;
    public Action onReset;
    public Action<bool> onReveal;
    private void Start()
    {
        gameManager = GameManager.Instance;
        onReset = HandleReset;
        onReveal = HandleReveal;
    }
    private void ChangeIcon()
    {
        icon.sprite = icons[valueOfCell];
    }
    private void CheckValue()
    {
        if (valueOfCell == 9)
        {
            background.color = Color.red;
            gameManager.onMassageLostGame?.Invoke();
        }
        else if (valueOfCell == 0)
        {
            gameManager.onCheckAndRevealBlankCells?.Invoke(indexRow, indexColumn, valueOfCell);
        }
    }
    private bool IsFlagged
    {
        set
        {
            isFlagged = value;
            if (isFlagged)
            {
                flagGameObject.SetActive(true);
            }
            else
            {
                flagGameObject.SetActive(false);
            }
        }
    }
    public int IndexRow
    {
        set { indexRow = value; }
    }
    public int IndexColumn
    {
        set { indexColumn = value; }
    }
    public int Value
    {
        get { return valueOfCell; }
        set
        {
            valueOfCell = value;
            ChangeIcon();
        }
    }
    public bool IsClicked
    {
        get { return isClicked; }
        set 
        { 
            isClicked = value;
            if (isClicked)
            {
                unrevealedGameObject.SetActive(false);
            }
            else
            {
                unrevealedGameObject.SetActive(true);
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isClicked)
        {
            if (!isFlagged)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    HandleReveal(true);
                }
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (!isFlagged)
                {
                    IsFlagged = true;
                }
                else
                {
                    IsFlagged = false;
                }
            }
        }
    }
    private void HandleReveal(bool isCheckedValue)
    {
        if (!isFlagged)
        {
            IsClicked = true;
            if (isCheckedValue)
            {
                CheckValue();
                gameManager.onCheckWon?.Invoke();
            }
        }
    }
    private void HandleReset()
    {
        background.color = Color.white;
        Value = 0;
        IsClicked = false;
        IsFlagged = false;
    }
}

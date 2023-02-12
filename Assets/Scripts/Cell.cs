using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    [Header("Manual")]
    [SerializeField] private Sprite[] allIconOfCell = null;
    [SerializeField] private GameObject unrevealedGameObject = null;
    [SerializeField] private GameObject flagGameObject = null;
    [SerializeField] private Image iconOfCell = null;
    [Header("Detail")]
    [SerializeField] private int valueOfCell = 0;
    private int indexRow = 0;
    private int indexColumn = 0;
    private bool isClicked = false;
    private bool isFlagged = false;
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
        set { valueOfCell = value; ChangeIcon(); }
    }
    public bool IsClicked
    {
        get { return isClicked; }
    }
    
    private void ChangeIcon()
    {
        if(valueOfCell != -1)
        {
            iconOfCell.sprite = allIconOfCell[valueOfCell];
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
                    RevealCell();
                    CheckValue();
                }
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if(!isFlagged)
                {
                    Flagged();
                }
                else
                {
                    RemoveTheFlag();
                }
            }
        }
    }
    private void RevealCell()
    {
        isClicked = true;
        unrevealedGameObject.SetActive(false);
    }
    private void Flagged()
    {
        ChangeActiveOfFlag(true);
    }
    private void RemoveTheFlag()
    {
        ChangeActiveOfFlag(false);
    }
    private void ChangeActiveOfFlag(bool isTrue)
    {
        flagGameObject.SetActive(isTrue);
        isFlagged = !isFlagged;
    }
    private void CheckValue()
    {
        if (valueOfCell == -1)
        {
            LostGame();
        }
        else if(valueOfCell == 0)
        {
            RevealAutomaticallyBlankCell();
        }
    }
    private void RevealAutomaticallyBlankCell()
    {

    }
    private void LostGame()
    {

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    SlotBehaviour[,] board = new SlotBehaviour[3,3];
    [SerializeField] SlotBehaviour slotPrefab;
    [SerializeField] Transform slotHolder;

    [HideInInspector] public bool playersTurn = true;
    void Start()
    {
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                SlotBehaviour temp = Instantiate(slotPrefab, slotHolder);
                temp.gameManager = this;
                temp.index = new int[2] {x,y};
                board[x,y] = temp;
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerHasMoved()
    {
        playersTurn = false;
        // if (CheckForWins(SlotState.Player))
        if (CreatesTwiInARow(board[1,0].index,SlotState.Player)){ Debug.Log("Player Wins"); }
        AiTurn();
    }

    private void AiTurn()
    {

        MinMax();

        if(CheckForWins(SlotState.AI)) { Debug.Log("Ai wins"); }

        playersTurn = true;
    }


    private Vector2 MinMax()
    {
        return Vector2.down;
    }

    private bool CreatesTwiInARow(int[] index, SlotState targetState)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (y == 0 && x == 0)
                    continue;

                int deltaX = x + index[0];
                int deltaY = y + index[1];
                if (deltaY>= board.GetLength(0) || deltaY < 0 || deltaX >= board.GetLength(1) || deltaX < 0)
                    continue;

                if (board[deltaX, deltaY].State == targetState)
                    return true;
            }
        }



        return false;
    }





































    private bool CheckForWins(SlotState state)
    {
        //Checks rows
        for (int i = 0; i < board.GetLength(0); i ++)
        {
            if (board[0,i].State == state && board[1,i].State == state && board[2,i].State == state)
            {
                return true;
            }
        }

        //Checks coloumns
        for (int i = 0; i < board.GetLength(1); i ++)
        {
            if (board[i, 0].State == state && board[i, 1].State == state && board[i, 2].State == state)
            {
                return true;
            }
        }

        //Checks diagonals
        if ((board[0, 0].State == state && board[1, 1].State == state && board[2, 2].State == state) ||
            (board[0, 2].State == state && board[1, 1].State == state && board[2, 0].State == state))
        {
            return true;
        }

        return false;
    }
}

 public enum SlotState
{
    Neutral,
    Player,
    AI
}

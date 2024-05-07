using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //TODO:
    //*Finns behöver boardet vara slotbehavoiur[,] i minmax?

    SlotBehaviour[,] board = new SlotBehaviour[3,3];
    public SlotState[,] stateBoard = new SlotState[3, 3];
    [SerializeField] SlotBehaviour slotPrefab;
    [SerializeField] Transform slotHolder;
    public int MaxSearchDepth = 3;

    [HideInInspector] public bool playersTurn = true;
    void Start()
    {
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                SlotBehaviour temp = Instantiate(slotPrefab, slotHolder);
                temp.gameManager = this;
                temp.index = new Vector2Int(x,y);
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
        if (CheckForWins(board,SlotState.Player)){ Debug.Log("Player Wins"); }
        AiTurn();
    }

    private void AiTurn()
    {

        //MinMax();


        Debug.Log("af");

        if(CheckForWins(board, SlotState.AI)) { Debug.Log("Ai wins"); }

        Vector2Int temp = MinMax(board, 1, true);

        board[temp.x, temp.y].State = SlotState.AI;
        playersTurn = true;
    }


    private Vector2Int MinMax(SlotBehaviour[,] board, int deapth, bool maxPlayer)
    {
        if(deapth ==0)
        {
            //user dumb dumb
            return new Vector2Int(0,0);
            Debug.LogWarning("Deapth is 0");
        }

        List<Vector2Int> moves;
        moves = GetPossibleMoves(board);
        
        //Node is a leaf  
        if(moves.Count == 0)
        {
            return new Vector2Int(-1,-1);
            Debug.LogError("No moves");
        }

        int bestMoveValue = int.MinValue;
        Vector2Int bestMove = new Vector2Int(0,0);

        for (int i = 0; i < moves.Count; i++)
        {
            SlotBehaviour[,] modifiedBoard = (SlotBehaviour[,])board.Clone();
            MakeMove(moves[i],SlotState.AI,ref modifiedBoard);

            int newValue = MinMaxValue(board,deapth+1,!maxPlayer);

            if(newValue > bestMoveValue)
            {
                bestMoveValue = newValue;
                bestMove = moves[i];
            }
        }

        return bestMove;
    }

    private void MakeMove(Vector2Int position, SlotState state,ref SlotBehaviour[,] board)
    {
        board[position.x, position.y].State = state;
    }
    private int MinMaxValue(SlotBehaviour[,] board, int searchDepth, bool playerMax)
    {
        if (MaxSearchDepth <= searchDepth)
            return GetHeuristicValue(board);

        Vector2Int move;
        List<Vector2Int> moves = GetPossibleMoves(board);
        
        if(moves.Count == 0)
        {
            return MinMaxValue(board, searchDepth + 1, !playerMax);
        }

        int bestValue = int.MaxValue;
        if (playerMax)
            bestValue = int.MinValue;

        for (int i = 0; i < moves.Count; i++)
        {
            SlotBehaviour[,] modifiedBoard = (SlotBehaviour[,])board.Clone();
            MakeMove(moves[i], SlotState.AI, ref modifiedBoard);

            int newValue = MinMaxValue(modifiedBoard, searchDepth+1, !playerMax);
            if (playerMax)
            {
                if (newValue > bestValue)
                    bestValue = newValue; 
            }
            else
            {
                if (newValue < bestValue)
                    bestValue = newValue;
            }
        }
        return bestValue;
    }


    private List<Vector2Int> GetPossibleMoves(SlotBehaviour[,] board)
    {
        List<Vector2Int> temp = new List<Vector2Int>();
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                if (board[x, y].State == SlotState.Neutral)
                    temp.Add(board[x, y].index);
            }
        }

        return temp;
    }

    private SlotBehaviour[,] DeepClone(SlotBehaviour[,] list)
    {
        for (int i = 0; i < list.GetLength(0); i++)
        {
            for (int j = 0; j < list.GetLength(1); j++)
            {
                list[i, j] = list[i, j].Clone();
            }
        }
    }

    private int GetHeuristicValue(SlotBehaviour[,] board)
    {
        if (CheckForWins(board, SlotState.AI))
            return 1;
        return 0;
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
                if (deltaY >= board.GetLength(0) || deltaY < 0 || deltaX >= board.GetLength(1) || deltaX < 0)
                    continue;

                if (board[deltaX, deltaY].State == targetState)
                    return true;
            }
        }
        return false;
    }


    private bool CheckForWins(SlotBehaviour[,] board,SlotState state)
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

//public class Vector2Int
//{
//    public int x;
//    public int y;
//    public Vector2Int(int x, int y)
//    {
//        this.x = x;
//        this.y = y;
//    }
//}

 public enum SlotState
{
    Neutral,
    Player,
    AI
}

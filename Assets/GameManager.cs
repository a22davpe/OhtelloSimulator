using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //TODO:
    //*Finns beh�ver boardet vara slotbehavoiur[,] i minmax?

    SlotBehaviour[,] board = new SlotBehaviour[8,8];
    public SlotState[,] stateBoard = new SlotState[8, 8];
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
                stateBoard[x, y] = SlotState.Neutral;
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
        if (CheckForWins(stateBoard,SlotState.Player)){ Debug.Log("Player Wins"); }
        AiTurn();
    }

    private void AiTurn()
    {

        //MinMax();

        

        if(CheckForWins(stateBoard, SlotState.AI)) { Debug.Log("Ai wins"); }

        Vector2Int temp = MinMax(stateBoard, 0, true);

        board[temp.x, temp.y].State = SlotState.AI;
        stateBoard[temp.x, temp.y] = SlotState.AI;
        Flip(temp, SlotState.AI);
        playersTurn = true;
    }


    private Vector2Int MinMax(SlotState[,] board, int deapth, bool maxPlayer)
    {

        List<Vector2Int> moves;
        moves = GetPossibleMoves(board);
        
        //Node is a leaf  
        if(moves.Count == 0)
        {
            Debug.LogError("No moves");
            return new Vector2Int(-1,-1);
        }

        int bestMoveValue = int.MinValue;
        Vector2Int bestMove = new Vector2Int(0,0);

        for (int i = 0; i < moves.Count; i++)
        {
            SlotState[,] modifiedBoard = (SlotState[,])board.Clone();
            MakeMove(moves[i],SlotState.AI,ref modifiedBoard);

            int newValue = MinMaxValue(modifiedBoard,deapth+1,maxPlayer);
            //Debug.Log($"minmax: {newValue}");
            if(newValue > bestMoveValue)
            {
                bestMoveValue = newValue;
                bestMove = moves[i];
            }
        }

        return bestMove;
    }

    private void MakeMove(Vector2Int position, SlotState state,ref SlotState[,] board)
    {
        board[position.x, position.y] = state;
    }


    private int MinMaxValue(SlotState[,] board, int searchDepth, bool playerMax)
    {
        if (MaxSearchDepth <= searchDepth)
            return GetHeuristicValue(board);
        
        List<Vector2Int> moves = GetPossibleMoves(board);
        //Debug.Log($"MovesCount: {moves.Count}");
        
        if(moves.Count == 0)
        {
            return MinMaxValue(board, searchDepth + 1, !playerMax);
        }

        int bestValue = int.MaxValue;
        if (playerMax)
            bestValue = int.MinValue;

        //den plasserar bara ut ai!!!!!!!!!!!!!!!
        for (int i = 0; i < moves.Count; i++)
        {
            SlotState[,] modifiedBoard = (SlotState[,])board.Clone();
            if(playerMax)
                MakeMove(moves[i], SlotState.AI, ref modifiedBoard);
            else
                MakeMove(moves[i], SlotState.Player, ref modifiedBoard);


            int newValue = MinMaxValue(modifiedBoard, searchDepth+1, !playerMax);
            //Debug.Log($"newValue: {newValue}, Depth: {searchDepth}");
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
            //Debug.Log($"bestValue: {bestValue}");
        }
        return bestValue;
    }


    private List<Vector2Int> GetPossibleMoves(SlotState[,] board)
    {
        List<Vector2Int> temp = new List<Vector2Int>();
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                if (board[x, y] == SlotState.Neutral)
                    temp.Add(new Vector2Int(x,y));
            }
        }

        return temp;
    }

    //borde vet om det är ai eller spelare som checkar
    private int GetHeuristicValue(SlotState[,] board)
    {
        var temp = Score(board,SlotState.AI) - Score(board, SlotState.Player);
        //Debug.Log(temp);
        return temp;
    }

    private int Score(SlotState[,] board, SlotState slotState)
    {
        int temp = 0;
        foreach (var item in board)
        {
            if (item == slotState)
                temp++;
        }
        return temp;
    }

    public void Flip(Vector2Int newTile, SlotState state)
    {

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (y == 0 && x == 0)
                    continue;

                if(CheckDirection(new Vector2Int(x,y),newTile,state, out List<Vector2Int> result))
                {
                    foreach (var item in result)
                    {
                        board[item.x,item.y].State = state;
                    }
                }
            }
        }



    }

    private bool CheckDirection(Vector2Int direction,Vector2Int startPoint, SlotState userState, out List<Vector2Int> result)
    {
        SlotState opponentState = SlotState.Player;

        if (userState == SlotState.Player)
            opponentState = SlotState.AI;

        result = new List<Vector2Int>();
        int i = 0;
        while (true)
        {
            i++;
            int deltaX = startPoint.x + direction.x*i;
            int deltaY = startPoint.y + direction.y*i;
            
            //checkar om utanför board
            if (deltaY >= board.GetLength(0) || deltaY < 0 || deltaX >= board.GetLength(1) || deltaX < 0)
                return false;

            if (stateBoard[deltaX, deltaY] == SlotState.Neutral)
                return false;

            if (stateBoard[deltaX, deltaY] == userState)
                return true;

            if (stateBoard[deltaX, deltaY] == opponentState)
                result.Add(new Vector2Int(deltaX, deltaY));
            

            if (i > 10)
                break;
        }

        Debug.LogError("Något är trasigt");
        return false;
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


    private bool CheckForWins(SlotState[,] board,SlotState state)
    {
        //Checks rows
        for (int i = 0; i < board.GetLength(0); i ++)
        {
            if (board[0,i] == state && board[1,i] == state && board[2,i] == state)
            {
                return true;
            }
        }

        //Checks coloumns
        for (int i = 0; i < board.GetLength(1); i ++)
        {
            if (board[i, 0] == state && board[i, 1] == state && board[i, 2] == state)
            {
                return true;
            }
        }

        //Checks diagonals
        if ((board[0, 0] == state && board[1, 1] == state && board[2, 2] == state) ||
            (board[0, 2] == state && board[1, 1] == state && board[2, 0] == state))
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

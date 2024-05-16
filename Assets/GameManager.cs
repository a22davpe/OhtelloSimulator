using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    private bool GameIsOver => Score(stateBoard, SlotState.Neutral) == 0;

    public UnityEvent onWin;
    public UnityEvent onLose;
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
                
                if((x== 3 && y == 3) || (x==4 && y==4))
                {
                    temp.State = SlotState.AI;
                }

                else if ((x == 3 && y == 4) || (x == 4 && y == 3))
                {
                    temp.State = SlotState.Player;
                }
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

        if (GameIsOver)
        {
            if (Score(stateBoard, SlotState.Player) > Score(stateBoard, SlotState.Player))
                onWin.Invoke();
            else
                onLose.Invoke();
            return;
        }

        AiTurn();
    }

    private void AiTurn()
    {
        //Kollar om AI har några pjäser kvar
        if (Score(stateBoard, SlotState.AI) == 0)
            onWin.Invoke(); 

        Vector2Int temp = MinMax(stateBoard, 0, true);

        if (temp != new Vector2Int(-1, -1))
        { 
            board[temp.x, temp.y].State = SlotState.AI;
            stateBoard[temp.x, temp.y] = SlotState.AI;
            Flip(temp, SlotState.AI); 
        }

        if (GameIsOver)
        {
            if (Score(stateBoard, SlotState.Player) > Score(stateBoard, SlotState.Player))
                onWin.Invoke();
            else
                onLose.Invoke();
            return;
        }

        if (Score(stateBoard, SlotState.Player) == 0)
            onLose.Invoke();

        if (GetPossibleMoves(stateBoard, SlotState.Player).Count == 0)
            AiTurn();
        playersTurn = true;
    }


















    //
    //                      Minmax
    //
    private Vector2Int MinMax(SlotState[,] board, int deapth, bool maxPlayer)
    {

        List<Vector2Int> moves;
        moves = GetPossibleMoves(board, SlotState.AI);
        
        if(moves.Count == 0)
        {
            Debug.Log("No moves ai");
            return new Vector2Int(-1,-1);
        }

        int bestMoveValue = int.MinValue;
        Vector2Int bestMove = new Vector2Int(0,0);

        for (int i = 0; i < moves.Count; i++)
        {
            SlotState[,] modifiedBoard = (SlotState[,])board.Clone();
            MakeMove(moves[i],SlotState.AI,ref modifiedBoard);

            int newValue = MinMaxValue(modifiedBoard,1,!maxPlayer);
            //Debug.Log($"minmax: {newValue}, pos: {moves[i]}");
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
        Flip(position, state, ref board);
    }


    private int MinMaxValue(SlotState[,] board, int searchDepth, bool playerMax)
    {
        if (MaxSearchDepth <= searchDepth)
            return GetHeuristicValue(board);

        List<Vector2Int> moves;
        if (playerMax)
            moves = GetPossibleMoves(board, SlotState.AI);
        else
            moves = GetPossibleMoves(board, SlotState.Player);
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


    private List<Vector2Int> GetPossibleMoves(SlotState[,] board, SlotState player)
    {
        List<Vector2Int> temp = new List<Vector2Int>();
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                if(board[x, y] != SlotState.Neutral)
                    continue;

                if (IsPlayable(new Vector2Int(x, y),player, board)){
                    temp.Add(new Vector2Int(x, y));
                }
            }
        }

        return temp;
    }


    public bool IsPlayable(Vector2Int position, SlotState playerState, SlotState[,] board)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (y == 0 && x == 0)
                    continue;

                if (CheckDirection(new Vector2Int(x, y), position, playerState,board, out List<Vector2Int> result))
                {
                    return true;
                }
            }
        }
        return false;
    }

    //borde vet om det är ai eller spelare som checkar
    private int GetHeuristicValue(SlotState[,] board)
    {
        var temp = Score(board,SlotState.AI) - Score(board, SlotState.Player);
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

    public void Flip(Vector2Int newTile, SlotState state, ref SlotState[,] board)
    {

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (y == 0 && x == 0)
                    continue;

                if(CheckDirection(new Vector2Int(x,y),newTile,state,board, out List<Vector2Int> result))
                {
                    foreach (var item in result)
                    {
                        board[item.x,item.y] = state;
                    }
                }
            }
        }
    }

    public void Flip(Vector2Int newTile, SlotState state)
    {

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (y == 0 && x == 0)
                    continue;

                if (CheckDirection(new Vector2Int(x, y), newTile, state, stateBoard, out List<Vector2Int> result))
                {
                    foreach (var item in result)
                    {
                        board[item.x, item.y].State = state;
                    }
                }
            }
        }
    }


    private bool CheckDirection(Vector2Int direction,Vector2Int startPoint, SlotState userState, SlotState[,] board, out List<Vector2Int> result)
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

            if (board[deltaX, deltaY] == SlotState.Neutral)
                return false;

            if (board[deltaX, deltaY] == userState)
            {
                if(i>1)
                    return true;
                return false;
            }

            if (board[deltaX, deltaY] == opponentState)
                result.Add(new Vector2Int(deltaX, deltaY));
            

            if (i > 10)
                break;
        }

        Debug.LogError("Något är trasigt");
        return false;
    }

}

 public enum SlotState
{
    Neutral,
    Player,
    AI
}

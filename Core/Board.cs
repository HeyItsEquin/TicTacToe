namespace TicTacToe.Core;

// Player CurrentPlayer, int[] GetLegalMoves(), bool MakeMove(int), void UndoMove(), Player IsGameOver()

public enum Player
{
    None = 0,
    X,
    O,
    Both
}

public class Board
{
    public Player CurrentPlayer { get; private set; }
    private Player hasWon;

    private Player[] boardState = new Player[9];

    private readonly Stack<int> moveHistory = new();
    
    public Board()
    {
        // Could've done this outside of constructor
        // this just felt better logic-wise
        CurrentPlayer = Player.X;
    }
    
    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < 3; i++)
        {
            int j;
            for (j = 0; j < 3; j++)
            {
                s += $" {GetPlayerString(boardState[ToIndex(3,i,j)])} ";
                if (j != 2) s += "|";
            }
            if (i != 2) s += "\n";
        }
        return s;
    }

    public static string GetPlayerString(Player p)
    {
        return p switch
        {
            Player.X => "X",
            Player.O => "O",
            _ => "-"
        };
    }

    private void SwitchPlayer()
        => CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;

    public bool MakeMove(int move)
    {
        if (!IsLegalMove(move)) return false;
        boardState[move] = CurrentPlayer;
        if (WinningMove(move)) hasWon = CurrentPlayer;
        moveHistory.Push(move);
        SwitchPlayer(); 
        return true;
    }

    public void UndoMove()
    {
        var lastMove = moveHistory.Pop();
        boardState[lastMove] = Player.None;
        hasWon = Player.None;
        SwitchPlayer();
    }
    
    public bool IsLegalMove(int move) => GetLegalMoves().Contains(move);

    public int[] GetLegalMoves()
    {
        List<int> legal = [];
        for (int i = 0; i < boardState.Length; i++)
            if (boardState[i] == Player.None) legal.Add(i);
        return [..legal];
    }
    
    public Player GetWinner()
    {
        if (!boardState.Contains(Player.None) && hasWon == Player.None)
            return Player.Both;
        return hasWon;
    }

    private bool WinningMove(int move)
    {
        int row = move/3;
        int col = move % 3;

        if (CheckForSpan(boardState,3,3,row,col,0,1,Player.None)) return true;
        if (CheckForSpan(boardState,3,3,row,col,1,0,Player.None)) return true;
        if (CheckForSpan(boardState,3,3,row,col,1,1,Player.None)) return true;
        if (CheckForSpan(boardState,3,3,row,col,1,-1,Player.None)) return true;
        
        return false;
    }
    
    public static int ToIndex(int rowLen, int row, int col)
    {
        return (row*rowLen)+col;
    }

    private static int SpanWalk<T>(T[] arr, int y, int x, int ystep, int xstep, int rows, int cols)
    {
        var cmp = EqualityComparer<T>.Default;
        T val = arr[ToIndex(cols,y,x)];
        int i = 1;

        while (true)
        {
            int row = y+(ystep*i);
            int col = x+(xstep*i);
            if (row >= rows || row < 0) break;
            if (col >= cols || col < 0) break;

            int idx = ToIndex(cols,row,col);
            if (!cmp.Equals(arr[idx],val)) break;

            i++;
        }

        return i-1;
    }

    private static bool CheckForSpan<T>(T[] arr, int rowLen, int len, int y, int x, int ystep, int xstep, T? empty)
    {
        if (xstep < -1 || xstep > 1 || ystep < -1 || ystep > 1) return false;
        if (rowLen <= 0 || arr.Length == 0 || len <= 0) return false;
        if (xstep == 0 && ystep == 0) return false;
        if (arr.Length % rowLen != 0) return false;
        int rows = arr.Length/rowLen;
        int cols = rowLen;
        
        if (x >= cols || x < 0 || y >= rows || y < 0)
            return false;
        
        var cmp = EqualityComparer<T>.Default;
        T val = arr[ToIndex(rowLen,y,x)];
        if (empty is not null && cmp.Equals(empty,val)) return false;

        int pRun = SpanWalk(arr, y, x, ystep, xstep, rows, cols);
        xstep *= -1;
        ystep *= -1;
        int nRun = SpanWalk(arr, y, x, ystep, xstep, rows, cols);

        return nRun+1+pRun >= len;
    }
}
using TicTacToe.Core;

namespace TicTacToe.Bot;

public class GameBot
{
    private Board game;
    private Player maximizer;
    private Player minimizer;

    public GameBot(Board b, Player p)
    {
        game = b;
        maximizer = p;
        minimizer = p == Player.X ? Player.O : Player.X;
    }
    
    public int BestMove()
    {
        var best = int.MinValue;
        var chosenMove = -1;
        
        foreach (var move in game.GetLegalMoves())
        {
            game.MakeMove(move);
            var val = Minimax(false);
            game.UndoMove();

            if (val > best)
            {
                best = val;
                chosenMove = move;
            }
        }
        
        return chosenMove;
    }
    
    private int Minimax(bool isMaximizer)
    {
        var result = Evaluate(maximizer);
        
        if (result is not null)
            return result ?? 0;
        
        if (isMaximizer)
        {
            var best = int.MinValue;

            foreach (var move in game.GetLegalMoves())
            {
                game.MakeMove(move);
                var val = Minimax(false);
                game.UndoMove();

                best = Math.Max(best,val);
            }

            return best;
        }
        else
        {
            var best = int.MaxValue;

            foreach (var move in game.GetLegalMoves())
            {
                game.MakeMove(move);
                var val = Minimax(true);
                game.UndoMove();

                best = Math.Min(best,val);
            }

            return best;
        }
    }
    
    private int? Evaluate(Player maximizer)
    {
        if (game.GetWinner() == Player.None)
            return null;
        if (game.GetWinner() == Player.Both)
            return 0;

        if (game.GetWinner() == maximizer)
            return 1;
        else
            return -1;
    }
}
using System.ComponentModel;
using TicTacToe.Core;

namespace TicTacToe.Cli;

public class Terminal
{
    private readonly Board game = new();
    
    private bool running = false;

    public void Start()
    {
        running = true;
        while (running)
        {
            Clear();
            PrintGame();
            ShowPrompt();
            var ki = Console.ReadKey();
            int move = int.Parse(ki.KeyChar.ToString());
            game.MakeMove(move-1);
            
            var winner = game.GetWinner();
            if (winner != Player.None)
            {
                Clear();
                switch(winner)
                {
                    case Player.X:
                        Console.WriteLine("X has won!");
                    break;
                    case Player.O:
                        Console.WriteLine("O has won!");
                    break;
                    default:
                        Console.WriteLine("It's a draw!");
                    break;
                }
                running = false;
            }
        }
    }

    private void Clear()
    {
        Console.Clear();
    }

    private void PrintGame()
    {
        Console.WriteLine(game);
        Console.WriteLine();
    }
    
    private void ShowPrompt()
    {
        Console.Write($"({Board.GetPlayerString(game.CurrentPlayer)}) 1-9 > ");
    }
}
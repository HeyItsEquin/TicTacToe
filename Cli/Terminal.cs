using TicTacToe.Core;

namespace TicTacToe.Cli;

public class Terminal
{
    private readonly Board game = new();
    
    private bool running = false;

    private string? prevCommandResult;    

    public void Start()
    {
        running = true;
        while (running)
        {
            Clear();
            PrintGame();
            ShowCommandOutput();
            ShowPrompt();
            var line = Console.ReadLine() ?? "";
            RunCommand(line);
            
            var winner = game.GetWinner();
            if (winner != Player.None)
            {
                Stop();
                Console.WriteLine(winner switch
                {
                   Player.X => "X has won!",
                   Player.O => "O has won!",
                   _ => "It's a draw!" 
                });
            }
        }
    }

    private void RunCommand(string line)
    {
        if (line == "")
        {
            prevCommandResult = null;
            return;
        }
        var parts = line.Split(" ");
        var verb = parts[0];
        var result = CommandRegistry.TryGetCommand(verb,out var fn);
        if (!result)
        {
            prevCommandResult = $"Command Not Found: {verb}";
            return;
        }
        
        var res = fn(new CommandContext(verb,parts[1..],game));
        if (res.Result == ResultType.Exit)
            Stop();
            
        prevCommandResult = res.Message == string.Empty ? null : res.Message;
    }
    
    private void Stop()
    {
        Clear();
        running = false;
    }
    private void Clear() => Console.Clear();

    private void ShowCommandOutput()
    {
        if (prevCommandResult is null) return;

        Console.WriteLine($"{prevCommandResult}\n");
    }

    private void PrintGame()
    {
        Console.WriteLine($"{game}\n");
    }
    
    private void ShowPrompt()
    {
        Console.Write($"({Board.GetPlayerString(game.CurrentPlayer)}) > ");
    }
}
using TicTacToe.Core;
using TicTacToe.Bot;
using TicTacToe.Core.Utils;
using System.Drawing;

namespace TicTacToe.Cli;

public class Game
{
    private Board game = new();
    private Player botPlaying = Player.None;

    private bool running = false;
    private bool inGame = false;
    
    private bool shouldEndGame = false;

    private CommandResult? lastOutput;

    public void StartNewGame(bool againstBot,Player botPlayer = Player.None)
    {
        game = new();
        inGame = true;
        botPlaying = botPlayer;
    }
    
    public void RestartGame()
    {
        game = new();
    }
    
    public bool InGame() => inGame;

    public void Start()
    {
        running = true;
        while (running)
        {
            Console.Clear();
            if (inGame)
                GameRepl();
            else
                Repl();
        }
    }
    
    private void GameRepl()
    {
        ShowGameBoard();
        ShowOutput();
            
        if (shouldEndGame)
        {
            shouldEndGame = false;
            inGame = false;
            Prompt();            
        }
        else
            GamePrompt();

        if (botPlaying != Player.None && game.CurrentPlayer == botPlaying && inGame)
        {
            var bot = new GameBot(game,botPlaying);
            game.MakeMove(bot.BestMove());
        }
        else
        {
            var inp = Console.ReadLine() ?? "";
            Evaluate(inp);
        }
        
        CheckWinner();
    }

    private void Repl()
    {
        ShowOutput();
        Prompt();

        var inp = Console.ReadLine() ?? ""; 
        Evaluate(inp);
    }
    
    private void Evaluate(string line)
    {
        if (line == "")
        {
            lastOutput = null;
            return;
        }
        
        var c = CommandRegistry.FromString(line, inGame, out var args);
        if (c is null)
        {
            lastOutput = new CommandResult(ResultType.Error,$"Command Not Found: {line}");
            return;
        }

        var result = c.Execute(args,game,this); 
        if (result.Result == ResultType.Exit)
            running = false;
        lastOutput = result;
    }

    private void ShowGameBoard()
    {
        Console.WriteLine($"{game}\n");
    }

    private void ShowOutput()
    {
        if (lastOutput is null) return;
        if (lastOutput.Message == string.Empty) return;
        switch(lastOutput.Result)
        {
            case ResultType.Ok:
                Console.WriteLine($"{Text.Colored(Color.LightBlue,lastOutput.Message)}\n");
            break;
            case ResultType.Error:
                Console.WriteLine($"{Text.Colored(Color.Red,lastOutput.Message)}\n");
            break;
            default:
                return;
        }
    }

    private void Prompt() => Console.Write("> ");
    private void GamePrompt() => Console.Write($"({game.CurrentPlayer}) > ");

    public void EndGame()
    {
        shouldEndGame = true;
    }

    private void CheckWinner()
    {
        var winner = game.GetWinner();
        if (winner != Player.None && inGame)
        {
            var winStr = winner switch
            {
                Player.X => "X has won!",
                Player.O => "O has won!",
                _ => "It's a draw!"  
            };

            lastOutput = new CommandResult(ResultType.Ok,winStr);
            EndGame();
        }
    }
}

public class Terminal
{
    private readonly Board game = new();
    private Player human = Player.None;

    private bool running = false;

    private string? prevCommandResult;    

    public void StartAgainstBot(bool startFirst)
    {
        if (startFirst)
            human = Player.X;
        else
            human = Player.O;
        running = true;
        while (running)
        {
            if (game.CurrentPlayer == human)
            {
                PlayerInput();
            }
            else
            {
                var bot = new GameBot(game,game.CurrentPlayer);
                game.MakeMove(bot.BestMove());
            }
            CheckWinner();
        }
    }

    public void Start()
    {
        running = true;
        while (running)
        {
            PlayerInput();
            CheckWinner();
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
        var result = CommandRegistry.TryGetCommand(verb,out var command);
        if (!result)
        {
            prevCommandResult = $"Command Not Found: {verb}";
            return;
        }

        var res = command!.Execute(parts[1..],game);
        if (res.Result == ResultType.Exit)
            running = false;
        prevCommandResult = res.Message == string.Empty ? null : res.Message;
    }
    
    private void PlayerInput()
    {
        Clear();
        PrintGame();
        ShowCommandOutput();
        ShowPrompt();
        var line = Console.ReadLine() ?? "";
        RunCommand(line);
    }
    
    private void CheckWinner()
    {
        var winner = game.GetWinner();
        if (winner != Player.None)
        {
            Stop();
            PrintGame();
            Console.WriteLine(winner switch
            {
                Player.X => "X has won!",
                Player.O => "O has won!",
                _ => "It's a draw!"
            });
            Console.ReadKey(true);
        }
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
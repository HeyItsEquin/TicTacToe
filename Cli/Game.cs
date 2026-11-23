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
        }

        if (botPlaying != Player.None && game.CurrentPlayer == botPlaying && inGame)
        {
            var bot = new GameBot(game,botPlaying);
            game.MakeMove(bot.BestMove());
        }
        else
        {
            var inp = new Prompt($"({Board.GetPlayerString(game.CurrentPlayer)}) > ").GetInput();
            Evaluate(inp);
        }
        
        CheckWinner();
    }

    private void Repl()
    {
        ShowOutput();
        var inp = new Prompt("> ").GetInput(); 
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

    public void EndGame() => shouldEndGame = true;

    private void CheckWinner()
    {
        var winner = game.GetWinner();
        if (winner != Player.None && inGame)
        {
            var winStr = winner switch
            {
                Player.X => "Winner: X!",
                Player.O => "Winner: O!",
                _ => "Winner: Both (Draw)!"  
            };

            lastOutput = new CommandResult(ResultType.Ok,winStr);
            EndGame();
        }
    }
}
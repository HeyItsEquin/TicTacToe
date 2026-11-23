using TicTacToe.Bot;
using TicTacToe.Core;

internal class Commands
{
    private static CommandResult Ok(string msg = "")
        => new(ResultType.Ok,msg);
    private static CommandResult Error(string msg = "")
        => new(ResultType.Error,msg);
    private static CommandResult Exit(string msg = "")
        => new(ResultType.Exit,msg);

    public static CommandResult StartCommand(CommandContext c)
        => Error("Already in-game");
    
    public static CommandResult RestartCommand(CommandContext c)
    {
        if (c.GameCtx is null)
            return Error($"Command '{c.Caller.Verb}': CommandContext.GameCtx is null");
        
        c.GameCtx.RestartGame();
        return Ok();
    }

    public static CommandResult MoveCommand(CommandContext c)
    {
        if (c.Arguments.Count == 0)
            return Error(c.Caller.Usage);
        
        if (c.Arguments.Count == 1)
        {
            if (!int.TryParse(c.Arguments[0], out var move))
                return Error($"Argument <index> is not a valid move: '{c.Arguments[0]}'");

            if (move < 1 || move > 9)
                return Error($"Argument <index> out of range (1-9): {move}");

            move--;
            if (!c.Game.MakeMove(move))
                return Error($"Illegal Move: {move+1}");
        }

        if (c.Arguments.Count == 2)
        {
            if (!int.TryParse(c.Arguments[0], out var x))
                return Error($"Argument <x> is not a valid move: '{c.Arguments[0]}'");
            if (!int.TryParse(c.Arguments[1], out var y))
                return Error($"Argument <y> is not a valid move: '{c.Arguments[1]}'");
            
            string outOfRangeMsg = "";
            if (x < 1 || x > 3)
                outOfRangeMsg += $"Argument <x> out of range (1-3): {x}\n";
            if (y < 1 || y > 3)
                outOfRangeMsg += $"Argument <y> out of range (1-3): {y}\n";
            if (outOfRangeMsg != string.Empty)
                return Error(outOfRangeMsg.Trim());
            
            var idx = Board.ToIndex(3,--y,--x);
            if (!c.Game.MakeMove(idx))
                return Error($"Illegal Move: {x+1},{y+1}");
        }        
        
        return Ok();
    }
    
    public static CommandResult BestMove(CommandContext c)
    {
        var bot = new GameBot(c.Game,c.Game.CurrentPlayer);
        var move = bot.BestMove();
        if (move < 0)
            return Error("No legal moves remaining");
        return Ok($"Best Move: {move+1}");
    }

    public static CommandResult HelpCommand(CommandContext c)
    {
        List<(string usage,string help)> messages = [];
        var longest = 0;
        foreach (var command in CommandRegistry.GetCommandList())
        {
            var usage = command.Usage;
            var help = command.Help;
            if (usage.Length > longest)
                longest = usage.Length;
            messages.Add((command.Usage, command.Help));
        }

        var msg = "Commands:\n";
        foreach (var (usage,help) in messages)
            msg += $"{usage.PadRight(longest,' ')} - {help}\n";

        return Ok(msg.Trim());
    }
    
    public static CommandResult ExitCommand(CommandContext c)
    {
        return Exit();
    }
}

internal class NonGameCommands
{
    private static CommandResult Ok(string msg = "")
        => new(ResultType.Ok,msg);
    private static CommandResult Error(string msg = "")
        => new(ResultType.Error,msg);
    private static CommandResult Exit(string msg = "")
        => new(ResultType.Exit,msg);
    
    public static CommandResult StartCommand(CommandContext c)
    {
        if (c.GameCtx is null)
            return Error($"Command '{c.Caller.Verb}': CommandContext.GameCtx is null");
        
        if (c.Arguments.Count == 0)
            c.GameCtx.StartNewGame(false);

        if (c.Arguments.Count == 2)
        {
            var player = c.Arguments[1].ToLowerInvariant();
            if (player != "x" && player != "o")
                return Error($"Argument <which> is not a valid player: '{c.Arguments[1]}'");
            var botPlaying = player == "x" ? Player.X : Player.O;
            c.GameCtx.StartNewGame(true,botPlaying);
        }

        return Ok();
    }
    
    public static CommandResult RestartCommand(CommandContext c)
        => Error("Not currently in-game");
    public static CommandResult MoveCommand(CommandContext c)
        => Error("Not currently in-game");
    public static CommandResult BestMove(CommandContext c)
        => Error("Not currenly in-game");
    
    public static CommandResult HelpCommand(CommandContext c)
    {
        List<(string usage,string help)> messages = [];
        var longest = 0;
        foreach (var command in CommandRegistry.GetNonGameCommandList())
        {
            var usage = command.Usage;
            var help = command.Help;
            if (usage.Length > longest)
                longest = usage.Length;
            messages.Add((command.Usage, command.Help));
        }

        var msg = "Commands:\n";
        foreach (var (usage,help) in messages)
            msg += $"{usage.PadRight(longest,' ')} - {help}\n";

        return Ok(msg.Trim());
    }

    public static CommandResult ExitCommand(CommandContext c)
        => Exit();
}

public static class CommandRegistry
{
    private static readonly List<Command> commands =
    [        
        new Command(
            "start",
            ["s"],
            "'[s/start] bot <which>' | '[s/start]'",
            "(While not in-game) Start a new game either against a player or against a bot playing either X or O",
            Commands.StartCommand
        ),
        new Command(
            "restart",
            ["r","re"],
            "'[r/re/restart]'",
            "(While in-game) Start a new game with the current game's settings",
            Commands.RestartCommand
        ),
        new Command(
            "move",
            ["m"],
            "'[m/move] <index>' | '[m/move] <x> <y>'",
            "(While in-game) Attempt to play the specified move",
            Commands.MoveCommand
        ),
        new Command(
            "best",
            ["b"],
            "'[b/best]'",
            "(While in-game) Outputs the optimal next move",
            Commands.BestMove
        ),
        new Command(
            "help",
            ["?"],
            "'[?/help]'",
            "Shows this dialog",
            Commands.HelpCommand
        ),
        new Command(
            "quit",
            ["exit","x","q"],
            "'[quit/q/exit/x]'",
            "Quits the application",
            Commands.ExitCommand
        ),
    ];
    
    private static readonly List<Command> nonGameCommands =
    [
        new Command(
            "start",
            ["s"],
            "'[s/start] bot <which>' | '[s/start]'",
            "(While not in-game) Start a new game either against a player or against a bot playing either X or O",
            NonGameCommands.StartCommand
        ),
        new Command(
            "restart",
            ["r","re"],
            "'[r/re/restart]'",
            "(While in-game) Start a new game with the current game's settings",
            NonGameCommands.RestartCommand
        ),
        new Command(
            "move",
            ["m"],
            "'[m/move] <index>' | '[m/move] <x> <y>'",
            "(While in-game) Attempt to play the specified move",
            NonGameCommands.MoveCommand
        ),
        new Command(
            "best",
            ["b"],
            "'[b/best]'",
            "(While in-game) Outputs the optimal next move",
            NonGameCommands.BestMove
        ),
        new Command(
            "help",
            ["?"],
            "'[?/help]'",
            "Shows this dialog",
            NonGameCommands.HelpCommand
        ),
        new Command(
            "quit",
            ["exit","x","q"],
            "'[quit/q/exit/x]'",
            "Quits the application",
            NonGameCommands.ExitCommand
        )
    ];
    
    internal static List<Command> GetNonGameCommandList() => nonGameCommands;
    internal static List<Command> GetCommandList() => commands;

    public static Command? FromString(string input, bool inGame, out string[] args)
    {
        args = [];
        if (input == "") return null;
        var parts = input.Split(" ");
        var verb = parts[0];

        args = parts[1..];
        Command? command;
        if (inGame)
            TryGetCommand(verb,out command);
        else
            TryGetCommandNotInGame(verb,out command);
        return command;
    }
    
    public static bool TryGetCommand(string command, out Command? c)
    {
        c = commands.Find((co) => command == co.Verb || co.Aliases.Contains(command));
        return c is not null; 
    }
    
    public static bool TryGetCommandNotInGame(string command, out Command? c)
    {
        c = nonGameCommands.Find((co) => command == co.Verb || co.Aliases.Contains(command));
        return c is not null;
    }
}
using TicTacToe.Bot;
using TicTacToe.Core;

public enum ResultType
{
    Ok,
    Error,
    Exit
}
public class CommandResult(ResultType res, string msg)
{
    public ResultType Result { get; private set; } = res;
    public string Message { get; private set; } = msg;
}
public class CommandContext
{
    public string Verb { get; private set; }
    public List<string> Arguments { get; private set; }
    public Board Game { get; private set; }

    public CommandContext(string verb, IEnumerable<string> args, Board game)
    {
        Verb = verb;
        Game = game;

        Arguments = [];
        foreach (var arg in args)
            Arguments.Add(arg.Trim());
    }
}

internal class Commands
{
    public static CommandResult MoveCommand(CommandContext c)
    {
        if (c.Arguments.Count == 0)
            return new CommandResult(ResultType.Error,"Usage: 'move <index>' | 'move <x> <y>'");
        
        if (c.Arguments.Count == 1)
        {
            if (!int.TryParse(c.Arguments[0], out var move))
                return new CommandResult(ResultType.Error,$"Argument <index> is not a valid move: '{c.Arguments[0]}'");

            if (move < 1 || move > 9)
                return new CommandResult(ResultType.Error,$"Argument <index> out of range (1-9): {move}");

            move--;
            if (!c.Game.MakeMove(move))
                return new CommandResult(ResultType.Error,$"Illegal Move: {move+1}");
        }

        if (c.Arguments.Count == 2)
        {
            if (!int.TryParse(c.Arguments[0], out var x))
                return new CommandResult(ResultType.Error,$"Argument <x> is not a valid move: '{c.Arguments[0]}'");
            if (!int.TryParse(c.Arguments[1], out var y))
                return new CommandResult(ResultType.Error,$"Argument <y> is not a valid move: '{c.Arguments[1]}'");
            
            string outOfRangeMsg = "";
            if (x < 1 || x > 3)
                outOfRangeMsg += $"Argument <x> out of range (1-3): {x}\n";
            if (y < 1 || y > 3)
                outOfRangeMsg += $"Argument <y> out of range (1-3): {y}\n";
            if (outOfRangeMsg != string.Empty)
                return new CommandResult(ResultType.Error,outOfRangeMsg.Trim());
            
            var idx = Board.ToIndex(3,--y,--x);
            if (!c.Game.MakeMove(idx))
                return new CommandResult(ResultType.Error,$"Illegal Move: {x+1},{y+1}");
        }        

        return new CommandResult(ResultType.Ok,"");
    }
    
    public static CommandResult BestMove(CommandContext c)
    {
        var bot = new GameBot(c.Game,c.Game.CurrentPlayer);
        var move = bot.BestMove();
        if (move < 0)
            return new CommandResult(ResultType.Error,"No legal moves remaining");
        return new CommandResult(ResultType.Ok,$"Best Move: {move+1}");
    }

    public static CommandResult HelpCommand(CommandContext c)
    {
        string message = 
            "Commands:\n" +
            "    'move <index>' | 'move <x> <y>' - Attempt to play at the specified board position\n" +
            "    'best'                          - Returns the best move that can currently be played\n" +
            "    'help'                              - Show this message\n" +
            "    'quit' | 'exit'                     - Exit the game";
        
        return new CommandResult(ResultType.Error,message);
    }
    
    public static CommandResult ExitCommand(CommandContext c)
    {
        return new CommandResult(ResultType.Exit,"");
    }
}

public static class CommandRegistry
{
    private static Dictionary<string,Func<CommandContext,CommandResult>> commands = new()
    {
        ["move"] = Commands.MoveCommand,
        ["best"] = Commands.BestMove,
        ["help"] = Commands.HelpCommand,
        ["quit"] = Commands.ExitCommand,
        ["exit"] = Commands.ExitCommand
    };
    
    public static bool TryGetCommand(string command, out Func<CommandContext,CommandResult> fn)
        => commands.TryGetValue(command.ToLowerInvariant(),out fn!);
}
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
            return new CommandResult(ResultType.Error,"Usage: 'move <index>' | 'move <row> <col>'");            
        
        if (c.Arguments.Count == 1)
        {
            if (!int.TryParse(c.Arguments[0], out var move))
                return new CommandResult(ResultType.Error,$"Argument <index> is not a valid move: '{c.Arguments[0]}'");

            if (move < 1 || move > 9)
                return new CommandResult(ResultType.Error,$"Argument <index> out of range (1-9): {move}");

            move--;
            if (!c.Game.MakeMove(move))
                return new CommandResult(ResultType.Error,$"Illegal Move: {move}");
        }
        
        if (c.Arguments.Count == 2)
        {
            if (!int.TryParse(c.Arguments[0], out var row))
                return new CommandResult(ResultType.Error,$"Argument <row> is not a valid row: '{c.Arguments[0]}'");
            
            if (!int.TryParse(c.Arguments[1], out var col))
                return new CommandResult(ResultType.Error,$"Argument <col> is not a valid column: '{c.Arguments[1]}'");
            
            string outOfRangeMsg = "";
            if (row < 1 || row > 3) outOfRangeMsg += $"Argument <row> out of range (1-3): {row}\n";
            if (col < 1 || col > 3) outOfRangeMsg += $"Argument <col> out of range (1-3): {col}";

            if (outOfRangeMsg != string.Empty)
                return new CommandResult(ResultType.Error,outOfRangeMsg.Trim());
            
            var idx = Board.ToIndex(3,--row,--col);
            if (!c.Game.MakeMove(idx))
                return new CommandResult(ResultType.Error,$"Illegal Move: {row},{col}");
        }

        return new CommandResult(ResultType.Ok,"");
    }
    
    public static CommandResult HelpCommand(CommandContext c)
    {
        string message = 
            "Commands:\n" +
            "    'move <index>' | 'move <row> <col>' - Attempt to play at the specified board position\n" +
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
        ["help"] = Commands.HelpCommand,
        ["quit"] = Commands.ExitCommand,
        ["exit"] = Commands.ExitCommand
    };
    
    public static bool TryGetCommand(string command, out Func<CommandContext,CommandResult> fn)
        => commands.TryGetValue(command.ToLowerInvariant(),out fn);
}
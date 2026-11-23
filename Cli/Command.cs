using TicTacToe.Cli;
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
    public Command Caller { get; private set; }
    public List<string> Arguments { get; private set; }
    public Board Game { get; private set; }
    public Game? GameCtx { get; private set; }

    public CommandContext(Command c, IEnumerable<string> args, Board game, Game? ctx = null)
    {
        Caller = c;
        Game = game;
        GameCtx = ctx;

        Arguments = [];
        foreach (var arg in args)
            Arguments.Add(arg.Trim());
    }
}

public class Command
{
    public string Verb { get; private set; }
    public List<string> Aliases { get; private set; }
    public string Usage { get; private set; }
    public string Help { get; private set; }
    public CommandContext? Context { get; private set; }
    
    private Func<CommandContext,CommandResult> callback;

    public Command(string verb, IEnumerable<string> aliases, string usage, string help, Func<CommandContext,CommandResult> cb)
    {
        Verb = verb;
        
        Aliases = [];
        foreach (var alias in aliases)
            Aliases.Add(alias);
        
        Usage = usage;
        Help = help;
        
        callback = cb;
    }
    
    public CommandResult Execute(IEnumerable<string> args, Board game, Game? ctx = null)
    {
        return callback(new CommandContext(this,args,game,ctx));
    }
}

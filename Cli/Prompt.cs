using System.Drawing;
using Pastel;
using TicTacToe.Core.Utils;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace TicTacToe.Cli;

public class Prompt
{
    private readonly string _prompt;
    
    public Prompt(string prompt)
    {
        _prompt = prompt;
    }
    
    public string GetInput()
    {
        string buffer = "";

        int startRow = Console.CursorTop;
        int startColumn = Console.CursorLeft;
        
        while (true)
        {
            Redraw(startRow,startColumn,buffer);

            var ki = Console.ReadKey(true);

            switch (ki.Key)
            {
                case ConsoleKey.Enter:
                    Console.WriteLine(); return buffer;
                case ConsoleKey.Backspace:
                    if (buffer.Length > 0)
                        buffer = buffer[..^1];
                    break;
                default:
                    if (!char.IsControl(ki.KeyChar))
                        buffer += ki.KeyChar;
                    break;
            }
        }
    }
    
    private void Redraw(int row, int column, string buffer)
    {
        Console.SetCursorPosition(column, row);

        Console.Write("\x1b[2K");

        Console.Write(_prompt);
        WriteHighlighted(buffer);

        int cursorColumn = Console.CursorLeft;

        if (cursorColumn >= Console.WindowWidth)
            cursorColumn = Console.WindowWidth-1;
        
        Console.SetCursorPosition(cursorColumn,row);
    }

    private static void WriteHighlighted(string input)
    {
        foreach (var part in Highlight(input))
        {
            Console.Write(part.Text.Pastel(part.Color));
        }
    }
    
    private static IEnumerable<(string Text, Color Color)> Highlight(string input)
    {
        var parts = Regex.Split(input, @"(\s+)");
        bool firstWord = true;

        foreach (var part in parts)
        {
            if (part.Length == 0) continue;

            if (char.IsWhiteSpace(part[0]))
            {
                yield return (part, Color.White);
                continue;
            }

            if (firstWord)
            {
                firstWord = false;
                if (!CommandRegistry.IsValidCommand(part))
                    yield return (part.Bold(), Color.PaleVioletRed);
                else
                    yield return (part, Color.OliveDrab);
            }
            else
            {
                Color c = GetArgType(part) switch
                {
                    ArgType.Number => Color.White,
                    ArgType.Player => Color.Goldenrod,
                    _ => Color.Tan
                };
                yield return (part, c);
            }
        }
    }
    
    private static ArgType GetArgType(string arg)
    {
        arg = arg.Trim();
        if (int.TryParse(arg, out var _))
            return ArgType.Number;
        if (arg.ToLowerInvariant() == "x" || arg.ToLowerInvariant() == "o")
            return ArgType.Player;
        return ArgType.None;
    }
        
    private enum ArgType
    {
        None,
        Number,
        Player,
    }
}
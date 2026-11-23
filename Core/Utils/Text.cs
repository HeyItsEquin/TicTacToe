using System.Drawing;
using Pastel;

namespace TicTacToe.Core.Utils;

public static class Text
{
    public static string Colored(Color color, string text)
        => text.Pastel(color);
    
    public static string Bold(this string text)
        => $"\x1b[1m{text}\x1b[0m";
}
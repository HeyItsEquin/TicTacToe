# TicTacToe
TicTacToe CLI App

## Why?
I have a friend whose currently working on a game, a game involving chess. Now, he wants his players to be able to play against a bot. Sounds simple enough, right?
No. Turns out, the initial choice (Stockfish) can't actually play at a level low enough to give his players a chance. So we both looked around, and it seems there just isn't any good solution.
I then decided to try and make my own, but I know nothing about how chess bots worked, so I decided to do a small project to try and get down some of the fundamentals. And thus, this was born.
A TicTacToe CLI game where you can play against a friend or against a bot, a bot that will always win.

## Requirements
  - You must have the .NET framework 10.0 installed on your system
  - For the app to work properly, it has to be run in a terminal emulator that supports 16-bit true color (this is PowerShell on Win10, Windows Terminal on Win11, or basically any terminal emulator on Linux)

## How To Play
Download and unzip the latest release and run the executable (`TicTacToe` on Linux, or `TicTacToe.exe` on Windows)
You will enter into a CLI environment, you can type `?` or `help` to see any available commands, and you should be able to figure out where to go from there.

## Building from source
```
# Clone the repo
git clone https://github.com/HeyItsEquin/TicTacToe.git && cd TicTacToe

# Build
dotnet build
# OR
dotnet run
```

## Technologies
  - Built with .NET Framework 10.0/C#

## Third-Party Libraries
This project uses [Pastel](https://github.com/adamralph/pastel),
a .NET library for colored console output, licensed under the
[MIT License](https://github.com/adamralph/pastel/blob/master/LICENSE).

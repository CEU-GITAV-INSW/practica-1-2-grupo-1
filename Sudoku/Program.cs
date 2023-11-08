//#define DebugAlgorithm // <- uncomment to watch the generation algorithm

using System;
using System.Diagnostics;                          
using System.Threading;	
using Towel;
using System.Media;
using System.IO;

bool closeRequested = false;
int?[,] generatedBoard = null;
int?[,] activeBoard = null;
Random random = new Random(); // Se agrega la declaración de Random
bool paused = false; // Agregado para indicar si el temporizador está pausado
SoundPlayer musica = new SoundPlayer("resources/musica.wav");

musica.PlayLooping();


while (!closeRequested)
{
NewPuzzle:
	
	Console.Clear();

    bool validInput = false;
    int maxBlanks = 80; // Todas las casillas menos 1
    int selectedBlanks = maxBlanks;

    while (!validInput)
    {
        Console.Clear();
        Console.WriteLine("Sudoku");
        Console.WriteLine();
		Console.WriteLine("Press 'R' for a random number of initially filled cells, or");
        Console.WriteLine("Choose the number of initially filled cells (0 to " + maxBlanks + "): ");

		string input = "";
        ConsoleKeyInfo keyInfo;

		do
    {
        keyInfo = Console.ReadKey(true);

        if (keyInfo.Key == ConsoleKey.R)
        {
            selectedBlanks = random.Next(0, maxBlanks + 1);
            validInput = true;
        }
        else if (keyInfo.Key == ConsoleKey.Enter)
        {
            if (int.TryParse(input, out selectedBlanks) && selectedBlanks >= 0 && selectedBlanks <= maxBlanks)
            {
                validInput = true;
            }
            else
            {
                Console.WriteLine("\nInvalid input. Please enter a number between 0 and " + maxBlanks + ".");
                input = "";
            }
        }
        else if (char.IsDigit(keyInfo.KeyChar))
        {
            Console.Write(keyInfo.KeyChar);
            input += keyInfo.KeyChar;
        }
        else if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Delete)
        {
            Console.WriteLine("\nInvalid input. Please enter a number between 0 and " + maxBlanks + ".");
            input = "";
        }
    } while (!validInput);

	Console.Clear();

	// Obtener el tiempo deseado del usuario
    int minutes, seconds;
    do
    {
        Console.WriteLine("Enter the desired time to solve the sudoku (in minutes and seconds):");
        Console.Write("Minutes: ");
    } while (!int.TryParse(Console.ReadLine(), out minutes));

    do
    {
        Console.Write("Seconds: ");
    } while (!int.TryParse(Console.ReadLine(), out seconds));

    // Iniciar el temporizador
    Stopwatch timer = new Stopwatch();
	Thread timerThread = new Thread(() =>
      {
        timer.Start();
        while (minutes > 0 || seconds > 0)
        {
            Thread.Sleep(1000); // Esperar 1 segundo
            if (!closeRequested && seconds == 0)
            {
                minutes--;
                seconds = 59;
            }
            else if (!closeRequested && !paused)
            {
                seconds--;
            }
        }
    });
    timerThread.Start();


	generatedBoard = Sudoku.Generate(random, 81 - selectedBlanks);
	activeBoard = new int?[9, 9];

     for (int i = 0; i < 9; i++)
    {
        for (int j = 0; j < 9; j++)
        {
            if (generatedBoard[i, j].HasValue)
            {
                activeBoard[i, j] = generatedBoard[i, j];
            }
        }
    }

	int x = 0;
	int y = 0;

	Console.Clear();

	while (!closeRequested && ContainsNulls(activeBoard) && (minutes > 0 || seconds > 0))
	{
		Console.SetCursorPosition(0, 0);
		Console.WriteLine("Sudoku");
		Console.WriteLine();
		ConsoleWrite(activeBoard, generatedBoard);
		Console.WriteLine();
		Console.WriteLine($"Remaining Time: {TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds)}{(paused ? " (Paused)" : " (Tic Tac)")}");
		Console.WriteLine("Press arrow keys to select a cell.");
		Console.WriteLine("Press 1-9 to insert values.");
		Console.WriteLine("Press [delete] or [backspace] to remove.");
		Console.WriteLine("Press [escape] to exit.");
		Console.WriteLine("Press [end] to generate a new sudoku.");
		Console.WriteLine($"Press [P] to {(paused ? "resume" : "pause")} the timer."); 

		Console.SetCursorPosition(y * 2 + 2 + (y / 3 * 2), x + 3 + +(x / 3));


		ConsoleKeyInfo key = Console.ReadKey(true);
		switch (key.Key)
		{
			case ConsoleKey.P:                                                                  
                paused = !paused;																
                Console.WriteLine(paused ? "Game Paused" : "Game Resumed");					
                break;
			case ConsoleKey.UpArrow: x = x <= 0 ? 8 : x - 1; break;
			case ConsoleKey.DownArrow: x = x >= 8 ? 0 : x + 1; break;
			case ConsoleKey.LeftArrow: y = y <= 0 ? 8 : y - 1; break;
			case ConsoleKey.RightArrow: y = y >= 8 ? 0 : y + 1; break;

			case ConsoleKey.D1: case ConsoleKey.NumPad1: activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 1, x, y) ? 1 : activeBoard[x, y]; break;
			case ConsoleKey.D2: case ConsoleKey.NumPad2: activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 2, x, y) ? 2 : activeBoard[x, y]; break;
			case ConsoleKey.D3: case ConsoleKey.NumPad3: activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 3, x, y) ? 3 : activeBoard[x, y]; break;
			case ConsoleKey.D4: case ConsoleKey.NumPad4: activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 4, x, y) ? 4 : activeBoard[x, y]; break;
			case ConsoleKey.D5: case ConsoleKey.NumPad5: activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 5, x, y) ? 5 : activeBoard[x, y]; break;
			case ConsoleKey.D6: case ConsoleKey.NumPad6: activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 6, x, y) ? 6 : activeBoard[x, y]; break;
			case ConsoleKey.D7: case ConsoleKey.NumPad7: activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 7, x, y) ? 7 : activeBoard[x, y]; break;
			case ConsoleKey.D8: case ConsoleKey.NumPad8: activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 8, x, y) ? 8 : activeBoard[x, y]; break;
			case ConsoleKey.D9: case ConsoleKey.NumPad9: activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 9, x, y) ? 9 : activeBoard[x, y]; break;

			case ConsoleKey.End: goto NewPuzzle;
			case ConsoleKey.Backspace: case ConsoleKey.Delete: activeBoard[x, y] = generatedBoard[x, y] ?? null; break;
			case ConsoleKey.Escape: closeRequested = true; break;
		}
		if (paused)                  
   		 {
         	continue; // Salta al siguiente ciclo sin actualizar el temporizador
    	 }

	 	 // Actualizar el temporizador solo si no está pausado
   		 if (timer.Elapsed.TotalSeconds >= 1)
    	 {
       	 	seconds--;
         	timer.Restart();
    	 }
	}
	 // Detener el temporizador
    timerThread.Join();
	

	if (!closeRequested)
	{
		Console.Clear();
		Console.WriteLine("Sudoku");
		Console.WriteLine();
		ConsoleWrite(activeBoard, generatedBoard);
		Console.WriteLine();
		Console.WriteLine((minutes == 0 && seconds == 0) ? "Time's up!" : "You Win!"); 
		Console.WriteLine($"Time Elapsed: {TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds)}");  
		Console.WriteLine();
		Console.WriteLine("Play Again [enter], or quit [escape]?");
	GetInput:
		switch (Console.ReadKey(true).Key)
		{
			case ConsoleKey.Enter: break;
			case ConsoleKey.Escape:
				closeRequested = true;
				Console.Clear();
				break;
			default: goto GetInput;
		}
	}
}
}
Console.Clear();
Console.Write("Sudoku was closed.");

bool IsValidMove(int?[,] board, int?[,] lockedBoard, int value, int x, int y)
{
	// Locked
	if (lockedBoard[x, y] is not null)
	{
		return false;
	}
	// Square
	for (int i = x - x % 3; i <= x - x % 3 + 2; i++)
	{
		for (int j = y - y % 3; j <= y - y % 3 + 2; j++)
		{
			if (board[i, j] == value)
			{
				return false;
			}
		}
	}
	// Row
	for (int i = 0; i < 9; i++)
	{
		if (board[x, i] == value)
		{
			return false;
		}
	}
	// Column
	for (int i = 0; i < 9; i++)
	{
		if (board[i, y] == value)
		{
			return false;
		}
	}
	return true;
}

bool ContainsNulls(int?[,] board)
{
	for (int i = 0; i < 9; i++)
	{
		for (int j = 0; j < 9; j++)
		{
			if (board[i, j] is null)
			{
				return true;
			}
		}
	}
	return false;
}

void ConsoleWrite(int?[,] board, int?[,] lockedBoard)
{
	ConsoleColor consoleColor = Console.ForegroundColor;
	Console.ForegroundColor = ConsoleColor.DarkGray;
	Console.WriteLine("╔═══════╦═══════╦═══════╗");
	for (int i = 0; i < 9; i++)
	{
		Console.Write("║ ");
		for (int j = 0; j < 9; j++)
		{
			if (lockedBoard is not null && lockedBoard[i, j] is not null)
			{
				Console.Write((lockedBoard[i, j].HasValue ? lockedBoard[i, j].ToString() : "■") + " ");
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write((board[i, j].HasValue ? board[i, j].ToString() : "■") + " ");
				Console.ForegroundColor = ConsoleColor.DarkGray;
			}
			if (j == 2 || j == 5)
			{
				Console.Write("║ ");
			}
		}
		Console.WriteLine("║");
		if (i == 2 || i == 5)
		{
			Console.WriteLine("╠═══════╬═══════╬═══════╣");
		}
	}
	Console.WriteLine("╚═══════╩═══════╩═══════╝");
	Console.ForegroundColor = consoleColor;
}

public static class Sudoku
{
	public static int?[,] Generate(
		Random? random = null,
		int? blanks = null)
	{
		random ??= new Random();
		if (blanks.HasValue && blanks < 0 || 81 < blanks)
		{
			throw new ArgumentOutOfRangeException(nameof(blanks), blanks.Value, $"{nameof(blanks)} < 0 || 81 < {nameof(blanks)}");
		}
		else if (!blanks.HasValue)
		{
			blanks = random.Next(0, 82);
		}

		int?[,] board = new int?[9, 9];
		(int[] Values, int Count)[,] valids = new (int[] Values, int Count)[9, 9];
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				valids[i, j] = (new int[9], -1);
			}
		}

		#region GetValidValues

		void GetValidValues(int row, int column)
		{
			bool SquareValid(int value, int row, int column)
			{
				for (int i = row - row % 3; i <= row; i++)
				{
					for (int j = column - column % 3; j <= column - column % 3 + 2 && !(i == row && j == column); j++)
					{
						if (board[i, j] == value)
						{
							return false;
						}
					}
				}
				return true;
			}

			bool RowValid(int value, int row, int column)
			{
				for (int i = 0; i < column; i++)
				{
					if (board[row, i] == value)
					{
						return false;
					}
				}
				return true;
			}

			bool ColumnValid(int value, int row, int column)
			{
				for (int i = 0; i < row; i++)
				{
					if (board[i, column] == value)
					{
						return false;
					}
				}
				return true;
			}

			valids[row, column].Count = 0;
			for (int i = 1; i <= 9; i++)
			{
				if (SquareValid(i, row, column) &&
					RowValid(i, row, column) &&
					ColumnValid(i, row, column))
				{
					valids[row, column].Values[valids[row, column].Count++] = i;
				}
			}
		}

		#endregion


		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				GetValidValues(i, j);
				while (valids[i, j].Count == 0)
				{
					board[i, j] = null;
					i = j == 0 ? i - 1 : i;
					j = j == 0 ? 8 : j - 1;
#if DebugAlgorithm
					Console.SetCursorPosition(0, 0);
					Program.ConsoleWrite(board, null);
					Console.WriteLine("Press [Enter] To Continue...");
					Console.ReadLine();
#endif
				}
				int index = random.Next(0, valids[i, j].Count);
				int value = valids[i, j].Values[index];
				valids[i, j].Values[index] = valids[i, j].Values[valids[i, j].Count - 1];
				valids[i, j].Count--;
				board[i, j] = value;
#if DebugAlgorithm
				Console.SetCursorPosition(0, 0);
				Program.ConsoleWrite(board, null);
				Console.WriteLine("Press [Enter] To Continue...");
				Console.ReadLine();
#endif
			}
		}

		foreach (int i in random.NextUnique(Math.Max(1, blanks.Value), 0, 81))
		{
			int row = i / 9;
			int column = i % 9;
			board[row, column] = null;
		}

		return board;
	}
}
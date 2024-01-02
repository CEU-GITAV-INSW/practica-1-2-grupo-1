﻿
//#define DebugAlgorithm // <- uncomment to watch the generation algorithm

using System;
using System.Diagnostics;
using System.Threading;
using Towel;
using System.Collections.Generic;

bool closeRequested = false;
bool goMenuPrincipal = false;
int?[,] generatedBoard = null;
int?[,] activeBoard = null;
Random random = new Random(); // Se agrega la declaración de Random
int[] puntuaciones = new int[7];

bool paused = false; // Agregado para indicar si el temporizador está pausado

string musicFilePath = "resources/musica.wav";
MusicManager musicManager = new MusicManager(musicFilePath, true); //second parameter sets true to "play in loop"
bool muteMusic = false;

int color = 1;

// Variables de control
bool enMenuPrincipal = true;
bool enJuego = false;
bool enConfiguracion = false;

// Variables 
int x = 0; // Fila actual
int y = 0; // Columna actual

//Variables de configuración
int n_color = 0;
bool IsMusicMuted = false;

void handleInput()
        {
            if (enMenuPrincipal)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.NumPad1:
                    case ConsoleKey.D1:
                        menuConfiguracion(isMusicMuted, n_color);
                        break;
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.D2:
                        iniciarPartida();
                        break;
                    case ConsoleKey.NumPad3:
                    case ConsoleKey.D3:
                        mostrarRanking();
                        break;
                }
            }
            else if (enJuego)
            {
                
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        x = x <= 0 ? 8 : x - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        x = x >= 8 ? 0 : x + 1;
                        break;
                    case ConsoleKey.LeftArrow:
                        y = y <= 0 ? 8 : y - 1;
                        break;
                    case ConsoleKey.RightArrow:
                        y = y >= 8 ? 0 : y + 1;
                        break;
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 1, x, y) ? 1 : activeBoard[x, y];
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 2, x, y) ? 2 : activeBoard[x, y];
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 3, x, y) ? 3 : activeBoard[x, y];
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 4, x, y) ? 4 : activeBoard[x, y];
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 5, x, y) ? 5 : activeBoard[x, y];
                        break;
                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 6, x, y) ? 6 : activeBoard[x, y];
                        break;
                    case ConsoleKey.D7:
                    case ConsoleKey.NumPad7:
                        activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 7, x, y) ? 7 : activeBoard[x, y];
                        break;
                    case ConsoleKey.D8:
                    case ConsoleKey.NumPad8:
                        activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 8, x, y) ? 8 : activeBoard[x, y];
                        break;
                    case ConsoleKey.D9:
                    case ConsoleKey.NumPad9:
                        activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 9, x, y) ? 9 : activeBoard[x, y];
                        break;
                    case ConsoleKey.Backspace:
                    case ConsoleKey.Delete:
                        activeBoard[x, y] = generatedBoard[x, y] ?? null;
                        break;
                    case ConsoleKey.Escape:
                        enJuego = false;
                        enMenuPrincipal = true;
                        break;
                    case ConsoleKey.End:
                        goto NewPuzzle;
                        break;
                    case ConsoleKey.P:
                        // Lógica para pausar el timer
                        break;
                    case ConsoleKey.M:
                        // Lógica para mutear la música
                        break;
                    case ConsoleKey.N:
                        // Lógica para indicar pistas
                        break;
                    case ConsoleKey.K:
                        // Lógica para pausar la música
                        break;
                        
                }
            }
            else if (enConfiguracion)
            {
                
                char tecla = Console.ReadKey().KeyChar;
                switch (tecla)
                {
                    case 'm':
                    case 'M':
                        musicManager.StopMusic();
                        IsMusicMuted = true;
                        menuConfiguracion(IsMusicMuted, n_color);

                        break;
                    case 'u':
                    case 'U':
                        musicManager.PlayMusic();
                        IsMusicMuted = false;
                        menuConfiguracion(IsMusicMuted, n_color);

                        break;
                    case 'c':
                    case 'C':
                        if (n_color<4) n_color++;
                        else if (n_color == 4) n_color = 0;
                        menuConfiguracion(IsMusicMuted, n_color)
                        break;
                    case (char)ConsoleKey.Enter:
                        enConfiguracion = false;
                        enMenuPrincipal = true;
                        break;
                }
            }
        }

void menuConfiguracion(bool muted_, int color_)
{
    if (!muted_)
    {
        Console.Clear();
        Console.Write("__________SETTINGS_________\n");
        Console.Write("- Sound [ON] \n- Colors [1-5]");
        Console.Write("\n\nM - mute / U - unmute.\nC - change background color");
        Console.Write("\n\n... Press Enter to apply and go back to menu");
    } 
    else if (muted_)
    {
        Console.Clear();
        Console.Write("__________SETTINGS_________\n");
        Console.Write("- Sound [OFF] \n- Colors [1-5]");
        Console.Write("\n\nM - mute / U - unmute.\nC - change background color");
        Console.Write("\n\n... Press Enter to apply and go back to menu");
    }

		  	do{
			//System.ConsoleKey tecla = System.Console.ReadKey().Key;
			//char tecla = Console.ReadKey().Key;
			    	goMenuPrincipal = false;
		    		
                    else if (tecla == 'C' || tecla == 'c')
                    {

                        if (color == 1) {Console.Clear();  Console.BackgroundColor = ConsoleColor.DarkGray; color++;}
                        else if (color == 2) {Console.Clear(); Console.BackgroundColor = ConsoleColor.DarkRed; color++;}
                        else if (color == 3) {Console.Clear(); Console.BackgroundColor = ConsoleColor.DarkMagenta; color++;}
                        else if (color == 4) {Console.Clear(); Console.BackgroundColor = ConsoleColor.DarkGreen; color++;}
                        else if (color == 5) {Console.Clear(); Console.BackgroundColor = ConsoleColor.DarkYellow; color = 1;}

                        Console.Write("__________SETTINGS_________\n");
				    	Console.Write("- Sound [ON] \n- Colors [1-5]");
				    	Console.Write("\n\nM - mute / U - unmute.\nC - change background color");
				    	Console.Write("\n\n... Press Enter to apply and go back to menu");
                    }
			    	else if (tecla == (char)ConsoleKey.Enter) goMenuPrincipal = true; tecla = 'x';
			   } while (!goMenuPrincipal);
}

while (!closeRequested)
{
  do{
		//TEXTO MENU INICIO
       // Console.BackgroundColor = ConsoleColor.White;
        Console.Clear();
		Console.WriteLine("		 ____________________");
		Console.WriteLine("		| (1) Configuracion  |");
		Console.WriteLine("		 _____________________");
		Console.WriteLine("		|(2) Iniciar partida  |");
		Console.WriteLine("		 ____________________");
		Console.WriteLine("		| (3) Ver ranking    |");
		Console.WriteLine("		 ____________________");
    switch (Console.ReadKey(true).Key)
    {
        case ConsoleKey.NumPad2: case ConsoleKey.D2:
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
                    Console.WriteLine($"Press [M] to turn the music ON/OFF");
                    Console.WriteLine($"Press [N] to get a hint");
                    Console.WriteLine($"Press [K] to Pause/Unpause the music");

                    Console.SetCursorPosition(y * 2 + 2 + (y / 3 * 2), x + 3 + +(x / 3));


                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.N:
                            if (activeBoard[x, y] == null && generatedBoard[x, y] == null)
                            {
                                int validValue = GetValidQuadrantValue(activeBoard, x, y);
                                activeBoard[x, y] = validValue;
                                //Console.WriteLine($"Hint: ({x + 1}, {y + 1}) = {validValue}");
                            }
                            else
                            {
                                Console.WriteLine("\t\t\t  Cannot provide a hint for a filled or locked cell.");
                            }
                        break;
                            case ConsoleKey.M:
                            muteMusic = !muteMusic;
                            if (muteMusic)
                                {
                                musicManager.AdjustVolume(0f); 
                                }
                            else
                                {
                                musicManager.AdjustVolume(1f); 
                                }
                            break;

                            case ConsoleKey.P:
                            paused = !paused;
                            Console.WriteLine(paused ? "Game Paused" : "Game Resumed");
                            break;
                        case ConsoleKey.K:
                                musicManager.PauseResumeMusic(); 
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
                    // Calcular la puntuación en segundos
                    int puntuacionEnSegundos = (int)timer.Elapsed.TotalSeconds;

                    // Agregar la puntuación a la matriz y ordenar la matriz
                    AgregarPuntuacion(puntuaciones, puntuacionEnSegundos);
                }
            }
            break; //CASO 1

        case ConsoleKey.NumPad1: case ConsoleKey.D1:
        	
        break;

        case ConsoleKey.NumPad3: case ConsoleKey.D3:
            MostrarRanking(puntuaciones);
            break;
    } //cierre switch
  }while (!closeRequested);

}
Console.Clear();
Console.Write("Sudoku was closed.");

static int GetValidQuadrantValue(int?[,] board, int x, int y)
{
    List<int> validValues = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    for (int i = x - x % 3; i <= x - x % 3 + 2; i++)
    {
        for (int j = y - y % 3; j <= y - y % 3 + 2; j++)
        {
            if (board[i, j].HasValue && validValues.Contains(board[i, j].Value))
            {
                validValues.Remove(board[i, j].Value);
            }
        }
    }
    validValues.Shuffle();
    if (validValues.Count > 0)
    {
        return validValues[0];
    }
    return -1;
}


static void MostrarRanking(int[] puntuaciones)
{
    Console.Clear();
    Console.WriteLine("Ranking de las 7 mejores puntuaciones:");

    // Ordenar la matriz de puntuaciones en orden ascendente
    Array.Sort(puntuaciones);

    // Mostrar las 7 mejores puntuaciones
    for (int i = 0; i < Math.Min(7, puntuaciones.Length); i++)
    {
        Console.WriteLine($"{i + 1}. {TimeSpan.FromSeconds(puntuaciones[i])}");
    }

    Console.WriteLine("Presiona cualquier tecla para volver al menú principal.");
    Console.ReadKey(true);
}



static void AgregarPuntuacion(int[] puntuaciones, int puntuacion)
{
    for (int i = 0; i < puntuaciones.Length; i++)
    {
        if (puntuaciones[i] == 0 || puntuacion < puntuaciones[i])
        {
            Array.Copy(puntuaciones, i, puntuaciones, i + 1, puntuaciones.Length - i - 1);
            puntuaciones[i] = puntuacion;
            break;
        }
    }
}

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
    Console.ForegroundColor = ConsoleColor.White;
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
                if (board[i, j].HasValue)
                {
                    Console.ForegroundColor = ConsoleColor.Green; // Cambia el color para los números ingresados por el jugador
                    Console.Write(board[i, j]);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("■");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" ");
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


public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        Random random = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
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
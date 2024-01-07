//#define DebugAlgorithm // <- uncomment to watch the generation algorithm

using System;
using System.Diagnostics;
using System.Threading;
using Towel;
using System.Collections.Generic;

// Variables de estado y control
bool closeRequested = false;
bool timeUp = false;
bool validInput = false;
bool enMenuPrincipal = true;
bool enConfiguracion = false;
bool enJuego = false;
bool enPreJuego = false;
bool enPostJuego = false;

bool nuevoJuego = false;
bool timerCreated = false;

//Variables de configuración
int n_color = 1;
bool IsMusicMuted = false;

// Variables de tablero y juego

int?[,] generatedBoard = null;
int?[,] activeBoard = null;

// Configuraciones de puntuación y aleatoriedad
Random random = new Random(); // Se agrega la declaración de Random
int[] puntuaciones = new int[7];
int maxBlanks = 80; // Todas las casillas menos 1
int selectedBlanks = maxBlanks;

// Configuraciones de tiempo y temporizador
int minutes = 0;
int seconds = 0;
Stopwatch timer = new Stopwatch();
bool paused = false; // Agregado para indicar si el temporizador está pausado

int valorInicialMinutes = 0; 
int valorInicialSeconds = 0;
int userInputMinutes = 0;
int userInputSeconds = 0;

bool timerRunning = false;




// Variables de posición para el selector o cursor
int x = 0; 
int y = 0;
int x_cur = 0;
int y_cur = 0;

// Variables relacionadas con la música y el sonido
string musicFilePath = "resources/musica.wav";
MusicManager musicManager = new MusicManager(musicFilePath, true); // El segundo parámetro establece true para "reproducir en bucle"
bool muteMusic = false;

// Variables de personalización de interfaz
int color = 1;



Console.Clear();
Show_menuPrincipal();
while (!closeRequested) // GAME LOOP
{   
    handleInput();
    update();
    render();
}

void handleInput()
{
    if (enMenuPrincipal)
    {
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.NumPad1: case ConsoleKey.D1:
                enMenuPrincipal = false;
                enConfiguracion = true;
                break;
            case ConsoleKey.NumPad2: case ConsoleKey.D2:
                //iniciarPartida();
                //iniciarPartida();
                enMenuPrincipal = false;
                enPreJuego = true;
                break;
            case ConsoleKey.NumPad3: case ConsoleKey.D3:
                MostrarRanking(puntuaciones);
                break;
        }
    }
    if (enPreJuego)
    {
        //Falta por separar entre input handle y lo que va en render (si tiene sentido hacerlo)
        Console.Clear();

        bool validInput = false;
        int maxBlanks = 80; // Todas las casillas menos 1
        selectedBlanks = maxBlanks;

        while (!validInput)
        {
            Console.Clear();
            Console.WriteLine("Sudoku");
            Console.WriteLine();
            Console.WriteLine("Press 'R' for a random number of initially filled cells, or");
            Console.WriteLine("Choose the number of initially filled cells (0 to " + maxBlanks + "): ");

            string input = "";
            ConsoleKeyInfo keyInfo;
            
            //ELEGIR NUMERO DE BLANKS -
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
            //ELEGIR NUMERO DE BLANKS -
            Console.Clear();
            // ELEGIR TIEMPO
            do
            {
                Console.WriteLine("Enter the desired time to solve the sudoku (in minutes and seconds):");
                Console.Write("Minutes: ");
            } while (!int.TryParse(Console.ReadLine(), out userInputMinutes));
            minutes = userInputMinutes;
            do
            {
                Console.Write("Seconds: ");
            } while (!int.TryParse(Console.ReadLine(), out userInputSeconds));
            seconds = userInputSeconds;
            Console.Clear();
        }
    }
    else if (enJuego)
    {
        Show_Juego();
        if (Console.KeyAvailable == true)
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
                case ConsoleKey.D1: case ConsoleKey.NumPad1:
                    activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 1, x, y) ? 1 : activeBoard[x, y];
                    break;
                case ConsoleKey.D2: case ConsoleKey.NumPad2:
                    activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 2, x, y) ? 2 : activeBoard[x, y];
                    break;
                case ConsoleKey.D3: case ConsoleKey.NumPad3:
                    activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 3, x, y) ? 3 : activeBoard[x, y];
                    break;
                case ConsoleKey.D4: case ConsoleKey.NumPad4:
                    activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 4, x, y) ? 4 : activeBoard[x, y];
                    break;
                case ConsoleKey.D5: case ConsoleKey.NumPad5:
                    activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 5, x, y) ? 5 : activeBoard[x, y];
                    break;
                case ConsoleKey.D6: case ConsoleKey.NumPad6:
                    activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 6, x, y) ? 6 : activeBoard[x, y];
                    break;
                case ConsoleKey.D7: case ConsoleKey.NumPad7:
                    activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 7, x, y) ? 7 : activeBoard[x, y];
                    break;
                case ConsoleKey.D8: case ConsoleKey.NumPad8:
                    activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 8, x, y) ? 8 : activeBoard[x, y];
                    break;
                case ConsoleKey.D9: case ConsoleKey.NumPad9:
                    activeBoard[x, y] = IsValidMove(activeBoard, generatedBoard, 9, x, y) ? 9 : activeBoard[x, y];
                    break;


                case ConsoleKey.Backspace: case ConsoleKey.Delete:
                    activeBoard[x, y] = generatedBoard[x, y] ?? null;
                    break;

                case ConsoleKey.Escape:
                    enJuego = false;
                    enMenuPrincipal = true;
                    break;

                case ConsoleKey.End:
                    nuevoJuego = true;
                    //paused = !paused;                                              
                    //iniciarPartida();
                    break;


                case ConsoleKey.P: // Pausar el timer
                    paused = !paused;
                    Console.WriteLine(paused ? "Game Paused" : "Game Resumed"); 
                    break;

                case ConsoleKey.M: //Mutear la música
                    muteMusic = !muteMusic;
                    if (muteMusic) musicManager.AdjustVolume(0f); 
                    else musicManager.AdjustVolume(1f); 
                    break;

                case ConsoleKey.N:  //Indicar pistas
                    if (activeBoard[x, y] == null && generatedBoard[x, y] == null)
                    {
                        int validValue = GetValidQuadrantValue(activeBoard, x, y);
                        activeBoard[x, y] = validValue;
                        //Console.WriteLine($"Hint: ({x + 1}, {y + 1}) = {validValue}");
                    }
                    else Console.WriteLine("\t\t\t  Cannot provide a hint for a filled or locked cell.");
                    break;

                case ConsoleKey.K: // Pausar la música
                    musicManager.PauseResumeMusic(); 
                    break;

        }
            /*if (paused)
            {
                continue; // Salta al siguiente ciclo sin actualizar el temporizador
            }

            // Actualizar el temporizador solo si no está pausado
            if (timer.Elapsed.TotalSeconds >= 1)
            {
                seconds--;
                timer.Restart();
            }*/
        }
        else System.Threading.Thread.Sleep(100); 
;
    }
    else if (enConfiguracion)
    {
        Show_menuConfiguracion();
        char tecla = Console.ReadKey().KeyChar;
        switch (tecla)
        {
            case 'm': case 'M':
                musicManager.StopMusic();
                IsMusicMuted = true;
                Show_menuConfiguracion();

                break;
            case 'u': case 'U':
                musicManager.PlayMusic();
                IsMusicMuted = false;
                Show_menuConfiguracion();
                break;


            case 'c': case 'C':
                if (n_color<5) n_color++;
                else if (n_color == 5) n_color = 1;
                Show_menuConfiguracion();
                break;

            case (char)ConsoleKey.Enter:
                enConfiguracion = false;
                enMenuPrincipal = true;
                Show_menuPrincipal();
                break;
        }
    }
    else if (enPostJuego)
    {
        do
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Enter: 
                    validInput = true;
                    nuevoJuego = true;
                break;
                case ConsoleKey.Escape:
                    validInput = true;
                    closeRequested = true;
                    Console.Clear();
                break;
                case ConsoleKey.R:
                    MostrarRanking(puntuaciones);
                break;  
                default:
                    validInput = false;
                break;              
            }
        }while (!validInput);
    }
}



void update()
{
    if (enPostJuego) 
    {
        finalizarJuego();
        enPostJuego = false;
        enPreJuego = true;
        nuevoJuego = false;
    }
    else if (enPreJuego)
    {
        if(!timerCreated)
        {
            Thread timerThread = new Thread(() =>
            {
                timer.Start();
                DateTime startTime = DateTime.Now;

                while (minutes > 0 || seconds > 0) /*&& !tokenSource.Token.IsCancellationRequested)*/
                {
                    Thread.Sleep(1000); // Wait for 1 second
                    if (!paused)

                    {
                        TimeSpan elapsed = DateTime.Now - startTime;
                        if (elapsed.TotalSeconds >= 1)
                        {
                            seconds--;

                            if (seconds < 0)
                            {
                                minutes--;
                                seconds = 59;
                            }

                            startTime = DateTime.Now;
                        }
                    }
                }
            });
            timerThread.Start();
            timerCreated = true;
        }


        //CREAR SUDOKU
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

        enPreJuego = false;
        enJuego = true;
    }
    else if (enJuego)
    {
        if (nuevoJuego) 
        {
            enJuego = false;
            enPreJuego = true;
            nuevoJuego = false;
        }
        else if (seconds == 0 && minutes == 0) 
        {
            enJuego = false;
            enPostJuego = true;
        }
        if(!ContainsNulls(activeBoard)) // Si no quedan huecos en blanco
        {
            enJuego = false;
            enPostJuego = true;
        }

    }
}

//RENDERs() - - - - - - - - - - - -
void render()
{
    if (enMenuPrincipal) Show_menuPrincipal();
    else if (enConfiguracion) Show_menuConfiguracion();
    else if (enPreJuego) Show_PreJuego();
    else if (enJuego) Show_Juego();
    else if (enPostJuego) Show_PostJuego();
    //else finalizarJuego();
    
}
void Show_menuPrincipal()
{
    Console.Clear();
	Console.WriteLine("		 ____________________");
	Console.WriteLine("		| (1) Configuracion  |");
	Console.WriteLine("		 _____________________");
	Console.WriteLine("		|(2) Iniciar partida  |");
	Console.WriteLine("		 ____________________");
	Console.WriteLine("		| (3) Ver ranking    |");
	Console.WriteLine("		 ____________________");    
}

void Show_menuConfiguracion()
{
    Console.Clear();

    switch (n_color)
    {
        case 1: Console.Clear(); Console.BackgroundColor = ConsoleColor.DarkGray; break;
        case 2: Console.Clear(); Console.BackgroundColor = ConsoleColor.DarkRed; break;
        case 3: Console.Clear(); Console.BackgroundColor = ConsoleColor.DarkMagenta; break;
        case 4: Console.Clear(); Console.BackgroundColor = ConsoleColor.DarkGreen; break;
        case 5: Console.Clear(); Console.BackgroundColor = ConsoleColor.DarkYellow; break;
    }

    if (!IsMusicMuted)
    {
        Console.Clear();
        Console.Write("__________SETTINGS_________\n");
        Console.Write("- Sound [ON] \n- Colors [" + (n_color) + "/5]");      
        Console.Write("\n\nM - mute / U - unmute.\nC - change background color");
        Console.Write("\n\n... Press Enter to apply and go back to menu");
    } 
    else if (IsMusicMuted)
    {
        Console.Clear();
        Console.Write("__________SETTINGS_________\n");
        Console.Write("- Sound [OFF] \n- Colors [" + (n_color) + "/5]");      
        Console.Write("\n\nM - mute / U - unmute.\nC - change background color");
        Console.Write("\n\n... Press Enter to apply and go back to menu");
    }
}

void Show_PreJuego(){}
void Show_Juego()
{
    Console.SetCursorPosition(x_cur, y_cur);
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
}

void Show_PostJuego()
{
    Console.Clear();
    Console.WriteLine("Sudoku");
    Console.WriteLine();
    ConsoleWrite(activeBoard, generatedBoard);
    Console.WriteLine();
    Console.WriteLine((minutes == 0 && seconds == 0) ? "Time's up!" : "You Win!");
    Console.WriteLine($"Time Elapsed: {TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds)}");
    Console.WriteLine();
    Console.WriteLine("Play Again [enter], or quit [escape]? \nOr see ranking ;) [R]");
}

//RENDERs() - - - - - - - - - - - -

Console.Clear();
Console.Write("Sudoku was closed.");



void finalizarJuego()
{
        // Calcular la puntuación en segundos
        int puntuacionEnSegundos = (int)timer.Elapsed.TotalSeconds;
        // Añadir puntuacion al ranking y ordenarla
        AgregarPuntuacion(puntuaciones, puntuacionEnSegundos);

        // Mostrar resultados y solicitar input para jugar de nuevo o salir
        //renderizarResultadosYsolicitarInput();
}

/*void renderizarResultadosYsolicitarInput()
{
    
    
    Show_PostJuego();
    enPostJuego=true;
    while(true)
    {
        ConsoleKey key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.Enter:
                enPostJuego=false;
                enMenuPrincipal=true;
                Show_menuPrincipal();
                return;
            case ConsoleKey.Escape:
                closeRequested = true;
                return;
            default:
                
                break;
        }
    }
}
*/


/*void TimerThread()
{
    while (minutes > 0 || seconds > 0)
    {
        Thread.Sleep(1000); // Esperar 1 segundo

        if (closeRequested)
        {
            break; // Salir del bucle si se solicita cerrar
        }

        if (!paused) // Continuar solo si el temporizador no está en pausa
        {
            if (seconds == 0 && minutes > 0)
            {
                minutes--;
                seconds = 59;
            }
            else
            {
                seconds--;
            }
        }
    }

    if (minutes == 0 && seconds == 0)
    {
        
        timeUp = true; // Indicar que el tiempo se ha acabado
        finalizarJuego();
    }
}*/

/*void iniciarPartida()
{
   // Genera un nuevo tablero de Sudoku con un número específico de celdas llenas.
    generatedBoard = Sudoku.Generate(random, 81 - selectedBlanks);
    activeBoard = new int?[9, 9];

    // Copia los valores del tablero generado al tablero activo donde el jugador hará sus movimientos.
    for (int i = 0; i < 9; i++)
    {
        for (int j = 0; j < 9; j++)
        {
            activeBoard[i, j] = generatedBoard[i, j];
        }
    }

    // Reinicia la posición del selector al inicio del tablero.
    x = 0;
    y = 0;

    // Preparativos adicionales antes de comenzar el juego
    Console.Clear();
    // Reiniciar el temporizador y establecer los minutos y segundos seleccionados
    timer.Restart(); // Reinicia el temporizador



    
}*/

//MÉTODOS DEL SUDOKU - - - - - - -

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

void MostrarRanking(int[] puntuaciones)
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

    if(enMenuPrincipal) Console.WriteLine("Presiona cualquier tecla para volver al menú principal.");
    else if(enPostJuego) Console.WriteLine("Play again [end] or exit [escape]?");
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
                    Console.ForegroundColor = GetContrastingColor(Console.BackgroundColor); //Color Usuario
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

ConsoleColor GetContrastingColor(ConsoleColor backgroundColor)
{
    switch (backgroundColor)
    {
        case ConsoleColor.DarkGray:
            return ConsoleColor.Cyan; 
        case ConsoleColor.DarkRed:
            return ConsoleColor.Yellow; 
        case ConsoleColor.DarkMagenta:
            return ConsoleColor.Green; 
        case ConsoleColor.DarkGreen:
            return ConsoleColor.Magenta; 
        case ConsoleColor.DarkYellow:
            return ConsoleColor.Blue; 
        default:
            return ConsoleColor.DarkYellow; 
    }
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

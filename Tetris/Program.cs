using System;
using System.Threading;

class Tetris
{
    static int ancho = 10;
    static int alto = 20;
    static int[,] tablero = new int[alto, ancho];
    static int[][] piezaActual;
    static int posXPiezaActual, posYPiezaActual;
    static bool juegoTerminado = false;
    static int velocidad;

    static bool EsRotacionValida(int[][] nuevaPieza)
    {
        int piezaAncho = nuevaPieza[0].Length;
        int piezaAlto = nuevaPieza.Length;

        for (int y = 0; y < piezaAlto; y++)
        {
            for (int x = 0; x < piezaAncho; x++)
            {
                if (nuevaPieza[y][x] == 1)
                {
                    int posXTablero = posXPiezaActual + x;
                    int posYTablero = posYPiezaActual + y;

                    if (posXTablero < 0 || posXTablero >= ancho || posYTablero >= alto || (posYTablero >= 0 && tablero[posYTablero, posXTablero] != 0))
                    {
                        return false; // La rotación no es válida
                    }
                }
            }
        }

        return true; // La rotación es válida
    }
    static void Main()
    {
        Console.WindowHeight = alto + 1;
        Console.WindowWidth = ancho * 2;
        Console.BufferHeight = Console.WindowHeight;
        Console.BufferWidth = Console.WindowWidth;

        SeleccionarDificultad(); // Agrega esta línea para seleccionar la dificultad

        Inicializar();
        while (!juegoTerminado)
        {
            if (Console.KeyAvailable)
            {
                var tecla = Console.ReadKey(true).Key;
                switch (tecla)
                {
                    case ConsoleKey.LeftArrow:
                        MoverIzquierda();
                        break;
                    case ConsoleKey.RightArrow:
                        MoverDerecha();
                        break;
                    case ConsoleKey.DownArrow:
                        MoverAbajo();
                        break;
                    case ConsoleKey.UpArrow:
                        RotarPieza();
                        break;
                }
            }
            else
            {
                MoverAbajo();
            }

            Dibujar();
            Thread.Sleep(velocidad); // Utiliza la variable velocidad para controlar la velocidad del juego
        }

        Console.Clear();
        Console.WriteLine("Juego Terminado");
    }

    static void SeleccionarDificultad()
    {
        Console.Clear();
        Console.WriteLine("Selecciona la dificultad:");
        Console.WriteLine("1 - Difícil");
        Console.WriteLine("2 - Medio");
        Console.WriteLine("3 - Fácil");
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey();
            if (key.Key == ConsoleKey.D1 || key.Key == ConsoleKey.NumPad1)
            {
                velocidad = 50; // Difícil
                break;
            }
            else if (key.Key == ConsoleKey.D2 || key.Key == ConsoleKey.NumPad2)
            {
                velocidad = 100; // Medio
                break;
            }
            else if (key.Key == ConsoleKey.D3 || key.Key == ConsoleKey.NumPad3)
            {
                velocidad = 200; // Fácil
                break;
            }
        } while (key.Key != ConsoleKey.Escape);
    }

    static void MostrarMensajePerdida()
    {
        Console.Clear();
        Console.WriteLine("Juego Terminado. ¡Has perdido!");
        Console.WriteLine("Presiona cualquier tecla para salir.");
        Console.ReadKey();
    }

    static void VerificarFinJuego()
    {
        if (posYPiezaActual == 0)
        {
            juegoTerminado = true;
            MostrarMensajePerdida();
        }
    }

    static void Inicializar()
    {
        ElegirPiezaAleatoria();
        posXPiezaActual = ancho / 2 - piezaActual[0].Length / 2;
        posYPiezaActual = 0;
        Dibujar();
    }

    static void ElegirPiezaAleatoria()
    {
        // Genera una pieza aleatoria (L, T o la original)
        Random random = new Random();
        int opcion = random.Next(3);
        if (opcion == 0)
        {
            piezaActual = new int[][] { new int[] { 1, 1, 1, 1 } };
        }
        else if (opcion == 1)
        {
            piezaActual = new int[][] { new int[] { 1, 1, 1 }, new int[] { 1, 0, 0 } };
        }
        else
        {
            piezaActual = new int[][] { new int[] { 1, 1, 1 }, new int[] { 0, 1, 0 } };
        }
    }

    static void MoverIzquierda()
    {
        Borrar();
        posXPiezaActual--;
        if (!EsMovimientoValido())
        {
            posXPiezaActual++;
        }
    }

    static void MoverDerecha()
    {
        Borrar();
        posXPiezaActual++;
        if (!EsMovimientoValido())
        {
            posXPiezaActual--;
        }
    }

    static void MoverAbajo()
    {
        Borrar();
        posYPiezaActual++;
        if (!EsMovimientoValido())
        {
            posYPiezaActual--;
            CombinarPieza();
            EliminarLineasCompletas();
            VerificarFinJuego();
            Inicializar();
            if (!EsMovimientoValido())
            {
                juegoTerminado = true;
                MostrarMensajePerdida();
            }
        }
    }

    static void RotarPieza()
    {
        Borrar();
        int originalAncho = piezaActual[0].Length;
        int originalAlto = piezaActual.Length;
        int[][] nuevaPieza = new int[originalAncho][];
        for (int x = 0; x < originalAncho; x++)
        {
            nuevaPieza[x] = new int[originalAlto];
            for (int y = 0; y < originalAlto; y++)
            {
                nuevaPieza[x][y] = piezaActual[y][originalAncho - 1 - x];
            }
        }

        // Verificar si la rotación es válida antes de aplicarla
        if (EsRotacionValida(nuevaPieza))
        {
            piezaActual = nuevaPieza;
        }
    }

    static bool EsMovimientoValido()
    {
        int piezaAncho = piezaActual[0].Length;
        int piezaAlto = piezaActual.Length;

        for (int y = 0; y < piezaAlto; y++)
        {
            for (int x = 0; x < piezaAncho; x++)
            {
                if (piezaActual[y][x] == 1)
                {
                    int posXTablero = posXPiezaActual + x;
                    int posYTablero = posYPiezaActual + y;

                    if (posXTablero < 0 || posXTablero >= ancho || posYTablero >= alto || (posYTablero >= 0 && tablero[posYTablero, posXTablero] != 0))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    static void CombinarPieza()
    {
        int piezaAncho = piezaActual[0].Length;
        int piezaAlto = piezaActual.Length;

        for (int y = 0; y < piezaAlto; y++)
        {
            for (int x = 0; x < piezaAncho; x++)
            {
                if (piezaActual[y][x] == 1)
                {
                    tablero[posYPiezaActual + y, posXPiezaActual + x] = 1;
                }
            }
        }
    }

    static void EliminarLineasCompletas()
    {
        for (int y = alto - 1; y >= 0; y--)
        {
            bool lineaCompleta = true;
            for (int x = 0; x < ancho; x++)
            {
                if (tablero[y, x] == 0)
                {
                    lineaCompleta = false;
                    break;
                }
            }

            if (lineaCompleta)
            {
                for (int i = y; i > 0; i--)
                {
                    for (int x = 0; x < ancho; x++)
                    {
                        tablero[i, x] = tablero[i - 1, x];
                    }
                }
                y++;
            }
        }
    }

    static void Dibujar()
    {
        Console.Clear();

        for (int y = 0; y < alto; y++)
        {
            for (int x = 0; x < ancho; x++)
            {
                Console.SetCursorPosition(x * 2, y);
                Console.Write(tablero[y, x] == 0 ? "  " : "■");
            }
        }

        int piezaAncho = piezaActual[0].Length;
        int piezaAlto = piezaActual.Length;

        for (int y = 0; y < piezaAlto; y++)
        {
            for (int x = 0; x < piezaAncho; x++)
            {
                if (piezaActual[y][x] == 1)
                {
                    Console.SetCursorPosition((posXPiezaActual + x) * 2, posYPiezaActual + y);
                    Console.Write("■");
                }
            }
        }
    }

    static void Borrar()
    {
        int piezaAncho = piezaActual[0].Length;
        int piezaAlto = piezaActual.Length;

        for (int y = 0; y < piezaAlto; y++)
        {
            for (int x = 0; x < piezaAncho; x++)
            {
                if (piezaActual[y][x] == 1)
                {
                    Console.SetCursorPosition((posXPiezaActual + x) * 2, posYPiezaActual + y);
                    Console.Write("  ");
                }
            }
        }
    }
}

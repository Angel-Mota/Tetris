using System;
using System.Threading;

class Tetris
{
    static int ancho = 10;
    static int alto = 20;
    static int[,] tablero = new int[alto, ancho];
    static int[,] piezaActual = new int[,] { { 1, 1, 1, 1 } };
    static int posXPiezaActual, posYPiezaActual;
    static bool juegoTerminado = false;
    static bool rotated = false;

    static void Main()
    {
        Console.WindowHeight = alto + 1;
        Console.WindowWidth = ancho * 2;
        Console.BufferHeight = Console.WindowHeight;
        Console.BufferWidth = Console.WindowWidth;

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
            Thread.Sleep(100);
        }

        Console.Clear();
        Console.WriteLine("Juego Terminado");
    }

    static void Inicializar()
    {
        posXPiezaActual = ancho / 2 - piezaActual.GetLength(1) / 2;
        posYPiezaActual = 0;
        rotated = false;
        Dibujar();
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
        }
    }

    static void RotarPieza()
    {
        Borrar();
        if (!rotated)
        {
            // Rotar la pieza 90 grados en sentido antihorario
            int[,] nuevaPieza = new int[piezaActual.GetLength(1), piezaActual.GetLength(0)];
            for (int y = 0; y < piezaActual.GetLength(0); y++)
            {
                for (int x = 0; x < piezaActual.GetLength(1); x++)
                {
                    nuevaPieza[x, piezaActual.GetLength(0) - 1 - y] = piezaActual[y, x];
                }
            }
            piezaActual = nuevaPieza;
            rotated = true;
        }
        else
        {
            // Volver la pieza a su estado original
            piezaActual = new int[,] { { 1, 1, 1, 1 } };
            rotated = false;
        }

        if (!EsMovimientoValido())
        {
            // Si la rotación no es válida, deshacer la rotación
            if (rotated)
            {
                piezaActual = new int[,] { { 1, 1, 1, 1 } };
            }
            else
            {
                int[,] nuevaPieza = new int[piezaActual.GetLength(1), piezaActual.GetLength(0)];
                for (int y = 0; y < piezaActual.GetLength(0); y++)
                {
                    for (int x = 0; x < piezaActual.GetLength(1); x++)
                    {
                        nuevaPieza[x, piezaActual.GetLength(0) - 1 - y] = piezaActual[y, x];
                    }
                }
                piezaActual = nuevaPieza;
            }
        }
    }

    static bool EsMovimientoValido()
    {
        for (int y = 0; y < piezaActual.GetLength(0); y++)
        {
            for (int x = 0; x < piezaActual.GetLength(1); x++)
            {
                if (piezaActual[y, x] == 1)
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
        for (int y = 0; y < piezaActual.GetLength(0); y++)
        {
            for (int x = 0; x < piezaActual.GetLength(1); x++)
            {
                if (piezaActual[y, x] == 1)
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

    static void VerificarFinJuego()
    {
        if (posYPiezaActual == 0)
        {
            juegoTerminado = true;
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

        for (int y = 0; y < piezaActual.GetLength(0); y++)
        {
            for (int x = 0; x < piezaActual.GetLength(1); x++)
            {
                if (piezaActual[y, x] == 1)
                {
                    Console.SetCursorPosition((posXPiezaActual + x) * 2, posYPiezaActual + y);
                    Console.Write("■");
                }
            }
        }
    }

    static void Borrar()
    {
        for (int y = 0; y < piezaActual.GetLength(0); y++)
        {
            for (int x = 0; x < piezaActual.GetLength(1); x++)
            {
                if (piezaActual[y, x] == 1)
                {
                    Console.SetCursorPosition((posXPiezaActual + x) * 2, posYPiezaActual + y);
                    Console.Write("  ");
                }
            }
        }
    }
}
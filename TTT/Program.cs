using System;

namespace TTT
{
    class Program
    {
        static char[] board = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        static int currentPlayer = 1; // первый игрок это "X", второй игрок это "O"
        static int choice;
        static int gameStatus = 0; // переменная со статусом игры: "0" игра продолжается, "1" кто-то из игроков выиграл, "-1" это ничья

        static void Main(string[] args)
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Добро пожаловать в игру!\n");
                Console.WriteLine("Игрок номер 1 играет крестиками (X), а игрок номер 2 играет ноликами (O).\n");

                DrawBoard();

                bool validInput = false;
                while (!validInput)
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Игрок {0}. Введите номер клетки от 1 до 9:", (currentPlayer % 2 == 0) ? "2 (O)" : "1 (X)");
                    string input = Console.ReadLine();

                    // Проверям на валидность введеное игроком значение
                    validInput = int.TryParse(input, out choice) &&
                                choice >= 1 &&
                                choice <= 9 &&
                                board[choice - 1] != 'X' &&
                                board[choice - 1] != 'O';

                    if (!validInput)
                    {
                        Console.WriteLine("\nНекорректный ввод значения, попробуйте еще раз.");
                    }
                }

                // Заполняем клетку символом игрока
                char mark = (currentPlayer % 2 == 0) ? 'O' : 'X';
                board[choice - 1] = mark;

                // Проверяем условия победы
                gameStatus = CheckWin();

                // Меняем игрока
                currentPlayer++;

            } while (gameStatus == 0);

            Console.Clear();
            DrawBoard();

            if (gameStatus == 1)
            {
                Console.WriteLine("\nИгрок {0} победил!", (currentPlayer % 2 == 0) ? "1 (X)" : "2 (O)");
            }
            else
            {
                Console.WriteLine("\nНичья!");
            }

            Console.ReadLine();
        }

        private static void DrawBoard()
        {
            // Нарисуем игровое поле цифрами от 1 до 9.
            Console.WriteLine("     *     *     ");
            Console.WriteLine("  {0}  *  {1}  *  {2}  ", board[0], board[1], board[2]);
            Console.WriteLine(" * * * * * * * * ");
            Console.WriteLine("     *     *     ");
            Console.WriteLine("  {0}  *  {1}  *  {2}  ", board[3], board[4], board[5]);
            Console.WriteLine(" * * * * * * * * ");
            Console.WriteLine("     *     *     ");
            Console.WriteLine("  {0}  *  {1}  *  {2}  ", board[6], board[7], board[8]);
            Console.WriteLine("     *     *     ");
        }

        private static int CheckWin()
        {
            // Горизонтальные победы
            if (board[0] == board[1] && board[1] == board[2])
                return 1;
            if (board[3] == board[4] && board[4] == board[5])
                return 1;
            if (board[6] == board[7] && board[7] == board[8])
                return 1;

            // Вертикальные победы
            if (board[0] == board[3] && board[3] == board[6])
                return 1;
            if (board[1] == board[4] && board[4] == board[7])
                return 1;
            if (board[2] == board[5] && board[5] == board[8])
                return 1;

            // Диагональные победы
            if (board[0] == board[4] && board[4] == board[8])
                return 1;
            if (board[2] == board[4] && board[4] == board[6])
                return 1;

            // Ничья
            bool isDraw = true;
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] != 'X' && board[i] != 'O')
                {
                    isDraw = false;
                    break;
                }
            }
            if (isDraw)
                return -1;

            return 0;
        }
    }
}
using System;
using System.Text;

namespace FitnessClubApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            bool running = true;

            while (running)
            {
                ShowMenu();
                string choice = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (choice)
                {
                    case "1": ShowAbout(); Pause(); break;
                    case "2": ShowCostTable(); Pause(); break;
                    case "3": ShowPriceTable(); Pause(); break;
                    case "0":
                        running = false;
                        Console.WriteLine("До побачення!");
                        break;
                    default:
                        Console.WriteLine("Немає такого пункту. Спробуйте ще раз.");
                        Pause();
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("=== Фітнес-клуб ===");
            Console.WriteLine("1 — довідка про програму");
            Console.WriteLine("2 — таблиця вартості абонемента");
            Console.WriteLine("3 — таблиця за місяцями й типами абонементів");
            Console.WriteLine("0 — вихід");
            Console.Write("Ваш вибір: ");
        }

        // Показує тему проєкту, головну сутність і її поля.
        static void ShowAbout()
        {
            Console.WriteLine("Тема проєкту: Фітнес-клуб.");
            Console.WriteLine("Головна сутність: абонемент.");
            Console.WriteLine("Поля абонемента:");
            Console.WriteLine("- int номер — призначає програма;");
            Console.WriteLine("- string прізвище клієнта;");
            Console.WriteLine("- int код типу абонемента — з довідника;");
            Console.WriteLine("- DateTime дата початку;");
            Console.WriteLine("- int кількість місяців;");
            Console.WriteLine("- double ціна;");
            Console.WriteLine("- bool чи активний.");
            Console.WriteLine("Підсумкова величина: абонементи, строк яких закінчується протягом тижня.");
        }

        // Зчитує межі й крок та передає їх методу табулювання.
        static void ShowCostTable()
        {
            double from;
            double to;
            double step;
            double intervals;

            while (true)
            {
                from = ReadDouble("Початок проміжку в місяцях: ", 1, 1000);
                to = ReadDouble("Кінець проміжку в місяцях: ", from, 1000);
                step = ReadDouble("Крок (наприклад, 0,5): ", 0.01, 100);
                intervals = (to - from) / step;

                if (intervals >= 7 && intervals <= 999)
                {
                    break;
                }

                Console.WriteLine("Потрібно щонайменше 8 рядків. Введіть межі та крок ще раз.");
            }

            Console.WriteLine("Місячна ціна для прикладу: 1000 грн.");
            PrintCostTable(from, to, step);
        }

        // Табулює вартість і вартість зі знижкою для переданих меж та кроку.
        static void PrintCostTable(double from, double to, double step)
        {
            const double monthlyRate = 1000;
            Console.WriteLine("{0,10}{1,22}{2,24}", "Місяців", "Без знижки, грн", "Зі знижкою, грн");

            for (double months = from; months <= to + 1e-9; months += step)
            {
                double fullPrice = CalculateCost(months, monthlyRate);
                double discountedPrice = CalculateDiscountedCost(months, monthlyRate);
                Console.WriteLine("{0,10:F1}{1,22:F2}{2,24:F2}", months, fullPrice, discountedPrice);
            }
        }

        // Повертає повну вартість абонемента без виведення на екран.
        static double CalculateCost(double months, double monthlyRate)
        {
            return months * monthlyRate;
        }

        // Повертає вартість зі знижкою 5% для строку від трьох місяців.
        static double CalculateDiscountedCost(double months, double monthlyRate)
        {
            double fullPrice = CalculateCost(months, monthlyRate);
            return months >= 3 ? fullPrice * 0.95 : fullPrice;
        }

        // Будує таблицю вкладеними циклами й рахує підсумки по краях.
        static void ShowPriceTable()
        {
            int rows = ReadInt("Кількість рядків (місяців, 1–20): ", 1, 20);
            int columns = ReadInt("Кількість стовпців (типів, 1–20): ", 1, 20);
            double[] columnTotals = new double[columns];
            double grandTotal = 0;

            Console.WriteLine("Умовний тариф: тип 1 — 500 грн/місяць; наступний тип дорожчий на 250 грн.");
            Console.Write("{0,10}", "Місяць");
            for (int column = 1; column <= columns; column++)
            {
                Console.Write("{0,14}", "Тип " + column);
            }
            Console.WriteLine("{0,16}", "Разом, грн");

            for (int row = 1; row <= rows; row++)
            {
                double rowTotal = 0;
                Console.Write("{0,10}", row);
                for (int column = 1; column <= columns; column++)
                {
                    double monthlyRate = 500 + (column - 1) * 250;
                    double value = row * monthlyRate;
                    rowTotal += value;
                    columnTotals[column - 1] += value;
                    grandTotal += value;
                    Console.Write("{0,14:F2}", value);
                }
                Console.WriteLine("{0,16:F2}", rowTotal);
            }

            Console.Write("{0,10}", "Разом");
            for (int column = 0; column < columns; column++)
            {
                Console.Write("{0,14:F2}", columnTotals[column]);
            }
            Console.WriteLine("{0,16:F2}", grandTotal);
        }

        // Читає число з клавіатури й повторює запит, доки значення не буде в межах.
        static double ReadDouble(string prompt, double min, double max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                double number;

                if (!double.TryParse(input, out number) || double.IsNaN(number) || double.IsInfinity(number))
                {
                    Console.WriteLine("Помилка: введіть число.");
                    continue;
                }

                if (number < min || number > max)
                {
                    Console.WriteLine("Помилка: значення має бути від " + min + " до " + max + ".");
                    continue;
                }

                return number;
            }
        }

        // Читає ціле число з клавіатури й повторює запит, доки значення не буде в межах.
        static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                int number;

                if (!int.TryParse(input, out number))
                {
                    Console.WriteLine("Помилка: введіть ціле число.");
                    continue;
                }

                if (number < min || number > max)
                {
                    Console.WriteLine("Помилка: значення має бути від " + min + " до " + max + ".");
                    continue;
                }

                return number;
            }
        }

        static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Натисніть будь-яку клавішу, щоб повернутися до меню...");
            Console.ReadKey(true);
            Console.Clear();
        }
    }
}


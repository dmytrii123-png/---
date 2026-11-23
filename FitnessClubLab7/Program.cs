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

            // Головний цикл повертає користувача до меню після кожної дії.
            while (running)
            {
                ShowMenu();
                Console.Write("Ваш вибір: ");
                string choice = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ShowAbout();
                        Pause();
                        break;
                    case "2":
                        ShowSubscriptionCostTable();
                        Pause();
                        break;
                    case "3":
                        ShowMonthlyPriceTable();
                        Pause();
                        break;
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
            Console.WriteLine("2 — таблиця вартості абонемента за тривалістю");
            Console.WriteLine("3 — таблиця вартості за місяцями й типами абонементів");
            Console.WriteLine("0 — вихід");
        }

        // Довідка містить тему, сутність, її поля та підсумок для теми №3.
        static void ShowAbout()
        {
            Console.WriteLine("Тема проєкту: Фітнес-клуб.");
            Console.WriteLine("Головна сутність: абонемент.");
            Console.WriteLine("Поля абонемента:");
            Console.WriteLine("- int номер — номер призначає програма;");
            Console.WriteLine("- string прізвище клієнта;");
            Console.WriteLine("- int код типу абонемента — посилання на довідник;");
            Console.WriteLine("- DateTime дата початку;");
            Console.WriteLine("- int кількість місяців;");
            Console.WriteLine("- double ціна;");
            Console.WriteLine("- bool чи активний.");
            Console.WriteLine("Підсумкова величина: абонементи, що завершуються протягом тижня.");
        }

        // Показуємо повну вартість і вартість зі знижкою для різної тривалості.
        static void ShowSubscriptionCostTable()
        {
            Console.Write("Введіть ціну абонемента за місяць у гривнях: ");
            string input = Console.ReadLine() ?? "";
            double monthlyPrice;

            if (!double.TryParse(input, out monthlyPrice) || monthlyPrice <= 0)
            {
                Console.WriteLine("Помилка: введіть додатну числову ціну.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Знижка 5% діє для абонементів від 3 місяців.");
            Console.WriteLine("Крок таблиці — пів місяця.");
            Console.WriteLine("{0,10}{1,22}{2,24}", "Місяців", "Без знижки, грн", "Зі знижкою, грн");

            for (double months = 1.0; months <= 5.0 + 1e-9; months += 0.5)
            {
                double fullPrice = monthlyPrice * months;
                double discountedPrice = months >= 3.0 ? fullPrice * 0.95 : fullPrice;
                Console.WriteLine("{0,10:F1}{1,22:F2}{2,24:F2}", months, fullPrice, discountedPrice);
            }
        }

        // Рядки — тривалість у місяцях, стовпці — типи абонементів.
        // Ціни типів умовні для навчальної таблиці: 500 грн і +250 грн за наступний тип.
        static void ShowMonthlyPriceTable()
        {
            Console.Write("Кількість рядків (місяців): ");
            int rows;
            if (!TryReadPositiveNumber(out rows))
            {
                Console.WriteLine("Помилка: потрібно ввести додатне ціле число. Повернення до меню.");
                return;
            }

            Console.Write("Кількість стовпців (типів абонемента): ");
            int columns;
            if (!TryReadPositiveNumber(out columns))
            {
                Console.WriteLine("Помилка: потрібно ввести додатне ціле число. Повернення до меню.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Умовна ціна за місяць: тип 1 — 500 грн, кожен наступний тип дорожчий на 250 грн.");
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
                    double cellPrice = row * monthlyRate;
                    rowTotal += cellPrice;
                    Console.Write("{0,14:F2}", cellPrice);
                }

                Console.WriteLine("{0,16:F2}", rowTotal);
            }
        }

        static bool TryReadPositiveNumber(out int number)
        {
            string input = Console.ReadLine() ?? "";
            return int.TryParse(input, out number) && number > 0;
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

using System;
using System.Text;

namespace FitnessClubApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            double[] values = new double[100];
            int count = 0;
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
                    case "4":
                        Console.Write("Введіть числове значення: ");
                        string valueText = Console.ReadLine() ?? "";
                        double newValue;
                        if (double.TryParse(valueText, out newValue) && !double.IsNaN(newValue) && !double.IsInfinity(newValue))
                        {
                            AddValue(values, ref count, newValue, values.Length);
                        }
                        else
                        {
                            Console.WriteLine("Помилка: введіть коректне число.");
                        }
                        Pause();
                        break;
                    case "5":
                        Console.Write("Введіть номер значення для видалення: ");
                        string indexText = Console.ReadLine() ?? "";
                        int recordNumber;
                        if (int.TryParse(indexText, out recordNumber))
                        {
                            RemoveAt(values, ref count, recordNumber - 1);
                        }
                        else
                        {
                            Console.WriteLine("Помилка: введіть цілий номер.");
                        }
                        Pause();
                        break;
                    case "6":
                        PrintValues(values, count);
                        Pause();
                        break;
                    case "7":
                        Statistics(values, count, out double sum, out double average);
                        FindMaximum(values, count, out double maximum, out int maximumIndex);
                        if (count == 0)
                        {
                            Console.WriteLine("Масив порожній. Сума й середнє дорівнюють нулю.");
                        }
                        else
                        {
                            Console.WriteLine("Сума значень: {0:F2}", sum);
                            Console.WriteLine("Середнє значення: {0:F2}", average);
                            Console.WriteLine("Найбільше значення: {0:F2}, номер: {1}", maximum, maximumIndex + 1);
                        }
                        Pause();
                        break;
                    case "8":
                        ShowPassByValueDemo();
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
            Console.WriteLine("2 — таблиця вартості абонемента");
            Console.WriteLine("3 — таблиця за місяцями й типами абонементів");
            Console.WriteLine("4 — додати числове значення");
            Console.WriteLine("5 — видалити значення за номером");
            Console.WriteLine("6 — показати масив");
            Console.WriteLine("7 — показати статистику");
            Console.WriteLine("8 — дослід передавання за значенням і посиланням");
            Console.WriteLine("0 — вихід");
            Console.Write("Ваш вибір: ");
        }
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

        // Додає значення в спільний масив і змінює лічильник через ref.
        static void AddValue(double[] values, ref int count, double newValue, int capacity)
        {
            if (count >= capacity)
            {
                Console.WriteLine("Місця більше немає.");
                return;
            }

            values[count] = newValue;
            count++;
            Console.WriteLine("Значення додано.");
        }

        // Видаляє елемент і зсуває решту вліво; неправильний номер нічого не змінює.
        static void RemoveAt(double[] values, ref int count, int index)
        {
            if (index < 0 || index >= count)
            {
                Console.WriteLine("Неправильний номер. Масив не змінено.");
                return;
            }

            for (int i = index; i < count - 1; i++)
            {
                values[i] = values[i + 1];
            }

            count--;
            values[count] = 0;
            Console.WriteLine("Значення видалено.");
        }

        // Виводить лише зайняту частину масиву з номерами записів.
        static void PrintValues(double[] values, int count)
        {
            if (count == 0)
            {
                Console.WriteLine("Масив порожній.");
                return;
            }

            Console.WriteLine("{0,6}{1,16}", "Номер", "Значення");
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine("{0,6}{1,16:F2}", i + 1, values[i]);
            }
        }

        // Повертає суму й середнє через out-параметри.
        static void Statistics(double[] values, int count, out double sum, out double average)
        {
            sum = 0;
            for (int i = 0; i < count; i++)
            {
                sum += values[i];
            }

            average = count > 0 ? sum / count : 0;
        }

        // Повертає максимум та його індекс; для порожнього масиву індекс дорівнює -1.
        static void FindMaximum(double[] values, int count, out double maximum, out int index)
        {
            if (count <= 0)
            {
                maximum = 0;
                index = -1;
                return;
            }

            maximum = values[0];
            index = 0;
            for (int i = 1; i < count; i++)
            {
                if (values[i] > maximum)
                {
                    maximum = values[i];
                    index = i;
                }
            }
        }

        // Демонструє, що int копіюється, а елемент переданого масиву змінюється назовні.
        static void ShowPassByValueDemo()
        {
            int number = 5;
            double[] sample = { 50, 70, 90 };

            Console.WriteLine("До викликів: число = {0}; перший елемент масиву = {1:F2}.", number, sample[0]);
            ChangeNumberWithoutRef(number);
            ChangeFirstArrayValue(sample);
            Console.WriteLine("Після викликів: число = {0}; перший елемент масиву = {1:F2}.", number, sample[0]);
        }

        static void ChangeNumberWithoutRef(int number)
        {
            number = 999;
        }

        static void ChangeFirstArrayValue(double[] values)
        {
            if (values.Length > 0)
            {
                values[0] += 100;
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


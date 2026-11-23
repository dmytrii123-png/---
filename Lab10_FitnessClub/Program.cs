using System;
using System.Globalization;
using System.Text;

namespace FitnessClubApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            const int Capacity = 100;
            int[] ids = new int[Capacity];
            string[] clientSurnames = new string[Capacity];
            int[] typeIds = new int[Capacity];
            DateTime[] startDates = new DateTime[Capacity];
            int[] durationsMonths = new int[Capacity];
            double[] prices = new double[Capacity];
            bool[] isActive = new bool[Capacity];
            int count = 0;
            int nextId = 1;
            bool demoDataAdded = false;

            // Зберігаємо попередню вправу лабораторної 9 на масиві double.
            double[] values = new double[100];
            int valueCount = 0;

            bool running = true;
            while (running)
            {
                ShowMenu();
                Console.Write("Ваш вибір: ");
                string choice = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ShowSubscriptions(ids, clientSurnames, typeIds, startDates, durationsMonths, prices, isActive, count);
                        Pause();
                        break;
                    case "2":
                        AddSubscription(ids, clientSurnames, typeIds, startDates, durationsMonths, prices, isActive,
                            Capacity, ref count, ref nextId);
                        Pause();
                        break;
                    case "3":
                        FillSampleData(ids, clientSurnames, typeIds, startDates, durationsMonths, prices, isActive,
                            Capacity, ref count, ref nextId, ref demoDataAdded);
                        Pause();
                        break;
                    case "4":
                        int expiringCount = CountExpiringSubscriptions(startDates, durationsMonths, isActive, count);
                        Console.WriteLine("Абонементів, строк яких завершується протягом наступних 7 днів: {0}.", expiringCount);
                        Pause();
                        break;
                    case "5":
                        ShowCostTable();
                        Pause();
                        break;
                    case "6":
                        ShowPriceTable();
                        Pause();
                        break;
                    case "7":
                        ManageValues(values, ref valueCount);
                        break;
                    case "8":
                        ShowAbout();
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
            Console.WriteLine("1 — показати всі абонементи");
            Console.WriteLine("2 — додати абонемент");
            Console.WriteLine("3 — додати 6 демонстраційних записів");
            Console.WriteLine("4 — підсумок: абонементи, що завершуються протягом тижня");
            Console.WriteLine("5 — таблиця вартості за тривалістю");
            Console.WriteLine("6 — таблиця вартості за місяцями й типами");
            Console.WriteLine("7 — попередня вправа з масивом double (лабораторна 9)");
            Console.WriteLine("8 — довідка про програму");
            Console.WriteLine("0 — вихід");
        }

        static void ShowAbout()
        {
            Console.WriteLine("Наскрізний проєкт №3: Фітнес-клуб.");
            Console.WriteLine("Головна сутність: абонемент.");
            Console.WriteLine("Поля: номер; прізвище клієнта; код типу абонемента; дата початку;");
            Console.WriteLine("кількість місяців; ціна; ознака активності.");
            Console.WriteLine("Код типу поки вводиться числом вручну; довідник буде додано пізніше.");
            Console.WriteLine("Підсумок: кількість активних абонементів, строк яких завершується за 7 днів.");
        }

        // Виводить лише заповнені записи, перебираючи масиви до count.
        static void ShowSubscriptions(int[] ids, string[] clientSurnames, int[] typeIds,
            DateTime[] startDates, int[] durationsMonths, double[] prices, bool[] isActive, int count)
        {
            if (count == 0)
            {
                Console.WriteLine("Абонементів поки немає.");
                return;
            }

            Console.WriteLine("{0,4}{1,-24}{2,9}{3,12}{4,9}{5,14}{6,11}",
                "No", "Прізвище", "Тип ID", "Початок", "Міс.", "Ціна, грн", "Активний");

            for (int i = 0; i < count; i++)
            {
                string activeText = isActive[i] ? "так" : "ні";
                Console.WriteLine("{0,4}{1,-24}{2,9}{3,12}{4,9}{5,14:F2}{6,11}",
                    ids[i], clientSurnames[i], typeIds[i], startDates[i].ToString("dd.MM.yyyy"),
                    durationsMonths[i], prices[i], activeText);
            }

            Console.WriteLine("Записів: {0}.", count);
        }

        // Перевіряє місткість, збирає кожне поле окремо й передає готовий запис на збереження.
        static void AddSubscription(int[] ids, string[] clientSurnames, int[] typeIds,
            DateTime[] startDates, int[] durationsMonths, double[] prices, bool[] isActive,
            int capacity, ref int count, ref int nextId)
        {
            if (count >= capacity)
            {
                Console.WriteLine("Сховище заповнене. Новий абонемент додати не можна.");
                return;
            }

            string surname = ReadText("Прізвище клієнта: ");
            int typeId = ReadInt("Код типу абонемента (поки вручну, 1–1000): ", 1, 1000);
            DateTime startDate = ReadDate("Дата початку (дд.мм.рррр): ");
            int months = ReadInt("Тривалість у місяцях (1–60): ", 1, 60);
            double price = ReadDouble("Ціна абонемента у гривнях (0,01–1000000): ", 0.01, 1000000);
            bool active = ReadBool("Абонемент активний? (так/ні): ");

            int newId;
            bool stored = StoreSubscription(ids, clientSurnames, typeIds, startDates, durationsMonths,
                prices, isActive, capacity, ref count, ref nextId, surname, typeId, startDate,
                months, price, active, out newId);

            if (stored)
            {
                Console.WriteLine("Додано абонемент №{0}.", newId);
            }
            else
            {
                Console.WriteLine("Сховище заповнене. Абонемент не додано.");
            }
        }

        // Записує усі паралельні поля в один індекс і лише після цього збільшує лічильник.
        static bool StoreSubscription(int[] ids, string[] clientSurnames, int[] typeIds,
            DateTime[] startDates, int[] durationsMonths, double[] prices, bool[] isActive,
            int capacity, ref int count, ref int nextId, string surname, int typeId,
            DateTime startDate, int months, double price, bool active, out int assignedId)
        {
            if (count >= capacity)
            {
                assignedId = 0;
                return false;
            }

            assignedId = nextId;
            ids[count] = assignedId;
            clientSurnames[count] = surname;
            typeIds[count] = typeId;
            startDates[count] = startDate;
            durationsMonths[count] = months;
            prices[count] = price;
            isActive[count] = active;
            count++;
            nextId++;
            return true;
        }

        // Додає шість прикладів без введення з клавіатури; дати рахуються від сьогодні.
        static void FillSampleData(int[] ids, string[] clientSurnames, int[] typeIds,
            DateTime[] startDates, int[] durationsMonths, double[] prices, bool[] isActive,
            int capacity, ref int count, ref int nextId, ref bool demoDataAdded)
        {
            if (demoDataAdded)
            {
                Console.WriteLine("Демонстраційні записи вже додані.");
                return;
            }

            const int sampleCount = 6;
            if (capacity - count < sampleCount)
            {
                Console.WriteLine("Для шести демонстраційних записів бракує місця.");
                return;
            }

            DateTime today = DateTime.Today;
            int assignedId;
            StoreSubscription(ids, clientSurnames, typeIds, startDates, durationsMonths, prices, isActive,
                capacity, ref count, ref nextId, "Бондаренко", 1, today.AddDays(-29), 1, 1200, true, out assignedId);
            StoreSubscription(ids, clientSurnames, typeIds, startDates, durationsMonths, prices, isActive,
                capacity, ref count, ref nextId, "Коваленко", 2, today.AddDays(-27), 1, 1800, true, out assignedId);
            StoreSubscription(ids, clientSurnames, typeIds, startDates, durationsMonths, prices, isActive,
                capacity, ref count, ref nextId, "Мельник", 1, today.AddMonths(-3).AddDays(2), 3, 3200, true, out assignedId);
            StoreSubscription(ids, clientSurnames, typeIds, startDates, durationsMonths, prices, isActive,
                capacity, ref count, ref nextId, "Шевченко", 3, today.AddMonths(-6), 12, 6700, true, out assignedId);
            StoreSubscription(ids, clientSurnames, typeIds, startDates, durationsMonths, prices, isActive,
                capacity, ref count, ref nextId, "Петренко", 2, today.AddMonths(-1), 6, 4100, true, out assignedId);
            StoreSubscription(ids, clientSurnames, typeIds, startDates, durationsMonths, prices, isActive,
                capacity, ref count, ref nextId, "Ткаченко", 3, today.AddDays(-29), 1, 900, false, out assignedId);

            demoDataAdded = true;
            Console.WriteLine("Додано 6 демонстраційних абонементів.");
        }

        // Повертає кількість активних абонементів, що завершуються від сьогодні до сьомого дня включно.
        static int CountExpiringSubscriptions(DateTime[] startDates, int[] durationsMonths,
            bool[] isActive, int count)
        {
            DateTime today = DateTime.Today;
            DateTime lastDay = today.AddDays(7);
            int expiringCount = 0;

            for (int i = 0; i < count; i++)
            {
                if (!isActive[i])
                {
                    continue;
                }

                DateTime endDate = startDates[i].AddMonths(durationsMonths[i]).Date;
                if (endDate >= today && endDate <= lastDay)
                {
                    expiringCount++;
                }
            }

            return expiringCount;
        }

        static string ReadText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string value = (Console.ReadLine() ?? "").Trim();
                if (value.Length > 0)
                {
                    return value;
                }

                Console.WriteLine("Помилка: поле не може бути порожнім.");
            }
        }

        static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                int value;
                if (int.TryParse(input, out value) && value >= min && value <= max)
                {
                    return value;
                }

                Console.WriteLine("Помилка: введіть ціле число від {0} до {1}.", min, max);
            }
        }

        static double ReadDouble(string prompt, double min, double max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                double value;
                if (double.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out value)
                    && !double.IsNaN(value) && !double.IsInfinity(value) && value >= min && value <= max)
                {
                    return value;
                }

                Console.WriteLine("Помилка: введіть число від {0} до {1}.", min, max);
            }
        }

        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                DateTime value;
                if (DateTime.TryParseExact(input.Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out value))
                {
                    return value;
                }

                Console.WriteLine("Помилка: дата має бути у форматі дд.мм.рррр.");
            }
        }

        static bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
                if (input == "так" || input == "т")
                {
                    return true;
                }
                if (input == "ні" || input == "н")
                {
                    return false;
                }

                Console.WriteLine("Помилка: введіть «так» або «ні».");
            }
        }

        static void ShowCostTable()
        {
            double from = ReadDouble("Початок проміжку в місяцях: ", 0.1, 120);
            double to = ReadDouble("Кінець проміжку в місяцях: ", from, 120);
            double step = ReadDouble("Додатний крок: ", 0.1, 120);
            double intervals = (to - from) / step;

            if (intervals < 7 - 1e-9 || intervals > 999)
            {
                Console.WriteLine("Задайте проміжок щонайменше на 8 рядків і не більше ніж на 1000.");
                return;
            }

            double monthlyPrice = ReadDouble("Ціна за місяць у гривнях: ", 0.01, 1000000);
            PrintCostTable(from, to, step, monthlyPrice);
        }

        static void PrintCostTable(double from, double to, double step, double monthlyPrice)
        {
            Console.WriteLine("Знижка 5% діє для абонементів від 3 місяців.");
            Console.WriteLine("{0,10}{1,22}{2,24}", "Місяців", "Без знижки, грн", "Зі знижкою, грн");

            for (double months = from; months <= to + 1e-9; months += step)
            {
                double fullPrice = CalculateCost(months, monthlyPrice);
                double discountedPrice = CalculateDiscountedCost(months, monthlyPrice);
                Console.WriteLine("{0,10:F1}{1,22:F2}{2,24:F2}", months, fullPrice, discountedPrice);
            }
        }

        static double CalculateCost(double months, double monthlyPrice)
        {
            return months * monthlyPrice;
        }

        static double CalculateDiscountedCost(double months, double monthlyPrice)
        {
            double fullPrice = CalculateCost(months, monthlyPrice);
            return months >= 3 ? fullPrice * 0.95 : fullPrice;
        }

        static void ShowPriceTable()
        {
            int rows = ReadInt("Кількість рядків (місяців, 1–20): ", 1, 20);
            int columns = ReadInt("Кількість стовпців (типів, 1–20): ", 1, 20);
            double[] columnTotals = new double[columns];
            double grandTotal = 0;

            Console.WriteLine("Умовний тариф: тип 1 — 500 грн/місяць; кожен наступний тип дорожчий на 250 грн.");
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

        static void ManageValues(double[] values, ref int count)
        {
            bool managing = true;
            while (managing)
            {
                Console.WriteLine("=== Масив double (лабораторна 9) ===");
                Console.WriteLine("1 — додати значення");
                Console.WriteLine("2 — видалити значення за номером");
                Console.WriteLine("3 — показати значення");
                Console.WriteLine("4 — сума, середнє та максимум");
                Console.WriteLine("5 — дослід передавання за значенням і масиву");
                Console.WriteLine("0 — повернутися до головного меню");
                Console.Write("Ваш вибір: ");
                string choice = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        double newValue = ReadDouble("Числове значення: ", -1000000000, 1000000000);
                        AddValue(values, ref count, newValue, values.Length);
                        Pause();
                        break;
                    case "2":
                        if (count == 0)
                        {
                            Console.WriteLine("Масив порожній.");
                        }
                        else
                        {
                            int number = ReadInt("Номер значення для видалення: ", 1, count);
                            RemoveAt(values, ref count, number - 1);
                        }
                        Pause();
                        break;
                    case "3":
                        PrintValues(values, count);
                        Pause();
                        break;
                    case "4":
                        Statistics(values, count, out double sum, out double average);
                        FindMaximum(values, count, out double maximum, out int maximumIndex);
                        if (count == 0)
                        {
                            Console.WriteLine("Масив порожній. Сума й середнє дорівнюють нулю.");
                        }
                        else
                        {
                            Console.WriteLine("Сума: {0:F2}; середнє: {1:F2}.", sum, average);
                            Console.WriteLine("Максимум: {0:F2}, номер: {1}.", maximum, maximumIndex + 1);
                        }
                        Pause();
                        break;
                    case "5":
                        ShowPassByValueDemo();
                        Pause();
                        break;
                    case "0":
                        managing = false;
                        break;
                    default:
                        Console.WriteLine("Немає такого пункту.");
                        Pause();
                        break;
                }
            }
        }

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

        static void Statistics(double[] values, int count, out double sum, out double average)
        {
            sum = 0;
            for (int i = 0; i < count; i++)
            {
                sum += values[i];
            }

            average = count > 0 ? sum / count : 0;
        }

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

        static void ShowPassByValueDemo()
        {
            int number = 5;
            double[] sample = { 50, 70, 90 };
            Console.WriteLine("До: число = {0}; перший елемент = {1:F2}.", number, sample[0]);
            ChangeNumberWithoutRef(number);
            ChangeFirstArrayValue(sample);
            Console.WriteLine("Після: число = {0}; перший елемент = {1:F2}.", number, sample[0]);
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

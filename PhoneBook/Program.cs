// Подключаем пространства имён.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Создаем новый класс Abonent с ключевым словом "public".
public class Abonent
{
    public string PhoneNumber { get; set; }
    public string NameAbonent { get; set; }
    public Abonent(string name, string phoneNumber)
    {
        PhoneNumber = phoneNumber;
        NameAbonent = name;
    }
    // Переопределелим стандартый метод ToString() в удобный для нас вывод данных.
    public override string ToString()
    {
        return $"{NameAbonent} : {PhoneNumber}";
    }
}

// Создадим нерасширяемый класс Phonebook.
// Данные будут храниться в файле phonebook.txt в формате Имя : номер.
public sealed class Phonebook
{
    private static readonly Lazy<Phonebook> instance = new Lazy<Phonebook>(() => new Phonebook());
    private readonly List<Abonent> abonents;
    private readonly string filePath = "phonebook.txt";

    public static Phonebook Instance => instance.Value;

    private Phonebook()
    {
        abonents = new List<Abonent>();
        LoadFromFile();
    }

    // Сделаем метод по добавление абонента с проверкой на дублирование абонента.
    public void AddAbonent(string name, string phoneNumber)
    {
        if (abonents.Any(a => a.NameAbonent.Equals(name, StringComparison.OrdinalIgnoreCase) || a.PhoneNumber == phoneNumber))
        {
            Console.WriteLine("Абонент с таким номером или именем уже существует в телефонной книге!");
            return;
        }

        var abonent = new Abonent(name, phoneNumber);
        abonents.Add(abonent);
        Console.WriteLine("Абонент успешно добавлен.");
        SaveToFile();
    }

    // Сделаем метод по удаление абонента.
    public void DeleteAbonent(string phoneNumber)
    {
        var abonent = abonents.FirstOrDefault(a => a.PhoneNumber == phoneNumber);
        if (abonent != null)
        {
            abonents.Remove(abonent);
            Console.WriteLine("Абонент успешно удален.");
            SaveToFile();
        }
        else
        {
            Console.WriteLine("Абонент с таким номером не найден.");
        }
    }

    // Сделаем методы по обновлению данных у абонента.
    public Abonent GetAbonentByPhoneNumber(string phoneNumber)
    {
        return abonents.FirstOrDefault(a => a.PhoneNumber == phoneNumber);
    }

    public Abonent GetAbonentByName(string name)
    {
        return abonents.FirstOrDefault(a => a.NameAbonent.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    // Сделаем метод по публикации всей телефонной книги.
    public void DisplayAllAbonents()
    {
        if (abonents.Count == 0)
        {
            Console.WriteLine("\nТелефонная книга пуста.");
            return;
        }

        Console.WriteLine("Список абонентов:");
        foreach (var abonent in abonents)
        {
            Console.WriteLine($"{abonent.NameAbonent} : {abonent.PhoneNumber}");
        }
    }

    // Загрузка телефонной книги из файла.
    private void LoadFromFile()
    {
        if (!File.Exists(filePath)) return;

        try
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length == 2)
                {
                    abonents.Add(new Abonent(parts[0], parts[1]));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке телефонной книги: {ex.Message}");
        }
    }

    // Сохранение телефонной книги в файл.
    private void SaveToFile()
    {
        try
        {
            var lines = abonents.Select(a => a.ToString()).ToArray();
            File.WriteAllLines(filePath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении телефонной книги: {ex.Message}");
        }
    }
}

public class Program
{
    public static void Main()
    {
        var phonebook = Phonebook.Instance;

        while (true)
        {
            Console.WriteLine("\nМеню телефонной книги\n");
            Console.WriteLine("1. Добавить абонента;");
            Console.WriteLine("2. Удалить абонента;");
            Console.WriteLine("3. Найти абонента по номеру;");
            Console.WriteLine("4. Найти номер по имени абонента;");
            Console.WriteLine("5. Показать всех абонентов;");
            Console.WriteLine("6. Выход.\n");
            Console.Write("Выберите действие: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddAbonent(phonebook);
                    break;
                case "2":
                    DeleteAbonent(phonebook);
                    break;
                case "3":
                    FindByPhoneNumber(phonebook);
                    break;
                case "4":
                    FindByName(phonebook);
                    break;
                case "5":
                    phonebook.DisplayAllAbonents();
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Выберите корректный пункт меню.");
                    break;
            }
        }
    }

    private static void AddAbonent(Phonebook phonebook)
    {

        Console.Write("Введите имя абонента: ");
        var name = Console.ReadLine();
        Console.Write("Введите номер телефона: ");
        var phoneNumber = Console.ReadLine();

        // Проверка на ввод пустых значений
        if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Номер телефона и имя не могут быть пустыми!");
            return;
        }

        phonebook.AddAbonent(name, phoneNumber);
    }

    private static void DeleteAbonent(Phonebook phonebook)
    {
        Console.Write("Введите номер телефона для удаления: ");
        var phoneNumber = Console.ReadLine();
        phonebook.DeleteAbonent(phoneNumber);
    }

    private static void FindByPhoneNumber(Phonebook phonebook)
    {
        Console.Write("Введите номер телефона для поиска: ");
        var phoneNumber = Console.ReadLine();
        var abonent = phonebook.GetAbonentByPhoneNumber(phoneNumber);

        if (abonent != null)
        {
            Console.WriteLine($"Найден абонент: {abonent.NameAbonent} - {abonent.PhoneNumber}");
        }
        else
        {
            Console.WriteLine("Абонент с таким номером не найден.");
        }
    }

    private static void FindByName(Phonebook phonebook)
    {
        Console.Write("Введите имя абонента для поиска: ");
        var name = Console.ReadLine();
        var abonent = phonebook.GetAbonentByName(name);

        if (abonent != null)
        {
            Console.WriteLine($"Найден номер телефона: {abonent.NameAbonent} - {abonent.PhoneNumber}");
        }
        else
        {
            Console.WriteLine("Абонент с таким именем не найден.");
        }
    }
}
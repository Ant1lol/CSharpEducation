// Система учета сотрудников. Employee accounting system
// Создайте консольное приложение, которое позволит управлять данными сотрудников. Программа должна обеспечивать функционал добавления, обновления, получения информации о работниках и расчета их зарплаты.
// Подключаем пространства имён.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Создадим пользовательский класс исключения для обработки специфических ошибок при указании возраста и оклада сотрудника 
public class InvalidEmployeeDataException : Exception
{
    public InvalidEmployeeDataException(string message) : base(message) { }
}
// Создаем новый класс Employee с ключевым словом "public".
public class Employee
{
    public int TabID { get; set; } // Табельный номер
    public string NameEmployee { get; set; } // ФИО сотрудника
    public int AgeEmployee { get; set; } // Возраст сотрудника
    public int SalaryEmployee { get; set; } // Оклад сотрудника
    public int BonusEmployee { get; set; } // Премия сотрудника
    public double TotalSalary { get; set; } // Зарплата сотрудника

    public Employee(int id, string name, int age, int salary, int bonus, double ratio)
    {
        TabID = id;
        NameEmployee = name;
        AgeEmployee = age;
        SalaryEmployee = salary;
        BonusEmployee = bonus;
        TotalSalary = salary + bonus + (salary*ratio);

    }
    // Переопределелим стандартный метод ToString() в удобный для нас вывод данных.
    public override string ToString()
    {
        return $"{TabID};{NameEmployee};{AgeEmployee};{SalaryEmployee};{BonusEmployee};{TotalSalary:F2}";
    }

}

// Создадим нерасширяемый класс PAS (система учета сотрудников, Personnel Accounting System).
// Данные будут храниться в файле PAS.txt в формате Табельный номер ; ФИО ; Возраст ; Оклад ; Премия ; Зарплата.
public sealed class PAS
{
    private static readonly Lazy<PAS> instance = new Lazy<PAS>(() => new PAS());
    private readonly List<Employee> staff;
    private readonly string filePath = "PAS.txt";
    private readonly double ratio = 0.13; // Уральский коэффициент для расчета зарплаты
    public static PAS Instance => instance.Value;

    private PAS()
    {
        staff = new List<Employee>();
        LoadFromFile();
    }
    
    // Напишем метод для проверки корректности возраста и оклада
    
    private void ValidateEmployee(int age, int salary)
    {
        if (age < 18 || age > 70)
            throw new InvalidEmployeeDataException("Возраст должен быть от 18 до 70 лет");

        if (salary < 10000)
            throw new InvalidEmployeeDataException("Зарплата не может быть меньше 10000");
    }

    // Сделаем метод по добавлению сотрудника с проверкой на дублирование табельного номера.
    public void AddEmployee(int id, string name, int age, int salary, int bonus)
    {
        try
        {
            if (staff.Any(a => a.TabID == id))
            {
                Console.WriteLine("\nСотрудник с таким табельным номером уже существует в базе данных!");
                return;
            }

            // Добавим валидацию возраста и оклада
            if (age < 18 || age > 70)
                throw new InvalidEmployeeDataException("Возраст должен быть от 18 до 70 лет");

            ValidateEmployee(age, salary);

            var employee = new Employee(id, name, age, salary, bonus, ratio);
            staff.Add(employee);
            Console.WriteLine("\nСотрудник успешно добавлен.");
            SaveToFile();
            
        }
        catch (InvalidEmployeeDataException ex)
        {
            Console.WriteLine($"Ошибка валидации: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении сотрудника: {ex.Message}");
        }
    }
    // Сделаем метод по удалению сотрудника.
    public void DeleteEmployee(int id)
    {
        var employee = staff.FirstOrDefault(a => a.TabID == id);
        if (employee != null)
        {
            staff.Remove(employee);
            Console.WriteLine("\nСотрудник успешно удален.");
            SaveToFile();
        }
        else
        {
            Console.WriteLine("\nСотрудник с таким табельным номером не найден.");
        }
    }

    // Сделаем методы по обновлению данных у сотрудника.
    public Employee GetEmployeetByTabID(int id)
    {
        return staff.FirstOrDefault(a => a.TabID == id);
    }
    public Employee GetEmployeetByName(string name)
    {
        return staff.FirstOrDefault(a => a.NameEmployee.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
    public Employee GetEmployeetBySalary(int salary)
    {
        return staff.FirstOrDefault(a => a.SalaryEmployee == salary);
    }
    public Employee GetEmployeetByBonus(int bonus)
    {
        return staff.FirstOrDefault(a => a.BonusEmployee == bonus);
    }

    // Сделаем метод по публикации всех сотрудников из файла.
    public void DisplayAllEmployee()
    {
        if (staff.Count == 0)
        {
            Console.WriteLine("\nБаза данных сотрудников пуста.");
            return;
        }

        Console.WriteLine("\nСписок всех сотрудников.");
        foreach (var employee in staff)
        {
            Console.WriteLine($"Табельный номер {employee.TabID}; ФИО {employee.NameEmployee};" +
                              $" Возраст {employee.AgeEmployee}; Оклад {employee.SalaryEmployee};" +
                              $" Премия {employee.BonusEmployee}; Зарплата: {employee.TotalSalary:F2}");
        }
    }

    // Загрузка всех сотрудников из файла.
    private void LoadFromFile()
    {
        try
        {
            // Проверяем существование файла с данными на локальном носителе
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, ""); // Создаем пустой файл, если его не существует
                return;
            }
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 6 &&
                    int.TryParse(parts[0], out int id)
                    && int.TryParse(parts[2], out int age)
                    && int.TryParse(parts[3], out int salary)
                    && int.TryParse(parts[4], out int bonus)
                    && double.TryParse(parts[5], out double totalSalary)
                    )
                {
                    var employee = new Employee(id, parts[1], age, salary, bonus, ratio);
                    employee.TotalSalary = totalSalary;
                    staff.Add(employee);
                }
            }
        }

        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Файл данных не найден: {ex.FileName}. Будет создан новый файл.");
            File.WriteAllText(filePath, ""); // Создаем новый файл
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Ошибка ввода-вывода при работе с файлом: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Нет доступа к файлу на локальном диске: {ex.Message}");
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке данных из файла: {ex.Message}");
        }
    }

    // Сохранение данных в файл.
    private void SaveToFile()
    {
        try
        {
            var lines = staff.Select(a => a.ToString()).ToArray();
            File.WriteAllLines(filePath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
        }
    }
}

public class Program
{
    public static void Main()
    {
        var pas = PAS.Instance;

        while (true)
        {
            Console.WriteLine("\nМеню системы учета сотрудников.");
            Console.WriteLine("1. Добавить сотрудника;");
            Console.WriteLine("2. Удалить сотрудника;");
            Console.WriteLine("3. Найти ФИО сотрудника по табельному номеру;");
            Console.WriteLine("4. Найти табельный номер по ФИО сотрудника;");
            Console.WriteLine("5. Показать всех сотрудников;");
            Console.WriteLine("6. Выход.\n");
            Console.Write("Выберите необходимое действие: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddEmployee(pas);
                    break;
                case "2":
                    DeleteEmployee(pas);
                    break;
                case "3":
                    FindByTabID(pas);
                    break;
                case "4":
                    FindByName(pas);
                    break;
                case "5":
                    pas.DisplayAllEmployee();
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Выберите корректный пункт меню.");
                    break;
            }
        }
    }

    private static void AddEmployee(PAS pas)
    {
        // Считываем данные и проверяем на ввод пустых и неверных значений
        Console.Write("\nВведите табельный номер сотрудника: ");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id) || id <= 0)
        {
            Console.WriteLine("Табельный номер должен быть положительным числом!");
            Console.Write("\nУкажите табельный номер: ");
        }

        Console.Write("\nВведите ФИО: ");
        var name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("ФИО сотрудника не может быть пустым!");
            return;
        }

        int age;
        while (true)
        {
            Console.Write("\nУкажите возраст: ");
            if (!int.TryParse(Console.ReadLine(), out age))
            {
                Console.WriteLine("Возраст должен быть числом!");
                continue;
            }

            try
            {
                if (age < 18 || age > 70)
                    throw new InvalidEmployeeDataException("Возраст должен быть от 18 до 70 лет");
                break; // Если валидация прошла успешно, выходим из цикла
            }
            catch (InvalidEmployeeDataException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        Console.Write("\nВведите оклад сотрудника: ");
        int salary;
        while (!int.TryParse(Console.ReadLine(), out salary) || salary <= 0)
        {
            Console.WriteLine("Оклад должен быть положительным числом!");
            Console.Write("\nВведите оклад сотрудника: ");
        }

        Console.Write("\nУкажите премию сотрудника: ");
        int bonus;
        while (!int.TryParse(Console.ReadLine(), out bonus) || bonus < 0)
        {
            Console.WriteLine("Размер премии должен быть положительным числом!");
            Console.Write("\nУкажите премию сотрудника: ");
        }

        pas.AddEmployee(id, name, age, salary, bonus);
    }

    private static void DeleteEmployee(PAS pas)
    {
        Console.Write("\nВведите табельный номер для удаления: ");
        int id;
        int.TryParse(Console.ReadLine(), out id);
        pas.DeleteEmployee(id);
    }

    private static void FindByTabID(PAS pas)
    {
        Console.Write("\nВведите табельный номер для поиска: ");
        int id;
        int.TryParse(Console.ReadLine(), out id);
        var employee = pas.GetEmployeetByTabID(id);

        if (employee != null)
        {
            Console.WriteLine($"\nНайден сотрудник: {employee.NameEmployee} с табельным номером {employee.TabID}");
        }
        else
        {
            Console.WriteLine("\nСотрудник с таким табельным номером не найден в системе.");
        }
    }

    private static void FindByName(PAS pas)
    {
        Console.Write("\nВведите ФИО сотрудника для поиска: ");
        var name = Console.ReadLine();
        var employee = pas.GetEmployeetByName(name);

        if (employee != null)
        {
            Console.WriteLine($"\nНайден табельный номер по {employee.NameEmployee} - {employee.TabID}");
        }
        else
        {
            Console.WriteLine("\nСотрудник с таким ФИО не найден.");
        }
    }
}
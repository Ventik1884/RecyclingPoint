using WebApp.Models;
using System;
using System.Linq;

namespace WebApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(RecPointContext db)
        {
            db.Database.EnsureCreated();

            if(db.Positions.Any())
            {
                return;
            }

            const int positionsNumber = 25;
            const int storageTypesNumber = 25;
            const int recyclableTypesNumber = 25;
            const int employeesNumber = 200;
            const int storagesNumber = 200;
            const int acceptedRecyclableNumber = 200;

            Random random = new Random(33);

            string[] positions =
            {
                "Уборщик",
                "Столяр",
                "Электрик",
                "Программист",
                "Начальник",
                "Секретарь",
                "Консультант"
            };

            // Заполнение таблицы должностей
            for (int i = 0; i < positionsNumber; i++)
            {
                string positionName = positions[random.Next(positions.Length)] 
                    + random.Next(positions.Length).ToString();
                Position position = new() 
                { 
                    Name = positionName 
                };
                db.Positions.Add(position);
            }
            db.SaveChanges();

            string[] storageTypes =
{
                "Производственный",
                "Таможенный",
                "Розничный",
                "Резервный",
                "Транзитно-перевалочный",
                "Распределительный"
            };

            // Заполнение таблицы видов складов
            for (int i = 0; i < storageTypesNumber; i++)
            {
                string storageTypeName = storageTypes[random.Next(storageTypes.Length)] + 
                    random.Next(storageTypes.Length).ToString();
                int temperature = random.Next(25);
                int humidity = random.Next(10, 70);
                string requirement = "Требования пожарной безопасности" + random.Next(25);
                string equipment = "Оборудование" + random.Next(25);

                StorageType storageType = new()
                {
                    Name = storageTypeName,
                    Temperature = temperature,
                    Humidity = humidity,
                    Requirement = requirement,
                    Equipment = equipment
                };
                db.StorageTypes.Add(storageType);
            }
            db.SaveChanges();

            string[] recyclableTypes =
            {
                "Дерево",
                "Текстиль",
                "Стекло",
                "Металлолом",
                "Бумага",
                "Пластмасса",
                "Резина"
            };

            // Заполнение таблицы типов вторсырья
            for (int i = 0; i < recyclableTypesNumber; i++)
            {
                string recType = recyclableTypes[random.Next(recyclableTypes.Length)]
                    + random.Next(recyclableTypes.Length).ToString();
                double price = random.NextDouble() * 100 + 1;
                string description = "Описание" + random.Next(50);
                string acceptanceCondition = "Условия принятия" + random.Next(25);
                string storageCondition = "Условия хранения" + random.Next(25);

                RecyclableType recyclableType = new() 
                { 
                    Name = recType, 
                    Price = price,
                    Description = description,
                    AcceptanceCondition = acceptanceCondition,
                    StorageCondition = storageCondition
                };
                db.RecyclableTypes.Add(recyclableType);
            }
            db.SaveChanges();

            string[] menNames =
{
                "Иван",
                "Федор",
                "Петр",
                "Виктор",
                "Степан",
                "Вечеслав",
                "Артем",
                "Александр"
            };
            string[] womanNames =
            {
                "Мария",
                "Анна",
                "Дарья",
                "Валерия",
                "Диана",
                "Наталья",
                "Владислава",
                "Виктория",
                "Ксения",
                "Вечеслава",
                "Ульяна",
                "Екатерина",
                "Светлана",
                "Анастасия",
                "Александра",
                "Евгения"
            };
            string[] surnames =
            {
                "Заяц",
                "Дрозд",
                "Пикун",
                "Манько",
                "Довбань",
                "Петренко",
                "Иваненко",
                "Емельяненко",
                "Дмитриенко",
                "Зайченко",
                "Гоголь",
                "Громыко"
            };

            for(int i = 0; i < employeesNumber / 2; i++)
            {
                string employeeName = menNames[random.Next(menNames.Length)];
                string employeeSurname = surnames[random.Next(surnames.Length)];
                string employeePatronymic = menNames[random.Next(menNames.Length)] + "ович";
                int experience = random.Next(35);
                int positionId = db.Positions
                    .Select(p => p.Id).ToList()[random.Next(db.Positions.Count())];

                Employee employee = new()
                {
                    Name = employeeName,
                    Surname = employeeSurname,
                    Patronymic = employeePatronymic,
                    PositionId = positionId,
                    Experience = experience
                };
                db.Employees.Add(employee);
            }
            db.SaveChanges();
            for (int i = 0; i < employeesNumber / 2; i++)
            {
                string employeeName = womanNames[random.Next(womanNames.Length)];
                string employeeSurname = surnames[random.Next(surnames.Length)];
                string employeePatronymic = menNames[random.Next(menNames.Length)] + "овна";
                int experience = random.Next(35);
                int positionId = db.Positions
                    .Select(p => p.Id).ToList()[random.Next(db.Positions.Count())];

                Employee employee = new()
                {
                    Name = employeeName,
                    Surname = employeeSurname,
                    Patronymic = employeePatronymic,
                    PositionId = positionId,
                    Experience = experience
                };
                db.Employees.Add(employee);
            }
            db.SaveChanges();

            // Заполнение таблицы складов
            for (int i = 0;i < storagesNumber; i++)
            {
                string storageName = "Cклад_" + i.ToString();
                int num = i;
                int square = random.Next(15, 900);
                int capacity = square * random.Next(1, 5);
                int occupancy = (int)(capacity * random.NextDouble());
                int depreciation = random.Next(101);
                DateTime checkDate = DateTime.Now - new TimeSpan(random.Next(10000),0,0,0);
                int storageTypeId = db.StorageTypes
                    .Select(st => st.Id).ToList()[random.Next(db.StorageTypes.Count())];
                Storage storage = new()
                {
                    Name = storageName,
                    Number = num,
                    Square = square,
                    Capacity = capacity,
                    Occupancy = occupancy,
                    Depreciation = depreciation,
                    CheckDate = checkDate,
                    StorageTypeId = storageTypeId
                };
                db.Storages.Add(storage);
            }
            db.SaveChanges();

            // Заполнение таблицы принятого вторсырья
            for(int i = 0;i < acceptedRecyclableNumber;i++)
            {
                int employeeId = db.Employees
                    .Select(e => e.Id).ToList()[random.Next(db.Employees.Count())];
                int storageId = db.Storages
                    .Select(s => s.Id).ToList()[random.Next(db.Storages.Count())];
                int recTypeId = db.RecyclableTypes
                    .Select(rt => rt.Id).ToList()[random.Next(db.RecyclableTypes.Count())];
                int quantity = random.Next(1, 2001);
                DateTime date = DateTime.Now - new TimeSpan(random.Next(3650), 0, 0, 0);
                AcceptedRecyclable acceptedRecyclable = new()
                {
                    EmployeeId = employeeId,
                    StorageId = storageId,
                    RecyclableTypeId = recTypeId,
                    Quantity = quantity,
                    Date = date
                };
                db.AcceptedRecyclables.Add(acceptedRecyclable);
            }
            db.SaveChanges();
        }
    }
}

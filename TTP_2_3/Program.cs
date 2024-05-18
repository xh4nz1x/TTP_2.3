internal class Program
{
    private static void Main(string[] args)
    {
        int userId = 0;
        bool AuthMenuWork = true;
        while (AuthMenuWork)
        {
            int AuthMenu =
                StringToInt(
                    AntiEmptyStringMenu(
                        "1 - Авторизация \n2 - Регистрация \n3 - Выход \n\nВыберите нужное действие: "));
            switch (AuthMenu)
            {
                case 0:
                    break;
                case 1:
                    userId = LoginUser();
                    AuthMenuWork = false;
                    break;
                case 2:
                    userId = RegisterUser();
                    AuthMenuWork = false;
                    break;
                case 3:
                    break;
                default:
                    Console.WriteLine("| Неизвестное действие! |");
                    break;
            }
        }

        if (userId != 0)
        {
            MainMenu(userId);
        }
    }

    private static int LoginUser()
    {
        int userId = 0;
        string username = AntiEmptyString("Введите имя пользователя (string): ");
        string password = AntiEmptyString("Введите пароль (string): ");

        userId = DBRequests.LoginUserQuery(username, password);

        return userId;
    }

    private static int RegisterUser()
    {
        int userId = 0;
        string username = AntiEmptyString("Придумайте имя пользователя (string): ");

        while (true)
        {
            string password = AntiEmptyString("Придумайте надежный пароль (string): ");
            string passwordRepeat = AntiEmptyString("Повторно введите пароль (string): ");

            if (password != passwordRepeat)
            {
                Console.WriteLine("\n| Ошибка: Пароли несовпадают! |");
                continue;
            }

            userId = DBRequests.RegisterUserQuery(username, password);
            if (userId == -1)
            {
                Console.WriteLine("\n| Ошибка: Данный пользователь уже существует! |");
                RegisterUser();
            }
            else
            {
                break;
            }
        }

        return userId;
    }

    private static void MainMenu(int userId)
    {
        Console.WriteLine("\n| Успешная авторизация! |");

        bool mainMenuWork = true;
        while (mainMenuWork)
        {
            int mainMenu =
                StringToInt(
                    AntiEmptyStringMenu(
                        "1 - Просмотреть задачи \n2 - Добавить новую задачу \n3 - Удалить задачу \n4 - Отредактировать задачу \n5 - Выход \n\nВыберите нужное действие: "));
            switch (mainMenu)
            {
                case 0:
                    break;
                case 1:
                    List<DBRequests.Task> allTasks = DBRequests.GetTasksForUserQuery(userId);

                    if (allTasks.Count == 0)
                    {
                        Console.WriteLine("\n| Задачи - отсутствуют! |");
                    }
                    else
                    {
                        bool viewMenuWork = true;
                        while (viewMenuWork)
                        {
                            int viewMenu =
                                StringToInt(
                                    AntiEmptyStringMenu(
                                        "1 - Просмотреть актуальные задачи \n2 - Вывести список всех задач, которые уже прошли \n3 - Вывести все задачи \n4 - Назад \n\nВыберите нужное действие: "));
                            switch (viewMenu)
                            {
                                case 0:
                                    break;
                                case 1:
                                    List<DBRequests.Task> presentTasks = allTasks.FindAll(task =>
                                        DateTime.Now.Date <= task.GetDueDate().Date &&
                                        (task.GetDueDate().Date > DateTime.Now.Date ||
                                         task.GetDueDate().TimeOfDay > DateTime.Now.TimeOfDay));

                                    if (presentTasks.Count == 0)
                                    {
                                        Console.WriteLine("\n| Актуальные задачи - отсутствуют! |");
                                    }
                                    else
                                    {
                                        bool viewPresentMenuWork = true;
                                        while (viewPresentMenuWork)
                                        {
                                            int viewPresentMenu =
                                                StringToInt(
                                                    AntiEmptyStringMenu(
                                                        "1 - Вывести все задачи \n2 - Вывести задачи на сегодня \n3 - Вывести задачи на завтра \n4 - Вывести задачи на неделю \n5 - Назад \n\nВыберите нужное действие: "));
                                            switch (viewPresentMenu)
                                            {
                                                case 0:
                                                    break;
                                                case 1:
                                                    foreach (var task in presentTasks)
                                                    {
                                                        PrintTask(task);
                                                    }

                                                    break;
                                                case 2:
                                                    List<DBRequests.Task> todayTasks = presentTasks.FindAll(task =>
                                                        DateTime.Today == task.GetDueDate().Date &&
                                                        task.GetDueDate().TimeOfDay > DateTime.Now.TimeOfDay);

                                                    if (todayTasks.Count == 0)
                                                    {
                                                        Console.WriteLine("\n| Задачи на сегодня - отсутствуют! |");
                                                    }

                                                    foreach (var task in todayTasks)
                                                    {
                                                        PrintTask(task);
                                                    }

                                                    break;
                                                case 3:
                                                    List<DBRequests.Task> tomorrowTasks = presentTasks.FindAll(task =>
                                                        DateTime.Today.AddDays(1) == task.GetDueDate().Date);

                                                    if (tomorrowTasks.Count == 0)
                                                    {
                                                        Console.WriteLine("\n| Задачи на завтра - отсутствуют! |");
                                                    }

                                                    foreach (var task in tomorrowTasks)
                                                    {
                                                        PrintTask(task);
                                                    }

                                                    break;
                                                case 4:
                                                    List<DBRequests.Task> weekTasks = presentTasks.FindAll(task =>
                                                        DateTime.Today <= task.GetDueDate().Date &&
                                                        task.GetDueDate().Date <= DateTime.Today.AddDays(7));

                                                    if (weekTasks.Count == 0)
                                                    {
                                                        Console.WriteLine("\n| Задачи на завтра - отсутствуют! |");
                                                    }

                                                    foreach (var task in weekTasks)
                                                    {
                                                        PrintTask(task);
                                                    }

                                                    break;
                                                case 5:
                                                    viewPresentMenuWork = false;
                                                    break;
                                                default:
                                                    Console.WriteLine("\n| Неизвестное действие! |");
                                                    break;
                                            }
                                        }
                                    }

                                    break;
                                case 2:
                                    List<DBRequests.Task> pastTasks = allTasks.FindAll(task =>
                                        task.GetDueDate().Date < DateTime.Today ||
                                        (task.GetDueDate().Date == DateTime.Today &&
                                         task.GetDueDate().TimeOfDay < DateTime.Now.TimeOfDay));

                                    if (pastTasks.Count == 0)
                                    {
                                        Console.WriteLine("\n| Задачи, которые уже прошли - отсутствуют! |");
                                    }
                                    else
                                    {
                                        foreach (var task in pastTasks)
                                        {
                                            PrintTask(task);
                                        }
                                    }

                                    break;
                                case 3:
                                    foreach (var task in allTasks)
                                    {
                                        PrintTask(task);
                                    }

                                    break;
                                case 4:
                                    viewMenuWork = false;
                                    break;
                                default:
                                    Console.WriteLine("\n| Неизвестное действие! |");
                                    break;
                            }
                        }
                    }

                    break;
                case 2:
                    string title = AntiEmptyString("Введите название задачи (string): ");
                    string description = AntiEmptyString("Введите описание задачи (string): ");
                    while (true)
                    {
                        string dueDateString = AntiEmptyString("Введите дату и время (в формате ДД.ММ.ГГГГ ЧЧ:ММ): ");

                        if (DateTime.TryParseExact(dueDateString, "dd.MM.yyyy HH:mm", null,
                                System.Globalization.DateTimeStyles.None, out DateTime dueDate))
                        {
                            if (dueDate < DateTime.Now)
                            {
                                Console.WriteLine("\n| Ошибка: дата должна быть не раньше текущей даты! |");
                                continue;
                            }

                            string formattedDueDate = dueDate.ToString("yyyy-MM-dd HH:mm:ss");

                            DBRequests.NewTaskQuery(userId, title, description, formattedDueDate);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\n| Ошибка: невозможно преобразовать данное значение в DateTime! |");
                            continue;
                        }
                    }

                    break;
                case 3:
                    List<DBRequests.Task> listTasks = DBRequests.GetTasksForUserQuery(userId);
                    foreach (var task in listTasks)
                    {
                        PrintTitleAndIdTask(task);
                    }

                    int taskId =
                        StringToInt(
                            AntiEmptyStringMenu(
                                "\nВведите id задачи, которую нужно удалить (int): "));
                    if (taskId == 0)
                    {
                        continue;
                    } 
                    
                    int checkTask = DBRequests.CheckTaskQuery(userId, taskId);
                    if (checkTask == -1)
                    {
                        Console.WriteLine("\n| Ошибка: данная задача - недоступна! |");
                    }
                    else
                    {
                        DBRequests.DeleteTaskQuery(taskId);
                    }

                    break;
                case 4:
                    listTasks = DBRequests.GetTasksForUserQuery(userId);
                    foreach (var task in listTasks)
                    {
                        PrintTitleAndIdTask(task);
                    }

                    taskId =
                        StringToInt(
                            AntiEmptyStringMenu(
                                "\nВведите id задачи, которую нужно отредактировать (int): "));
                    if (taskId == 0)
                    {
                        continue;
                    } 
                    
                    checkTask = DBRequests.CheckTaskQuery(userId, taskId);
                    if (checkTask == -1)
                    {
                        Console.WriteLine("\n| Ошибка: данная задача - недоступна! |");
                    }
                    else
                    {
                        DBRequests.Task task = DBRequests.GetTask(taskId);
                        string titleEdit = task.GetTitle();
                        string descriptionEdit = task.GetDescription();
                        string dueDateEdit = task.GetDueDate().ToString("yyyy-MM-dd HH:mm:ss");
                        
                        bool EditMenuWork = true;
                        while (EditMenuWork)
                        {
                            int EditMenu =
                                StringToInt(
                                    AntiEmptyStringMenu(
                                        "1 - Изменить название задачи \n2 - Изменить описание задачи \n3 - Изменить сроки выполнения задачи \n4 - Назад \n\nВыберите нужное действие: "));
                            switch (EditMenu)
                            {
                                case 0:
                                    break;
                                case 1:
                                    titleEdit = AntiEmptyString("Введите название задачи (string): ");
                                    DBRequests.EditTaskQuery(taskId, titleEdit, descriptionEdit, dueDateEdit);
                                    EditMenuWork = false;
                                    break;
                                case 2:
                                    descriptionEdit = AntiEmptyString("Введите описание задачи (string): ");
                                    DBRequests.EditTaskQuery(taskId, titleEdit, descriptionEdit, dueDateEdit);
                                    EditMenuWork = false;
                                    break;
                                case 3:
                                    while (true)
                                    {
                                        string dueDateString = AntiEmptyString("Введите дату и время (в формате ДД.ММ.ГГГГ ЧЧ:ММ): ");

                                        if (DateTime.TryParseExact(dueDateString, "dd.MM.yyyy HH:mm", null,
                                                System.Globalization.DateTimeStyles.None, out DateTime dueDate))
                                        {
                                            if (dueDate < DateTime.Now)
                                            {
                                                Console.WriteLine("\n| Ошибка: дата должна быть не раньше текущей даты! |");
                                                continue;
                                            }
                                            
                                            dueDateEdit = dueDate.ToString("yyyy-MM-dd HH:mm:ss");
                                            
                                            DBRequests.EditTaskQuery(taskId, titleEdit, descriptionEdit, dueDateEdit);
                                            EditMenuWork = false;
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("\n| Ошибка: невозможно преобразовать данное значение в DateTime! |");
                                        }
                                    }
                                    break;
                                case 4:
                                    EditMenuWork = false;
                                    break;
                                default:
                                    Console.WriteLine("\n| Неизвестное действие! |");
                                    break;
                            }
                        }
                    }

                    break;
                case 5:
                    mainMenuWork = false;
                    break;
                default:
                    Console.WriteLine("\n| Неизвестное действие! |");
                    break;
            }
        }
    }

    public static int StringToInt(string inputStr)
    {
        int input = 0;
        try
        {
            input = Convert.ToInt32(inputStr);
        }

        catch (FormatException)
        {
            Console.WriteLine();
            Console.WriteLine("\n| Ошибка: невозможно преобразовать данное значение в int |");
        }

        return input;
    }

    public static string AntiEmptyString(string inputText)
    {
        string input;
        while (true)
        {
            Console.Write(inputText);
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) break;
        }

        return input;
    }

    public static string AntiEmptyStringMenu(string inputText)
    {
        string input;
        while (true)
        {
            Console.WriteLine();
            Console.Write(inputText);
            input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input)) break;
        }

        return input;
    }

    public static void PrintTask(DBRequests.Task task)
    {
        Console.WriteLine($"\nID задачи: {task.GetTaskId()}");
        Console.WriteLine($"Название: {task.GetTitle()}");
        Console.WriteLine($"Описание: {task.GetDescription()}");
        Console.WriteLine($"Сроки: {task.GetDueDate()}");
    }

    public static void PrintTitleAndIdTask(DBRequests.Task task)
    {
        Console.WriteLine($"\nID задачи: {task.GetTaskId()} | Название: {task.GetTitle()}");
    }
}
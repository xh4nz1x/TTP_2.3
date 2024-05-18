using Npgsql;

public class DBRequests
{
    public static int LoginUserQuery(string username, string password)
    {
        var querySql = $"SELECT userid FROM users " +
                       $"WHERE username = '{username}' AND password = '{password}'";

        using var cmdQuery = new NpgsqlCommand(querySql, DBService.GetSqlConnection());
        var result = cmdQuery.ExecuteScalar();
        if (result != null)
        {
            return Convert.ToInt32(result);
        }

        Console.WriteLine("\n| Ошибка: Неверный логин или пароль! |");

        return 0;
    }

    public static int RegisterUserQuery(string username, string password)
    {
        var querySqlCheckUsername = $"SELECT userid FROM users " +
                                    $"WHERE username = '{username}'";

        using var cmdQuerySqlCheckUsername = new NpgsqlCommand(querySqlCheckUsername, DBService.GetSqlConnection());
        var resultCheckUsername = cmdQuerySqlCheckUsername.ExecuteScalar();
        if (resultCheckUsername == null)
        {
            var querySqlRegistration = $"INSERT INTO users (username, password)" +
                                       $"VALUES " +
                                       $"    ('{username}', '{password}')";

            using var cmdQuerySqlRegistration = new NpgsqlCommand(querySqlRegistration, DBService.GetSqlConnection());
            var resultRegistration = cmdQuerySqlRegistration.ExecuteScalar();

            var querySqlGetId = $"SELECT userid FROM users " +
                                $"WHERE username = '{username}' AND password = '{password}'";

            using var cmdQuerySqlGetId = new NpgsqlCommand(querySqlGetId, DBService.GetSqlConnection());
            var resultGetId = cmdQuerySqlGetId.ExecuteScalar();
            if (resultGetId != null)
            {
                return Convert.ToInt32(resultGetId);
            }

            else
            {
                Console.WriteLine("\n| Неизвестная ошибка! |");
            }
        }
        else
        {
            return -1;
        }

        return 0;
    }

    public static List<Task> GetTasksForUserQuery(int userId)
    {
        List<Task> tasks = new List<Task>();

        var querySql = $"SELECT taskid, userid, title, description, duedate " +
                       $"FROM tasks " +
                       $"WHERE userid = '{userId}'";

        using var cmdQuery = new NpgsqlCommand(querySql, DBService.GetSqlConnection());

        using (NpgsqlDataReader reader = cmdQuery.ExecuteReader())
        {
            while (reader.Read())
            {
                int taskIdRead = reader.GetInt32(0);
                int userIdRead = reader.GetInt32(1);
                string titleRead = reader.GetString(2);
                string descriptionRead = reader.GetString(3);
                DateTime dueDateRead = reader.GetDateTime(4);

                Task task = new Task(taskIdRead, userIdRead, titleRead, descriptionRead, dueDateRead);
                tasks.Add(task);
            }
        }

        return tasks;
    }
    
    public static Task GetTask(int taskId)
    {
        var querySql = $"SELECT title, description, duedate " +
                       $"FROM tasks " +
                       $"WHERE taskid = '{taskId}'";

        using var cmdQuery = new NpgsqlCommand(querySql, DBService.GetSqlConnection());

        using (NpgsqlDataReader reader = cmdQuery.ExecuteReader())
        {
            if (reader.Read())
            {
                string titleRead = reader.GetString(0);
                string descriptionRead = reader.GetString(1);
                DateTime dueDateRead = reader.GetDateTime(2);

                Task task = new Task(titleRead, descriptionRead, dueDateRead);
            
                return task;
            }

            return null;
        }
    }

    public static int CheckTaskQuery(int userId, int taskId)
    {
        var querySqlCheckTask = $"SELECT title FROM tasks " +
                                $"WHERE taskid = '{taskId}' and userid = '{userId}'";

        using var cmdQuerySqlCheckTask = new NpgsqlCommand(querySqlCheckTask, DBService.GetSqlConnection());
        var resultCheckTask = cmdQuerySqlCheckTask.ExecuteScalar();
        if (resultCheckTask == null)
        {
            return -1;
        }

        return 0;
    }
    
    public static void DeleteTaskQuery(int taskId)
    {
        var querySql = $"DELETE FROM tasks " +
                                       $"WHERE taskId = '{taskId}'";

        using var cmdQuery = new NpgsqlCommand(querySql, DBService.GetSqlConnection());
        cmdQuery.ExecuteNonQuery();
        
        Console.WriteLine("\n| Задача - удалена! |");
    }
    
    public static void EditTaskQuery(int taskid, string title, string description, string duedate)
    {
        var querySql = $"UPDATE tasks " +
                                 $"SET title = '{title}', description = '{description}', duedate = '{duedate}' " +
                                 $"WHERE taskid = '{taskid}'";

        using var cmdQuery = new NpgsqlCommand(querySql, DBService.GetSqlConnection());
        cmdQuery.ExecuteNonQuery();
        Console.WriteLine("\n| Задача - отредактирована! |");
    }

    public static void NewTaskQuery(int userid, string title, string description, string duedate)
    {
        var querySql = $"INSERT INTO tasks (userid, title, description, duedate)" +
                       $"VALUES " +
                       $"    ('{userid}', '{title}', '{description}', '{duedate}')";

        using var cmdQuery = new NpgsqlCommand(querySql, DBService.GetSqlConnection());
        cmdQuery.ExecuteNonQuery();

        Console.WriteLine("\n| Задача успешно добавлена! |");
    }

    public class Task
    {
        private int TaskId { get; set; }
        private int UserId { get; set; }
        private string Title { get; set; }
        private string Description { get; set; }
        private DateTime DueDate { get; set; }

        public Task(int taskId, int userId, string title, string description, DateTime dueDate)
        {
            TaskId = taskId;
            UserId = userId;
            Title = title;
            Description = description;
            DueDate = dueDate;
        }

        public Task(string title, string description, DateTime dueDate)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
        }

        public int GetTaskId()
        {
            return TaskId;
        }

        public int GetUserId()
        {
            return UserId;
        }

        public string GetTitle()
        {
            return Title;
        }

        public string GetDescription()
        {
            return Description;
        }

        public DateTime GetDueDate()
        {
            return DueDate;
        }
    }
}
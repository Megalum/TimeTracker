using System;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace TimeTracker
{
    class DataBase
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=DESKTOP-GHPD068\SQLEXPRESS;Initial Catalog=TimeTracker;Integrated Security=True");

        private static SqlCommand ExecuteCommand;

        public void openConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }

        public void closeConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

        public SqlConnection getConnection()
        {
            return sqlConnection;
        }

        public SqlDataReader GetElementsList() 
        {
            openConnection();
            string Query = $"SELECT * FROM Tracker ORDER BY Day_Of_Completion ASC, Time_Start ASC";
            ExecuteCommand = new SqlCommand(Query, getConnection());
            return ExecuteCommand.ExecuteReader();
        }

        public async Task<bool> EndStartlementTaskAsync(string id, string status, string start, string end)
        {
            string timeCondition = string.IsNullOrEmpty(end) ? "Time_Start" : "Time_Stop";
            string timeValue = string.IsNullOrEmpty(end) ? start : end;
            string Query = $"UPDATE Tracker SET {timeCondition} = \'{timeValue}\', " +
                              $"Status_Task = \'{status}\'" +
                              $"WHERE id = \'{id}\'";
            return await ExecuteQueryAsync(Query);
        }

        public async Task<bool> ChangesStatus(string id)
        {
            string Query = $"UPDATE Tracker SET Status_Task = \'Пропущено\'" +
                            $"WHERE id = \'{id}\'";
            return await ExecuteQueryAsync(Query);
        }
        public async Task<bool> EndChangesStatus(string id)
        {
            string Query = $"UPDATE Tracker SET Status_Task = \'Выполнено\', " +
                            $"Time_Stop = \'23:59:59\' " +
                            $"WHERE id = \'{id}\'";
            return await ExecuteQueryAsync(Query);
        }

        public async Task<bool> UpdateElementAsync(string id, string task, TimeSpan start, TimeSpan stop) 
        {
            string Query = $"UPDATE Tracker SET Time_Start = \'{start}\', " +
                   $"Time_Stop = \'{stop}\', " +
                   $"Task = \'{task}\' " +
                   $"WHERE Id = {id}";
            return await ExecuteQueryAsync(Query);
        }

        public async Task<bool> InsertElementAsync(string completionpDay, string task, TimeSpan start, TimeSpan stop, string status)
        {
            string Query = $"INSERT INTO Tracker (Day_Of_Completion, Time_Start, Time_Stop, Task, Status_Task) " +
                               $"VALUES (\'{completionpDay}\', " +
                               $"\'{start}\', " +
                               $"\'{stop}\', " +
                               $"\'{task}\', " +
                               $"\'{status}\')";
            return await ExecuteQueryAsync(Query);
        }

        public async Task<bool> DeleteElemenyByIdAsync(string id)
        {
            string Query = $"DELETE FROM Tracker WHERE Id = {id}";
            return await ExecuteQueryAsync(Query);
        }

        public async Task<bool> ExecuteQueryAsync(string command)
        {
            try
            {
                openConnection();
                ExecuteCommand = new SqlCommand(command, getConnection());
                await ExecuteCommand.ExecuteReaderAsync();
            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
            finally { closeConnection(); }
            return await Task.FromResult(true);
        }
    }
}

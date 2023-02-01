using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace PassWordGenerate
{
	class MyClass
	{
		
		const int totalTask= 7;
        const int total = 30000000;
		const int recordsPerTask = (total / totalTask);
		const int leftoutRecords=(total % totalTask);
		const string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\Meet  Kerasiya\source\repos\PracticeSession2\PracticeSession2\Database1.mdf"";Integrated Security=True;Connect Timeout=30";
		static int completed = 0;
		static Random random = new Random();

        static void Main(string[] args)
		{
			
			Stopwatch watch= new Stopwatch();
			watch.Start();
			DBConnect();
			List<Task> tasks = new List<Task>();
			for(int i=0;i<totalTask;i++)
			{
				Task t = AddDataToDB(recordsPerTask);
				tasks.Add(t);
			}
			Task task=Task.WhenAll(tasks);
			if(leftoutRecords>0)
			{
				Task.Run(
					() => AddDataToDB(leftoutRecords));
			}
			try
			{
                task.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            watch.Stop();
            Console.WriteLine("Total time : " + (watch.ElapsedMilliseconds / 1000));
            Console.WriteLine(leftoutRecords);
        }

        static void DBConnect()
        {
            
            SqlConnection cnn = new SqlConnection(connectionString);
            try
            {
                cnn.Open();

                Console.WriteLine("Conncetion established");
                SqlCommand cmd = new SqlCommand("TRUNCATE TABLE PasswordTable ", cnn);
                cmd.ExecuteNonQuery();
                
                cnn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not establish sql connection");
                Console.WriteLine(e.ToString());
                Environment.Exit(1);
            }
        }
       
		private static async Task AddDataToDB(int recordsPerTask)
        {
			SqlConnection cnn = new SqlConnection(connectionString);
			string password;
            string query = "Insert Into PasswordTable (password) VALUES (@pass)";
			await cnn.OpenAsync();
			for(int i=0;i<=recordsPerTask;i++)
			{
				
				Console.WriteLine(completed);
                if(completed>=total)
                {
                    break;
                }
				password = GeneratePassword();
				using(SqlCommand cmd=new SqlCommand(query,cnn))
				{
					cmd.Parameters.AddWithValue("@pass",password);
					try
					{
						completed++;
						await cmd.ExecuteNonQueryAsync();
					}
					catch (Exception ex)
					{
						i--;
					}
				}
				
			}
        }


        
        const string smalls = "abcdefghijklmnopqrstuvwxyz";
        const string capitals = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numbers = "0123456789";
        const string specials = "~&*()-_=+<!@#$%^,>.";
        

        private static string GeneratePassword()
        {
		
            int total_special = random.Next(2, 5);
            int total_number = random.Next(2, 5);
            int total_smalls = random.Next(2, 5);
            int total_capital = 20 - total_smalls - total_number - total_special;
            string result = "";
            for (int i = 0; i < total_special; i++)
            {
                result+=(specials[random.Next(specials.Length)]);
            }
            for (int i = 0; i < total_capital; i++)
            {
                result+=(capitals[random.Next(capitals.Length)]);
            }

            for (int i = 0; i < total_number; i++)
            {
                result+=(numbers[random.Next(numbers.Length)]);
            }
            for (int i = 0; i < total_smalls; i++)
            {
                result+=(smalls[random.Next(smalls.Length)]);
            }
            return result;
        }
    }
}
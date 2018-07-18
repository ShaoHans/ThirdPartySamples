using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DapperSample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (DapperHelper db = new DapperHelper())
            {
                var projects = db.Query<Project>("select top 10 * from PM_Project").ToList();
                projects.ForEach(p => Console.WriteLine(p.ToString()));
                Console.WriteLine("==========================================");

                projects = db.Query<Project>("select * from PM_Project where ProjectAmount > @ProjectAmount", new { ProjectAmount = 0 }).ToList();
                projects.ForEach(p => Console.WriteLine(p.ToString()));
                Console.WriteLine("==========================================");

                var tasks = db.Query<dynamic>("select top 10 * from PM_Task ").ToList();
                tasks.ForEach(t => { Console.WriteLine($"任务名称：{t.TaskName}，制作人：{t.AssignToName}"); });
                Console.WriteLine("==========================================");

                projects = db.Query<Project>("select * from PM_Project where Status in @StatusArr", new { StatusArr = new int[] { 8, 16 } }).ToList();
                projects.ForEach(p => Console.WriteLine(p.ToString()));
                Console.WriteLine("==========================================");

                projects = db.Query<Project, User, Project>("select top 5 p.*,a.* from PM_Project p inner join Sys_Account a on p.CreateBy = a.AccountName",
                    (p, u) =>
                    {
                        p.User = u;
                        return p;
                    }, splitOn: "AccountId").ToList();
                projects.ForEach(p =>
                {
                    Console.WriteLine(p.ToString());
                    Console.WriteLine(p.User.ToString());
                });
                Console.WriteLine("==========================================");

                List<User> users;
                string sql = @"select top 3 * from PM_Project;
                               select top 3 * from Sys_Account;";
                using (var mutil = db.QueryMultiple(sql))
                {
                    projects = mutil.Read<Project>().ToList();
                    users = mutil.Read<User>().ToList();

                    projects.ForEach(p => Console.WriteLine(p.ToString()));
                    users.ForEach(u => Console.WriteLine(u.ToString()));
                }
                Console.WriteLine("==========================================");

                int maleCount = Convert.ToInt32(db.ExecuteScalar("select count(*) from Sys_Account where Sex = @Sex",
                    new { Sex = new DbString { Value = "Male", IsFixedLength = true, Length = 10, IsAnsi = true } }));
                Console.WriteLine($"男性用户数量：{maleCount}");
                Console.WriteLine("==========================================");
            }

            Console.ReadKey();
        }
    }
}

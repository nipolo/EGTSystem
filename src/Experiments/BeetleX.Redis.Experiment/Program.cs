using System;
using System.Threading.Tasks;

namespace BeetleX.Redis.Experiment
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var DB = new RedisDB(1)
            {
                DataFormater = new JsonFormater()
            };
            //DefaultRedis.Instance.Host
            //    .AddWriteHost("redisServer")
            //    .Password = "password";
            //DefaultRedis.Instance.DataFormater = new JsonFormater();
            //var DB = DefaultRedis.Instance;
            DB.Host.AddWriteHost("redisServer");

            var table = DB.CreateHashTable("beetle:experiment");
            var result = await table.MSet(("field1", Employee.Employee1), ("field2", Employee.Employee2));

            var employees = await table.Get<Employee, Employee>("field1", "field2");
            await table.Del("emp2");
            await table.Keys();
        }
    }

    internal class Employee
    {
        public static Employee Employee1 = new Employee() { Prop1 = "1.1", Prop2 = "1.2" };
        public static Employee Employee2 = new Employee() { Prop1 = "2.1", Prop2 = "2.2" };

        public string Prop1 { get; set; }

        public string Prop2 { get; set; }
    }
}

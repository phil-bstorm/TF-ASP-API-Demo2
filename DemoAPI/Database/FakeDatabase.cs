using DemoAPI.Database.Intefaces;

namespace DemoAPI.Database
{
    public class FakeDatabase : IFakeDatabase
    {
        private List<string> Employees { get; set; } = ["Della", "Picsou"];

        public List<string> GetAllEmployees()
        {
            return Employees;
        }

        public string CreateEmployee(string name)
        {
            Employees.Add(name);
            return name;
        }
    }
}

namespace DemoAPI.Database.Intefaces
{
    public interface IFakeDatabase
    {
        List<string> GetAllEmployees();
        string CreateEmployee(string name);
    }
}

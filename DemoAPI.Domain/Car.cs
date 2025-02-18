namespace DemoAPI.Domain
{
    public class Car
    {
        public int Id { get; set; }
        public required string Brand { get; set; }
        public required string Model { get; set; }
        public required string Color { get; set; }
        public required int HorsePower { get; set; }
        public bool IsNew { get; set; }
    }
}

using System.Text;

public record Concert(int Id, string Name, int AvailableTickets)
{
    public static List<Concert> All => new()
    {
        new(1, "Taylor Swift", 10),
        new(2, "Foo Fighters", 10),
        new(3, "Glass Animals", 10)
    };
}
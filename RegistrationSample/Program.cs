// See https://aka.ms/new-console-template for more information

using Proto;
using RegistrationSample;
using static Spectre.Console.AnsiConsole;

var actorSystem = new ActorSystem();
var prop = Props.FromProducer(() => new ConcertOrganizerActor());

var concertOrganizerPid = actorSystem.Root.Spawn(prop);

do
{
    MarkupLine("[yellow]0.[/] [green]Get Current Ticket Counts[/]");
    foreach (var concert in Concert.All) {
        MarkupLine($"[yellow]{concert.Id}.[/] [green]{concert.Name}[/]");
    }
    
    var id = Ask<int>("[green]Enter a concert #:[/]");

    if (id == 0) {
        var result =
        await actorSystem.Root.RequestAsync<AllTicketsCountsResponse>(
            concertOrganizerPid,
            new AllTicketsCountsRequest());

        OutputCurrentTickets(result);
    }
    else
    {
        var numberOfTickets = Ask<int>("[green]Enter # of tickets:[/]");
        actorSystem.Root.Send(
            concertOrganizerPid,
            new Register(id, numberOfTickets));
        MarkupLine("Processing...");
        await Task.Delay(100);
    }
    
    MarkupLine("");
}
while(true);

void OutputCurrentTickets(AllTicketsCountsResponse response)
{
    if (!response.Concerts.Any()) {
        MarkupLine("[green]No Registrations Occurred Yet![/]");
        return;
    }
    
    foreach (var (id, name, currentCount) in response.Concerts) {
        MarkupLine($"[yellow]{id}.[/] [green]{name}[/] [yellow]({currentCount})[/]");
    }
}

public record Register(int ConcertId, int Tickets);
public record AllTicketsCountsRequest;
public record AllTicketsCountsResponse(List<CurrentConcertTicketCountResponse> Concerts);
public record CurrentConcertTicketCountRequest;
public record CurrentConcertTicketCountResponse(int Id, string Name, int CurrentCount);


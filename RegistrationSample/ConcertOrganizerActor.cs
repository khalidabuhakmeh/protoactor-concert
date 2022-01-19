using Proto;
using static Spectre.Console.AnsiConsole;

namespace RegistrationSample;

public class ConcertOrganizerActor : IActor
{
    private readonly Dictionary<int, PID> concerts = new();
    
    public async Task ReceiveAsync(IContext context)
    {
        switch (context.Message)
        {
            case Register register:
                ProcessRegistration(context, register);
                break;
            case AllTicketsCountsRequest m:
                await GetTicketCountsFromActiveConcerts(context);
                break;
        }
    }

    private async Task GetTicketCountsFromActiveConcerts(IContext context)
    {
        List<CurrentConcertTicketCountResponse> counts =
            new();
        
        foreach (var concert in concerts)
        {
            var result = await
                context.RequestAsync<CurrentConcertTicketCountResponse>(
                    concert.Value, new CurrentConcertTicketCountRequest());
            
            counts.Add(result);
        }
        
        context.Respond(new AllTicketsCountsResponse(counts));
    }

    private void ProcessRegistration(IContext context, Register register)
    {
        var concert = Concert.All.Find(c => c.Id == register.ConcertId);
        if (concert is null) {
            MarkupLine($"[red]Concert #{register.ConcertId} doesn't exist[/]");
            return;
        }
        
        PID CreateConcertActor()
        {
            var props = Props.FromProducer(() => new ConcertActor(concert));
            var pid = context.SpawnNamed(props, $"concert-{concert.Id}");
            concerts.Add(concert.Id, pid);
            MarkupLine($"[yellow]ConcertActor created for {concert.Name}({concert.Id})[/]");
            return pid;
        }

        var concertPid =
            concerts.TryGetValue(concert.Id, out var result)
                ? result
                : CreateConcertActor();
        
        // forward the message to the concert
        context.Forward(concertPid);
    }
}
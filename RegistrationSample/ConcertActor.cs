using Proto;
using static Spectre.Console.AnsiConsole;

namespace RegistrationSample;

public class ConcertActor : IActor
{
    private int currentTickets;
    private readonly Concert concert;

    public ConcertActor(Concert concert)
    {
        this.concert = concert;
        currentTickets = concert.AvailableTickets;
    }

    public Task ReceiveAsync(IContext context)
    {
        switch (context.Message)
        {
            case Register register:
                RegisterForConcertIfPossible(register);
                break;
            case CurrentConcertTicketCountRequest:
                context.Respond(new CurrentConcertTicketCountResponse(concert.Id, concert.Name, currentTickets));
                break;
        }

        return Task.CompletedTask;
    }

    private void RegisterForConcertIfPossible(Register register)
    {
        if (currentTickets - register.Tickets >= 0)
        {
            currentTickets -= register.Tickets;
            MarkupLine(
                $"registered {register.Tickets} tickets for {concert.Name}({concert.Id})\n{currentTickets} tickets left.");
            return;
        }

        MarkupLine($"[red]Not enough tickets! {currentTickets} tickets left for {concert.Name}.[/]");
    }
}
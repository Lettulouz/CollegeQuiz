using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CollegeQuizWeb.Hubs;

public class QuizSessionHub : Hub
{
    public Task JoinSession(string sessionId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
    }

    public Task LeaveSession(string sessionId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
    }
}
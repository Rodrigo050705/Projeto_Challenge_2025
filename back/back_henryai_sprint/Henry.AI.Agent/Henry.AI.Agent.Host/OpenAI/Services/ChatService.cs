using Mgb.DependencyInjections.DependencyInjectons.Interfaces;
using OpenAI.Chat;

namespace Henry.AI.Agent.Host.OpenAI.Services;

public class ChatService : IChatService, ITransientDependency
{
    private readonly ChatClient _chatClient;

    public ChatService(ChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> SendMessageAsync(string prompt)
    {
        ChatCompletion completion = await _chatClient.CompleteChatAsync(prompt);
        var result = completion.Content[0].Text;
        return result;
    }
}
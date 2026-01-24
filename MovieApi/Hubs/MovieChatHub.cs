using Microsoft.AspNetCore.SignalR;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;

public class MovieChatHub : Hub
{
    private readonly ChatClient _chatClient;

    // Inject the ChatClient (Registered in Program.cs)
    public MovieChatHub(ChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task AskAssistant(string userQuery)
    {
        // 1. Create the message history
        // Note: For a production app, you'd pull previous messages from a database/cache
        List<ChatMessage> messages = new()
        {
            new SystemChatMessage("You are a helpful movie assistant for 'Movie-Vault'. Suggest movies and provide details."),
            new UserChatMessage(userQuery)
        };

        try
        {
            // 2. Start the streaming request
            AsyncCollectionResult<StreamingChatCompletionUpdate> updates = 
                _chatClient.CompleteChatStreamingAsync(messages);

            // 3. Iterate through the stream as tokens arrive
            await foreach (StreamingChatCompletionUpdate update in updates)
            {
                // Each update can have multiple content parts (usually just one)
                foreach (ChatMessageContentPart part in update.ContentUpdate)
                {
                    if (!string.IsNullOrEmpty(part.Text))
                    {
                        // 4. Send the specific text token to the React client
                        await Clients.Caller.SendAsync("ReceiveToken", part.Text);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReceiveToken", $"[Error: {ex.Message}]");
        }
    }
}
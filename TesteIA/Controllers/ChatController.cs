using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using TesteIA.Plugins;

namespace TesteIA.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController(Kernel kernel) : ControllerBase
{
    private readonly Kernel _kernel = kernel;

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromQuery] string message)
    {
        var chatHistory = new ChatHistory();
        var chatCompletion = _kernel.GetRequiredService<IChatCompletionService>();

        _kernel.Plugins.AddFromType<ProductPlugin>("Plugins", _kernel.Services);


        // Se a função chama antes ou depois
        OllamaPromptExecutionSettings settings = new OllamaPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        chatHistory.AddUserMessage(message);

        var result1 = await chatCompletion.GetChatMessageContentAsync(chatHistory, executionSettings: settings, kernel: _kernel);

        //var result = await _kernel.InvokePromptAsync<string>(message);

        return Ok(result1);
    }
}

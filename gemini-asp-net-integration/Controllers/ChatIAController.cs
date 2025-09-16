using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace gemini_asp_net_integration.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatIAController : ControllerBase
{
    private readonly IKernelBuilder _kernelBuilder;
    private readonly IChatCompletionService _chatService;

    public ChatIAController(IKernelBuilder kernelBuilder)
    {
        _kernelBuilder = kernelBuilder;
        _chatService = _kernelBuilder.Build().GetRequiredService<IChatCompletionService>();
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromQuery] string prompt)
    {
        var result = await _chatService.GetChatMessageContentAsync(prompt, kernel: _kernelBuilder.Build());

        return Ok(result);
    }
}

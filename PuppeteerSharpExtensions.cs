using PuppeteerSharp;

namespace UnityDownloader;

public static class PuppeteerSharpExtensions
{
    public static async Task<string> GetInnerTextAsync(this IElementHandle elementHandle)
    {
        return await elementHandle.EvaluateFunctionAsync<string>("e => e.innerText");
    }
}
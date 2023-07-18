using System.Threading;

namespace TooniverseAPI.Mappings.Utils.Http;

public abstract class Request
{
    private readonly HttpClient _client = new();
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(2.5);

    private readonly int _retryLimit = 3;


    protected async Task<HttpResponseMessage?> SendAsync(HttpRequestMessage request, int retryCount = 0)
    {
        
        using var cts = new CancellationTokenSource(_timeout);

        try
        {
            var newRequest = new HttpRequestMessage()
            {
                RequestUri = request.RequestUri,
                Method = request.Method,
                Content = request.Content,
            };
            if (request?.Headers != null)
                foreach (var header in request.Headers)
                    newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            
            var response = await _client.SendAsync(newRequest, cts.Token);
            return response;
        }
        catch (OperationCanceledException)
        {
            if (retryCount >= _retryLimit) return null;
            return await SendAsync(request, retryCount + 1);
        }
    }

    protected async Task<HttpResponseMessage> SendWithProxyAsync(HttpRequestMessage request, int retryCount = 0)
    {
        using var cts = new CancellationTokenSource(_timeout);

        try
        {
            var url = Proxy.GetLeastUsedProxy() + request.RequestUri!;
            var newUri = new Uri(url);

            var newRequest = new HttpRequestMessage()
            {
                RequestUri = newUri,
                Method = request.Method,
                Content = request.Content
            };

            newRequest.Headers.Add("Origin", newUri.Host);

            if (request.Headers != null)
                foreach (var header in request.Headers)
                    newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);

            var response = await _client.SendAsync(newRequest, cts.Token);
            return response;
        }
        catch (OperationCanceledException)
        {
            if (retryCount >= _retryLimit) return null;

            return await SendWithProxyAsync(request, retryCount + 1);
        }
    }
}
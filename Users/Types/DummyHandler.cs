namespace Users.Types
{
    public class DummyHandler:DelegatingHandler
    {
        override protected async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("DummyHandler is processing the request.");
            return await base.SendAsync(request, cancellationToken);
        }
    }
}

// This class was copied from:
//  https://github.com/n-develop/HttpClientMock/blob/master/HttpClientMock.Tests/MockHttpMessageHandler.cs
// then edited to adjust my needs.

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jobberwocky.Test.Helpers
{
  public class MockHttpMessageHandler : HttpMessageHandler
  {
    private string response;
    private HttpStatusCode statusCode;

    public string Input { get; private set; }
    public int NumberOfCalls { get; private set; }

    public void SetResponse(string response, HttpStatusCode statusCode)
    {
      this.response = response;
      this.statusCode = statusCode;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      NumberOfCalls++;

      if (request.Content != null)
      {
        this.Input = await request.Content.ReadAsStringAsync();
      }

      return new HttpResponseMessage
      {
        StatusCode = statusCode,
        Content = new StringContent(response)
      };
    }
  }
}

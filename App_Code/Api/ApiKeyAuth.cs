using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

public class ApiKeyFilter : ActionFilterAttribute, IAuthenticationFilter
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		HttpRequestMessage request = context.Request;
		AuthenticationHeaderValue authorization = request.Headers.Authorization;

		if (authorization == null)
		{
			context.ErrorResult = new AuthenticationFailureResult("Unauthorized", request);
			return;
		}
		if (authorization.Scheme != "ApiKey")
		{
			context.ErrorResult = new AuthenticationFailureResult("Unauthorized", request);
			return;
		}
		if (String.IsNullOrEmpty(authorization.Parameter))
		{
			context.ErrorResult = new AuthenticationFailureResult("Unauthorized", request);
			return;
		}
		if (authorization.Parameter != Settings.CurrentSettings.SERVERAPIKEY)
		{
			context.ErrorResult = new AuthenticationFailureResult("Unauthorized", request);
			return;
		}
		context.Principal = new ClaimsPrincipal();
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	async Task IAuthenticationFilter.ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		return;
	}
}

public class AuthenticationFailureResult : IHttpActionResult
{
	public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
	{
		ReasonPhrase = reasonPhrase;
		Request = request;
	}

	public string ReasonPhrase { get; private set; }

	public HttpRequestMessage Request { get; private set; }

	public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
	{
		return Task.FromResult(Execute());
	}

	private HttpResponseMessage Execute()
	{
		HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
		response.RequestMessage = Request;
		response.ReasonPhrase = ReasonPhrase;
		return response;
	}
}
<%@ WebHandler Language="C#" Class="Events" %>

using System;
using System.Web;
using System.Net.WebSockets;
using System.Web.WebSockets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class Events : IHttpHandler
{
	private static readonly List<WebSocket> Clients = new List<WebSocket>();
	private static readonly object _lock = new object();

	public void ProcessRequest(HttpContext context)
	{
		if (context.IsWebSocketRequest)
			context.AcceptWebSocketRequest(WebSocketRequest);
	}
	private async Task WebSocketRequest(AspNetWebSocketContext context)
	{
		var socket = context.WebSocket;

		lock (_lock)
		{
			Clients.Add(socket);
		}

		while (true)
		{
			var buffer = new ArraySegment<byte>(new byte[1024]);
			var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
			for (int i = 0; i < Clients.Count; i++)
			{
				WebSocket client = Clients[i];
				try
				{
					if (client.State == WebSocketState.Open)
					{
						await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
					}
				}
				catch (ObjectDisposedException)
				{
					lock (_lock)
					{
						Clients.Remove(client);
						i--;
					}
				}
			}
		}
	}
	public bool IsReusable
	{
		get
		{
			return false;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Web;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;

namespace Sitecore.Modules
{
	public class CalendarScriptModule : IHttpModule
	{
		#region IHttpModule Members

		public void Init(HttpApplication context)
		{
			context.PreSendRequestContent += context_PreSendRequestContent;
		}

		public void Dispose()
		{
		}

		#region Private methods

		private static void context_PreSendRequestContent(object sender, EventArgs e)
		{
			var application = (HttpApplication)sender;
			HttpResponse response = application.Response;

			if ((response.StatusCode == 0x12e) &&
					(response.Headers["requestFrom"] == "CalendarModule")) //302 Found
			{
				if (IsAsyncPostBackRequest(application.Request.Headers))
				{
					string redirectLocation = response.RedirectLocation;
					var list = new List<HttpCookie>(response.Cookies.Count);

					for (int i = 0; i < response.Cookies.Count; i++)
					{
						list.Add(response.Cookies[i]);
					}

					response.ClearContent();
					response.ClearHeaders();

					for (int j = 0; j < list.Count; j++)
					{
						response.AppendCookie(list[j]);
					}

					response.Cache.SetCacheability(HttpCacheability.NoCache);
					response.ContentType = "text/plain";

					EncodeString(response.Output, "pageRedirect", string.Empty, redirectLocation);
				}
			}
		}

		private static bool IsAsyncPostBackRequest(NameValueCollection headers)
		{
			string[] values = headers.GetValues("X-MicrosoftAjax");

			if (values != null)
			{
				for (int i = 0; i < values.Length; i++)
				{
					string[] strArray2 = values[i].Split(new[] { ',' });

					for (int j = 0; j < strArray2.Length; j++)
					{
						if (strArray2[j].Trim() == "Delta=true")
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		private static void EncodeString(TextWriter writer, string type, string id, string content)
		{
			if (id == null)
			{
				id = string.Empty;
			}

			if (content == null)
			{
				content = string.Empty;
			}

			int num = 0;

			for (int i = 0; i < content.Length; i++)
			{
				if (content[i] == '\x00ff')
				{
					num++;
				}
				else if (content[i] == '\0')
				{
					num += 2;
				}
			}

			writer.Write((content.Length + num).ToString(CultureInfo.InvariantCulture));
			writer.Write('|');
			writer.Write(type);
			writer.Write('|');
			writer.Write(id);
			writer.Write('|');

			int index = 0;
			char[] buffer = content.ToCharArray();

			for (int j = 0; j < buffer.Length; j++)
			{
				if (buffer[j] == '\x00ff')
				{
					writer.Write(buffer, index, j - index);
					writer.Write("\x00ff\x00ff");
					index = j + 1;
				}
				else if (buffer[j] == '\0')
				{
					writer.Write(buffer, index, j - index);
					writer.Write("\\\x00ff\\");
					index = j + 1;
				}
			}

			writer.Write(buffer, index, buffer.Length - index);
			writer.Write('|');
		}

		#endregion

		#endregion
	}
}

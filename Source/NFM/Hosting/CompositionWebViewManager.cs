using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.FileProviders;
using Microsoft.Web.WebView2.Core;

namespace NFM.Hosting;

/// <summary>
/// Attaches Blazor to a raw <see cref="CoreWebView2"/>. The stock WebView2 manager is bound to the
/// WinForms control, which creates its own non-composition controller - so it can't host the viewport.
/// </summary>
sealed class CompositionWebViewManager : WebViewManager
{
	public static Uri AppBase { get; } = new("https://nfm.local/");

	private const string HostShim = """
		window.external = {
			sendMessage: message => window.chrome.webview.postMessage(message),
			receiveMessage: callback => window.chrome.webview.addEventListener('message', e => callback(e.data))
		};
		""";

	private readonly CoreWebView2 webview;
	private readonly CoreWebView2Environment environment;

	private CompositionWebViewManager(CoreWebView2 webview, CoreWebView2Environment environment, IServiceProvider services,
		Dispatcher dispatcher, IFileProvider files, string hostPage)
		: base(services, dispatcher, AppBase, files, new JSComponentConfigurationStore(), hostPage)
	{
		this.webview = webview;
		this.environment = environment;
	}

	public static async Task<CompositionWebViewManager> CreateAsync(CoreWebView2 webview, CoreWebView2Environment environment,
		IServiceProvider services, Dispatcher dispatcher, string contentRoot, string hostPage)
	{
		CompositionWebViewManager manager = new(webview, environment, services, dispatcher,
			new PhysicalFileProvider(contentRoot), hostPage);

		webview.AddWebResourceRequestedFilter($"{AppBase}*", CoreWebView2WebResourceContext.All);
		webview.WebResourceRequested += manager.OnWebResourceRequested;
		webview.WebMessageReceived += manager.OnWebMessageReceived;

		await webview.AddScriptToExecuteOnDocumentCreatedAsync(HostShim);

		return manager;
	}

	/// <summary>
	/// Raised for page messages that aren't Blazor's, which are posted as objects rather than strings.
	/// </summary>
	public event Action<string> OnHostMessage = delegate {};

	protected override void NavigateCore(Uri absoluteUri)
	{
		webview.Navigate(absoluteUri.AbsoluteUri);
	}

	protected override void SendMessage(string message)
	{
		webview.PostWebMessageAsString(message);
	}

	private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
	{
		string json = e.WebMessageAsJson;

		if (json.StartsWith('{'))
		{
			OnHostMessage.Invoke(json);
			return;
		}

		MessageReceived(new Uri(e.Source), e.TryGetWebMessageAsString());
	}

	private void OnWebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
	{
		bool allowFallbackOnHostPage = e.ResourceContext == CoreWebView2WebResourceContext.Document;
		string uri = e.Request.Uri.Split('?')[0];

		if (!TryGetResponseContent(uri, allowFallbackOnHostPage, out int statusCode, out string statusMessage,
			out Stream content, out IDictionary<string, string> headers))
		{
			return;
		}

		string headerText = string.Join(Environment.NewLine, headers.Select(o => $"{o.Key}: {o.Value}"));
		e.Response = environment.CreateWebResourceResponse(content, statusCode, statusMessage, headerText);
	}
}

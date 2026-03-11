using System.Net.Http.Json;
using Portfolio.Models;

namespace Portfolio.Pages.ProgrammingBlogs;

public partial class ProgrammingBlogs : ComponentBase
{
	[Inject]
	public required HttpClient HttpClient { get; init; }

	private BlogMetaData[]? BlogPreviews { get; set; }

	private const string _blogIndexPath = "documents/blogs/index.json";

	protected override async Task OnInitializedAsync()
	{
		// TODO TEST CODE REMOVE
		await Task.Delay(1000);

		BlogPreviews = await HttpClient.GetFromJsonAsync<BlogMetaData[]>(_blogIndexPath);

		// BlogPreviews = [

		// ];
	}
}
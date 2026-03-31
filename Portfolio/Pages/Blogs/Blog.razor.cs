namespace Portfolio.Pages.Blogs;

public partial class Blog
{
	[Inject]
	public required HttpClient HttpClient { get; init; }

	[Parameter]
	public required string FileName { get; set; }

	private string? _blogMarkup;

	protected override async Task OnInitializedAsync()
	{
		_blogMarkup = await HttpClient.GetStringAsync($"documents/blogs/{FileName}.html");
	}
}
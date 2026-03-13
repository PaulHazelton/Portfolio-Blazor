namespace Portfolio.Pages.ProgrammingBlogs;

public partial class Blog
{
	[Inject]
	public required HttpClient HttpClient { get; init; }

	[Parameter]
	public required string FileName { get; set; }

	// private string? _markdown;
	private string? _blogMarkup;

	protected override async Task OnInitializedAsync()
	{
		var markdown = await HttpClient.GetStringAsync($"documents/blogs/{FileName}.md");

		// _blogMarkup = Markdown.ToHtml(markdown);
	}
}
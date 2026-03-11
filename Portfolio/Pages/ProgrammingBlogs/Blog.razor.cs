namespace Portfolio.Pages.ProgrammingBlogs;

public partial class Blog
{
	[Inject]
	public required HttpClient HttpClient { get; init; }

	[Parameter]
	public required string FileName { get; set; }

	private string? _markdown;

	protected override async Task OnInitializedAsync()
	{
		_markdown = await HttpClient.GetStringAsync($"documents/blogs/{FileName}.md");
	}
}
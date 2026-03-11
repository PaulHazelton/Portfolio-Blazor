using Portfolio.Models;

namespace Portfolio.Pages.ProgrammingBlogs;

public partial class BlogPreviewItem : ComponentBase
{
	[Parameter, EditorRequired]
	public required BlogMetaData BlogPreview { get; set; }
}
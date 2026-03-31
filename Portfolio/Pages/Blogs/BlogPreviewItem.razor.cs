using Portfolio.Models;

namespace Portfolio.Pages.Blogs;

public partial class BlogPreviewItem
{
	[Parameter, EditorRequired]
	public required BlogMetaData BlogPreview { get; set; }
}
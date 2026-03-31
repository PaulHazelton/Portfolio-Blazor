using Portfolio.Models;

namespace Portfolio.Pages.Projects;

public partial class ProjectPreviewItem
{
	[Parameter, EditorRequired]
	public required ProjectMetaData ProjectPreview { get; set; }

	[Parameter, EditorRequired]
	public required RenderFragment Description { get; set; }

	[Parameter, EditorRequired]
	public required RenderFragment Image { get; set; }
}
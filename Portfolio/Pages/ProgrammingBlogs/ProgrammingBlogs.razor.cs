using Portfolio.Models;

namespace Portfolio.Pages.ProgrammingBlogs;

public partial class ProgrammingBlogs : ComponentBase
{
	private BlogPreview[]? BlogPreviews { get; set; }

	protected override async Task OnInitializedAsync()
	{
		// TODO TEST CODE REMOVE
		await Task.Delay(1000);

		

		// BlogPreviews = [
			
		// ];
	}
}
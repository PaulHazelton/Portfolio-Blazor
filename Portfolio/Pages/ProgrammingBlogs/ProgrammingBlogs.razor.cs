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
		// 	new("Is Loading Pattern in Blazor", new(2025, 10, 1), "It's a common pattern to have an `IsLoading` bool property in the code-behind file for a Blazor component, and then check if this is false before using certain data. While this can get the job done, and appears in many Microsoft tutorials, I think it is lacking in many ways. Nonetheless, I have some suggestions on how to improve the use of this pattern while also reducing nullable warnings."),
		// 	new("")
		// ];
	}
}
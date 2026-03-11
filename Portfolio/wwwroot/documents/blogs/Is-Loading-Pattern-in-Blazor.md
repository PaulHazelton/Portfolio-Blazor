# Is Loading Pattern in Blazor

It's a common pattern to have an `IsLoading` bool property in the code-behind file for a Blazor component, and then check if this is false before using certain data. While this can get the job done, and appears in many Microsoft tutorials, I think it is lacking in many ways. Nonetheless, I have some suggestions on how to improve the use of this pattern while also reducing nullable warnings.

Here the `IsLoading` property has an attribute that explicitly specifies what variables will be initialized after `IsLoading` is `false`.

```c#
[MemberNotNullWhen(false, nameof(Model), nameof(SomeProperty))]
protected bool IsLoading { get; set; } = true;

protected SomeEntity? Model { get; set; }
protected IEnumerable<string>? SomeProperty { get; set; }
```

So here in the razor file we can put everything in an if statement that checks `IsLoading` and there will be no warnings.

```html
@if (IsLoading)
{
	<h1>Loading</h1>
}
else
{
	<h2>@Model.Name</h2>
	@foreach (var thing in SomeProperty)
	{
		<span>@thing</span>
	}
}
```

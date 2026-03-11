# Nullable Reference Types With Blazor

My team first started using nullable reference types with Blazor, we were immediately met with hundreds of warnings. In my opinion, the main issue stems from the component lifecycle methods like `OnInitializedAsync` and `OnParametersSetAsync` being used over constructors ([which weren't even an option until C# 12](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection?view=aspnetcore-10.0#use-di-in-services)) which doesn't jive well with nullable reference types. So I've taken the time to compile notes from various Microsoft tutorials combined with my own opinions to create my recommendation for how to use Blazor with nullable reference types.
## Dependency Injection

### How Microsoft Does It

Microsoft recommends just assigning `default!` when using the `[Inject]` attribute.

```c#
[Inject]
private IJSRuntime JSRuntime { get; set; } = default!;
```

See here: [learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection?view=aspnetcore-6.0#request-a-service-in-a-component)

> Since injected services are expected to be available, the default literal with the null-forgiving operator (`default!`) is assigned in .NET 6 or later. For more information, see [Nullable reference types (NRTs) and .NET compiler null-state static analysis](https://learn.microsoft.com/en-us/aspnet/core/migration/50-to-60?view=aspnetcore-6.0#nullable-reference-types-nrts-and-net-compiler-null-state-static-analysis).

### How I Do It

I prefer just using the `required` attribute. I also try to use the most restricted access modifier I need, but keep in mind that the `required` attribute means the property needs to be public. However, I like to use `init` for the setter, to make it clear that only the DI container should be setting the property.

So here's how I usually do dependency injection in my Blazor components:

```c#
[Inject]
public required IJRuntime JSRuntime { get; init; }
```

## Parameters

### Required Parameters

If the parameter is required, decorate it with the `[EditorRequired]` attribute so that the consumer of the component will get a warning at compile time if they don't provide the parameter.

Mark it as `required` to let the compiler know that it won't be null. Or use a question mark to mark it as nullable.

```c#
[Parameter, EditorRequired]
public required string RequiredParam { get; set; }

[Parameter] [EditorRequired]
public string? RequiredNullableParam { get; set; }
```

### Optional Parameters

If the parameter is optional, don't use the `[EditorRequired]` attribute. If it's optional, you can provide a default value.

```c#
[Parameter]
public string? OptionalParam { get; set; }

[Parameter]
public string OptionalParamWithDefaultValue { get; set; } = "default value";
```

### Optional Event Callback Parameters

The `EventCallback`in Blazor is a struct, so it can't be null in the first place. You can check it using `HasDelegate`, but it will not throw an error if`HasDelegate`is false, and you call `InvokeAsync()` on it.

```c#
[Parameter]
public EventCallback OptionalCallback { get; set; }

protected async Task SomeFunction()
{
	// Check HasDelegate, which is false when OptionalCallback hasn't been assigned a meaningful value.
	if (OptionalCallback.HasDelegate)
		await OptionalCallback.InvokeAsync();
	
	// Calling it anyway when HasDelegate is false does not throw an error.
	await OptionalCallback.InvokeAsync();
}
```

It may be clearer to explicitly mark it as nullable when it is optional, and to not do so when it is required. This has the drawback of allowing for "two zeros" as I would call it: a null value, and a default value that is not null but `HasDelegate` is false. I think the pros outweigh the cons though, so I prefer this style.

```c#
[Parameter]
public EventCallback? OptionalCallback { get; set; }

[Parameter, EditorRequired]
public required EventCallback RequiredCallback { get; set; }

protected async Task SomeFunction()
{
	// Compiler warns if you don't check for null
	if (OptionalCallback.HasValue)
		await OptionalCallback.Value.InvokeAsync();

	// No need to check the required one
	await RequiredCallback.InvokeAsync();
}
```

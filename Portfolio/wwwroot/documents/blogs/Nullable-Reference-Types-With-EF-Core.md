# Nullable Reference Types With EF Core

EF Core entities were another big source of warnings when we enabled the nullable feature in C#. 
Navigation properties may be required database-wise but may be null if they were not included when queried. Initialization was often most convenient with a parameterless constructor, but this now caused many warnings. After scouring Microsoft docs, reading other blogs online, and spending over 4 years waging war against null reference exceptions in C#, here's my thoughts on the best ways to use EF Core with NRT enabled.

Much of these notes is based around the Microsoft documentation, mixed in with my opinions and preferences. [Here's Microsoft's documentation for this, it's worth a read.](https://learn.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types)

## Creating Entities

### Constructors and Initialization

It looks like EF Core requires entities to have a constructor. Even if this ever changes, I prefer using constructors for the sake of clarity anyway. For these notes we will assume that EF requires a constructor to be present.

I propose 3 ways to define an entity in terms of initialization.

#### Method 1: Database Constructor and Program Constructor

The entity will have 2 (or more) constructors: an internal constructor (optionally with parameters for each required property, with matching names) intended only for use by EF when loading the object from the database, and one or more public constructors for use by the program when actually creating new entities. While this method is a bit verbose, it is my preference.

```c#
[Table(nameof(MyEntity))]
[PrimaryKey(nameof(Id))]
public class MyEntity
{
    public Guid Id { get; set; }
    
    [Required]
	public string Name { get; set; }
	
	[Required]
	public string Details { get; set; }
	
    // Navigation property
    public List<ChildEntity> Children { get; private set; }
    
    // Record that will exist as extra columns on the same table
    public CreatedInformation CreatedInformation { get; private set; }
    
    // EF Constructor
    // Could omit the parameters and just set all properties to `default!`.
    internal MyEntity(Guid id, string name, string details)
    {
	    Id = id;
		Name = name;
		Details = details;
		
		// EF core can't set navigation properties, so we have to assign default values
		// EF core will set these values automatically after the constructor is called.
		Children = [];
		CreatedInformation = default!;
	}
	
	// API Post Constructor
	public MyEntity(string name, string details, List<ChildEntity> children, string userId)
	{
		Id = Guid.NewGuid();
		Name = name;
		Details = details;
		Children = children;
		CreatedInformation = new(userId);
	}
}

public record CreatedInformation(Guid CreatedBy, DateTime CreatedOn);
```

| Pros                                                                                                       | Cons                                                                                                                                                                                                                                                                                                |
| ---------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ✅ Clarity: it is very clear what properties are expected to be not null, and which aren't.                 | ❌ Verbose: [EF Core cannot set navigation properties using a constructor](https://learn.microsoft.com/en-us/ef/core/modeling/constructors#binding-to-mapped-properties), so they have to be initialized with `default!`. And all properties must be initialized in the constructor.                 |
| ✅ Safe: if a new property is added or changed, warnings will be generated if we forget to initialize them. | ❌ Verbose: all required properties must essentially be initialized twice.                                                                                                                                                                                                                           |
| ✅ More control: it allows for clear initialization logic even in the database constructor.                 | ℹ️ Internal Constructor: If the constructor is private, there's an info warning about an unused private member. You can get around this by marking it as internal rather than, which is not ideal since it should only be used by EF Core. But I think making it internal is the best middle ground. |

#### Method 2: Suppress Warning

This method has the least boilerplate and is probably the simplest, but at the cost of ugly `#pragma`code. It's my second favorite method, and I think it is the most palatable option for most people on the team.

```c#
[Table(nameof(MyEntity))]
[PrimaryKey(nameof(Id))]
public class MyEntity
{
	public Guid Id { get; set; }
	
	[Required]
	public string Name { get; set; }
	
	[Required]
	public string Details { get; set; }
	
	// Navigation property
	public List<ChildEntity> Children { get; set; }
	
	// Record that will exist as extra columns on the same table
	public CreatedInformation CreatedInformation { get; set; }
	
	// EF Constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private MyEntity() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	
	// API Post Constructor
	public MyEntity(string name, string details, List<ChildEntity> children, string userId)
	{
		Id = Guid.NewGuid();
		Name = name;
		Details = details;
		Children = children;
		CreatedInformation = new(userId);
	}
}
```

| Pros                                                                                                           | Cons                                    |
| -------------------------------------------------------------------------------------------------------------- | --------------------------------------- |
| ✅ Clarity: less boilerplate, and the only constructors with any meaning are the only ones with any content.    | ❌ Ugly: pragma warning disable is ugly. |
| ✅ Low risk. if a new property is added or changed, warnings will be generated if we forget to initialize them. |                                         |

#### Method 3: Required Members with Optional `SetsRequiredMembers` Attribute

This time the EF Core constructor is parameter-less and empty, and all the required properties have the `required`modifier. This actually works just fine if you don't intend on using a constructor and want to only use the object-initializer syntax. I dislike this method because it opens us up to a [pit of failure](https://github.com/dotnet/csharplang/discussions/6405) (lmao) which kind of defeats the purpose of the null reference analyzer.

```c#
[Table(nameof(MyEntity))]
[PrimaryKey(nameof(Id))]
public class MyEntity
{
	public Guid Id { get; set; }
	
	[Required]
	public required string Name { get; set; }
	
	[Required]
	public required string Details { get; set; }
	
	// Navigation property
	public required List<ChildEntity> Children { get; set; }
	
	// Record that will exist as extra columns on the same table
	public required CreatedInformation CreatedInformation { get; set; }
	
	// EF Constructor
	internal MyEntity() { }
	
	// API Post Constructor
	// This attribute is required so consumers of MyEntity can just call the constructor without manually initializing all the required properties.
	// Or omit this constructor entirely if you want to just use object-initializer syntax.
	[SetsRequiredMembers]
	public MyEntity(string name, string details, List<ChildEntity> children, string userId)
	{
		Id = Guid.NewGuid();
		Name = name;
		Details = details;
		Children = children;
		CreatedInformation = new(userId);
	}
}
```

| Pros                                                                               | Cons                                                                                                                                                                                                                                                                                                                                                                                     |
| ---------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ✅ No boilerplate: Properties only have to be initialized in the "real" constructor | ❌❌ Unsafe: If we add or change a property, the compiler will not warn us if we forget to initialize it.<br><br>[There's a big warning on the Microsoft page about this.](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required)<br><br>I recommend commenting out the attribute while working with the entity itself to get the warning back temporarily. |
## Navigation Properties

There's a [section about this on the Microsoft docs](https://learn.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types#navigating-and-including-nullable-relationships) that discusses this in more detail.

### Required Navigations

In my opinion, *required* navigations should always have the the foreign key as a **non-nullable** property included on the entity.

For the reference navigation itself, unfortunately I don't know of a great way that doesn't require the programmer to just *remember to include the needed data* or find out at a runtime exception. 

Microsoft's note on this is pretty interesting:

> [!quote] [Microsoft on Required Navigations](https://learn.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types#required-navigation-properties)
> This means that required navigations from the dependent to the principal:
> - Should be non-nullable if it is considered a programmer error to access a navigation when it is not loaded.
> - Should be nullable if it acceptable for application code to check the navigation to determine whether or not the relationship is loaded.

You mark the navigation properties as nullable so that when you go use the object you get a warning that the property might be null. Then you can go and put null forgiveness operators or null-coalesce-and-throws at the top of the function. By doing it this way, you are forced to write more boilerplate, but you're less likely to forget to write the correct include statements. As an added bonus, prepending your functions with null-coalesce-and-throws can serve as added documentation.

```c#
public Guid BlogId { get; set; }
public Blog? Blog { get; set; }
```

Alternatively, Microsoft proposes a "stricter approach" that can combine the previously mentioned boilerplate onto the entity itself. Now the null-coalesce-and-throws can be put in one place, and navigation properties that are required don't have to be marked as nullable.

```c#
private Address? _shippingAddress;
public Address ShippingAddress
{
    set => _shippingAddress = value;
    get => _shippingAddress
           ?? throw new InvalidOperationException("Uninitialized property: " + nameof(ShippingAddress));
}
```

### Optional Navigations

Optional navigations should also be paired with the an ID property, if they have one, this time **nullable**. The navigation object is also nullable since it is optional. The downside here is you have to careful, a null value could mean you forgot to include the property or the property is actually missing.

```c#
public Guid? OptionalInfoId { get; set; }
public Info? OptionalInfo { get; set; }
```

## Queries

### Includes

The expressions for includes or then-includes that target nullable properties are used by EF Core to identify properties and thus will not produce null reference exceptions. So just use the null forgiveness operator "`!`" in these situations.

Example
```c#
var basicContents = await Context.Set<BasicContent>()
	.Include(c => c.ValueSet!.ValueSetVersionItems)
```
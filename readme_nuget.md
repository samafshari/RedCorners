# RedCorners

RedCorners brings some neat utilities to your C# projects.

For more documentation, visit [https://redcorners.com/core/](https://redcorners.com/core/)

## Extensions

RedCorners offers a number of extension methods to boost productivity. All extensions are under the `RedCorners` namespace:
```c#
using RedCorners;
```

### String Manipulation

#### string string.Head(int take = 20)

This method returns the first `take=20` characters of a non-null string. If the original input has less of equal to `take` characters, the original string is returned, otherwise the output contains the first `take` characters from the input, followed by `...`, making it a `take+3` characters output.

In case the input is `null`, an empty string is returned.

Examples:
```c#
Console.WriteLine($"Hello, World! This is a very long text.".Head());
Console.WriteLine($"Hello, World! This is a very long text.".Head(100));
// Hello, World! This i...
// Hello, World! This is a very long text.
```

#### HashSet<string> string.Hashtags()

This method takes a string as an input, and returns a `HashSet<string>` containing the hashtags mentioned in the input. If the input is `null`, an empty `HashSet<string>` is returned.

Hashtags are converted to LowerInvariant forms.

Example:
```c#
string input = "Hello #World #EVERYBODY!";
var hashtags = input.Hashtags();
Console.WriteLine($"Hashtags: {string.Join(" ", hashtags)}");
// Hashtags: #world #everybody
```

#### string string.RemoveDuplicateTags(...)

```
string.RemoveDuplicateTags(bool humanFormatted = false, string separator = ",")
```

This method takes a `separator` separated input containing tags, removes duplicate tags, trims tags and converts them to lowercase, and returns a new string with the result. If `humanFormatted` is `true`, the output tags will have a space between them. Example:
```c#
Console.WriteLine("food, Drinks, FooD,fun".RemoveDuplicateTags());
Console.WriteLine("food, Drinks, FooD,fun".RemoveDuplicateTags(true));
// food,drinks,fun
// food, drinks, fun
```

#### string string.RemovePrefix(string prefix)

This method returns a string without the specified `prefix`. If the input is `null`, `null` is returned. Examples:
```c#
Console.WriteLine("Hello, World!".RemovePrefix("Hello"));
Console.WriteLine("Hello, World!".RemovePrefix("Foo"));
// , World!
// Hello, World!
```

#### bool string.IsValidEmail()

This method returns `true` if the input is a valid email address. It relies on `System.Net.Mail.MailAddress`.  Example:
```c#
Assert("foo@hotmail.com");
Assert(!"not an email address");
```

### I/O Extensions

#### string string.CreateDirectoryAndReturn()

This method takes a path as its input, creates that path if it doesn't exist, and returns the input. It is useful when chained with other I/O actions. Example:
```c#
this.FilePath = Path
    .Combine(basePath, "ObjectStorage", bucket, typeFileName)
    .CreateDirectoryAndReturn();
```

### Date/Time Extensions

#### DateTime long.DateFromEpochMs()

This method returns a UTC DateTime equivalent to the input `long` in milliseconds. Example:
```c#
long epoch = 1557738320000;
Console.WriteLine(epoch.DateFromEpochMs());
// 5/13/2019 9:05:20 AM
```

#### long DateTime.ToEpochMs()

This method is the opposite of `long.DateFromEpochMs()`, where it converts a `DateTime` to epoch milliseconds, as `long`.

#### int DateTime.ToWeekNumber()

This method returns the week number for the input `DateTime`, relative to `2019-03-11`.

#### DateTime int.GetFirstDayOfWeek()

This method returns the DateTime for the first day of the input week number. Example:
```c#
DateTime input = DateTime.UtcNow;
Console.WriteLine(input);
int weekNumber = input.ToWeekNumber();
Console.WriteLine(weekNumber);
Console.WriteLine(weekNumber.GetFirstDayOfWeek());
weekNumber++;
Console.WriteLine(weekNumber.GetFirstDayOfWeek());
// 5/13/2019 9:20:15 AM
// 9
// 5/13/2019 12:00:00 AM
// 5/20/2019 12:00:00 AM
```

#### DateTime int.GetLastDayOfWeek()

This method returns the DateTime for the first day of the next week. Identical to:
```c#
(weeknumber + 1).GetFirstDayOfWeek();
```

### Injection Extensions

RedCorners provides extensions that facilitate converting data where models have different base classes, but share some properties.

#### void Inject(object destination)

This method looks at the properties of the `destination` object, and where there is an identical property in the source, it copies the value of the source to that property of the destination. Example:
```c#
class Contact
{
    public int Id { get; set; }
    public int Name { get; set; }
    public int Email { get; set; }
}

class UpdateContactModel
{
    public int Name { get; set; }
    public int Email { get; set; }
}

...

Contact original = new Contact {
    Id = 10,
    Name = "John",
    Email = "john@redcorners.com"
};

UpdateContactModel update = new UpdateContactModel {
    Name = "Sarah",
    Email = "sarah@redcorners.com"
};

// Inject Name and Email from [update] to [original]
update.Inject(original);

Console.WriteLine(original.Id);
// Prints [10], because UpdateContactModel doesn't have a property named [Id]

Console.WriteLine(original.Name);
// Prints [Sarah], because it was injected from the UpdateContactModel
```

#### T object.ReturnAs<T>() where T : new()

This method injects an object to a new object of type `T` and returns the new `T`. Example:
```c#
Contact contact = update.ReturnAs<Contact>();

Console.WriteLine(original.Id);
// Prints [0], because UpdateContactModel doesn't have a property named [Id], so the default(int) is used.

Console.WriteLine(original.Name);
// Prints [Sarah], because it was injected from the UpdateContactModel
```

#### List<T> IEnumerable<object>.ReturnAsList<T>() : where T : new()

This method returns a list of objects of type `T` from an arbitrary list of objects of any type, by doing one-by-one `Inject`s.

## Components

Components are under the `RedCorners.Components` namespace.

### Benchmark

`RedCorners.Components.Benchmark` is using a `Stopwatch` to time some actions. Example:

```c#
Benchmark benchmark = new Benchmark();
await Task.Delay(1234);
Console.WriteLine(benchmark.ToString());
await Task.Delay(4321);
Console.WriteLine(benchmark.StopToString());
// 1.24s
// 5.57s
```

#### TimeSpan Benchmark.Stop()

Stops the timer and returns the duration as a `TimeSpan`.

#### string Benchmark.ToString()

Returns the duration as a string with two decimal points and the `s` prefix. Does not stop the timer, and can be used multiple times.

#### string Benchmark.StopToString()

Same as `ToString()`, but stops the timer.

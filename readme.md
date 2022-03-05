#Synonms.Functional

The Functional library is an implementation of the 'Railway' programming paradigm plus some other functional bells and whistles.
The purpose of the library largely boils down to removing `null` checking and exceptions and adding semantic meaning.

While concessions have been made for simplicity, effort has been made to align correctly to functional programming principles.  A useful (if rather mind boggling) post about functional programming which helped shape this library is linked below.


##Maybe
This type represents an optional value which may or may not be present, much like the Nullable<> type or types suffixed with ?.  

###Purpose
- Implicitly perform null checking and prevent null reference exceptions

###Use case
- Anywhere where null objects are appropriate
- Where you find yourself using nullable types or performing null checks
- Where you find yourself throwing exceptions in non-exceptional circumstances to indicate that something was not found

###Notes
 - The underlying value must not be accessed directly as it is unsafe and instead methods like Bind() and Match() are used to safely consume the value if it is present.
 - A common use case for Maybe is to model the result of an action which does not return a value and could fail, using `Maybe<Fault>`.
 
###Examples
As well as the constructor you can use the static helper methods `None` and `Some` to create a `Maybe`:

```c#
Maybe<int> maybeWithoutAValue = Maybe<int>.None;
Maybe<int> maybeWithAValue = Maybe<int>.Some(1234);
```

Use `Match` to perform different actions depending on whether the value is present or not:

```c#
Maybe<int> maybe = TryAndGetSomeInt();
maybe.Match(
    someInt => { /* This executes if you have a value */ },
    () => { /* This executes if you do not have any value */ });
```

You also use `Match` to convert from a `Maybe<>` back to a 'regular' value (which can be of a different type):

```c#
Maybe<int> maybe = TryAndGetSomeInt();
string someString = maybe.Match(
    someInt => "I got an int",
    () => "I am empty handed");
```

Use `Bind` to compose `Maybe` together, short-circuiting when a value is missing: 

```c#
Maybe<int> TryAndGetSomeInt()
{
  /* ... */
}

Maybe<string> TryAndGetSomeStringUsingAnInt(int value)
{
  /* ... */
}

Maybe<string> maybe = TryAndGetSomeInt()
    .Bind(someInt => TryAndGetSomeStringUsingAnInt(someInt));
```

In the above example if we were unable to get the `int` value then `TryAndGetSomeStringUsingAnInt()` would not be called and we would instead simply receive `Maybe<string>.None`.

Use `Coalesce` to fallback to a default if there is no value:

```c#
int result = TryAndGetSomeInt().Coalesce(99);
```

Use `Collect` to execute several `Maybe<T>` operations and return them as a collection.  This is particularly useful where you have several validation steps that each return `Maybe<Fault>` and you want to aggregate a list of all potential failures which you can easily do with `Reduce`:

```c#
Maybe<Fault> validationResult = ValidateFirstThing()
    .Collect(() => ValidateSecondThing())
    .Collect(() => ValidateThirdThing())
    .Reduce(faults => new AggregateFault(faults));
```

Use `Map` to 'lift' a function.  This is very similar to `Bind` but with a slightly different projection function signature (it returns a regular `T` instead of `Maybe<T>`):

```c#
Maybe<string> maybe = TryAndGetSomeInt().Map(someInt => $"Hello, I'm just a regular string with {someInt} in it");
```


##OneOf
This type represents a value which may be of one form or another.

###Purpose
- Allows the representation of a value which may take one of two different forms

###Use case
- When you want to represent a value which may be of one form or another, for example when parsing JSON a numeric value could be an `int` or a `decimal`.

###Notes
- The underlying value must not be accessed directly as it is unsafe and instead methods like Match() are used to conditionally consume the value depending on which form it is.

###Examples

Use `Match` to perform different actions depending on which form the value takes:

```c#
OneOf<int, string> oneOf = GetSomethingWhichIsAnIntOrAString();
oneOf.Match(
    someInt => { /* This executes if you have an int value */ },
    someString => { /* This executes if you have a string value */ });
```


##Result

The most common form of `OneOf<,>` is that returned by functions where there is either a successful return value or a fault, i.e. `OneOf<TSuccess, Fault>`.  `Result<TSuccess>` is simply the explicit modeling of that form.

###Purpose
- Allows the representation of a value which may take one of two different forms, where one form is deemed successful and the other form is a `Fault`

###Use case
- When you want to represent an operation which can either succeed and return a value or fail in non-exceptional circumstances
- Where you find yourself throwing exceptions in non-exceptional circumstances to indicate that something failed
- Where you find yourself explicitly catching/handling exceptions

###Notes
- 'Non-exceptional circumstances' are defined as situations where it is expected that a failure would happen in everyday use.  An example of this would be the operation "map incoming resource X to domain model Y".  It is a perfectly reasonable scenario that this could be executed with an incoming resource which is not valid as per business rules.  In this case modeling the validation failure(s) with a `Fault` is preferred over throwing an exception.
- The `Bind` concept is mis-used slightly for ease of use.  Projection functions are applied in the success state only and a new `Result` is created.  In the failure state the existing `Fault` is mapped directly to the returned `Result`. 

###Examples
As well as the constructor you can use the static helper methods `Success` and `Failure` to create a `Result`:

```c#
Result<int> resultWithAnInt = Result<int>.Success(123);
Result<int> resultWithAFault = Result<int>.Failure(new SomeFault("oh no"));
```

Use `Bind` to compose `Result` together, short-circuiting when a failure occurs:

```c#
Result<int> TryAndGetSomeInt()
{
  /* ... */
}

Result<string> TryAndGetSomeStringUsingAnInt(int value)
{
  /* ... */
}

Result<string> result = TryAndGetSomeInt()
    .Bind(someInt => TryAndGetSomeStringUsingAnInt(someInt));
```

In the above example if the `int` operation failed `TryAndGetSomeStringUsingAnInt()` would not be called and the `Fault` would be passed along in the final `Result<string>`.



##Further reading

- https://fsharpforfunandprofit.com/posts/elevated-world

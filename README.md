# fisher-yates-dotnet

A C# library implementing the Fisher-Yates shuffle to randomly select unique integers from a range without replacement. Provides a bag that yields random values from a min–max range until exhausted.

**WARNING: Not thread-safe.**

**Note:** Maximum range size is 1,000,000.

## Examples

### Extract next random number between 13-27

```csharp
using FisherYates;

var bag = new FisherYatesBag(13, 27);
var number = bag.GetNextRandom();

Console.WriteLine(number); // e.g., "19"
```

### Extract 4 random numbers between 0-10

```csharp
using FisherYates;

var bag = new FisherYatesBag(10);
var numbers = bag.ExtractKRandomNumbers(4);

Console.WriteLine(string.Join(", ", numbers)); // e.g., "7, 2, 9, 0"
```

### Extract all numbers between 1-10 in random order

```csharp
using FisherYates;

var bag = new FisherYatesBag(1, 10);
IEnumerable<int> allNumbers = bag.GetAllRandomized();

// Output: All numbers from 1 to 10 in random order
foreach (var number in allNumbers) Console.WriteLine(number); // e.g., "8, 2, 4, 6, 1, 7, 3, 9, 5, 10"
```

### Generate a simple password from a character set

```csharp
using FisherYates;

string characterSet = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ0123456789!@#$%^&*";
int passwordLength = 12;

var bag = new FisherYatesBag(0, characterSet.Length - 1);
var password = new char[passwordLength];

for (int i = 0; i < passwordLength; i++)
{
    int randomIndex = bag.GetNextRandom();
    password[i] = characterSet[randomIndex];
}

string result = new string(password);
Console.WriteLine(result); // e.g., "aB3$kL9@mN2#"
```

## Testing

The test suite includes a chi-square test (`FisherYatesBag_Distribution_IsUniform_ChiSquare`) that verifies the uniformity of the random distribution. This statistical test uses a significance level of p=0.01, which means there is approximately a 1% chance of a false positive (Type I error). **If this test occasionally fails, it does not necessarily indicate a problem with the implementation** - it may simply be a rare but expected statistical event. If the test fails repeatedly, it may indicate an actual issue with the random number generation.

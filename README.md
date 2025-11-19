# fisher-yates-dotnet

A C# library implementing the Fisher-Yates shuffle to randomly select unique integers from a range without replacement. Provides a bag that yields random values from a min–max range until exhausted.

**WARNING: Not thread-safe.**

## Examples

### Extract next random number between 13-27

```csharp
using FisherYates;

var bag = new FisherYates(13,27);
var number = bag.GetNextRandom();

Console.WriteLine(number); // e.g., "19"
```

### Extract 4 random numbers between 0-10

```csharp
using FisherYates;

var bag = new FisherYates(10);
var numbers = bag.ExtractKRandomNumbers(4);

Console.WriteLine(string.Join(", ", numbers)); // e.g., "7, 2, 9, 0"
```

### Extract all numbers between 1-10 in random order

```csharp
using FisherYates;

var bag = new FisherYates(1, 10);
IEnumerable<int> allNumbers = bag.GetAllRandomized();

// Output: All numbers from 1 to 10 in random order
foreach (var number in allNumbers) Console.WriteLine(number); //eg. "8,2,4,6,1,7,3,9,5,10"
```

### Generate a simple password from a character set

```csharp
using FisherYates;

string characterSet = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ0123456789!@#$%^&*";
int passwordLength = 12;

var bag = new FisherYates(0, characterSet.Length - 1);
var password = new char[passwordLength];

for (int i = 0; i < passwordLength; i++)
{
    int randomIndex = bag.GetNextRandom();
    password[i] = characterSet[randomIndex];
}

string result = new string(password);
Console.WriteLine(result); // e.g., "aB3$kL9@mN2#"
```

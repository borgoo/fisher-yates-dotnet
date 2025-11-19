namespace FisherYates.Test;

public class Tests
{
    private readonly int _maxAllowedRangeSize = 1_000_000;

    [Test]
    public void FisherYatesBag_WhenSingleValueConstructorIsInitializedAndGetAllRandomizedIsRequested_ThenReturnsTheRandomizedSeriesWithNoDuplicates()
    {
        const int max = 10;
        FisherYatesBag fisherYates = new(max);
        var result = fisherYates.GetAllRandomized();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Count(), Is.EqualTo(max+1));
            Assert.That(result, Is.Unique);
        }

    }

    [Test]
    public void FisherYatesBag_WhenInvalidMinParametersIsPassedToTheConstructor_ThenThrowAnException()
    {
        const int min = 11;
        const int max = 10;
        Assert.Catch<ArgumentException>(() =>
        {
            FisherYatesBag fisherYates = new(min, max);
        });
    }

    [Test]
    public void FisherYatesBag_WhenTooBigRangeValuesIsPassedToContructor_ThenThrowAnException()
    {
        const int min = 10;
        int max = min + _maxAllowedRangeSize;
        Assert.Catch<ArgumentException>(() =>
        {
            FisherYatesBag fisherYates = new(min, max);
        });
    }

    [TestCase(-10)]
    [TestCase(0)]
    [TestCase(1)]
    public void FisherYatesBag_WhenInvalidMaxParametersArePassedToTheConstructor_ThenThrowAnException(int max)
    {
        const int min = 10;
        Assert.Catch<ArgumentException>(() =>
        {
            FisherYatesBag fisherYates = new(min, max);
        });
    }

    [Test]
    public void FisherYatesBag_WhenTooManyNextRandomAreRequested_ThenThrowAnException()
    {
        const int max = 0;
        FisherYatesBag fisherYates = new(max);
        fisherYates.GetNextRandom();
        Assert.Catch<InvalidOperationException>(() =>
        {
            fisherYates.GetNextRandom();
        });
    }

    [Test]
    public void FisherYatesBag_WhenTooManyNextRandomAreRequestedButTheStructureIsCorrectlyReset_ThenReturnTheValues()
    {
        const int max = 1;
        const int min = max;
        FisherYatesBag fisherYates = new(min, max);
        var result = fisherYates.GetNextRandom();
        Assert.That(result, Is.EqualTo(max));
        fisherYates.Reset();

        result = fisherYates.GetNextRandom();
        Assert.That(result, Is.EqualTo(max));
        fisherYates.Reset();

        result = fisherYates.GetNextRandom();
        Assert.That(result, Is.EqualTo(max));
    }

    [Test]
    public void FisherYatesBag_WhenTooManyNextRandomAreRequestedButTheStructureIsCorrectlyResetAfterAnExceptionCatch_ThenReturnTheValues()
    {
        const int max = 2;
        FisherYatesBag fisherYates = new(max);
        fisherYates.GetNextRandom();
        fisherYates.GetNextRandom();
        fisherYates.GetNextRandom();
        Assert.Catch<InvalidOperationException>(() =>
        {
            fisherYates.GetNextRandom();
        });
        fisherYates.Reset();

        fisherYates.GetNextRandom();
        fisherYates.GetNextRandom();
        Assert.That(fisherYates.RemainingCount, Is.EqualTo(1));


    }

    [Test]
    public void FisherYatesBag_WhenConsecutivesGetAllRemainigRandomizedAreRequestedWithoutReset_ThenThrowException()
    {
        const int max = 2;
        FisherYatesBag fisherYates = new(max);
        fisherYates.GetAllRemainingRandomized();
        Assert.Catch<InvalidOperationException>(() =>
        {
            fisherYates.GetAllRemainingRandomized();
        });

    }


    [Test]
    public void FisherYatesBag_WhenConsecutivesGetAllRemainigRandomizedAreRequestedWithReset_ThenThrowsNoExceptions()
    {
        const int max = 2;
        FisherYatesBag fisherYates = new(max);
        fisherYates.GetAllRemainingRandomized();
        fisherYates.Reset();
        fisherYates.GetAllRemainingRandomized();
    }

    [TestCase(true)]
    [TestCase(false)]
    public void FisherYatesBag_Distribution_IsUniform_ChiSquare(bool useReset)
    {
        //H0: The distribution is uniform
        //H1: The distribution is not uniform
        const int max = 3;
        const int n = max + 1;
        const int trials = 200_000;
        const double chiSquareCriticalValue = 21.666; // FROM TABLES: Chi-square critical @p=0.01 with 9 degrees of freedom (maxtrix n x n -> 4*4)
        const double expected = trials / (double)n;

        int[,] numOfValOccurencesInPosition = new int[n, n]; //numOfValOccurencesInPosition[0,2] = how many times number 2 appeared in position 0

        FisherYatesBag fisherYates = new(max);
        for (int t = 0; t < trials; t++)
        {
            if(!useReset) fisherYates = new(max);
            int[] permutation = [..fisherYates.GetAllRandomized()];
            for (int i = 0; i < n; i++) numOfValOccurencesInPosition[i, permutation[i]]++;
            if (useReset) fisherYates.Reset();
        }

        double chiSquare = 0;
        for (int pos = 0; pos < n; pos++) {
            for (int val = 0; val < n; val++) {
                double foundOccurences = numOfValOccurencesInPosition[pos, val];
                chiSquare += (foundOccurences - expected) * (foundOccurences - expected) / expected;
            }
        }

        //if chiSquare < chiSquareCriticalValue => Accept H0, otherwise reject H0 (Accept H1)
        Assert.That(chiSquare, Is.LessThan(chiSquareCriticalValue),
            $"With useRest = {useReset}, distribution is likely non-uniform. χ² = {chiSquare}");
    }

    [Test]
    public void FisherYatesBag_ExtractKRandomNumbers_WhenTooManyNumbersAreRequestedFromBeginning_ThenThrowAnException()
    {
        const int max = 10;
        FisherYatesBag fisherYates = new(max);
        Assert.Catch<ArgumentException>(() =>
        {
            fisherYates.ExtractKRandomNumbers(max + 2);
        });
    }

    [Test]
    public void FisherYatesBag_ExtractKRandomNumbers_WhenTooManyNumbersAreRequestedAfterSomeExtractions_ThenThrowAnException()
    {
        const int max = 10;
        const int k = 5;
        const int remaining = max - k;

        FisherYatesBag fisherYates = new(max);
        var result =fisherYates.ExtractKRandomNumbers(k);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Count(), Is.EqualTo(k));
            Assert.That(result, Is.Unique);
        }

        Assert.Catch<ArgumentException>(() =>
        {
            fisherYates.ExtractKRandomNumbers(remaining +2);
        });
    }

    [Test]
    public void FisherYatesBag_ExtractKRandomNumbers_WhenExactlyBagSizeAsKIsRequested_ThenReturnsTheTheValues()
    {
        const int max = 10;
        const int k = max + 1; //0-10
        FisherYatesBag fisherYates = new(max);
        var result = fisherYates.ExtractKRandomNumbers(k);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Count(), Is.EqualTo(k));
            Assert.That(result, Is.Unique);
        }
    }

    [Test]
    public void FisherYatesBag_When1MRangeBagIsBuilt_ThenReturnsTheTheValues()
    {
        int max = _maxAllowedRangeSize;
        FisherYatesBag fisherYates = new(max);
        var result = fisherYates.GetAllRandomized();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Count(), Is.EqualTo(max+1));
            Assert.That(result, Is.Unique);
        }
    }


}
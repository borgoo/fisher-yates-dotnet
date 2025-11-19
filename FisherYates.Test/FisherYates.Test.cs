namespace FisherYates.Test
{
    public class Tests
    {
        private static void AssertThatFisherYatesCompletelyRandomizeTheRange(FisherYates fisherYates, int max, int min = 0)
        {

            var result = fisherYates.GetAllRandomized().ToHashSet();

            Assert.That(result, Has.Count.EqualTo(max - min + 1));

            bool ok = true;
            for (int i = min; i <= max; i++)
            {

                if (!result.Contains(i))
                {
                    ok = false;
                    break;
                }
            }

            Assert.That(ok, Is.True);


        }

        [Test]
        public void FisherYates_WhenSingleValueConstructorIsInitializedAndGetAllRandomizedIsRequested_ThenReturnsTheRandomizedSeriesWithNoDuplicates()
        {
            const int max = 10;
            FisherYates fisherYates = new(max);

            AssertThatFisherYatesCompletelyRandomizeTheRange(fisherYates, max);


        }

        [Test]
        public void FisherYates_WhenInvalidMinParametersIsPassedToTheConstructor_ThenThrowAnException()
        {
            const int min = 11;
            const int max = 10;
            Assert.Catch<ArgumentException>(() =>
            {
                FisherYates fisherYates = new(min, max);
            });
        }

        [TestCase(-10)]
        [TestCase(0)]
        [TestCase(1)]
        public void FisherYates_WhenInvalidMaxParametersArePassedToTheConstructor_ThenThrowAnException(int max)
        {
            const int min = 10;
            Assert.Catch<ArgumentException>(() =>
            {
                FisherYates fisherYates = new(min, max);
            });
        }

        [Test]
        public void FisherYates_WhenTooManyNextRandomAreRequested_ThenThrowAnException()
        {
            const int max = 0;
            FisherYates fisherYates = new(max);
            fisherYates.GetNextRandom();
            Assert.Catch<InvalidOperationException>(() =>
            {
                fisherYates.GetNextRandom();
            });
        }

        [Test]
        public void FisherYates_WhenTooManyNextRandomAreRequestedButTheStructureIsCorrectlyReset_ThenReturnTheValues()
        {
            const int max = 1;
            const int min = max;
            FisherYates fisherYates = new(min, max);
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
        public void FisherYates_WhenTooManyNextRandomAreRequestedButTheStructureIsCorrectlyResetAfterAnExceptionCatch_ThenReturnTheValues()
        {
            const int max = 2;
            FisherYates fisherYates = new(max);
            fisherYates.GetNextRandom();
            fisherYates.GetNextRandom();
            fisherYates.GetNextRandom();
            Assert.Catch<InvalidOperationException>(() =>
            {
                fisherYates.GetNextRandom();
            });
            fisherYates.Reset();

            AssertThatFisherYatesCompletelyRandomizeTheRange(fisherYates, max);


        }

        [Test]
        public void FisherYates_WhenConsecutivesGetAllRemainigRandomizedAreRequestedWithoutReset_ThenThrowException()
        {
            const int max = 2;
            FisherYates fisherYates = new(max);
            fisherYates.GetAllRemainingRandomized();
            Assert.Catch<InvalidOperationException>(() =>
            {
                fisherYates.GetAllRemainingRandomized();
            });

        }


        [Test]
        public void FisherYates_WhenConsecutivesGetAllRemainigRandomizedAreRequestedWithReset_ThenThrowsNoExceptions()
        {
            const int max = 2;
            FisherYates fisherYates = new(max);
            fisherYates.GetAllRemainingRandomized();
            fisherYates.Reset();
            fisherYates.GetAllRemainingRandomized();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FisherYates_Distribution_IsUniform_ChiSquare(bool useReset)
        {
            //H0: The distribution is uniform
            //H1: The distribution is not uniform
            const int max = 3;
            const int n = max + 1;
            const int trials = 200_000;
            const double chiSquareCriticalValue = 21.666; // FROM TABLES: Chi-square critical @p=0.01 with 9 degrees of freedom (maxtrix n x n -> 4*4)
            const double expected = trials / (double)n;

            int[,] numOfValOccurencesInPosition = new int[n, n]; //numOfValOccurencesInPosition[0,2] = how many times number 2 appeared in position 0

            FisherYates fisherYates = new(max);
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
        public void FisherYates_ExtractKRandomNumbers_WhenTooManyNumbersAreRequestedFromBeginning_ThenThrowAnException()
        {
            const int max = 10;
            FisherYates fisherYates = new(max);
            Assert.Catch<ArgumentException>(() =>
            {
                fisherYates.ExtractKRandomNumbers(max + 2);
            });
        }

        [Test]
        public void FisherYates_ExtractKRandomNumbers_WhenTooManyNumbersAreRequestedAfterSomeExtractions_ThenThrowAnException()
        {
            const int max = 10;
            const int k = 5;
            const int remaining = max - k;

            FisherYates fisherYates = new(max);
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
        public void FisherYates_ExtractKRandomNumbers_WhenExactlyBagSizeAsKIsRequested_ThenReturnsTheTheValues()
        {
            const int max = 10;
            FisherYates fisherYates = new(max);
            var result = fisherYates.ExtractKRandomNumbers(max);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Count(), Is.EqualTo(max));
                Assert.That(result, Is.Unique);
            }
        }


    }
}
namespace FisherYates;

public class FisherYatesBag : IFisherYatesBag
{
    private int _remainingCount;
    private readonly int _min;
    private readonly int _max;
    private readonly int[] _possibleValues;

    public int RemainingCount => _remainingCount;


    public FisherYatesBag(int min, int max)
    {
        if (max < min) throw new ArgumentException($"{nameof(max)} must be greater or equal to {nameof(min)}.");
        if(max > 1_000_000) throw new ArgumentException($"{nameof(max)} is too large. Maximum allowed is 1,000,000.");
        
        _min = min;
        _max = max;
        int count = _max - _min + 1;
        _possibleValues = new int[count];
        _remainingCount = count;

        for (int i = 0; i < count; i++) _possibleValues[i] = _min + i;
    }

    public FisherYatesBag(int max) : this(0, max) { }


    public bool TryGetNextRandom(out int? value) {
        value = null;
        if (_remainingCount <= 0) return false;
        value = GetNextRandom();
        return true;

    }

    public int GetNextRandom()
    {
        if(_remainingCount <= 0) throw new InvalidOperationException("No more unique random numbers available.");
        int randomIndex = Random.Shared.Next(0, _remainingCount);
        int value = _possibleValues[randomIndex];
        int lastPosition = _remainingCount - 1;
        (_possibleValues[randomIndex], _possibleValues[lastPosition]) = (_possibleValues[lastPosition], _possibleValues[randomIndex]);
        _remainingCount--;
        return value;

    }

    public void Reset()  => _remainingCount = _possibleValues.Length;

    public IEnumerable<int> GetAllRandomized()
    {
        if(_remainingCount < _possibleValues.Length) Reset();
        return GetAllRemainingRandomized();       
    }

    public IEnumerable<int> GetAllRemainingRandomized()
    {
        if (_remainingCount <= 0) throw new InvalidOperationException("No more unique random numbers available.");
        List<int> results = new(_remainingCount);
        while (_remainingCount > 0) results.Add(GetNextRandom());
        return results;
    }

    public IEnumerable<int> ExtractKRandomNumbers(int k)
    {
        if(k > _remainingCount) throw new ArgumentException($"Too many numbers requested.");
        List<int> results = new(k);
        while (k > 0)
        {
            results.Add(GetNextRandom());
            k--;
        }
        return results;
    }
}

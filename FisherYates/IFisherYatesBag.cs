namespace FisherYates;

public interface IFisherYatesBag
{
    /// <summary>
    /// Just update the RemainingCount to the total number of possible values.
    /// (WARNING: this method doesn't reset the possible values array to the initial state!)
    /// </summary>
    public void Reset();


    /// <summary>
    /// Extract k random numbers from the bag.
    /// </summary>
    /// <param name="k">The number of numbers to extract</param>
    /// <returns>The extracted numbers</returns>
    public IEnumerable<int> ExtractKRandomNumbers(int k);

    /// <summary>
    /// Get the next random number from the bag.
    /// </summary>
    /// <returns></returns>
    public int GetNextRandom();

    /// <summary>
    /// Try to get the next random number from the bag.
    /// </summary>
    /// <param name="value">The next random number from the bag</param>
    /// <returns>true if a value was found, false otherwise</returns>
    public bool TryGetNextRandom(out int? value);

    /// <summary>
    /// Reset the bag and return all the possible values in a random order (consuming the bag).
    /// </summary>
    /// <returns></returns>
    public IEnumerable<int> GetAllRandomized();

    /// <summary>
    /// Return all reamining possible values from the bag in a random order (consuming the bag).
    /// </summary>
    /// <returns></returns>
    public IEnumerable<int> GetAllRemainingRandomized();

    /// <summary>
    /// Get the number of remaining possible values in the bag.
    /// </summary>
    public int RemainingCount { get; }
}

using System.Text;

namespace ProductCatalog.Domain.Search;

/// <summary>
/// Generic in-memory search with fuzzy matching and weighted field scoring. BCL only.
/// </summary>
public class ProductSearchEngine<T> where T : class
{
    private readonly Func<T, string>[] _textSelectors;
    private readonly int[] _weights;
    private IReadOnlyList<T>? _source;

    public ProductSearchEngine(IEnumerable<KeyValuePair<Func<T, string>, int>> fieldWeights)
    {
        var list = fieldWeights.ToList();
        _textSelectors = list.Select(x => x.Key).ToArray();
        _weights = list.Select(x => x.Value).ToArray();
    }

    public void SetSource(IReadOnlyList<T> source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public IReadOnlyList<T> Search(string? query, int? maxResults = null)
    {
        if (_source is null || _source.Count == 0)
            return Array.Empty<T>();

        if (string.IsNullOrWhiteSpace(query))
            return _source;

        var normalizedQuery = Normalize(query);
        var scored = new List<(T Item, int Score)>();

        foreach (var item in _source)
        {
            int score = 0;
            for (int i = 0; i < _textSelectors.Length; i++)
            {
                var text = _textSelectors[i](item);
                if (string.IsNullOrEmpty(text)) continue;
                var normalizedText = Normalize(text);
                score += WeightedFuzzyScore(normalizedQuery, normalizedText, _weights[i]);
            }
            if (score > 0)
                scored.Add((item, score));
        }

        var ordered = scored.OrderByDescending(x => x.Score).Select(x => x.Item).ToList();
        if (maxResults.HasValue && maxResults.Value > 0)
            return ordered.Take(maxResults.Value).ToList();
        return ordered;
    }

    private static string Normalize(string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        var sb = new StringBuilder(s.Length);
        foreach (var c in s.ToLowerInvariant())
            if (char.IsLetterOrDigit(c) || c == ' ')
                sb.Append(c);
        return sb.ToString();
    }

    private static int WeightedFuzzyScore(string query, string text, int weight)
    {
        if (string.IsNullOrEmpty(query) || string.IsNullOrEmpty(text)) return 0;
        if (text.Contains(query, StringComparison.Ordinal))
            return weight * (10 + query.Length);
        int subScore = 0;
        int ti = 0;
        foreach (var qc in query)
        {
            while (ti < text.Length && text[ti] != qc) ti++;
            if (ti < text.Length)
            {
                subScore += 2;
                ti++;
            }
            else
                return 0;
        }
        return weight * (1 + subScore);
    }

}



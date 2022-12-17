namespace OpenLR.Networks;

internal class MatchScoreCache
{
    private float?[] _cache;

    public MatchScoreCache()
    {
        _cache = new float?[1024];
    }

    public float? Get(uint edgeTypeId)
    {
        if (_cache.Length <= edgeTypeId)
        {
            return null;
        }

        return _cache[edgeTypeId];
    }

    public void Set(uint edgeTypeId, float factor)
    {
        var cache = _cache;
        if (cache.Length <= edgeTypeId)
        {
            var newSize = _cache.Length + 1024;
            while (newSize <= edgeTypeId)
            {
                newSize += 1024;
            }

            var newCache = new float?[newSize];
            cache.CopyTo(newCache, 0);

            newCache[edgeTypeId] = factor;
            _cache = newCache;
            return;
        }

        cache[edgeTypeId] = factor;
    }
}

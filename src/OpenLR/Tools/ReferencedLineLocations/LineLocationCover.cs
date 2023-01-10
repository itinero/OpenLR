using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Itinero.Network;

namespace OpenLR.Tools.LineLocations;

public class LineLocationCover : IEnumerable<(EdgeId edgeId, bool forward, ushort tail, ushort head)>
{
    private Dictionary<(EdgeId edgeId, bool forward), (ushort tail, ushort head)> _coverage = new();

    /// <summary>
    /// Adds a new edge cover.
    /// </summary>
    /// <param name="edge">The edge id.</param>
    /// <param name="forward">The forward flag.</param>
    /// <param name="tailOffset">The tail offset.</param>
    /// <param name="headOffset">The head offset.</param>
    /// <exception cref="Exception"></exception>
    public void Add(EdgeId edge, bool forward, ushort tailOffset = 0, ushort headOffset = ushort.MaxValue)
    {
        if (tailOffset > headOffset) throw new Exception("Tail offset has to be smaller than head offset");

        // when offsets are equal, nothing is covered.
        if (tailOffset == headOffset) return;

        // get existing offsets, if any.
        var e = (edge, forward);
        if (_coverage.TryGetValue(e, out var offsets))
        {
            if (offsets.tail < tailOffset) tailOffset = offsets.tail;
            if (offsets.head > headOffset) headOffset = offsets.head;
        }

        // write offsets.
        _coverage[e] = (tailOffset, headOffset);
    }

    public IEnumerator<(EdgeId edgeId, bool forward, ushort tail, ushort head)> GetEnumerator()
    {
        return _coverage.Select(x => (x.Key.edgeId, x.Key.forward, x.Value.tail, x.Value.head)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

// using System.Collections;
// using System.Collections.Generic;
//
// namespace OpenLR.Tools.ApplyCover;
//
// internal class EdgeCover : IEnumerable<(ushort tailOffset, ushort headOffset, bool forward, bool backward)>
// {
//     private readonly List<(ushort tailOffset, ushort headOffset)> _forwardCovers = new();
//     private readonly List<(ushort tailOffset, ushort headOffset)> _backwardCovers = new();
//
//     public void Add(ushort tailOffset, ushort headOffset, bool forward)
//     {
//         if (forward)
//         {
//             AddStatic(_forwardCovers, tailOffset, headOffset);
//         }
//         else
//         {
//             AddStatic(_backwardCovers, tailOffset, headOffset);
//         }
//     }
//
//     private static void AddStatic(List<(ushort tailOffset, ushort headOffset)> covers, ushort tailOffset, ushort headOffset)
//     {
//         var next = -1;
//         for (var c = 0; c < covers.Count; c++)
//         {
//             var cover = covers[c];
//
//             if (cover.tailOffset > headOffset)
//             {
//                 next = c;
//                 continue;
//             }
//             if (cover.headOffset < tailOffset) continue;
//             
//             // there is overlap.
//             // expand in both direction.
//             if (cover.tailOffset < tailOffset) tailOffset = cover.tailOffset;
//             covers[c] = (tailOffset, headOffset);
//             
//             // look at the next and get rid of overlaps.
//             while (c + 1 < covers.Count)
//             {
//                 var nextCover = covers[c];
//                 if (nextCover.tailOffset > headOffset) break;
//                 
//                 // remove next cover, it overlaps, but expand previous with next head.
//                 covers[c] = (tailOffset, nextCover.headOffset);
//             }
//
//             return;
//         }
//
//         if (next == -1)
//         {
//             covers.Add((tailOffset, headOffset));
//             return;
//         }
//         
//         covers.Insert(next, (tailOffset, headOffset));
//     }
//
//     private IEnumerable<(ushort tailOffset, ushort headOffset, bool forward, bool backward)>
//         EnumerateForwardAndBackward()
//     {
//         var f = 0;
//         var b = 0;
//
//         ushort tail = 0;
//         while (true)
//         {
//             if (f < _forwardCovers.Count && b < _backwardCovers.Count)
//             {
//                 var nextForward = _forwardCovers[f];
//                 var nextBackward = _backwardCovers[b];
//
//                 if (tail > nextForward.tailOffset) nextForward = (tail, nextForward.headOffset);
//                 if (tail >= nextForward.headOffset)
//                 {
//                     f++;
//                     continue;
//                 }
//                 if (tail > nextBackward.tailOffset) nextBackward = (tail, nextBackward.headOffset);
//                 if (tail >= nextBackward.headOffset)
//                 {
//                     b++;
//                     continue;
//                 }
//
//                 if (nextForward.tailOffset < nextBackward.tailOffset)
//                 {
//                     // first return next forward until backward tail.
//                     yield return (nextForward.tailOffset, nextBackward.tailOffset, true, false);
//                     tail = nextBackward.tailOffset;
//                     continue;
//                 }
//
//                 if (nextBackward.tailOffset < nextForward.tailOffset)
//                 {
//                     // first return next backward until forward tail.
//                     yield return (nextBackward.tailOffset, nextForward.tailOffset, true, false);
//                     tail = nextForward.tailOffset;
//                     continue;
//                 }
//             }
//             else if (f < _forwardCovers.Count)
//             {
//                 var nextForward = _forwardCovers[f];
//
//                 if (tail > nextForward.tailOffset) nextForward = (tail, nextForward.headOffset);
//                 if (tail >= nextForward.headOffset)
//                 {
//                     f++;
//                     continue;
//                 }
//                 
//                 yield return (nextBackward.tailOffset, nextForward.tailOffset, true, false);
//             }
//             else if (b < _backwardCovers.Count)
//             {
//                 
//             }
//             
//             break;
//         }
//     }
//
//     public IEnumerator<(ushort tailOffset, ushort headOffset, bool forward, bool backward)> GetEnumerator()
//     {
//         return this.EnumerateForwardAndBackward().GetEnumerator();
//     }
//
//     IEnumerator IEnumerable.GetEnumerator()
//     {
//         return this.GetEnumerator();
//     }
// }

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooniverseAPI.Mappings.Utils;

public class ChunkedExecutor<T, TR>
{
    private readonly IList<T> _items;
    private readonly int _chunkSize;
    private readonly Func<T, Task<TR>> _executor;
    private readonly Action<IList<T>> _perChunkCallback;
    private readonly Action<IList<TR>> _perResultCallback;

    public ChunkedExecutor(IList<T> items, int chunkSize, Func<T, Task<TR>> executor,
        Action<IList<T>> perChunkCallback = null, Action<IList<TR>> perResultCallback = null)
    {
        _items = items;
        _chunkSize = chunkSize;
        _executor = executor;
        _perChunkCallback = perChunkCallback;
        _perResultCallback = perResultCallback;
    }

    public async Task<IList<TR>> Execute()
    {
        var results = new ConcurrentBag<TR>();

        for (var i = 0; i < _items.Count; i += _chunkSize)
        {
            var chunk = _items.Skip(i).Take(_chunkSize).ToList();

            _perChunkCallback?.Invoke(chunk);

            var tasks = chunk.Select(async item =>
            {
                var result = await _executor(item);
                if (result != null) results.Add(result);
            }).ToList();

            await Task.WhenAll(tasks);
            _perResultCallback?.Invoke(results.ToList());
        }

        return results.ToList();
    }
}
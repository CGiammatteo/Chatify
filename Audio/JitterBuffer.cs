using System.Collections.Concurrent;

namespace PicoVoiceClient.Audio;

public sealed class JitterBuffer<T>
{
    private readonly ConcurrentQueue<T> _queue = new();
    private readonly int _target;

    public JitterBuffer(int targetFrames)
    {
        _target = targetFrames;
    }

    public void Push(T frame) => _queue.Enqueue(frame);

    public bool TryPop(out T frame) => _queue.TryDequeue(out frame);

    public bool Ready => _queue.Count >= _target;
}
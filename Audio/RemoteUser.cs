using System.Collections.Concurrent;

namespace PicoVoiceClient.Audio;

public sealed class RemoteUser
{
    public byte Id { get; init; }

    public JitterBuffer<float[]> Jitter { get; init; }

    public ConcurrentQueue<float> FloatQueue { get; init; }
}
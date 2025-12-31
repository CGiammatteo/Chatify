using NAudio.Wave;
using System.Collections.Concurrent;

namespace PicoVoiceClient.Audio;

public sealed class Pcm16SampleProvider : ISampleProvider
{
    private readonly ConcurrentQueue<short> _queue;
    private readonly WaveFormat _format;

    public Pcm16SampleProvider(ConcurrentQueue<short> queue)
    {
        _queue = queue;
        _format = WaveFormat.CreateIeeeFloatWaveFormat(
            Config.SAMPLE_RATE,
            1
        );
    }

    public WaveFormat WaveFormat => _format;

    public int Read(float[] buffer, int offset, int count)
    {
        for (int i = 0; i < count; i++)
        {
            buffer[offset + i] =
                _queue.TryDequeue(out var s)
                    ? s / 32768f
                    : 0f;
        }
        return count;
    }
}
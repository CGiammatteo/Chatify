using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Collections.Concurrent;

namespace PicoVoiceClient.Audio;

public sealed class AudioPlayback
{
    private readonly WaveFormat _format;
    private readonly MixingSampleProvider _mixer;
    private readonly WaveOutEvent _output;

    private readonly ConcurrentDictionary<byte, RemoteUser> _users = new();

    public AudioPlayback()
    {
        _format = WaveFormat.CreateIeeeFloatWaveFormat(
            Config.SAMPLE_RATE,
            Config.CHANNELS
        );

        _mixer = new MixingSampleProvider(_format)
        {
            ReadFully = true
        };

        _output = new WaveOutEvent();
        _output.Init(_mixer);
    }

    public void Start()
    {
        _output.Play();
        Console.WriteLine("Audio output started");
    }

    public void PushDecodedFrame(byte senderId, short[] pcm16)
    {
        var user = _users.GetOrAdd(senderId, id =>
        {
            var q = new ConcurrentQueue<float>();
            var provider = new FloatQueueSampleProvider(_format, q);

            _mixer.AddMixerInput(provider);
            Console.WriteLine($"User {id} joined");

            return new RemoteUser
            {
                Id = id,
                Jitter = new JitterBuffer<float[]>(targetFrames: 4),
                FloatQueue = q
            };
        });

        float[] frame = new float[pcm16.Length];
        for (int i = 0; i < pcm16.Length; i++)
            frame[i] = pcm16[i] / 32768f;

        user.Jitter.Push(frame);
    }

    public void DrainJitterBuffers()
    {
        foreach (var user in _users.Values)
        {
            if (!user.Jitter.Ready)
                continue;

            if (user.Jitter.TryPop(out var frame))
            {
                foreach (var sample in frame)
                    user.FloatQueue.Enqueue(sample);
            }
        }
    }

    private sealed class FloatQueueSampleProvider : ISampleProvider
    {
        private readonly ConcurrentQueue<float> _queue;
        private readonly WaveFormat _format;

        public FloatQueueSampleProvider(
            WaveFormat format,
            ConcurrentQueue<float> queue)
        {
            _format = format;
            _queue = queue;
        }

        public WaveFormat WaveFormat => _format;

        public int Read(float[] buffer, int offset, int count)
        {
            int written = 0;

            for (int i = 0; i < count; i++)
            {
                if (_queue.TryDequeue(out float s))
                    buffer[offset + i] = s;
                else
                    buffer[offset + i] = 0f;

                written++;
            }

            return written;
        }
    }
}

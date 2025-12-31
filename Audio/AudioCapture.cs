using NAudio.Wave;
using System.Collections.Concurrent;

namespace PicoVoiceClient.Audio;

public sealed class AudioCapture
{
    private readonly WaveInEvent _waveIn;

    public event Action<short[]>? OnSamples;

    public AudioCapture()
    {
        _waveIn = new WaveInEvent
        {
            WaveFormat = new WaveFormat(
                Config.SAMPLE_RATE,
                16,
                Config.CHANNELS
            ),
            BufferMilliseconds = Config.FRAME_MS,
            NumberOfBuffers = 3
        };

        _waveIn.DataAvailable += OnData;
    }

    private void OnData(object? _, WaveInEventArgs e)
    {
        int samples = e.BytesRecorded / 2;
        short[] pcm = new short[samples];

        Buffer.BlockCopy(e.Buffer, 0, pcm, 0, e.BytesRecorded);
        OnSamples?.Invoke(pcm);
    }

    public void Start()
    {
        _waveIn.StartRecording();
        Console.WriteLine("Microphone active");
    }
}

using Concentus;
using Concentus.Enums;

namespace PicoVoiceClient.Audio;

public sealed class OpusEncoderWrapper
{
    private readonly IOpusEncoder _encoder;

    public OpusEncoderWrapper()
    {
        _encoder = OpusCodecFactory.CreateEncoder(
            Config.SAMPLE_RATE,
            Config.CHANNELS,
            OpusApplication.OPUS_APPLICATION_VOIP
        );

        _encoder.Bitrate = 32000;   
        _encoder.UseVBR = true;
        _encoder.Complexity = 5;
        _encoder.SignalType = OpusSignal.OPUS_SIGNAL_VOICE;
    }

    public byte[] Encode(short[] pcm)
    {
        byte[] buffer = new byte[Config.OPUS_MAX_PACKET];

        int len = _encoder.Encode(
            pcm.AsSpan(),
            Config.SAMPLES_PER_FRAME,
            buffer.AsSpan(),
            buffer.Length
        );

        return buffer[..len];
    }

}

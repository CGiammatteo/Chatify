using Concentus;

namespace PicoVoiceClient.Audio;

public sealed class OpusDecoderWrapper
{
    private readonly IOpusDecoder _decoder;

    public OpusDecoderWrapper()
    {
        _decoder = OpusCodecFactory.CreateDecoder(
            Config.SAMPLE_RATE,
            Config.CHANNELS
        );
    }

    public short[] Decode(byte[] opusData)
    {
        short[] pcm = new short[Config.SAMPLES_PER_FRAME];

        _decoder.Decode(
            opusData.AsSpan(),
            pcm.AsSpan(),
            pcm.Length,
            false
        );

        return pcm;
    }
}

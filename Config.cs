namespace PicoVoiceClient;

public static class Config
{
    public const string SERVER_HOST = "";
    public const int SERVER_PORT = 5005;

    // AUDIO
    public const int SAMPLE_RATE = 48000;
    public const int CHANNELS = 1;
    public const int FRAME_MS = 20;

    public const int SAMPLES_PER_FRAME =
        SAMPLE_RATE * FRAME_MS / 1000;

    // OPUS
    public const int OPUS_MAX_PACKET = 400;

    // NETWORK
    public const int HEADER_BYTES = 1;
}
using PicoVoiceClient.Audio;
using System.Collections.Concurrent;
using PicoVoiceClient.Networking;
using PicoVoiceClient.Utils;

namespace PicoVoiceClient;

class Program
{
    static void Main()
    {
        Console.Title = "Pico Voice Client";

        byte clientId = (byte)Random.Shared.Next(1, 255);
        Console.WriteLine($"Client ID: {clientId}");

        var endpoint = NetUtils.ResolveEndpoint(
            Config.SERVER_HOST,
            Config.SERVER_PORT
        );

        Console.WriteLine($"Target: {endpoint}");

        var playback = new AudioPlayback();
        playback.Start();

        var capture = new AudioCapture();
        var udp = new UdpVoiceClient(endpoint, clientId, playback);

        capture.OnSamples += udp.SendPcmFrame;
        udp.StartReceiving();

        capture.Start();

        Console.WriteLine("Voice running (Ctrl+C to exit)");

        _ = Task.Run(async () =>
        {
            while (true)
            {
                playback.DrainJitterBuffers();
                await Task.Delay(Config.FRAME_MS);
            }
        });

        Thread.Sleep(Timeout.Infinite);
    }
}
using PicoVoiceClient.Audio;
using System.Net;
using System.Net.Sockets;

namespace PicoVoiceClient.Networking;

public sealed class UdpVoiceClient
{
    private readonly UdpClient _udp;
    private readonly IPEndPoint _server;
    private readonly byte _clientId;
    private readonly AudioPlayback _playback;

    private readonly OpusEncoderWrapper _encoder = new();
    private readonly OpusDecoderWrapper _decoder = new();

    public UdpVoiceClient(
        IPEndPoint server,
        byte clientId,
        AudioPlayback playback
    )
    {
        _server = server;
        _clientId = clientId;
        _playback = playback;

        _udp = new UdpClient(0);
        _udp.Client.ReceiveBufferSize = 1 << 20;
    }

    public void StartReceiving()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                UdpReceiveResult res;
                try { res = await _udp.ReceiveAsync(); }
                catch { continue; }

                var data = res.Buffer;
                if (data.Length < 2)
                    continue;

                byte senderId = data[0];
                if (senderId == _clientId)
                    continue;

                byte[] opus = new byte[data.Length - 1];
                Buffer.BlockCopy(data, 1, opus, 0, opus.Length);

                short[] pcm = _decoder.Decode(opus);
               _playback.PushDecodedFrame(senderId, pcm);

            }
        });
    }

    public void SendPcmFrame(short[] pcm)
    {
        var opus = _encoder.Encode(pcm);

        byte[] packet = new byte[1 + opus.Length];
        packet[0] = _clientId;

        Buffer.BlockCopy(opus, 0, packet, 1, opus.Length);

        _udp.Send(packet, packet.Length, _server);
    }
}
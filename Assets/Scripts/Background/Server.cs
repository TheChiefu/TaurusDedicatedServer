using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// Public class regarding status of server, such as port, player count, player data, and etc.
/// </summary>
public class Server
{
    public static ServerData serverData { get; private set; }
    public static bool running { get; private set; }


    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static TcpListener tcpListener;
    private static UdpClient udpListener;


    

    /// <summary>
    /// Start server from given server data values
    /// </summary>
    /// <param name="sd"></param>
    public static void Start(ServerData sd)
    {
        //Copy data from given data
        serverData = sd;

        Console.WriteLine("Starting server...");
        InitializeServerData();

        //Start Listeners
        tcpListener = new TcpListener(IPAddress.Any, sd.port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        udpListener = new UdpClient(sd.port);
        udpListener.BeginReceive(UDPReceiveCallback, null);


        Console.WriteLine($"Server started on port {sd.port}.");
        running = true;
    }

    public static void Stop()
    {
        running = false;
        tcpListener.Stop();
        udpListener.Close();
    }

    /// <summary>
    /// Initializes all necessary server data. MUST update packetHandler with new ClientPackets when new ones are added to ensure data is updated properly.
    /// </summary>
    private static void InitializeServerData()
    {
        //Populate client list with empty clients
        for (int i = 1; i <= serverData.maxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }

        //ServerHandle class manages what to do with the ClientPackets via a function call
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
            { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
            { (int)ClientPackets.playerShoot, ServerHandle.PlayerShoot },
            { (int)ClientPackets.playerThrowItem, ServerHandle.PlayerThrowItem },
            { (int)ClientPackets.playerAnimationData, ServerHandle.PlayerAnimationData },
            { (int)ClientPackets.videoPlayerStatus, ServerHandle.VideoPlayerStatus },
            { (int)ClientPackets.playerCosmetics, ServerHandle.PlayerCosmetics }
        };
        Console.WriteLine("Initialized packets.");
    }


    // Handlers //

    /// <summary>Handles new TCP connections.</summary>
    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= serverData.maxPlayers; i++)
        {
            Console.WriteLine("Connecting player: " + i);
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
        }

        Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
    }

    /// <summary>Receives incoming UDP data.</summary>
    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (_data.Length < 4)
            {
                return;
            }

            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                if (_clientId == 0)
                {
                    return;
                }

                if (clients[_clientId].udp.endPoint == null)
                {
                    // If this is a new connection
                    clients[_clientId].udp.Connect(_clientEndPoint);
                    return;
                }

                if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                {
                    // Ensures that the client is not being impersonated by another by sending a false clientID
                    clients[_clientId].udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Error receiving UDP data: {_ex}");
        }
    }

    /// <summary>Sends a packet to the specified endpoint via UDP.</summary>
    /// <param name="_clientEndPoint">The endpoint to send the packet to.</param>
    /// <param name="_packet">The packet to send.</param>
    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
        }
    }
}
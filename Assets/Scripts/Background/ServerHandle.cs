using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    /// <summary>
    /// Acknowledge client can connect to server
    /// </summary>
    /// <param name="_fromClient"></param>
    /// <param name="_packet"></param>
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {

        PlayerData pd = new PlayerData(
            _packet.ReadInt(),
            _packet.ReadString(),
            _packet.ReadInt(),
            _packet.ReadInt()
            );

        Server.clients[_fromClient].id = pd.id;
        Server.clients[_fromClient].username = pd.username;
        Server.clients[_fromClient].modelIndex = pd.modelIndex;
        Server.clients[_fromClient].materialIndex = pd.materialIndex;

        Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} | \"{pd.username}\" connected successfully and is now player: {_fromClient}.");
        if (_fromClient != pd.id)
        {
            Console.WriteLine($"Player \"{pd.username}\" (ID: {_fromClient}) has assumed the wrong client ID ({pd.id})!");
        }

        // PREVIOUSLY - SEND PLAYER INTO GAME
        // Server.clients[_fromClient].SendIntoGame(pd);

        //Send Map and Gamemode data to connecting client
        
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        Quaternion _rotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
    }

    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        Vector3 _shootDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.Shoot(_shootDirection);
    }

    public static void PlayerThrowItem(int _fromClient, Packet _packet)
    {
        Vector3 _throwDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.ThrowItem(_throwDirection);
    }

    public static void PlayerAnimationData(int _fromClient, Packet _packet)
    {
        float[] _data = new float[_packet.ReadInt()];
        for(int i = 0; i < _data.Length; i++)
        {
            _data[i] = _packet.ReadFloat();
        }

        Player _player = Server.clients[_fromClient].player;

        ServerSend.PlayerAnimationData(_player, _data);
    }

    public static void VideoPlayerStatus(int _fromClient, Packet _packet)
    {
        bool isPlaying = _packet.ReadBool();
        int currentIndex = _packet.ReadInt();

        ServerSend.VideoPlayerStatus(isPlaying, currentIndex);
    }

    public static void PlayerCosmetics(int _fromClient, Packet _packet)
    {

        Player _player = Server.clients[_fromClient].player;
        int  modelIndex = _packet.ReadInt();

        ServerSend.PlayerCosmetics(_player.id, modelIndex);
    }
}
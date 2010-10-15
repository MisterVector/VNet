﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        //User Functions
        //This class is for methods that deal with a single user, not multiple users
        protected void SendLogonResult(User user, LogonResult result)
        {
            //if (result == LogonResult.SUCCESS)
            //{
            //}
            //else
            //{
                user.Packet.Clear()
                    .InsertByte((byte)result)
                    .InsertByte(0) //challenge byte
                    .InsertStringNT("server ver")
                    .InsertStringNT("TestEnv")
                    .InsertDWord(0) //ping
                    .InsertByte(0) //rank
                    .Send(VNET_LOGON);
            //}
        }

        protected void SendServerInfo(User user)
        {

        }

        protected void SendChannelList(User user, Channel channel)
        {
            List<User> users = GetUsersInChannel(user, channel);
            if (users == null)
                return;
            user.Packet.Clear()
                .InsertByte((int)ListType.UsersInChannel);
            foreach (User u in users)
                user.Packet.InsertStringNT(u.Username)
                    .InsertDWord(u.Ping) //ping
                    .InsertByte((byte)u.Flags);
            user.Packet.Send(VNET_CHANNELLIST);
        }

        protected void JoinUserToChannel(User user, Channel channel)
        {
            RemoveUserFromChannel(user);
            channel.AddUser(user, false);
            //TODO: notify channel user joined
            SendChannelList(user, channel);
        }

        protected void RemoveUserFromChannel(User user)
        {
            if (user.Channel == null)
                return;
            //TODO: notify channel user left
            user.Channel.RemoveUser(user);
        }
    }
}

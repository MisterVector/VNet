﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void HandlePacket(User user, PacketReader reader)
        {
            try
            {
                byte packetId = reader.ReadByte();
                switch (packetId)
                {
                    case VNET_LOGON: //0x01
                        string username = reader.ReadStringNT();
                        string password = reader.ReadStringNT();
                        string client = reader.ReadStringNT();
                        byte queueSharing = reader.ReadByte();

                        //check client name

                        //check username+pass combo

                        //remember challenge

                        if (GetUserByName(username) != null)
                        {
                            SendLogonResult(user, LogonResult.ACCOUNT_IN_USE);
                            return;
                        }

                        user.Username = username;
                        user.Client = client;
                        user.IsOnline = true;
                        SendLogonResult(user, LogonResult.SUCCESS);
                        JoinUserToChannel(user, MainChannel);

                        break;
                    case VNET_SERVERCHALLENGE: //0x02
                        break;
                    case VNET_CHATEVENT: //0x03
                        break;
                    case 0x04:
                        break;
                    case 0x05:
                        break;
                    case 0x06:
                        break;
                    case 0x07:
                        break;
                    case 0x08:
                        break;
                    case 0x09:
                        break;
                    case 0x0A:
                        break;
                    case 0x0B:
                        break;
                    case 0x0C:
                        break;
                }
            }
            catch (Exception ex)
            {
                //TODO: log exception
            }
        }

        

    }
}

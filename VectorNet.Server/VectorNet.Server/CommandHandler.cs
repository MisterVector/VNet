﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void HandleCommand(User user, string cmd)
        {
            string[] aryCmd = cmd.ToString().Split(' ');
            string cmd1 = aryCmd[0].ToLower();
            List<string> msgs;
            Channel channel;
            User targetUser;

            switch (cmd1)
            {
                case "users":
                    SendList(user, ListType.UsersOnServer);
                    break;

                case "join":
                case "j":
                    if ((channel = ExtractChannelFromParameterOne(user, ref aryCmd, true, "You must specify a channel.")) == null) return;

                    if (channel.IsUserBanned(user))
                        SendServerError(user, "You are banned from that channel.");
                    else
                        JoinUserToChannel(user, channel);

                    break;

                case "me":
                case "em":
                case "emote":

                    break;

                case "who":
                    if ((channel = ExtractChannelFromParameterOne(user, ref aryCmd, false, "You must specify a channel.")) == null) return;

                    List<User> u = GetUsersInChannel(user, channel, false);
                    if (u.Count == 0)
                        SendServerError(user, "That channel doesn't exist.");
                    else
                    {
                        msgs = new List<string>();
                        msgs.Add("Users in channel " + channel.Name + ":");
                        for (int i = 0; i < u.Count; i++)
                        {
                            if (i % 2 == 0)
                                msgs.Add(u[i].Username);
                            else
                                msgs[msgs.Count - 1] += ", " + u[i].Username;
                        }
                        foreach (string msg in msgs)
                            SendServerInfo(user, msg);
                        msgs = null;
                    }

                    break;

                case "ban":
                case "banip":
                case "ipban":
                    if (RequireOperator(user) == false) return;
                    if ((targetUser = ExtractUserFromParameterOne(user, ref aryCmd, "You must specify a user to ban.")) == null) return;
                    if (RequireModerationRights(user, targetUser) == false) return;

                    if (cmd1 == "ban")
                    {
                        if (user.Channel.BannedUsers.Contains(targetUser.Username))
                            SendServerError(user, "That user is already banned from this channel.");
                        else
                            BanUserByUsername(user, targetUser, user.Channel);
                    }
                    else
                    {
                        if (user.Channel.BannedIPs.Contains(targetUser.IPAddress))
                            SendServerError(user, "That user's IP is already banned from this channel.");
                        else
                            BanUserByIP(user, targetUser, user.Channel);
                    }
                                    
                    break;

                case "unban":
                case "unipban":
                case "unbanip":
                    if (RequireOperator(user) == false) return;
                    if ((targetUser = ExtractUserFromParameterOne(user, ref aryCmd, "You must specify a user to unban.")) == null) return;
                    if (RequireModerationRights(user, targetUser) == false) return;


                    if (cmd1 == "unban")
                    {
                        if (!user.Channel.IsUserBanned(targetUser))
                            SendServerError(user, "That user is not banned from this channel.");
                        else
                            UnbanUser(user, targetUser, user.Channel, false);
                    }
                    else
                    {
                        if (!user.Channel.IsUserBanned(targetUser))
                            SendServerError(user, "That user is not IP banned.");
                        else
                            UnbanUserByIP(user, targetUser, user.Channel);
                    }

                    break;

                case "op":
                    if (RequireOperator(user) == false) return;
                    if ((targetUser = ExtractUserFromParameterOne(user, ref aryCmd, "You must specify a user to promote to Operator.")) == null) return;
                    if (RequireModerationRights(user, targetUser) == false) return;


                    if (user.Channel == targetUser.Channel)
                        ; //TODO: Determine environment for opping users
                    else
                        SendServerError(user, "That user is not in the same channel as you.");

                    break;

                default:
                    SendServerError(user, "That is not a valid command.");
                    break;
            }
        }

        protected User ExtractUserFromParameterOne(User user, ref string[] str, string failMsgTooShort)
        {
            if (RequireParameterOne(user, ref str, failMsgTooShort) == false)
                return null;

            User ret = GetUserByName(str[1]);
            if (ret == null)
                SendServerError(user, "There is no user by the name \"" + str[1] + "\" online.");

            return ret;
        }

        protected Channel ExtractChannelFromParameterOne(User user, ref string[] str, bool allowCreation, string failMsgTooShort)
        {
            if (RequireParameterOne(user, ref str, failMsgTooShort) == false)
                return null;

            string cmd = String.Join(" ", str);
            cmd = cmd.Substring(cmd.IndexOf(' ') + 1);

            Channel ret = GetChannelByName(user, cmd, allowCreation);
            if (ret == null)
                SendServerError(user, "That channel does not exist.");

            return ret;
        }

        protected bool RequireParameterOne(User user, ref string[] str, string failMsgTooShort)
        {
            if (str.Length >= 2 && str[1].Length > 0)
                return true;

            SendServerError(user, failMsgTooShort);
            return false;
        }

        protected bool RequireAdmin(User user)
        {
            if (user.Flags == UserFlags.Admin)
                return true;
            SendServerError(user, "You must be an Admin to use that command.");
            return false;
        }

        protected bool RequireModerator(User user)
        {
            if (user.Flags == UserFlags.Admin
                || user.Flags == UserFlags.Moderator)
                return true;
            SendServerError(user, "You must be a Moderator or higher to use that command.");
            return false;
        }

        protected bool RequireOperator(User user)
        {
            if (user.Flags == UserFlags.Admin
                || user.Flags == UserFlags.Moderator
                || user.Flags == UserFlags.Operator)
                return true;
            SendServerError(user, "You must be an Operator or higher to use that command.");
            return false;
        }

        protected bool RequireModerationRights(User user, User targetUser)
        {
            if (CanUserModerateUser(user, targetUser))
                return true;
            SendServerError(user, "You do not have sufficient rights to performs actions on that user.");
            return false;
        }

        public void HandleConsoleCommand(string cmd)
        {
            HandleCommand(console, cmd);
        }

        protected bool ContainsNonPrintable(string str)
        {
            foreach (char c in str)
                if ((byte)c < 32)
                    return true;
            return false;
        }

    }
}

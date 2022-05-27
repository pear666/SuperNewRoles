﻿using Hazel;
using SuperNewRoles.CustomRPC;
using SuperNewRoles.Helpers;
using SuperNewRoles.Mode;
using System;
using System.Collections.Generic;
using System.Text;
using SuperNewRoles.EndGame;

namespace SuperNewRoles.Roles
{
    public static class Arsonist
    {
        public static void ArsonistDouse(this PlayerControl target, PlayerControl source = null)
        {
            try
            {
                if (source == null) source = PlayerControl.LocalPlayer;
                MessageWriter Writer = RPCHelper.StartRPC(CustomRPC.CustomRPC.ArsonistDouse);
                Writer.Write(source.PlayerId);
                Writer.Write(target.PlayerId);
                Writer.EndRPC();
                RPCProcedure.ArsonistDouse(source.PlayerId, target.PlayerId);
            } catch (Exception e)
            {
                SuperNewRolesPlugin.Logger.LogError(e);
            }
        }

        public static List<PlayerControl> GetDouseData(this PlayerControl player)
        {
            return RoleClass.Arsonist.DouseDatas.ContainsKey(player.PlayerId) ? RoleClass.Arsonist.DouseDatas[player.PlayerId] : new List<PlayerControl>();
        }

        public static List<PlayerControl> GetUntarget()
        {
            if (RoleClass.Arsonist.DouseDatas.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
            {
                return RoleClass.Arsonist.DouseDatas[PlayerControl.LocalPlayer.PlayerId];
            }
            return new List<PlayerControl>();
        }

        public static bool IsDoused(this PlayerControl source, PlayerControl target)
        {
            if (source == null || source.Data.Disconnected || target == null || target.Data.Disconnected || target.IsBot()) return true;
            if (source.PlayerId == target.PlayerId) return true;
            if (RoleClass.Arsonist.DouseDatas.ContainsKey(source.PlayerId))
            {
                if (RoleClass.Arsonist.DouseDatas[source.PlayerId].IsCheckListPlayerControl(target))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<PlayerControl> GetIconPlayers(PlayerControl player = null)
        {
            if (player == null) player = PlayerControl.LocalPlayer;
            if (RoleClass.Arsonist.DouseDatas.ContainsKey(player.PlayerId))
            {
                return RoleClass.Arsonist.DouseDatas[player.PlayerId];
            }
            return new List<PlayerControl>();
        }
        public static bool IsViewIcon(PlayerControl player) {
            if (player == null) return false;
            foreach (var data in RoleClass.Arsonist.DouseDatas)
            {
                foreach (PlayerControl Player in data.Value)
                {
                    if (player.PlayerId == Player.PlayerId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsButton() {
            return RoleHelpers.isAlive(PlayerControl.LocalPlayer) && PlayerControl.LocalPlayer.isRole(RoleId.Arsonist) && ModeHandler.isMode(ModeId.Default);
        }

        public static bool IsWin(PlayerControl Arsonist)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId != Arsonist.PlayerId && !IsDoused(Arsonist, player))
                {
                    return false;
                }
            }
            if (Arsonist.isDead()) return false;
            return true;
        }

        public static bool IsArsonistWinFlag()
        {
            foreach (PlayerControl player in RoleClass.Arsonist.ArsonistPlayer)
            {
                if (IsWin(player))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool SetArsonistWin()
        {
            if (IsArsonistWinFlag()) 
            {
                return true;
            }
            return false;
        }
        public static void ArsonistWin()
        {
            if (RoleClass.Arsonist.ArsonistWin)
            {
                RPCProcedure.SetWinArsonist();
            }
        }
    }
}

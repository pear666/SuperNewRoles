using System.Collections.Generic;
using Hazel;
using SuperNewRoles.Helpers;

namespace SuperNewRoles.Sabotage
{
    public static class SabotageManager
    {
        public static CustomSabotage thisSabotage;
        public static List<ButtonBehavior> CustomButtons;
        public enum CustomSabotage
        {
            None,
            CognitiveDeficit
        }
        public static bool IsOK(CustomSabotage sabotage)
        {
            if (!Options.SabotageSetting.GetBool()) return false;
            return sabotage switch
            {
                CustomSabotage.CognitiveDeficit => PlayerControl.GameOptions.MapId == 4 && Options.CognitiveDeficitSetting.GetBool(),
                _ => false,
            };
        }
        public static bool IsOKMeeting()
        {
            return RoleHelpers.IsSabotage()
                ? false
                : thisSabotage == CustomSabotage.None
|| thisSabotage switch
                {
                    CustomSabotage.CognitiveDeficit => CognitiveDeficit.main.IsLocalEnd,
                    _ => false,
                };
        }
        public static InfectedOverlay InfectedOverlayInstance;
        public const float SabotageMaxTime = 30f;
        public static void SetSabotage(PlayerControl player, CustomSabotage Sabotage, bool Is)
        {
            switch (Sabotage)
            {
                case CustomSabotage.CognitiveDeficit:
                    if (Is)
                    {
                        CognitiveDeficit.main.StartSabotage();
                    }
                    else
                    {
                        CognitiveDeficit.main.EndSabotage(player);
                    }
                    break;
            }
        }
        public static void ClearAndReloads()
        {
            InfectedOverlayInstance = null;
            thisSabotage = CustomSabotage.None;
            CustomButtons = new List<ButtonBehavior>();
            if (IsOK(CustomSabotage.CognitiveDeficit))
            {
                CognitiveDeficit.main.DefaultDistanceTime = Options.CognitiveDeficitReleaseTimeSetting.GetFloat();
                CognitiveDeficit.main.DefaultUpdateTime = Options.CognitiveDeficitOutfitUpdateTimeSetting.GetFloat();
                CognitiveDeficit.main.IsAllEndSabotage = Options.CognitiveDeficitIsAllEndSabotageSetting.GetBool();
            }
        }
        public static void Update()
        {
            if (CustomButtons.Count > 0)
            {
                if (InfectedOverlayInstance != null)
                {
                    float specialActive = (InfectedOverlayInstance.doors != null && InfectedOverlayInstance.doors.IsActive) ? 1f : InfectedOverlayInstance.SabSystem.PercentCool;
                    foreach (ButtonBehavior button in CustomButtons)
                    {
                        button.spriteRenderer.material.SetFloat("_Percent", specialActive);
                    }
                }
            }
            switch (thisSabotage)
            {
                case CustomSabotage.CognitiveDeficit:
                    CognitiveDeficit.main.Update();
                    break;
            }
        }
        public static void CustomSabotageRPC(PlayerControl p, CustomSabotage type, bool Is)
        {
            MessageWriter writer = RPCHelper.StartRPC(CustomRPC.CustomRPC.SetCustomSabotage);
            writer.Write(CachedPlayer.LocalPlayer.PlayerId);
            writer.Write((byte)type);
            writer.Write(Is);
            writer.EndRPC();
            SetSabotage(p, type, Is);
        }
    }
}
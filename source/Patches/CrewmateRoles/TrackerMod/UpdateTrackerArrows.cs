using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.ImpostorRoles.CamouflageMod;
using TownOfUs.Extensions;
using System;

namespace TownOfUs.CrewmateRoles.TrackerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateTrackerArrows
    {
        public static Sprite Sprite => TownOfUs.Arrow;
        private static DateTime _time = DateTime.UnixEpoch;
        private static float Interval => CustomGameOptions.UpdateInterval;
        public static void Postfix(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Tracker)) return;

            var role = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);

            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                var tracker = (Tracker) role;
                if (PlayerControl.LocalPlayer.Data.IsDead || tracker.Player.Data.IsDead)
                {
                    role.TrackerArrows.Values.DestroyAll();
                    role.TrackerArrows.Clear();
                    return;
                }

                foreach (var arrow in role.TrackerArrows)
                {
                    var player = Utils.PlayerById(arrow.Key);
                    if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected)
                    {
                        role.DestroyArrow(arrow.Key);
                        continue;
                    }
                    if (!CamouflageUnCamouflage.IsCamoed)
                        if (RainbowUtils.IsRainbow(player.GetDefaultOutfit().ColorId))
                        {
                            arrow.Value.image.color = RainbowUtils.Rainbow;
                        }
                        else
                        {
                            arrow.Value.image.color = Palette.PlayerColors[player.GetDefaultOutfit().ColorId];
                        }
                    else
                    {
                        arrow.Value.image.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                    }

                    if (_time <= DateTime.UtcNow.AddSeconds(-Interval))
                        arrow.Value.target = player.transform.position;
                }
                if (_time <= DateTime.UtcNow.AddSeconds(-Interval))
                    _time = DateTime.UtcNow;
            }
        }
    }
}
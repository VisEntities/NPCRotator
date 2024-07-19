/*
 * Copyright (C) 2024 Game4Freak.io
 * This mod is provided under the Game4Freak EULA.
 * Full legal terms can be found at https://game4freak.io/eula/
 */

using Rust;
using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("NPC Rotator", "VisEntities", "1.0.0")]
    [Description(" ")]
    public class NPCRotator : RustPlugin
    {
        #region Fields

        private static NPCRotator _plugin;
        private const int LAYER_PLAYER = Layers.Mask.Player_Server;

        #endregion Fields

        #region Oxide Hooks

        private void Init()
        {
            _plugin = this;
            PermissionUtil.RegisterPermissions();
        }

        private void Unload()
        {
            _plugin = null;
        }

        private void OnPlayerInput(BasePlayer player, InputState input)
        {
            if (player == null || input == null)
                return;

            if (!PermissionUtil.HasPermission(player, PermissionUtil.USE))
                return;

            Item activeItem = player.GetActiveItem();
            if (activeItem == null || activeItem.info.shortname != "hammer")
                return;

            if (!input.WasJustPressed(BUTTON.FIRE_PRIMARY))
                return;

            RaycastHit raycastHit;
            if (!Physics.Raycast(player.eyes.HeadRay(), out raycastHit, 10f, LAYER_PLAYER, QueryTriggerInteraction.Ignore))
                return;

            BasePlayer targetPlayer = raycastHit.GetEntity() as BasePlayer;
            if (targetPlayer == null)
                return;

            Vector3 directionToPlayer = (player.transform.position - targetPlayer.transform.position).normalized;

            targetPlayer.OverrideViewAngles(Quaternion.LookRotation(directionToPlayer).eulerAngles);
            targetPlayer.SendNetworkUpdateImmediate();
        }

        #endregion Oxide Hooks

        #region Permissions

        private static class PermissionUtil
        {
            public const string USE = "npcrotator.use";
            private static readonly List<string> _permissions = new List<string>
            {
                USE,
            };

            public static void RegisterPermissions()
            {
                foreach (var permission in _permissions)
                {
                    _plugin.permission.RegisterPermission(permission, _plugin);
                }
            }

            public static bool HasPermission(BasePlayer player, string permissionName)
            {
                return _plugin.permission.UserHasPermission(player.UserIDString, permissionName);
            }
        }

        #endregion Permissions
    }
}
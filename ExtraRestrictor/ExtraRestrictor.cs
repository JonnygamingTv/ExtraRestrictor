using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Linq;
using UnityEngine;
using Rocket.API.Collections;
using Rocket.API;
using System.Collections;
using Logger = Rocket.Core.Logging.Logger;
using System;
using System.Collections.Generic;

namespace ExtraConcentratedJuice.ExtraRestrictor
{
    public class ExtraRestrictor : RocketPlugin<ExtraRestrictorConfiguration>
    {
        public static ExtraRestrictor Instance { get; private set; }
        private Dictionary<ushort, string> RestrictItems;
        private Dictionary<ushort, string> RestrictCraft;
        private Dictionary<ushort, string> RestrictVehicle;

        protected override void Load()
        {
            Instance = this;
            
            RestrictItems = Configuration.Instance.Restricted
                .ToDictionary(x => x.Id, x => x.Bypass);
            RestrictCraft = Configuration.Instance.RestrictedCrafting
                .ToDictionary(x => x.Id, x => x.Bypass);
            RestrictVehicle = Configuration.Instance.RestrictedVehicleEnter
                .ToDictionary(x => x.Id, x => x.Bypass);

            if (RestrictItems.Count > 0)
            {
                UnturnedPlayerEvents.OnPlayerInventoryAdded += OnInventoryUpdated;
                UnturnedPlayerEvents.OnPlayerWear += OnWear;
                ItemManager.onTakeItemRequested += TakeItemRequestHandler;
            }
            if(RestrictCraft.Count > 0)
            {
                PlayerCrafting.onCraftBlueprintRequested += OnCraft;
            }
            if (RestrictVehicle.Count > 0)
            {
                VehicleManager.onEnterVehicleRequested += OnEnterVehicle;
            }

            Logger.Log("ExtraRestrictor Loaded!");
            Logger.Log("Users with the permission extrarestrictor.bypass will bypass restrictions.");
            Logger.Log($"Ignore admins: {Configuration.Instance.IgnoreAdmins}");
            Logger.Log("==============");
            Logger.Log("Restricted vehicles: " + RestrictVehicle.Count.ToString());
            Logger.Log("Restricted crafting: " + RestrictCraft.Count.ToString());
            Logger.Log("Restricted items: " + RestrictItems.Count.ToString());

            foreach (var item in Configuration.Instance.Restricted
                .Select(x => $"ID: {x.Id} | Name: {Assets.find(EAssetType.ITEM, x.Id)?.name ?? "> INVALID ID <"} | Bypass: {(x.Bypass ?? "None")}"))
                Logger.Log(item);

            Logger.Log("==============");
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerInventoryAdded -= OnInventoryUpdated;
            UnturnedPlayerEvents.OnPlayerWear -= OnWear;
            ItemManager.onTakeItemRequested -= TakeItemRequestHandler;
            VehicleManager.onEnterVehicleRequested -= OnEnterVehicle;
            PlayerCrafting.onCraftBlueprintRequested -= OnCraft;

            Logger.Log("ExtraRestrictor unloaded!");
        }

        private void OnInventoryUpdated(UnturnedPlayer player, InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P)
        {
            if ((player.IsAdmin && Configuration.Instance.IgnoreAdmins) || !RestrictItems.TryGetValue(P.item.id, out string bypass) || player.HasPermission("extrarestrictor.bypass"))
                return;

            if(!player.HasPermission(bypass))
            {
                player.Inventory.removeItem((byte)inventoryGroup, inventoryIndex);
                UnturnedChat.Say(player, Util.Translate("item_restricted", Assets.find(EAssetType.ITEM, P.item.id).name, P.item.id), Color.red);
            }
        }

        private void OnWear(UnturnedPlayer player, UnturnedPlayerEvents.Wearables wear, ushort id, byte? quality)
        {
            if ((player.IsAdmin && Configuration.Instance.IgnoreAdmins) || !RestrictItems.TryGetValue(id, out string bypass) || player.HasPermission("extrarestrictor.bypass"))
                return;

            if(!player.HasPermission(bypass))
            {
                // Gotta wait until the next frame for the item to be removed
                switch (wear)
                {
                    #region WearSwitch
                    case UnturnedPlayerEvents.Wearables.Backpack:
                        StartCoroutine(InvokeOnNextFrame(() =>
                        player.Player.clothing.askWearBackpack(0, 0, new byte[0], true)));
                        break;
                    case UnturnedPlayerEvents.Wearables.Glasses:
                        StartCoroutine(InvokeOnNextFrame(() =>
                        player.Player.clothing.askWearGlasses(0, 0, new byte[0], true)));
                        break;
                    case UnturnedPlayerEvents.Wearables.Hat:
                        StartCoroutine(InvokeOnNextFrame(() =>
                        player.Player.clothing.askWearHat(0, 0, new byte[0], true)));
                        break;
                    case UnturnedPlayerEvents.Wearables.Mask:
                        StartCoroutine(InvokeOnNextFrame(() =>
                        player.Player.clothing.askWearMask(0, 0, new byte[0], true)));
                        break;
                    case UnturnedPlayerEvents.Wearables.Pants:
                        StartCoroutine(InvokeOnNextFrame(() =>
                        player.Player.clothing.askWearPants(0, 0, new byte[0], true)));
                        break;
                    case UnturnedPlayerEvents.Wearables.Shirt:
                        StartCoroutine(InvokeOnNextFrame(() =>
                        player.Player.clothing.askWearShirt(0, 0, new byte[0], true)));
                        break;
                    case UnturnedPlayerEvents.Wearables.Vest:
                        StartCoroutine(InvokeOnNextFrame(() =>
                        player.Player.clothing.askWearVest(0, 0, new byte[0], true)));
                        break;
                    #endregion
                }
            }
        }
        private void TakeItemRequestHandler(Player _player, byte _x, byte _y, uint instanceID, byte to_x, byte to_y, byte to_rot, byte to_page, ItemData itemData, ref bool shouldAllow)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(_player);
            if ((player.IsAdmin && Configuration.Instance.IgnoreAdmins) || !RestrictItems.TryGetValue(itemData.item.id, out string bypass) || player.HasPermission("extrarestrictor.bypass"))
                return;

            if (!player.HasPermission(bypass))
            {
                shouldAllow = false;
                UnturnedChat.Say(player, Util.Translate("item_restricted", Assets.find(EAssetType.ITEM, itemData.item.id).name, itemData.item.id), Color.red);
            }
        }
        private void OnCraft(PlayerCrafting crafting, ref ushort itemID, ref byte blueprintIndex, ref bool shouldAllow)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(crafting.player);
            if ((player.IsAdmin && Configuration.Instance.IgnoreAdmins) || !RestrictCraft.TryGetValue(itemID, out string bypass) || player.HasPermission("extrarestrictor.bypass"))
                return;

            if (!player.HasPermission(bypass))
            {
                shouldAllow = false;
                UnturnedChat.Say(player, Util.Translate("item_restricted", Assets.find(EAssetType.ITEM, itemID).name, itemID), Color.red);
            }
        }

        private void OnEnterVehicle(Player pl, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(pl);
            if ((player.IsAdmin && Configuration.Instance.IgnoreAdmins) || !RestrictVehicle.TryGetValue(vehicle.id, out string bypass) || player.HasPermission("extrarestrictor.bypass"))
                return;

            if (!player.HasPermission(bypass))
            {
                shouldAllow = false;
                UnturnedChat.Say(player, Util.Translate("vehicle_restricted", Assets.find(EAssetType.ITEM, vehicle.id).name, vehicle.id), Color.red);
            }
        }

        private IEnumerator InvokeOnNextFrame(System.Action action)
        {
            yield return new WaitForFixedUpdate();
            action();
        }

        public override TranslationList DefaultTranslations =>
            new TranslationList
            {
                { "item_restricted", "You do not have access to this restricted item. ({0}, {1})" },
                { "vehicle_restricted", "You do not have access to this restricted vehicle. ({0}, {1})" }
            };
    }
}

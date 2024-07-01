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

namespace ExtraConcentratedJuice.ExtraRestrictor
{
    public class ExtraRestrictor : RocketPlugin<ExtraRestrictorConfiguration>
    {
        public static ExtraRestrictor Instance { get; private set; }

        protected override void Load()
        {
            Instance = this;

            UnturnedPlayerEvents.OnPlayerInventoryAdded += OnInventoryUpdated;
            UnturnedPlayerEvents.OnPlayerWear += OnWear;
            ItemManager.onTakeItemRequested += TakeItemRequestHandler;

            Logger.Log("ExtraRestrictor Loaded!");
            Logger.Log("Users with the permission extrarestrictor.bypass will bypass restrictions.");
            Logger.Log($"Ignore admins: {Configuration.Instance.IgnoreAdmins}");
            Logger.Log("==============");
            Logger.Log("Restricted items:");

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
        }

        private void OnInventoryUpdated(UnturnedPlayer player, InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P)
        {
            if ((player.IsAdmin && Configuration.Instance.IgnoreAdmins) || player.GetPermissions().Any(x => x.Name == "extrarestrictor.bypass"))
                return;

            RestrictedItem item = Configuration.Instance.Restricted.FirstOrDefault(x => x.Id == P.item.id);

            if (item != null && !player.GetPermissions().Any(x => x.Name == item.Bypass))
            {
                player.Inventory.removeItem((byte)inventoryGroup, inventoryIndex);
                UnturnedChat.Say(player, Util.Translate("item_restricted", Assets.find(EAssetType.ITEM, P.item.id).name, P.item.id), Color.red);
            }
        }

        private void OnWear(UnturnedPlayer player, UnturnedPlayerEvents.Wearables wear, ushort id, byte? quality)
        {
            if ((player.IsAdmin && Configuration.Instance.IgnoreAdmins) || player.GetPermissions().Any(x => x.Name == "extrarestrictor.bypass"))
                return;

            RestrictedItem item = Configuration.Instance.Restricted.FirstOrDefault(x => x.Id == id);

            if (item != null && !player.GetPermissions().Any(x => x.Name == item.Bypass))
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
            if ((player.IsAdmin && Configuration.Instance.IgnoreAdmins) || player.GetPermissions().Any(x => x.Name == "extrarestrictor.bypass"))
                return;

            RestrictedItem item = Configuration.Instance.Restricted.FirstOrDefault(x => x.Id == P.item.id);

            if (item != null && !player.GetPermissions().Any(x => x.Name == item.Bypass))
            {
                shouldAllow = false;
                UnturnedChat.Say(player, Util.Translate("item_restricted", Assets.find(EAssetType.ITEM, P.item.id).name, P.item.id), Color.red);
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
                { "item_restricted", "You do not have access to this restricted item. ({0}, {1})" }
            };
    }
}

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

namespace ExtraConcentratedJuice.ExtraRestrictor
{
    public class ExtraRestrictor : RocketPlugin<ExtraRestrictorConfiguration>
    {
        public static ExtraRestrictor instance;

        protected override void Load()
        {
            instance = this;

            UnturnedPlayerEvents.OnPlayerInventoryAdded += OnInventoryUpdated;
            UnturnedPlayerEvents.OnPlayerWear += OnWear;
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerInventoryAdded -= OnInventoryUpdated;
            UnturnedPlayerEvents.OnPlayerWear -= OnWear;
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
                // Doesn't remove an item without delayed invocation oof
                switch (wear)
                {
                    #region WearSwitch
                    case UnturnedPlayerEvents.Wearables.Backpack:
                        StartCoroutine(DelayedInvoke(0.1F, () =>
                        player.Player.clothing.askWearBackpack(0, 0, new byte[0], true)));
                        break;
                    case UnturnedPlayerEvents.Wearables.Glasses:
                        StartCoroutine(DelayedInvoke(0.1F, () =>
                        player.Player.clothing.askWearGlasses(0, 0, new byte[0], true)));
                        break;
                    case UnturnedPlayerEvents.Wearables.Hat:
                        StartCoroutine(DelayedInvoke(0.1F, () =>
                        player.Player.clothing.askWearHat(0, 0, new byte[0], true)));
                        break;
                    case UnturnedPlayerEvents.Wearables.Mask:
                        StartCoroutine(DelayedInvoke(0.1F, () =>
                        player.Player.clothing.askWearMask(0, 0, new byte[0], true)));
                        break;
                    case UnturnedPlayerEvents.Wearables.Pants:
                        StartCoroutine(DelayedInvoke(0.1F, () =>
                        player.Player.clothing.askWearPants(0, 0, new byte[0], true)));
                        break;
                    case UnturnedPlayerEvents.Wearables.Shirt:
                        StartCoroutine(DelayedInvoke(0.1F, () =>
                        player.Player.clothing.askWearShirt(0, 0, new byte[0], true)));
                        break;
                    case UnturnedPlayerEvents.Wearables.Vest:
                        StartCoroutine(DelayedInvoke(0.1F, () =>
                        player.Player.clothing.askWearVest(0, 0, new byte[0], true)));
                        break;
                        #endregion
                }
            }
        }

        private IEnumerator DelayedInvoke(float time, System.Action action)
        {
            yield return new WaitForSeconds(time);
            action();
        }

        public override TranslationList DefaultTranslations =>
            new TranslationList
            {
                { "item_restricted", "You do not have access to this restricted item. ({0}, {1})" }
            };
    }
}

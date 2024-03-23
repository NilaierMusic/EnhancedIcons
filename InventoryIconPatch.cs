using HarmonyLib;
using System.Collections.Concurrent;
using UnityEngine;

namespace EnhancedIcons
{
    public static class InventoryIconPatch
    {
        private static readonly ConcurrentDictionary<string, Sprite> iconCache = new ConcurrentDictionary<string, Sprite>();

        [HarmonyPatch(typeof(HUDManager), "Update")]
        [HarmonyPostfix]
        public static void Update_Postfix(HUDManager __instance)
        {
            var localPlayerController = GameNetworkManager.Instance?.localPlayerController;
            if (localPlayerController == null) return;

            var itemName = localPlayerController.currentlyHeldObjectServer?.itemProperties?.itemName;
            if (itemName == null) return;

            var imagePath = $"{itemName}.png";

            if (iconCache.TryGetValue(imagePath, out var cachedIcon))
            {
                __instance.itemSlotIcons[localPlayerController.currentItemSlot].sprite = cachedIcon;
                return;
            }

            var customIcon = IconLoader.LoadCustomIcon(imagePath);
            if (customIcon != null)
            {
                iconCache[imagePath] = customIcon;
                __instance.itemSlotIcons[localPlayerController.currentItemSlot].sprite = customIcon;
            }
        }
    }
}
using HarmonyLib;
using System.Collections.Concurrent;
using UnityEngine;
using System.Collections.Generic;

namespace EnhancedIcons
{
    public static class InventoryIconPatch
    {
        private static readonly LRUCache<string, Sprite> iconCache = new LRUCache<string, Sprite>(100);

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
                iconCache.Add(imagePath, customIcon);
                __instance.itemSlotIcons[localPlayerController.currentItemSlot].sprite = customIcon;
            }
            else
            {
                // Fallback to the original icon
                __instance.itemSlotIcons[localPlayerController.currentItemSlot].sprite = __instance.itemSlotIcons[localPlayerController.currentItemSlot].sprite;
            }

            // Check if the currently held object is in a belt bag
            var beltBagItem = localPlayerController.currentlyHeldObjectServer?.GetComponent<BeltBagItem>();
            if (beltBagItem != null)
            {
                foreach (var item in beltBagItem.Items)
                {
                    var beltBagItemName = item?.itemProperties?.itemName;
                    if (beltBagItemName == null) continue;

                    var beltBagImagePath = $"{beltBagItemName}.png";

                    if (iconCache.TryGetValue(beltBagImagePath, out var beltBagCachedIcon))
                    {
                        __instance.itemSlotIcons[localPlayerController.currentItemSlot].sprite = beltBagCachedIcon;
                        continue;
                    }

                    var beltBagCustomIcon = IconLoader.LoadCustomIcon(beltBagImagePath);
                    if (beltBagCustomIcon != null)
                    {
                        iconCache.Add(beltBagImagePath, beltBagCustomIcon);
                        __instance.itemSlotIcons[localPlayerController.currentItemSlot].sprite = beltBagCustomIcon;
                    }
                    else
                    {
                        // Fallback to the original icon
                        __instance.itemSlotIcons[localPlayerController.currentItemSlot].sprite = __instance.itemSlotIcons[localPlayerController.currentItemSlot].sprite;
                    }
                }
            }
        }

        public static void ClearCache()
        {
            iconCache.Clear();
        }
    }

    public class LRUCache<TKey, TValue>
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _cacheMap;
        private readonly LinkedList<CacheItem> _lruList;

        public LRUCache(int capacity)
        {
            _capacity = capacity;
            _cacheMap = new Dictionary<TKey, LinkedListNode<CacheItem>>(capacity);
            _lruList = new LinkedList<CacheItem>();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_cacheMap.TryGetValue(key, out var node))
            {
                value = node.Value.Value;
                _lruList.Remove(node);
                _lruList.AddLast(node);
                return true;
            }

            value = default;
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            if (_cacheMap.TryGetValue(key, out var node))
            {
                _lruList.Remove(node);
                _cacheMap.Remove(key);
            }
            else if (_cacheMap.Count >= _capacity)
            {
                var lruNode = _lruList.First;
                _lruList.RemoveFirst();
                _cacheMap.Remove(lruNode.Value.Key);
            }

            var cacheItem = new CacheItem(key, value);
            var newNode = new LinkedListNode<CacheItem>(cacheItem);
            _lruList.AddLast(newNode);
            _cacheMap[key] = newNode;
        }

        public void Clear()
        {
            _cacheMap.Clear();
            _lruList.Clear();
        }

        private class CacheItem
        {
            public TKey Key { get; }
            public TValue Value { get; }

            public CacheItem(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}

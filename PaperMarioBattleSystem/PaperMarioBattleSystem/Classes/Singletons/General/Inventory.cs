﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.BadgeGlobals;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Player's inventory.
    /// </summary>
    public class Inventory : ICleanup
    {
        #region Singleton Fields

        public static Inventory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Inventory();
                }

                return instance;
            }
        }

        public static bool HasInstance => (instance != null);

        private static Inventory instance = null;

        #endregion

        #region Partner Inventory

        /// <summary>
        /// The Partner inventory holding all Partners in Mario's party.
        /// </summary>
        public readonly PartnerInventory partnerInventory = new PartnerInventory();

        #endregion

        #region Currency Fields

        /// <summary>
        /// The max number of Coins that can be held at a time.
        /// </summary>
        public const uint MaxCoins = 999u;

        /// <summary>
        /// The max number of Star Pieces that can be held at a time.
        /// </summary>
        public const uint MaxStarPieces = 99u;

        /// <summary>
        /// The number of Coins the Player has.
        /// </summary>
        public uint Coins { get; private set; } = 0u;

        /// <summary>
        /// The number of Star Pieces the Player has.
        /// </summary>
        public uint StarPieces { get; private set; } = 0u;

        #endregion

        #region Badge Related Fields

        /// <summary>
        /// The number of Badge Points (BP) Mario currently has available.
        /// </summary>
        public int BP { get; private set; } = 3;

        /// <summary>
        /// The max number of Badge Points (BP) Mario has.
        /// </summary>
        public int MaxBP { get; private set; } = 3;

        /// <summary>
        /// All Badges the Player owns.
        /// </summary>
        private readonly List<Badge> AllBadges = new List<Badge>();

        /// <summary>
        /// Counts for all Badges the Player owns.
        /// </summary>
        private readonly Dictionary<BadgeTypes, int> AllBadgeCounts = new Dictionary<BadgeTypes, int>();

        /// <summary>
        /// The Badges the Player owns that are active.
        /// </summary>
        private readonly List<Badge> ActiveBadges = new List<Badge>();
        
        /// <summary>
        /// Counts for the active Badges the Player owns.
        /// </summary>
        private readonly Dictionary<BadgeTypes, int> ActiveBadgeCounts = new Dictionary<BadgeTypes, int>();

        #endregion

        #region Item Related Fields

        /// <summary>
        /// The Player's Item inventory.
        /// </summary>
        private readonly List<Item> Items = new List<Item>(10);

        /// <summary>
        /// The key items the Player possesses.
        /// </summary>
        private readonly List<Item> KeyItems = new List<Item>(20);

        #endregion

        private Inventory()
        {
            
        }

        public void CleanUp()
        {
            partnerInventory.CleanUp();

            ActiveBadges.Clear();
            ActiveBadgeCounts.Clear();
            AllBadges.Clear();
            AllBadgeCounts.Clear();

            Items.Clear();
            KeyItems.Clear();

            instance = null;
        }

        #region Currency Methods

        /// <summary>
        /// Gives the player a number of coins.
        /// </summary>
        /// <param name="coinsAdded">The number of coins to give.</param>
        public void AddCoins(uint coinsAdded)
        {
            Coins = UtilityGlobals.Clamp(Coins + coinsAdded, 0u, MaxCoins);
        }

        /// <summary>
        /// Subtracts a number of coins from the player.
        /// </summary>
        /// <param name="coinsSubtracted">The number of coins to subtract.</param>
        public void SubtractCoins(uint coinsSubtracted)
        {
            Coins = UtilityGlobals.Clamp(Coins - coinsSubtracted, 0u, MaxCoins);
        }

        /// <summary>
        /// Gives the player a number of Star Pieces.
        /// </summary>
        /// <param name="starPiecesAdded">The number of Star Pieces to give.</param>
        public void AddStarPieces(uint starPiecesAdded)
        {
            StarPieces = UtilityGlobals.Clamp(StarPieces + starPiecesAdded, 0u, MaxStarPieces);
        }

        /// <summary>
        /// Subtracts a number of Star Pieces from the player.
        /// </summary>
        /// <param name="starPiecesSubtracted">The number of Star Pieces to subtract.</param>
        public void SubtractStarPieces(uint starPiecesSubtracted)
        {
            StarPieces = UtilityGlobals.Clamp(StarPieces - starPiecesSubtracted, 0u, MaxStarPieces);
        }

        #endregion

        #region Item Methods

        /// <summary>
        /// Adds an Item to the Player's Inventory.
        /// </summary>
        /// <param name="item">The Item to add.</param>
        public void AddItem(Item item)
        {
            if (item.ItemCategory == Item.ItemCategories.KeyItem)
            {
                if (KeyItems.Count < KeyItems.Capacity)
                {
                    KeyItems.Add(item);
                }
                else
                {
                    Debug.LogWarning($"Unable to add Key Item {item.Name} as there are too many Key Items in the Inventory. This should not happen.");
                }
            }
            else
            {
                if (Items.Count < Items.Capacity)
                {
                    Items.Add(item);
                }
                else
                {
                    Debug.LogWarning($"Unable to add Item {item.Name} as the Inventory is full.");
                }
            }
        }

        /// <summary>
        /// Removes an Item from the Player's Inventory.
        /// </summary>
        /// <param name="item">The Item to remove.</param>
        public void RemoveItem(Item item)
        {
            if (item.ItemCategory == Item.ItemCategories.KeyItem)
            {
                KeyItems.Remove(item);
            }
            else
            {
                Items.Remove(item);
            }
        }

        /// <summary>
        /// Finds the first instance of an Item by name.
        /// </summary>
        /// <param name="name">The name of the Item to find.</param>
        /// <param name="keyItem">Whether the Item is a key item or not.</param>
        /// <returns>The Item if found, otherwise null.</returns>
        public Item FindItem(string name, bool keyItem)
        {
            if (keyItem == true)
            {
                return KeyItems.Find((keyitem) => keyitem.Name == name);
            }
            else
            {
                return Items.Find((item) => item.Name == name);
            }
        }

        /// <summary>
        /// Finds and returns the first Item in the Inventory with any of the designated ItemTypes.
        /// </summary>
        /// <param name="itemTypes">The ItemTypes enum values to look for.</param>
        /// <returns>An item with any of the designated ItemTypes. If not found, null.</returns>
        public Item FindItem(Item.ItemCategories itemCategory, Item.ItemTypes itemTypes)
        {
            return Items.Find((item) => UtilityGlobals.ItemTypesHasFlag(item.ItemType, itemTypes) == true);
        }

        /// <summary>
        /// Finds and returns all instances of Items in the Inventory with any of the designated ItemTypes.
        /// </summary>
        /// <param name="itemTypes">The ItemTypes enum values to look for.</param>
        /// <returns>An array of all items with any of the designated ItemTypes. If none are found, an empty array.</returns>
        public Item[] FindItems(Item.ItemCategories itemCategory, Item.ItemTypes itemTypes)
        {
            List<Item> itemsToFind = new List<Item>();

            //If searching for Key Items, add all Key Items to the list
            if (itemCategory == Item.ItemCategories.KeyItem)
            {
                itemsToFind.AddRange(KeyItems);
            }
            else
            {
                //Look in the Item list for all Items with these ItemTypes and add them
                itemsToFind.AddRange(Items.FindAll((item) => UtilityGlobals.ItemTypesHasFlag(item.ItemType, itemTypes) == true));
            }

            return itemsToFind.ToArray();
        }

        #endregion

        #region Badge Methods

        /// <summary>
        /// Tells if the Badge can be equipped or not based on how much BP it costs.
        /// </summary>
        public bool CanEquip(Badge badge)
        {
            return (badge != null && BP >= badge.BPCost);
        }

        /// <summary>
        /// Adds a Badge to the Player's Inventory.
        /// </summary>
        /// <param name="badge">The Badge to add.</param>
        public void AddBadge(Badge badge)
        {
            if (badge == null)
            {
                Debug.LogError($"Attempting to add a null Badge to the Inventory!");
                return;
            }

            //Add to the badge list
            AllBadges.Add(badge);

            //Increment number if a Badge of this type already exists
            if (HasBadgeType(badge.BadgeType) == true)
            {
                AllBadgeCounts[badge.BadgeType]++;
            }
            //Otherwise add a new entry with a count of 1
            else
            {
                AllBadgeCounts.Add(badge.BadgeType, 1);
            }

            Debug.Log($"Added {badge.Name} to the Inventory!");
        }

        /// <summary>
        /// Removes a Badge from the Player's Inventory.
        /// If the Badge is active, it also unequips the Badge and removes it from the active Badge list.
        /// </summary>
        /// <param name="badge">The Badge to remove.</param>
        public void RemoveBadge(Badge badge)
        {
            if (badge == null)
            {
                Debug.LogError($"Attempting to remove a null Badge from the Inventory!");
                return;
            }

            int count = 0;
            if (AllBadgeCounts.TryGetValue(badge.BadgeType, out count) == false)
            {
                Debug.LogWarning($"Badge of type {badge.BadgeType} cannot be removed because it doesn't exist in the Inventory!");
                return;
            }

            //Remove from the all badges list
            AllBadges.Remove(badge);
            
            count--;
            
            if (count <= 0)
            {
                AllBadgeCounts.Remove(badge.BadgeType);
            }
            else
            {
                AllBadgeCounts[badge.BadgeType] = count;
            }

            //Remove from active badges if it's active, and unequip it
            if (badge.Equipped == true)
            {
                //Unequip badge to remove its effects and deactivate it
                badge.UnEquip();
            }

            Debug.Log($"Removed {badge.Name} from the Inventory!");
        }

        /// <summary>
        /// Finds the first instance of a Badge with a particular BadgeType.
        /// </summary>
        /// <param name="badgeType">The BadgeType of the Badge.</param>
        /// <param name="badgeFilter">The filter for finding the Badge.</param>
        /// <returns>null if no Badge was found, otherwise the first Badge matching the parameters.</returns>
        public Badge GetBadge(BadgeTypes badgeType, BadgeFilterType badgeFilter)
        {
            if (HasBadgeType(badgeType) == true)
            {
                //Look through all Badges
                if (badgeFilter == BadgeFilterType.All)
                {
                    return AllBadges.Find((badge) => badge.BadgeType == badgeType);
                }
                //Look through all equipped Badges
                else if (badgeFilter == BadgeFilterType.Equipped)
                {
                    return GetActiveBadge(badgeType);
                }
                //Look through all unequipped Badges
                else if (badgeFilter == BadgeFilterType.UnEquipped)
                {
                    return AllBadges.Find((badge) => (badge.BadgeType == badgeType && badge.Equipped == false));
                }
            }

            return null;
        }

        /// <summary>
        /// Finds all instances of a Badge with a particular BadgeType.
        /// </summary>
        /// <param name="badgeType">The BadgeType of the Badges.</param>
        /// <param name="badgeFilter">The filter for finding the Badges.</param>
        /// <returns>A list of all Badges matching the parameters, and an empty list if none were found.</returns>
        public List<Badge> GetBadges(BadgeTypes badgeType, BadgeFilterType badgeFilter)
        {
            if (HasBadgeType(badgeType) == true)
            {
                if (badgeFilter == BadgeFilterType.All)
                {
                    return AllBadges.FindAll((badge) => badge.BadgeType == badgeType);
                }
                else if (badgeFilter == BadgeFilterType.Equipped)
                {
                    return GetActiveBadges(badgeType);
                }
                else if (badgeFilter == BadgeFilterType.UnEquipped)
                {
                    return AllBadges.FindAll((badge) => (badge.BadgeType == badgeType && badge.Equipped == false));
                }
            }

            return new List<Badge>();
        }

        /// <summary>
        /// Gets the number of Badges of a particular BadgeType in the Player's Inventory.
        /// </summary>
        /// <param name="badgeType">The BadgeType to find.</param>
        /// <returns>The number of Badges of the BadgeType in the Player's Inventory.</returns>
        public int GetBadgeCount(BadgeTypes badgeType)
        {
            int count = 0;
            AllBadgeCounts.TryGetValue(badgeType, out count);

            return count;
        }

        /// <summary>
        /// Tells whether the Player owns a Badge of a particular BadgeType or not.
        /// </summary>
        /// <param name="badgeType">The BadgeType.</param>
        /// <returns>true if the Player owns a Badge of the BadgeType, false if not.</returns>
        public bool HasBadgeType(BadgeTypes badgeType)
        {
            return AllBadgeCounts.ContainsKey(badgeType);
        }

        /*Active Badges*/

        /// <summary>
        /// Adds a Badge to the active Badge list.
        /// Do not add new Badges instances here. Add only Badges that are already in the Inventory.
        /// </summary>
        /// <param name="badge">The Badge to activate.</param>
        public void ActivateBadge(Badge badge)
        {
            if (badge == null)
            {
                Debug.LogError($"Attempting to activate a null Badge!");
                return;
            }

            //The Badge is already equipped, so it shouldn't be added again
            if (badge.Equipped == true)
            {
                Debug.LogError($"This instance of {badge.Name} is already equipped and thus cannot be activated.");
                return;
            }

            //Add to the active Badge list
            ActiveBadges.Add(badge);

            //Increment number if a Badge of this type is already active
            if (IsBadgeTypeActive(badge.BadgeType) == true)
            {
                ActiveBadgeCounts[badge.BadgeType]++;
            }
            //Otherwise add a new entry with a count of 1
            else
            {
                ActiveBadgeCounts.Add(badge.BadgeType, 1);
            }

            Debug.Log($"Activated a(n) {badge.Name} Badge!");
        }

        /// <summary>
        /// Removes a Badge from the active Badge list.
        /// </summary>
        /// <param name="badge">The Badge to deactivate.</param>
        public void DeactivateBadge(Badge badge)
        {
            if (badge == null)
            {
                Debug.LogError($"Attempting to deactivate a null Badge!");
                return;
            }

            //The Badge isn't equipped, so it shouldn't be removed
            if (badge.Equipped == false)
            {
                Debug.LogError($"This instance of {badge.Name} isn't equipped and thus cannot be deactivated.");
                return;
            }

            //Remove from the active badges list
            ActiveBadges.Remove(badge);
            ActiveBadgeCounts[badge.BadgeType]--;
            if (ActiveBadgeCounts[badge.BadgeType] <= 0)
            {
                ActiveBadgeCounts.Remove(badge.BadgeType);
            }

            Debug.Log($"Deactivated a(n) {badge.Name} Badge!");
        }

        /// <summary>
        /// Finds the first instance of an active Badge with a particular BadgeType.
        /// </summary>
        /// <param name="badgeType">The BadgeType of the Badge.</param>
        /// <returns>null if no Badge was found, otherwise the first active Badge matching the BadgeType.</returns>
        private Badge GetActiveBadge(BadgeTypes badgeType)
        {
            if (IsBadgeTypeActive(badgeType) == false) return null;

            return ActiveBadges.Find((badge) => badge.BadgeType == badgeType);
        }

        /// <summary>
        /// Finds all instances of active Badges with a particular BadgeType.
        /// </summary>
        /// <param name="badgeType">The BadgeType of the Badges.</param>
        /// <returns>A list of all active Badges of the BadgeType, and an empty list if none were found.</returns>
        private List<Badge> GetActiveBadges(BadgeTypes badgeType)
        {
            if (IsBadgeTypeActive(badgeType) == false) return new List<Badge>();

            return ActiveBadges.FindAll((badge) => badge.BadgeType == badgeType);
        }

        /// <summary>
        /// Gets the number of active Badges of a particular BadgeType the Player has equipped.
        /// </summary>
        /// <param name="badgeType">The BadgeType to find.</param>
        /// <returns>The number of active Badges of the BadgeType.</returns>
        public int GetActiveBadgeCount(BadgeTypes badgeType)
        {
            int count = 0;
            ActiveBadgeCounts.TryGetValue(badgeType, out count);

            return count;
        }

        /// <summary>
        /// Tells whether the Player has an active Badge of a particular type.
        /// </summary>
        /// <param name="badgeType">The BadgeType.</param>
        /// <returns>true if the Player owns the Badge and the Badge is active, false otherwise.</returns>
        public bool IsBadgeTypeActive(BadgeTypes badgeType)
        {
            return ActiveBadgeCounts.ContainsKey(badgeType);
        }

        /// <summary>
        /// Gets all active Badges affecting a particular BattleEntity.
        /// </summary>
        /// <param name="battleEntity">The BattleEntity to find the active Badges on.</param>
        /// <returns>A new List of all active Badges affecting the BattleEntity.</returns>
        public List<Badge> GetActiveBadgesOnEntity(BattleEntity battleEntity)
        {
            return ActiveBadges.FindAll((badge) => badge.EntityEquipped == battleEntity);
        }

        #endregion


        #region Partner Inventory Class

        /// <summary>
        /// The inventory for Mario's Partners.
        /// This tells which Partners Mario currently has available.
        /// </summary>
        public class PartnerInventory : ICleanup
        {
            private readonly Dictionary<PartnerTypes, BattlePartner> Partners = new Dictionary<PartnerTypes, BattlePartner>();

            public PartnerInventory()
            {
                
            }

            public void CleanUp()
            {
                Partners.Clear();
            }

            /// <summary>
            /// Adds a Partner to the party.
            /// </summary>
            /// <param name="battlePartner">The BattlePartner to add to the party.</param>
            public void AddPartner(BattlePartner battlePartner)
            {
                if (battlePartner == null)
                {
                    Debug.LogError($"{nameof(battlePartner)} is null! Partner will not be added as a result");
                    return;
                }

                if (HasPartner(battlePartner.PartnerType) == true)
                {
                    Debug.LogError($"Mario already has {battlePartner.PartnerType} as a Partner");
                    return;
                }

                Partners.Add(battlePartner.PartnerType, battlePartner);
            }

            /// <summary>
            /// Removes a Partner from the party.
            /// </summary>
            /// <param name="partnerType">The PartnerType of the Partner.</param>
            public void RemovePartner(PartnerTypes partnerType)
            {
                BattlePartner battlePartner = GetPartner(partnerType);

                if (battlePartner == null)
                {
                    Debug.LogError($"Mario doesn't have {partnerType} as a Partner, so he/she cannot be removed from the party");
                    return;
                }

                Partners.Remove(partnerType);
            }

            /// <summary>
            /// Gets a particular Partner in the party.
            /// </summary>
            /// <param name="partnerType">The PartnerType of the Partner.</param>
            /// <returns>A BattlePartner with the specified PartnerType if it exists in the party, otherwise null.</returns>
            public BattlePartner GetPartner(PartnerTypes partnerType)
            {
                BattlePartner partner = null;
                Partners.TryGetValue(partnerType, out partner);

                return partner;
            }

            /// <summary>
            /// Tells whether a particular Partner is in the party or not.
            /// </summary>
            /// <param name="partner">The PartnerTypes of the Partner.</param>
            /// <returns>true if a BattlePartner with the specified PartnerType is in the party, otherwise false.</returns>
            public bool HasPartner(PartnerTypes partner)
            {
                return Partners.ContainsKey(partner);
            }

            /// <summary>
            /// Adds all Partners in the party to the supplied list.
            /// </summary>
            /// <param name="partnerList">The list of Partners to fill.</param>
            public void GetAllPartners(List<BattlePartner> partnerList)
            {
                //Add them in the order they are defined in the PartnerTypes enum
                PartnerTypes[] partnerTypes = EnumUtility.GetValues<PartnerTypes>.EnumValues;
                for (int i = 0; i < partnerTypes.Length; i++)
                {
                    BattlePartner partner = GetPartner(partnerTypes[i]);

                    if (partner != null)
                    {
                        partnerList.Add(partner);
                    }
                }
            }

            /// <summary>
            /// Returns all Partners in the party.
            /// If none are in the party, an empty array is returned.
            /// </summary>
            /// <returns>An array of BattlePartners in the party. An empty array if no BattlePartners exist in the party.</returns>
            public BattlePartner[] GetAllPartners()
            {
                //Return an empty array if there are no Partners
                if (Partners.Count == 0) return Array.Empty<BattlePartner>();

                List<BattlePartner> battlePartners = new List<BattlePartner>();
                GetAllPartners(battlePartners);
                
                return battlePartners.ToArray();
            }

            /// <summary>
            /// Returns how many Partners are in the party.
            /// </summary>
            /// <returns>An int representing the number of Partners in the party.</returns>
            public int GetPartnerCount()
            {
                return Partners.Count;
            }
        }

        #endregion
    }
}

using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Menus
{
    internal class Weapons : Drawing
    {
        static bool hasAllWeapons = false;
        static bool infiniteAmmo = false;
        public static async Task Draw()
        {
            SetMenuTitle("Mikex", "give urself weapons!");

            void GiveWeapon(int ped, string weapName, int ammoCount, bool equip, int group, bool leftHanded, float condition) => Function.Call((Hash)0x5E3BDDBCB83F3D84, ped, GenHash(weapName), ammoCount, equip, true /* unk, no weap if 0 */, group, false /* ? */, 1056964608, 1065353216, leftHanded, condition);

            var r = AddBool("Mikex gives all weps", ref hasAllWeapons);
            var i = AddBool("Infinite Ammo", ref infiniteAmmo);

            var pedId = Function.Call<int>(Hash.PLAYER_PED_ID);

            if (IsEntryPressed(r))
            {
                Debug.WriteLine($"GENERATING A WEAPON FOR MIKEX THE BEST");
                foreach (string w in weapons)
                {
                    GiveWeapon(pedId, w, 69, true, 1, false, 0.0f);
                }
            }

            if (IsEntryPressed(i))
            {
                Debug.WriteLine("Infinite ammo:O yeehaw");
                foreach (string w in weapons)
                {
                    Function.Call(Hash.SET_PED_INFINITE_AMMO, pedId, true, GenHash(w));
                }
            }

            StyleMenu();

            await Task.FromResult(0);
        }

        static string[] weapons = new string[]
        {
            "weapon_revolver_schofield",
            "WEAPON_BOW",
            "WEAPON_THROWN_THROWING_KNIVES",
            "WEAPON_MELEE_TORCH",
            "WEAPON_FISHINGROD",
            "WEAPON_SHOTGUN_PUMP",
            "WEAPON_THROWN_DYNAMITE"
        };
    }
}

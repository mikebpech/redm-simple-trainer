using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Menus
{
    internal class World : Drawing
    {
        static bool isSpeeding = false;
        static bool startGamer = false;

        public static async Task Draw()
        {
            SetMenuTitle("World", "u r god");

            int getveh = AddMenuEntry("Get current vehicle");

            var pedId = Function.Call<int>(Hash.PLAYER_PED_ID);

            var s = AddBool("SUPER Speed", ref isSpeeding);

            var f = AddBool("Start gamertag?", ref startGamer);

            if (IsEntryPressed(f))
            {
                int headDisplayId = Function.Call<int>(Hash.CREATE_FAKE_MP_GAMER_TAG, pedId, "mikex", false, false, "", false);

                Debug.WriteLine($"My head id is {headDisplayId}");

                Debug.WriteLine("calling 4 funcs");
                Function.Call((Hash)0x25B9C78A25105C35, headDisplayId, 12, true);
                Function.Call((Hash)0x5F57522BC1EB9D9D, headDisplayId, 12, true);
                Function.Call((Hash)0x95384C6CE1526EFF, headDisplayId, 12, true);
            }

            if (IsEntryPressed(s))
            {
                var veh = Function.Call<int>(Hash.GET_VEHICLE_PED_IS_IN, pedId, true);

                if (veh > 0)
                {
                    Function.Call(Hash.SET_VEHICLE_FORWARD_SPEED, veh, 150.0f);
                }
                else Debug.WriteLine("No vehicle idiot...");
            }

            if (IsEntryPressed(getveh))
            {
                var veh = Function.Call<int>(Hash.GET_VEHICLE_PED_IS_IN, pedId, true);
                Debug.WriteLine(veh.ToString());
                if (veh > 0)
                {
                    Debug.WriteLine($"current vehicle is.... {veh}");
                    Function.Call((Hash)0x4759cc730f947c81);
                }
                else
                {
                    Debug.WriteLine("No veh??");
                }
            }

            StyleMenu();

            await Task.FromResult(0);
        }
    }
}

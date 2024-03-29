﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace client
{
    class Keyboard : Globals
    {
        public static bool IsControlPressedWrap(int group, Control ctrl) => Function.Call<bool>(Hash.IS_CONTROL_PRESSED, group, (uint)ctrl);
        public static bool IsDisabledControlPressedWrap(int group, Control ctrl) => Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 0, (uint)ctrl);
        public static bool IsControlJustPressedWrap(int group, Control ctrl) => Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, (uint)ctrl);
        public static bool IsDisabledControlJustPressedWrap(int group, Control ctrl) => Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, (uint)ctrl);
        public static void DisableControlActionWrap(int group, Control control, bool val) => Function.Call(Hash.DISABLE_CONTROL_ACTION, group, (uint)control, val);

        private static bool trainerSwitchPressed()
        {
            bool isPressed = false;

            if (IsDisabledControlPressedWrap(0, Control.Map))
            {
                if (!isPressed)
                {
                    isPressed = true;

                    return true;
                }
            }
            else
            {
                isPressed = false;
            }

            return false;
        }

        private static Dictionary<Control, int> ms_controlStates = new Dictionary<Control, int>();

        private static int GetTickCount() => Function.Call<int>(Hash.GET_GAME_TIMER);

        internal static bool isControlPressedFor(Control ctrl, int msec)
        {
            bool ctrlPressed(Control action) =>
                IsControlPressedWrap(0, action) || IsDisabledControlPressedWrap(0, action);
            
            // make a new entry if none exists
            if (!ms_controlStates.ContainsKey(ctrl))
            {
                ms_controlStates[ctrl] = -1;
            }

            if (!ctrlPressed(ctrl))
            {
                ms_controlStates[ctrl] = -1;

                return false;
            }

            if (ms_controlStates[ctrl] == -1 && ctrlPressed(ctrl))
            {
                ms_controlStates[ctrl] = GetTickCount();
            }

            return (GetTickCount() - ms_controlStates[ctrl]) > msec;
        }

        private static int delay = 150;

        private static readonly Dictionary<int, int> controlDelays = new Dictionary<int, int>()
        {
            { 0, 150 },
            { 600, 100 },
            { 1200, 75 },
            { 1800, 45 },
            { 2400, 30 }
        };

        private static void NavKeyHeld()
        {
            delay = 150;

            foreach (var control in new[] { Control.FrontendUp, Control.FrontendDown, Control.FrontendLeft, Control.FrontendRight })
            {
                foreach (var controlDelay in controlDelays.Reverse())
                {
                    if (isControlPressedFor(control, controlDelay.Key))
                    {
                        delay = controlDelay.Value;
                        return;
                    }
                }
            }
        }

        private static void getButtonState(out bool a, out bool b, out bool up, out bool down, out bool l, out bool r)
        {
            a = b = up = down = l = r = false;

            a = IsDisabledControlPressedWrap(0, Control.FrontendAccept);
            b = IsDisabledControlPressedWrap(0, Control.FrontendCancel);
            up = IsDisabledControlPressedWrap(0, Control.FrontendUp);
            down = IsDisabledControlPressedWrap(0, Control.FrontendDown);
            r = IsDisabledControlPressedWrap(0, Control.FrontendRight);
            l = IsDisabledControlPressedWrap(0, Control.FrontendLeft);
        }

        internal static void MonitorKeys()
        {
            NavKeyHeld();

            if (Function.Call<bool>(Hash.IS_PAUSE_MENU_ACTIVE))
                return;

            if ((GetTickCount() - g_menu_delayCounter > delay))
            {
                if (g_menu_subMenu == MenuId.MENU_NOTOPEN)
                {
                    if (trainerSwitchPressed() || IsDisabledControlPressedWrap(0, Control.ScriptLb) && IsDisabledControlPressedWrap(0, Control.ScriptPadDown) && g_menu_subMenu == 0)
                    {
                        g_menu_delayCounter = GetTickCount();
                        g_menu_subMenu = MenuId.MENU_MAIN;
                        g_menu_subMenuLevel = 0;
                        g_menu_currentOption = 1;
                    }

                    g_menu_newTimerTick = true;
                }
                else
                {
                    bool a, b, up, down, l, r;
                    getButtonState(out a, out b, out up, out down, out l, out r);

                    if (IsDisabledControlPressedWrap(0, Control.FrontendCancel) && (g_menu_subMenu != 0))
                    {
                        g_menu_delayCounter = GetTickCount();

                        if (g_menu_subMenu == MenuId.MENU_MAIN)
                        {
                            g_menu_subMenu = MenuId.MENU_NOTOPEN;
                        }
                        else
                        {
                            g_menu_subMenu = g_menu_lastSubMenu[g_menu_subMenuLevel - 1];
                            g_menu_currentOption = g_menu_lastOption[g_menu_subMenuLevel - 1];
                            g_menu_subMenuLevel--;
                        }
                    }
                    else if (a || IsDisabledControlPressedWrap(0, Control.FrontendAccept) && (g_menu_subMenu != 0))
                    {
                        g_menu_delayCounter = GetTickCount();
                        g_menu_optionPress = true;
                    }
                    else if (up || IsDisabledControlPressedWrap(0, Control.FrontendUp) && (g_menu_subMenu != 0))
                    {
                        g_menu_delayCounter = GetTickCount();

                        g_menu_currentOption--;
                        g_menu_currentOption = g_menu_currentOption < 1 ? g_menu_optionCount : g_menu_currentOption;
                    }
                    else if (down || IsDisabledControlPressedWrap(0, Control.FrontendDown) && (g_menu_subMenu != 0))
                    {
                        g_menu_delayCounter = GetTickCount();
                        g_menu_currentOption++;
                        g_menu_currentOption = g_menu_currentOption > g_menu_optionCount ? 1 : g_menu_currentOption;
                    }
                    else if (l || IsDisabledControlPressedWrap(0, Control.CellphoneLeft))
                    {
                        g_menu_delayCounter = GetTickCount();

                        left_press = true;
                    }
                    else if (r || IsDisabledControlPressedWrap(0, Control.CellphoneRight))
                    {
                        g_menu_delayCounter = GetTickCount();

                        right_press = true;
                    }
                }
            }
        }
    }
}

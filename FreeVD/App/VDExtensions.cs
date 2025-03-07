﻿using FreeVD.Lib.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindowsDesktop;

namespace FreeVD
{
    public static class VDExtensions
    {
        public static int GetNumber(this VirtualDesktop desktop)
        {
            return VirtualDesktop.GetDesktops().ToList().IndexOf(desktop) + 1;
        }

        public static bool Contains(this VirtualDesktop desktop, Window window)
        {
            return window.DesktopNumber == desktop.GetNumber();
        }

        public static void GatherWindows(this VirtualDesktop desktop)
        {
            foreach (var window in Window.GetOpenWindows())
            {
                if (!desktop.Contains(window) && !window.IsDesktop)
                {
                    try
                    {
                        window.MoveToDesktop(desktop);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error moving window: {ex.Message}");
                    }
                }
            }
        }

        public static void GotoDesktop(int desktopNumber)
        {
            if (desktopNumber < 1)
            {
                MessageBox.Show($"Error: Virtual Desktop number is less: {desktopNumber}");
                return;
            }
            Window.EnsureDesktops(desktopNumber);
            User32.SetForegroundWindow(Window.GetTaskbar().Handle);
            var desktops = VirtualDesktop.GetDesktops();
            if (desktops == null)
            {
                MessageBox.Show("Error: Could not find any Virtual Desktop");
                return;
            }
            if (desktops.Count() < desktopNumber)
            {
                MessageBox.Show($"Error: Could not switch to Virtual Desktop #{desktopNumber}");
                return;
            }
            desktops[desktopNumber - 1].Switch();
        }
    }
}

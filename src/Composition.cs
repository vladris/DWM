using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Dwm
{
    #region Colorization structure

    /// <summary>
    /// Stores colorization information
    /// </summary>
    public struct Colorization
    {
        // Private members
        private Color color;
        private bool opaque;

        // Creates the structure given a colorization color value and an opaque blend value
        internal Colorization(int colorization, bool opaqueBlend)
        {
            // Color format is ARGB
            color = Color.FromArgb(colorization);
            opaque = opaqueBlend;
        }

        /// <summary>
        /// Gets the color of the colorization
        /// </summary>
        public Color Color
        {
            get
            {
                return color;
            }
        }

        /// <summary>
        /// Gets a value indicating whether colorization is opaque or not
        /// </summary>
        public bool Opaque
        {
            get
            {
                return opaque;
            }
        }
    }

    #endregion

    /// <summary>
    /// Provides static methods to set or retrieve window compostion properties
    /// </summary>
    public class Composition
    {
        // If the OS is Vista or higher, Dwm is avaialbe, otherwise it is not
        private static bool available = Environment.OSVersion.Version.Major >= 6;

        // Gets a value indicating whether window composition is enabled or not
        private static bool GetComposition()
        {
            // Native API call
            return DwmIsCompositionEnabled();
        }

        // Sets the composition to enabled or disabled
        private static void SetComposition(bool enabled)
        {
            // Native API call
            DwmEnableComposition(enabled);
        }

        /// <summary>
        /// Gets a value indicating if Dwm is available on the system
        /// </summary>
        public static bool Available
        {
            get
            {
                return available;
            }
        }

        /// <summary>
        /// Gets or sets the window composition state
        /// </summary>
        public static bool Enabled
        {
            get
            {
                return GetComposition();
            }
            set
            {
                SetComposition(value);
            }
        }

        /// <summary>
        /// Get colorization information
        /// </summary>
        /// <returns>A Colorization structure containing the colorization information</returns>
        public static Colorization GetColorization()
        {
            // Retrieve color and opaque blend values
            int color;
            bool opaqueBlend;

            // Native API call
            DwmGetColorizationColor(out color, out opaqueBlend);

            // Create and return structure
            return new Colorization(color, opaqueBlend);
        }

        #region API imports

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmEnableComposition(bool compositionAction);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmGetColorizationColor(out int colorization, out bool opaqueBlend);

        #endregion
    }
}

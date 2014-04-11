using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dwm
{
    #region Enumerations

    // Specifies which command is passed through the BlurBehindProperties structure
    [Flags()]
    internal enum BlurBehindFlags
    {
        Enable = 0x0001, // Enable Blur Behind
        BlurRegion = 0x0002, // Define blur region
        TransitionOnMaximized = 0x0004 // Color transition to maximized
    }

    #endregion

    #region Structures

    /// <summary>
    /// Stores a set of four integers defining the margins of an extended frame
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        // Private members
        private int leftWidth;
        private int rightWidth;
        private int topHeight;
        private int bottomHeight;

        /// <summary>
        /// </summary>
        /// <param name="leftWidth">Left width of the extended frame</param>
        /// <param name="rightWidth">Right width of the extended frame</param>
        /// <param name="topHeight">Top height of the extended frame</param>
        /// <param name="bottomHeight">Bottom height of the extended frame</param>
        public Margins(int leftWidth, int rightWidth, int topHeight, int bottomHeight)
        {
            this.leftWidth = leftWidth;
            this.rightWidth = rightWidth;
            this.topHeight = topHeight;
            this.bottomHeight = bottomHeight;
        }

        /// <summary>
        /// Gets or sets the left width
        /// </summary>
        public int LeftWidth
        {
            get
            {
                return leftWidth;
            }
            set
            {
                leftWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the right width
        /// </summary>
        public int RightWidth
        {
            get
            {
                return rightWidth;
            }
            set
            {
                rightWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the top height
        /// </summary>
        public int TopHeight
        {
            get
            {
                return topHeight;
            }
            set
            {
                topHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the bottom height
        /// </summary>
        public int BottomHeight
        {
            get
            {
                return bottomHeight;
            }
            set
            {
                bottomHeight = value;
            }
        }
    }

    // Stores the Blur Behind properties and a flag to indicate which properties 
    // are set by the DwmEnableBlurBehindWindow method
    [StructLayout(LayoutKind.Sequential)]
    internal struct BlurBehindProperties
    {
        public BlurBehindFlags flags; // Flag
        public bool enable; // Enable or disable Blur Behind
        public IntPtr rgnBlur; // Region to which blur is applied
        public bool transitionOnMaximized; // Enable or disable transition on maximized
    }

    #endregion

    /// <summary>
    /// Provides Blur Behind functionality to a form
    /// <remarks>Although it is a part of Blur Behind, extending the frame into the client 
    /// area is actually mutually exclusive with the blur effect so only one of them can
    /// be applied to a form at any time.</remarks>
    /// </summary>
    public class BlurBehind
    {
        // Form handle
        private IntPtr hwnd;

        // Stores the state of Blur Behind
        private bool enabled = false;

        // Existance of extended frame
        private bool extendedFrame = false;

        // Properties - this structure is passed to the DwmEnableBlurBehindWindow method
        private BlurBehindProperties properties;

        /// <summary>
        /// Creates a new instance of BlurBehind from the given handle
        /// </summary>
        /// <param name="hwnd">Handle of a form to which Blur Behind will be applied</param>
        public BlurBehind(IntPtr hwnd)
        {
            this.hwnd = hwnd;

            // Create properties structure
            properties = new BlurBehindProperties();
        }

        /// <summary>
        /// Creates a new instance of BlurBehind from the given form
        /// </summary>
        /// <param name="form">Form to which Blur Behind will be applied</param>
        public BlurBehind(Form form)
            : this(form.Handle)
        {
        }

        /// <summary>
        /// Sets the status of Blur Behind
        /// </summary>
        /// <param name="enabled">True to enable Blur Behind, false to disable Blur Behind</param>
        public void SetBlurBehind(bool enabled)
        {
            // Extended frames and blur are mutually exclusive so extended frame must be 
            // deactivated when blur is activated
            if (enabled && extendedFrame)
            {
                RemoveExtendedFrame();
            }

            // Prepare command
            properties.flags = BlurBehindFlags.Enable;
            properties.enable = enabled;

            // Retain Blur Behind state
            this.enabled = enabled;

            // Native API call
            DwmEnableBlurBehindWindow(hwnd, ref properties);
        }

        /// <summary>
        /// Sets the region to which blur is applied
        /// </summary>
        /// <param name="region">Region that defines the area to which blur will be applied.</param>
        public void SetBlurBehindRegion(Region region)
        {
            // Check if Blur Behind is disabled and enable it
            if (!enabled)
            {
                SetBlurBehind(true);
            }

            // Prepare command
            properties.flags = BlurBehindFlags.BlurRegion;

            // A null region means the whole form will be blurred
            if (region != null)
            {
                // A Graphics reference is also needed to identify the surface this region
                // belongs to but we get that from the form handle
                properties.rgnBlur = region.GetHrgn(Graphics.FromHwnd(hwnd));
            }
            else
            {
                // Set property to NULL so the whole form will be blurred
                properties.rgnBlur = IntPtr.Zero;
            }

            // Native API call
            DwmEnableBlurBehindWindow(hwnd, ref properties);
        }

        /// <summary>
        /// Sets the color transition to maximized
        /// </summary>
        /// <param name="transition">True to perform the color transition, false otherwise</param>
        public void SetTransitionOnMaximized(bool transition)
        {
            // Prepare command
            properties.flags = BlurBehindFlags.TransitionOnMaximized;
            properties.transitionOnMaximized = transition;

            // Native API call
            DwmEnableBlurBehindWindow(hwnd, ref properties);
        }

        /// <summary>
        /// Extends the frame of the form into the client area
        /// </summary>
        /// <param name="margins">The margins that define the frame extention</param>
        public void ExtendFrameIntoClientArea(Margins margins)
        {
            // We check if all margin elements are 0, because that means the extended frame 
            // will be disabled
            extendedFrame = !((margins.LeftWidth == 0) && (margins.RightWidth == 0) && 
                (margins.TopHeight == 0) && (margins.BottomHeight == 0));

            // If Blur Behind is enabled it must be disabled because it is mutually exclusive
            // with the extended frame.
            if (enabled && extendedFrame)
            {
                // Deactivate Blur Behind
                SetBlurBehind(false);
            }

            // Native API call
            DwmExtendFrameIntoClientArea(hwnd, ref margins);
        }

        /// <summary>
        /// Removes the extended frame
        /// </summary>
        public void RemoveExtendedFrame()
        {
            // We just need to call ExtendFrameIntoClientArea with all margin elements set to 0
            ExtendFrameIntoClientArea(new Margins());
        }

        /// <summary>
        /// Gets a value indicating if Blur Behind is enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                return enabled;
            }
        }

        /// <summary>
        /// Gets a value indicating if the frame is extended
        /// </summary>
        public bool ExtendedFrame
        {
            get
            {
                return extendedFrame;
            }
        }

        #region API imports

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref BlurBehindProperties blurBehind);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins margins);

        #endregion
    }
}

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dwm
{
    #region Enumerations

    /// <summary>
    /// Rendering policies for the non client area of the window
    /// </summary>
    public enum NonClientRenderingPolicy
    {
        /// <summary>
        /// Use default rendering
        /// </summary>
        UseWindowStyle,

        /// <summary>
        /// Disable rendering of the non client area
        /// </summary>
        Disabled,

        /// <summary>
        /// Enable rendering of the non client area
        /// </summary>
        Enabled
    }

    /// <summary>
    /// Flip 3D policies
    /// </summary>
    public enum Flip3DPolicy
    {
        /// <summary>
        /// Default behaviour
        /// </summary>
        Default,
        
        /// <summary>
        /// Excludes the window from Flip 3D, showing it below the other windows
        /// </summary>
        ExcludeBelow,

        /// <summary>
        /// Excludes the window from Flip 3D, showing it above the other windows
        /// </summary>
        ExcludeAbove
    }

    // Window attribute values that determine the attribute retrieved by DwmGetWindowAttribute
    // or set by DwmSetWindowAttribute
    internal enum WindowAttribute
    {
        NcRenderingEnabled = 1, // Non client rendering enabled
        NcRenderingPolicy, // Non client rendering policy
        TransitionsForceDisabled, // Transitions force disabled
        AllowNcPaint, // Allow non client paint
        CaptionButtonBounds, // Bounds of caption buttons
        NonClientRtlLayout, // Non client right-to-left layout
        ForceIconicRepresentation, // Force iconic representation instead of thumbnail
        Flip3DPolicy, // Flip 3D policy
        ExtendedFrameBounds // Bounds of extended frame 
    }

    #endregion

    /// <summary>
    /// Provides methods do retrieve/modify the attributes of a form
    /// </summary>
    public class FormAttributes
    {
        // Form handle
        private IntPtr hwndForm;

        /// <summary>
        /// Creates a new instance of FormAttributes from a given handle
        /// </summary>
        /// <param name="hwndForm">Handle of a form</param>
        public FormAttributes(IntPtr hwndForm)
        {
            this.hwndForm = hwndForm;
        }

        /// <summary>
        /// Creates a new instance of FormAttributes from a given form
        /// </summary>
        /// <param name="form">Form</param>
        public FormAttributes(Form form)
            : this(form.Handle)
        {
        }

        // Gets a boolean value for the specified attribute
        private bool GetBoolValue(WindowAttribute attribute)
        {
            // Stores the result 
            bool result;

            // Native API call
            DwmGetWindowAttribute(hwndForm, attribute, out result, sizeof(int));

            return result;
        }

        // Sets a boolean value for the specified attribute
        private void SetBoolValue(WindowAttribute attribute, ref bool value)
        {
            // Native API call
            DwmSetWindowAttribute(hwndForm, attribute, ref value, sizeof(int));
        }

        // Sets an int value for the specified attribute
        private void SetIntValue(WindowAttribute attribute, ref int value)
        {
            // Native API call
            DwmSetWindowAttribute(hwndForm, attribute, ref value, sizeof(int));
        }

        // Gets a rectangle value for the given attribute
        private Rectangle GetRectangleValue(WindowAttribute attribute)
        {
            // Stores the result
            Rectangle result = new Rectangle();

            // Native API call
            DwmGetWindowAttribute(hwndForm, attribute, out result, Marshal.SizeOf(result));

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether non client rendering is enabled
        /// </summary>
        public bool NonClientRendering
        {
            get
            {
                return GetBoolValue(WindowAttribute.NcRenderingEnabled);
            }
        }

        /// <summary>
        /// Sets the rendering policy of the non client area of the form
        /// </summary>
        /// <param name="policy">Non client rendering policy</param>
        public void SetNonClientRenderingPolicy(NonClientRenderingPolicy policy)
        {
            int value = (int)policy;
            SetIntValue(WindowAttribute.NcRenderingPolicy, ref value);
        }

        /// <summary>
        /// Enables or disables force transitions for the form
        /// </summary>
        /// <param name="disable">True to disable transitions</param>
        public void ForceDisableTransitions(bool disable)
        {
            SetBoolValue(WindowAttribute.TransitionsForceDisabled, ref disable);
        }

        /// <summary>
        /// Enables or disables painting of the non client area or the form
        /// </summary>
        /// <param name="allow">True to allow painting of the non client area</param>
        public void SetNonClientPaint(bool allow)
        {
            SetBoolValue(WindowAttribute.AllowNcPaint, ref allow);
        }

        /// <summary>
        /// Enables or disables right-to-left layout for the non client area of the form
        /// </summary>
        /// <param name="rtl">True to activate right-to-left layout</param>
        public void SetNonClientRtlLayout(bool rtl)
        {
            SetBoolValue(WindowAttribute.NonClientRtlLayout, ref rtl);
        }

        /// <summary>
        /// Enables or disables iconic representation of the form in thumbnails
        /// </summary>
        /// <param name="force">True to force iconic representation</param>
        public void ForceIconicRepresentation(bool force)
        {
            SetBoolValue(WindowAttribute.ForceIconicRepresentation, ref force);
        }

        /// <summary>
        /// Sets the Flip 3D policy of the form
        /// </summary>
        /// <param name="policy">Flip 3D policy to apply</param>
        public void SetFlip3DPolicy(Flip3DPolicy policy)
        {
            int value = (int)policy;
            SetIntValue(WindowAttribute.Flip3DPolicy, ref value);
        }

        /// <summary>
        /// Gets the bounds of the caption buttons
        /// </summary>
        /// <returns>Rectangle structure containing the bounds of the caption buttons</returns>
        public Rectangle GetCaptionButtonBounds()
        {
            return GetRectangleValue(WindowAttribute.CaptionButtonBounds);
        }

        /// <summary>
        /// Gets the bounds of the extended frame
        /// </summary>
        /// <returns>Rectangle structure containing the bounds of the extended frame</returns>
        public Rectangle GetExtendedFrameBounds()
        {
            return GetRectangleValue(WindowAttribute.ExtendedFrameBounds);
        }

        #region API imports

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmGetWindowAttribute(IntPtr hwnd, WindowAttribute attribute, out bool attributeValue, int attributeSize);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmGetWindowAttribute(IntPtr hwnd, WindowAttribute attribute, out Rectangle attributeValue, int attributeSize);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmSetWindowAttribute(IntPtr hwnd, WindowAttribute attribute, ref bool attributeValue, int attributeSize);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmSetWindowAttribute(IntPtr hwnd, WindowAttribute attribute, ref int attributeValue, int attributeSize);
        
        #endregion
    }
}

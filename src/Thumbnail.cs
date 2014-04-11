using System;
using System.Drawing;
using System.Runtime.InteropServices; 
using System.Windows.Forms;

namespace Dwm
{
    #region Enumerations

    // Specifies which command is passed through the ThumbnailProperties structure
    [Flags()]
    internal enum ThumbnailFlags
    {
        RectDestination = 0x0000001, // Rectangle destination of thumbnail
        RectSource = 0x0000002, // Rectangle source of thumbnail
        Opacity = 0x0000004, // Opacity
        Visible = 0x0000008, // Visibility
        SourceClientAreaOnly = 0x00000010 // Displays only the client area of the source
    }

    // Stores the Thumbnail properties and a flag to indicate which properties 
    // are set by the DwmUpdateThumbnailProperties method
    [StructLayout(LayoutKind.Sequential)]
    internal struct ThumbnailProperties
    {
        public ThumbnailFlags flags; // Flag
        public Rectangle destination; // Destination rectangle
        public Rectangle source; // Source rectangle
        public byte opacity; // Opacity
        public bool visible; // Visibility
        public bool sourceClientAreaOnly; // Displays only the client area of the source
    }
    
    #endregion

    /// <summary>
    /// Provides methods to display a thumbnail of a given form on another form
    /// </summary>
    public class Thumbnail : IDisposable
    {
        // Thumbnail handle
        private IntPtr handle;

        // ThumbnailProperties structure to be passed to DwmUpdateThumbnailProperties
        private ThumbnailProperties properties;

        // Indicates whether the object was disposed
        private bool disposed = false;

        /// <summary>
        /// Creates a new thumbnail given a destination form handle and a source form handle
        /// </summary>
        /// <param name="hwndDestination">Handle of the form where the thumbnail will appear</param>
        /// <param name="hwndSource">Handle of the form for which the thumbnail is created</param>
        public Thumbnail(IntPtr hwndDestination, IntPtr hwndSource)
        {
            // Native API call
            handle = DwmRegisterThumbnail(hwndDestination, hwndSource);

            // Initialize properties structure
            properties = new ThumbnailProperties();
        }

        /// <summary>
        /// Creates a new thumbnail given a destination form and a source form handle
        /// </summary>
        /// <param name="destination">Form where the thumbnail will appear</param>
        /// <param name="hwndSource">Handle of the form for which the thumbnail is created</param>
        public Thumbnail(Form destination, IntPtr hwndSource)
            : this(destination.Handle, hwndSource)
        {
        }

        /// <summary>
        /// Creates a new thumbnail given a destination form and a source form
        /// </summary>
        /// <param name="destination">Form where the thumbnail will appear</param>
        /// <param name="source">Form for which the thumbnail is created</param>
        public Thumbnail(Form destination, Form source)
            : this(destination.Handle, source.Handle)
        {
        }

        /// <summary>
        /// Gets the size of the thumbnail source
        /// </summary>
        /// <returns>Size structure indicating the size of the thumbnail source</returns>
        public Size GetSourceSize()
        {
            // Initialize structure
            Size result = new Size();

            // Check if handle is valid
            if (handle != IntPtr.Zero)
            {
                try
                {
                    // Native API call
                    DwmQueryThumbnailSourceSize(handle, out result);
                }
                // If source or destination handles become invalid, the thumbnail handle
                // will also become invalid and an ArgumentException will be thrown
                catch (ArgumentException)
                {
                    // Handle became invalid - set it to null
                    this.handle = IntPtr.Zero;
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the rectangle that will be shown by the thumbnail
        /// </summary>
        /// <param name="sourceRectangle">Rectangle on the source form to be shown in thumbnail</param>
        public void SetSourceRectangle(Rectangle sourceRectangle)
        {
            // Check if handle is valid
            if (handle != IntPtr.Zero)
            {
                // Prepare command
                properties.flags = ThumbnailFlags.RectSource;
                properties.source = sourceRectangle;

                try
                {
                    // Native API call
                    DwmUpdateThumbnailProperties(handle, ref properties);
                }
                // If source or destination handles become invalid, the thumbnail handle
                // will also become invalid and an ArgumentException will be thrown
                catch (ArgumentException)
                {
                    // Handle became invalid - set it to null
                    handle = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Sets the rectangle on which the thumbnail will be drawn
        /// </summary>
        /// <param name="destinationRectangle">Rectangle on the destination form on which to draw 
        /// the thumbnail</param>
        public void SetDestinationRectangle(Rectangle destinationRectangle)
        {
            // Check if handle is valid
            if (handle != IntPtr.Zero)
            {
                // Prepare command
                properties.flags = ThumbnailFlags.RectDestination;
                properties.destination = destinationRectangle;

                try
                {
                    // Native API call
                    DwmUpdateThumbnailProperties(handle, ref properties);
                }
                catch (ArgumentException)
                {
                    // Handle became invalid - set it to null
                    handle = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Sets the opacity of the thumbnail
        /// </summary>
        /// <param name="opacity">Opacity value</param>
        public void SetOpacity(byte opacity)
        {
            // Check if handle is valid
            if (handle != IntPtr.Zero)
            {
                // Prepare command
                properties.flags = ThumbnailFlags.Opacity;
                properties.opacity = opacity;

                try
                {
                    // Native API call
                    DwmUpdateThumbnailProperties(handle, ref properties);
                }
                catch (ArgumentException)
                {
                    // Handle became invalid - set it to null
                    handle = IntPtr.Zero;
                }
            }
        }


        /// <summary>
        /// Sets the visiblity of the thumbnail
        /// </summary>
        /// <param name="visible">True to show the thumbnail, false to hide it</param>
        public void SetVisible(bool visible)
        {
            // Check if handle is valid
            if (handle != IntPtr.Zero)
            {
                // Prepare command
                properties.flags = ThumbnailFlags.Visible;
                properties.visible = visible;

                try
                {
                    // Native API call
                    DwmUpdateThumbnailProperties(handle, ref properties);
                }
                catch (ArgumentException)
                {
                    // Handle became invalid - set it to null 
                    handle = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Sets the thumbnail to show only the client area of the source
        /// </summary>
        /// <param name="sourceClientAreaOnly">True to show only client area, false otherwise</param>
        public void SetSourceClientAreaOnly(bool sourceClientAreaOnly)
        {
            // Check if handle is valid
            if (handle != IntPtr.Zero)
            {
                // Prepare command
                properties.flags = ThumbnailFlags.SourceClientAreaOnly;
                properties.sourceClientAreaOnly = sourceClientAreaOnly;

                try
                {
                    // Native API call
                    DwmUpdateThumbnailProperties(handle, ref properties);
                }
                catch (ArgumentException)
                {
                    // Handle became invalid - set it to null
                    handle = IntPtr.Zero;
                }
            }
        }

        // Unregister the thumbnail
        private void UnRegisterThumbnail()
        {
            // Check if handle is valid
            if (handle != IntPtr.Zero)
            {
                try
                {
                    // Native API call
                    DwmUnregisterThumbnail(handle);
                }
                catch (ArgumentException)
                {
                }

                handle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Dispose of the thumbnail
        /// </summary>
        public void Dispose()
        {
            // Unregister the thumbnail to free the handle
            if (!this.disposed)
            {
                UnRegisterThumbnail();
            }
            disposed = true;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of the thumbnail and destroy the object
        /// </summary>
        ~Thumbnail() 
        {
            Dispose();
        }

        #region API imports

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmQueryThumbnailSourceSize(IntPtr hThumbnail, out Size size);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern IntPtr DwmRegisterThumbnail(IntPtr hwndDestination, IntPtr hwndSource);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmUnregisterThumbnail(IntPtr hThumbnail);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmUpdateThumbnailProperties(IntPtr hThumbnailId, ref ThumbnailProperties ptnProperties);

        #endregion
    }
}

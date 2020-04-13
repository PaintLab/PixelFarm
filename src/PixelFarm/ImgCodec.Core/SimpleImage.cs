//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// =============================================================================== 
using System;

namespace ImageTools
{

    public class SimpleImage
    {
        public byte[] Pixels { get; private set; }
        public int PixelHeight { get; private set; }
        public int PixelWidth { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this image has been loaded.
        /// </summary>
        /// <value><c>true</c> if this image has been loaded; otherwise, <c>false</c>.</value>

        public bool IsFilled { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleImage"/> class
        /// with the height and the width of the image.
        /// </summary>
        /// <param name="width">The width of the image in pixels.</param>
        /// <param name="height">The height of the image in pixels.</param>
        /// <exception cref="ArgumentException">
        ///     <para><paramref name="width"/> is equals or less than zero.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="height"/> is equals or less than zero.</para>
        /// </exception>
        public SimpleImage(int width, int height)
        {
            //Contract.Requires<ArgumentException>(width >= 0, "Width must be greater or equals than zero.");
            //Contract.Requires<ArgumentException>(height >= 0, "Height must be greater or equals than zero.");
            //Contract.Ensures(IsFilled);

            PixelWidth = width;
            PixelHeight = height;

            //32 bit img
            //consider layz init,            
            Pixels = new byte[PixelWidth * PixelHeight * 4];
            IsFilled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleImage"/> class
        /// by making a copy from another image.
        /// </summary>
        /// <param name="other">The other, where the clone should be made from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="other"/> is not loaded.</exception>
        public SimpleImage(SimpleImage other)
        {
            //Contract.Requires<ArgumentNullException>(other != null, "Other image cannot be null.");
            //Contract.Requires<ArgumentException>(other.IsFilled, "Other image has not been loaded.");
            //Contract.Ensures(IsFilled);

            byte[] pixels = other.Pixels;

            PixelWidth = other.PixelWidth;
            PixelHeight = other.PixelHeight;
            Pixels = new byte[pixels.Length];

            Array.Copy(pixels, Pixels, pixels.Length);

            IsFilled = other.IsFilled;
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleImage"/> class.
        /// </summary>
        public SimpleImage()
        {
        }


        /// <summary>
        /// Sets the pixel array of the image.
        /// </summary>
        /// <param name="width">The new width of the image.
        /// Must be greater than zero.</param>
        /// <param name="height">The new height of the image.
        /// Must be greater than zero.</param>
        /// <param name="pixels">The array with colors. Must be a multiple
        /// of four, width and height.</param>
        /// <exception cref="ArgumentException">
        /// 	<para><paramref name="width"/> is smaller than zero.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="height"/> is smaller than zero.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="pixels"/> is not a multiple of four, 
        /// 	width and height.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="pixels"/> is null.</exception>
        public void SetPixels(int width, int height, byte[] pixels)
        {

            if (pixels.Length != width * height * 4)
            {
                throw new ArgumentException(
                    "Pixel array must have the length of width * height * 4.",
                    "pixels");
            }

            PixelWidth = width;
            PixelHeight = height;
            Pixels = pixels;
            IsFilled = true;
        }
        public int BitDepth { get; set; }
        public ExtraImageInfo ExtraImageInfo { get; set; }
    }
}

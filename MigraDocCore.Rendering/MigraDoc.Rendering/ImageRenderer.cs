#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Klaus Potzesny (mailto:Klaus.Potzesny@PdfSharpCore.com)
//
// Copyright (c) 2001-2009 empira Software GmbH, Cologne (Germany)
//
// http://www.PdfSharpCore.com
// http://www.migradoc.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.IO;
using System.Diagnostics;
using MigraDocCore.DocumentObjectModel;
using PdfSharpCore.Drawing;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.Rendering.MigraDoc.Rendering.Resources;
using PdfSharpCore.Fonts;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;

namespace MigraDocCore.Rendering
{
    /// <summary>
    /// Renders images.
    /// </summary>
    internal class ImageRenderer : ShapeRenderer
    {
        internal ImageRenderer(XGraphics gfx, Image image, FieldInfos fieldInfos)
          : base(gfx, image, fieldInfos)
        {
            this.image = image;
            ImageRenderInfo renderInfo = new ImageRenderInfo();
            renderInfo.shape = this.shape;
            this.renderInfo = renderInfo;
        }

        internal ImageRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos)
          : base(gfx, renderInfo, fieldInfos)
        {
            this.image = (Image)renderInfo.DocumentObject;
        }

        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            ImageFormatInfo formatInfo = (ImageFormatInfo)this.renderInfo.FormatInfo;
            formatInfo.ImageSource = image.Source;
            formatInfo.Failure = this.failure;
            CalculateImageDimensions();
            base.Format(area, previousFormatInfo);
        }

        protected override XUnit ShapeHeight
        {
            get
            {
                ImageFormatInfo formatInfo = (ImageFormatInfo)this.renderInfo.FormatInfo;
                return formatInfo.Height + this.lineFormatRenderer.GetWidth();
            }
        }

        protected override XUnit ShapeWidth
        {
            get
            {
                ImageFormatInfo formatInfo = (ImageFormatInfo)this.renderInfo.FormatInfo;
                return formatInfo.Width + this.lineFormatRenderer.GetWidth();
            }
        }

        internal override void Render()
        {
            RenderFilling();

            ImageFormatInfo formatInfo = (ImageFormatInfo)this.renderInfo.FormatInfo;
            Area contentArea = this.renderInfo.LayoutInfo.ContentArea;
            XRect destRect = new XRect(contentArea.X, contentArea.Y, formatInfo.Width, formatInfo.Height);

            if (formatInfo.Failure == ImageFailure.None)
            {
                try
                {
                    XRect srcRect = new XRect(formatInfo.CropX, formatInfo.CropY, formatInfo.CropWidth, formatInfo.CropHeight);
                    using (var xImage = XImage.FromImageSource(formatInfo.ImageSource))
                        this.gfx.DrawImage(xImage, destRect, srcRect, XGraphicsUnit.Point); //Pixel.
                }
                catch (Exception)
                {
                    RenderFailureImage(destRect);
                }
            }
            else
                RenderFailureImage(destRect);

            RenderLine();
        }

        void RenderFailureImage(XRect destRect)
        {
            this.gfx.DrawRectangle(XBrushes.LightGray, destRect);
            string failureString;
            ImageFormatInfo formatInfo = (ImageFormatInfo)this.RenderInfo.FormatInfo;

            switch (formatInfo.Failure)
            {
                case ImageFailure.EmptySize:
                    failureString = AppResources.DisplayEmptyImageSize;
                    break;

                case ImageFailure.FileNotFound:
                    failureString = AppResources.DisplayImageFileNotFound;
                    break;

                case ImageFailure.InvalidType:
                    failureString = AppResources.DisplayInvalidImageType;
                    break;

                case ImageFailure.NotRead:
                default:
                    failureString = AppResources.DisplayImageNotRead;
                    break;
            }

            // Create stub font
            XFont font = new XFont(GlobalFontSettings.FontResolver.DefaultFontName, 8);
            this.gfx.DrawString(failureString, font, XBrushes.Red, destRect, XStringFormats.Center);
        }

        private void CalculateImageDimensions()
        {
            ImageFormatInfo formatInfo = (ImageFormatInfo)this.renderInfo.FormatInfo;

            if (formatInfo.Failure == ImageFailure.None)
            {
                XImage xImage = null;
                try
                {
                    xImage = XImage.FromImageSource(formatInfo.ImageSource);
                }
                catch (InvalidOperationException ex)
                {
                    Debug.WriteLine(string.Format(AppResources.InvalidImageType, ex.Message));
                    formatInfo.Failure = ImageFailure.InvalidType;
                }

                try
                {
                    XUnit usrWidth = image.Width.Point;
                    XUnit usrHeight = image.Height.Point;
                    bool usrWidthSet = !this.image.IsNull("Width");
                    bool usrHeightSet = !this.image.IsNull("Height");

                    XUnit resultWidth = usrWidth;
                    XUnit resultHeight = usrHeight;

                    double xPixels = xImage.PixelWidth;
                    bool usrResolutionSet = !image.IsNull("Resolution");

                    double horzRes = usrResolutionSet ? (double)image.Resolution : xImage.HorizontalResolution;
                    XUnit inherentWidth = XUnit.FromInch(xPixels / horzRes);
                    double yPixels = xImage.PixelHeight;
                    double vertRes = usrResolutionSet ? (double)image.Resolution : xImage.VerticalResolution;
                    XUnit inherentHeight = XUnit.FromInch(yPixels / vertRes);

                    bool lockRatio = this.image.IsNull("LockAspectRatio") ? true : image.LockAspectRatio;

                    double scaleHeight = this.image.ScaleHeight;
                    double scaleWidth = this.image.ScaleWidth;
                    bool scaleHeightSet = !this.image.IsNull("ScaleHeight");
                    bool scaleWidthSet = !this.image.IsNull("ScaleWidth");

                    if (lockRatio)
                    {
                        if (usrWidthSet && usrHeightSet)
                        {
                            if (inherentHeight / usrHeight > inherentWidth / usrWidth)
                            {
                                usrWidthSet = false;
                            }
                            else
                            {
                                usrHeightSet = false;
                            }
                        }
                        if (usrWidthSet && !usrHeightSet)
                        {
                            resultHeight = inherentHeight / inherentWidth * usrWidth;
                        }
                        else if (usrHeightSet && !usrWidthSet)
                        {
                            resultWidth = inherentWidth / inherentHeight * usrHeight;
                        }
                        else if (!usrHeightSet && !usrWidthSet)
                        {
                            resultHeight = inherentHeight;
                            resultWidth = inherentWidth;
                        }

                        if (scaleHeightSet || scaleHeightSet && scaleWidthSet && scaleHeight < scaleWidth)
                        {
                            resultHeight = resultHeight * scaleHeight;
                            resultWidth = resultWidth * scaleHeight;
                        }
                        else if (scaleWidthSet || scaleHeightSet && scaleWidthSet && scaleHeight > scaleWidth)
                        {
                            resultHeight = resultHeight * scaleWidth;
                            resultWidth = resultWidth * scaleWidth;
                        }
                    }
                    else
                    {
                        if (!usrHeightSet)
                            resultHeight = inherentHeight;

                        if (!usrWidthSet)
                            resultWidth = inherentWidth;

                        if (scaleHeightSet)
                            resultHeight = resultHeight * scaleHeight;
                        if (scaleWidthSet)
                            resultWidth = resultWidth * scaleWidth;
                    }

                    formatInfo.CropWidth = (int)xPixels;
                    formatInfo.CropHeight = (int)yPixels;
                    if (!this.image.IsNull("PictureFormat"))
                    {
                        PictureFormat picFormat = this.image.PictureFormat;
                        //Cropping in pixels.
                        XUnit cropLeft = picFormat.CropLeft.Point;
                        XUnit cropRight = picFormat.CropRight.Point;
                        XUnit cropTop = picFormat.CropTop.Point;
                        XUnit cropBottom = picFormat.CropBottom.Point;
                        formatInfo.CropX = (int)(horzRes * cropLeft.Inch);
                        formatInfo.CropY = (int)(vertRes * cropTop.Inch);
                        formatInfo.CropWidth -= (int)(horzRes * ((XUnit)(cropLeft + cropRight)).Inch);
                        formatInfo.CropHeight -= (int)(vertRes * ((XUnit)(cropTop + cropBottom)).Inch);

                        //Scaled cropping of the height and width.
                        double xScale = resultWidth / inherentWidth;
                        double yScale = resultHeight / inherentHeight;

                        cropLeft = xScale * cropLeft;
                        cropRight = xScale * cropRight;
                        cropTop = yScale * cropTop;
                        cropBottom = yScale * cropBottom;

                        resultHeight = resultHeight - cropTop - cropBottom;
                        resultWidth = resultWidth - cropLeft - cropRight;
                    }
                    if (resultHeight <= 0 || resultWidth <= 0)
                    {
                        formatInfo.Width = XUnit.FromCentimeter(2.5);
                        formatInfo.Height = XUnit.FromCentimeter(2.5);
                        Debug.WriteLine(AppResources.EmptyImageSize);
                        this.failure = ImageFailure.EmptySize;
                    }
                    else
                    {
                        formatInfo.Width = resultWidth;
                        formatInfo.Height = resultHeight;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format(AppResources.ImageNotReadable, image.Source.ToString(), ex.Message));
                    formatInfo.Failure = ImageFailure.NotRead;
                }
                finally
                {
                    if (xImage != null)
                        xImage.Dispose();
                }
            }
            if (formatInfo.Failure != ImageFailure.None)
            {
                if (!this.image.IsNull("Width"))
                    formatInfo.Width = this.image.Width.Point;
                else
                    formatInfo.Width = XUnit.FromCentimeter(2.5);

                if (!this.image.IsNull("Height"))
                    formatInfo.Height = this.image.Height.Point;
                else
                    formatInfo.Height = XUnit.FromCentimeter(2.5);
                return;
            }
        }

        Image image;
        ImageFailure failure;
    }
}

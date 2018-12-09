using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;

namespace CMSSolutions.Websites.Extensions
{
    public class ResizePhoto
    {
        public static string Resize(string sourceFile, int maxWidth, int maxHeight, bool preserverAspectRatio = true, int quality = 100)
        {
            System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(sourceFile));
            maxWidth = maxWidth == 0 ? sourceImage.Width : maxWidth;
            maxHeight = maxHeight == 0 ? sourceImage.Height : maxHeight;

            Size oSize = preserverAspectRatio ? GetAspectRatioSize(maxWidth, maxHeight, sourceImage.Width, sourceImage.Height) : new Size(maxWidth, maxHeight);
            Image oResampled;
            if (sourceImage.PixelFormat == PixelFormat.Indexed || sourceImage.PixelFormat == PixelFormat.Format1bppIndexed || sourceImage.PixelFormat == PixelFormat.Format4bppIndexed || sourceImage.PixelFormat == PixelFormat.Format8bppIndexed || sourceImage.PixelFormat.ToString() == "8207")
            {
                oResampled = new Bitmap(oSize.Width, oSize.Height, PixelFormat.Format24bppRgb);
            }
            else
            {
                oResampled = new Bitmap(oSize.Width, oSize.Height, sourceImage.PixelFormat);
            }
            Graphics oGraphics = Graphics.FromImage(oResampled);
            Rectangle oRectangle;
            if (quality > 80)
            {
                oGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                oRectangle = new Rectangle(-1, -1, oSize.Width + 1, oSize.Height + 1);
            }
            else
            {
                oRectangle = new Rectangle(0, 0, oSize.Width, oSize.Height);
            }
            oGraphics.FillRectangle(new SolidBrush(Color.White), oRectangle);
            oGraphics.DrawImage(sourceImage, oRectangle);    
            sourceImage.Dispose();
            String extension = System.IO.Path.GetExtension(sourceFile).ToLower();
            var memo = new System.IO.MemoryStream();
            if (extension == ".jpg" || extension == ".jpeg")
            {
                ImageCodecInfo oCodec = GetJpgCodec();
                if (oCodec != null)
                {
                    var aCodecParams = new EncoderParameters(1);
                    aCodecParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                    oResampled.Save(memo, oCodec, aCodecParams);
                    byte[] imageBytes = memo.GetBuffer();
                    memo.Close();
                    string base64String = Convert.ToBase64String(imageBytes);
                    return "data:image/png;base64," + base64String;
                }
            }
            else
            {
                switch (extension)
                {
                    case ".gif":
                        {
                            var quantizer = new OctreeQuantizer(255, 8);
                            using (Bitmap quantized = quantizer.Quantize(oResampled))
                            {
                                quantized.Save(memo, ImageFormat.Gif);
                                byte[] imageBytes = memo.GetBuffer();
                                memo.Close();
                                string base64String = Convert.ToBase64String(imageBytes);
                                return "data:image/png;base64," + base64String;
                            }
                        }
                    case ".png":
                        {
                            oResampled.Save(memo, ImageFormat.Png);
                            byte[] imageBytes = memo.GetBuffer();
                            memo.Close();
                            string base64String = Convert.ToBase64String(imageBytes);
                            return "data:image/png;base64," + base64String;
                        }
                    case ".bmp":
                        {
                            oResampled.Save(memo, ImageFormat.Png);
                            byte[] imageBytes = memo.GetBuffer();
                            memo.Close();
                            string base64String = Convert.ToBase64String(imageBytes);
                            return "data:image/png;base64," + base64String;
                        }
                }
            }

            oGraphics.Dispose();
            oResampled.Dispose();
            return string.Empty;
        }

        private static ImageCodecInfo GetJpgCodec()
        {
            ImageCodecInfo[] aCodecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo oCodec = null;  
            for (int i = 0; i < aCodecs.Length; i++)
            {
                if (aCodecs[i].MimeType.Equals("image/jpeg"))
                {
                    oCodec = aCodecs[i];
                    break;
                }
            }   
            return oCodec;
        }

        private static Size GetAspectRatioSize(int maxWidth, int maxHeight, int actualWidth, int actualHeight)
        {
            Size oSize = new System.Drawing.Size(maxWidth, maxHeight);
            float iFactorX = (float)maxWidth / (float)actualWidth;
            float iFactorY = (float)maxHeight / (float)actualHeight;
            if (iFactorX != 1 || iFactorY != 1)
            {
                if (iFactorX < iFactorY) { oSize.Height = (int)Math.Round((float)actualHeight * iFactorX); }
                else if (iFactorX > iFactorY) { oSize.Width = (int)Math.Round((float)actualWidth * iFactorY); }
            }  
            if (oSize.Height <= 0) oSize.Height = 1;
            if (oSize.Width <= 0) oSize.Width = 1;
            return oSize;
        }
    }

    public class OctreeQuantizer : Quantizer
    {
        public OctreeQuantizer(int maxColors, int maxColorBits)
            : base(false)
        {
            if (maxColors > 255)
            {
                throw new ArgumentOutOfRangeException("maxColors", maxColors, "The number of colors should be less than 256");
            }
            if ((maxColorBits < 1) | (maxColorBits > 8))
            {
                throw new ArgumentOutOfRangeException("maxColorBits", maxColorBits, "This should be between 1 and 8");
            }
            _octree = new Octree(maxColorBits);
            _maxColors = maxColors;
        }
        protected override void InitialQuantizePixel(Color32 pixel)
        {
            _octree.AddColor(pixel);
        }
        protected override byte QuantizePixel(Color32 pixel)
        {
            byte paletteIndex = (byte)_maxColors;
            if (pixel.Alpha > 0)
            {
                paletteIndex = (byte)_octree.GetPaletteIndex(pixel);
            }
            return paletteIndex;
        }
        protected override ColorPalette GetPalette(ColorPalette original)
        {
            ArrayList palette = _octree.Palletize(_maxColors - 1);
            for (int index = 0; index < palette.Count; index++)
            {
                original.Entries[index] = (Color)palette[index];
            }
            original.Entries[_maxColors] = Color.FromArgb(0, 0, 0, 0);
            return original;
        }
        private Octree _octree;
        private int _maxColors;
        private class Octree
        {
            public Octree(int maxColorBits)
            {
                _maxColorBits = maxColorBits;
                _leafCount = 0;
                _reducibleNodes = new OctreeNode[9];
                _root = new OctreeNode(0, _maxColorBits, this);
                _previousColor = 0;
                _previousNode = null;
            }
            public void AddColor(Color32 pixel)
            {
                if (_previousColor == pixel.ARGB)
                {
                    if (null == _previousNode)
                    {
                        _previousColor = pixel.ARGB;
                        _root.AddColor(pixel, _maxColorBits, 0, this);
                    }
                    else
                    {
                        _previousNode.Increment(pixel);
                    }
                }
                else
                {
                    _previousColor = pixel.ARGB;
                    _root.AddColor(pixel, _maxColorBits, 0, this);
                }
            }
            public void Reduce()
            {
                int index;
                for (index = _maxColorBits - 1; (index > 0) && (null == _reducibleNodes[index]); index--) ;
                OctreeNode node = _reducibleNodes[index];
                _reducibleNodes[index] = node.NextReducible;
                _leafCount -= node.Reduce();
                _previousNode = null;
            }
            public int Leaves
            {
                get { return _leafCount; }
                set { _leafCount = value; }
            }
            protected OctreeNode[] ReducibleNodes
            {
                get { return _reducibleNodes; }
            }
            protected void TrackPrevious(OctreeNode node)
            {
                _previousNode = node;
            }
            public ArrayList Palletize(int colorCount)
            {
                while (Leaves > colorCount)
                {
                    Reduce();
                }
                ArrayList palette = new ArrayList(Leaves);
                int paletteIndex = 0;
                _root.ConstructPalette(palette, ref paletteIndex);
                return palette;
            }
            public int GetPaletteIndex(Color32 pixel)
            {
                return _root.GetPaletteIndex(pixel, 0);
            }
            private static int[] mask = new int[8] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };
            private OctreeNode _root;
            private int _leafCount;
            private OctreeNode[] _reducibleNodes;
            private int _maxColorBits;
            private OctreeNode _previousNode;
            private int _previousColor;
            protected class OctreeNode
            {
                public OctreeNode(int level, int colorBits, Octree octree)
                {
                    _leaf = (level == colorBits);

                    _red = _green = _blue = 0;
                    _pixelCount = 0;
                    if (_leaf)
                    {
                        octree.Leaves++;
                        _nextReducible = null;
                        _children = null;
                    }
                    else
                    {
                        _nextReducible = octree.ReducibleNodes[level];
                        octree.ReducibleNodes[level] = this;
                        _children = new OctreeNode[8];
                    }
                }
                public void AddColor(Color32 pixel, int colorBits, int level, Octree octree)
                {
                    if (_leaf)
                    {
                        Increment(pixel);
                        octree.TrackPrevious(this);
                    }
                    else
                    {
                        int shift = 7 - level;
                        int index = ((pixel.Red & mask[level]) >> (shift - 2)) |
                                    ((pixel.Green & mask[level]) >> (shift - 1)) |
                                    ((pixel.Blue & mask[level]) >> (shift));
                        OctreeNode child = _children[index];
                        if (null == child)
                        {
                            child = new OctreeNode(level + 1, colorBits, octree);
                            _children[index] = child;
                        }
                        child.AddColor(pixel, colorBits, level + 1, octree);
                    }

                }
                public OctreeNode NextReducible
                {
                    get { return _nextReducible; }
                    set { _nextReducible = value; }
                }
                public OctreeNode[] Children
                {
                    get { return _children; }
                }
                public int Reduce()
                {
                    _red = _green = _blue = 0;
                    int children = 0;
                    for (int index = 0; index < 8; index++)
                    {
                        if (null != _children[index])
                        {
                            _red += _children[index]._red;
                            _green += _children[index]._green;
                            _blue += _children[index]._blue;
                            _pixelCount += _children[index]._pixelCount;
                            ++children;
                            _children[index] = null;
                        }
                    }
                    _leaf = true;
                    return (children - 1);
                }
                public void ConstructPalette(ArrayList palette, ref int paletteIndex)
                {
                    if (_leaf)
                    {
                        _paletteIndex = paletteIndex++;
                        palette.Add(Color.FromArgb(_red / _pixelCount, _green / _pixelCount, _blue / _pixelCount));
                    }
                    else
                    {
                        for (int index = 0; index < 8; index++)
                        {
                            if (null != _children[index])
                                _children[index].ConstructPalette(palette, ref paletteIndex);
                        }
                    }
                }
                public int GetPaletteIndex(Color32 pixel, int level)
                {
                    int paletteIndex = _paletteIndex;

                    if (!_leaf)
                    {
                        int shift = 7 - level;
                        int index = ((pixel.Red & mask[level]) >> (shift - 2)) |
                                    ((pixel.Green & mask[level]) >> (shift - 1)) |
                                    ((pixel.Blue & mask[level]) >> (shift));
                        if (null != _children[index])
                            paletteIndex = _children[index].GetPaletteIndex(pixel, level + 1);
                        else
                            throw new Exception("Didn't expect this!");
                    }
                    return paletteIndex;
                }
                public void Increment(Color32 pixel)
                {
                    _pixelCount++;
                    _red += pixel.Red;
                    _green += pixel.Green;
                    _blue += pixel.Blue;
                }
                private bool _leaf;
                private int _pixelCount;
                private int _red;
                private int _green;
                private int _blue;
                private OctreeNode[] _children;
                private OctreeNode _nextReducible;
                private int _paletteIndex;

            }
        }
    }

    public abstract class Quantizer
    {
        public Quantizer(bool singlePass)
        {
            _singlePass = singlePass;
            _pixelSize = Marshal.SizeOf(typeof(Color32));
        }
        public Bitmap Quantize(Image source)
        {
            int height = source.Height;
            int width = source.Width;
            Rectangle bounds = new Rectangle(0, 0, width, height);
            Bitmap copy = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Bitmap output = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            using (Graphics g = Graphics.FromImage(copy))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                g.DrawImage(source, bounds);
            }
            BitmapData sourceData = null;
            try
            {
                sourceData = copy.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                if (!_singlePass)
                {
                    FirstPass(sourceData, width, height);
                }
                output.Palette = this.GetPalette(output.Palette);
                SecondPass(sourceData, output, width, height, bounds);
            }
            finally
            {
                copy.UnlockBits(sourceData);
            }
            return output;
        }
        protected virtual void FirstPass(BitmapData sourceData, int width, int height)
        {
            IntPtr pSourceRow = sourceData.Scan0;
            for (int row = 0; row < height; row++)
            {
                IntPtr pSourcePixel = pSourceRow;
                for (int col = 0; col < width; col++)
                {
                    InitialQuantizePixel(new Color32(pSourcePixel));
                    pSourcePixel = (IntPtr)(pSourcePixel.ToInt64() + _pixelSize);
                }
                pSourceRow = (IntPtr)(pSourceRow.ToInt64() + sourceData.Stride);
            }
        }
        protected virtual void SecondPass(BitmapData sourceData, Bitmap output, int width, int height, Rectangle bounds)
        {
            BitmapData outputData = null;

            try
            {
                outputData = output.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                IntPtr pSourceRow = sourceData.Scan0;
                IntPtr pSourcePixel = pSourceRow;
                IntPtr pPreviousPixel = pSourcePixel;
                IntPtr pDestinationRow = outputData.Scan0;
                IntPtr pDestinationPixel = pDestinationRow;
                byte pixelValue = QuantizePixel(new Color32(pSourcePixel));
                Marshal.WriteByte(pDestinationPixel, pixelValue);
                for (int row = 0; row < height; row++)
                {
                    pSourcePixel = pSourceRow;
                    pDestinationPixel = pDestinationRow;
                    for (int col = 0; col < width; col++)
                    {
                        if (Marshal.ReadInt32(pPreviousPixel) != Marshal.ReadInt32(pSourcePixel))
                        {
                            pixelValue = QuantizePixel(new Color32(pSourcePixel));
                            pPreviousPixel = pSourcePixel;
                        }
                        Marshal.WriteByte(pDestinationPixel, pixelValue);
                        pSourcePixel = (IntPtr)(pSourcePixel.ToInt64() + _pixelSize);
                        pDestinationPixel = (IntPtr)(pDestinationPixel.ToInt64() + 1);

                    }
                    pSourceRow = (IntPtr)(pSourceRow.ToInt64() + sourceData.Stride);
                    pDestinationRow = (IntPtr)(pDestinationRow.ToInt64() + outputData.Stride);
                }
            }
            finally
            {
                output.UnlockBits(outputData);
            }
        }
        protected virtual void InitialQuantizePixel(Color32 pixel) { }
        protected abstract byte QuantizePixel(Color32 pixel);
        protected abstract ColorPalette GetPalette(ColorPalette original);
        private bool _singlePass;
        private int _pixelSize;
        [StructLayout(LayoutKind.Explicit)]
        public struct Color32
        {
            public Color32(IntPtr pSourcePixel)
            {
                this = (Color32)Marshal.PtrToStructure(pSourcePixel, typeof(Color32));
            }
            [FieldOffset(0)]
            public byte Blue;
            [FieldOffset(1)]
            public byte Green;
            [FieldOffset(2)]
            public byte Red;
            [FieldOffset(3)]
            public byte Alpha;
            [FieldOffset(0)]
            public int ARGB;
            public Color Color
            {
                get { return Color.FromArgb(Alpha, Red, Green, Blue); }
            }
        }
    }
}

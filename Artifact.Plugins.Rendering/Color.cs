using ColorHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColorConverter = ColorHelper.ColorConverter;

namespace Artifact.Plugins.Rendering
{
    public class ColorRGB
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public float UnitR => R / 255f;
        public float UnitG => G / 255f;
        public float UnitB => B / 255f;
        public float UnitA => A / 255f;

        public ColorRGB(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public ColorHSB ToHsb()
        {
            HSL hsl = ColorConverter.RgbToHsl(new RGB(R, G, B));

            return new ColorHSB(hsl.H, hsl.S, hsl.L, A);
        }
    }

    public class ColorHSB
    {
        public int H { get; set; }
        public byte S { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public float UnitH => H / 255f;
        public float UnitS => S / 255f;
        public float UnitB => B / 255f;
        public float UnitA => A / 255f;

        public ColorHSB(int h, byte s, byte b, byte a)
        {
            H = h;
            S = s;
            B = b;
            A = a;
        }

        public ColorRGB ToRgb()
        {
            RGB rgb = ColorConverter.HslToRgb(new HSL(H, S, B));

            return new ColorRGB(rgb.R, rgb.G, rgb.B, A);
        }
    }
}

// ----------------------------------------------------
// GeoHex.cs
//
// License: MIT
// Created by mattak
// Copyright 2016 mattak all rights reserved.
// ----------------------------------------------------

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GeoHex
{
    public static class Pow3
    {
        private static readonly int[] POW3 =
        {
            1, // (long) Math.Pow(3, 0),
            3, // (long) Math.Pow(3, 1),
            9, // (long) Math.Pow(3, 2),
            27, // (long) Math.Pow(3, 3),
            81, // (long) Math.Pow(3, 4),
            243, // (long) Math.Pow(3, 5),
            729, // (long) Math.Pow(3, 6),
            2187, // (long) Math.Pow(3, 7),
            6561, // (long) Math.Pow(3, 8),
            19683, // (long) Math.Pow(3, 9),
            59049, // (long) Math.Pow(3, 10),
            177147, // (long) Math.Pow(3, 11),
            531441, // (long) Math.Pow(3, 12),
            1594323, // (long) Math.Pow(3, 13),
            4782969, // (long) Math.Pow(3, 14),
            14348907, // (long) Math.Pow(3, 15),
            43046721, // (long) Math.Pow(3, 16),
            129140163, // (long) Math.Pow(3, 17),
            387420489, // (long) Math.Pow(3, 18),
        };

        public static int Calc(int times)
        {
            // hard coding for performance
            if (times >= 0 && times <= 18)
            {
                return POW3[times];
            }
            else
            {
                return (int) Math.Pow(3, times);
            }
        }
    }

    public struct XY
    {
        // double is important for `Location2XY()`
        public readonly double x;
        public readonly double y;

        public XY(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct Location
    {
        public readonly double latitude;
        public readonly double longitude;

        public Location(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }

    public struct Zone
    {
        public readonly double latitude;
        public readonly double longitude;
        public readonly long x;
        public readonly long y;
        public readonly string code;

        public Zone(double latitude, double longitude, long x, long y, string code)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.x = x;
            this.y = y;
            this.code = code;
        }

        public Location[] GetHexCoords()
        {
            double h_lat = this.latitude;
            double h_lon = this.longitude;
            XY h_xy = GEOHEX.Location2XY(h_lon, h_lat);
            double h_x = h_xy.x;
            double h_y = h_xy.y;
            double h_deg = 1.7320508075688767; // Math.Tan(Math.PI*(60.0/180.0));
            double h_size = this.GetHexSize();
            double h_top = GEOHEX.XY2Location(h_x, h_y + h_deg*h_size).latitude;
            double h_btm = GEOHEX.XY2Location(h_x, h_y - h_deg*h_size).latitude;

            double h_l = GEOHEX.XY2Location(h_x - 2*h_size, h_y).longitude;
            double h_r = GEOHEX.XY2Location(h_x + 2*h_size, h_y).longitude;
            double h_cl = GEOHEX.XY2Location(h_x - 1*h_size, h_y).longitude;
            double h_cr = GEOHEX.XY2Location(h_x + 1*h_size, h_y).longitude;

            return new Location[]
            {
                new Location(h_lat, h_l),
                new Location(h_top, h_cl),
                new Location(h_top, h_cr),
                new Location(h_lat, h_r),
                new Location(h_btm, h_cr),
                new Location(h_btm, h_cl)
            };
        }

        public int GetLevel()
        {
            return this.code.Length - 2;
        }

        public double GetHexSize()
        {
            return GEOHEX.CalcHexSize(this.GetLevel());
        }

        public override bool Equals(object obj)
        {
            Zone zone = (Zone) obj;
            return this.code.Equals(zone.code);
        }

        public override int GetHashCode()
        {
            return this.code.GetHashCode();
        }
    }

    public static class GEOHEX
    {
        private static readonly double[] HEXSIZE =
        {
            // h_base/Math.Pow(3, level+3)
            20037508.34,
            6679169.446666666,
            2226389.8155555557,
            742129.9385185185,
            247376.6461728395,
            82458.88205761317,
            27486.29401920439,
            9162.098006401464,
            3054.0326688004875,
            1018.0108896001626,
            339.3369632000542,
            113.11232106668473,
            37.70410702222824,
            12.56803567407608,
            4.189345224692027,
            1.3964484082306756,
            0.4654828027435586,
            0.15516093424785285,
            0.05172031141595095,
            0.017240103805316983,
            0.005746701268438995,
            0.001915567089479665,
        };

        private const string h_key = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const double h_base = 20037508.34;
        private const double h_deg = 0.5235987755982988;// Math.PI*(30.0d/180.0d);
        private const double h_k = 0.5773502691896257; // Math.Tan(h_deg);
        private const double h_k_invert = 1.7320508075688774; // Math.Tan(h_deg);
        private const string INC15 = "[15]";
        private const string EXC125 = "[^125]";

        public static Zone GetZoneByLocation(double latitude, double longitude, int level)
        {
            if (latitude < -90.0 || latitude > 90.0)
            {
                throw new ArgumentException("latitude must be from -90 to 90. " + latitude);
            }
            if (longitude < -180.0 || longitude > 180.0)
            {
                throw new ArgumentException("longitude must be from -180 to 180." + longitude);
            }
            if (level < 0 || level > 15)
            {
                throw new ArgumentException("geohex level must be from 0 to 15." + level);
            }

            XY xy = GetXYByLocation(latitude, longitude, level);
            return GetZoneByXY(xy.x, xy.y, level);
        }

        public static Zone GetZoneByCode(string code)
        {
            XY xy = GetXYByCode(code);
            int level = code.Length - 2;
            Zone zone = GetZoneByXY(xy.x, xy.y, level);
            return zone;
        }

        public static XY GetXYByLocation(double latitude, double longitude, int level)
        {
            double h_size = CalcHexSize(level);
            XY z_xy = Location2XY(longitude, latitude);
            double lon_grid = z_xy.x;
            double lat_grid = z_xy.y;
            double unit_x = 6*h_size;
            double unit_y = 6*h_size*h_k;
            double h_pos_x = (lon_grid + lat_grid/h_k)/unit_x;
            double h_pos_y = (lat_grid - h_k*lon_grid)/unit_y;
            int h_x_0 = (int) Math.Floor(h_pos_x);
            int h_y_0 = (int) Math.Floor(h_pos_y);
            double h_x_q = h_pos_x - h_x_0;
            double h_y_q = h_pos_y - h_y_0;
            int h_x = (int) Math.Round(h_pos_x);
            int h_y = (int) Math.Round(h_pos_y);

            if (h_y_q > -h_x_q + 1)
            {
                if ((h_y_q < 2*h_x_q) && (h_y_q > 0.5*h_x_q))
                {
                    h_x = h_x_0 + 1;
                    h_y = h_y_0 + 1;
                }
            }
            else if (h_y_q < -h_x_q + 1)
            {
                if ((h_y_q > (2*h_x_q) - 1) && (h_y_q < (0.5*h_x_q) + 0.5))
                {
                    h_x = h_x_0;
                    h_y = h_y_0;
                }
            }

            XY inner_xy = AdjustXY(h_x, h_y, level);
            return inner_xy;
        }

        private static XY GetXYByCode(string code)
        {
            int level = code.Length - 2;
            int h_x = 0;
            int h_y = 0;

            char[] codeChars = code.ToCharArray();
            string h_dec9 =
                new StringBuilder("").Append(
                    h_key.IndexOf(codeChars[0])*30 + h_key.IndexOf(codeChars[1])
                    )
                    .Append(code.Substring(2))
                    .ToString();
            if (RegMatch(h_dec9[0], INC15) && RegMatch(h_dec9[1], EXC125) &&
                RegMatch(h_dec9[2], EXC125))
            {
                if (h_dec9[0] == '5')
                {
                    h_dec9 = "7" + h_dec9.Substring(1, h_dec9.Length - 1);
                }
                else if (h_dec9[0] == '1')
                {
                    h_dec9 = "3" + h_dec9.Substring(1, h_dec9.Length - 1);
                }
            }

            int d9xlen = h_dec9.Length;
            for (int i = 0; i < level + 3 - d9xlen; i++)
            {
                h_dec9 = "0" + h_dec9;
                d9xlen++;
            }

            StringBuilder h_dec3 = new StringBuilder();
            {
                int dec9i;
                string h_dec0;
                for (int i = 0; i < d9xlen; i++)
                {
                    dec9i = int.Parse(h_dec9[i].ToString());
                    h_dec0 = IntToString(dec9i, new char[] {'0', '1', '2'});
                    if (h_dec0.Length == 1)
                    {
                        h_dec3.Append("0");
                    }
                    h_dec3.Append(h_dec0);
                }
            }

            List<char> h_decx = new List<char>();
            List<char> h_decy = new List<char>();

            for (int i = 0; i < h_dec3.Length/2; i++)
            {
                h_decx.Add(h_dec3[i*2]);
                h_decy.Add(h_dec3[i*2 + 1]);
            }

            {
                int h_pow;
                int level_plus_2 = level + 2;
                for (int i = 0; i <= level_plus_2; i++)
                {
                    h_pow = Pow3.Calc(level_plus_2 - i);

                    if (h_decx[i] == '0')
                    {
                        h_x -= h_pow;
                    }
                    else if (h_decx[i] == '2')
                    {
                        h_x += h_pow;
                    }
                    if (h_decy[i] == '0')
                    {
                        h_y -= h_pow;
                    }
                    else if (h_decy[i] == '2')
                    {
                        h_y += h_pow;
                    }
                }
            }

            XY inner_xy = AdjustXY(h_x, h_y, level);
            return inner_xy;
        }

        public static Zone GetZoneByXY(double x, double y, int level)
        {
            double h_size = CalcHexSize(level);
            int h_x = (int) Math.Round(x);
            int h_y = (int) Math.Round(y);
            double unit_x = 6*h_size;
            double unit_y = 6*h_size*h_k;
            double h_lat = (h_k*h_x*unit_x + h_y*unit_y) * 0.5;
            double h_lon = (h_lat - h_y*unit_y) / h_k;
            Location z_loc = XY2Location(h_lon, h_lat);
            double z_loc_x = z_loc.longitude;
            double z_loc_y = z_loc.latitude;
            double max_hsteps = Pow3.Calc(level + 2);
            double hsteps = Math.Abs(h_x - h_y);

            if (hsteps == max_hsteps)
            {
                if (h_x > h_y)
                {
                    int tmp = h_x;
                    h_x = h_y;
                    h_y = tmp;
                }
                z_loc_x = -180;
            }

            StringBuilder h_code = new StringBuilder();
            List<int> code3_x = new List<int>();
            List<int> code3_y = new List<int>();
            long mod_x = h_x;
            long mod_y = h_y;

            int level_plus_2 = level + 2;
            int h_pow, h_pow_half;
            for (int i = 0; i <= level_plus_2; i++)
            {
                h_pow = Pow3.Calc(level_plus_2 - i);
                h_pow_half = (h_pow>>1) + (h_pow&0x1); // (int)Math.Ceiling((double)h_pow/2);
                if (mod_x >= h_pow_half)
                {
                    code3_x.Add(2);
                    mod_x -= h_pow;
                }
                else if (mod_x <= -h_pow_half)
                {
                    code3_x.Add(0);
                    mod_x += h_pow;
                }
                else
                {
                    code3_x.Add(1);
                }

                if (mod_y >= h_pow_half)
                {
                    code3_y.Add(2);
                    mod_y -= h_pow;
                }
                else if (mod_y <= -h_pow_half)
                {
                    code3_y.Add(0);
                    mod_y += h_pow;
                }
                else
                {
                    code3_y.Add(1);
                }

                if (i == 2 && (z_loc_x >= 0 || z_loc_x == -180))
                {
                    if (code3_x[0] == 2 && code3_y[0] == 1 && code3_x[1] == code3_y[1] &&
                        code3_x[2] == code3_y[2])
                    {
                        code3_x[0] = 1;
                        code3_y[0] = 2;
                    }
                    else if (code3_x[0] == 1 && code3_y[0] == 0 && code3_x[1] == code3_y[1] &&
                             code3_x[2] == code3_y[2])
                    {
                        code3_x[0] = 0;
                        code3_y[0] = 1;
                    }
                }
            }

            for (int i = 0; i < code3_x.Count; i++)
            {
                // TODO: it's not good for memory efficient
                StringBuilder code3 = new StringBuilder();
                StringBuilder code9 = new StringBuilder();
                code3.Append(code3_x[i]).Append(code3_y[i]);
                code9.Append(StringToInt(code3.ToString(), 3));
                h_code.Append(code9);
            }

            string h_2 = h_code.ToString().Substring(3);
            int h_1 = int.Parse(h_code.ToString().Substring(0, 3));
            int h_a1 = h_1/30; // (int) Math.Floor((double) h_1/30);
            int h_a2 = h_1%30;
            StringBuilder h_code_r = new StringBuilder();
            h_code_r.Append(h_key[h_a1]).Append(h_key[h_a2]).Append(h_2.ToString());
            return new Zone(z_loc_y, z_loc_x, h_x, h_y, h_code_r.ToString());
        }

        public static XY AdjustXY(int x, int y, int level)
        {
            int max_hsteps = Pow3.Calc(level + 2);
            int hsteps = Math.Abs(x - y);

            if (x > y && hsteps == max_hsteps)
            {
                int tmp = x;
                x = y;
                y = tmp;
            }
            else if (hsteps > max_hsteps)
            {
                int diff = hsteps - max_hsteps;
                int diff_x = diff/2; // (long) Math.Floor((double) (diff/2));
                int diff_y = diff - diff_x;

                if (x > y)
                {
                    int edge_x = x - diff_x;
                    int edge_y = y + diff_y;
                    int h_xy = edge_x;
                    edge_x = edge_y;
                    edge_y = h_xy;
                    x = edge_x + diff_x;
                    y = edge_y - diff_y;
                }
                else if (y > x)
                {
                    int edge_x = x + diff_x;
                    int edge_y = y - diff_y;
                    int h_xy = edge_x;
                    edge_x = edge_y;
                    edge_y = h_xy;
                    x = edge_x - diff_x;
                    y = edge_y + diff_y;
                }
            }

            return new XY(x, y);
        }

        public static XY Location2XY(double longitude, double latitude)
        {
            // double x = longitude*h_base/180.0;
            // double y = Math.Log(Math.Tan((90.0 + latitude)*Math.PI/360.0))/(Math.PI/180.0);
            // y *= h_base/180.0;
            double x = longitude * 111319.49077777777;
            double y = Math.Log(Math.Tan((90.0 + latitude)*0.008726646259971648)) * 6378136.999112156;

            return new XY(x, y);
        }

        public static Location XY2Location(double x, double y)
        {
            // double lon = (x/h_base)*180.0;
            // double lat = (y/h_base)*180.0;
            // lat = 180/Math.PI*(2.0*Math.Atan(Math.Exp(lat*Math.PI/180.0)) - Math.PI/2.0);
            double lon = x * 0.000008983152842445679;
            double lat = y * 1.5678559431056452e-7;
            lat = 114.59155902616465 * Math.Atan(Math.Exp(lat)) - 90.0;

            return new Location(lat, lon);
        }

        public static double CalcHexSize(int level)
        {
            // return h_base/Pow3.Calc(level + 3);
            return HEXSIZE[level+3];
        }

        private static bool RegMatch(string cs, string pattern)
        {
            return Regex.IsMatch(cs, pattern, RegexOptions.IgnoreCase);
        }

        private static bool RegMatch(char ch, string pattern)
        {
            return RegMatch(ch.ToString(), pattern);
        }

        private static string IntToString(int value, char[] baseChars)
        {
            string result = string.Empty;
            int targetBase = baseChars.Length;

            do
            {
                result = baseChars[value%targetBase] + result;
                value = value/targetBase;
            } while (value > 0);

            return result.ToString();
        }

        private static int StringToInt(string text, int radix)
        {
            int result = 0;

            for (int i = 0; i < text.Length; i++)
            {
                result = radix*result + text[i] - '0';
            }

            return result;
        }
    }
}

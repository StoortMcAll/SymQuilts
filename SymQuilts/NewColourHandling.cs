using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SymQuilts
{
    public partial class Game1
    {
        Color[] PrimaryColors = new Color[27] {Color.Black, new Color(32, 0, 0), new Color(0, 0, 32),
                                                new Color(128, 0, 0),new Color(0, 128, 0), new Color(0, 0, 128),//3
                                                Color.Red, Color.Lime, Color.Blue,//6
                                                new Color(255, 128, 0), new Color(128, 255, 0), new Color(128, 0, 255),//9
                                                new Color(255, 0, 128), new Color(0, 255, 128), new Color(0, 128, 255),//12
                                                new Color(255, 128, 128), new Color(128, 255, 128), new Color(128, 128, 255),//15
                                                Color.Yellow, Color.Magenta, Color.Cyan,//18
                                                new Color(255, 255, 128), new Color(255, 128, 255), new Color(128, 255, 255),//21
                                                Color.White, new Color(255, 240, 255), new Color(240, 255, 255), };//22

        List<Color> SpreadColorList = new List<Color>();

        int SpreadColourCount = 27;
        public static bool ColourChangeInProgress = false;

        private void Initialise_SpreadColourList(int type = 0)
        {
            if (type > -1) SpreadColorList = new List<Color>();

            switch (type)
            {
                case 0://ReSet Spread
                    SpreadColorList = new List<Color>() { PrimaryColors[0], PrimaryColors[6], PrimaryColors[13],
                                                            PrimaryColors[3], PrimaryColors[12], PrimaryColors[17],
                                                            PrimaryColors[16],  PrimaryColors[18],  PrimaryColors[24] };
                    break;
                case 1://BlackRedGreenYellow
                    SpreadColorList = new List<Color>() { PrimaryColors[0], PrimaryColors[6], PrimaryColors[7], PrimaryColors[18] };
                    break;
                case 2://BlackRedYellow
                    SpreadColorList = new List<Color>() { PrimaryColors[0], PrimaryColors[6], PrimaryColors[18] };
                    break;
                case 3:
                    for (int i = 0; i < PrimaryColors.Length; i += 3)
                        SpreadColorList.Add(PrimaryColors[i]);
                    break;
                default:
                    break;
            }

            CreateColourSpread_Sprectrum(SpreadColorList);
        }

        private void NewRandom_ColourSeries2()
        {
            List<Color> SpreadColorList = new List<Color>();

            for (int i = 0; i < randomize.Next(15) + 5; i++)
                SpreadColorList.Add(PrimaryColors[randomize.Next(SpreadColourCount)]);

            CreateColourSpread_Sprectrum(SpreadColorList);
        }

        private void NewRandom_ColourSeries()
        {
            int spreadlistindex = 0;

            List<Color> SpreadColorList = new List<Color>();

            while (spreadlistindex < SpreadColourCount / 3)
            {
                if (randomize.NextDouble() < 0.5)
                    SpreadColorList.Add(PrimaryColors[Select_One_Of_Three(ref spreadlistindex)]);
                else spreadlistindex += 3;
            }

            while (SpreadColorList.Count < 2)
                SpreadColorList.Add(PrimaryColors[Select_One_Of_Three(ref spreadlistindex)]);

            CreateColourSpread_Sprectrum(SpreadColorList);
        }

        private int Select_One_Of_Three(ref int spreadlistindex)
        {
            int result = randomize.Next(SpreadColourCount / 3) * 3 + randomize.Next(3);
            spreadlistindex += 3;

            return result;
        }

        private void CreateColourSpread_Sprectrum(List<Color> spreadColorList)
        {
            int spreadCount = spreadColorList.Count;

            int maxPerPair = _numColors / (spreadCount - 1);
            int counter = 0;

            for (int i = 0; i < spreadCount - 1; i++)
            {
                Colour2Colour_Spectrum(ref counter, maxPerPair, spreadColorList[i], spreadColorList[i + 1]);
            }

            if (counter < _numColors)
            {
                for (int i = counter; i < _numColors; i++)
                    mcolor[i + 1] = mcolor[i];
            }
        }

        private void Colour2Colour_Spectrum(ref int colorCount, int maxPerPair, Color startCol, Color endColor)
        {
            //double rAdd, gAdd, bAdd;
            double red = startCol.R, green = startCol.G, blue = startCol.B;
            double rAdd = endColor.R - red, gAdd = endColor.G - green, bAdd = endColor.B - blue;

            if (rAdd != 0) rAdd /= maxPerPair;
            if (gAdd != 0) gAdd /= maxPerPair;
            if (bAdd != 0) bAdd /= maxPerPair;

            for (int i = 0; i < maxPerPair; i++)
            {
                red += rAdd; green += gAdd; blue += bAdd;
                mcolor[colorCount++] = new Color((int)red, (int)green, (int)blue, 255);
            }
        }
    }
}

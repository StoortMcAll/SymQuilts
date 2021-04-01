using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace SymQuilts
{
    public partial class Game1
    {
        private void SetColors(bool resetCol, bool keepCols = false)
        {
            ColourChangeInProgress = true;

            if (resetCol)
            {
                if (keepCols) Initialise_SpreadColourList(-1);
                else Initialise_SpreadColourList(colorCycle);
               
            }
            else
            {
                bool odd = (colorCycle / 2.0f == colorCycle / 2);
                if (odd) NewRandom_ColourSeries();
                else NewRandom_ColourSeries2();
            }
            colorCycle = (colorCycle + 1) % 4;

            NewFieldSetAvailable = true;
            ColourChangeInProgress = false;
        }

        private void ReverseColors()
        {
            ColourChangeInProgress = true;

            Color[] tmpColor = new Color[_numColors + 1];
            for (int i = 0; i < _numColors + 1; i++)
                tmpColor[i] = mcolor[_numColors - i];
            mcolor = tmpColor;

            ColourChangeInProgress = false;
        }


        private void Draw_Colour_Band(int yPos)
        {
            float spread = (float)_numColors / _screenWid;
            for (int i = 0; i < _screenWid; i++)
            {
                spriteBatch.Draw(_1by1, new Rectangle((int)i, yPos, 1, 16), mcolor[(int)(i * spread)]);
            }
        }

        private void Draw_Colour_Band2(int yPos)
        {
            float _colorSpread = sampler.maxIter;
            if (_colorSpread < 1) _colorSpread = 1;
            else
                _colorSpread = _screenWid / sampler.maxIter;
            if (_colorSpread > _numColors) _colorSpread = _numColors;
            float _widthSpread = 1f / (float)_screenWid;

            for (int i = 0; i < _screenWid; i++)
            {
                spriteBatch.Draw(_1by1, new Rectangle((int)i, yPos, 1, 16), mcolor[(int)(_colorSpread * i * _widthSpread)]);
            }
        }



        private void SaveSingleTestFrame(Texture2D textuteToSave)
        {
            int counter = 0;
            string fileString = "", fileType = "";
            switch (quiltType)
            {
                case QuiltType.Square:
                    fileType = "Square";
                    break;
                case QuiltType.Hexagon:
                    fileType = "Hexagon";
                    break;
                case QuiltType.Icon:
                    fileType = "Icon";
                    break;
                case QuiltType.Icon3:
                    fileType = "Icon3";
                    break;
                default:
                    break;
            }

            do
            {
                fileString = "D:/" + fileType + counter.ToString("000") + ".png";
                counter++;
            } while (File.Exists(fileString));

            FileStream setStream = File.Open(fileString, FileMode.Create);
            //StreamWriter writer = new StreamWriter(setStream);
            textuteToSave.SaveAsPng(setStream, textuteToSave.Width, textuteToSave.Height);
            //writer.Close();
        }

    }
}
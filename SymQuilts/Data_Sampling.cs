using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymQuilts
{
    public partial class Game1
    {
        private void DoSampling(int yPosition)
        {
            switch (sampleState)
            {
                case SampleState.findMax:
                    MaxHitCount = 0;
                    MinHitCount = 9999;
                    for (int i = 0; i < fieldN; i++)
                    {
                        for (int j = 0; j < fieldN; j++)
                        {
                            if ((int)field[j, i] > MaxHitCount)
                                MaxHitCount = (int)field[j, i];
                            if ((int)field[j, i] < MinHitCount)
                                MinHitCount = (int)field[j, i];
                        }
                    }

                    sampler.maxIter = MaxHitCount;
                    sampleState = SampleState.setSpectrum;
                    break;
                case SampleState.setSpectrum:
                    sampler.spectrum = new long[MaxHitCount + 1];

                    for (int i = 0; i < fieldN; i++)
                    {
                        for (int j = 0; j < fieldN; j++)
                        {
                            sampler.spectrum[(long)field[j, i]]++;
                        }
                    }

                    sampleState = SampleState.cutSpurious;
                    break;
                case SampleState.cutSpurious:

                    //  if (MaxHitCount > )
                    sampleState = SampleState.setDisplayData;
                    break;
                case SampleState.setDisplayData:
                    if (sampler.maxIter > 0)
                    {
                        long overRange = 0;
                        float spread = (float)_numColors / _screenWid;
                        float colorBand = _screenWid;
                        if (sampler.maxIter > _numColors)
                        {
                            for (int i = 0; i < (sampler.maxIter - _numColors); i++)
                                overRange += sampler.spectrum[_numColors + i];
                            if (overRange >= _screenHit) overRange = _screenHit - 1;
                        }
                        else
                            colorBand = sampler.maxIter / spread;

                        DrawSamples = new List<DrawSamplingData>();

                        for (int i = 0; i < (colorBand - 1); i++)
                        {
                            long count = 0;

                            for (float hite = (i * spread); hite < ((i + 1) * spread); hite++)
                                count += sampler.spectrum[(int)hite];

                            if (count >= (int)(_screenHit * 0.8f)) count = (int)(_screenHit * 0.8f);

                            DrawSamples.Add(
                                new DrawSamplingData(
                                    new Rectangle(i, yPosition - (int)count, 1, (int)count),
                                    mcolor[(int)(i * spread)]));

                            //spriteBatch.Draw(_1by1, new Rectangle(i, yPosition - (int)count, 1, (int)count), mcolor[(int)(i * spread)]);
                        }
                        if (overRange > 0)
                        {
                            DrawSamples.Add(
                                new DrawSamplingData(
                                    new Rectangle(_screenWid - 1, yPosition - (int)overRange, 1, (int)overRange),
                                    mcolor[_numColors]));
                            //spriteBatch.Draw(_1by1, new Rectangle(_screenWid - 1, yPosition - (int)overRange, 1, (int)overRange), mcolor[_numColors]);
                        }
                    }

                    sampleState = SampleState.ended;
                    break;
                default:
                    break;
            }

           
        }

        private void DisplaySampleData()
        {
            if (DrawSamples == null) return;

            DrawSamplingData sample;
            for (int i = 0; i < DrawSamples.Count; i++)
            {
                sample = DrawSamples[i];

                spriteBatch.Draw(_1by1, sample.Rect, sample.Col);
            }
        }

    }
}

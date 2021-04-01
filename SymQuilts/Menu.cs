using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SymQuilts
{
    public partial class Game1
    {
        bool FirstPass = true;
        int TextHeight;

        private void DoMenuState()
        {
            switch (quiltType)
            {
                case QuiltType.Square:
                    DoMenuKeys(ref Quilt);
                    break;
                case QuiltType.Hexagon:
                    Variables var = Hexgn;
                    DoMenuKeys(ref var);
                    Hexgn = (VectorVars)var;
                    break;
                case QuiltType.Icon:
                    DoMenuKeys(ref Icon1);
                    break;
              
                case QuiltType.Icon3:
                    var = Icon3;
                    DoMenuKeys(ref var);
                    Icon3 = (IconThree)var;
                    break;
                default:
                    break;
            }
        }

        private void DoMenuKeys(ref Variables var)
        {
            bool resetAll = false;
            bool SizeChanged = false;

            if (newKeyboard.IsKeyUp(Keys.Q) && oldKeyboard.IsKeyDown(Keys.Q))
            {
                quiltType++;
                if ((int)quiltType == 3) quiltType++;
                if ((int)quiltType == 5) quiltType = QuiltType.Square;

                resetAll = true;
            }

            if (newKeyboard.IsKeyUp(Keys.F) && oldKeyboard.IsKeyDown(Keys.F))
                RandomiseTillGoodValues = true;

            if (newKeyboard.IsKeyUp(Keys.C) && oldKeyboard.IsKeyDown(Keys.C))
            {
                SetColors(true);
            }
            if (newKeyboard.IsKeyUp(Keys.N) && oldKeyboard.IsKeyDown(Keys.N))
            {
                SetColors(false);
            }
            if (newKeyboard.IsKeyUp(Keys.V) && oldKeyboard.IsKeyDown(Keys.V))
            {
                ReverseColors();
            }

            #region Reset
            if ((newKeyboard.IsKeyUp(Keys.R) && oldKeyboard.IsKeyDown(Keys.R)) || RandomiseTillGoodValues || resetAll)
            {
                //iterates = 0;
                //MaxHitCount = 0;
                
                if (newKeyboard.IsKeyDown(Keys.LeftShift) || newKeyboard.IsKeyDown(Keys.RightShift) || resetAll)
                {
                    switch (quiltType)
                    {
                        case QuiltType.Square:
                            DoInitializeQuilt();
                            break;
                        case QuiltType.Hexagon:
                            DoInitializeHexagon();
                            break;
                        case QuiltType.Icon:
                            DoInitializeIcon1();
                            break;
                      
                        case QuiltType.Icon3:
                            DoInitializeIcon3();
                            break;
                        default:
                            DoExit = true;
                            return;
                    }
                }
                else
                {
                    var.InitialiseCoords(quiltType);
                    if (quiltType == QuiltType.Hexagon) Hexgn.SetVectorThree();
                }

                QuickReset();
            }
            #endregion

            double add = 0.0001d;
            if (newKeyboard.IsKeyDown(Keys.LeftShift) || newKeyboard.IsKeyDown(Keys.RightShift)) add = -0.0001d;

            if (newKeyboard.IsKeyDown(Keys.LeftControl) || newKeyboard.IsKeyDown(Keys.RightControl))
                add *= 10;
            if (newKeyboard.IsKeyDown(Keys.LeftAlt) || newKeyboard.IsKeyDown(Keys.RightAlt))
                add *= 100;

            if (newKeyboard.IsKeyUp(Keys.F) && oldKeyboard.IsKeyDown(Keys.F))
                var.shift = 0.5d - var.shift;

            if (newKeyboard.IsKeyDown(Keys.X)) var.x += add;

            if (newKeyboard.IsKeyDown(Keys.Y)) var.y += add;

            if (newKeyboard.IsKeyDown(Keys.Z)) fieldNscale += add;

            if (newKeyboard.IsKeyDown(Keys.D)) var.delta += add;

            if (newKeyboard.IsKeyDown(Keys.L)) var.lambda += add;

            if (newKeyboard.IsKeyDown(Keys.A)) var.alpha += add;

            if (newKeyboard.IsKeyDown(Keys.B)) var.beta += add;

            if (newKeyboard.IsKeyDown(Keys.G)) var.gamma += add;

            if (newKeyboard.IsKeyDown(Keys.O)) var.omega += add;

            if (newKeyboard.IsKeyDown(Keys.M)) var.ma += add;

            if (newKeyboard.IsKeyUp(Keys.T) && oldKeyboard.IsKeyDown(Keys.T))
                isThreaded = !isThreaded;

            #region Randomize Values
            if ((newKeyboard.IsKeyUp(Keys.S) && oldKeyboard.IsKeyDown(Keys.S)) ||
                        RandomiseTillGoodValues)
            {
                switch (quiltType)
                {
                    case QuiltType.Square:
                        var = DoRandomiseInitializeQuilt();
                        break;
                    case QuiltType.Hexagon:
                        var = DoRandomiseInitializeHexagon();
                        break;
                    case QuiltType.Icon:
                        var = DoRandomInitializeIcon1();
                        break;
                    case QuiltType.Icon3:
                        var = DoRandomizeIcon3();
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region Begin Iterations
            if ((newKeyboard.IsKeyUp(Keys.I) && oldKeyboard.IsKeyDown(Keys.I)) ||
                        RandomiseTillGoodValues)
            {
                programState = ProgramState.iterate;
                isPaused = false;
                maxitsPerFrame = _itersPerFrame;
            }
            #endregion

            #region Width
            if (newKeyboard.IsKeyUp(Keys.W) && oldKeyboard.IsKeyDown(Keys.W))
            {
                if (newKeyboard.IsKeyDown(Keys.LeftShift) || newKeyboard.IsKeyDown(Keys.RightShift))
                {
                    if (Width > 32)
                    {
                        NewWidth /= 2;
                        SizeChanged = true;
                    }
                }
                else
                {
                    if (Width < 1024)
                    {
                        NewWidth *= 2;
                        SizeChanged = true;
                    }
                }
            }
            #endregion
            #region Height
            if (newKeyboard.IsKeyUp(Keys.H) && oldKeyboard.IsKeyDown(Keys.H))
            {
                if (newKeyboard.IsKeyDown(Keys.LeftShift) || newKeyboard.IsKeyDown(Keys.RightShift))
                {
                    if (Height > 32)
                    {
                        NewHeight /= 2;
                        SizeChanged = true;
                    }
                }
                else
                {
                    if (Height < 1024)
                    {
                        NewHeight *= 2;
                        SizeChanged = true;
                    }
                }
            }

            if (SizeChanged)
            {
                while (iterateGenerateFrame) { }

                Width = NewWidth; Height = NewHeight;
                freshFrame[0] = new Texture2D(GraphicsDevice, Width, Height);
                freshFrame[1] = new Texture2D(GraphicsDevice, Width, Height);

                //GenerateTexture.Update_NewFieldSet(field);
            }
            #endregion

            #region Goto Sampling
            if (newKeyboard.IsKeyDown(Keys.E))
            {
                RandomiseTillGoodValues = false;
                programState = ProgramState.sampling;
                sampleState = SampleState.findMax;
                isPaused = false;
                _isPauseReleased = true;
            }
            #endregion
        }

        private void MenuText(Variables var, GameTime gameTime)
        {
            int inset = 8;
            TextYPos(2);
            spriteBatch.DrawString(font, "Runtime       : " + (Stopwatch.ElapsedMilliseconds / 1000.0f).ToString(), new Vector2(0, TextYPos()), Color.Azure);
            TextYPos();
            spriteBatch.DrawString(font, "(Q)uilt type  : " + quiltType.ToString(), new Vector2(0, TextYPos()), Color.LightBlue);
            TextYPos();
            spriteBatch.DrawString(font, "(I)terate", new Vector2(0, TextYPos()), Color.LightGoldenrodYellow);
            TextYPos();
            spriteBatch.DrawString(font, "Iterations    : " + iterates.ToString("###,###,###,###,##inset"), new Vector2(inset, TextYPos()), Color.Gray);
            TextYPos();
            TextYPos();
            spriteBatch.DrawString(font, "RandomiseTill Values(F)ound  : " + (RandomiseTillGoodValues ? "True" : "False"), new Vector2(inset, TextYPos()), Color.White);
            TextYPos();

            if (RandomiseTillGoodValues && ValuesFailed)
            {
                spriteBatch.DrawString(font, "RandomiseTill Tries          : " + Randomized_Tries, new Vector2(inset, TextYPos()), Color.Gray);
            }
            else
            {
                TextYPos();
                TextYPos();
                spriteBatch.DrawString(font, "Is(T)hreaded  : " + (isThreaded ? "True" : "False"), new Vector2(inset, TextYPos()), Color.White);
                TextYPos();
                TextYPos();
                spriteBatch.DrawString(font, "(R)eset All ", new Vector2(inset, TextYPos()), Color.LightCoral);
                TextYPos();
                spriteBatch.DrawString(font, "(r)eset Coords ", new Vector2(inset, TextYPos()), Color.LightCoral);
                TextYPos();


                TextYPos();
                spriteBatch.DrawString(font, "Randomi(S)e all ", new Vector2(inset, TextYPos()), Color.LightGoldenrodYellow);
                TextYPos();
                TextYPos();
                spriteBatch.DrawString(font, "(X,Y)         : " + var.x.ToString("0.0000") + " : " + var.y.ToString("0.0000"), new Vector2(inset, TextYPos()), Color.LightGray);
                TextYPos();
                spriteBatch.DrawString(font, "(W)idth  ^2   : " + Width.ToString(), new Vector2(inset, TextYPos()), Color.LightSeaGreen);
                spriteBatch.DrawString(font, "(H)eight ^2   : " + Height.ToString(), new Vector2(inset, TextYPos()), Color.LightSalmon);
                TextYPos();
                spriteBatch.DrawString(font, "(Z)oom        : " + fieldNscale.ToString("0.0000"), new Vector2(inset, TextYPos()), Color.LightSkyBlue);
                TextYPos();
                TextYPos();
                spriteBatch.DrawString(font, "Reset (C)olour Spread", new Vector2(inset, TextYPos()), Color.White);
                spriteBatch.DrawString(font, "Ra(N)dom Colour Spread", new Vector2(inset, TextYPos()), Color.White);
                spriteBatch.DrawString(font, "Re(V)erse Colour Spread", new Vector2(inset, TextYPos()), Color.White);

                TextYPos();
                spriteBatch.DrawString(font, "Goto Sample and Sav(E)", new Vector2(inset, TextYPos()), Color.LightGoldenrodYellow);
                TextYPos();
                spriteBatch.DrawString(font, "(ESC)ape to Desktop", new Vector2(inset, TextYPos()), Color.Crimson);
                TextYPos();
                TextYPos();

                switch (quiltType)
                {
                    case QuiltType.Square:
                        ParametersText(var);
                        break;
                    case QuiltType.Hexagon:
                        ParametersText(var);
                        break;
                    case QuiltType.Icon:
                        ParametersText(var);
                        break;
                    case QuiltType.Icon2:
                        ParametersText(var);
                        break;
                    case QuiltType.Icon3:
                        ParametersText(var);
                        break;
                    default:
                        break;
                }
            }
        }


        private void DoIterKeys()
        {
            if (newKeyboard.IsKeyUp(Keys.C) && oldKeyboard.IsKeyDown(Keys.C))
            {
                SetColors(true);
            }
            if (newKeyboard.IsKeyUp(Keys.N) && oldKeyboard.IsKeyDown(Keys.N))
            {
                SetColors(false);
            }
            if (newKeyboard.IsKeyUp(Keys.V) && oldKeyboard.IsKeyDown(Keys.V))
            {
                ReverseColors();
            }

            if (newKeyboard.IsKeyUp(Keys.F) && oldKeyboard.IsKeyDown(Keys.F))
                RandomiseTillGoodValues = false;

            if (!isPaused)
            {
                if (newKeyboard.IsKeyDown(Keys.P))
                {
                    isPaused = true;
                    _isPauseReleased = false;
                }
            }
            else
            {
                if (_isPauseReleased)
                {
                    if (newKeyboard.IsKeyUp(Keys.P) && oldKeyboard.IsKeyDown(Keys.P))
                    {
                        isPaused = !isPaused;
                        frameCount = 0;
                        oldIterates = iterates;
                    }
                }
                else
                    if (newKeyboard.IsKeyUp(Keys.P)) _isPauseReleased = true;
            }

            if (newKeyboard.IsKeyDown(Keys.M) && (FirstPass == false && ValuesFailed == false))
            {
                Randomized_Tries = 0;
                programState = ProgramState.menu;
            }

            if (newKeyboard.IsKeyDown(Keys.E))
            {
                RandomiseTillGoodValues = false;
                programState = ProgramState.sampling;
                sampleState = SampleState.findMax;
                isPaused = false;
                _isPauseReleased = true;
            }
        }

        private void IterateText()
        {
            int inset = 8;
            TextYPos(1);

            spriteBatch.DrawString(font, "Iterations    : " + iterates.ToString("###,###,###,###,##0"), new Vector2(inset, TextYPos()), Color.GhostWhite);
            TextYPos();
            spriteBatch.DrawString(font, "Min FieldPeak : " + MinHitCount.ToString("###,##0"), new Vector2(inset, TextYPos()), Color.PapayaWhip);
            spriteBatch.DrawString(font, "Max FieldPeak : " + MaxHitCount.ToString("###,##0"), new Vector2(inset, TextYPos()), Color.PapayaWhip);

            TextYPos();
            spriteBatch.DrawString(font, "FPS           : " + frameCount.ToString(), new Vector2(inset, TextYPos()), Color.White);
            spriteBatch.DrawString(font, "Its Per Frame  : " + itsPerFrame.ToString(), new Vector2(inset, TextYPos()), Color.White);
            TextYPos();
            TextYPos();
            spriteBatch.DrawString(font, "(M)enu ", new Vector2(inset, TextYPos()), Color.White);
            TextYPos();
            TextYPos();
            spriteBatch.DrawString(font, "RandomiseTill Values(F)Ound  : " + (RandomiseTillGoodValues ? "True" : "False"), new Vector2(inset, TextYPos()), Color.White);
            TextYPos();
            spriteBatch.DrawString(font, "RandomiseTill Tries          : " + Randomized_Tries, new Vector2(inset, TextYPos()), Color.White);
            TextYPos();
            TextYPos();
           
            spriteBatch.DrawString(font, "(R)eset Run", new Vector2(inset, TextYPos()), Color.White);
            TextYPos();
            spriteBatch.DrawString(font, "Reset (C)olour Spread", new Vector2(inset, TextYPos()), Color.LightBlue);
            spriteBatch.DrawString(font, "Ra(N)dom Colour Spread", new Vector2(inset, TextYPos()), Color.LightBlue);
            spriteBatch.DrawString(font, "Re(V)erse Colour Spread", new Vector2(inset, TextYPos()), Color.LightBlue);
            TextYPos();
            spriteBatch.DrawString(font, "(P)ause", new Vector2(inset, TextYPos()), Color.Red);
            TextYPos();
            spriteBatch.DrawString(font, "(E)nd Run ", new Vector2(inset, TextYPos()), Color.Red);
            TextYPos();
            spriteBatch.DrawString(font, "(ESC)ape to Desktop", new Vector2(inset, TextYPos()), Color.Crimson);
        }


        private void SamplingText()
        {
            int inset = 8;
            TextYPos(5);
            spriteBatch.DrawString(font, "Back to Main (M)enu ", new Vector2(inset, TextYPos()), Color.White);
            TextYPos();
            spriteBatch.DrawString(font, "Reset (C)olour Spread", new Vector2(inset, TextYPos()), Color.Blue);
            spriteBatch.DrawString(font, "Ra(N)dom Colour Spread", new Vector2(inset, TextYPos()), Color.Blue);
            spriteBatch.DrawString(font, "Re(V)erse Colour Spread", new Vector2(inset, TextYPos()), Color.Blue);
            TextYPos();
            spriteBatch.DrawString(font, "Max FieldPeak : " + MaxHitCount.ToString("###,##0"), new Vector2(inset, TextYPos()), Color.PapayaWhip);
            TextYPos();
            spriteBatch.DrawString(font, "Shift  Colour Spread to (Z)ero", new Vector2(inset, TextYPos()), Color.Blue);
            spriteBatch.DrawString(font, "Culll  Colour Spread (T)op 2%", new Vector2(inset, TextYPos()), Color.Blue);
            TextYPos();
            spriteBatch.DrawString(font, "Smooth Quilt - (1, 2, 3, 4)", new Vector2(inset, TextYPos()), Color.Wheat);
            TextYPos();
            spriteBatch.DrawString(font, "(S)ave Quilt", new Vector2(inset, TextYPos()), Color.White);
            TextYPos();
            spriteBatch.DrawString(font, "(W)rite Data", new Vector2(inset, TextYPos()), Color.White);
            spriteBatch.DrawString(font, "(R)ead  Data", new Vector2(inset, TextYPos()), Color.White);
            TextYPos();
            spriteBatch.DrawString(font, "(ESC)ape to Desktop", new Vector2(inset, TextYPos()), Color.Crimson);
            if (isMouseHeld)
            {
                spriteBatch.DrawString(font, sampleX1.ToString(), sampleTex1, Color.White);
                spriteBatch.DrawString(font, sampleX2.ToString(), sampleTex2, Color.White);
                spriteBatch.Draw(_1by1, new Rectangle((int)sampleLin1.X, _screenHit - 32, 1, 16), Color.White);
                spriteBatch.Draw(_1by1, new Rectangle((int)sampleLin2.X, _screenHit - 32, 1, 16), Color.White);
            }
        }

        private void DoSamplingKeys()
        {
            if ((newKeyboard.IsKeyUp(Keys.M) && oldKeyboard.IsKeyDown(Keys.M)) ||
                    (newKeyboard.IsKeyUp(Keys.Space) && oldKeyboard.IsKeyDown(Keys.Space)) ||
                    (newKeyboard.IsKeyUp(Keys.Enter) && oldKeyboard.IsKeyDown(Keys.Enter)))
            {
                isMouseHeld = false;
                programState = ProgramState.menu;
            }

            if (newKeyboard.IsKeyUp(Keys.T) && oldKeyboard.IsKeyDown(Keys.T))
            {
                TrimTopEnd_Field();
            }

            if (newKeyboard.IsKeyUp(Keys.C) && oldKeyboard.IsKeyDown(Keys.C))
            {
                SetColors(true);
            }
            if (newKeyboard.IsKeyUp(Keys.N) && oldKeyboard.IsKeyDown(Keys.N))
            {
                SetColors(false);
            }
            if (newKeyboard.IsKeyUp(Keys.V) && oldKeyboard.IsKeyDown(Keys.V))
            {
                ReverseColors();
            }

            if (newKeyboard.IsKeyUp(Keys.D1) && oldKeyboard.IsKeyDown(Keys.D1))
                SampleSmooth(0);
            if (newKeyboard.IsKeyUp(Keys.D2) && oldKeyboard.IsKeyDown(Keys.D2))
                SampleSmooth(1);
            if (newKeyboard.IsKeyUp(Keys.D3) && oldKeyboard.IsKeyDown(Keys.D3))
                SampleSmooth(2);
            if (newKeyboard.IsKeyUp(Keys.D4) && oldKeyboard.IsKeyDown(Keys.D4))
                SampleSmooth(3);

            if (newKeyboard.IsKeyUp(Keys.S) && oldKeyboard.IsKeyDown(Keys.S))
                SaveSingleTestFrame(freshFrame[_fFdb]);

            if (newKeyboard.IsKeyUp(Keys.W) && oldKeyboard.IsKeyDown(Keys.W))
            {
                SaveSingleData("D:\\TestStream.txt");
            }
            if (newKeyboard.IsKeyUp(Keys.R) && oldKeyboard.IsKeyDown(Keys.R))
            {
                ReadSingleData("D:\\TestStream.txt");
                sampleState = SampleState.findMax;
            }
        }

        private void TrimTopEnd_Field()
        {
            float percentOfMax = _numColors / (float)MaxHitCount;

            for (int y = 0; y < fieldN; y++)
                for (int x = 0; x < fieldN; x++)
                    if ((field[x, y] *= percentOfMax) > MaxHitCount) MaxHitCount = (int)field[x, y];

            sampleState = SampleState.findMax;

            //GenerateTexture.Update_NewFieldSet(field);
        }

        private void ParametersText(Variables var)
        {
            int inset = 8;

            spriteBatch.DrawString(font, "(A)lpha     = " + var.alpha.ToString("0.0000"),
                                                        new Vector2(inset, TextYPos()), Color.Wheat);
            spriteBatch.DrawString(font, "(B)eta      = " + var.beta.ToString("0.0000"),
                                                        new Vector2(inset, TextYPos()), Color.Wheat);

            spriteBatch.DrawString(font, "(G)amma     = " + var.gamma.ToString("0.0000"),
                                                        new Vector2(inset, TextYPos()), Color.Wheat);

            spriteBatch.DrawString(font, "(L)ambda    = " + var.lambda.ToString("0.0000"),
                                                        new Vector2(inset, TextYPos()), Color.Wheat);

            spriteBatch.DrawString(font, "(M)a        = " + var.ma.ToString("0.0000"),
                                                          new Vector2(inset, TextYPos()), Color.Wheat);

            spriteBatch.DrawString(font, "(O)mega     = " + var.omega.ToString("0.0000"),
                                                        new Vector2(inset, TextYPos()), Color.Wheat);




            if (quiltType == QuiltType.Hexagon || quiltType == QuiltType.Icon3)
                spriteBatch.DrawString(font, "(D)elta     = " + var.delta.ToString("0.0000"),
                                                            new Vector2(inset, TextYPos()), Color.Wheat);

            if (quiltType != QuiltType.Icon3)
                spriteBatch.DrawString(font, "shi(F)t by  = " + var.shift.ToString("0.0000"),
                                                            new Vector2(inset, TextYPos()), Color.Wheat);
        }


        private void TextYPos(int newYPos)
        {
            textYPos = newYPos;
        }
        private float TextYPos()
        {
            return textYPos++ * TextHeight;
        }

    }
}

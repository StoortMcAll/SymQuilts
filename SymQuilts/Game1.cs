using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using SplineCurve;
using Debug_Window;
using Windows_Library;
using UserInputLibrary;

namespace SymQuilts
{
    public enum ProgramState { menu, iterate, sampling }
    public enum QuiltType { Square, Hexagon, Icon, Icon2, Icon3 }
    public enum SampleState { findMax, setDisplayData,
        cutSpurious,
        ended,
        setSpectrum
    }
    public enum DataState { Null, Free, Updating, Updated, Active };
    
    public partial class Game1 : Game
    {
        #region Variable Declaration

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        static string MainPath = @"D:\Stoort\";
        static string MeshStore = MainPath + @"Meshes\";
        static string XmlStore = MainPath + @"XmlStore\";
        const string GUIDataPath = @"D:\Documents\Visual Studio 2015\Projects 2015\Model_Animator_Mono\Model_Animator_Mono\bin\Windows\x86\Debug\";
        const string ProjectDataFile = "AS_Project_GUI.data";
        enum FrameGrouping { All, AllMaximise, Loader, Saver, None }
        public enum File_Caller { Project, Lister, Linker }
        public enum File_Caller_Mode { Load, Save, Create };

        static FrameGrouping OpenNewFrameGroup = FrameGrouping.AllMaximise;
        static FrameGrouping CurrentFrameGroup, OldFrameGroup;


        internal struct OpenFrameGroups
        {
            public List<string> Titles;
            internal List<GUI.SetElementValues> ValueSets;
            internal OpenFrameGroups(List<string> titles, List<GUI.SetElementValues> valuesets)
            {
                Titles = titles;
                ValueSets = valuesets;
            }
        }
        List<OpenFrameGroups> FrameGroupsValuesList = new List<OpenFrameGroups>();

        Stopwatch Stopwatch = new Stopwatch();

        
        private static Point WorkingWindowSize, OutputWindowSize;
        private static Vector2 AspectRatio;

       
        private static List<string> ReturnResults;
        //static RenderTargetBinding[] Bindings = new RenderTargetBinding[2];
        //static RenderTarget2D[] RenderBuffers = new RenderTarget2D[2];
        //static int DoubleBuffer = 0;

        bool FocusLost = false;

        List<string> FrameTitleList;

        UserInput Userinput;

        int Timer = 0;

        static bool Quit;


        SpriteFont font;
        Texture2D _1by1;
        public static Texture2D[] freshFrame = new Texture2D[2];
        public static int _fFdb = 0;
        KeyboardState newKeyboard, oldKeyboard;
        static public MouseState mouse;

        ProgramState programState = ProgramState.menu;
        SampleState sampleState = SampleState.findMax;
        QuiltType quiltType = QuiltType.Square;

        public int[,] _smoothing = new int[4, 10] { { 1, 5, 1, 5, 9, 5, 1, 5, 1, 33 }, { 2, 4, 2, 4, 7, 4, 2, 4, 2, 31 }, { 4, 3, 4, 3, 3, 3, 4, 3, 4, 31 }, { 3, 2, 9, 4, 1, 8, 5, 6, 7, 45 } };

        public static List<string> DebugText;
        bool isThreaded = false;
        public const int fieldN = 1024, halfieldN = 512;
        public static float[,] field = new float[fieldN, fieldN];
        public static float[,] NewFieldSet = new float[fieldN, fieldN];
        public double fieldNscale = 1.0;
        public static int MaxHitCount, MinHitCount;

        public const int _itersPerFrame = 10000;
        int itsPerFrame = _itersPerFrame, maxitsPerFrame = _itersPerFrame;

        public static int Width = 256, NewWidth = 256, Height = 256, NewHeight = 256;
        public const int _numColors = 2048;
        private static List<int> ColourScale;
        public const int _screenWid = 1600, _screenHit = 900;

        bool isPaused = false; bool _isPauseReleased = false;
        int midX, midY, textYPos = 0;

        int sampleX1, sampleX2;
        Vector2 sampleLin1, sampleLin2, sampleTex1, sampleTex2;

        public struct DrawSamplingData
        {
            public Rectangle Rect;
            public Color Col;

            public DrawSamplingData(Rectangle rect, Color col)
            {
                Rect = rect;
                Col = col;
            }
        }

        List<DrawSamplingData> DrawSamples;

        Vector2 fontHeight;
        bool isMouseHeld = false, isCancelled = false;
        static Random randomize = new Random();

        int[,] mcount;
        int _icon1NDepth;
        public static Color[] mcolor = new Color[_numColors + 1];
        
        Color[] frameCol;
        int colorCycle;
        
        int frameCount = 60, oldIterates, iterates = 0;
        
        public static bool iterateGenerateFrame = false;

        public static bool RandomiseTillGoodValues = false, NewFieldSetAvailable = false;
        int Randomized_Tries = 0;
        static bool ValuesFailed = false;
        bool DoExit = false;

        public class Sampler
        {
            public int maxIter;
            public int minIter;
            public long[] spectrum = new long[2048];
        }
        public class Variables
        {
            public void InitialiseCoords(QuiltType type)
            {
                switch (type)
                {
                    case QuiltType.Square:
                        x = 0.1; y = 0.334;
                        break;
                    case QuiltType.Hexagon:
                        x = 0.1; y = 0.3;
                        break;
                    case QuiltType.Icon:
                        x = 0.1; y = -0.1;
                        break;
                    case QuiltType.Icon2:
                        x = 0.1; y = 0.003;
                        break;
                    case QuiltType.Icon3:
                        x = 0.001; y = 0.002;
                        break;
                    default:
                        break;
                }
               
                xnew = x; ynew = y;
            }

            public double x, y;
            public double xnew, ynew;
            public double lambda, alpha, beta, gamma, omega, ma;
            public double shift;
            public double delta;
        }

        public class VectorVars : Variables
        {
            public double k11, k12, k21, k22, el11, el12, el21, el22, el31, el32;
            public double em11, em12, em21, em22, em31, em32;
            public double en11, en12, en21, en22, en31, en32;
            public double enh11, enh12, enh21, enh22, enh31, enh32;

            public double a11, a12, a21, a22, a31, a32;
            public double ah11, ah12, ah21, ah22, ah31, ah32;

            public void SetVectors(double sk11 = 1, double sk12 = 0, double sk21 = 0.5,
                                        double sel11 = 1, double sel21 = 0)
            {
                k11 = sk11; k12 = sk12;
                k21 = sk21; k22 = sq3 / 2;

                el11 = sel11; el12 = -1 / sq3;
                el21 = sel21; el22 = 2 / sq3;
                el31 = -(el11 + el21); el32 = -(el12 + el22);

                em11 = 2 * el11 + el21; em12 = 2 * el12 + el22;
                em21 = 2 * el21 + el31; em22 = 2 * el22 + el32;
                em31 = 2 * el31 + el11; em32 = 2 * el32 + el12;

                en11 = 3 * el11 + 2 * el21; en12 = 3 * el12 + 2 * el22;
                en21 = 3 * el21 + 2 * el31; en22 = 3 * el22 + 2 * el32;
                en31 = 3 * el31 + 2 * el11; en32 = 3 * el32 + 2 * el12;

                enh11 = 3 * el11 + el21; enh12 = 3 * el12 + el22;
                enh21 = 3 * el21 + el31; enh22 = 3 * el22 + el32;
                enh31 = 3 * el31 + el11; enh32 = 3 * el32 + el12;
            }

            public void SetVectorThree()
            {
                a11 = beta; a12 = gamma;
                a21 = (-a11 - sq3 * a12) / 2; a22 = (sq3 * a11 - a12) / 2;
                a31 = -a11 - a21; a32 = -a12 - a22;

                ah11 = a11; ah12 = -a12;
                ah21 = (-ah11 - sq3 * ah12) / 2; ah22 = (sq3 * ah11 - ah12) / 2;
                ah31 = -ah11 - ah21; ah32 = -ah12 - ah22;
            }

        }
        public class IconThree : Variables
        {
            public double degree, npdegree, scale;
        }
        public class IconTrig
        {
            public static double[] c;
            public static double[] s;
        }

        Variables Quilt = new Variables();
        VectorVars Hexgn = new VectorVars();
        Variables Icon1 = new Variables();
        IconThree Icon3 = new IconThree();
        Sampler sampler = new Sampler();

        static double p2 = 2 * Math.PI;
        static double sq3 = Math.Sqrt(3);

        public static Action BuildTexture_Multitask, Make_FieldSet_Copy;
 
        #endregion

        #region Style_Time_Testing_Variables
        const int NumberOfStyles = 3;
        const int NumberOfRuns = 5;

        long[,] Test_Run_Times = new long[NumberOfStyles, NumberOfRuns];
        long[] Run_Averages = new long[NumberOfStyles];
        #endregion

        int UPS, FPS;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = _screenWid;
            graphics.PreferredBackBufferHeight = _screenHit;
            graphics.ApplyChanges();

          //  graphics.IsFullScreen = true;

            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void Initialize()
        {
            //Test_Time_Per_Style();

            int x = 33;

            int y = x % 20;

            y = 1357 % 512;

            y = 583 % 499;

            DebugWindow.Initialise(this, graphics, spriteBatch);
            BuildTexture_Multitask = delegate() { GenerateTexture
                .Generate_Texture2D(); };
            Make_FieldSet_Copy = delegate () {
                GenerateTexture.Update_NewFieldSet();
            };

            DoInitializeQuilt();
            DoInitializeHexagon();
            DoInitializeIcon1();
            DoInitializeIcon3();
            SetColors(true);
            mcount = new int[Width, Height];

            freshFrame[0] = new Texture2D(GraphicsDevice, Width, Height);
            freshFrame[1] = new Texture2D(GraphicsDevice, Width, Height);
            freshFrame[0].SetData(new Color[Width * Height]);
            freshFrame[1].SetData(new Color[Width * Height]);

            itsPerFrame = _itersPerFrame;
            base.Initialize();
        }

        protected override void LoadContent()
        {
			font = Content.Load<SpriteFont>("Overlay");

            TextHeight = (int)font.MeasureString("0").Y - 5;


            _1by1 = new Texture2D(GraphicsDevice, 1, 1);
            _1by1.SetData(new Color[1] { Color.White });

            ColourScale = Catmul_Spline.Catmul_Init(graphics, _numColors);

            GenerateTexture.Initialise(_numColors, fieldN);
        }

        protected override void UnloadContent()
        {
        }



        /// <summary>
        /// Main Update
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            DebugText = new List<string>();

            newKeyboard = Keyboard.GetState();
            
            if (newKeyboard.IsKeyDown(Keys.Escape)) DoExit = true;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) DoExit = true;

            mouse = Mouse.GetState();

            if (DoExit)
            {
                Stopwatch.Restart();
                while (iterateGenerateFrame && Stopwatch.ElapsedMilliseconds < 2000) ; this.Exit();
            }
            else
            {
                if (programState == ProgramState.menu)
                {
                    DoMenuState();
                }
                else if (programState == ProgramState.iterate)
                {
                    DoIterState();
                }
                else if (programState == ProgramState.sampling)
                {
                    DoSamplingKeys();
                }

                Update_FPS();
            }

            if (programState != ProgramState.iterate && Stopwatch.IsRunning)
            {
                Stopwatch.Stop();
            }

            //if (Update_Catmul()) ;// GenerateTexture.Update_NewFieldSet(field);

            if (!iterateGenerateFrame) RegenTexture();

            oldKeyboard = newKeyboard;

           

            base.Update(gameTime);
        }




        private void Update_FPS()
        {
            UPS++;
            if (UPS >= 60)
            {
               // if (FPS > 59 && itsPerFrame > maxitsPerFrame) maxitsPerFrame = itsPerFrame;
                UPS = 0;
                
              //  if (FPS < 50 || FPS > 54) itsPerFrame = (int)(maxitsPerFrame * ((float)FPS / 55.0f));

                frameCount = FPS;
                FPS = 0;
                //{
                //    itsPerFrame -= (30 - frameCount) * 10;
                //    if (itsPerFrame < 100) itsPerFrame = 100;
                //    itsPerFrame = 1000;
                //}
                //else if (frameCount > 54) itsPerFrame = 15000;// itsPerFrame += (frameCount - 54) * 25;
            }
        }

        private bool Update_Catmul()
        {
            return Catmul_Spline.Update(mouse, MaxHitCount < _numColors ? _numColors : MaxHitCount);
          
        }

        private void SampleUpdate()
        {
            if (mouse.LeftButton == ButtonState.Pressed && isCancelled == false)
            {
                if (mouse.RightButton == ButtonState.Pressed)
                {
                    isCancelled = true;
                    isMouseHeld = false;
                    IsMouseVisible = true;
                }
                else
                {
                    fontHeight = font.MeasureString("0000");
                    fontHeight.Y -= 3;
                    if (isMouseHeld)
                    {
                        sampleX2 = (int)(mouse.X * _numColors / _screenWid);
                        sampleLin2 = sampleTex2 = new Vector2(mouse.X, _screenHit - fontHeight.Y);
                        if (sampleTex2.X < 0) sampleTex2.X = 0;
                        if (sampleTex2.X > _screenWid - fontHeight.X) sampleTex2.X = _screenWid - fontHeight.X;
                        if (sampleX2 < 0) sampleX2 = -1;
                        if (sampleX2 > _numColors) sampleX2 = _numColors;
                    }
                    else
                    {
                        if (mouse.Y > 850 && mouse.Y < 884 && mouse.X > -1 && mouse.X < _screenWid)
                        {
                            float spread = (float)_numColors / _screenWid;
                            sampleX1 = sampleX2 = (int)(mouse.X * spread);
                            sampleLin1 = sampleTex1 = new Vector2(mouse.X, _screenHit - fontHeight.Y);
                            if (sampleTex1.X > _screenWid - fontHeight.X) sampleTex1.X = _screenWid - fontHeight.X;
                            sampleLin2 = sampleLin1; sampleTex2 = sampleTex1;
                            isMouseHeld = true;
                            IsMouseVisible = false;
                        }
                    }
                }
            }
            else
            {
                if (mouse.LeftButton == ButtonState.Released)
                {
                    if (isMouseHeld && isCancelled == false && sampleX1 != sampleX2)
                    {
                        isMouseHeld = false;
                        IsMouseVisible = true;
                        if (sampleX2 < 0)
                        {
                            for (int i = 0; i < Height; i++)
                            {
                                for (int j = 0; j < Width; j++)
                                {
                                    int temp = mcount[j, i] - sampleX1;
                                    if (temp < 0) temp = 0;
                                    mcount[j, i] = temp;
                                }
                            }
                        }
                        else if (sampleX2 >= _numColors)
                        {
                            sampler.maxIter = MaxHitCount;
                            //FindMaxIteration();
                            //CullOutliers();
                            float spread = (_numColors - 1) / (float)sampleX1;
                            for (int i = 0; i < Height; i++)
                            {
                                for (int j = 0; j < Width; j++)
                                {
                                    float _mc = mcount[j, i];
                                    if (_mc > sampleX1)
                                        mcount[j, i] = _numColors -1;
                                    else
                                        mcount[j, i] = (int)(_mc * spread);
                                }
                            }

                            //FindMaxIteration();
                        }
                        else
                        {

                            if (sampleX1 > sampleX2)
                            {
                                int temp = sampleX1;
                                sampleX1 = sampleX2;
                                sampleX2 = temp;
                            }
                            float spread = (float)_numColors / (sampleX2 - sampleX1);
                            for (int i = 0; i < Height; i++)
                            {
                                for (int j = 0; j < Width; j++)
                                {
                                    float temp = mcount[j, i] - sampleX1;
                                    if (temp < 0) temp = 0;
                                    mcount[j, i] = (int)(temp * spread);
                                }
                            }
                        }
                        
                        sampleState = SampleState.findMax;
                    }
                    else
                    {
                        isCancelled = false;
                        isMouseHeld = false;
                        IsMouseVisible = true;
                    }
                }
            }
        }

        private void SampleSmooth(int _smoothType)
        {
            float[,] tempCol = new float[fieldN, fieldN];
            for (int i = 0; i < fieldN; i++)
            {
                for (int j = 0; j < fieldN; j++)
                {
                    int jl = j - 1; if (jl < 0) jl = fieldN - 1;
                    int jr = j + 1; if (jr == fieldN) jr = 0;
                    int iu = i - 1; if (iu < 0) iu = fieldN - 1;
                    int id = i + 1; if (id == fieldN) id = 0;
                    float p1 = field[jl, iu] * _smoothing[_smoothType, 0];
                    float p2 = field[j, iu] * _smoothing[_smoothType, 1];
                    float p3 = field[jr, iu] * _smoothing[_smoothType, 2];
                    float p4 = field[jl, i] * _smoothing[_smoothType, 3];
                    float p5 = field[j, i] * _smoothing[_smoothType, 4];
                    float p6 = field[jr, i] * _smoothing[_smoothType, 5];
                    float p7 = field[jl, id] * _smoothing[_smoothType, 6];
                    float p8 = field[j, id] * _smoothing[_smoothType, 7];
                    float p9 = field[jr, id] * _smoothing[_smoothType, 8];
                    tempCol[j, i] = (p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9) / _smoothing[_smoothType, 9];
                }
            }

            field = tempCol;
            RegenTexture();
            sampleState = SampleState.findMax;
        }

        private void RegenTexture()
        {
            Task.Factory.StartNew(BuildTexture_Multitask);
        }



        /// <summary>
        /// Draw
        /// NOTE : PlotQuilt uses `Task.Factory` parallel routine and fills Texture framebuffer on completion.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            FPS++;

            if (TValues != null && TValues[0] != null)
            {
                for (int i = 0; i < TValues.Length; i++)
                {
                    DebugWindow.Add("Tval" + TValues[i].qid.ToString(), TValues[i].running);
                }
            }
            DebugWindow.Update();

            if (Update_Catmul()) { }

            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin();

            switch (programState)
            {
                case ProgramState.menu:
                    QuiltOutline();
                    DrawQuilt();
                    DisplaySampleData();
                    switch (quiltType)
	                {
                        case QuiltType.Square:
                            MenuText(Quilt, gameTime);
                            break;
                        case QuiltType.Hexagon:
                            MenuText(Hexgn, gameTime);
                            break;
                        case QuiltType.Icon:
                            MenuText(Icon1, gameTime);
                            break;
                        case QuiltType.Icon3:
                            MenuText(Icon3, gameTime);
                            break;
                        default:
                            break;
	                }
                    Draw_Colour_Band(5);
                    break;
                case ProgramState.iterate:
                    QuiltOutline();
                    DrawQuilt();
                    
                    IterateText();
                    if (isPaused)
                    {
                        
                        Draw_Colour_Band(_screenHit - 32);
                        DisplaySampleData();
                        Paused();
                    }
                    break;
                case ProgramState.sampling:
                    Draw_Colour_Band(_screenHit - 32);
                    DoSampling(_screenHit - 36);
                    DrawQuilt();
                    SamplingText();
                    break;
                default:
                    break;
            }

            Catmul_Spline.Draw(ref spriteBatch);

            Vector2 pos = new Vector2(800, 20);
            foreach (var line in DebugText)
            {
                spriteBatch.DrawString(font, line, pos, Color.White);
                pos.Y += 20;
            }

            DebugWindow.Draw();

            spriteBatch.End();

            base.Draw(gameTime);
        }


        private void Paused()
        {
            if (sampleState == SampleState.ended) sampleState = SampleState.findMax;
            DoSampling(_screenHit - 36);
        }

        private void QuiltOutline()
        {
            midX = _screenWid / 2 - Width / 2;
            midY = _screenHit / 2 - Height / 2;
            spriteBatch.Draw(_1by1, new Rectangle(midX - 1, midY - 1, Width + 2, 1), Color.White);
            spriteBatch.Draw(_1by1, new Rectangle(midX - 1, midY - 1, 1, Height + 2), Color.White);
            spriteBatch.Draw(_1by1, new Rectangle(midX - 1, midY + Height + 1, Width + 2, 1), Color.White);
            spriteBatch.Draw(_1by1, new Rectangle(midX + Width + 1, midY - 1, 1, Height + 2), Color.White);

        }

        private void DrawQuilt()
        {
            if (freshFrame[_fFdb] != null) spriteBatch.Draw(freshFrame[_fFdb], new Vector2(midX, midY), Color.White);
        }





        private void SaveSingleData(string _dataFile)
        {
            if (File.Exists(_dataFile)) File.Delete(_dataFile);

            StreamWriter _dataStreamOut = new StreamWriter(_dataFile);
            //todo write data mcount width not matching Width and Height
           
            for (int y = 0; y < fieldN; y++)
            {
                for (int x = 0; x < fieldN; x++)
                {
                    _dataStreamOut.WriteLine(field[x, y]);
                }
            }
            _dataStreamOut.Close();
        }

        private void ReadSingleData(string _dataFile)
        {
            if (!File.Exists(_dataFile)) return;

            StreamReader _dataStreamIn = new StreamReader("D:\\TestStream.txt");
         
            freshFrame[0] = new Texture2D(GraphicsDevice, Width, Height);
            freshFrame[1] = new Texture2D(GraphicsDevice, Width, Height);
            frameCol = new Color[Width * Height];
            mcount = new int[Width, Height];

            MaxHitCount = 0;

            float maxtemp = 0;
            for (int y = 0; y < fieldN; y++)
            {
                for (int x = 0; x < fieldN; x++)
                {
                    maxtemp = (float)Convert.ToDouble(_dataStreamIn.ReadLine());

                    if ((int)maxtemp > MaxHitCount) MaxHitCount = (int)maxtemp;

                    field[x, y] = maxtemp;
                }
            }
            _dataStreamIn.Close();
        }

    }
}

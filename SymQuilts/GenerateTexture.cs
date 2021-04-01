using Microsoft.Xna.Framework;
using SplineCurve;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SymQuilts
{
    public static class GenerateTexture
    {
        static int ColourRange = 2048;
        static int FieldN = 1024;
        static internal float[,] NewFieldSet = new float[FieldN, FieldN];
        static bool Update_FieldSet_Active;
        static List<int> ColourScale = new List<int>();

        static DataState FieldState;

        public static void Initialise(int colourRange, int fieldN)
        {
            ColourRange = colourRange;
         //   FieldN = fieldN;

           // NewFieldSet = new float[fieldN, fieldN];

            FieldState = DataState.Null;
            ColourScale = Catmul_Spline.Get_NewColourScale();

            Update_NewFieldSet();
        }

        internal static void Update_NewFieldSet(float[,] field = null)
        {
            Update_FieldSet_Active = true;

            switch (FieldState)
            {
                case DataState.Null:
                    FieldState = DataState.Updating;
                    Array.Clear(NewFieldSet, 0, NewFieldSet.Length);
                    //NewFieldSet = new float[FieldN, FieldN];
                    FieldState = DataState.Updated;
                    break;
                case DataState.Free:
                    FieldState = DataState.Updating;
                    break;
                case DataState.Updating:
                    for (int y = 0; y < FieldN; y++)
                        for (int x = 0; x < FieldN; x++)
                            NewFieldSet[x, y] = Game1.field[x, y];
                    FieldState = DataState.Updated;
                    break;
                default:
                    break;
            }

            Update_FieldSet_Active = false;
        }

        public static void Generate_Texture2D()
        {
            Game1.iterateGenerateFrame = true;

            if (FieldState != DataState.Updated)
            {
                //if (FieldState == DataState.Free)
                //    Update_NewFieldSet(Game1.field);
                if (Update_FieldSet_Active == false) Task.Factory.StartNew(Game1.Make_FieldSet_Copy);
                Game1.iterateGenerateFrame = false;
                return;
            }


            FieldState = DataState.Active;

            int maxIterates, count = 0;
            
            int[,] mcount;

            int width = Game1.Width, height = Game1.Height;

            ScaleToTextureSize(out maxIterates, out mcount, width, height);

            if (Catmul_Spline.NewColourScale_Available)
                ColourScale = Catmul_Spline.Get_NewColourScale();

            int mc, loop;
            Color[] frameCol = new Color[width * height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    mc = mcount[j, i];

                    loop = 0;
                    while (mc > ColourScale[loop] && loop < ColourRange) loop++;

                    frameCol[count++] = Game1.mcolor[loop];
                }
            }

            Game1.freshFrame[1 - Game1._fFdb].SetData(frameCol);

            Game1._fFdb = 1 - Game1._fFdb;

            FieldState = DataState.Free;
            Game1.iterateGenerateFrame = false;
        }

        public static void ScaleToTextureSize(out int maxIterates, out int[,] mcount, int width, int height)
        {
            #region chatter
         
            #endregion
            maxIterates = 0;
            mcount = new int[width, height];

            float reduxX = width / (float)FieldN;
            float reduxY = height / (float)FieldN;

            int xpos, ypos;
            int tmp;

            for (int y = 0; y < FieldN; y++)
            {
                ypos = (int)(y * reduxY);

                for (int x = 0; x < FieldN; x++)
                {
                    xpos = (int)(x * reduxX);
                    tmp = (int)(mcount[xpos, ypos] + NewFieldSet[x, y]);
                    if (tmp > maxIterates) maxIterates = tmp;
                    mcount[xpos, ypos] = tmp;
                }
            }

          //  FieldState = DataState.Free;
        }
    }
}

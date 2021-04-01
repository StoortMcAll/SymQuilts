using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SymQuilts
{
    public partial class Game1
    {
        const float ValuesRandomize_Range = 2;
        const float ValuesRandomize_Halved = ValuesRandomize_Range / 2;

        private void DoInitializeQuilt()
        {
            Quilt.x = Quilt.xnew = 0.1d;
            Quilt.y = Quilt.ynew = 0.334d;
            
            Quilt.alpha = 0.2d;
            Quilt.beta = 0.1d;
            Quilt.gamma = -0.9d;
            Quilt.lambda = -0.59d;
            Quilt.ma = 0.05d;
            Quilt.omega = -0.34d;

            Quilt.shift = 0;
        }
        private Variables DoRandomiseInitializeQuilt()
        {
            DoInitializeQuilt();

            if (randomize.NextDouble() < 0.5) Quilt.x = Quilt.xnew = Get_Random_In_Range();
            if (randomize.NextDouble() < 0.5) Quilt.y = Quilt.ynew = Get_Random_In_Range();

            //if (randomize.NextDouble() < 0.5)
                Quilt.lambda = Get_Random_In_Range(2.0);
            //if (randomize.NextDouble() < 0.5)
                Quilt.alpha = Get_Random_In_Range(2.0);
            //if (randomize.NextDouble() < 0.5)
                Quilt.beta = Get_Random_In_Range(2.0);
            //if (randomize.NextDouble() < 0.5)
                Quilt.gamma = Get_Random_In_Range(2.0);
            //if (randomize.NextDouble() < 0.5)
                Quilt.ma = Get_Random_In_Range(2.0);
            //if (randomize.NextDouble() < 0.5)
                Quilt.omega = Get_Random_In_Range(2.0);
            Quilt.shift = randomize.Next(2) == 0 ? 0 : 0.5d;
            
            return Quilt;
        }
        //_square = new SquareValues(0.1, 0.3, -0.1, -0.076, 0.0, -0.59, 0.0, 0.0); ;
        private void DoInitializeHexagon()
        {
            Hexgn.x = Hexgn.xnew = 0.1d;
            Hexgn.y = Hexgn.ynew = 0.3d;

            Hexgn.alpha = -0.1d;
            Hexgn.beta = -0.076d;
            Hexgn.gamma = 0d;
            Hexgn.lambda = -0.59d;
            Hexgn.ma = 0d;
            Hexgn.omega = 0d;

            Hexgn.shift = 0;
            Hexgn.delta = 0.1d;

            Hexgn.SetVectors();
            Hexgn.SetVectorThree();
        }
        private Variables DoRandomiseInitializeHexagon()
        {
            DoInitializeHexagon();

            if (randomize.NextDouble() < 0.5) Hexgn.x = Hexgn.xnew = Get_Random_In_Range();
            if (randomize.NextDouble() < 0.5) Hexgn.y = Hexgn.ynew = Get_Random_In_Range();

            //if (randomize.NextDouble() < 0.5)
                Hexgn.delta = Get_Random_In_Range(2.0);
            //if (randomize.NextDouble() < 0.5)
                Hexgn.beta = Get_Random_In_Range(2.0);
            //if (randomize.NextDouble() < 0.5)
                Hexgn.gamma = Get_Random_In_Range(2.0);
            //if (randomize.NextDouble() < 0.5)
                Hexgn.ma = Get_Random_In_Range(2.0);
            //if (randomize.NextDouble() < 0.5)
                Hexgn.omega = Get_Random_In_Range(2.0);
            //if (randomize.NextDouble() < 0.5)
                Hexgn.shift = Get_Random_In_Range(2.0);
           // if (randomize.NextDouble() < 0.5)
                        Hexgn.SetVectors(
                            (randomize.NextDouble() < 0.5) ? Get_Random_In_Range(0, 1) : 1,
                            (randomize.NextDouble() < 0.5) ? Get_Random_In_Range(0, 1) : 0,
                            (randomize.NextDouble() < 0.5) ? Get_Random_In_Range(0, 1) : 0.5,
                            (randomize.NextDouble() < 0.5) ? Get_Random_In_Range(0, 1) : 1,
                            (randomize.NextDouble() < 0.5) ? Get_Random_In_Range(0, 1) : 0 );

            Hexgn.SetVectorThree();

            return Hexgn;
        }
        
        private void DoInitializeIcon1(int icondepth = 24)
        {
            _icon1NDepth = icondepth;
            IconTrig.c = new double[_icon1NDepth];
            IconTrig.s = new double[_icon1NDepth];

            for (int i = 0; i < _icon1NDepth; i++)
            {
                IconTrig.c[i] = Math.Cos(2 * Math.PI * i / _icon1NDepth);
                IconTrig.s[i] = Math.Sin(2 * Math.PI * i / _icon1NDepth);
            }

            Icon1.x = Icon1.xnew = 0.1d;
            Icon1.y = Icon1.ynew = -0.1d;

			Icon1.alpha = 0.3d;
			Icon1.beta = 0.65d;
			Icon1.gamma = 0.43d;
			Icon1.lambda = 0.4d;
			Icon1.ma = 0.0d;
			Icon1.omega = 0.9d;
		}
        private Variables DoRandomInitializeIcon1()
        {
            double _a1Test, _a2Test, _a3Test;

            int icondepth = (int)((randomize.NextDouble() < 0.5) ? Get_Random_In_Range(62, 3) : 24);
            DoInitializeIcon1(icondepth);
            randomize = new Random();
            do
            {
                if (randomize.NextDouble() < 0.5) Icon1.alpha = Get_Random_In_Range();
                if (randomize.NextDouble() < 0.5) Icon1.beta = Get_Random_In_Range();
                if (randomize.NextDouble() < 0.5) Icon1.gamma = Get_Random_In_Range();
                if (randomize.NextDouble() < 0.5) Icon1.lambda = Get_Random_In_Range();
                if (randomize.NextDouble() < 0.5) Icon1.ma = Get_Random_In_Range();
                if (randomize.NextDouble() < 0.5) Icon1.omega = Get_Random_In_Range();

                _a1Test = Icon1.alpha * Icon1.alpha + Icon1.gamma * Icon1.gamma;
                _a2Test = Icon1.beta * Icon1.beta + Icon1.lambda * Icon1.lambda;
                _a3Test = Icon1.alpha * Icon1.lambda - Icon1.beta * Icon1.gamma;
                Randomized_Tries++;
            } while (_a1Test > 1 || _a2Test > 1 || (_a1Test + _a2Test) > 1 - Math.Pow(_a3Test, 2));
            
            if (randomize.NextDouble() < 0.5) Icon1.x = Icon1.xnew = Get_Random_In_Range();
            if (randomize.NextDouble() < 0.5) Icon1.y = Icon1.ynew = Get_Random_In_Range();

            return Icon1;
        }
        
        private void DoInitializeIcon3()
        {
            Icon3.x = Icon3.xnew = 0.001;
            Icon3.y = Icon3.ynew = 0.002;

            Icon3.alpha = 5;
            Icon3.beta = 1.5;
            Icon3.gamma = 1;
            Icon3.lambda = -2.7;

            Icon3.delta = 0;

            Icon3.ma = 0;
            Icon3.omega = 0;

           

            Icon3.degree = 7;
            Icon3.npdegree = 2;
        }
        private Variables DoRandomizeIcon3()
        {
            DoInitializeIcon3();

            Icon3.y = Get_Random_In_Range();
            Icon3.x = Get_Random_In_Range();
            if (randomize.NextDouble() < 0.5) Icon3.alpha = Get_Random_In_Range(0.0, 5.0);
            if (randomize.NextDouble() < 0.5) Icon3.beta = Get_Random_In_Range(-3.0, 3.0);
            if (randomize.NextDouble() < 0.5) Icon3.gamma = Get_Random_In_Range(-2.0, 2.0);
            if (randomize.NextDouble() < 0.5) Icon3.lambda = Get_Random_In_Range(-3.0, 3.0);

            if (randomize.NextDouble() < 0.5) Icon3.delta = 1;// Get_Random_In_Range(0, 2);
            if (randomize.NextDouble() < 0.5) Icon3.ma = Get_Random_In_Range(-2, 2);
            if (randomize.NextDouble() < 0.5) Icon3.omega = Get_Random_In_Range(-1, 1);
           

            if (randomize.NextDouble() < 0.5) Icon3.degree = Get_Random_In_Range(94, 2);
            if (randomize.NextDouble() > 0.75) Icon3.npdegree = Get_Random_In_Range(30, 2);
            else Icon3.npdegree = 0;

            Randomized_Tries++;

            return Icon3;
        }

        public static float Get_Random_In_Range(int valuesRandomize_Scalar, int minimum)
        {
            return (float)((randomize.NextDouble() * (valuesRandomize_Scalar - minimum)) + minimum);
        }
        public static double Get_Random_In_Range(double valuesRandomize_Scalar)
        {
            return ((randomize.NextDouble() * ValuesRandomize_Range) - ValuesRandomize_Halved) * valuesRandomize_Scalar;
        }
        public static double Get_Random_In_Range(double minval, double maxval)
        {
            return (randomize.NextDouble() * (maxval - minval)) + minval;
        }
        public static double Get_Random_In_Range()
        {
            double val = randomize.NextDouble();
            return (val * ValuesRandomize_Range) - ValuesRandomize_Halved;
        }


        private void QuickReset()
        {   
            midX = _screenWid / 2 - Width / 2;
            midY = _screenHit / 2 - Height / 2;

            FirstPass = true;
            isPaused = false;

            fieldNscale = 1;
            MaxHitCount = 0;
            field = new float[fieldN, fieldN];
            
            iterates = 0; oldIterates = 0;

            Randomized_Tries = 0;
            iterateGenerateFrame = NewFieldSetAvailable = false;
            GenerateTexture.Initialise(_numColors, fieldN);

            itsPerFrame = _itersPerFrame;
            
            frameCount = 0;
        }

    }
}

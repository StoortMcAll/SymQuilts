using Debug_Window;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace SymQuilts
{
    public partial class Game1
    {
        const int threadcount = 8;
        public class ThreadValues
        {
            public bool running;
            public int qid;
            public double qx; 
            public double qy; 

            public ThreadValues(int id, double x, double y)
            {
                running = true;
                qid = id;
                qx = x;
                if (qx >= 1) qx -= 1.0;
                qy = y;
                if (qy >= 1) qy -= 1.0;
            }
        }

        ThreadValues[] TValues = new ThreadValues[threadcount];

        private void DoIterState()
        {
            ValuesFailed = false;

            if (isPaused)
            {
                DoSampling(_screenHit - 36);
            }
            else
            {
                if (!Stopwatch.IsRunning) Stopwatch.Stop();
                Stopwatch.Reset();
                Stopwatch.Start();

                switch (quiltType)
                {
                    case QuiltType.Square:
                        IterateSquare();
                        break;
                    case QuiltType.Hexagon:
                        IterateHexagon();
                        break;
                    case QuiltType.Icon:
                        IterateIcon();
                        break;
                    case QuiltType.Icon3:
                        ValuesFailed = IterateIcon3();
                        break;
                    default:
                        break;
                }

                Stopwatch.Stop();
            }

            if (FirstPass)
            {
                FirstPass = false;

                sampleState = SampleState.findMax;
                DoSampling(36);
                sampleState = SampleState.findMax;

                if (MaxHitCount > 0) ValuesFailed = true;
            }
         
            DoIterKeys();

            if (ValuesFailed)
            {
                Randomized_Tries++;
                programState = ProgramState.menu;
            }
            else
            {
                Task.Factory.StartNew(Make_FieldSet_Copy); 
            }
        }

        private void IterateSquare()
        {
            bool stop = false;
            int itcount = 0;

            if (isThreaded)
            {
                #region Multitask

                if (FirstPass)
                {
                    for (int i = 0; i < threadcount; i++)
                    {
                        TValues[i] = new ThreadValues(i, Quilt.x + i * 0.00127, Quilt.y + i * 0.00172);
                    }
                }

                Parallel.ForEach (TValues, tvalue =>
                {
                    double x = tvalue.qx; double y = tvalue.qy;
                    double xnew, ynew;

                   // for (int thisRun = 0; thisRun < itsPerFrame; thisRun++)
                   while(!stop)
                    {
                        double p2x = p2 * x; double p2y = p2 * y;
                        double sx = Math.Sin(p2x); double sy = Math.Sin(p2y);

                        xnew = (Quilt.lambda + Quilt.alpha * Math.Cos(p2y)) * sx
                               - Quilt.omega * sy
                               + Quilt.beta * Math.Sin(2 * p2x)
                               + Quilt.gamma * Math.Sin(3 * p2x) * Math.Cos(2 * p2y)
                               + Quilt.ma * x
                               + Quilt.shift;

                        ynew = (Quilt.lambda + Quilt.alpha * Math.Cos(p2x)) * sy
                               + Quilt.omega * sx
                               + Quilt.beta * Math.Sin(2 * p2y)
                               + Quilt.gamma * Math.Sin(3 * p2y) * Math.Cos(2 * p2x)
                               + Quilt.ma * y
                               + Quilt.shift;

                        xnew = (xnew - (int)xnew) + 1;
                        xnew -= (int)xnew;
                        ynew = (ynew - (int)ynew) + 1;
                        ynew -= (int)ynew;

                        //tmpHit = (int)(
                        field[(int)(xnew * fieldN), (int)(ynew * fieldN)] += 0.1f;
                        //if (tmpHit > MaxHitCount) MaxHitCount = tmpHit;
                        //if (MinHitCount > tmpHit) MinHitCount = tmpHit;

                        x = xnew; y = ynew;
                        if (Stopwatch.ElapsedMilliseconds > 12) stop = true;
                        itcount++;
                    }
                    tvalue.qx = x; tvalue.qy = y;
                    TValues[tvalue.qid].running = false;
                });
                #endregion
            }
            else
            {
                #region Singletask
                double x = Quilt.x; double y = Quilt.y;
                double xnew, ynew;

                //for (int thisRun = 0; thisRun < itsPerFrame; thisRun++)
                while (!stop)
                {
                    double p2x = p2 * x; double p2y = p2 * y;
                    double sx = Math.Sin(p2x); double sy = Math.Sin(p2y);

                    xnew = (Quilt.lambda + Quilt.alpha * Math.Cos(p2y)) * sx
                           - Quilt.omega * sy
                           + Quilt.beta * Math.Sin(2 * p2x)
                           + Quilt.gamma * Math.Sin(3 * p2x) * Math.Cos(2 * p2y)
                           + Quilt.ma * x
                           + Quilt.shift;

                    ynew = (Quilt.lambda + Quilt.alpha * Math.Cos(p2x)) * sy
                           + Quilt.omega * sx
                           + Quilt.beta * Math.Sin(2 * p2y)
                           + Quilt.gamma * Math.Sin(3 * p2y) * Math.Cos(2 * p2x)
                           + Quilt.ma * y
                           + Quilt.shift;

                    xnew = (xnew - (int)xnew) + 1;
                    xnew -= (int)xnew;
                    ynew = (ynew - (int)ynew) + 1;
                    ynew -= (int)ynew;

                    //tmpHit = (int)(
                    field[(int)(xnew * fieldN), (int)(ynew * fieldN)] += 0.1f;
                    //if (tmpHit > MaxHitCount) MaxHitCount = tmpHit;
                    //if (MinHitCount > tmpHit) MinHitCount = tmpHit;

                    x = xnew; y = ynew;

                    if (Stopwatch.ElapsedMilliseconds > 15) stop = true;
                    itcount++;
                }
                Quilt.x = x; Quilt.y = y;
                #endregion
            }

            itsPerFrame = itcount;
            iterates += itcount;
            //(isThreaded ? (int)itsPerFrame * 8 : (int)itsPerFrame);
        }

        private void IterateHexagon()
        {
            bool stop = false;
            int itcount = 0;

            if (isThreaded)
            {
                #region Threaded
                if (FirstPass)
                {
                    for (int i = 0; i < threadcount; i++)
                    {
                        TValues[i] = new ThreadValues(i, Hexgn.x + i * 0.0127, Hexgn.y + i * 0.0172);
                    }
                }

                Parallel.ForEach(TValues, tvalue =>
                {
                    double bx = 0, by = 0, xnew = tvalue.qx, ynew = tvalue.qy;
                    //for (int thisRun = 0; thisRun < itsPerFrame; thisRun++)
                    while (!stop)
                    {
                        double s11 = Math.Sin(p2 * (Hexgn.el11 * xnew + Hexgn.el12 * ynew));
                        double s12 = Math.Sin(p2 * (Hexgn.el21 * xnew + Hexgn.el22 * ynew));
                        double s13 = Math.Sin(p2 * (Hexgn.el31 * xnew + Hexgn.el32 * ynew));
                        double s21 = Math.Sin(p2 * (Hexgn.em11 * xnew + Hexgn.em12 * ynew));
                        double s22 = Math.Sin(p2 * (Hexgn.em21 * xnew + Hexgn.em22 * ynew));
                        double s23 = Math.Sin(p2 * (Hexgn.em31 * xnew + Hexgn.em32 * ynew));
                        double s31 = Math.Sin(p2 * (Hexgn.en11 * xnew + Hexgn.en12 * ynew));
                        double s32 = Math.Sin(p2 * (Hexgn.en21 * xnew + Hexgn.en22 * ynew));
                        double s33 = Math.Sin(p2 * (Hexgn.en31 * xnew + Hexgn.en32 * ynew));
                        double s3h1 = Math.Sin(p2 * (Hexgn.enh11 * xnew + Hexgn.enh12 * ynew));
                        double s3h2 = Math.Sin(p2 * (Hexgn.enh21 * xnew + Hexgn.enh22 * ynew));
                        double s3h3 = Math.Sin(p2 * (Hexgn.enh31 * xnew + Hexgn.enh32 * ynew));

                        double sx = (Hexgn.el11 * s11 + Hexgn.el21 * s12 + Hexgn.el31 * s13);
                        double sy = (Hexgn.el12 * s11 + Hexgn.el22 * s12 + Hexgn.el32 * s13);
                        xnew = Hexgn.ma * xnew + Hexgn.lambda * sx - Hexgn.omega * sy;
                        ynew = Hexgn.ma * ynew + Hexgn.lambda * sy + Hexgn.omega * sx;
                        xnew = xnew + Hexgn.alpha * (Hexgn.em11 * s21 + Hexgn.em21 * s22 + Hexgn.em31 * s23);
                        ynew = ynew + Hexgn.alpha * (Hexgn.em12 * s21 + Hexgn.em22 * s22 + Hexgn.em32 * s23);
                        xnew = xnew + Hexgn.a11 * s31 + Hexgn.a21 * s32 + Hexgn.a31 * s33;
                        ynew = ynew + Hexgn.a12 * s31 + Hexgn.a22 * s32 + Hexgn.a32 * s33;
                        xnew = xnew + Hexgn.ah11 * s3h1 + Hexgn.ah21 * s3h2 + Hexgn.ah31 * s3h3;

                        by = 2 * ynew / sq3; bx = xnew - by / 2;

                        bx = (bx - (int)bx) + 1;
                        bx -= (int)bx;
                        by = (by - (int)by) + 1;
                        by -= (int)by;

                        xnew = bx * Hexgn.k11 + by * Hexgn.k21; ynew = bx * Hexgn.k12 + by * Hexgn.k22;

                        //tmpHit = (int)(
                        field[(int)(bx * fieldN), (int)(by * fieldN)] += 0.1f;
                        //if (tmpHit > MaxHitCount) MaxHitCount = tmpHit;
                        //if (MinHitCount > tmpHit) MinHitCount = tmpHit;

                        if (Stopwatch.ElapsedMilliseconds > 15) stop = true;
                        itcount++;
                    }

                    tvalue.qx = bx; tvalue.qy = by;
                });
                #endregion
            }
            else
            {
                #region Not-Threaded
                double bx = 0, by = 0, xnew = Hexgn.x, ynew = Hexgn.y;
                //for (int thisRun = 0; thisRun < itsPerFrame; thisRun++)
                while (!stop)
                {
                    double s11 = Math.Sin(p2 * (Hexgn.el11 * xnew + Hexgn.el12 * ynew));
                    double s12 = Math.Sin(p2 * (Hexgn.el21 * xnew + Hexgn.el22 * ynew));
                    double s13 = Math.Sin(p2 * (Hexgn.el31 * xnew + Hexgn.el32 * ynew));
                    double s21 = Math.Sin(p2 * (Hexgn.em11 * xnew + Hexgn.em12 * ynew));
                    double s22 = Math.Sin(p2 * (Hexgn.em21 * xnew + Hexgn.em22 * ynew));
                    double s23 = Math.Sin(p2 * (Hexgn.em31 * xnew + Hexgn.em32 * ynew));
                    double s31 = Math.Sin(p2 * (Hexgn.en11 * xnew + Hexgn.en12 * ynew));
                    double s32 = Math.Sin(p2 * (Hexgn.en21 * xnew + Hexgn.en22 * ynew));
                    double s33 = Math.Sin(p2 * (Hexgn.en31 * xnew + Hexgn.en32 * ynew));
                    double s3h1 = Math.Sin(p2 * (Hexgn.enh11 * xnew + Hexgn.enh12 * ynew));
                    double s3h2 = Math.Sin(p2 * (Hexgn.enh21 * xnew + Hexgn.enh22 * ynew));
                    double s3h3 = Math.Sin(p2 * (Hexgn.enh31 * xnew + Hexgn.enh32 * ynew));

                    double sx = (Hexgn.el11 * s11 + Hexgn.el21 * s12 + Hexgn.el31 * s13);
                    double sy = (Hexgn.el12 * s11 + Hexgn.el22 * s12 + Hexgn.el32 * s13);

                    xnew = Hexgn.ma * xnew + Hexgn.lambda * sx - Hexgn.omega * sy;
                    ynew = Hexgn.ma * ynew + Hexgn.lambda * sy + Hexgn.omega * sx;
                    xnew = xnew + Hexgn.alpha * (Hexgn.em11 * s21 + Hexgn.em21 * s22 + Hexgn.em31 * s23);
                    ynew = ynew + Hexgn.alpha * (Hexgn.em12 * s21 + Hexgn.em22 * s22 + Hexgn.em32 * s23);
                    xnew = xnew + Hexgn.a11 * s31 + Hexgn.a21 * s32 + Hexgn.a31 * s33;
                    ynew = ynew + Hexgn.a12 * s31 + Hexgn.a22 * s32 + Hexgn.a32 * s33;
                    xnew = xnew + Hexgn.ah11 * s3h1 + Hexgn.ah21 * s3h2 + Hexgn.ah31 * s3h3;
                    ynew = ynew + Hexgn.ah12 * s3h1 + Hexgn.ah22 * s3h2 + Hexgn.ah32 * s3h3;

                    by = 2 * ynew / sq3; bx = xnew - by / 2;

                    bx = (bx - (int)bx) + 1;
                    bx -= (int)bx;
                    by = (by - (int)by) + 1;
                    by -= (int)by;

                    xnew = bx * Hexgn.k11 + by * Hexgn.k21;
                    ynew = bx * Hexgn.k12 + by * Hexgn.k22;

                    //tmpHit = (int)(
                    field[(int)(bx * fieldN), (int)(by * fieldN)] += 0.1f;
                    //if (tmpHit > MaxHitCount) MaxHitCount = tmpHit;
                    //if (MinHitCount > tmpHit) MinHitCount = tmpHit;

                    if (Stopwatch.ElapsedMilliseconds > 15) stop = true;
                    itcount++;
                }

                Hexgn.x = bx; Hexgn.y = by;
                #endregion
            }

            itsPerFrame = itcount;
            iterates += itcount;
            //(isThreaded ? (int)itsPerFrame * 8 : (int)itsPerFrame);
        }

        private void IterateIcon()
        {
            int itcount = 0;
            int loopcount;
            if (isThreaded)
            {
                #region Threaded
                if (FirstPass)
                {
                    for (int i = 0; i < threadcount; i++)
                    {
                        TValues[i] = new ThreadValues(i, Icon1.x + i * 0.00127, Icon1.y + i * 0.00172);
                    }
                }
                ParallelLoopResult loopResult = new ParallelLoopResult();
                loopResult = (Parallel.ForEach(TValues, tvalue =>
                {
                    // double _x = tvalue.qx; double _y = tvalue.qy;

                    IconLoop(out loopcount, ref tvalue.qx, ref tvalue.qy);
                    //tvalue.qx = _x; tvalue.qy = _y;
                    itcount += loopcount;

                }));

                #endregion
            }
            else
            {
                #region Not-Threaded
                
                // double _x = Icon1.x; double _y = Icon1.y;
                IconLoop(out loopcount, ref Icon1.x, ref Icon1.y);
                itcount += loopcount;
                // Icon1.x = _x; Icon1.y = _y;
                #endregion
            }

            itsPerFrame = itcount;
            iterates += itcount;//(isThreaded ? (int)itsPerFrame * 8 : (int)itsPerFrame);
        }

        private void IconLoop(out int loopcount, ref double _x, ref double _y)
        {
            int tmpHit = 0;
            bool stop = false;
            loopcount = 0;
            Random trandomize = new Random();
            double _xnew, _ynew, bx, by, _x1, _y1;
            //for (int thisRun = 0; thisRun < itsPerFrame; thisRun++)
            while(!stop)
            {
                _xnew = Icon1.alpha * _x + Icon1.beta * _y + Icon1.ma;
                _ynew = Icon1.gamma * _x + Icon1.lambda * _y + Icon1.omega;

                int serpoint = trandomize.Next(_icon1NDepth);
                _x1 = _xnew; _y1 = _ynew;

                _xnew = IconTrig.c[serpoint] * _x1 - IconTrig.s[serpoint] * _y1;
                _ynew = IconTrig.s[serpoint] * _x1 - IconTrig.c[serpoint] * _y1;

                //if (randomize.Next(2) == 1) _ynew = -_ynew;
                bx = _xnew; by = _ynew;

                bx = (bx - (int)bx) + 1;
                bx -= (int)bx;
                by = (by - (int)by) + 1;
                by -= (int)by;

                //tmpHit = (int)(
                field[(int)(bx * fieldN), (int)(by * fieldN)] += 0.1f;
                //if (tmpHit > MaxHitCount) MaxHitCount = tmpHit;
                //if (MinHitCount > tmpHit) MinHitCount = tmpHit;

                if (Stopwatch.ElapsedMilliseconds > 15) stop = true;
                loopcount++;
                _x = _xnew; _y = _ynew;
            }
        }



        private bool IterateIcon3()
        {
            bool fail = false;
            int oobCount = 0;
            double xpos = Icon3.x; double ypos = Icon3.y;
            double p, zzbar, zz, oldx, oldy;

            double zreal, zimag, za, zb, zn, zc, zd;
            int bx, by;

            int it = 0;
            int tmpHit;

            Complex z = new Complex(Icon3.x, Icon3.y);
            Complex cplxi = new Complex(0, 1);
            Complex conjZ = new Complex(), temp = new Complex();

            while (it++ < itsPerFrame)
            {
                conjZ = Complex.Conjugate(z);

                if (Icon3.delta != 0)
                {
                    temp = conjZ * cplxi;
                    temp = Complex.Pow(temp, Icon3.degree);
                }
                else temp = cplxi;

                z = (Icon3.lambda +
                        Icon3.alpha * z * conjZ +
                        Icon3.beta * Complex.Pow(z, Icon3.npdegree).Real +
                        Icon3.ma * temp) *
                        z + Icon3.gamma * Complex.Pow(conjZ, Icon3.degree - 1);
                //
                oldx = xpos; oldy = ypos;

                zzbar = oldx * oldx + oldy * oldy;
                //
                if (Icon3.delta != 0)
                {
                    zz = Math.Sqrt(zzbar);
                    zc = 1;
                    zd = 0;
                    zreal = oldx / zz;
                    zimag = oldy / zz;

                    for (int j = 0; j < Icon3.npdegree * Icon3.degree; j++)
                    {
                        za = zc * zreal - zd * zimag;
                        zb = zd * zreal + zc * zimag;
                        zc = za;
                        zd = zb;
                    }
                }
                else zc = zz = 0;

                zreal = oldx;
                zimag = oldy;

                for (int i = 0; i < Icon3.degree - 2; i++)
                {
                    za = zreal * oldx - zimag * oldy;
                    zb = zimag * oldx + zreal * oldy;
                    zreal = za;
                    zimag = zb;
                }

                zn = oldx * zreal - oldy * zimag;
                p = Icon3.lambda + Icon3.alpha * zzbar + Icon3.beta * zn + Icon3.ma * zz * zc;

                xpos = p * oldx + Icon3.gamma * zreal - Icon3.omega * oldy;
                ypos = p * oldy - Icon3.gamma * zimag + Icon3.omega * oldx;

                //xpos = (float)z.Real; ypos = (float)z.Imaginary;

                bx = (int)((xpos * fieldNscale + 1) * halfieldN);
                by = (int)((ypos * fieldNscale + 1) * halfieldN);

                if (bx > -1 && bx < fieldN && by > -1 && by < fieldN)
                {
                    //   tmpHit = (int)(
                    field[bx, by] += 0.1f;
                    oobCount = 0;
                    //if (tmpHit > MaxHitCount) MaxHitCount = tmpHit;
                    //if (MinHitCount > tmpHit) MinHitCount = tmpHit;
                }
                else
                {
                    if (double.IsNaN(xpos) || double.IsNaN(ypos))
                    {
                        it = (int)itsPerFrame;
                        fail = true;
                    }
                    else oobCount++;
                }
            }

            iterates += (int)itsPerFrame;

            if (oobCount > itsPerFrame / 2) fail = true;
            //Icon3.x = z.Real; Icon3.y = z.Imaginary;
            Icon3.x = xpos; Icon3.y = ypos;

            return fail;
        }

    }
}

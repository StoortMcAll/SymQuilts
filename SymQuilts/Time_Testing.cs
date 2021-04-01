using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SymQuilts
{
    public partial class Game1
    {

        private void Test_Time_Per_Style()
        {
            for (int s = 0; s < NumberOfStyles; s++)
            {
                for (int n = 0; n < NumberOfRuns; n++)
                {
                    Test_Run_Times[s, n] = Time_Test_Runs.Timer_Comtainer(s);
                    Run_Averages[s] += Test_Run_Times[s, n];
                }
                Run_Averages[s] /= NumberOfRuns;
            }
        }

        internal static class Time_Test_Runs
        {
            static Stopwatch stopWatch;
            internal static long Timer_Comtainer(int style)
            {
                stopWatch = new Stopwatch();
                stopWatch.Reset();
                stopWatch.Start();

                switch (style)
                {
                    case 0:
                        Style_1();
                        break;
                    case 1:
                        Style_2();
                        break;
                    case 2:
                        Style_3();
                        break;
                    default:
                        break;
                }

                stopWatch.Stop();

                return stopWatch.ElapsedMilliseconds;
            }

            private static void Style_1()
            {
                double xnew, ynew;

                for (int thisRun = 0; thisRun < 10000000; thisRun++)
                {
                    xnew = Get_Random_In_Range();
                    ynew = Get_Random_In_Range();

                    xnew = ((xnew = (xnew - (int)xnew) + 1) - (int)xnew);
                    ynew = ((ynew = (ynew - (int)ynew) + 1) - (int)ynew);
                
                }
            }//Debug-Time 679 // Release - 330
            //
            private static void Style_2()
            {
                double xnew, ynew;

                for (int thisRun = 0; thisRun < 10000000; thisRun++)
                {
                    
                    xnew = Get_Random_In_Range();
                    ynew = Get_Random_In_Range();

                    xnew = (xnew - (int)xnew) + 1;
                    xnew -= (int)xnew;
                    ynew = (ynew - (int)ynew) + 1;
                    ynew -= (int)ynew;

                }
            }//Time 679 // Release - 332

            private static void Style_3()
            {
                const int add = 16;
                double xnew, ynew;

                for (int thisRun = 0; thisRun < 10000000; thisRun++)
                {
                    xnew = Get_Random_In_Range();
                    ynew = Get_Random_In_Range();

                    xnew -= (int)xnew;
                    ynew -= (int)ynew;
                }
            }//Time 573 // Release - 223
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static System.Convert;

namespace Banknote
{
    class Basis
    {
        static void Main(string[] args)
        {
            double[][] input = File.ReadAllLines("data_banknote_authentication.txt").Select(s => s.Split(',').Select(p => Double.Parse(p.Replace('.', ','))).ToArray()).ToArray();
            WorkBaknotes f = new WorkBaknotes(input);
        }   
    }
    class WorkBaknotes
    {
        double[][] input;
        double[,] OneParametr;
        double[,] TwoParametr;
        double[,] ThreeParametr;
        double[,] FourParametr;
        double[] AverageDistance = new double[4];
        public WorkBaknotes(double[][] input)
        {
            this.input = input;
            if(!File.Exists("answer.txt"))
            {
                CreatArrayTraining();
                SortParametr(ref OneParametr);
                SortParametr(ref TwoParametr);
                SortParametr(ref ThreeParametr);
                SortParametr(ref FourParametr);
                Check();
            }
        }
        private void CreatArrayTraining()
        {
            int LenInput = input.GetLength(0);
            int NumFalse = 0;
            int NumTrue = 0;
            for (int i = 0; i < LenInput; i++)
            {
                if (input[i][4] == 0)
                    NumFalse += 1;
                else
                    NumTrue += 1;
            }
            NumFalse = ToInt32(NumFalse * 0.7);
            NumTrue = ToInt32(NumTrue * 0.7);
            OneParametr = new double[NumFalse + NumTrue, 2];
            TwoParametr = new double[NumFalse + NumTrue, 2];
            ThreeParametr = new double[NumFalse + NumTrue, 2];
            FourParametr = new double[NumFalse + NumTrue, 2];
            for (int InpStr = 0, ArrStr = 0; NumFalse > 0 || NumTrue > 0; InpStr++)
            {
                if(input[InpStr][4] == 0 && NumFalse > 0)
                {
                    NumFalse--;
                }
                else if (input[InpStr][4] == 1 && NumTrue > 0)
                {
                    NumTrue--;
                }
                else
                {
                    continue;
                }
                CopyString(InpStr, ArrStr);
                ArrStr++;
            }
            void CopyString(int InpStr, int ArrStr)
            {
                OneParametr[ArrStr, 0] = input[InpStr][0];
                TwoParametr[ArrStr, 0] = input[InpStr][1];
                ThreeParametr[ArrStr, 0] = input[InpStr][2];
                FourParametr[ArrStr, 0] = input[InpStr][3];
                OneParametr[ArrStr, 1] = TwoParametr[ArrStr, 1] = ThreeParametr[ArrStr, 1] = FourParametr[ArrStr, 1] = input[InpStr][4];
            }
        }
        private void SortParametr(ref double[,] ArrayParametr)
        {
            double[] temp = new double[2];
            double gap = ArrayParametr.GetLength(0);
            bool NoStop = true;
            while(gap > 1 || NoStop)
            {
                gap /= 1.247330950103979;
                if (gap < 1)
                    gap = 1;
                NoStop = false;
                for(int i = 0; i + gap < ArrayParametr.GetLength(0); i++)
                {
                    int igap = i + (int)gap;
                    if(ArrayParametr[i, 0] > ArrayParametr[igap, 0])
                    {
                        temp[0] = ArrayParametr[i, 0];
                        temp[1] = ArrayParametr[i, 1];
                        ArrayParametr[i, 0] = ArrayParametr[igap, 0];
                        ArrayParametr[i, 1] = ArrayParametr[igap, 1];
                        ArrayParametr[igap, 0] = temp[0];
                        ArrayParametr[igap, 1] = temp[1];
                        NoStop = true;
                    }
                }
            }
        }
        private void CalculationAverageDist(double[,] ArrayParametr, int elem)
        {
            double SumDistance = 0;
            for(int i = 0; i < ArrayParametr.GetLength(0) - 1; i++)
            {
                SumDistance += ArrayParametr[i + 1, 0] - ArrayParametr[i, 0];
            }
            AverageDistance[elem] = SumDistance / (ArrayParametr.GetLength(0) - 1);
        }
        private void ArrayProcessing(ref double[,] ArrayParametr, int elem)
        {
            double[] Distance = new double[2];
            double[] Answer = new double[2];
            for(int i = 1; i < ArrayParametr.GetLength(0)-1; i++)
            {
                for(int j = -1, n = 0; j <= 1; j++)
                {
                    if (j == 0)
                        continue;
                    try
                    {
                        Distance[n] = Math.Abs(Math.Abs(ArrayParametr[i, 0]) - Math.Abs(ArrayParametr[i + j, 0]));
                        Answer[n] = ArrayParametr[i + j, 1];
                    }
                    catch
                    {
                        continue;
                    }
                    n++;
                }
                if(Distance[0] >= AverageDistance[elem] && Distance[1] >= AverageDistance[elem])
                {
                    continue;
                }
                if (Distance[0] > Distance[1])
                    ArrayParametr[i, 1] = Answer[1];
                else
                    ArrayParametr[i, 1] = Answer[0];
            }
        }
        private void Check()
        {
            double TrueAnswer, FalseAnswer;
            TrueAnswer = FalseAnswer = 0;
            for(int i = 0; i < input.GetLength(0); i++)
            {
                int NumTrue = ApplicationMethod(OneParametr, i, 0) + ApplicationMethod(TwoParametr, i, 1) +
                   + ApplicationMethod(ThreeParametr, i, 2) + ApplicationMethod(FourParametr, i, 3);
                if (NumTrue == 4)
                    TrueAnswer++;
                else
                    FalseAnswer++;
            }
            Console.WriteLine(TrueAnswer);
            Console.WriteLine(FalseAnswer);
            Console.WriteLine(TrueAnswer / (TrueAnswer + FalseAnswer));
            int ApplicationMethod(double[,] AnswerCheck, int str, int stb)
            {
                for(int i = 0; i < AnswerCheck.GetLength(0) - 1; i++)
                {
                    if(input[str][stb] <= AnswerCheck[i + 1, 0] && input[str][stb] >= AnswerCheck[i, 0])
                    {
                        int NumFalse, NumTrue;
                        NumFalse = NumTrue = 0;
                        double DistanceFalse, DistanceTrue;
                        DistanceFalse = DistanceTrue = 0;
                        for(int j = 0; j <= 1; j++)
                        {
                            try
                            {
                                double Distance = Math.Abs(Math.Abs(AnswerCheck[i + j, 0]) - Math.Abs(input[str][stb]));
                                if (AnswerCheck[i + j, 1] == 0)
                                {
                                    if (DistanceFalse > Distance || DistanceFalse == 0)
                                        DistanceFalse = Distance;
                                    NumFalse++;
                                }
                                else
                                {
                                    if (DistanceTrue > Distance || DistanceTrue == 0)
                                        DistanceTrue = Distance;
                                    NumTrue++;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        if (NumFalse > NumTrue)
                            return ToInt32(input[str][4] == 0);
                        else if (NumTrue > NumFalse)
                            return ToInt32(input[str][4] == 1);
                        else
                        {
                            if(DistanceFalse > DistanceTrue)
                                return ToInt32(input[str][4] == 1);
                            return ToInt32(input[str][4] == 0);
                        }

                    }
                }
                if (input[str][stb] <= AnswerCheck[0, 0])
                    return ToInt32(AnswerCheck[0, 1] == input[str][4]);
                return ToInt32(AnswerCheck[AnswerCheck.GetLength(0) - 1, 1] == input[str][4]);
            }
        }
        private void Check2()
        {
            double TrueAnswer, FalseAnswer;
            TrueAnswer = FalseAnswer = 0;
            for (int i = 0; i < input.GetLength(0); i++)
            {
                int NumTrue = ApplicationMethod(OneParametr, i, 0) + ApplicationMethod(TwoParametr, i, 1) +
                   +ApplicationMethod(ThreeParametr, i, 2) + ApplicationMethod(FourParametr, i, 3);
                if (NumTrue == 4)
                    TrueAnswer++;
                else
                    FalseAnswer++;
            }
            Console.WriteLine(TrueAnswer);
            Console.WriteLine(FalseAnswer);
            Console.WriteLine(TrueAnswer / (TrueAnswer + FalseAnswer));
            int ApplicationMethod(double[,] AnswerCheck, int str, int stb)
            {
                if (input[str][stb] >= AnswerCheck[AnswerCheck.GetLength(0) - 1, 0])
                    return ToInt32(AnswerCheck[AnswerCheck.GetLength(0) - 1, 1] == input[str][4]);
                else if (input[str][stb] <= AnswerCheck[0, 0])
                    return ToInt32(AnswerCheck[0, 1] == input[str][4]);
                for (int i = 0; i < AnswerCheck.GetLength(0) - 1; i++)
                {
                    if (input[str][stb] <= AnswerCheck[i + 1, 0] && input[str][stb] >= AnswerCheck[i, 0])
                    {
                        if (AnswerCheck[i + 1, 0] - input[str][stb] <= input[str][stb] - AnswerCheck[i, 0])
                            return ToInt32(AnswerCheck[i + 1, 1] == input[str][4]);
                        else
                            return ToInt32(AnswerCheck[i, 1] == input[str][4]);
                    }
                }
                return 0;
            }
        }
    }
}
using System;

namespace GrowthCurves
{
    class Program
    {
        static void Main(string[] args)
        {
            double urban = 0.50;
            int returnPeriod = 2;
            int region = 10;
            int[] dob = new int[] { 1, 2, 5, 20, 25, 30, 50, 100, 250, 500, 1000 };
            foreach (int item in dob)
            {
                Console.WriteLine("return period" + (item.ToString()));
                for (int i = 1; i < 12; i += 1)
                {
                    //Console.WriteLine(i);
                    Console.WriteLine(i.ToString() + "\t" + QFInalForTheEntireUK(138.3, 287.4, i, item, 0.5).ToString());
                    //Console.WriteLine(i.ToString() + "\t" + QUnifiedForGB(5.75, 11.07, 9, i, 0.48).ToString());
                    //Console.WriteLine("next plz");
                }
            }
            //Console.WriteLine(GofTUrbanToRuralUnderFIfty(9,50,0.48));
            Console.WriteLine(KFactorUnderFifty(5.75, 11.07, 9, 50, 0.48));
            Console.WriteLine(GrowthCurveUnderFiftyVariateY(0.48, returnPeriod));
            ////Console.WriteLine(GrowthCUrveUnderFiftyYear(region, returnPeriod, urban));
            ////Console.WriteLine(GrowthOverHundred(10,100));
            //Console.WriteLine(QbelowFifty(5.75, 11.07, 9, 5, 0.48));
            //Console.WriteLine(QaboveHundread(5.75, 11.07, 9, 100, 0.48));
            //Console.WriteLine(Irish(1, 100));
            Console.WriteLine(Linearinterpolation(6.5, 2, 7, 5, 12));

        }
        public static double QFInalForTheEntireUK(double qbarRural, double qbarUrban, int region, int returnPeriod, double urban)
        //THis is the final thing.... Whole of the United Kingdom calculated through and through... God bless the queen
        {
            if (region == 11)
            {
                return Irish(qbarUrban, returnPeriod);
            }
            else if (region >= 1 && region <= 10)
            {
                return QUnifiedForGB(qbarRural, qbarUrban, region, returnPeriod, urban);
            }
            else
            {
                return 0;
            }
        }
        public static double Irish(double qbarUrban, int returnPeriod)

        {
            double year = Convert.ToDouble(returnPeriod);
            if (year == 1)
            {
                double xi = 0.83;
                return xi * qbarUrban;
            }
            else if (year > 1 && year < 501)
            {
                double y = -Math.Log(-Math.Log(1 - 1 / year));
                double power = 0.05 * y;
                double xi = -3.33 + 4.2 * Math.Exp(power);
                return xi * qbarUrban;
            }
            else
            {
                return 0;
            }
        }
        public static double QUnifiedForGB(double qbarRural, double qbarUrban, int region, int returnPeriod, double urban)
        {
            if (returnPeriod <= 50)
            {
                return QbelowFifty(qbarRural, qbarUrban, region, returnPeriod, urban);
            }
            else if (returnPeriod >= 100)
            {
                return QaboveHundread(qbarRural, qbarUrban, region, returnPeriod, urban);
            }
            else if (returnPeriod > 50 && returnPeriod < 100)
            {
                double year = Convert.ToDouble(returnPeriod);
                return Linearinterpolation(year, 50, 100, QbelowFifty(qbarRural, qbarUrban, region, 50, urban), QaboveHundread(qbarRural, qbarUrban, region, 100, urban));
            }
            else
            {
                return 0.0;
            }
        }
        public static double QbelowFifty(double qbarRural, double qbarUrban, int region, int returnPeriod, double urban)
        {
            double G = GofTUrbanToRuralUnderFIfty(region, returnPeriod, urban);
            return GrowthCUrveUnderFiftyYear(region, returnPeriod, urban) * qbarUrban;
        }
        public static double QaboveHundread(double qbarRural, double qbarUrban, int region, int returnPeriod, double urban)
        {
            double urbanGrowthFactor = UrbanGrowthFactorOverHundread(qbarRural, qbarUrban, region, returnPeriod, urban);
            return urbanGrowthFactor * qbarUrban;
        }
        public static double UrbanGrowthFactorOverHundread(double qbarRural, double qbarUrban, int region, int returnPeriod, double urban)
        {
            //step 19
            double gOfT = GofTUrbanToRuralOverFIfty(qbarRural, qbarUrban, region, returnPeriod, urban);
            double ruralGrowthFactor = GrowthOverHundred(region, returnPeriod);
            return gOfT * ruralGrowthFactor;
        }
        public static double GofTUrbanToRuralOverFIfty(double qbarRural, double qbarUrban, int region, int returnPeriod, double urban)
        {
            double rmaf = RMAF(qbarRural, qbarUrban);
            double K = KFactorUnderFifty(qbarRural, qbarUrban, region, 50, urban);
            double underExp = -K * (Math.Log(100.00) - 1.8);
            return rmaf + (1 - rmaf) * Math.Exp(underExp);
        }
        public static double GofTUrbanToRuralUnderFIfty(int region, int returnPeriod, double urban)
        {
            //step 16
            return GrowthCUrveUnderFiftyYear(region, 50, urban) / GrowthCUrveUnderFiftyYear(region, 50, 0.0);
        }
        public static double KFactorUnderFifty(double qbarRural, double qbarUrban, int region, int returnPeriod, double urban)
        {
            //step17
            double rmaf = RMAF(qbarRural, qbarUrban);
            double gt = GofTUrbanToRuralUnderFIfty(region, returnPeriod, urban);
            double underLog = ((1 - rmaf) / (gt - rmaf));
            return 0.48 * Math.Log(underLog);
        }
        public static double RMAF(double qbarRural, double qbarUrban)
        {

            return qbarRural / qbarUrban;
        }
        public static double QUnderFifty(double qbarUrban, int region, int returnPeriod, double urban)
        {
            return qbarUrban * GrowthCUrveUnderFiftyYear(region, returnPeriod, urban);
        }
        public static double GrowthOverHundred(int region, int returnPeriodOverHundre)
        {
            if (region == 1)
            {
                return +-4.61055284585535e-18 * Math.Pow(returnPeriodOverHundre, 6) + 1.999165105050586e-14 * Math.Pow(returnPeriodOverHundre, 5) + -3.7339206545134484e-11 * Math.Pow(returnPeriodOverHundre, 4) + 3.7559228179260725e-08 * Math.Pow(returnPeriodOverHundre, 3) + -2.148858974360152e-05 * Math.Pow(returnPeriodOverHundre, 2) + 0.007618449578745731 * Math.Pow(returnPeriodOverHundre, 1) + 1.899020326079027 * Math.Pow(returnPeriodOverHundre, 0);
            }
            else if (region == 2)
            {
                return +-1.2057397854513545e-17 * Math.Pow(returnPeriodOverHundre, 6) + 4.1357709559806624e-14 * Math.Pow(returnPeriodOverHundre, 5) + -5.882618766001835e-11 * Math.Pow(returnPeriodOverHundre, 4) + 4.6695335236202165e-08 * Math.Pow(returnPeriodOverHundre, 3) + -2.3336168046875465e-05 * Math.Pow(returnPeriodOverHundre, 2) + 0.007993957023441967 * Math.Pow(returnPeriodOverHundre, 1) + 2.022751741956595 * Math.Pow(returnPeriodOverHundre, 0);
            }
            else if (region == 3)
            {
                return +-1.393908923322141e-17 * Math.Pow(returnPeriodOverHundre, 6) + 5.120660603018954e-14 * Math.Pow(returnPeriodOverHundre, 5) + -7.618802657045816e-11 * Math.Pow(returnPeriodOverHundre, 4) + 5.952595700601307e-08 * Math.Pow(returnPeriodOverHundre, 3) + -2.674511396013423e-05 * Math.Pow(returnPeriodOverHundre, 2) + 0.007649578743699909 * Math.Pow(returnPeriodOverHundre, 1) + 1.5300879839112833 * Math.Pow(returnPeriodOverHundre, 0);
            }
            else if (region == 4)
            {
                return +-3.976415741129032e-18 * Math.Pow(returnPeriodOverHundre, 6) + 1.9649892591097084e-14 * Math.Pow(returnPeriodOverHundre, 5) + -4.1662347456506626e-11 * Math.Pow(returnPeriodOverHundre, 4) + 4.6074203574233845e-08 * Math.Pow(returnPeriodOverHundre, 3) + -2.7931153846165104e-05 * Math.Pow(returnPeriodOverHundre, 2) + 0.010220677590385498 * Math.Pow(returnPeriodOverHundre, 1) + 1.7851432880843283 * Math.Pow(returnPeriodOverHundre, 0);
            }
            else if (region == 5)
            {
                return +-1.869006810186129e-17 * Math.Pow(returnPeriodOverHundre, 6) + 7.649898685203432e-14 * Math.Pow(returnPeriodOverHundre, 5) + -1.2878077151622047e-10 * Math.Pow(returnPeriodOverHundre, 4) + 1.132794379695484e-07 * Math.Pow(returnPeriodOverHundre, 3) + -5.566047008550996e-05 * Math.Pow(returnPeriodOverHundre, 2) + 0.016774919824191388 * Math.Pow(returnPeriodOverHundre, 1) + 2.337965057817572 * Math.Pow(returnPeriodOverHundre, 0);
            }
            else if (region == 6)
            {
                return +-1.4271131918210215e-17 * Math.Pow(returnPeriodOverHundre, 6) + 5.878848820032566e-14 * Math.Pow(returnPeriodOverHundre, 5) + -1.0062179106307371e-10 * Math.Pow(returnPeriodOverHundre, 4) + 9.0771497021573e-08 * Math.Pow(returnPeriodOverHundre, 3) + -4.6027279202306814e-05 * Math.Pow(returnPeriodOverHundre, 2) + 0.014390252334815978 * Math.Pow(returnPeriodOverHundre, 1) + 2.1299646268760943 * Math.Pow(returnPeriodOverHundre, 0);
            }
            else if (region == 7)
            {
                return +-1.4271131918210215e-17 * Math.Pow(returnPeriodOverHundre, 6) + 5.878848820032566e-14 * Math.Pow(returnPeriodOverHundre, 5) + -1.0062179106307371e-10 * Math.Pow(returnPeriodOverHundre, 4) + 9.0771497021573e-08 * Math.Pow(returnPeriodOverHundre, 3) + -4.6027279202306814e-05 * Math.Pow(returnPeriodOverHundre, 2) + 0.014390252334815978 * Math.Pow(returnPeriodOverHundre, 1) + 2.1299646268760943 * Math.Pow(returnPeriodOverHundre, 0);
            }
            else if (region == 8)
            {
                return +-1.4764930059071265e-17 * Math.Pow(returnPeriodOverHundre, 6) + 6.343763426125138e-14 * Math.Pow(returnPeriodOverHundre, 5) + -1.095563249388054e-10 * Math.Pow(returnPeriodOverHundre, 4) + 9.596113442122527e-08 * Math.Pow(returnPeriodOverHundre, 3) + -4.532878917382203e-05 * Math.Pow(returnPeriodOverHundre, 2) + 0.012637347456470804 * Math.Pow(returnPeriodOverHundre, 1) + 1.5239280327512084 * Math.Pow(returnPeriodOverHundre, 0);
            }
            else if (region == 9)
            {
                return +2.9636594342485364e-18 * Math.Pow(returnPeriodOverHundre, 6) + -6.939013056663141e-15 * Math.Pow(returnPeriodOverHundre, 5) + 5.266846443720207e-14 * Math.Pow(returnPeriodOverHundre, 4) + 1.1743890183887636e-08 * Math.Pow(returnPeriodOverHundre, 3) + -1.2064757834757069e-05 * Math.Pow(returnPeriodOverHundre, 2) + 0.005716208845620515 * Math.Pow(returnPeriodOverHundre, 1) + 1.717343963226319 * Math.Pow(returnPeriodOverHundre, 0);
            }
            else if (region == 10)
            {
                return +-1.393908923322141e-17 * Math.Pow(returnPeriodOverHundre, 6) + 5.120660603018954e-14 * Math.Pow(returnPeriodOverHundre, 5) + -7.618802657045816e-11 * Math.Pow(returnPeriodOverHundre, 4) + 5.952595700601307e-08 * Math.Pow(returnPeriodOverHundre, 3) + -2.674511396013423e-05 * Math.Pow(returnPeriodOverHundre, 2) + 0.007649578743699909 * Math.Pow(returnPeriodOverHundre, 1) + 1.5300879839112833 * Math.Pow(returnPeriodOverHundre, 0);
            }
            else
                return 0;
        }
        public static double GrowthCUrveUnderFiftyYear(int region, int returnPeriodYear, double urban)
        {
            double variateY = GrowthCurveUnderFiftyVariateY(urban, returnPeriodYear);
            if (region == 1)
            {
                return +-0.0018370370370373013 * Math.Pow(variateY, 6) + 0.02214700854701162 * Math.Pow(variateY, 5) + -0.10088319088320485 * Math.Pow(variateY, 4) + 0.21694094794097898 * Math.Pow(variateY, 3) + -0.1894895622895969 * Math.Pow(variateY, 2) + 0.2924365190365378 * Math.Pow(variateY, 1) + 0.8199564879564841 * Math.Pow(variateY, 0);
            }
            else if (region == 2)
            {
                return +0.0010666666666667988 * Math.Pow(variateY, 6) + -0.011774358974360553 * Math.Pow(variateY, 5) + 0.047179487179494606 * Math.Pow(variateY, 4) + -0.07761771561773252 * Math.Pow(variateY, 3) + 0.07974685314687247 * Math.Pow(variateY, 2) + 0.17282564102563017 * Math.Pow(variateY, 1) + 0.8401072261072283 * Math.Pow(variateY, 0);
            }
            else if (region == 3)
            {
                return +-0.0009481481481481502 * Math.Pow(variateY, 6) + 0.01137777777777775 * Math.Pow(variateY, 5) + -0.05173789173789176 * Math.Pow(variateY, 4) + 0.1104957264957281 * Math.Pow(variateY, 3) + -0.11022305102305699 * Math.Pow(variateY, 2) + 0.3123782439782513 * Math.Pow(variateY, 1) + 0.8400497280497253 * Math.Pow(variateY, 0);
            }
            else if (region == 4)
            {
                return +-0.00029629629629633554 * Math.Pow(variateY, 6) + 0.003042735042735556 * Math.Pow(variateY, 5) + -0.010911680911683517 * Math.Pow(variateY, 4) + 0.01717560217560847 * Math.Pow(variateY, 3) + 0.015893291893284745 * Math.Pow(variateY, 2) + 0.24684459984460325 * Math.Pow(variateY, 1) + 0.8001554001554001 * Math.Pow(variateY, 0);
            }
            else if (region == 5)
            {
                return +0.00023703703703699706 * Math.Pow(variateY, 6) + -0.0034598290598286236 * Math.Pow(variateY, 5) + 0.019088319088317516 * Math.Pow(variateY, 4) + -0.03958197358197171 * Math.Pow(variateY, 3) + 0.08584247604247697 * Math.Pow(variateY, 2) + 0.24632618492618216 * Math.Pow(variateY, 1) + 0.789894327894329 * Math.Pow(variateY, 0);
            }
            else if (region == 6)
            {
                return +-0.0008296296296296794 * Math.Pow(variateY, 6) + 0.009135042735043276 * Math.Pow(variateY, 5) + -0.03680911680911908 * Math.Pow(variateY, 4) + 0.07080963480963942 * Math.Pow(variateY, 3) + -0.020955659155663827 * Math.Pow(variateY, 2) + 0.29763341103341373 * Math.Pow(variateY, 1) + 0.7699502719502714 * Math.Pow(variateY, 0);
            }
            else if (region == 7)
            {
                return +-0.0008296296296296794 * Math.Pow(variateY, 6) + 0.009135042735043276 * Math.Pow(variateY, 5) + -0.03680911680911908 * Math.Pow(variateY, 4) + 0.07080963480963942 * Math.Pow(variateY, 3) + -0.020955659155663827 * Math.Pow(variateY, 2) + 0.29763341103341373 * Math.Pow(variateY, 1) + 0.7699502719502714 * Math.Pow(variateY, 0);
            }
            else if (region == 8)
            {
                return +0.0002370370370371002 * Math.Pow(variateY, 6) + -0.001818803418804157 * Math.Pow(variateY, 5) + 0.003703703703706931 * Math.Pow(variateY, 4) + -0.00014141414142049985 * Math.Pow(variateY, 3) + 0.015143175343180819 * Math.Pow(variateY, 2) + 0.2725639471639457 * Math.Pow(variateY, 1) + 0.779964257964258 * Math.Pow(variateY, 0);
            }
            else if (region == 9)
            {
                return +-0.0008296296296296796 * Math.Pow(variateY, 6) + 0.010365811965812466 * Math.Pow(variateY, 5) + -0.04860398860399059 * Math.Pow(variateY, 4) + 0.10577466977467385 * Math.Pow(variateY, 3) + -0.09213281533282043 * Math.Pow(variateY, 2) + 0.2651788655788698 * Math.Pow(variateY, 1) + 0.8399968919968904 * Math.Pow(variateY, 0);
            }
            else if (region == 10)
            {
                return +-0.0004148148148146525 * Math.Pow(variateY, 6) + 0.0052854700854680844 * Math.Pow(variateY, 5) + -0.025840455840446683 * Math.Pow(variateY, 4) + 0.0604980574980385 * Math.Pow(variateY, 3) + -0.05519228179226466 * Math.Pow(variateY, 2) + 0.23583185703185267 * Math.Pow(variateY, 1) + 0.849951825951825 * Math.Pow(variateY, 0);
            }
            else
            {
                return 0;
            }

        }
        public static double GrowthCurveUnderFiftyVariateY(double urbanRatio, int returnPerdiod)
        {
            if (returnPerdiod == 1)
            {
                return 0;
            }
            if (urbanRatio == 0.00)
            {
                return Math.Round(ZeroResult(returnPerdiod), 2);
            }
            else if (urbanRatio == 0.25)
            {
                return Math.Round(QuarterResult(returnPerdiod), 2);
            }
            else if (urbanRatio == 0.50)
            {
                return Math.Round(HalfResult(returnPerdiod), 2);
            }
            else if (urbanRatio == 0.75)
            {
                return Math.Round(ThreequartersResult(returnPerdiod), 2);
            }
            else if (0.00 < urbanRatio && urbanRatio < 0.25)
            {
                return Math.Round(ZeroResult(returnPerdiod) + urbanRatio / 0.25 * (-ZeroResult(returnPerdiod) + QuarterResult(returnPerdiod)), 2);
            }
            else if (0.25 < urbanRatio && urbanRatio < 0.5)
            {
                return Math.Round(QuarterResult(returnPerdiod) + (urbanRatio - 0.25) / 0.25 * ((-QuarterResult(returnPerdiod) + HalfResult(returnPerdiod))), 2);
            }
            else if (0.5 < urbanRatio && urbanRatio < 0.75)
            {
                return Math.Round(HalfResult(returnPerdiod) + (urbanRatio - 0.5) / 0.25 * ((-HalfResult(returnPerdiod) + ThreequartersResult(returnPerdiod))), 2);
            }
            else
            {
                return 0;
            }
            double ZeroResult(int z)
            {
                double result;
                double x = Convert.ToDouble(z);
                if (x > 25)
                {
                    result = 0.028 * x + 2.5;
                }
                else
                {
                    result = 9.399825550184075e-07 * Math.Pow(x, 5) + -0.0001066203032742477 * Math.Pow(x, 4) + 0.0043380929616195045 * Math.Pow(x, 3) + -0.08195265029519803 * Math.Pow(x, 2) + 0.8018243927802975 * Math.Pow(x, 1) + -0.9388670826620519 * Math.Pow(x, 0);
                }
                return result;
            }
            double QuarterResult(int z)
            {
                double result;
                double x = Convert.ToDouble(z);
                if (x > 25)
                {
                    result = 0.0168 * x + 2.51;

                }
                else
                {
                    result = 8.667538915723792e-07 * Math.Pow(x, 5) - 9.86984836285179e-05 * Math.Pow(x, 4) + 0.004036701858560034 * Math.Pow(x, 3) - 0.07665008185719663 * Math.Pow(x, 2) + 0.7415947027642078 * Math.Pow(x, 1) - 0.687331253354478 * Math.Pow(x, 0);
                }
                return result;
            }
            double HalfResult(int z)
            {
                double result;
                double x = Convert.ToDouble(z);
                if (x > 25)
                {
                    result = 0.0132 * x + 2.34;

                }
                else
                {
                    result = 9.519524959738484e-07 * Math.Pow(x, 5) + -0.00010732218196453143 * Math.Pow(x, 4) + 0.004318443739934022 * Math.Pow(x, 3) + -0.07974936795488624 * Math.Pow(x, 2) + 0.7272978764088518 * Math.Pow(x, 1) + -0.5184591384859801 * Math.Pow(x, 0);
                }
                return result;
            }
            double ThreequartersResult(int z)
            {
                double result;
                double x = Convert.ToDouble(z);
                if (x > 25)
                {
                    result = 0.0096 * x + 2.19;
                    ;
                }
                else
                {
                    result = 1.038962694578209e-06 * Math.Pow(x, 5) + -0.00011621922973693708 * Math.Pow(x, 4) + 0.004612284051259677 * Math.Pow(x, 3) + -0.08300135131505533 * Math.Pow(x, 2) + 0.7136517143046996 * Math.Pow(x, 1) + -0.35037003488958696 * Math.Pow(x, 0);
                }
                return result;
            }
            //[ 8.66753892e-07 - 9.86984836e-05  4.03670186e-03 - 7.66500819e-02  7.41594703e-01 - 6.87331253e-01]

        }
        public static double Linearinterpolation(double ourValueX, double x1, double x2, double y1, double y2)
        {
            double dist = (ourValueX - x1) / (x2 - x1);
            return y1 + (y2 - y1) * dist;
        }
    }
}

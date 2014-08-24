//MIT  2014, WinterDev 
//adapt from Wikipedia

namespace ImageTransformation
{


    class LinearEq3
    {
        public readonly double coeff_X;
        public readonly double coeff_Y;
        public readonly double coeff_Z;
        public readonly double sumResult;
        public LinearEq3(double coeff_X, double coeff_Y, double coeff_Z, double sumResult)
        {
            this.coeff_X = coeff_X;
            this.coeff_Y = coeff_Y;
            this.coeff_Z = coeff_Z;
            this.sumResult = sumResult;
        }
        public LinearEq3 Multiply(double n)
        {
            return new LinearEq3(n * coeff_X, n * coeff_Y, n * coeff_Z, n * sumResult);
        }

        public LinearEq3 Sub(LinearEq3 another)
        {
            return new LinearEq3(
                this.coeff_X - another.coeff_X,
                this.coeff_Y - another.coeff_Y,
                this.coeff_Z - another.coeff_Z,
                this.sumResult - another.sumResult);
        }
        public LinearEq3 Add(LinearEq3 another)
        {
            return new LinearEq3(
                this.coeff_X + another.coeff_X,
                this.coeff_Y + another.coeff_Y,
                this.coeff_Z + another.coeff_Z,
                this.sumResult + another.sumResult);
        }
#if DEBUG
        public override string ToString()
        {
            return coeff_X + "x + " + coeff_Y + "y + " + coeff_Z + " z = " + sumResult;
        }
#endif


        public static void Resolve(LinearEq3 line1,
            LinearEq3 line2,
            LinearEq3 line3,
            out double resolve_x,
            out double resolve_y,
            out double resolve_z)
        {
            //from wikipedia: Linear_algebra
            //Gaussian elimination 
            //3 points
            //eg. 2x+y-z  =   8 // L1
            //   -3x-y+2z = -11 // L2
            //   -2x+y+2z =  -3 // L3

            //todo : find x,y,z

            //solve steps:
            //step 1: eliminate x from L2,L3
            //step 2: eliminate y from L3 -> found z
            //resubstitue back


            //step 1, eliminate x from L2 & L3
            double xratio = line2.coeff_X / line1.coeff_X;
            var line1_prepare_for_line2SUB = line1.Multiply(xratio);
            var line2_remove_x = line2.Sub(line1_prepare_for_line2SUB);

            xratio = line3.coeff_X / line1.coeff_X;
            var line1_prepapre_for_line3SUB = line1.Multiply(xratio);
            var line3_remove_x = line3.Sub(line1_prepapre_for_line3SUB);

            //----------------------------------------
            //remove y from line2_remove_x and line3_remove_x
            double yratio = line3_remove_x.coeff_Y / line2_remove_x.coeff_Y;
            var line2_prepare = line2_remove_x.Multiply(yratio);
            var line3_remove_xy = line3_remove_x.Sub(line2_prepare);

            //-------
            //found z =>  
            var simpleZ = line3_remove_xy.sumResult / line3_remove_xy.coeff_Z;
            //found y
            var simpleY = (line2_prepare.sumResult - (simpleZ * line2_prepare.coeff_Z)) / line2_prepare.coeff_Y;
            //found x
            var simpleX = (line1.sumResult - ((simpleZ * line1.coeff_Z) + (simpleY * line1.coeff_Y))) / line1.coeff_X;

            resolve_x = simpleX;
            resolve_y = simpleY;
            resolve_z = simpleZ;
        }

    }

    class LinearEq4
    {
        public readonly double c0;
        public readonly double c1;
        public readonly double c2;
        public readonly double c3;

        public readonly double sum;

        public LinearEq4(double c0, double c1, double c2, double c3, double sum)
        {
            this.c0 = c0;
            this.c1 = c1;
            this.c2 = c2;
            this.c3 = c3;
            this.sum = sum;
        }
        public LinearEq4 Multiply(double n)
        {
            return new LinearEq4(n * c0, n * c1, n * c2, n * c3, n * sum);
        }

        public LinearEq4 Sub(LinearEq4 another)
        {
            return new LinearEq4(
                this.c0 - another.c0,
                this.c1 - another.c1,
                this.c2 - another.c2,
                this.c3 - another.c3,
                this.sum - another.sum
                );
        }
        public LinearEq4 Add(LinearEq4 another)
        {
            return new LinearEq4(
                this.c0 + another.c0,
                this.c1 + another.c1,
                this.c2 + another.c2,
                this.c3 + another.c3,
                this.sum - another.sum
                );
        }
#if DEBUG
        public override string ToString()
        {
            return c0 + "c0 + " + c1 + "c1 + " + c2 + "c2 +" + c3 + "c3 =" + this.sum;
        }
#endif

        public static void Resolve(LinearEq4 line0,
            LinearEq4 line1,
            LinearEq4 line2,
            LinearEq4 line3,
            out double resolve_c0,
            out double resolve_c1,
            out double resolve_c2,
            out double resolve_c3)
        {
            //from wikipedia: Linear_algebra
            //Gaussian elimination 
            //step 1
            //primary line=line0
            //eliminate c0 from line1,line2, line3
            var c10ratio = line1.c0 / line0.c0;
            var line1_remove_c0 = line1.Sub(line0.Multiply(c10ratio));
            //---------------------------------------------
            var c20ratio = line2.c0 / line0.c0;
            var line2_remove_c0 = line2.Sub(line0.Multiply(c20ratio));
            //---------------------------------------------
            var c30ratio = line3.c0 / line0.c0;
            var line3_remove_c0 = line3.Sub(line0.Multiply(c30ratio));
            //---------------------------------------------
            //step2:
            //primary line = line1
            //eliminate c1 from line2 , line3
            var c21ratio = line2_remove_c0.c1 / line1_remove_c0.c1;
            var line2_remove_c1 = line2_remove_c0.Sub(line1_remove_c0.Multiply(c21ratio));

            var c31ratio = line3_remove_c0.c1 / line1_remove_c0.c1;
            var line3_remove_c1 = line3_remove_c0.Sub(line1_remove_c0.Multiply(c31ratio));
            //---------------------------------------------
            //step3:
            //primary line = line2
            //eliminate c2 from line3
            var c32ratio = line3_remove_c1.c2 / line2_remove_c1.c2;
            var line3_remove_c2 = line3_remove_c1.Sub(line2_remove_c1.Multiply(c32ratio));
            //---------------------------------------------
            //substitue back 
            //line3
            var r_c3 = line3_remove_c2.c3;
            if (r_c3 != 0)
            {
                r_c3 = line3_remove_c2.sum / r_c3;
            }
            //---------------------------------------------
            //line2
            var r_c2 = line2_remove_c1.sum - (line2_remove_c1.c3 * r_c3);
            if (line2_remove_c1.c2 != 0)
            {
                r_c2 = r_c2 / line2_remove_c1.c2;
            }
            //line 1
            var r_c1 = line1_remove_c0.sum - ((line1_remove_c0.c3 * r_c3) + (line1_remove_c0.c2 * r_c2));
            if (line1_remove_c0.c1 != 0)
            {
                r_c1 = r_c1 / line1_remove_c0.c1;
            }
            var r_c0 = line0.sum - ((line0.c1 * r_c1) + (line0.c2 * r_c2) + line0.c3 * r_c3);
            if (line0.c0 != null)
            {
                r_c0 = r_c0 / line0.c0;
            }


            resolve_c0 = r_c0;
            resolve_c1 = r_c1;
            resolve_c2 = r_c2;
            resolve_c3 = r_c3;


            ////step 1, eliminate x from L2 & L3
            //double xratio = line2.coeff_X / line1.coeff_X;
            //var line1_prepare_for_line2SUB = line1.Multiply(xratio);
            //var line2_remove_x = line2.Sub(line1_prepare_for_line2SUB);

            //xratio = line3.coeff_X / line1.coeff_X;
            //var line1_prepapre_for_line3SUB = line1.Multiply(xratio);
            //var line3_remove_x = line3.Sub(line1_prepapre_for_line3SUB);

            ////----------------------------------------
            ////remove y from line2_remove_x and line3_remove_x
            //double yratio = line3_remove_x.coeff_Y / line2_remove_x.coeff_Y;
            //var line2_prepare = line2_remove_x.Multiply(yratio);
            //var line3_remove_xy = line3_remove_x.Sub(line2_prepare);

            ////-------
            ////found z =>  
            //var simpleZ = line3_remove_xy.sumResult / line3_remove_xy.coeff_Z;
            ////found y
            //var simpleY = (line2_prepare.sumResult - (simpleZ * line2_prepare.coeff_Z)) / line2_prepare.coeff_Y;
            ////found x
            //var simpleX = (line1.sumResult - ((simpleZ * line1.coeff_Z) + (simpleY * line1.coeff_Y))) / line1.coeff_X;

            //resolve_x = simpleX;
            //resolve_y = simpleY;
            //resolve_z = simpleZ;
        }

    }
}
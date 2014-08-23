//MIT  2014, WinterDev 
//adapt from Wikipedia

namespace ImageTransformation
{

    class LinearEqSolver
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

        public void Resolve(LinearEq3 line1,
            LinearEq3 line2,
            LinearEq3 line3,
            out double resolve_x,
            out double resolve_y,
            out double resolve_z)
        {
            //step 1, eliminate x from L2 vs L1
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
    }


}
//MIT, 2009-2015, Rene Schulte and WriteableBitmapEx Contributors, https://github.com/teichgraf/WriteableBitmapEx

using PixelFarm.Agg;
namespace WinFormGdiPlus
{
    public class Particle
    {

        public Point Position;
        public Point Velocity;
        public Color Color;
        public double Lifespan;
        public double Elapsed;



        public void Initiailize()
        {
            Elapsed = 0;
        }

        public void Update(double elapsedSeconds)
        {
            Elapsed += elapsedSeconds;
            if (Elapsed > Lifespan)
            {
                Color.A = 0;
                return;
            }
            Color.A = (byte)(255 - ((255 * Elapsed)) / Lifespan);
            Position.X += Velocity.X * elapsedSeconds;
            Position.Y += Velocity.Y * elapsedSeconds;
        }

    }

}
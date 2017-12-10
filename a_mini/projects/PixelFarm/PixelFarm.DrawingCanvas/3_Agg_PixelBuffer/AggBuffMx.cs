//BSD, 2014-2017, WinterDev 
namespace PixelFarm.Agg
{
    public abstract class AggBuffMx
    { 
        static AggBuffMx s_impl;
        public static void SetNaiveBufferImpl(AggBuffMx impl)
        {
            s_impl = impl;
        }
        protected abstract void InnerMemSet(byte[] dest, int startAt, byte value, int count);
        protected abstract void InnerMemCopy(byte[] dest_buffer, int dest_startAt, byte[] src_buffer, int src_StartAt, int len);

        public static void MemSet(byte[] dest, int startAt, byte value, int count)
        {
            s_impl.InnerMemSet(dest, startAt, value, count);
        }
        public static void MemCopy(byte[] dest_buffer, int dest_startAt, byte[] src_buffer, int src_StartAt, int len)
        {
            s_impl.InnerMemCopy(dest_buffer, dest_startAt, src_buffer, src_StartAt, len);
        }
    } 
}
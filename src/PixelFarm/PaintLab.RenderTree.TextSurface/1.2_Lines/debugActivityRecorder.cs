﻿//Apache2, 2014-present, WinterDev

using System;
using System.IO;
namespace LayoutFarm.TextFlow
{
#if DEBUG
    class dbugCoreConst
    {
        public const string dbugRootFolder = "./";
    }
    public class debugActivityRecorder
    {
        StreamWriter strmWriter;
        string outputFileName = null;
        int msgCounter = 0;
        int indentCount = 0;
        int myTraceCount = 0;
        static int tracerCount = 0;
        public debugActivityRecorder()
        {
            myTraceCount = tracerCount;
            ++tracerCount;
            //outputFileName = dbugCoreConst.dbugRootFolder + "\\invalidate\\" + myTraceCount + "_" + Guid.NewGuid().ToString() + ".txt";
        }
        public void BeginContext()
        {
            indentCount++;
        }
        public void EndContext()
        {
            indentCount--;
        }
        public void Start(StreamWriter writer)
        {
            //fs = new FileStream(outputFileName, FileMode.Create);
            strmWriter = writer;
            strmWriter.AutoFlush = true;
        }
        public void Stop()
        {
            strmWriter.Flush();
            //strmWriter.Close();
            //strmWriter.Dispose();
            //fs.Close();
            //fs.Dispose();
            //fs = null;
        }

        public void WriteInfo(string info)
        {
            ++msgCounter;
            ShouldBreak();
            strmWriter.Write(new string('\t', indentCount));
            strmWriter.Write(info);
            strmWriter.Write("\r\n"); strmWriter.Flush();
        }
        internal void WriteInfo(VisualSelectionRange range)
        {
            if (range == null)
            {
                return;
            }

            WriteInfo(range.ToString());
        }
        void ShouldBreak()
        {
        }
    }
#endif
}
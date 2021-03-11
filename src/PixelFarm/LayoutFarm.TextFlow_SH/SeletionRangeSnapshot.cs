//Apache2, 2014-present, WinterDev


namespace LayoutFarm.TextFlow
{
    public readonly struct SelectionRangeSnapShot
    {
        public readonly int startLineNum;
        public readonly int startColumnNum;
        public readonly int endLineNum;
        public readonly int endColumnNum;
        public SelectionRangeSnapShot(int startLineNum, int startColumnNum, int endLineNum, int endColumnNum)
        {
            this.startLineNum = startLineNum;
            this.startColumnNum = startColumnNum;
            this.endLineNum = endLineNum;
            this.endColumnNum = endColumnNum;
        }
        public bool IsEmpty()
        {
            return startLineNum == 0 && startColumnNum == 0
                && endLineNum == 0 && endColumnNum == 0;
        }
        public static readonly SelectionRangeSnapShot Empty = new SelectionRangeSnapShot();

#if DEBUG
        public override string ToString() => startLineNum + ":" + startColumnNum + "," + endLineNum + ":" + endColumnNum;
#endif
    }
}
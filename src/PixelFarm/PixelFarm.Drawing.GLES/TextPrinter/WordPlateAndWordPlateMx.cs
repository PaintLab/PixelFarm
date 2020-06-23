//MIT, 2016-present, WinterDev
using System;
using System.Collections.Generic;
//
using PixelFarm.CpuBlit;
using PixelFarm.Drawing;

namespace PixelFarm.DrawingGL
{


    class WordPlateMx
    {

        Dictionary<ushort, WordPlate> _wordPlates = new Dictionary<ushort, WordPlate>();
        //**dictionay not guarantee sorted id**
        Queue<WordPlate> _wordPlatesQueue = new Queue<WordPlate>();
        WordPlate _latestPlate;

        int _defaultPlateW = 800;
        int _defaultPlateH = 600;

        static ushort s_totalPlateId = 0;

        public WordPlateMx()
        {
            MaxPlateCount = 20; //*** important!
            AutoRemoveOldestPlate = true;
        }

        public bool AutoRemoveOldestPlate { get; set; }
        public int MaxPlateCount { get; set; }
        public void SetDefaultPlateSize(int w, int h)
        {
            _defaultPlateH = h;
            _defaultPlateW = w;
        }
        public void ClearAllPlates()
        {
            foreach (WordPlate wordPlate in _wordPlates.Values)
            {
                wordPlate.Dispose();
            }
            _wordPlates.Clear();
            _wordPlatesQueue.Clear();
        }
        /// <summary>
        /// get current wordplate if there is avilable space, or create new one
        /// </summary>
        /// <param name="fmtPlate"></param>
        /// <returns></returns>
        public WordPlate GetWordPlate(GLRenderVxFormattedString fmtPlate)
        {
            if (_latestPlate != null &&
                _latestPlate.HasAvailableSpace(fmtPlate))
            {
                return _latestPlate;
            }
            return GetNewWordPlate();
        }
        public WordPlate GetNewWordPlate()
        {
            //create new and register  
            if (_wordPlates.Count == MaxPlateCount)
            {
                if (AutoRemoveOldestPlate)
                {
                    //**dictionay not guarantee sorted id**
                    //so we use queue, (TODO: use priority queue) 
                    WordPlate oldest = _wordPlatesQueue.Dequeue();
                    _wordPlates.Remove(oldest._plateId);
#if DEBUG
                    if (oldest.dbugUsedCount < 50)
                    {

                    }
                    //oldest.dbugSaveBackBuffer("word_plate_" + oldest._plateId + ".png");
#endif

                    oldest.Dispose();
                    oldest = null;
                }
            }

            if (s_totalPlateId + 1 >= ushort.MaxValue)
            {
                throw new NotSupportedException();
            }

            s_totalPlateId++;  //so plate_id starts at 1 

            WordPlate wordPlate = new WordPlate(s_totalPlateId, _defaultPlateW, _defaultPlateH);
            _wordPlates.Add(s_totalPlateId, wordPlate);
            _wordPlatesQueue.Enqueue(wordPlate);

#if DEBUG
            wordPlate.Cleared += WordPlate_Cleared;
#endif
            return _latestPlate = wordPlate;
        }
#if DEBUG
        private void WordPlate_Cleared(WordPlate obj)
        {

        }
#endif
    }

    public class WordPlate : IDisposable
    {
        bool _isInitBg;
        int _currentX;
        int _currentY;
        int _currentLineHeightMax;
        readonly int _plateWidth;
        readonly int _plateHeight;
        bool _full;

        internal readonly ushort _plateId;
        Dictionary<GLRenderVxFormattedString, bool> _wordStrips = new Dictionary<GLRenderVxFormattedString, bool>();
        internal Drawing.GLES2.MyGLBackbuffer _backBuffer;

        public event Action<WordPlate> Cleared;

        public WordPlate(ushort plateId, int w, int h)
        {
            _plateId = plateId;
            _plateWidth = w;
            _plateHeight = h;
            _backBuffer = new Drawing.GLES2.MyGLBackbuffer(w, h);
        }
#if DEBUG
        internal int dbugUsedCount;
        public void dbugSaveBackBuffer(string filename)
        {
            //save output
            using (Image img = _backBuffer.CopyToNewMemBitmap())
            {
                if (img is MemBitmap memBmp)
                {
                    memBmp.SaveImage(filename);
                }
            }
        }
#endif

        const int INTERLINE_SPACE = 1; //px
        const int INTERWORD_SPACE = 1; //px

        public void Dispose()
        {
            //clear all
            if (_backBuffer != null)
            {
                _backBuffer.Dispose();
                _backBuffer = null;
            }
            foreach (GLRenderVxFormattedString k in _wordStrips.Keys)
            {
                //essential!
                k.ClearOwnerPlate();
            }
            _wordStrips.Clear();
        }


        public bool Full => _full;

        public bool HasAvailableSpace(GLRenderVxFormattedString renderVxFormattedString)
        {
            //check if we have avaliable space for this?

            float width = renderVxFormattedString.Width;
            float previewY = _currentY;
#if DEBUG
            float previewX = _currentX;
#endif
            if (_currentX + width > _plateWidth)
            {
                //move to newline                    
                previewY += _currentLineHeightMax + INTERLINE_SPACE;

            }

            return previewY + renderVxFormattedString.SpanHeight < _plateHeight;
        }
        public bool CreateWordStrip(GLPainter painter, GLRenderVxFormattedString renderVxFormattedString)
        {
            //--------------
            //create stencil text buffer                  
            //we use white glyphs on black bg
            //--------------
            if (!_isInitBg)
            {
                //by default, we init bg to black for stencil buffer
                _isInitBg = true;
                painter.Clear(Color.Black);
            }



            float width = renderVxFormattedString.Width;

            if (_currentX + width > _plateWidth)
            {
                //move to newline
                _currentY += _currentLineHeightMax + INTERLINE_SPACE;//interspace =4 px
                _currentX = 0;
                //new line
                _currentLineHeightMax = (int)Math.Ceiling(renderVxFormattedString.SpanHeight);
            }

            //on current line
            //check available height
            if (_currentY + renderVxFormattedString.SpanHeight > _plateHeight)
            {
                _full = true;
                return false;
            }
            //----------------------------------


            if (renderVxFormattedString.SpanHeight > _currentLineHeightMax)
            {
                _currentLineHeightMax = (int)Math.Ceiling(renderVxFormattedString.SpanHeight);
            }


            Color prevColor = painter.FontFillColor;
            Color prevTextBgHint = painter.TextBgColorHint;
            bool prevPreparingWordStrip = painter.PreparingWordStrip;
            GlyphTexturePrinterDrawingTechnique prevTextDrawing = painter.TextPrinterDrawingTechnique;

            painter.TextBgColorHint = Color.Black;
            painter.FontFillColor = Color.White;
            painter.PreparingWordStrip = true;
            renderVxFormattedString.UseWithWordPlate = false;

            //----
            RequestFont reqFont = painter.CurrentFont; 
            //This is a temp FIX
            if (reqFont.Name.Contains("Emoji"))
            {
                //System.Diagnostics.Debugger.Break();

                //some font is color font,
                //eg some bitmap font, some svg, or color glyph
                //we will send background rgn for it to transparent bg
                //painter.ClearRect(Color.Transparent, _currentX, _currentY, width, _currentLineHeightMax);
                //painter.ClearRect(Color.Red, _currentX, _currentY, 200, 200);
                painter.TextPrinterDrawingTechnique = GlyphTexturePrinterDrawingTechnique.Copy;
                //painter.Clear(Color.Transparent);

                //choice 1
                //painter.ClearRect(Color.Red, _currentX, _currentY, width, _currentLineHeightMax);

                //--
                //choice 2
                Rectangle currentClip = painter.ClipBox;
                painter.SetClipBox(_currentX, _currentY, (int)(_currentX + width), _currentY + _currentLineHeightMax);
                painter.Clear(Color.Transparent);
                painter.SetClipBox(currentClip.Left, currentClip.Top, currentClip.Right, currentClip.Bottom); //restore
                //--
            }



            painter.DrawString(renderVxFormattedString, _currentX, _currentY);

            renderVxFormattedString.UseWithWordPlate = true;//restore
            painter.FontFillColor = prevColor;//restore
            painter.TextBgColorHint = prevTextBgHint;//restore
            painter.PreparingWordStrip = prevPreparingWordStrip;
            painter.TextPrinterDrawingTechnique = prevTextDrawing;
            //in this case we can dispose vbo inside renderVx
            //(we can recreate that vbo later)
            renderVxFormattedString.DisposeVbo();

            renderVxFormattedString.OwnerPlate = this;
            renderVxFormattedString.WordPlateLeft = (ushort)_currentX;
            renderVxFormattedString.WordPlateTop = (ushort)_currentY;
            renderVxFormattedString.UseWithWordPlate = true;

#if DEBUG
            dbugUsedCount++;
#endif
            _wordStrips.Add(renderVxFormattedString, true);
            //--------

            _currentX += (int)Math.Ceiling(renderVxFormattedString.Width) + INTERWORD_SPACE; //interspace x 1px

#if DEBUG
            //dbugSaveBackBuffer("dbug_test1.png");
#endif

            return true;
        }

        public void RemoveWordStrip(GLRenderVxFormattedString vx)
        {
            _wordStrips.Remove(vx);
            if (_full && _wordStrips.Count == 0)
            {
                Cleared?.Invoke(this);
            }
        }
    }

}
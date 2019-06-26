// 
// BacktrackingStringMatcher.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
//       Andrea Krüger <andrea@shakuras.homeunix.net>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;

//from namespace MonoDevelop.Core.Text
namespace PixelFarm.TreeCollection
{
    class BacktrackingStringMatcher : StringMatcher
    {
        readonly string _filterTextUpperCase;
        readonly ulong _filterTextLowerCaseTable;
        readonly ulong _filterIsNonLetter;
        readonly ulong _filterIsDigit;
        readonly string _filterText;
        int[] _cachedResult;

        public BacktrackingStringMatcher(string filterText)
        {
            _filterText = filterText ?? "";
            if (filterText != null)
            {
                for (int i = 0; i < filterText.Length && i < 64; i++)
                {
                    _filterTextLowerCaseTable |= char.IsLower(filterText[i]) ? 1ul << i : 0;
                    _filterIsNonLetter |= !char.IsLetterOrDigit(filterText[i]) ? 1ul << i : 0;
                    _filterIsDigit |= char.IsDigit(filterText[i]) ? 1ul << i : 0;
                }

                _filterTextUpperCase = filterText.ToUpper();
            }
            else
            {
                _filterTextUpperCase = "";
            }
        }

        public override bool CalcMatchRank(string name, out int matchRank)
        {
            if (_filterTextUpperCase.Length == 0)
            {
                matchRank = int.MinValue;
                return true;
            }
            var lane = GetMatch(name);
            if (lane != null)
            {
                if (name.Length == _filterText.Length)
                {
                    matchRank = int.MaxValue;
                    for (int n = 0; n < name.Length; n++)
                    {
                        if (_filterText[n] != name[n])
                            matchRank--;
                    }
                    return true;
                }
                // exact named parameter case see discussion in bug #9114
                if (name.Length - 1 == _filterText.Length && name[name.Length - 1] == ':')
                {
                    matchRank = int.MaxValue - 1;
                    for (int n = 0; n < name.Length - 1; n++)
                    {
                        if (_filterText[n] != name[n])
                            matchRank--;
                    }
                    return true;
                }
                int capitalMatches = 0;
                int nonCapitalMatches = 0;
                int matching = 0;
                int fragments = 0;
                int lastIndex = -1;
                for (int n = 0; n < lane.Length; n++)
                {
                    var ch = _filterText[n];
                    var i = lane[n];
                    bool newFragment = i > lastIndex + 1;
                    if (newFragment)
                        fragments++;
                    lastIndex = i;
                    if (ch == name[i])
                    {
                        matching += 1000 / (1 + fragments);
                        if (char.IsUpper(ch))
                            capitalMatches += Math.Max(1, 10000 - 1000 * fragments);
                    }
                    else if (newFragment || i == 0)
                    {
                        matching += 900 / (1 + fragments);
                        if (char.IsUpper(ch))
                            capitalMatches += Math.Max(1, 1000 - 100 * fragments);
                    }
                    else
                    {
                        var x = 600 / (1 + fragments);
                        nonCapitalMatches += x;
                    }
                }
                matchRank = capitalMatches + matching - fragments + nonCapitalMatches + _filterText.Length - name.Length;
                // devalue named parameters.
                if (name[name.Length - 1] == ':')
                    matchRank /= 2;
                return true;
            }
            matchRank = int.MinValue;
            return false;
        }

        public override bool IsMatch(string text)
        {
            int[] match = GetMatch(text);
            // no need to clear the cache
            _cachedResult = _cachedResult ?? match;
            return match != null;
        }

        int GetMatchChar(string text, int i, int j, bool onlyWordStart)
        {
            char filterChar = _filterTextUpperCase[i];
            char ch;
            // filter char is no letter -> next char should match it - see Bug 674512 - Space doesn't commit generics
            var flag = 1ul << i;
            if ((_filterIsNonLetter & flag) != 0)
            {
                for (; j < text.Length; j++)
                {
                    if (filterChar == text[j])
                        return j;
                }
                return -1;
            }
            // letter case
            ch = text[j];
            bool textCharIsUpper = char.IsUpper(ch);
            if (!onlyWordStart)
            {
                if (filterChar == (textCharIsUpper ? ch : char.ToUpper(ch)) && char.IsLetter(ch))
                {
                    // cases don't match. Filter is upper char & letter is low, now prefer the match that does the word skip.
                    if (!(textCharIsUpper || (_filterTextLowerCaseTable & flag) != 0) && j + 1 < text.Length)
                    {
                        int possibleBetterResult = GetMatchChar(text, i, j + 1, onlyWordStart);
                        if (possibleBetterResult >= 0)
                            return possibleBetterResult;
                    }
                    return j;
                }
            }
            else
            {
                if (textCharIsUpper && filterChar == ch && char.IsLetter(ch))
                {
                    return j;
                }
            }

            // no match, try to continue match at the next word start
            bool lastWasLower = false;
            bool lastWasUpper = false;
            int wordStart = j + 1;
            for (; j < text.Length; j++)
            {
                // word start is either a upper case letter (FooBar) or a char that follows a non letter
                // like foo:bar 
                ch = text[j];
                var category = char.GetUnicodeCategory(ch);
                if (category == System.Globalization.UnicodeCategory.LowercaseLetter)
                {
                    if (lastWasUpper && (j - wordStart) > 0)
                    {
                        if (filterChar == char.ToUpper(text[j - 1]))
                            return j - 1;
                    }
                    lastWasLower = true;
                    lastWasUpper = false;
                }
                else if (category == System.Globalization.UnicodeCategory.UppercaseLetter)
                {
                    if (lastWasLower)
                    {
                        if (filterChar == char.ToUpper(ch))
                            return j;
                    }
                    lastWasLower = false;
                    lastWasUpper = true;
                }
                else
                {
                    if (filterChar == ch)
                        return j;
                    if (j + 1 < text.Length && filterChar == char.ToUpper(text[j + 1]))
                        return j + 1;
                    lastWasLower = lastWasUpper = false;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the match indices.
        /// </summary>
        /// <returns>
        /// The indices in the text which are matched by our filter.
        /// </returns>
        /// <param name='text'>
        /// The text to match.
        /// </param>
        public override int[] GetMatch(string text)
        {
            if (string.IsNullOrEmpty(_filterTextUpperCase))
                return new int[0];
            if (string.IsNullOrEmpty(text) || _filterText.Length > text.Length)
                return null;
            int[] result;
            if (_cachedResult != null)
            {
                result = _cachedResult;
            }
            else
            {
                _cachedResult = result = new int[_filterTextUpperCase.Length];
            }
            int j = 0;
            int i = 0;
            bool onlyWordStart = false;
            while (i < _filterText.Length)
            {
                if (j >= text.Length)
                {
                    if (i > 0)
                    {
                        j = result[--i] + 1;
                        onlyWordStart = true;
                        continue;
                    }
                    return null;
                }

                j = GetMatchChar(text, i, j, onlyWordStart);
                onlyWordStart = false;
                if (j == -1)
                {
                    if (i > 0)
                    {
                        j = result[--i] + 1;
                        onlyWordStart = true;
                        continue;
                    }
                    return null;
                }
                else
                {
                    result[i] = j++;
                }
                i++;
            }
            _cachedResult = null;
            // clear cache
            return result;
        }
    }
}


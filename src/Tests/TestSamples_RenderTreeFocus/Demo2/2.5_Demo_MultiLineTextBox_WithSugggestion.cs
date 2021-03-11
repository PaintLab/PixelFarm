//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.TextFlow;
namespace LayoutFarm
{
    [DemoNote("2.5 MultiLineText_WithSuggestion")]
    public class Demo_MultiLineText_WithSuggestion : App
    {
        LayoutFarm.CustomWidgets.TextBox _textbox;
        LayoutFarm.CustomWidgets.ListBox _listbox;
        Dictionary<char, List<string>> _words = new Dictionary<char, List<string>>();
        //
        protected override void OnStart(AppHost host)
        {
            _textbox = new LayoutFarm.CustomWidgets.TextBox(400, 300, true);
            _textbox.SetLocation(20, 20);

            var style1 = new TextSpanStyle();
            style1.ReqFont = new PixelFarm.Drawing.RequestFont("tahoma", 10);
            style1.FontColor = new PixelFarm.Drawing.Color(0, 0, 0);
            _textbox.DefaultSpanStyle = style1;

            var textSplitter = new LayoutFarm.CustomWidgets.ContentTextSplitter();
            _textbox.TextSplitter = textSplitter;
            _listbox = new CustomWidgets.ListBox(300, 200);
            _listbox.SetLocation(0, 40);
            _listbox.Visible = false;
            //------------------------------------
            //create special text surface listener
            var textSurfaceListener = new TextSurfaceEventListener();
            textSurfaceListener.CharacterAdded += (s, e) => UpdateSuggestionList();
            textSurfaceListener.CharacterRemoved += (s, e) => UpdateSuggestionList();
            textSurfaceListener.PreviewArrowKeyDown += textSurfaceListener_PreviewArrowKeyDown;
            textSurfaceListener.PreviewEnterKeyDown += textSurfaceListener_PreviewEnterKeyDown;
            _textbox.TextEventListener = textSurfaceListener;
            //------------------------------------ 
            host.AddChild(_textbox);
            host.AddChild(_listbox);
            //------------------------------------ 
            BuildSampleCountryList();
        }
        void textSurfaceListener_PreviewArrowKeyDown(object sender, TextDomEventArgs e)
        {
            //update selection in list box 
            switch (e.Key)
            {
                case UIKeys.Down:
                    {
                        if (_listbox.Visible && _listbox.SelectedIndex < _listbox.ItemCount - 1)
                        {
                            _listbox.SelectedIndex++;
                            _listbox.EnsureSelectedItemVisible();
                            e.PreventDefault = true;
                        }
                    }
                    break;
                case UIKeys.Up:
                    {
                        if (_listbox.Visible && _listbox.SelectedIndex > 0)
                        {
                            _listbox.SelectedIndex--;
                            _listbox.EnsureSelectedItemVisible();
                            e.PreventDefault = true;
                        }
                    }
                    break;
            }
        }
        void textSurfaceListener_PreviewEnterKeyDown(object sender, TextDomEventArgs e)
        {
            //accept selected text
            if (!_listbox.Visible || _listbox.SelectedIndex < 0)
            {
                return;
            }
            if (_textbox.CurrentTextSpan != null)
            {
                _textbox.ReplaceCurrentTextRunContent(_currentLocalText.Length,
                    (string)_listbox.GetItem(_listbox.SelectedIndex).Tag);
                //------------------------------------- 
                //then hide suggestion list
                _listbox.ClearItems();
                _listbox.Visible = false;
                //-------------------------------------- 
            }
            e.PreventDefault = true;
        }
        static string GetString(char[] buffer, LayoutFarm.Composers.TextSplitBounds bound)
        {
            return new string(buffer, bound.startIndex, bound.length);
        }

        string _currentLocalText = null;
        List<LayoutFarm.Composers.TextSplitBounds> _textSplitBoundsList = new List<Composers.TextSplitBounds>();

        static int GetProperSplitBoundIndex(List<LayoutFarm.Composers.TextSplitBounds> _textSplitBoundsList, int charIndex)
        {
            int j = _textSplitBoundsList.Count;
            int accumChar = 0;
            for (int i = 0; i < j; ++i)
            {
                LayoutFarm.Composers.TextSplitBounds splittedBound = _textSplitBoundsList[i];
                if (accumChar + splittedBound.length >= charIndex)
                {
                    return i;
                }
                accumChar += splittedBound.length;
            }
            return -1;//not found?
        }
        void UpdateSuggestionList()
        {
            //find suggestion words 
            _currentLocalText = null;
            _listbox.ClearItems();
            Run currentSpan = _textbox.CurrentTextSpan;
            if (currentSpan == null)
            {
                _listbox.Visible = false;
                return;
            }
            //-------------------------------------------------------------------------
            //sample parse ...
            //In this example  all country name start with Captial letter so ...

            //try to get underlining text

            //int startAt, len;
            //textbox.FindCurrentUnderlyingWord(out startAt, out len);

            string currentTextSpanText = currentSpan.GetUpperString();
            //analyze content
            char[] textBuffer = currentTextSpanText.ToCharArray();
            _textSplitBoundsList.Clear();
            _textSplitBoundsList.AddRange(_textbox.TextSplitter.ParseWordContent(textBuffer, 0, textBuffer.Length));

            //get last part of splited text
            int m = _textSplitBoundsList.Count;
            if (m < 1)
            {
                return;
            }

            int splitBoundIndex = GetProperSplitBoundIndex(_textSplitBoundsList, _textbox.CurrentLineCharIndex);
            if (splitBoundIndex < 0)
            {
                return;
            }

            //find current split bounds
            Composers.TextSplitBounds selectBounds = _textSplitBoundsList[splitBoundIndex];
            _currentLocalText = GetString(textBuffer, selectBounds);


            char firstChar = _currentLocalText[0];
            if (_words.TryGetValue(firstChar, out List<string> keywords))
            {
                int j = keywords.Count;
                int listViewWidth = _listbox.Width;
                for (int i = 0; i < j; ++i)
                {
                    string choice = keywords[i].ToUpper();
                    if (StringStartsWithChars(choice, _currentLocalText))
                    {
                        CustomWidgets.ListItem item = new CustomWidgets.ListItem(listViewWidth, 17);
                        item.BackColor = KnownColors.LightGray;
                        item.Tag = item.Text = keywords[i];
                        _listbox.AddItem(item);
                    }
                }
            }
            if (_listbox.ItemCount > 0)
            {
                _listbox.Visible = true;
                //TODO: implement selectedIndex suggestion hint here ***
                _listbox.SelectedIndex = 0;
                //move listview under caret position 
                var caretPos = _textbox.CaretPosition;
                //temp fixed
                //TODO: review here
                _listbox.SetLocation(_textbox.Left + caretPos.X, _textbox.Top + caretPos.Y + 20);
                _listbox.EnsureSelectedItemVisible();
            }
            else
            {
                _listbox.Visible = false;
            }

            //-------------------------------------------------------------------------
        }
        static bool StringStartsWithChars(string srcString, string value)
        {
            int findingLen = value.Length;
            if (findingLen > srcString.Length)
            {
                return false;
            }
            //
            unsafe
            {
                fixed (char* srcStringBuff = srcString)
                fixed (char* findingChar = value)
                {
                    char* srcBuff1 = srcStringBuff;
                    char* findChar1 = findingChar;
                    for (int i = 0; i < findingLen; ++i)
                    {
                        //compare by values
                        if (*srcBuff1 != *findChar1)
                        {
                            return false;
                        }
                        srcBuff1++;
                        findChar1++;
                    }
                    //MATCH all
                    return true;
                }
            }
        }

        void BuildSampleCountryList()
        {
            AddKeywordList(@"
Afghanistan
Albania
Algeria
American Samoa
Andorra
Angola
Anguilla
Antarctica
Antigua and Barbuda
Argentina
Armenia
Aruba
Australia
Austria
Azerbaijan
Bahamas
Bahrain
Bangladesh
Barbados
Belarus
Belgium
Belize
Benin
Bermuda
Bhutan
Bolivia
Bosnia and Herzegovina
Botswana
Brazil
Brunei Darussalam
Bulgaria
Burkina Faso
Burundi
Cambodia
Cameroon
Canada
Cape Verde
Cayman Islands
Central African Republic
Chad
Chile
China
Christmas Island
Cocos (Keeling) Islands
Colombia
Comoros
Democratic Republic of the Congo (Kinshasa)
'Congo, Republic of (Brazzaville)'
Cook Islands
Costa Rica
Ivory Coast
Croatia
Cuba
Cyprus
Czech Republic
Denmark
Djibouti
Dominica
Dominican Republic
East Timor (Timor-Leste)
Ecuador
Egypt
El Salvador
Equatorial Guinea
Eritrea
Estonia
Ethiopia
Falkland Islands
Faroe Islands
Fiji
Finland
France
French Guiana
French Polynesia
French Southern Territories
Gabon
Gambia
Georgia
Germany
Ghana
Gibraltar
Great Britain
Greece
Greenland
Grenada
Guadeloupe
Guam
Guatemala
Guinea
Guinea-Bissau
Guyana
Haiti
Holy See
Honduras
Hong Kong
Hungary
Iceland
India
Indonesia
Iran (Islamic Republic of)
Iraq
Ireland
Israel
Italy
Jamaica
Japan
Jordan
Kazakhstan
Kenya
Kiribati
'Korea, Democratic People's Rep. (North Korea)'
'Korea, Republic of (South Korea)'
Kuwait
Kyrgyzstan
'Lao, People's Democratic Republic'
Latvia
Lebanon
Lesotho
Liberia
Libya
Liechtenstein
Lithuania
Luxembourg
Macau
'Macedonia, Rep. of'
Madagascar
Malawi
Malaysia
Maldives
Mali
Malta
Marshall Islands
Martinique
Mauritania
Mauritius
Mayotte
Mexico
'Micronesia, Federal States of'
'Moldova, Republic of'
Monaco
Mongolia
Montenegro
Montserrat
Morocco
Mozambique
'Myanmar, Burma'
Namibia
Nauru
Nepal
Netherlands
Netherlands Antilles
New Caledonia
New Zealand
Nicaragua
Niger
Nigeria
Niue
Northern Mariana Islands
Norway
Oman
Pakistan
Palau
Palestinian territories
Panama
Papua New Guinea
Paraguay
Peru
Philippines
Pitcairn Island
Poland
Portugal
Puerto Rico
Qatar
Reunion Island
Romania
Russian Federation
Rwanda
Saint Kitts and Nevis
Saint Lucia
Saint Vincent and the Grenadines
Samoa
San Marino
Sao Tome and Principe
Saudi Arabia
Senegal
Serbia
Seychelles
Sierra Leone
Singapore
Slovakia (Slovak Republic)
Slovenia
Solomon Islands
Somalia
South Africa
South Sudan
Spain
Sri Lanka
Sudan
Suriname
Swaziland
Sweden
Switzerland
'Syria, Syrian Arab Republic'
Taiwan (Republic of China)
Tajikistan
Tanzania; officially the United Republic of Tanzania
Thailand
Tibet
Timor-Leste (East Timor)
Togo
Tokelau
Tonga
Trinidad and Tobago
Tunisia
Turkey
Turkmenistan
Turks and Caicos Islands
Tuvalu
Uganda
Ukraine
United Arab Emirates
United Kingdom
United States
Uruguay
Uzbekistan
Vanuatu
Vatican City State (Holy See)
Venezuela
Vietnam
Virgin Islands (British)
Virgin Islands (U.S.)
Wallis and Futuna Islands
Western Sahara
Yemen
Zambia
Zimbabwe");
        }
        void AddKeywordList(string keywordString)
        {
            string[] seplist = keywordString.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            int j = seplist.Length;
            for (int i = 0; i < j; ++i)
            {
                string sepWord = seplist[i].Trim();
                if (sepWord.StartsWith("'"))
                {
                    sepWord = sepWord.Substring(1, sepWord.Length - 2);
                }
                char firstChar = sepWord[0];
                if (!_words.TryGetValue(firstChar, out List<string> list))
                {
                    list = new List<string>();
                    _words.Add(firstChar, list);
                }
                list.Add(sepWord);
                list.Sort();
            }
        }
    }
}
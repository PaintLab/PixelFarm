//Apache2, 2014-2018, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("2.5 MultiLineText_WithSuggestion")]
    class Demo_MultiLineText_WithSuggestion : DemoBase
    {
        LayoutFarm.CustomWidgets.TextBox textbox;
        LayoutFarm.CustomWidgets.ListView listView;
        Dictionary<char, List<string>> words = new Dictionary<char, List<string>>();
        protected override void OnStartDemo(SampleViewport viewport)
        {
            textbox = new LayoutFarm.CustomWidgets.TextBox(400, 300, true);
            textbox.SetLocation(20, 20);

            var style1 = new Text.TextSpanStyle();
            style1.FontInfo = new PixelFarm.Drawing.RequestFont("tahoma", 10);
            style1.FontColor = new PixelFarm.Drawing.Color(0, 0, 0);
            textbox.DefaultSpanStyle = style1;

            var textSplitter = new LayoutFarm.CustomWidgets.ContentTextSplitter();
            textbox.TextSplitter = textSplitter;
            listView = new CustomWidgets.ListView(300, 200);
            listView.SetLocation(0, 40);
            listView.Visible = false;
            //------------------------------------
            //create special text surface listener
            var textSurfaceListener = new LayoutFarm.Text.TextSurfaceEventListener();
            textSurfaceListener.CharacterAdded += (s, e) => UpdateSuggestionList();
            textSurfaceListener.CharacterRemoved += (s, e) => UpdateSuggestionList();
            textSurfaceListener.PreviewArrowKeyDown += new EventHandler<Text.TextDomEventArgs>(textSurfaceListener_PreviewArrowKeyDown);
            textSurfaceListener.PreviewEnterKeyDown += new EventHandler<Text.TextDomEventArgs>(textSurfaceListener_PreviewEnterKeyDown);
            textbox.TextEventListener = textSurfaceListener;
            //------------------------------------ 
            viewport.AddChild(textbox);
            viewport.AddChild(listView);
            //------------------------------------ 
            BuildSampleCountryList();
        }
        void textSurfaceListener_PreviewArrowKeyDown(object sender, Text.TextDomEventArgs e)
        {
            //update selection in list box 
            switch (e.key)
            {
                case UIKeys.Down:
                    {
                        if (listView.Visible && listView.SelectedIndex < listView.ItemCount - 1)
                        {
                            listView.SelectedIndex++;
                            listView.EnsureSelectedItemVisible();
                            e.PreventDefault = true;
                        }
                    }
                    break;
                case UIKeys.Up:
                    {
                        if (listView.Visible && listView.SelectedIndex > 0)
                        {
                            listView.SelectedIndex--;
                            listView.EnsureSelectedItemVisible();
                            e.PreventDefault = true;
                        }
                    }
                    break;
            }
        }
        void textSurfaceListener_PreviewEnterKeyDown(object sender, Text.TextDomEventArgs e)
        {
            //accept selected text
            if (!listView.Visible || listView.SelectedIndex < 0)
            {
                return;
            }
            if (textbox.CurrentTextSpan != null)
            {
                textbox.ReplaceCurrentTextRunContent(currentLocalText.Length,
                    (string)listView.GetItem(listView.SelectedIndex).Tag);
                //------------------------------------- 
                //then hide suggestion list
                listView.ClearItems();
                listView.Visible = false;
                //-------------------------------------- 
            }
            e.PreventDefault = true;
        }
        static string GetString(char[] buffer, LayoutFarm.Composers.TextSplitBound bound)
        {
            return new string(buffer, bound.startIndex, bound.length);
        }
        string currentLocalText = null;

        List<LayoutFarm.Composers.TextSplitBound> _textSplitBoundsList = new List<Composers.TextSplitBound>();

        static int GetProperSplitBoundIndex(List<LayoutFarm.Composers.TextSplitBound> _textSplitBoundsList, int charIndex)
        {
            int j = _textSplitBoundsList.Count;
            int accumChar = 0;
            for (int i = 0; i < j; ++i)
            {
                LayoutFarm.Composers.TextSplitBound splittedBound = _textSplitBoundsList[i];
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
            this.currentLocalText = null;
            listView.ClearItems();
            Text.EditableRun currentSpan = textbox.CurrentTextSpan;
            if (currentSpan == null)
            {
                listView.Visible = false;
                return;
            }
            //-------------------------------------------------------------------------
            //sample parse ...
            //In this example  all country name start with Captial letter so ...

            //try to get underlining text

            //int startAt, len;
            //textbox.FindCurrentUnderlyingWord(out startAt, out len);

            string currentTextSpanText = currentSpan.GetText().ToUpper();
            //analyze content
            char[] textBuffer = currentTextSpanText.ToCharArray();
            _textSplitBoundsList.Clear();
            _textSplitBoundsList.AddRange(textbox.TextSplitter.ParseWordContent(textBuffer, 0, textBuffer.Length));

            //get last part of splited text
            int m = _textSplitBoundsList.Count;
            if (m < 1)
            {
                return;
            }

            int splitBoundIndex = GetProperSplitBoundIndex(_textSplitBoundsList, textbox.CurrentLineCharIndex);
            if (splitBoundIndex < 0)
            {
                return;
            }

            //find current split bounds
            Composers.TextSplitBound selectBounds = _textSplitBoundsList[splitBoundIndex];
            this.currentLocalText = GetString(textBuffer, selectBounds);


            char firstChar = currentLocalText[0];
            List<string> keywords;
            if (words.TryGetValue(firstChar, out keywords))
            {
                int j = keywords.Count;
                int listViewWidth = listView.Width;
                for (int i = 0; i < j; ++i)
                {
                    string choice = keywords[i].ToUpper();
                    if (StringStartsWithChars(choice, currentLocalText))
                    {
                        CustomWidgets.ListItem item = new CustomWidgets.ListItem(listViewWidth, 17);
                        item.BackColor = Color.LightGray;
                        item.Tag = item.Text = keywords[i];
                        listView.AddItem(item);
                    }
                }
            }
            if (listView.ItemCount > 0)
            {
                listView.Visible = true;
                //TODO: implement selectedIndex suggestion hint here ***
                listView.SelectedIndex = 0;
                //move listview under caret position 
                var caretPos = textbox.CaretPosition;
                //temp fixed
                //TODO: review here
                listView.SetLocation(textbox.Left + caretPos.X, textbox.Top + caretPos.Y + 20);
                listView.EnsureSelectedItemVisible();
            }
            else
            {
                listView.Visible = false;
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
                List<string> list;
                if (!words.TryGetValue(firstChar, out list))
                {
                    list = new List<string>();
                    words.Add(firstChar, list);
                }
                list.Add(sepWord);
                list.Sort();
            }
        }
    }
}
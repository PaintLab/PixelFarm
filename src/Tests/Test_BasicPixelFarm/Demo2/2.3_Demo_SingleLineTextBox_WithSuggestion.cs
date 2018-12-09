//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("2.3 SingleLineText_WithSuggestion")]
    class Demo_SingleLineText_WithSuggestion : App
    {
        LayoutFarm.CustomWidgets.TextBox _textbox;
        LayoutFarm.CustomWidgets.ListView _listView;
        Dictionary<char, List<string>> _words = new Dictionary<char, List<string>>();
        //
        protected override void OnStart(AppHost host)
        {
            _textbox = new LayoutFarm.CustomWidgets.TextBox(400, 30, false);
            _textbox.SetLocation(20, 20);
            var style2 = new Text.TextSpanStyle();
            style2.ReqFont = new PixelFarm.Drawing.RequestFont("tahoma", 14);
            style2.FontColor = new PixelFarm.Drawing.Color(0, 0, 0);
            _textbox.DefaultSpanStyle = style2;

            var textSplitter = new LayoutFarm.CustomWidgets.ContentTextSplitter();
            _textbox.TextSplitter = textSplitter;
            _listView = new CustomWidgets.ListView(300, 200);
            _listView.SetLocation(0, 40);
            _listView.Visible = false;
            //------------------------------------
            //create special text surface listener
            var textSurfaceListener = new LayoutFarm.Text.TextSurfaceEventListener();
            textSurfaceListener.CharacterAdded += (s, e) => UpdateSuggestionList();
            textSurfaceListener.CharacterRemoved += (s, e) => UpdateSuggestionList();
            textSurfaceListener.PreviewArrowKeyDown += new EventHandler<Text.TextDomEventArgs>(textSurfaceListener_PreviewArrowKeyDown);
            textSurfaceListener.PreviewEnterKeyDown += new EventHandler<Text.TextDomEventArgs>(textSurfaceListener_PreviewEnterKeyDown);
            _textbox.TextEventListener = textSurfaceListener;
            //------------------------------------ 
            host.AddChild(_textbox);
            host.AddChild(_listView);
            //------------------------------------ 
            BuildSampleCountryList();
        }


        void textSurfaceListener_PreviewArrowKeyDown(object sender, Text.TextDomEventArgs e)
        {
            //update selection in list box
            switch (e.Key)
            {
                case UIKeys.Down:
                    {
                        if (_listView.SelectedIndex < _listView.ItemCount - 1)
                        {
                            _listView.SelectedIndex++;
                        }
                    }
                    break;
                case UIKeys.Up:
                    {
                        if (_listView.SelectedIndex > 0)
                        {
                            _listView.SelectedIndex--;
                        }
                    }
                    break;
            }
        }
        void textSurfaceListener_PreviewEnterKeyDown(object sender, Text.TextDomEventArgs e)
        {
            //accept selected text

            if (_textbox.CurrentTextSpan != null)
            {
                if (_listView.SelectedIndex >= 0)
                {
                    _textbox.ReplaceCurrentTextRunContent(currentLocalText.Length,
                        (string)_listView.GetItem(_listView.SelectedIndex).Tag);
                    //------------------------------------- 
                    //then hide suggestion list
                    _listView.ClearItems();
                    _listView.Visible = false;
                    //-------------------------------------- 
                }
            }
            e.PreventDefault = true;
        }

        static string GetString(char[] buffer, LayoutFarm.Composers.TextSplitBound bound)
        {
            return new string(buffer, bound.startIndex, bound.length);
        }
        string currentLocalText = null;
        void UpdateSuggestionList()
        {
            //find suggestion words 
            this.currentLocalText = null;
            _listView.ClearItems();
            if (_textbox.CurrentTextSpan == null)
            {
                _listView.Visible = false;
                return;
            }
            //-------------------------------------------------------------------------
            //simple parse ...
            //In this example  all country name start with Captial letter so ...
            string currentTextSpanText = _textbox.CurrentTextSpan.GetText().ToUpper();
            //analyze content
            char[] textBuffer = currentTextSpanText.ToCharArray();
            var results = new List<LayoutFarm.Composers.TextSplitBound>();
            results.AddRange(_textbox.TextSplitter.ParseWordContent(textBuffer, 0, textBuffer.Length));
            //get last part of splited text
            int m = results.Count;
            if (m < 1)
            {
                return;
            }
            Composers.TextSplitBound lastSplitPart = results[m - 1];
            this.currentLocalText = GetString(textBuffer, lastSplitPart);
            //char firstChar = currentTextSpanText[0];
            char firstChar = currentLocalText[0];
            List<string> keywords;
            if (_words.TryGetValue(firstChar, out keywords))
            {
                int j = keywords.Count;
                int listViewWidth = _listView.Width;
                for (int i = 0; i < j; ++i)
                {
                    string choice = keywords[i].ToUpper();
                    if (choice.StartsWith(currentLocalText))
                    {
                        CustomWidgets.ListItem item = new CustomWidgets.ListItem(listViewWidth, 17);
                        item.BackColor = Color.LightGray;
                        item.Tag = item.Text = keywords[i];
                        _listView.AddItem(item);
                    }
                }
            }
            if (_listView.ItemCount > 0)
            {
                _listView.Visible = true;
            }
            else
            {
                _listView.Visible = false;
            }

            //-------------------------------------------------------------------------
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
                string sepWord = seplist[i];
                if (sepWord.StartsWith("'"))
                {
                    sepWord = sepWord.Substring(1, sepWord.Length - 2);
                }
                char firstChar = sepWord[0];
                List<string> list;
                if (!_words.TryGetValue(firstChar, out list))
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
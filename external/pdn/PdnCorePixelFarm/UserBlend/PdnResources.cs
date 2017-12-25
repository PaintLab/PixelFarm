/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
namespace PaintDotNet
{
    static class BlendResources
    {


        static ResourceItems resItems = new ResourceItems();

        public static string GetString(string stringName)
        {
            string theString = resItems.GetString(stringName);
            if (theString == null)
            {
                Debug.WriteLine(stringName + " not found");
            }

            //return theString;
            return theString;
        }


    }
}

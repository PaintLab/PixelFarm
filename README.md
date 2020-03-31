PixelFarm
=========
Hardware and Software 2D Rendering Library

1.  Hardware Rendering Technology:
     
    The library uses OpenGL ES2+ and its Shading Langauge (GLSL) as its hardware-renderer backend.
    
    a lion cup below is read from svg file=> tessellated and rendered with GLES Painter
    
    ![gles2_aa_shader](https://cloud.githubusercontent.com/assets/7447159/20637925/221cc87a-b3c9-11e6-94a5-47c3b1026fd9.png)
  
  _GLES backend Painter_

    ---
    	 
	
2. Software Rendering Technology: 
   
    The library also provides a 'classic' (pure) software renderer.
    
    It uses a forked version of _Agg-Sharp_, in side this lib it is called _MiniAgg_

      >Agg-Sharp is the C# port of Anti-Grain Geometry (AGG)  version (version 2.4, BSD license)  

    and  Big thanks go to https://github.com/MatterHackers/agg-sharp

![lions](https://user-images.githubusercontent.com/7447159/77984163-06a59100-733b-11ea-9955-5fd7ac96c5d2.png)
  
 _Left: MiniAgg backend Painter  vs Right: Gdi+_
   
 Agg-based Painter provides a high quality graphics output.



![tiger](https://user-images.githubusercontent.com/7447159/34709205-cdf2a2de-f548-11e7-8075-1958c087a883.png)

_PixelFarm's Agg (1) vs Chrome (2), Ghost script's Tiger.svg(https://commons.wikimedia.org/wiki/File:Ghostscript_Tiger.svg)_
 
![tiger2](https://user-images.githubusercontent.com/7447159/34709373-8e048286-f549-11e7-8cbc-2941b7b9fa4e.png)

_Agg's result, bitmap zoom-in to see fine details_ 


  ---

The Agg has various customizable scanline rasterizers. 
 You can use its scanline technique it many ways.
 In this library, for example,  It customize the scanline rasterizer to create  
 _lcd-effect subpixel rendering_ effect (see below),
 _msdf3 texture_ (see https://github.com/PaintLab/PixelFarm/issues/55)  etc.
   




  ---
**PixelFarm's Lcd-effect Subpixel Rendering**
 
 The library provides a special scanline rasterizer that produces lcd-effect subpixel rendering output.
 You may need this when you want to make a font-glyph look sharper/easy to read on 
 general computer monitor (96 dpi).
 
   
![lcd_05](https://cloud.githubusercontent.com/assets/7447159/22738636/ceba4840-ee3a-11e6-8cd6-400b9d356fd7.png)

 If you look closely, It not just an anti-alias line, it is lcd-effect subpixel rendering antialias line.
  
![lcd_effect_zoom](https://user-images.githubusercontent.com/7447159/77986599-b978ed80-7341-11ea-9239-4f322af7d305.png)

_zoom view of above picture_
lcd effect subpixel rendering blends a single color to nearby pixels
you can see it not just a simple red or blue line

![lcd_07](https://cloud.githubusercontent.com/assets/7447159/22779712/6e1512c2-eeee-11e6-9352-8c0c4fc1dc95.png)

_black on white, lcd-effect_

With black line on white background, the output is not just black color, It has many color inside it.
Not only line but curves too, below images are lcd-effect on curves of font glyphs.


![lcd_08](https://cloud.githubusercontent.com/assets/7447159/22780442/590abe10-eef1-11e6-93f6-bf4bbcfa3f34.png)

 
![lcd_09](https://cloud.githubusercontent.com/assets/7447159/22780526/a0e65712-eef1-11e6-948a-eca8e8158aaa.png)

![typography_thanamas](https://user-images.githubusercontent.com/7447159/44314099-d4357180-a43e-11e8-95c3-56894bfea1e4.png)

	
![autofit_hfit01](https://cloud.githubusercontent.com/assets/7447159/26182259/282de0f4-3ba1-11e7-83ab-84ac1911526d.png)

The core library does not provide a text-rendering functions directly.  
It provide a 'blank text printer' (abstract) for you.

The library has one example of text printer, see=> PixelFarm.Typography

How to read a font file, layout the glyphs, print to text are special topics.
If you are interested, please visit  _Typography_ (https://github.com/LayoutFarm/Typography).
 

---

The HtmlRenderer example!
---

Now we can render a high quality graphics, we have a high quality font output too.

Why don't we try to render a Web?

 ![htmlbox_gles_with_selection](https://user-images.githubusercontent.com/7447159/49267623-fc952900-f48d-11e8-8ac8-03269c571c2c.png)
 
_HtmlRenderer on GLES2 surface, text are renderered with the Typography, please note the text selection on the Html Surface._  


If you are interested in HtmlRenderer, visit here => https://github.com/LayoutFarm/HtmlRenderer, 
 
---
**HOW TO BUILD**

see https://github.com/PaintLab/PixelFarm/issues/37

---

**SUB PROJECT ARRANGMENT**

see https://github.com/PaintLab/PixelFarm/tree/master/src

---
 
**License:**

The project is based on multiple open-sourced projects (listed below) all using permissive licenses.

A license for a whole project is [**MIT**](https://opensource.org/licenses/MIT).

but if you use some part of the code please check each source file's header for the licensing info.



**Geometry**

BSD, 2002-2005, Maxim Shemanarev, Anti-Grain Geometry - Version 2.4, http://www.antigrain.com

BSD, 2007-2014, Lars Brubaker, agg-sharp, https://github.com/MatterHackers/agg-sharp

ZLIB, 2015, burningmine, CurveUtils. https://github.com/burningmime/curves

Boost, 2010-2014, Angus Johnson, Clipper.

BSD, 2009-2010, Poly2Tri Contributors, https://github.com/PaintLab/poly2tri-cs

SGI, 2000, Eric Veach, Tesselate.

MS-PL, 2018, SVG.NET, https://github.com/vvvv/SVG

MIT, 2018, Rohaan Hamid, https://github.com/rohaanhamid/simplify-csharp

**Image Processing**

MIT, 2008, dotPDN LLC, Rick Brewster, Chris Crosetto, Tom Jackson, Michael Kelsey, Brandon Ortiz, Craig Taylor, Chris Trevino, and Luke Walker., OpenPDN v 3.36.7 (Paint.NET), https://github.com/rivy/OpenPDN

BSD, 2002-2005, Maxim Shemanarev, Anti-Grain Geometry - Version 2.4, http://www.antigrain.com

MIT, 2016, Viktor Chlumsky, https://github.com/Chlumsky/msdfgen

MIT, 2009-2015, Bill Reiss, Rene Schulte and WriteableBitmapEx Contributors, https://github.com/teichgraf/WriteableBitmapEx

Apache2, 2012, Hernán J. González, https://github.com/leonbloy/pngcs

Apache2, 2010, Sebastian Stehle, .NET Image Tools Development Group. , https://imagetools.codeplex.com/ 

MIT, 2018, Tomáš Pažourek, Colourful, https://github.com/tompazourek/Colourful

MIT, 2011, Inedo, https://github.com/Inedo/iconmaker

**Font**

Apache2, 2016-2017, WinterDev, Samuel Carlsson, Sam Hocevar and others, https://github.com/LayoutFarm/Typography

Apache2, 2014-2016, Samuel Carlsson, https://github.com/vidstige/NRasterizer

MIT, 2015, Michael Popoloski, https://github.com/MikePopoloski/SharpFont

The FreeType Project LICENSE (3-clauses BSD style),2003-2016, David Turner, Robert Wilhelm, and Werner Lemberg and others, https://www.freetype.org/

**Platforms**

MIT, 2015-2015, Xamarin, Inc., https://github.com/mono/SkiaSharp

MIT, 2006-2009,  Stefanos Apostolopoulos and other Open Tool Kit Contributors, https://github.com/opentk/opentk

MIT, 2013, Antonie Blom,  https://github.com/andykorth/Pencil.Gaming

**Demo**

MIT, 2017, Wiesław Šoltés, ColorBlender, https://github.com/wieslawsoltes/ColorBlender

BSD, 2015, Darren David darren-code@lookorfeel.com, https://github.com/nobutaka/EasingCurvePresets

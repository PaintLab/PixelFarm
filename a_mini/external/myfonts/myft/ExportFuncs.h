
#include <ft2build.h>
#include <hb.h>
#include <hb-ft.h>
#include FT_FREETYPE_H
#include FT_GLYPH_H
#include FT_OUTLINE_H 
 
#define MY_DLL_EXPORT __declspec(dllexport)  
 
struct ExportGlyph
{
	
	FT_Short unit_per_em;
	FT_Short ascender;
	FT_Short descender;
	FT_Short height;
	
	int advanceX;
	int advanceY;


	//glyph image bounding box
	int bboxXmin;
	int bboxXmax;
	int bboxYmin;
	int bboxYmax;

	int  img_width;
	int  img_height;
	int  img_horiBearingX;
	int  img_horiBearingY;
	int  img_horiAdvance;
	int  img_vertBearingX;
	int  img_vertBearingY;
	int  img_vertAdvance;

	//--------------------
	int bitmap_left;
	int bitmap_top;

	FT_Bitmap* bitmap;
	FT_Outline* outline;
};

struct ExportTypeFaceInfo
{ 
	bool hasKerning;
	hb_font_t *my_hb_ft_font;
    hb_buffer_t *my_hb_buf;
};

struct ProperGlyph
{			 
	 uint32_t codepoint;
	 hb_position_t  x_advance;
     hb_position_t  y_advance;
     hb_position_t  x_offset;
     hb_position_t  y_offset;	 
};

extern "C"{
	MY_DLL_EXPORT int MyFtLibGetVersion();
	MY_DLL_EXPORT int MyFtInitLib();
	MY_DLL_EXPORT void MyFtShutdownLib();

	//MY_DLL_EXPORT int MyFtNewFace(const char* faceName,int pxsize);
	MY_DLL_EXPORT FT_Face MyFtNewMemoryFace(const void* membuffer,int sizeInBytes,int pxsize);	
	MY_DLL_EXPORT void MyFtDoneFace(FT_Face face);
	MY_DLL_EXPORT void MyFtGetFaceInfo(FT_Face face,ExportTypeFaceInfo* exportTypeFaceInfo);


	MY_DLL_EXPORT void MyFtSetPixelSizes(FT_Face myface,int pxsize);
	MY_DLL_EXPORT void MyFtSetCharSize(FT_Face myface,int char_width,int char_height,int h_device_resolution, int v_device_resolution);
		
	MY_DLL_EXPORT int MyFtLoadChar(FT_Face myface,unsigned int charcode,ExportGlyph *expGlyph);
	MY_DLL_EXPORT int MyFtLoadGlyph(FT_Face myface,unsigned int glyphIndex,ExportGlyph *expGlyph);
		
	MY_DLL_EXPORT int MyFtSetupShapingEngine(FT_Face myface,
		const char* langName,int langNameLen,
		int direction,int scriptCode,ExportTypeFaceInfo* exportTypeInfo);

	MY_DLL_EXPORT  int MyFtShaping(hb_font_t *my_hb_ft_font, 
		const uint16_t* text,
		int charCount,
		ProperGlyph* properGlyphs);

}



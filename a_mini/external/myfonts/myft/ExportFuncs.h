
#include <ft2build.h>
#include <hb.h>
#include <hb-ft.h>
#include FT_FREETYPE_H
#include FT_GLYPH_H
#include FT_OUTLINE_H 
 
#define MY_DLL_EXPORT __declspec(dllexport)  



struct ExportTypeFace
{
	
	FT_Short unit_per_em;
	FT_Short ascender;
	FT_Short descender;
	FT_Short height;
	
	int advanceX;
	int advanceY;

	int bboxXmin;
	int bboxXmax;
	int bboxYmin;
	int bboxYmax;

	FT_Bitmap* bitmap;
	FT_Outline* outline;
};


extern "C"{
	MY_DLL_EXPORT int MyFtLibGetVersion();
	MY_DLL_EXPORT int MyFtInitLib();
	MY_DLL_EXPORT void MyFtShutdownLib();

	//MY_DLL_EXPORT int MyFtNewFace(const char* faceName,int pxsize);
	MY_DLL_EXPORT FT_Face MyFtNewMemoryFace(const void* membuffer,int sizeInBytes,int pxsize);
	MY_DLL_EXPORT void MyFtDoneFace(FT_Face face);

	MY_DLL_EXPORT int MyFtLoadChar(FT_Face myface,unsigned int charcode,ExportTypeFace *exportTypeFace);
	MY_DLL_EXPORT int MyFtSetupShapingEngine(FT_Face myface,const char* langName,int langNameLen,int direction,int scriptCode);
	MY_DLL_EXPORT int MyFtShaping(const uint16_t* text,int charCount); 
}



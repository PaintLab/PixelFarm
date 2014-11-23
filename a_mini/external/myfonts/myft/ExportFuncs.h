
#include <ft2build.h>
#include <hb.h>
#include <hb-ft.h>
#include FT_FREETYPE_H
#include FT_GLYPH_H
#include FT_OUTLINE_H 
 
#define MY_DLL_EXPORT __declspec(dllexport)  

extern "C"{
	MY_DLL_EXPORT int MyFtLibGetVersion();
	MY_DLL_EXPORT int MyFtInitLib();
	MY_DLL_EXPORT int MyFtNewFace(const char* faceName,int pxsize);
	MY_DLL_EXPORT int MyFtNewMemoryFace(const void* membuffer,int sizeInBytes,int pxsize);
 
	MY_DLL_EXPORT int MyFtLoadChar(unsigned int charcode,FT_Outline *glyph_outline);
	MY_DLL_EXPORT int MyFtSetupShapingEngine(const char* langName,int langNameLen,int direction);
	MY_DLL_EXPORT int MyFtShaping(const uint16_t* text,int charCount);
}
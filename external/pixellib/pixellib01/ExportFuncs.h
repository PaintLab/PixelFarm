#include "../npixel.h"

#define MY_DLL_EXPORT __declspec(dllexport)  
 
typedef void (*del02)(int oIndex,int methodName);

MY_DLL_EXPORT int LibGetVersion();
MY_DLL_EXPORT int RegisterManagedCallBack(del02 callback,int callBackKind);
MY_DLL_EXPORT int TestCallBack();  
MY_DLL_EXPORT int CallServices(int serviceNumber); 

MY_DLL_EXPORT IBITMAP* MakeBitmapWrapper(int bmpW,int bmpH,int stride,int bpp,void* rawPixelData);


MY_DLL_EXPORT void DrawImage(IVideo* winVideo,
	IBITMAP* bmp,
	int x1,int y1,int x2,int y2);

MY_DLL_EXPORT void DrawImage2(IVideo* winVideo,
	IBITMAP* bmp,
	int x1,int y1,int x2,int y2,int x3,int y3,int x4,int y4);
 
MY_DLL_EXPORT void SetBrushColor(IVideo* winVideo,char r,char g, char b,char a);	 
MY_DLL_EXPORT void SetLineWidth(IVideo* winVideo,double w);
MY_DLL_EXPORT void DrawLine(IVideo* winVideo,double x1,double y1,double x2,double y2);	 


MY_DLL_EXPORT 
IVideo* SetupMainWindow(HWND mainWindow);

MY_DLL_EXPORT int ShutdownMainWindow(HWND importHwnd,IVideo* winVideo	); 
 
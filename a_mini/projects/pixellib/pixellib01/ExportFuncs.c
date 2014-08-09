#include "ExportFuncs.h"  


del02 managedListner;

int main2( ); 
int main4(
	#ifdef __unix
	IVideoPrivateX11* winVideo
#else
	IVideoPrivateWin* winVideo
#endif
	);
int drawing2(
#ifdef __unix
	IVideoPrivateX11* winVideo
#else
	IVideoPrivateWin* winVideo
#endif
	);
int drawing4(
 #ifdef __unix
	IVideoPrivateX11* winVideo
#else
	IVideoPrivateWin* winVideo
#endif
	);

int LibGetVersion()
{	
	return 1;
};
//==================================
int RegisterManagedCallBack(del02 funcPtr,int callbackKind)
{	
	switch(callbackKind)
	{
	case 0:
		{
			//managedListner= (del02)funcPtr; 		 
			managedListner= funcPtr;//static_cast<del02>(funcPtr); 
			return 0;
		}break;
	case 1:
		{
			 
		}break;
	} 
	return 1;
}
int TestCallBack()
{
	managedListner(0,1);
	return 20;
}



int CallServices(
#ifdef __unix
	IVideoPrivateX11* winVideo 
#else
	IVideoPrivateWin* winVideo
#endif
		,int serviceNumber)
{
	switch(serviceNumber)
	{
		case 1:
		{		
			//intit
			//main2();  
			main4(winVideo);
		}break;
		case 2:
	    {   
			iscreen_convert(NULL, 1, winVideo);
			iscreen_update(NULL, 1, winVideo);
		}break;				
		case 3:
	    {
			ShutdownMainWindow(winVideo->hWnd,winVideo);

		}break; 
		case 4:
		{
			drawing2(winVideo);
		}break;
		case 5:
	    {

			drawing4(winVideo);

		}break;

		default:
		{
			
		}
	}
	return 0;
}
 
#ifdef __unix
	IVideoPrivateX11* 
#else
	IVideoPrivateWin* 
#endif
SetupMainWindow(HWND importHwnd)
{   
	IVideoPrivateWin* win = (IVideoPrivateWin*)malloc(sizeof(IVideoPrivateWin));
	memset(win,0,sizeof(IVideoPrivateWin)); 
	win->hWnd = importHwnd;
	 
	return win;
} 
int ShutdownMainWindow(HWND importHwnd,
#ifdef __unix
	IVideoPrivateX11* winVideo 
#else
	IVideoPrivateWin* winVideo
#endif
	)
{	
	iscreen_quit(winVideo);
	 
	return 0;
}
 
//----------------------------------------------------
 
int myUseExternalWindow;

int DrawImage( 
#ifdef __unix
	IVideoPrivateX11* winVideo 
#else
	IVideoPrivateWin* winVideo
#endif
	,
	IBITMAP* bmp,	
	int x1,int y1,int x2,int y2
	)
{	 
	IBITMAP *picture;
	ipixel_point_t pts[4] = 
		{ { x1, y2 }, { x1, y1 }, { x2, y1 }, { x2, y2 } };

	ipaint_raster(winVideo->paint, pts, bmp, NULL, 0xffffffff, 0);
	
	//ipaint_raster(winVideo->paint, pts, picture, NULL, 0xffffffff, 0);
	return 0;
}
 
IBITMAP* MakeBitmapWrapper(int bmpW,int bmpH,int stride, int bpp,void* rawPixelSrcData)
{
	IBITMAP* bmp= ibitmap_create(bmpW,bmpH,bpp);
	//copy data from src to new destination**
	memcpy_s(bmp->pixel,(bmpH * stride),rawPixelSrcData,(bmpH * stride));
	return bmp;
}
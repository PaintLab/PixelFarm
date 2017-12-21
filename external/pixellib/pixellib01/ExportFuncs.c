#include "ExportFuncs.h"  


del02 managedListner;

int main2(); 
int main4(IVideo* winVideo);
int drawing2(IVideo* winVideo);
int drawing4(IVideo* winVideo);

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



int CallServices( IVideo* winVideo,int serviceNumber)
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
 
IVideo* SetupMainWindow(HWND importHwnd)
{   
	IVideo* win = (IVideo*)malloc(sizeof(IVideo));
	memset(win,0,sizeof(IVideo)); 
	win->hWnd = importHwnd;
	 
	return win;
} 
int ShutdownMainWindow(HWND importHwnd,IVideo* winVideo)
{	
	iscreen_quit(winVideo);
	 
	return 0;
}
 
//----------------------------------------------------
 
int myUseExternalWindow;

void  DrawImage(IVideo* winVideo,IBITMAP* bmp,	
	int x1,int y1,int x2,int y2
	)
{	 
	 
	ipixel_point_t pts[4] = 
		{ { x1, y2 }, { x1, y1 }, { x2, y1 }, { x2, y2 } }; 
	ipaint_raster(winVideo->paint, pts, bmp, NULL, 0xffffffff, 0); 
	//ipaint_raster(winVideo->paint, pts, picture, NULL, 0xffffffff, 0);
	 
}

void DrawImage2(IVideo* winVideo,
	IBITMAP* bmp,
	int x1,int y1,int x2,int y2,int x3,int y3,int x4,int y4){	
	
		ipixel_point_t pts[4] = 
		{ { x1, y1 }, { x2, y2 }, { x3, y3 }, { x4, y4 } };  
	    ipaint_raster(winVideo->paint, pts, bmp, NULL, 0xffffffff, 0);  
}


IBITMAP* MakeBitmapWrapper(int bmpW,int bmpH,int stride, int bpp,void* rawPixelSrcData)
{
	IBITMAP* bmp= ibitmap_create(bmpW,bmpH,bpp);
	//copy data from src to new destination**
	memcpy_s(bmp->pixel,(bmpH * stride),rawPixelSrcData,(bmpH * stride));
	return bmp;
}
 
void SetBrushColor(IVideo* winVideo,char r,char g, char b,char a){
	ipaint_set_color(winVideo->paint, IRGBA_TO_A8R8G8B8(r, g, b, a));
}
void  SetLineWidth(IVideo* winVideo,double w){
	ipaint_line_width(winVideo->paint, w); 
}
void DrawLine(IVideo* winVideo,double x1,double y1,double x2,double y2){
	ipaint_draw_line(winVideo->paint, x1, y1, x2,y2);
}
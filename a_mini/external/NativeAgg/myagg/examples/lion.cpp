#include <stdlib.h>
#include <ctype.h>
#include <stdio.h>
#include "agg_basics.h"
#include "agg_rendering_buffer.h"
#include "agg_rasterizer_scanline_aa.h"
#include "agg_scanline_p.h"
#include "agg_renderer_scanline.h"
#include "agg_path_storage.h"
#include "agg_conv_transform.h"
#include "agg_bounding_rect.h"
#include "ctrl/agg_slider_ctrl.h"
#include "platform/agg_platform_support.h"
#include "../BasicCanvas.h"

//#define AGG_GRAY16
//#define AGG_BGR24
//#define AGG_BGR48 
//#define AGG_RGB_AAA
#define AGG_BGRA32
//#define AGG_RGBA32 
//#define AGG_ARGB32 
//#define AGG_ABGR32
//#define AGG_RGB565
//#define AGG_RGB555
#include "pixel_formats.h"

enum flip_y_e { flip_y = true };  

unsigned parse_lion(agg::path_storage& ps, agg::rgba8* colors, unsigned* path_idx); 
unsigned parse_lion2(void* mem,int len,int start,agg::path_storage& ps, agg::rgba8* colors, unsigned* path_idx); 
 

class MyBasicCanvas ;

class BasicSprite
{
public:
	 
	unsigned          g_npaths; 
	double            g_base_dx ;
	double            g_base_dy ;
	double            g_angle;
	double            g_scale;//= 1.0;
	double            g_skew_x;// = 0;
	double            g_skew_y;// = 0; 
	agg::path_storage g_path;  
	agg::rgba8        g_colors[100];
	unsigned          g_path_idx[100]; 

	void drawSprite(MyBasicCanvas* c);
	void init();
	void transform(double width, double height, double x, double y)
    {
        x -= width / 2;
        y -= height / 2;
        g_angle = atan2(y, x);
        g_scale = sqrt(y * y + x * x) / 100.0;
    } 
	void move(double x,double y)
	{
		g_base_dx =x;
		g_base_dy =y;
		 
	}
	void loadSpriteData()
	{
		parse_lion1();
	}

	agg::path_storage getInternalPathStore()
	{
		return g_path;
	}

private:
	void parse_lion1(); 

};

typedef agg::renderer_base<pixfmt> renderer_base;
typedef agg::renderer_scanline_aa_solid<renderer_base> renderer_solid;


class MyBasicCanvas : public agg::BasicCanvas
{ 

private: 	
	unsigned          g_npaths; 
	double            g_base_dx ;
	double            g_base_dy ;
	double            g_angle;
	double            g_scale;//= 1.0;
	double            g_skew_x;// = 0;
	double            g_skew_y;// = 0;  

	//BasicSprite sp01;
	//BasicSprite sp02;
	bool canvasInit;
 

	pixfmt* m_pxfmt;
	renderer_base* m_rb; 
	renderer_solid* m_r;
	
public: 
	agg::rasterizer_scanline_aa<> g_rasterizer; 
	agg::scanline_p8  g_scanline; 
	agg::path_storage g_path;  
	agg::rgba8        g_colors[100];
	unsigned          g_path_idx[100]; 
	
	renderer_solid myRenderSolid;

    MyBasicCanvas(agg::pix_format_e format, bool flip_y) :
        agg::BasicCanvas(format, flip_y) 
    {
			
			//init ------------
			g_npaths=g_base_dx =g_base_dy=g_angle=0;  
            g_skew_x=0;// = 0;
            g_skew_y=0;// = 0;  
			g_scale = 1.0;  
			memset(&g_scanline,0,sizeof(agg::scanline_p8));
			memset(&g_path,0,sizeof(agg::path_storage));
			memset(g_colors,0,sizeof(agg::rgba8) *100);
			memset(g_path_idx,0,sizeof(unsigned) *100);
			//memset(&sp01,0,sizeof(BasicSprite));
			//init ------------ 
		    canvasInit= false;
			 
			//pixfmt pixf(rbuf_window());//get pixel format
			//renderer_base rb(pixf); //init base render with specific format
			//renderer_solid r(rb);//init solid renderer with specific base renderer 
			//this->myRenderSolid = r;
			
			//-------------------
			//sp01.init(); 
			//sp01.move(400,380);
			//sp01.transform(100,100,0,0); 
			//-------------------
			//sp02.init();
			//sp02.move(200,380);
			//sp02.transform(200,200,50,50);
			//-------------------

    }	
    ~MyBasicCanvas()
	{  
		dispose();		 
	}
	void dispose()
	{
		delete m_pxfmt;m_pxfmt=0;
		delete m_rb; m_rb=0;
		delete m_r;m_r=0;
		canvasInit= false;
	}
    virtual void on_resize(int cx, int cy)
    {
        pixfmt pf(rbuf_window());
        renderer_base r(pf);
        r.clear(agg::rgba(1, 1, 1)); 
    }  
    virtual void on_draw()
    {	
		//-------------------------
		//clearCanvas();
		//-------------------------
		//pixfmt pixf(rbuf_window());//get pixel format
		//renderer_base rb(pixf); //init base render with specific format
		//renderer_solid r(rb);//init solid renderer with specific base renderer 
		//this->myRenderSolid = r;
		if(!canvasInit){
			this->m_pxfmt = new pixfmt(rbuf_window());
			this->m_rb = new renderer_base(*m_pxfmt);
			this->m_r = new renderer_solid(*this->m_rb);
			this->myRenderSolid = *m_r;
			canvasInit=true;
		} 
	 
		 //pixfmt pixf(rbuf_window());//get pixel format
		//renderer_base rb(pixf); //init base render with specific format
		//renderer_solid r(rb);//init solid renderer with specific base renderer 
		//this->myRenderSolid = r;
		//sp01.drawSprite(this);
		//sp02.drawSprite(this); 

    }
	void clearCanvas()
	{	 
		agg::rendering_buffer renderBuff = rbuf_window(); 

		if(!canvasInit){
			this->m_pxfmt = new pixfmt(renderBuff);
			this->m_rb = new renderer_base(*m_pxfmt);
			this->m_r = new renderer_solid(*this->m_rb);
			this->myRenderSolid = *m_r;
			canvasInit=true;
		} 



        int width = renderBuff.width();
        int height = renderBuff.height(); 
		//---------------------------------------
		//clear canvas bg
		renderBuff.clear(agg::int8u(255));
		//--------------------------------------- 
	}
    void transform(double width, double height, double x, double y)
    {
        x -= width / 2;
        y -= height / 2;
        g_angle = atan2(y, x);
        g_scale = sqrt(y * y + x * x) / 100.0;
    } 
	 
}; 

//======================================================
void BasicSprite::init()
{	 
	g_npaths=g_base_dx =g_base_dy=g_angle=0;  
    g_skew_x=0;// = 0;
    g_skew_y=0;// = 0;  
	g_scale = 1.0;  
	 
	memset(&g_path,0,sizeof(agg::path_storage));
	memset(g_colors,0,sizeof(agg::rgba8) *100);
	memset(g_path_idx,0,sizeof(unsigned) *100);	 
	  
	////parse_lion1();
	//loadSpriteData();

	//for(unsigned i = 0; i < g_npaths; i++)
 //   {
 //       g_colors[i].a = 0.4* 255;// agg::int8u(m_alpha_slider.value() * 255);
 //   }
};

void BasicSprite::drawSprite(MyBasicCanvas* canvas)
{	
	int width=50;
	int height=50;

    agg::trans_affine mtx;
    mtx *= agg::trans_affine_translation(-g_base_dx, -g_base_dy);
    mtx *= agg::trans_affine_scaling(g_scale, g_scale);
    mtx *= agg::trans_affine_rotation(g_angle + agg::pi);
    mtx *= agg::trans_affine_skewing(g_skew_x/1000.0, g_skew_y/1000.0);
    mtx *= agg::trans_affine_translation(width/2, height/2);
	
	
	agg::conv_transform<agg::path_storage, agg::trans_affine> trans(g_path, mtx);
    //agg::render_all_paths(g_rasterizer, g_scanline, r, trans, g_colors, g_path_idx, g_npaths);
	agg::render_all_paths(canvas->g_rasterizer, canvas->g_scanline, canvas->myRenderSolid, trans, g_colors, g_path_idx, g_npaths);
};

void BasicSprite::parse_lion1()
{	
		double g_x1=0,g_y1=0,g_x2=0,g_y2=0;         
		g_npaths = parse_lion(g_path, g_colors, g_path_idx);
		agg::pod_array_adaptor<unsigned> path_idx(g_path_idx, 100);
		agg::bounding_rect(g_path, path_idx, 0, g_npaths, &g_x1, &g_y1, &g_x2, &g_y2);
		g_base_dx = (g_x2 - g_x1) / 2.0;
		g_base_dy = (g_y2 - g_y1) / 2.0;
};

//int agg_main(int argc, char* argv[])
//{
//    MyBasicCanvas app(pix_format, flip_y);
//    app.caption(L"AGG Example. Lion\0");
//
//    if(app.init(512, 400, agg::window_resize))
//    {
//        return app.run();
//    }
//    return 1;
//}
//int agg_main2(int argc, char* argv[])
//{
//    MyBasicCanvas app(pix_format, flip_y);
//    app.caption(L"AGG Example. Lion\0");
//
//    if(app.init(512, 400, agg::window_resize))
//    {
//        //return app.run();
//    }
//    return 1;
//}

#define MY_DLL_EXPORT __declspec(dllexport)  

extern "C"{
	MY_DLL_EXPORT void* AggCreateCanvas(){ 

	    MyBasicCanvas* mycanvas = new MyBasicCanvas(pix_format, flip_y); 
		if(mycanvas->init(512, 400, agg::window_resize))
		{	 
		    return mycanvas;
		}
		return 0;
	}
	MY_DLL_EXPORT void CanvasClearBackground(void* canvas)
	{ 

		MyBasicCanvas* basicCanvas = (MyBasicCanvas*)canvas;
		basicCanvas->clearCanvas();

	}
	MY_DLL_EXPORT void* CreateSprite()
	{ 
	     
		 BasicSprite* basicSprite= new BasicSprite();		  
		 basicSprite->init();
		 basicSprite->loadSpriteData();

		 return basicSprite;
	}
	//MY_DLL_EXPORT void SpriteDraw(void* sprite,void* canvas)
	MY_DLL_EXPORT void SpriteDraw(BasicSprite* basicSprite,MyBasicCanvas* basicCanvas)
	{
		//draw sprite to canvas
		/*BasicSprite* basicSprite= (BasicSprite*)sprite;
		MyBasicCanvas* basicCanvas = (MyBasicCanvas*)canvas;*/
		basicSprite->drawSprite(basicCanvas);		
	}
	MY_DLL_EXPORT void SpriteMove(BasicSprite* basicSprite,double x,double y)
	{
		//draw sprite to canvas*   
		basicSprite->move(x,y);
	}
	MY_DLL_EXPORT agg::path_storage SpriteGetInternalPathStore(BasicSprite* basicSprite)
	{
		//draw sprite to canvas 
		return basicSprite->getInternalPathStore();
	} 
    MY_DLL_EXPORT void ParsePathDataStream(char* buffer,int startAt,int len)
	{
		//parse stream to path command 
	} 
	MY_DLL_EXPORT void AggApp_Move(MyBasicCanvas* app,float x, float y)
	{ 		 
	  
		app->transform(150,150,x,y);
	} 


}

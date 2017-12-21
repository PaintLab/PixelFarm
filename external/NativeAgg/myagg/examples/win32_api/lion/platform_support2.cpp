//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// class platform_support
//
//----------------------------------------------------------------------------

#include <windows.h>
#include <string.h>
#include "platform/agg_platform_support.h"
#include "platform/win32/agg_win32_bmp.h"
#include "util/agg_color_conv_rgb8.h"
#include "util/agg_color_conv_rgb16.h"
#include <wchar.h>
#include "../../../BasicCanvas.h"


namespace agg
{
    
    
    class platform_specific
    {
    public:
        platform_specific(pix_format_e format, bool flip_y);

        void create_pmap(unsigned width, unsigned height, 
                         rendering_buffer* wnd);

        void display_pmap(HDC dc, const rendering_buffer* src);
        bool load_pmap(const char* fn, unsigned idx, 
                       rendering_buffer* dst);

        bool save_pmap(const char* fn, unsigned idx, 
                       const rendering_buffer* src); 

        pix_format_e  m_format;
        pix_format_e  m_sys_format;
        bool          m_flip_y;
        unsigned      m_bpp;
        unsigned      m_sys_bpp;
        //HWND          m_hwnd;
        pixel_map     m_pmap_window;
        pixel_map     m_pmap_img[platform_support::max_images];
        //unsigned      m_keymap[256];
        //unsigned      m_last_translated_key;
        //int           m_cur_x;
        //int           m_cur_y;
        //unsigned      m_input_flags;
        bool          m_redraw_flag;
        //HDC           m_current_dc;
        LARGE_INTEGER m_sw_freq;
        LARGE_INTEGER m_sw_start;
    };


    //------------------------------------------------------------------------
    platform_specific::platform_specific(pix_format_e format, bool flip_y) :
        m_format(format),
        m_sys_format(pix_format_undefined),
        m_flip_y(flip_y),
        m_bpp(0),
        m_sys_bpp(0),
        m_redraw_flag(true) 
    {
          
        switch(m_format)
        {
        case pix_format_bw:
            m_sys_format = pix_format_bw;
            m_bpp = 1;
            m_sys_bpp = 1;
            break;

        case pix_format_gray8:
            m_sys_format = pix_format_gray8;
            m_bpp = 8;
            m_sys_bpp = 8;
            break;

        case pix_format_gray16:
            m_sys_format = pix_format_gray8;
            m_bpp = 16;
            m_sys_bpp = 8;
            break;

        case pix_format_rgb565:
        case pix_format_rgb555:
            m_sys_format = pix_format_rgb555;
            m_bpp = 16;
            m_sys_bpp = 16;
            break;

        case pix_format_rgbAAA:
        case pix_format_bgrAAA:
        case pix_format_rgbBBA:
        case pix_format_bgrABB:
            m_sys_format = pix_format_bgr24;
            m_bpp = 32;
            m_sys_bpp = 24;
            break;

        case pix_format_rgb24:
        case pix_format_bgr24:
            m_sys_format = pix_format_bgr24;
            m_bpp = 24;
            m_sys_bpp = 24;
            break;

        case pix_format_rgb48:
        case pix_format_bgr48:
            m_sys_format = pix_format_bgr24;
            m_bpp = 48;
            m_sys_bpp = 24;
            break;

        case pix_format_bgra32:
        case pix_format_abgr32:
        case pix_format_argb32:
        case pix_format_rgba32:
            m_sys_format = pix_format_bgra32;
            m_bpp = 32;
            m_sys_bpp = 32;
            break;

        case pix_format_bgra64:
        case pix_format_abgr64:
        case pix_format_argb64:
        case pix_format_rgba64:
            m_sys_format = pix_format_bgra32;
            m_bpp = 64;
            m_sys_bpp = 32;
            break;
        }
        ::QueryPerformanceFrequency(&m_sw_freq);
        ::QueryPerformanceCounter(&m_sw_start);
    }


    //------------------------------------------------------------------------
    void platform_specific::create_pmap(unsigned width, 
                                        unsigned height,
                                        rendering_buffer* wnd)
    {
        m_pmap_window.create(width, height, org_e(m_bpp));
        wnd->attach(m_pmap_window.buf(), 
                    m_pmap_window.width(),
                    m_pmap_window.height(),
                      m_flip_y ?
                      m_pmap_window.stride() :
                     -m_pmap_window.stride());
    }


    //------------------------------------------------------------------------
    static void convert_pmap(rendering_buffer* dst, 
                             const rendering_buffer* src, 
                             pix_format_e format)
    {
        switch(format)
        {
        case pix_format_gray8:
            break;

        case pix_format_gray16:
            color_conv(dst, src, color_conv_gray16_to_gray8());
            break;

        case pix_format_rgb565:
            color_conv(dst, src, color_conv_rgb565_to_rgb555());
            break;

        case pix_format_rgbAAA:
            color_conv(dst, src, color_conv_rgbAAA_to_bgr24());
            break;

        case pix_format_bgrAAA:
            color_conv(dst, src, color_conv_bgrAAA_to_bgr24());
            break;

        case pix_format_rgbBBA:
            color_conv(dst, src, color_conv_rgbBBA_to_bgr24());
            break;

        case pix_format_bgrABB:
            color_conv(dst, src, color_conv_bgrABB_to_bgr24());
            break;

        case pix_format_rgb24:
            color_conv(dst, src, color_conv_rgb24_to_bgr24());
            break;

        case pix_format_rgb48:
            color_conv(dst, src, color_conv_rgb48_to_bgr24());
            break;

        case pix_format_bgr48:
            color_conv(dst, src, color_conv_bgr48_to_bgr24());
            break;

        case pix_format_abgr32:
            color_conv(dst, src, color_conv_abgr32_to_bgra32());
            break;

        case pix_format_argb32:
            color_conv(dst, src, color_conv_argb32_to_bgra32());
            break;

        case pix_format_rgba32:
            color_conv(dst, src, color_conv_rgba32_to_bgra32());
            break;

        case pix_format_bgra64:
            color_conv(dst, src, color_conv_bgra64_to_bgra32());
            break;

        case pix_format_abgr64:
            color_conv(dst, src, color_conv_abgr64_to_bgra32());
            break;

        case pix_format_argb64:
            color_conv(dst, src, color_conv_argb64_to_bgra32());
            break;

        case pix_format_rgba64:
            color_conv(dst, src, color_conv_rgba64_to_bgra32());
            break;
        }
    }


    //------------------------------------------------------------------------
    void platform_specific::display_pmap(HDC dc, const rendering_buffer* src)
    {
        if(m_sys_format == m_format)
        {
            m_pmap_window.draw(dc);
        }
        else
        {
            pixel_map pmap_tmp;
            pmap_tmp.create(m_pmap_window.width(), 
                            m_pmap_window.height(),
                            org_e(m_sys_bpp));

            rendering_buffer rbuf_tmp;
            rbuf_tmp.attach(pmap_tmp.buf(),
                            pmap_tmp.width(),
                            pmap_tmp.height(),
                            m_flip_y ?
                              pmap_tmp.stride() :
                             -pmap_tmp.stride());

            convert_pmap(&rbuf_tmp, src, m_format);
            pmap_tmp.draw(dc);
        }
    }



    //------------------------------------------------------------------------
    bool platform_specific::save_pmap(const char* fn, unsigned idx, 
                                      const rendering_buffer* src)
    {
        if(m_sys_format == m_format)
        {
            return m_pmap_img[idx].save_as_bmp(fn);
        }

        pixel_map pmap_tmp;
        pmap_tmp.create(m_pmap_img[idx].width(), 
                          m_pmap_img[idx].height(),
                          org_e(m_sys_bpp));

        rendering_buffer rbuf_tmp;
        rbuf_tmp.attach(pmap_tmp.buf(),
                          pmap_tmp.width(),
                          pmap_tmp.height(),
                          m_flip_y ?
                          pmap_tmp.stride() :
                          -pmap_tmp.stride());

        convert_pmap(&rbuf_tmp, src, m_format);
        return pmap_tmp.save_as_bmp(fn);
    }



    //------------------------------------------------------------------------
    bool platform_specific::load_pmap(const char* fn, unsigned idx, 
                                      rendering_buffer* dst)
    {
        pixel_map pmap_tmp;
        if(!pmap_tmp.load_from_bmp(fn)) return false;

        rendering_buffer rbuf_tmp;
        rbuf_tmp.attach(pmap_tmp.buf(),
                        pmap_tmp.width(),
                        pmap_tmp.height(),
                        m_flip_y ?
                          pmap_tmp.stride() :
                         -pmap_tmp.stride());

        m_pmap_img[idx].create(pmap_tmp.width(), 
                               pmap_tmp.height(), 
                               org_e(m_bpp),
                               0);

        dst->attach(m_pmap_img[idx].buf(),
                    m_pmap_img[idx].width(),
                    m_pmap_img[idx].height(),
                    m_flip_y ?
                       m_pmap_img[idx].stride() :
                      -m_pmap_img[idx].stride());

        switch(m_format)
        {
        case pix_format_gray8:
            switch(pmap_tmp.bpp())
            {
            //case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_gray8()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_gray8()); break;
            //case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_gray8()); break;
            }
            break;

        case pix_format_gray16:
            switch(pmap_tmp.bpp())
            {
            //case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_gray16()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_gray16()); break;
            //case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_gray16()); break;
            }
            break;

        case pix_format_rgb555:
            switch(pmap_tmp.bpp())
            {
            case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_rgb555()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_rgb555()); break;
            case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_rgb555()); break;
            }
            break;

        case pix_format_rgb565:
            switch(pmap_tmp.bpp())
            {
            case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_rgb565()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_rgb565()); break;
            case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_rgb565()); break;
            }
            break;

        case pix_format_rgb24:
            switch(pmap_tmp.bpp())
            {
            case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_rgb24()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_rgb24()); break;
            case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_rgb24()); break;
            }
            break;

        case pix_format_bgr24:
            switch(pmap_tmp.bpp())
            {
            case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_bgr24()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_bgr24()); break;
            case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_bgr24()); break;
            }
            break;

        case pix_format_rgb48:
            switch(pmap_tmp.bpp())
            {
            //case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_rgb48()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_rgb48()); break;
            //case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_rgb48()); break;
            }
            break;

        case pix_format_bgr48:
            switch(pmap_tmp.bpp())
            {
            //case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_bgr48()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_bgr48()); break;
            //case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_bgr48()); break;
            }
            break;

        case pix_format_abgr32:
            switch(pmap_tmp.bpp())
            {
            case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_abgr32()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_abgr32()); break;
            case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_abgr32()); break;
            }
            break;

        case pix_format_argb32:
            switch(pmap_tmp.bpp())
            {
            case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_argb32()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_argb32()); break;
            case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_argb32()); break;
            }
            break;

        case pix_format_bgra32:
            switch(pmap_tmp.bpp())
            {
            case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_bgra32()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_bgra32()); break;
            case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_bgra32()); break;
            }
            break;

        case pix_format_rgba32:
            switch(pmap_tmp.bpp())
            {
            case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_rgba32()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_rgba32()); break;
            case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_rgba32()); break;
            }
            break;

        case pix_format_abgr64:
            switch(pmap_tmp.bpp())
            {
            //case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_abgr64()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_abgr64()); break;
            //case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_abgr64()); break;
            }
            break;

        case pix_format_argb64:
            switch(pmap_tmp.bpp())
            {
            //case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_argb64()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_argb64()); break;
            //case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_argb64()); break;
            }
            break;

        case pix_format_bgra64:
            switch(pmap_tmp.bpp())
            {
            //case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_bgra64()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_bgra64()); break;
            //case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_bgra64()); break;
            }
            break;

        case pix_format_rgba64:
            switch(pmap_tmp.bpp())
            {
            //case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_rgba64()); break;
            case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_rgba64()); break;
            //case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_rgba64()); break;
            }
            break;

        }

        return true;
    } 
     
     
    //------------------------------------------ 
	//basic sprite implementation
	//------------------------------------------
    BasicCanvas::BasicCanvas(pix_format_e format, bool flip_y) :
        m_specific(new platform_specific(format, flip_y)),
        m_format(format),
        m_bpp(m_specific->m_bpp), 
        m_flip_y(flip_y),
        m_initial_width(10),
        m_initial_height(10)
    {

        //wcscpy_s(m_caption,wcslen(L"Anti-Grain Geometry Application"), L"Anti-Grain Geometry Application"); 
    } 
    //------------------------------------------------------------------------
    BasicCanvas::~BasicCanvas()
    {
        delete m_specific;
    } 
       
    bool BasicCanvas::init(unsigned width, unsigned height, unsigned flags)
    {
        if(m_specific->m_sys_format == pix_format_undefined)
        {
            return false;
        }  
        m_specific->create_pmap(width, height, &m_rbuf_window);
        m_initial_width = width;
        m_initial_height = height;
        on_init();
        m_specific->m_redraw_flag = true; 
        return true;
    } 
    //------------------------------------------------------------------------
    const char* BasicCanvas::img_ext() const { return ".bmp"; } 
	 
    //------------------------------------------------------------------------
    bool BasicCanvas::load_img(unsigned idx, const char* file)
    {
       if(idx < max_images)
        {
            char fn[1024];
            strcpy(fn, file);
            int len = strlen(fn);
            if(len < 4 || stricmp(fn + len - 4, ".BMP") != 0)
            {
                strcat(fn, ".bmp");
            }
            return m_specific->load_pmap(fn, idx, &m_rbuf_img[idx]);
        } 
        return true;
    } 
    //------------------------------------------------------------------------
    bool BasicCanvas::save_img(unsigned idx, const char* file)
    {
        if(idx < max_images)
        {
            char fn[1024];
            strcpy(fn, file);
            int len = strlen(fn);
            if(len < 4 || stricmp(fn + len - 4, ".BMP") != 0)
            {
                strcat(fn, ".bmp");
            }
            return m_specific->save_pmap(fn, idx, &m_rbuf_img[idx]);
        } 
        return true;
    } 
    //------------------------------------------------------------------------
    bool BasicCanvas::create_img(unsigned idx, unsigned width, unsigned height)
    {
        if(idx < max_images)
        {
            if(width  == 0) width  = m_specific->m_pmap_window.width();
            if(height == 0) height = m_specific->m_pmap_window.height();
            m_specific->m_pmap_img[idx].create(width, height, org_e(m_specific->m_bpp));
            m_rbuf_img[idx].attach(m_specific->m_pmap_img[idx].buf(), 
                                   m_specific->m_pmap_img[idx].width(),
                                   m_specific->m_pmap_img[idx].height(),
                                   m_flip_y ?
                                    m_specific->m_pmap_img[idx].stride() :
                                   -m_specific->m_pmap_img[idx].stride());
            return true;
        }
        return false;
    } 
    //------------------------------------------------------------------------
    void BasicCanvas::on_init() {}
	void BasicCanvas::on_draw() {}
  
} 
extern "C"{
	 
	#define MY_DLL_EXPORT __declspec(dllexport)  
	MY_DLL_EXPORT int AggApp_Paint(void* i_app,HDC  paintDC){ 
			
			PAINTSTRUCT ps;   
			agg::BasicCanvas* app = (agg::BasicCanvas*)i_app;   
			//app->m_specific->m_redraw_flag= true;
            app->on_draw(); 
            app->m_specific->display_pmap(paintDC, &app->rbuf_window()); 
		return 0;
	} 

} 
 
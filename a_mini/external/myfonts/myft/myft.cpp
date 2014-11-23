// myft.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <ft2build.h>
#include <hb.h>
#include <hb-ft.h>
#include FT_FREETYPE_H
#include FT_GLYPH_H
#include FT_OUTLINE_H 

typedef struct _spanner_baton_t {
    /* rendering part - assumes 32bpp surface */
    uint32_t *pixels; // set to the glyph's origin.
    uint32_t *first_pixel, *last_pixel; // bounds check
    uint32_t pitch;
    uint32_t rshift;
    uint32_t gshift;
    uint32_t bshift;
    uint32_t ashift;

    /* sizing part */
    int min_span_x;
    int max_span_x;
    int min_y;
    int max_y;
} spanner_baton_t;

/* google this */
#ifndef unlikely
#define unlikely
#endif

/* This spanner is write only, suitable for write-only mapped buffers,
   but can cause dark streaks where glyphs overlap, like in arabic scripts.

   Note how spanners don't clip against surface width - resize the window
   and see what it leads to. */
void spanner_wo(int y, int count, const FT_Span* spans, void *user) {
    spanner_baton_t *baton = (spanner_baton_t *) user;
    uint32_t *scanline = baton->pixels - y * ( (int) baton->pitch / 4 );
    if (unlikely scanline < baton->first_pixel)
        return;
    for (int i = 0; i < count; i++) {
        uint32_t color =
            ((spans[i].coverage/2) << baton->rshift) |
            ((spans[i].coverage/2) << baton->gshift) |
            ((spans[i].coverage/2) << baton->bshift);

        uint32_t *start = scanline + spans[i].x;
        if (unlikely start + spans[i].len > baton->last_pixel)
            return;

        for (int x = 0; x < spans[i].len; x++)
            *start++ = color;
    }
}

/* This spanner does read/modify/write, trading performance for accuracy.
   The color here is simply half coverage value in all channels,
   effectively mid-gray.
   Suitable for when artifacts mostly do come up and annoy.
   This might be optimized if one does rmw only for some values of x.
   But since the whole buffer has to be rw anyway, and the previous value
   is probably still in the cache, there's little point to. */
void spanner_rw(int y, int count, const FT_Span* spans, void *user) {
    spanner_baton_t *baton = (spanner_baton_t *) user;
    uint32_t *scanline = baton->pixels - y * ( (int) baton->pitch / 4 );
    if (unlikely scanline < baton->first_pixel)
        return;

    for (int i = 0; i < count; i++) {
        uint32_t color =
            ((spans[i].coverage/2)  << baton->rshift) |
            ((spans[i].coverage/2) << baton->gshift) |
            ((spans[i].coverage/2) << baton->bshift);
        uint32_t *start = scanline + spans[i].x;
        if (unlikely start + spans[i].len > baton->last_pixel)
            return;

        for (int x = 0; x < spans[i].len; x++)
            *start++ |= color;
    }
}

/*  This spanner is for obtaining exact bounding box for the string.
    Unfortunately this can't be done without rendering it (or pretending to).
    After this runs, we get min and max values of coordinates used.
*/
void spanner_sizer(int y, int count, const FT_Span* spans, void *user) {
    spanner_baton_t *baton = (spanner_baton_t *) user;

    if (y < baton->min_y)
        baton->min_y = y;
    if (y > baton->max_y)
        baton->max_y = y;
    for (int i = 0 ; i < count; i++) {
        if (spans[i].x + spans[i].len > baton->max_span_x)
            baton->max_span_x = spans[i].x + spans[i].len;
        if (spans[i].x < baton->min_span_x)
            baton->min_span_x = spans[i].x;
    }
}

FT_SpanFunc spanner = spanner_wo;
void ftfdump(FT_Face ftf)
{
	for(int i=0;i<ftf->num_charmaps;++i)
	{
		printf("%d:  %s %s  %c%c%c%c  plat=%hu id=%hu\n",i,
			ftf->family_name,
			ftf->style_name,
			ftf->charmaps[i]->encoding >>24,
			(ftf->charmaps[i]->encoding >>16 ) &0xff,
			(ftf->charmaps[i]->encoding >>8) & 0xff,
			(ftf->charmaps[i]->encoding) & 0xff,
			ftf->charmaps[i]->platform_id,
			ftf->charmaps[i]->encoding_id);

	}
}
int force_ucs2_charmap(FT_Face ftf)
{
	for(int i=0;i< ftf->num_charmaps;++i)
	{
		if (  ((ftf->charmaps[i]->platform_id == 0) &&
			  (ftf->charmaps[i]->platform_id == 3)) ||
			  ((ftf->charmaps[i]->platform_id == 3) &&
			  (ftf->charmaps[i]->platform_id == 1)))
		{
			return FT_Set_Charmap(ftf,ftf->charmaps[i]);
		}		
	}
	return -1;
}
 
int console_main(int argc, _TCHAR* argv[])
{	
	printf("hello!");

	FT_Library ft;
	if(FT_Init_FreeType(&ft))
	{

		fprintf(stderr,"Could not init freetype lib\n");
		return 0;
	}
	//---------------------------------------------------
	//load font
	FT_Face face;
	if(FT_New_Face(ft,"c:\\Windows\\Fonts\\Tahoma.ttf",0,&face))
	{
		fprintf(stderr,"Could not openfont");
		return 0;
	}
	//---------------------------------------------------
	//load char
	FT_Set_Pixel_Sizes(face,0,48);

	if(FT_Load_Char(face,'X',FT_LOAD_RENDER))
	{
		fprintf(stderr,"Could not load character 'X'\n");
		return 0;
	}
	
	FT_GlyphSlot g= face->glyph;
	int w= g->bitmap.width;

	//--------------------------------------------------------
	ftfdump(face);
	force_ucs2_charmap(face);
	//--------------------------------------------------------
	//barfbuzz font structs
	hb_font_t *hb_ft_font;
	hb_ft_font = hb_ft_font_create(face,NULL);

	//--create a buffer for harfbuzz to use
	hb_buffer_t *buf= hb_buffer_create();
	hb_buffer_set_direction(buf,HB_DIRECTION_LTR);
	hb_buffer_set_script(buf,HB_SCRIPT_LATIN);
	hb_buffer_set_language(buf,hb_language_from_string("en",strlen("en")));
	
	//layout the text
	auto text= "BiAQ7";
	hb_buffer_add_utf8(buf,text,strlen(text),0,strlen(text));
	hb_shape(hb_ft_font,buf,NULL,0);

	unsigned int glyph_count;
	hb_glyph_info_t *glyph_info = hb_buffer_get_glyph_infos(buf,&glyph_count);
	hb_glyph_position_t *glyph_pos= hb_buffer_get_glyph_positions(buf,&glyph_count);


	  spanner_baton_t stuffbaton;
	  
            FT_Raster_Params ftr_params;
            ftr_params.target = 0;
            ftr_params.flags = FT_RASTER_FLAG_DIRECT | FT_RASTER_FLAG_AA;
            ftr_params.user = &stuffbaton;
            ftr_params.black_spans = 0;
            ftr_params.bit_set = 0;
            ftr_params.bit_test = 0;

			 /* Calculate string bounding box in pixels */
            ftr_params.gray_spans = spanner_sizer;

            /* See http://www.freetype.org/freetype2/docs/glyphs/glyphs-3.html */

            int max_x = INT_MIN; // largest coordinate a pixel has been set at, or the pen was advanced to.
            int min_x = INT_MAX; // smallest coordinate a pixel has been set at, or the pen was advanced to.
            int max_y = INT_MIN; // this is max topside bearing along the string.
            int min_y = INT_MAX; // this is max value of (height - topbearing) along the string.
            /*  Naturally, the above comments swap their meaning between horizontal and vertical scripts,
                since the pen changes the axis it is advanced along.
                However, their differences still make up the bounding box for the string.
                Also note that all this is in FT coordinate system where y axis points upwards.
             */


	int  posX =0;
	int  posY =0;
	for(unsigned n =0;n<glyph_count;++n)
	{

		if(FT_Load_Glyph(face,glyph_info[n].codepoint,0))
		{
			//error
		}
		else
		{
			if(face->glyph->format != FT_GLYPH_FORMAT_OUTLINE){
			}
			else
			{
				int gx= posX+ (glyph_pos[n].x_offset/64);
				int gy= posY+ (glyph_pos[n].x_offset/64);

				posX += glyph_pos[n].x_advance /64;
				posY += glyph_pos[n].y_advance /64;

			    //stuffbaton.pixels = (uint32_t *)(((uint8_t *) sdl_surface->pixels) + gy * sdl_surface->pitch) + gx;

		       FT_Outline glyph_outline = face->glyph->outline;
               printf("contours %d , points=%d \n",glyph_outline.n_contours,glyph_outline.n_points);

			   //--------------------------------------
			   printf("conture list...\n");
			   
			   for(int mm =0;mm< glyph_outline.n_contours;++mm)
			   {
				    printf("c_point: %d \n", glyph_outline.contours[mm]);
			   }
			   //--------------------------------------
			   printf("points...\n");
			   for(int mm =0;mm< glyph_outline.n_points;++mm)
			   {
				   auto vpoint = glyph_outline.points[mm];
				   auto vtag = glyph_outline.tags[mm];
				   auto has_dropout= ((vtag>>2)&0x1);
				   auto dropoutMode= vtag >>3;

				   if(vtag & 0x1)
				   { 
					   //on curve
					   if(has_dropout){
					     printf("[%d] on,dropoutMode=%d: %d,y:%d \n", mm,dropoutMode, vpoint.x,vpoint.y );
					   }
					   else
					   { printf("[%d] on,x: %d,y:%d \n",  mm,  vpoint.x,vpoint.y );
					   }
				   }
				   else
				   {
						//bit 1 set=> off curve, this is a control point
					   //if if this is a 2nd order or 3rd order control point
					    if( (vtag >> 1)&0x1)
						{
							printf("[%d] bzc3rd,  x: %d,y:%d \n", mm,vpoint.x,vpoint.y );
						}
						else
						{
							printf("[%d] bzc2nd,  x: %d,y:%d \n", mm,vpoint.x,vpoint.y );
						}
				   }
				  
			   }
			}			
		}
	}

	return 0;
}


//int _tmain(int argc, _TCHAR* argv[])
//{
//	 return console_main(argc,argv);
//}

#include "stdafx.h"
#include "ExportFuncs.h"  

#include <ft2build.h>
#include <hb.h>
#include <hb-ft.h>
#include FT_FREETYPE_H
#include FT_GLYPH_H
#include FT_OUTLINE_H 

FT_Library ft;
FT_Face myface;

hb_font_t *my_hb_ft_font;
hb_buffer_t *my_hb_buf;
//------------------------------------------------------------
int force_ucs2_charmap2(FT_Face ftf)
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
//------------------------------------------------------------
 
	int MyFtLibGetVersion()
	{	
		return 1;		
	};
	int MyFtInitLib()
	{ 
		return FT_Init_FreeType(&ft); 
	};
	int MyFtNewFace(const char* faceName, int pxsize)
	{	   
		int code=0;
		if(code= FT_New_Face(ft,faceName,0, &myface))
		{	//error
			return code;
		}		
		else
		{	
			FT_Set_Pixel_Sizes(myface,0,pxsize);
			force_ucs2_charmap2(myface);			
			return code;
		}
	};
	int MyFtNewMemoryFace(const void* membuffer,int sizeInBytes,int pxsize)
	{
		int code= 0;
		if(code= FT_New_Memory_Face(ft,(FT_Byte*)membuffer,sizeInBytes,0,&myface))
		{
			//error
		
			return code;
		}
		else
		{
			
			FT_Set_Pixel_Sizes(myface,0,pxsize);
			//-------------------------------------
			auto num_faces= myface->num_faces;
			auto face_index= myface->face_index;
			auto num_glyph= myface->num_glyphs;
			auto style_name= myface->style_name;
			auto familyName= myface->family_name; 
			//------------------------------------- 


			force_ucs2_charmap2(myface);			
			return code;
		} 

	}
	int MyFtLoadChar(unsigned int charcode, ExportTypeFace *exportTypeFace)
	{	  		 
		 if(!FT_Load_Char(myface,charcode,FT_LOAD_RENDER))
		 {  
			
			 //----------------------------------------
			//information about this glyph
			//exportTypeFace->n= myface->bbox;
			exportTypeFace->unit_per_em = myface->units_per_EM; 
			exportTypeFace->ascender= myface->ascender;
			exportTypeFace->descender= myface->descender;
			exportTypeFace->height = myface->height;

			exportTypeFace->advanceX = myface->glyph->advance.x;
			exportTypeFace->advanceY = myface->glyph->advance.y;
		    
			exportTypeFace->bboxXmin = myface->bbox.xMin;
			exportTypeFace->bboxXmax = myface->bbox.xMax;
			exportTypeFace->bboxYmin = myface->bbox.xMax;
			exportTypeFace->bboxYmax = myface->bbox.xMax;

			//---------------------------------------- 
			exportTypeFace->outline  = &myface->glyph->outline; 
		    exportTypeFace->bitmap  =  &myface->glyph->bitmap;		     

		 }
		 return 0;
	};
	 
int MyFtSetupShapingEngine(const char* langName,int langNameLen,int direction)
{
	 
	my_hb_ft_font = hb_ft_font_create(myface,NULL); 

	//--create a buffer for harfbuzz to use
	my_hb_buf= hb_buffer_create();
	hb_buffer_set_direction(my_hb_buf,HB_DIRECTION_LTR);
	//hb_buffer_set_script(my_hb_buf,HB_SCRIPT_LATIN);
	hb_buffer_set_script(my_hb_buf,HB_SCRIPT_THAI); 
	hb_buffer_set_language(my_hb_buf,hb_language_from_string("th",strlen("th"))); 

	return 0;
	
};
int MyFtShaping(const uint16_t* text,int charCount)
{	
	
	hb_buffer_add_utf16(my_hb_buf,text,charCount,0,charCount);
	hb_shape(my_hb_ft_font,my_hb_buf,NULL,0);

	//--------------------------
	unsigned int glyph_count;
	hb_glyph_info_t *glyph_info = hb_buffer_get_glyph_infos(my_hb_buf,&glyph_count);
	hb_glyph_position_t *glyph_pos= hb_buffer_get_glyph_positions(my_hb_buf,&glyph_count);
	
	int posX=0;
	int posY=0;
    for(unsigned int n =0;n<glyph_count;++n)
	{  
		 int gx= posX+ (glyph_pos[n].x_offset/64);
		 int gy= posY+ (glyph_pos[n].x_offset/64);
		 
		 //codepoint
		 auto code_point= glyph_info[n].codepoint;
		 
		 //position of each glyph
		 posX += glyph_pos[n].x_advance /64;
		 posY += glyph_pos[n].y_advance /64;			 
	}
	return 0; 
};




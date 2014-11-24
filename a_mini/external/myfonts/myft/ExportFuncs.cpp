#include "stdafx.h"
#include "ExportFuncs.h"  

#include <ft2build.h>
#include <hb.h>
#include <hb-ft.h>
#include FT_FREETYPE_H
#include FT_GLYPH_H
#include FT_OUTLINE_H 

FT_Library ft; 


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


//int MyFtNewFace(const char* faceName, int pxsize)
//{	   
//		int code=0;
//		FT_Face myFace = new FT_FaceRec();
//		if(code= FT_New_Face(ft,faceName,0, &myFace))
//		{	//error
//			return code;
//		}		
//		else
//		{	
//			FT_Set_Pixel_Sizes( myFace,0,pxsize);
//			force_ucs2_charmap2( myFace);			
//			return code;
//		}
//};

FT_Face MyFtNewMemoryFace(const void* membuffer, int sizeInBytes,int pxsize)
{
		int code= 0;
		//create on heap
		 
		FT_Face myface= new FT_FaceRec_();
		 

		if(code= FT_New_Memory_Face(ft,(FT_Byte*)membuffer,sizeInBytes,0,&myface))
		{
			//error
			delete myface; 
			return NULL;
		}
		else
		{
			force_ucs2_charmap2(myface);	
			FT_Set_Pixel_Sizes(myface,0,pxsize);
			//-------------------------------------
			//auto num_faces= myface->num_faces;
			//auto face_index= myface->face_index;
			//auto num_glyph= myface->num_glyphs;
			//auto style_name= myface->style_name;
			//auto familyName= myface->family_name; 
			////------------------------------------- 
			return myface;
		} 
}
void MyFtSetPixelSizes(FT_Face myface,int pxsize)
{
	FT_Set_Pixel_Sizes(myface,0,pxsize);
}
void MyFtSetCharSize(FT_Face myface,int char_width,int 
	char_height,int h_device_resolution, int v_device_resolution)
{
	FT_Set_Char_Size(myface,
		char_width, // in  1/64th of points
		char_height, // in 1/64th of points
		h_device_resolution,
		v_device_resolution);        
}

void MyFtDoneFace(FT_Face  face)
{
	if(face)
	{	
		FT_Done_Face(face);		 
	}
};

void MyFtGetFaceInfo(FT_Face face,ExportTypeFaceInfo* exportTypeFaceInfo)
{
	exportTypeFaceInfo->hasKerning = FT_HAS_KERNING(face);
};

int MyFtLoadGlyph(FT_Face myface,unsigned int glyphIndex,ExportGlyph *expGlyph)
{
	if(!FT_Load_Glyph(myface,glyphIndex,FT_LOAD_RENDER))
	{
		//1. bounding box		 
			FT_BBox bbox;
			FT_Glyph glyph;
			FT_Get_Glyph(myface->glyph,&glyph);
			FT_Glyph_Get_CBox(glyph,FT_LOAD_NO_SCALE,&bbox); 
			 
			expGlyph->bboxXmin = bbox.xMin;
			expGlyph->bboxXmax = bbox.xMax;
			expGlyph->bboxYmin = bbox.yMin;
			expGlyph->bboxYmax = bbox.yMax;
			//-------------------------------------------
			auto glypMetric= myface->glyph->metrics;
			expGlyph->img_height = glypMetric.height;
			expGlyph->img_width = glypMetric.width;
			expGlyph->img_horiBearingX  = glypMetric.horiBearingX;
			expGlyph->img_horiBearingY  = glypMetric.horiBearingY;
			expGlyph->img_horiAdvance = glypMetric.horiAdvance;

			expGlyph->img_vertBearingX  = glypMetric.vertBearingX;
			expGlyph->img_horiBearingY  = glypMetric.vertBearingY;
			expGlyph->img_vertAdvance = glypMetric.vertAdvance;
			//-------------------------------------------

			expGlyph->advanceX = myface->glyph->advance.x;
			expGlyph->advanceY = myface->glyph->advance.y;
			expGlyph->bitmap_left = myface->glyph->bitmap_left;
			expGlyph->bitmap_top = myface->glyph->bitmap_top;
			//---------------------------------------- 
			expGlyph->outline  = &myface->glyph->outline; 
		    expGlyph->bitmap  =  &myface->glyph->bitmap;		    
			 
	}
	return 0;
};
int MyFtLoadChar(FT_Face myface,unsigned int charcode, ExportGlyph *expGlyph)
{	  		 
		 if(!FT_Load_Char(myface,charcode,FT_LOAD_RENDER))
		 {  
			
			//1. bounding box		 
			FT_BBox bbox;
			FT_Glyph glyph;
			FT_Get_Glyph(myface->glyph,&glyph);
			FT_Glyph_Get_CBox(glyph,FT_LOAD_NO_SCALE,&bbox); 
			 
			expGlyph->bboxXmin = bbox.xMin;
			expGlyph->bboxXmax = bbox.xMax;
			expGlyph->bboxYmin = bbox.yMin;
			expGlyph->bboxYmax = bbox.yMax;
			//-------------------------------------------
			auto glypMetric= myface->glyph->metrics;
			expGlyph->img_height = glypMetric.height;
			expGlyph->img_width = glypMetric.width;
			expGlyph->img_horiBearingX  = glypMetric.horiBearingX;
			expGlyph->img_horiBearingY  = glypMetric.horiBearingY;
			expGlyph->img_horiAdvance = glypMetric.horiAdvance;

			expGlyph->img_vertBearingX  = glypMetric.vertBearingX;
			expGlyph->img_horiBearingY  = glypMetric.vertBearingY;
			expGlyph->img_vertAdvance = glypMetric.vertAdvance;
			//-------------------------------------------

			expGlyph->advanceX = myface->glyph->advance.x;
			expGlyph->advanceY = myface->glyph->advance.y;
			expGlyph->bitmap_left = myface->glyph->bitmap_left;
			expGlyph->bitmap_top = myface->glyph->bitmap_top;
			//---------------------------------------- 
			expGlyph->outline  = &myface->glyph->outline; 
		    expGlyph->bitmap  =  &myface->glyph->bitmap;		    
			 
		 }
		 return 0;
};
	 
hb_direction_t current_direction;
hb_script_t current_script;
hb_language_t current_lang;

int MyFtSetupShapingEngine(FT_Face myface,const char* langName,int langNameLen, int direction,int scriptCode,ExportTypeFaceInfo* exportTypeInfo)
{
	 
	exportTypeInfo->my_hb_ft_font = hb_ft_font_create(myface,NULL); 


	//--create a buffer for harfbuzz to use 
	current_direction = (hb_direction_t)direction;
	current_script = (hb_script_t)scriptCode;
	current_lang= hb_language_from_string(langName,langNameLen); 
	return 0; 
};

int MyFtShaping(hb_font_t *my_hb_ft_font, 
	const uint16_t* text,
	int charCount,
	ProperGlyph* properGlyphs)
{	
	
	auto my_hb_buf = hb_buffer_create(); 
	hb_buffer_set_direction(my_hb_buf,current_direction);	 
	hb_buffer_set_script(my_hb_buf,current_script); 
	hb_buffer_set_language(my_hb_buf,current_lang); 
	  
	hb_buffer_add_utf16(my_hb_buf,text,charCount,0,charCount);
	hb_shape(my_hb_ft_font,my_hb_buf,NULL,0); 
	//--------------------------
	unsigned int glyph_count=0;
	//count glyph
	hb_glyph_info_t *glyph_info = hb_buffer_get_glyph_infos(my_hb_buf,&glyph_count);
	hb_glyph_position_t *glyph_pos= hb_buffer_get_glyph_positions(my_hb_buf,&glyph_count);
	 
	int posX=0;
	int posY=0;
	int p=0;

    for(unsigned int n =0;n<glyph_count;++n)
	{  
		 properGlyphs[p].codepoint = glyph_info[n].codepoint;// 
		 properGlyphs[p].x_offset  = glyph_pos[n].x_offset;
		 properGlyphs[p].y_offset = glyph_pos[n].y_offset;
		 properGlyphs[p].x_advance = glyph_pos[n].x_advance;
		 properGlyphs[p].y_advance =glyph_pos[n].y_advance;
		 p++;  
	}
	
	hb_buffer_destroy(my_hb_buf);
	return 0;
};
 
 void MyFtShutdownLib()
 {
	 if(ft)
	 {
		 FT_Done_FreeType(ft);
		 ft=0;
	 }
 };




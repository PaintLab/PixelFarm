#include "../npixel.h" 

void drawing1(IVideo* winVideo)
{
	ipixel_point_t pts[3] = { { 100, 80 }, { 110, 120 }, { 210, 110 } };
	int i;

	ipaint_fill(winVideo->paint, NULL, 0xffffffff);

	ipaint_anti_aliasing(winVideo->paint, 3);
	ipaint_draw_polygon(winVideo->paint, pts, 3);

	for (i = 0; i < 3; i++) pts[i].x += 320;

	ipaint_anti_aliasing(winVideo->paint, 0);
	ipaint_draw_polygon(winVideo->paint, pts, 3);

	ipaint_set_color(winVideo->paint, 0x900000ff);
	ipaint_anti_aliasing(winVideo->paint, 3);

	ipaint_text_color(winVideo->paint, 0xff00aaaa);
	ipaint_text_background(winVideo->paint, 0);

	ipaint_cprintf(winVideo->paint, 100, 8, "ANTI ALIASING ON");
	ipaint_cprintf(winVideo->paint, 420, 8, "ANTI ALIASING OFF");
	ipaint_cprintf(winVideo->paint, 100, 220, "     ZOOM");
	ipaint_cprintf(winVideo->paint, 420, 220, "     ZOOM");

	// (100, 80, 210, 110) -> (100, 80, 110, 30)
	// 110, 40, 
	ibitmap_stretch(winVideo->cscreen, 20, 280, 280, 76, winVideo->cscreen, 100, 80, 110, 40, 0);
	ibitmap_stretch(winVideo->cscreen,350, 280, 280, 76, winVideo->cscreen, 420, 80, 110, 40, 0);
}

//! lib: .
//! link: pixel
//! win: 
int main1( IVideo* winVideo	)
{
	int retval;
	
	if ((retval = iscreen_init(640, 480, 32, winVideo->hWnd,winVideo)) != 0) {
		printf("error init\n");
		return -1;
	}

	winVideo->paint = ipaint_create(winVideo->cscreen);

	drawing1(winVideo);

	/*while (iscreen_dispatch() == 0) {
		iscreen_tick(32);
		if (iscreen_keyon(IKEY_ESCAPE)) break;

		iscreen_convert(NULL, 1);
		iscreen_update(NULL, 1);
	}*/

	return 0;
}


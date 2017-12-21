#include "../npixel.h"

 
//

void drawing4(IVideo* winVideo)
{	

	/*IBITMAP *picture;
	static ipixel_point_t pts[4] = 
		{ { 100, 100 }, { 100, 400 }, { 500, 400 }, { 500, 100 } };*/
	 /*
	static int mouse_on = -1;
	static int mouse_drag = -1;
	static int mouse_dx = 0;
	static int mouse_dy = 0;
	static int button = 0;
	int i;*/

	//1. clear bg
	ipaint_fill(winVideo->paint, NULL, 0xffffffff);

	//2. draw content ...
	
	//ipaint_raster(winVideo->paint, pts, picture, NULL, 0xffffffff, 0);

	//ipaint_set_color(winVideo->paint, 0xa000aaaa);

	/*for (i = 0; i < 4; i++) {
		double x2 = pts[(i + 1) & 3].x;
		double y2 = pts[(i + 1) & 3].y;
		ipaint_draw_line(winVideo->paint, pts[i].x, pts[i].y, x2, y2);
	}

	#define ABS(x) (((x) < 0)? (-(x)) : (x))
*/
	//mouse_on = -1;

	//for (i = 0; i < 4; i++) {
	//	double r = 4;

	//	if (ABS(iscreen_mx - pts[i].x) < 5 && 
	//		ABS(iscreen_my - pts[i].y) < 5)
	//		mouse_on = i;

	//	//printf("%d\n", mouse_on);

	//	ipaint_set_color(winVideo->paint, 0xa0ff0000);
	//	if (mouse_on == i || mouse_drag == i) r = 6;
	//	if (mouse_drag == i) ipaint_set_color(winVideo->paint, 0xa08000ff);
	//	ipaint_draw_circle(winVideo->paint, pts[i].x, pts[i].y, r);

	//	ipaint_text_color(winVideo->paint, 0xa0333333);
	//	ipaint_text_background(winVideo->paint, 0);
	//	ipaint_cprintf(winVideo->paint, pts[i].x - 4, pts[i].y - 16, "%d", i + 1);
	//}

	// if (button == 0 && (iscreen_mb & 2)) {
	//	button = 1;
	//	if (mouse_on >= 0) {
	//		mouse_dx = pts[mouse_on].x - iscreen_mx;
	//		mouse_dy = pts[mouse_on].y - iscreen_my;
	//		if (mouse_drag < 0) mouse_drag = mouse_on;
	//	}
	//}

	//if (iscreen_mb & 4) {
	//	pts[0].x = 100; pts[0].y = 100;
	//	pts[1].x = 100; pts[1].y = 400;
	//	pts[2].x = 500; pts[2].y = 400;
	//	pts[3].x = 500; pts[3].y = 100;
	//}

	//if (mouse_drag >= 0) {
	//	pts[mouse_drag].x = iscreen_mx + mouse_dx;
	//	pts[mouse_drag].y = iscreen_my + mouse_dy;
	//} 

	//if (iscreen_mb == 0) { button = 0; mouse_drag = -1; }

	/*ipaint_cprintf(winVideo->paint, 1,  0, "USE MOUSE TO DRAG THE FOUR CORNERS OF THE PICTURE");
	ipaint_cprintf(winVideo->paint, 1, 16, "CLICK RIGHT MOUSE BUTTON TO RESET");
*/

}


//! lib: .
//! link: pixel
//! win: 
int main4(IVideo* winVideo)
{	
	//prepare screen*

	int retval;   
	//IBITMAP *picture;
	if ((retval = iscreen_init(1024,600, 32,winVideo->hWnd,winVideo)) != 0) {
		printf("error init\n");
		return -1;
	}

	winVideo->paint = ipaint_create(winVideo->cscreen); 
	//picture = ipic_load_file("res_fish.bmp", 0, NULL);
	//assert(picture);    
	//ibitmap_imode(picture, overflow) = IBOM_REPEAT;


	/*while (iscreen_dispatch() == 0) {
		iscreen_tick(30);
		if (iscreen_keyon(IKEY_ESCAPE)) break;

		drawing4();

		iscreen_convert(NULL, 1);
		iscreen_update(NULL, 1);
	}
     */
	return 0;
}


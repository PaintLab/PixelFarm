#include "../npixel.h"

 

void drawing2(IVideo* winVideo)
{
	ipixel_point_t pts[3] = { { 100, 80 }, { 110, 120 }, { 210, 110 } };
	int i;
 

	ipaint_fill(winVideo->paint, NULL, 0xffffffff);

	ipaint_line_width(winVideo->paint, 20);

	for (i = 0; i < 15; i++) {
		double x1 = 580 / 15 * i + 20;
		double x2 = x1 + i * 5;
		int cc = 128 + 127 * i / 15;

		ipaint_set_color(winVideo->paint, IRGBA_TO_A8R8G8B8(0, cc, cc, 255));
		ipaint_line_width(winVideo->paint, 20);
		ipaint_draw_line(winVideo->paint, x1, 130, x2, 50);

		ipaint_set_color(winVideo->paint, IRGBA_TO_A8R8G8B8(cc, cc, 0, 255));
		ipaint_line_width(winVideo->paint, 0.5 + i * 1.5);
		ipaint_draw_line(winVideo->paint, x1, 280, x1 + 50, 190);

		ipaint_set_color(winVideo->paint, IRGBA_TO_A8R8G8B8(cc, 0, cc, 255));
		ipaint_draw_circle(winVideo->paint, x1, 350, i + 0.5);
	}

	
}

//! lib: .
//! link: pixel
//! win: 
int main2(IVideo* winVideo)
{
	int retval; 
	if ((retval = iscreen_init(800, 600, 32,winVideo->hWnd,winVideo)) != 0) {
		printf("error init\n");
		return -1;
	}

	//drawing canvas
	winVideo->paint = ipaint_create(winVideo->cscreen);
	drawing2(winVideo);

	/*while (iscreen_dispatch() == 0) {
		iscreen_tick(32);
		if (iscreen_keyon(IKEY_ESCAPE)) break;

		iscreen_convert(NULL, 1);
		iscreen_update(NULL, 1);
	}*/

	return 0;
}


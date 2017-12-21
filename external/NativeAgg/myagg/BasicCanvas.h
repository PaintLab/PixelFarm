
#include "agg_basics.h"
#include "agg_rendering_buffer.h"
#include "agg_trans_viewport.h"
#include "ctrl/agg_ctrl.h"
#include "platform/agg_platform_support.h"


namespace agg
{
	
	class BasicCanvas
    {
    public:
        enum max_images_e { max_images = 16 };

        // format - see enum pix_format_e {};
        // flip_y - true if you want to have the Y-axis flipped vertically.
        BasicCanvas(pix_format_e format, bool flip_y);
        virtual ~BasicCanvas();

        // Setting the windows caption (title). Should be able
        // to be called at least before calling init(). 
        // It's perfect if they can be called anytime.
        //void        caption(const wchar_t* cap);
        //const wchar_t* caption() const { return m_caption; }

        //--------------------------------------------------------------------
        // These 3 methods handle working with images. The image
        // formats are the simplest ones, such as .BMP in Windows or 
        // .ppm in Linux. In the applications the names of the files
        // should not have any file extensions. Method load_img() can
        // be called before init(), so, the application could be able 
        // to determine the initial size of the window depending on 
        // the size of the loaded image. 
        // The argument "idx" is the number of the image 0...max_images-1
        bool load_img(unsigned idx, const char* file);
        bool save_img(unsigned idx, const char* file);
        bool create_img(unsigned idx, unsigned width=0, unsigned height=0);

        //--------------------------------------------------------------------
        // init() and run(). See description before the class for details.
        // The necessity of calling init() after creation is that it's 
        // impossible to call the overridden virtual function (on_init()) 
        // from the constructor. On the other hand it's very useful to have
        // some on_init() event handler when the window is created but 
        // not yet displayed. The rbuf_window() method (see below) is 
        // accessible from on_init().
        bool init(unsigned width, unsigned height, unsigned flags);
        int  run();

        //--------------------------------------------------------------------
        // The very same parameters that were used in the constructor
        pix_format_e format() const { return m_format; }
        bool flip_y() const { return m_flip_y; }
        unsigned bpp() const { return m_bpp; }

        //--------------------------------------------------------------------
        // The following provides a very simple mechanism of doing someting
        // in background. It's not multithreading. When wait_mode is true
        // the class waits for the events and it does not ever call on_idle().
        // When it's false it calls on_idle() when the event queue is empty.
        // The mode can be changed anytime. This mechanism is satisfactory
        // to create very simple animations.
         
        //--------------------------------------------------------------------
        // These two functions control updating of the window. 
        // force_redraw() is an analog of the Win32 InvalidateRect() function.
        // Being called it sets a flag (or sends a message) which results
        // in calling on_draw() and updating the content of the window 
        // when the next event cycle comes.
        // update_window() results in just putting immediately the content 
        // of the currently rendered buffer to the window without calling
        // on_draw().
        //void force_redraw();
        //void update_window();

        //--------------------------------------------------------------------
        // So, finally, how to draw anythig with AGG? Very simple.
        // rbuf_window() returns a reference to the main rendering 
        // buffer which can be attached to any rendering class.
        // rbuf_img() returns a reference to the previously created
        // or loaded image buffer (see load_img()). The image buffers 
        // are not displayed directly, they should be copied to or 
        // combined somehow with the rbuf_window(). rbuf_window() is
        // the only buffer that can be actually displayed.
        rendering_buffer& rbuf_window()          { return m_rbuf_window; } 
        rendering_buffer& rbuf_img(unsigned idx) { return m_rbuf_img[idx]; } 
        

        //--------------------------------------------------------------------
        // Returns file extension used in the implementation for the particular
        // system.
        const char* img_ext() const;

        //--------------------------------------------------------------------
        void copy_img_to_window(unsigned idx)
        {
            if(idx < max_images && rbuf_img(idx).buf())
            {
                rbuf_window().copy_from(rbuf_img(idx));
            }
        }
        
        //--------------------------------------------------------------------
        void copy_window_to_img(unsigned idx)
        {
            if(idx < max_images)
            {
                create_img(idx, rbuf_window().width(), rbuf_window().height());
                rbuf_img(idx).copy_from(rbuf_window());
            }
        }
       
        //--------------------------------------------------------------------
        void copy_img_to_img(unsigned idx_to, unsigned idx_from)
        {
            if(idx_from < max_images && 
               idx_to < max_images && 
               rbuf_img(idx_from).buf())
            {
                create_img(idx_to, 
                           rbuf_img(idx_from).width(), 
                           rbuf_img(idx_from).height());
                rbuf_img(idx_to).copy_from(rbuf_img(idx_from));
            }
        }

        //--------------------------------------------------------------------
        // Event handlers. They are not pure functions, so you don't have
        // to override them all.
        // In my demo applications these functions are defined inside
        // the MyBasicCanvas class (implicit inlining) which is in general 
        // very bad practice, I mean vitual inline methods. At least it does
        // not make sense. 
        // But in this case it's quite appropriate bacause we have the only
        // instance of the MyBasicCanvas class and it is in the same file 
        // where this class is defined.
        virtual void on_init();
       /* virtual void on_resize(int sx, int sy);
        virtual void on_idle();
        virtual void on_mouse_move(int x, int y, unsigned flags);
        virtual void on_mouse_button_down(int x, int y, unsigned flags);
        virtual void on_mouse_button_up(int x, int y, unsigned flags);
        virtual void on_key(int x, int y, unsigned key, unsigned flags);
        virtual void on_ctrl_change();
       
        virtual void on_post_draw(void* raw_handler);*/
		 virtual void on_draw();
        //--------------------------------------------------------------------
        // Adding control elements. A control element once added will be 
        // working and reacting to the mouse and keyboard events. Still, you
        // will have to render them in the on_draw() using function 
        // render_ctrl() because platform_support doesn't know anything about 
        // renderers you use. The controls will be also scaled automatically 
        // if they provide a proper scaling mechanism (all the controls 
        // included into the basic AGG package do).
        // If you don't need a particular control to be scaled automatically 
        // call ctrl::no_transform() after adding.
        void add_ctrl(ctrl& c) {
			//m_ctrls.add(c); 
			c.transform(m_resize_mtx); 
		}

        //--------------------------------------------------------------------
        // Auxiliary functions. trans_affine_resizing() modifier sets up the resizing 
        // matrix on the basis of the given width and height and the initial
        // width and height of the window. The implementation should simply 
        // call this function every time when it catches the resizing event
        // passing in the new values of width and height of the window.
        // Nothing prevents you from "cheating" the scaling matrix if you
        // call this function from somewhere with wrong arguments. 
        // trans_affine_resizing() accessor simply returns current resizing matrix 
        // which can be used to apply additional scaling of any of your 
        // stuff when the window is being resized.
        // width(), height(), initial_width(), and initial_height() must be
        // clear to understand with no comments :-)
        void trans_affine_resizing(int width, int height)
        {
            if( window_keep_aspect_ratio)
            {
                //double sx = double(width) / double(m_initial_width);
                //double sy = double(height) / double(m_initial_height);
                //if(sy < sx) sx = sy;
                //m_resize_mtx = trans_affine_scaling(sx, sx);
                trans_viewport vp;
                vp.preserve_aspect_ratio(0.5, 0.5, aspect_ratio_meet);
                vp.device_viewport(0, 0, width, height);
                vp.world_viewport(0, 0, m_initial_width, m_initial_height);
                m_resize_mtx = vp.to_affine();
            }
            else
            {
                m_resize_mtx = trans_affine_scaling(
                    double(width) / double(m_initial_width),
                    double(height) / double(m_initial_height));
            }
        }
        const    trans_affine& trans_affine_resizing() const { return m_resize_mtx; }
        double   width()  const { return m_rbuf_window.width(); }
        double   height() const { return m_rbuf_window.height(); }
        double   initial_width()  const { return m_initial_width; }
        double   initial_height() const { return m_initial_height; }
       
        //--------------------------------------------------------------------
        // Get raw display handler depending on the system. 
        // For win32 its an HDC, for other systems it can be a pointer to some
        // structure. See the implementation files for detals.
        // It's provided "as is", so, first you should check if it's not null.
        // If it's null the raw_display_handler is not supported. Also, there's 
        // no guarantee that this function is implemented, so, in some 
        // implementations you may have simply an unresolved symbol when linking.
       // void* raw_display_handler();

        //--------------------------------------------------------------------
        // display message box or print the message to the console 
        // (depending on implementation)
        //void message(const char* msg);

        //--------------------------------------------------------------------
        // Stopwatch functions. Function elapsed_time() returns time elapsed 
        // since the latest start_timer() invocation in millisecods. 
        // The resolutoin depends on the implementation. 
        // In Win32 it uses QueryPerformanceFrequency() / QueryPerformanceCounter().
        //void   start_timer();
       // double elapsed_time() const;

        //--------------------------------------------------------------------
        // Get the full file name. In most cases it simply returns
        // file_name. As it's appropriate in many systems if you open
        // a file by its name without specifying the path, it tries to 
        // open it in the current directory. The demos usually expect 
        // all the supplementary files to be placed in the current 
        // directory, that is usually coincides with the directory where
        // the the executable is. However, in some systems (BeOS) it's not so. 
        // For those kinds of systems full_file_name() can help access files 
        // preserving commonly used policy.
        // So, it's a good idea to use in the demos the following:
        // FILE* fd = fopen(full_file_name("some.file"), "r"); 
        // instead of
        // FILE* fd = fopen("some.file", "r"); 
        //const char* full_file_name(const char* file_name);

    public:
        platform_specific* m_specific;
        //ctrl_container m_ctrls;

        // Sorry, I'm too tired to describe the private 
        // data membders. See the implementations for different
        // platforms for details.
    private:
        BasicCanvas(const BasicCanvas&);
        const BasicCanvas& operator = (const BasicCanvas&);

        pix_format_e     m_format;
        unsigned         m_bpp;
        rendering_buffer m_rbuf_window;
        rendering_buffer m_rbuf_img[max_images];
        //unsigned         m_window_flags;
        
        bool             m_flip_y; 
        int              m_initial_width;
        int              m_initial_height;
        trans_affine     m_resize_mtx;
    };


}
//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.ContentManagers
{
    public class ImageRequestEventArgs : EventArgs
    {
        public ImageRequestEventArgs(ImageBinder binder)
        {
            this.ImageBinder = binder;
        }

        //TODO: review here
        public object requestBy;
        public ImageBinder ImageBinder { get; private set; }
        public string ImagSource
        {
            get { return this.ImageBinder.ImageSource; }
        }
        public void SetResultImage(Image img)
        {
            this.ImageBinder.SetImage(img);
        }
    }

    public class ImageContentManager
    {
        /// <summary>
        /// raise when the manager can't get a specific image
        /// </summary>
        public event EventHandler<ImageRequestEventArgs> AskForImage;

        LinkedList<ImageBinder> _inputList = new LinkedList<ImageBinder>();
        LinkedList<ImageBinder> _outputList = new LinkedList<ImageBinder>();
        ImageCacheSystem _imageCacheLevel = new ImageCacheSystem();

        bool _hasSomeInputHint;

        object _outputListSync = new object();
        object _inputListSync = new object();
        bool _working = false;

        public ImageContentManager()
        {
            //TODO: review here****             
            UIPlatform.RegisterTimerTask(50, TimImageLoadMonitor_Tick);
        }


        void TimImageLoadMonitor_Tick(UITimerTask timer_task)
        {
            lock (_inputListSync)
            {
                if (_working)
                {
                    return;
                }
                if (!_hasSomeInputHint)
                {
                    return;
                }
                _working = true;
            }

            int j = _inputList.Count;
            //load image in this list

            //copy data out 
            for (int i = 0; i < j; ++i)
            {
                ImageBinder binder = _inputList.First.Value;
                _inputList.RemoveFirst();

                //wait until finish this  ....  

                //1. check from cache if not found
                //then send request to external ...  
                string imgSrc = binder.ImageSource;


                //img content manager can cache and optimize image resource usage
                //we support png, jpg,  svg



                if (this.imageCacheLevel0.TryGetCacheImage(
                    binder.ImageSource,
                    out Image foundImage))
                {
                    //process image infomation
                    //....  
                    binder.SetImage(foundImage);
                }
                else
                {
                    //not found in cache => request image loader
                    //image load/waiting should be done on another thread




                    this.AskForImage(
                        this,
                        new ImageRequestEventArgs(binder));

                    //....
                    //process image infomation
                    //.... 
                    if (binder.State == BinderState.Loaded)
                    {
                        //store to cache 
                        //TODO: implement caching policy  
                        _imageCacheLevel.AddCacheImage(binder.ImageSource, binder.Image);
                    }
                }

                //next image
            }
            if (j == 0)
            {
                _hasSomeInputHint = false;
            }

            _working = false;
        }

        public virtual bool AddRequestImage(ImageBinder contentReq)
        {
            if (contentReq.ImageSource == null && !contentReq.HasLazyFunc)
            {
                contentReq.State = BinderState.Blank;
                return true;
            }
            //binder and req box 
            //1. 
            contentReq.State = BinderState.Loading;
            //2.
            _inputList.AddLast(contentReq);
            //another thread will manage this request 
            //and store in outputlist         
            _hasSomeInputHint = true;

            return true;
        }
    }
}
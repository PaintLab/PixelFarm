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
        public string ImagSource => this.ImageBinder.ImageSource;
        
        public void SetResultImage(Image img)
        {
            this.ImageBinder.SetLocalImage(img);
        }
    }


    public class ImageLoadingQueueManager
    {
        /// <summary>
        /// raise when the manager can't get a specific image
        /// </summary>
        public event EventHandler<ImageRequestEventArgs> AskForImage;

        LinkedList<ImageBinder> _inputList = new LinkedList<ImageBinder>();
        LinkedList<ImageBinder> _outputList = new LinkedList<ImageBinder>();
        bool _hint_HasSomeInput;
        object _outputListSync = new object();
        object _inputListSync = new object();
        bool _working = false;
        ImageCacheSystem _imgCache;
        public ImageLoadingQueueManager()
        {
            //TODO: review here****             
            UIPlatform.RegisterTimerTask(50, TimImageLoadMonitor_Tick);
            //default img caching
            _imgCache = new ImageCacheSystem();
        }
        public ImageCacheSystem ImgCache
        {
            get => _imgCache;
            set => _imgCache = value;
        }

        void TimImageLoadMonitor_Tick(UITimerTask timer_task)
        {
            lock (_inputListSync)
            {
                if (_working)
                {
                    return;
                }
                if (!_hint_HasSomeInput)
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

                //img content manager can cache and optimize image resource usage
                //we support png, jpg,  svg 

                if (_imgCache != null && _imgCache.TryGetCacheImage(
                    binder.ImageSource,
                    out Image foundImage))
                {
                    //process image infomation
                    //....  
                    binder.SetLocalImage(foundImage);
                }
                else
                {
                    //not found in cache => request image loader
                    //image load/waiting should be done on another thread

                    //resolve this image 

                    ImageRequestEventArgs imgReq = new ImageRequestEventArgs(binder);
                    this.AskForImage(
                        this,
                        imgReq);

                    //....
                    //process image infomation
                    //.... 
                    if (binder.State == BinderState.Loaded && _imgCache != null)
                    {
                        //store to cache 
                        //TODO: implement caching policy  
                        _imgCache.Replace(binder.ImageSource, binder.LocalImage);
                    }
                }

                //next image
            }
            if (j == 0)
            {
                _hint_HasSomeInput = false;
            }

            _working = false;
        }

        public bool AddRequestImage(ImageBinder contentReq)
        {
            if (contentReq.ImageSource == null && !contentReq.HasLazyFunc)
            {
                contentReq.State = BinderState.Blank;
                return true;
            }
            
            if (_imgCache != null && _imgCache.TryGetCacheImage(
                contentReq.ImageSource,
                out Image foundImage))
            {
                //process image infomation
                //....  
                contentReq.SetLocalImage(foundImage);
                return true;
            }

            //binder and req box 
            //1. 
            contentReq.State = BinderState.Loading;
            //2.
            _inputList.AddLast(contentReq);
            //another thread will manage this request 
            //and store in outputlist         
            _hint_HasSomeInput = true;

            return true;
        }
    }
}
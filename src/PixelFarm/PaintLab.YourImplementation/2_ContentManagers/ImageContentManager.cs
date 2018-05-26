//BSD, 2014-2018, WinterDev 
//ArthurHub  , Jose Manuel Menendez Poo

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
        public event EventHandler<ImageRequestEventArgs> ImageLoadingRequest;

        LinkedList<ImageBinder> inputList = new LinkedList<ImageBinder>();
        LinkedList<ImageBinder> outputList = new LinkedList<ImageBinder>();
        ImageCacheSystem imageCacheLevel0 = new ImageCacheSystem();

        bool hasSomeInputHint;
       
        object outputListSync = new object();
        object inputListSync = new object();
        bool working = false;
        public ImageContentManager()
        {
            //TODO: review here**** 

            UIPlatform.RegisterTimerTask(50, TimImageLoadMonitor_Tick);
        }

        void TimImageLoadMonitor_Tick(UITimerTask timer_task)
        {
            lock (inputListSync)
            {
                if (working)
                {
                    return;
                }
                if (!hasSomeInputHint)
                {
                    return;
                }
                working = true;
            } 
             
            int j = inputList.Count;
            //load image in this list

            //copy data out 
            for (int i = 0; i < j; ++i)
            {
                var firstNode = inputList.First;
                inputList.RemoveFirst();
                ImageBinder binder = firstNode.Value;
                //wait until finish this  ....  

                //1. check from cache if not found
                //then send request to external ... 

                Image foundImage;
                if (this.imageCacheLevel0.TryGetCacheImage(
                    binder.ImageSource,
                    out foundImage))
                {
                    //process image infomation
                    //....  
                    binder.SetImage(foundImage);
                }
                else
                {
                    //not found in cache => request image loader
                    //image load/waiting should be done on another thread

                    this.ImageLoadingRequest(
                        this,
                        new ImageRequestEventArgs(binder));
                    
                    //....
                    //process image infomation
                    //.... 
                    if (binder.State == ImageBinderState.Loaded)
                    {
                        //store to cache 
                        //TODO: implement caching policy  
                        imageCacheLevel0.AddCacheImage(binder.ImageSource, binder.Image);
                    }
                }

                //next image
            }
            if (j == 0)
            {
                hasSomeInputHint = false;
            }
             
            working = false;
        } 
        public void AddRequestImage(ImageBinder contentReq)
        {
            if (contentReq.ImageSource == null && !contentReq.HasLazyFunc)
            {
                contentReq.State = ImageBinderState.NoImage;
                return;
            }
            //binder and req box 
            //1. 
            contentReq.State = ImageBinderState.Loading;
            //2.
            inputList.AddLast(contentReq);
            //another thread will manage this request 
            //and store in outputlist         
            hasSomeInputHint = true;
        }
    }
}
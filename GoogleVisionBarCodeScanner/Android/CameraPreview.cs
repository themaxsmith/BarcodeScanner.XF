using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Firebase.ML.Vision;
using Firebase.ML.Vision.Barcode;
using Firebase.ML.Vision.Common;
using Java.Lang;
using static Android.Views.View;

namespace GoogleVisionBarCodeScanner
{
    internal sealed class CameraPreview : ViewGroup, Android.Hardware.Camera.IPreviewCallback, IOnSuccessListener
    {

        //Source for structure: https://firebase.google.com/docs/ml-kit/android/read-barcodes

        private readonly FirebaseVisionBarcodeDetector _barcodeDetector;
        private readonly SurfaceView _surfaceView;
        private readonly IWindowManager _windowManager;
        public event Action<List<BarcodeResult>> OnDetected;

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            //Off the torch when exit page
            if (GoogleVisionBarCodeScanner.Methods.IsTorchOn())
                GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();



        }


        public CameraPreview(Context context, bool defaultTorchOn, bool virbationOnDetected, bool startScanningOnCreate, float? requestedFPS)
            : base(context)
        {
            Configuration.IsScanning = startScanningOnCreate;
            _windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            var options = new FirebaseVisionBarcodeDetectorOptions.Builder().SetBarcodeFormats(FirebaseVisionBarcode.FormatAllFormats).Build();
            
            _barcodeDetector = FirebaseVision.Instance
       .GetVisionBarcodeDetector(options);

            // https://github.com/jamesathey/FastAndroidCamera Used for camera access currently. hopfully can switch to camerax or camera 2

            Android.Hardware.Camera camera = Android.Hardware.Camera.Open();
            Android.Hardware.Camera.Parameters parameters = camera.GetParameters();

            // snip - set resolution, frame rate, preview format, etc.
            parameters.PictureFormat = ImageFormatType.Nv21;
            parameters.SetPreviewSize(1920, 1080);
            camera.SetParameters(parameters);

            // assuming the SurfaceView has been set up elsewhere
            camera.SetPreviewDisplay(_surfaceView.Holder);

            int numBytes = (parameters.PreviewSize.Width * parameters.PreviewSize.Height * ImageFormat.GetBitsPerPixel(parameters.PreviewFormat)) / 8;

          
            camera.StartPreview();
            camera.SetPreviewCallback(this);


            AddView(_surfaceView);


            //if (defaultTorchOn)
            //    AutoSwitchOnTorch();
        }
        FirebaseVisionImageMetadata meta = new FirebaseVisionImageMetadata.Builder()
                .SetFormat(FirebaseVisionImageMetadata.ImageFormatNv21).SetHeight(1080).SetWidth(1920).SetRotation(0).Build();

        bool doProcessFrame = true;
        public void OnPreviewFrame(byte[] data, Android.Hardware.Camera camera)
        {
            // Do per-frame video processing here
            // TODO: Only do one frame per secound to cut down on processing

            if (doProcessFrame)
            {

                FirebaseVisionImage image = FirebaseVisionImage.FromByteArray(data, meta);

                var result = _barcodeDetector.DetectInImage(image).AddOnSuccessListener(this);

            }
        }

        public void OnSuccess(List<FirebaseVisionBarcode> result)
        {
            List<FirebaseVisionBarcode> barcodes = result;
            List<BarcodeResult> barcodeResults = new List<BarcodeResult>();

            foreach (var barcode in barcodes)
            {
                barcodeResults.Add(new BarcodeResult
                {
                    BarcodeType = BarcodeTypes.Text, // needs to get correct type
                    DisplayValue = barcode.DisplayValue
                }) ;
            }
         
                    OnDetected?.Invoke(barcodeResults);
                
            
        }




        private void detectedBarcode()
        {

        }



        private void DetectProcessor_OnDetected(List<BarcodeResult> obj)
        {
            OnDetected?.Invoke(obj);

        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

            _surfaceView.Measure(msw, msh);
            _surfaceView.Layout(0, 0, r - l, b - t);

            // SetOrientation();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            throw new NotImplementedException();
        }
    }

   
}


        //public void SetOrientation()
        //{

        //    Android.Hardware.Camera camera = Methods.GetCamera(_cameraSource);
        //    switch (_windowManager.DefaultDisplay.Rotation)
        //    {
        //        case SurfaceOrientation.Rotation0:
        //            camera?.SetDisplayOrientation(90);
        //            break;
        //        case SurfaceOrientation.Rotation90:
        //            camera?.SetDisplayOrientation(0);
        //            break;
        //        case SurfaceOrientation.Rotation180:
        //            camera?.SetDisplayOrientation(270);
        //            break;
        //        case SurfaceOrientation.Rotation270:
        //            camera?.SetDisplayOrientation(180);
        //            break;
        //    }
        //}


//        private class OnSuccessListener : Java.Lang.Object,
//        {
//            public event Action<List<BarcodeResult>> OnDetected;
//            private readonly Context _context;
//            private readonly bool _vibrationOnDetected;

//            public DetectorProcessor(Context context, bool vibrationOnDetected)
//            {
//                _context = context;
//                _vibrationOnDetected = vibrationOnDetected;
//            }

//            public void ReceiveDetections(Detector.Detections detections)
//            {
//                var qrcodes = detections.DetectedItems;
//                if (qrcodes.Size() != 0)
//                {
//                    if (Configuration.IsScanning)
//                    {
//                        Configuration.IsScanning = false;
//                        if (_vibrationOnDetected)
//                        {
//                            Vibrator vib = (Vibrator)_context.GetSystemService(Context.VibratorService);
//                            vib.Vibrate(200);
//                        }
//                        List<BarcodeResult> barcodeResults = new List<BarcodeResult>();
//                        for (int i = 0; i < qrcodes.Size(); i++)
//                        {
//                            Barcode barcode = qrcodes.ValueAt(i) as Barcode;
//                            if (barcode == null) continue;
//                            var type = Methods.ConvertBarcodeResultTypes(barcode.ValueFormat);
//                            var value = barcode.DisplayValue;
//                            barcodeResults.Add(new BarcodeResult
//                            {
//                                BarcodeType = type,
//                                DisplayValue = value
//                            });
//                        }
//                        OnDetected?.Invoke(barcodeResults);
//                    }
//                }
//            }

//            public void Release()
//            {
//            }
//        }

//        private class SurfaceHolderCallback : Java.Lang.Object, ISurfaceHolderCallback
//        {
//            private readonly SurfaceView _cameraPreview;
          

      


//            public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
//            {

//            }

//            public void SurfaceCreated(ISurfaceHolder holder)
//            {
//                try
//                {

//                //    _cameraSource.Start(_cameraPreview.Holder);
//                }
//                catch (System.Exception e)
//                {
//                    Log.Error("BarcodeScanner.Droid", e.Message);
//                }
//            }

//            public void SurfaceDestroyed(ISurfaceHolder holder)
//            {
//               // _cameraSource.Stop();
//            }
//        }
//    }
//}

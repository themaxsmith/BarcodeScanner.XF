using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Vision;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GoogleVisionBarCodeScanner
{
    public static class Configuration
    {
        public static int BarcodeFormats = Firebase.ML.Vision.Barcode.FirebaseVisionBarcode.FormatCode128 | Firebase.ML.Vision.Barcode.FirebaseVisionBarcode.FormatCodabar |
             Firebase.ML.Vision.Barcode.FirebaseVisionBarcode.FormatCode39 | Firebase.ML.Vision.Barcode.FirebaseVisionBarcode.FormatCode93 | Firebase.ML.Vision.Barcode.FirebaseVisionBarcode.FormatDataMatrix |
             Firebase.ML.Vision.Barcode.FirebaseVisionBarcode.FormatEan13 | Firebase.ML.Vision.Barcode.FirebaseVisionBarcode.FormatEan8 | Firebase.ML.Vision.Barcode.FirebaseVisionBarcode.FormatItf |
             Firebase.ML.Vision.Barcode.FirebaseVisionBarcode.FormatPdf417 | Firebase.ML.Vision.Barcode.FirebaseVisionBarcode.FormatQrCode | Firebase.ML.Vision.Barcode.FirebaseVisionBarcode.FormatUpcA | Firebase.ML.Vision.Barcode.FirebaseVisionBarcode.FormatUpcE;
        internal static bool IsScanning = true;
        internal static CameraSource CameraSource;
    }
}

using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Android.OS;
using Android.Util;
using TingVoa.WebView.Views;
using TingVoa.WebView.Models;
using TingVoa.WebView.Services;

namespace TingVoa.WebView
{
    [Activity(Label = "TingVoa.WebView", MainLauncher = true, LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var webView = FindViewById<Android.Webkit.WebView>(Resource.Id.webView);
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.DomStorageEnabled = true;
            webView.Settings.DatabaseEnabled = true;

            // Use subclassed WebViewClient to intercept hybrid native calls
            webView.SetWebViewClient(new HybridWebViewClient(Application.Context));
            webView.AddJavascriptInterface(new MyAudioService(webView), "IAudioService");
            webView.AddJavascriptInterface(new MyWebService(webView), "IWebService");
            webView.AddJavascriptInterface(new MyConfigService(), "IConfigService");
            webView.AddJavascriptInterface(new SpinnerService(webView), "ISpinnerService");
            webView.Settings.AllowUniversalAccessFromFileURLs = true;
            Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
            StreamReader stream = new StreamReader(this.Assets.Open("www/index.html"));
            var html = stream.ReadToEnd();

            webView.LoadDataWithBaseURL("file:///android_asset/www/", html, "text/html", "utf-8", null);
            // Load the rendered HTML into the view with a base URL 
            // that points to the root of the bundled Assets folder

        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnDestroy()
        {
            MyAudioService.Release();
            base.OnDestroy();
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            var webView = FindViewById<Android.Webkit.WebView>(Resource.Id.webView);
            if (new MyConfigService().IsVolumeControl == true)
            {
                if (keyCode == Keycode.VolumeDown)
                {
                    webView.LoadUrl("javascript:nextLyric()");
                    return true;
                }

                if (keyCode == Keycode.VolumeUp)
                {
                    webView.LoadUrl("javascript:previousLyric()");
                    return true;
                }
            }
            return base.OnKeyDown(keyCode, e);
        }

        private class HybridWebViewClient : WebViewClient
        {
            private Context _context;
            private ProgressDialog _progressDialog;
            public HybridWebViewClient(Context context)
            {
                _context = context;
            }

            public override void OnPageStarted(Android.Webkit.WebView view, string url, Bitmap favicon)
            {
                //view.EvaluateJavascript("(function(){alert('sdf'};)()",null);
                _progressDialog = ProgressDialog.Show(view.Context, "请稍候", "正在载入...", true);
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                _progressDialog.Hide();
            }
        }
    }
}


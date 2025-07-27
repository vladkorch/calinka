using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calinka
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AddTab("https://www.bing.com");
        }

        private void AddTab(string url = "https://www.bing.com")
        {
            var tabItem = new TabItem();

            var webView = new WebView2();
            webView.NavigationCompleted += WebView_NavigationCompleted;
            webView.CoreWebView2InitializationCompleted += async (sender, e) =>
            {
                if (e.IsSuccess)
                    webView.CoreWebView2.Navigate(url);
                else
                    MessageBox.Show("WebView2 initialization failed.");
            };
            webView.Source = new Uri("about:blank");
            tabItem.Header = "New Tab";
            tabItem.Content = webView;

            Tabs.Items.Add(tabItem);
            Tabs.SelectedItem = tabItem;
        }

        // Helper to get active WebView2 control
        private WebView2 GetActiveWebView()
        {
            if (Tabs.SelectedItem is TabItem selectedTab && selectedTab.Content is WebView2 webView)
            {
                return webView;
            }
            return null;
        }

        // Update address bar when navigation completes
        private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            var webView = sender as WebView2;
            if (webView != null)
            {
                if (webView.Parent is TabItem tabItem)
                {
                    // Update tab header and address bar if it's the active tab
                    Dispatcher.Invoke(() =>
                    {
                        tabItem.Header = (webView.CoreWebView2?.DocumentTitle ?? webView.Source.ToString())
                 .Substring(0, Math.Min(16, (webView.CoreWebView2?.DocumentTitle ?? webView.Source.ToString()).Length));

                        if (Tabs.SelectedItem == tabItem)
                        {
                            AddressBar.Text = webView.Source.ToString();
                        }
                    });
                }
            }
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            var webView = GetActiveWebView();
            if (webView != null)
            {
                webView.CoreWebView2?.Navigate("https://www.bing.com");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var webView = GetActiveWebView();
            if (webView != null && webView.CoreWebView2 != null && webView.CoreWebView2.CanGoBack)
            {
                webView.CoreWebView2.GoBack();
            }
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            var webView = GetActiveWebView();
            if (webView != null && webView.CoreWebView2 != null && webView.CoreWebView2.CanGoForward)
            {
                webView.CoreWebView2.GoForward();
            }
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            var webView = GetActiveWebView();
            webView?.CoreWebView2?.Reload();
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            NavigateToAddress();
        }

        private void AddressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToAddress();
            }
        }

        private void NavigateToAddress()
        {
            var webView = GetActiveWebView();
            var text = AddressBar.Text.Trim();

            if (webView != null)
            {
                try
                {
                    Uri uri;

                    // If no scheme, assume https
                    if (!Uri.TryCreate(text, UriKind.Absolute, out uri))
                    {
                        uri = new Uri("https://" + text);
                    }
                    else if (string.IsNullOrEmpty(uri.Scheme))
                    {
                        uri = new Uri("https://" + text);
                    }

                    webView.CoreWebView2?.Navigate(uri.ToString());
                }
                catch (UriFormatException)
                {
                    MessageBox.Show("Invalid URL");
                }
            }
        }

        private void AddTab_Click(object sender, RoutedEventArgs e)
        {
            AddTab();
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var webView = GetActiveWebView();
            if (webView != null)
            {
                AddressBar.Text = webView.Source.ToString();
            }
        }
    }
}

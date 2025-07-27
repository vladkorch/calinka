using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Security.Policy;
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
            AddTab("http://[21e:a51c:885b:7db0:166e:927:98cd:d186]");
        }

        private void AddTab(string url = "https://www.bing.com")
        {
            var tabItem = new TabItem();

            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
            var titleText = new TextBlock { Text = "New Tab", Margin = new Thickness(0, 0, 5, 0) };
            var closeButton = new Button
            {
                Content = "x",
                Width = 16,
                Height = 16,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Padding = new Thickness(0),
                BorderThickness = new Thickness(0),
                Background = System.Windows.Media.Brushes.Transparent
            };
            closeButton.Click += (s, e) => CloseTab(tabItem);

            headerPanel.Children.Add(titleText);
            headerPanel.Children.Add(closeButton);
            tabItem.Header = headerPanel;

            var webView = new WebView2();
            webView.NavigationCompleted += WebView_NavigationCompleted;
            webView.CoreWebView2InitializationCompleted += (sender, e) =>
            {
                if (e.IsSuccess)
                {
                    webView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
                    webView.CoreWebView2.Navigate(url);
                }
                else
                    MessageBox.Show("WebView2 initialization failed.");
            };
            webView.Source = new Uri(url);
            tabItem.Content = webView;

            Tabs.Items.Add(tabItem);
            Tabs.SelectedItem = tabItem;
        }
        private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
            
            AddTab(e.Uri);
        }

        private void CloseTab(TabItem tabItem)
        {
            if (tabItem != null)
            {
                Tabs.Items.Remove(tabItem);

                if (Tabs.Items.Count == 0)
                {
                    AddTab();
                }
            }
        }

        private WebView2 GetActiveWebView()
        {
            if (Tabs.SelectedItem is TabItem selectedTab && selectedTab.Content is WebView2 webView)
            {
                return webView;
            }
            return null;
        }

        private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            var webView = sender as WebView2;
            if (webView != null && webView.Parent is TabItem tabItem)
            {
                Dispatcher.Invoke(() =>
                {
                    string title = webView.CoreWebView2?.DocumentTitle ?? webView.Source.ToString();
                    title = title.Substring(0, Math.Min(16, title.Length));

                    if (tabItem.Header is StackPanel header && header.Children.Count > 0 && header.Children[0] is TextBlock textBlock)
                    {
                        textBlock.Text = title;
                    }

                    if (Tabs.SelectedItem == tabItem)
                    {
                        AddressBar.Text = webView.Source.ToString();
                    }
                });
            }
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            var webView = GetActiveWebView();
            webView?.CoreWebView2?.Navigate("https://www.bing.com");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var webView = GetActiveWebView();
            if (webView?.CoreWebView2?.CanGoBack == true)
            {
                webView.CoreWebView2.GoBack();
            }
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            var webView = GetActiveWebView();
            if (webView?.CoreWebView2?.CanGoForward == true)
            {
                webView.CoreWebView2.GoForward();
            }
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            GetActiveWebView()?.CoreWebView2?.Reload();
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
            if (webView == null) return;

            string url = AddressBar.Text.Trim();
            if (string.IsNullOrWhiteSpace(url)) return;

            if (!url.Contains("://") && !url.StartsWith("about:"))
            {
                url = "http://" + url;
            }

            try
            {
                webView.CoreWebView2.Navigate(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not navigate to address: {ex.Message}");
            }
        }

        private void AddTab_Click(object sender, RoutedEventArgs e)
        {
            AddTab();
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var webView = GetActiveWebView();
            if (webView != null && webView.Source != null)
            {
                AddressBar.Text = webView.Source.ToString();
            }
            else
            {
                AddressBar.Text = string.Empty;
            }
        }
    }
}

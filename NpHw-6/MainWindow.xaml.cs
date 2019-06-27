using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NpHw_6
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                wordsListBox.Items.Clear();

                string data = GetHtml();
                data = Regex.Replace(data, "<(.|\\n)*?>", string.Empty);

                List<string> words = new List<string>();
                foreach (string word in data.Split(GetSplitSimbols()
                        , StringSplitOptions.RemoveEmptyEntries))
                {
                    if (word.Length > 1)
                        words.Add(word.ToLower());
                }
                List<string> deleteSentence = new List<string>();

                Dictionary<string, int> vacabulary = new Dictionary<string, int>();
                foreach (var word in words)
                {
                    if (vacabulary.ContainsKey(word))
                    {
                        vacabulary[word]++;
                    }
                    else
                    {
                        vacabulary.Add(word, 1);
                    }
                }

                foreach (KeyValuePair<string, int> orderItem in vacabulary.OrderByDescending(x => x.Value))
                {
                    double prosent = Math.Round(Convert.ToDouble(100) / words.Count * orderItem.Value, 2);
                    wordsListBox.Items.Add($"| {orderItem.Value} | {prosent}% | {orderItem.Key}");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private string GetHtml()
        {
            Uri url = new Uri(urlTextBox.Text);
            HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
            httpRequest.Method = WebRequestMethods.Http.Post;
            HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;

            var encoding = Encoding.GetEncoding(httpResponse.CharacterSet);
            using (var responseStream = httpResponse.GetResponseStream())
            using (var reader = new StreamReader(responseStream, encoding))
                return reader.ReadToEnd();
        }

        private static char[] GetSplitSimbols()
        {
            List<char> allSimbol = new List<char>();
            for (int i = 1; i < 257; i++)
            {
                allSimbol.Add((char)i);
            }
            List<char> engSimbols = new List<char>();
            for (int i = 65; i < 91; i++)
            {
                allSimbol.Remove((char)i);
            }
            List<char> rusSimbols = new List<char>();
            for (int i = 1040; i < 1072; i++)
            {
                allSimbol.Remove((char)i);
                if (i == 1045)
                    allSimbol.Remove((char)1025);
            }
            return allSimbol.ToArray();
        }
    }
}

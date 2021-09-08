using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Flow.FormatProviders.Pdf;
using Telerik.Windows.Documents.Flow.FormatProviders.Rtf;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.XamarinForms.PdfViewer;
using Xamarin.Forms;

namespace ticket_1534583
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
    }
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            TestWithUrlCommand = new Command(OnTestWithUrl);
            RtfTextOnlyCommand = new Command<object>(OnRtfTextOnly);
            RtfWithImageCommand = new Command<object>(OnRtfWithImage);
        }

        DocumentSource _Source = null;
        public DocumentSource Source
        {
            get => _Source;
            set
            {
                if(!ReferenceEquals(_Source, value))
                {
                    _Source = value;
                    OnPropertyChanged();
                }
            }
        }

        async Task<string> GetRtfStringFromURL(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            var stream = await response.Content.ReadAsStreamAsync();
            string rtfString = await new StreamReader(stream).ReadToEndAsync();
            return rtfString;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ICommand TestWithUrlCommand { get; }
        private void OnTestWithUrl(object o)
        {
            Source = new Uri("https://www.ivsoftware.com/sasquatch/terms-of-service.pdf");
        }

        public ICommand RtfTextOnlyCommand { get; }
        private async void OnRtfTextOnly(object arg)
        {
            var providerRTF = new RtfFormatProvider();
            var providerPDF = new PdfFormatProvider();

            var rtfString = await GetRtfStringFromURL("https://www.ivsoftware.com/proto-21/text-only.rtf");
            RadFlowDocument doc = providerRTF.Import(rtfString);
            RadFixedDocument fixedDocument = providerPDF.ExportToFixedDocument(doc);

            Source = fixedDocument;
        }

        public ICommand RtfWithImageCommand { get; }
        private async void OnRtfWithImage(object arg)
        {
            var providerRTF = new RtfFormatProvider();
            var providerPDF = new PdfFormatProvider();

            var rtfString = await GetRtfStringFromURL("https://www.ivsoftware.com/proto-21/malabre-document.rtf");
            RadFlowDocument doc = providerRTF.Import(rtfString);
            RadFixedDocument fixedDocument = providerPDF.ExportToFixedDocument(doc);

            Source = fixedDocument;
        }
    }
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Win32;
using Steganography.Models;
using Steganography.Shared;
using Steganography;
using Steganography.Decrypt;
using Steganography.Encrypt;
using Color = System.Drawing.Color;

namespace Steganography
{
    public class MainCalculations : INotifyPropertyChanged
    {
        private ImageInfo _downloadedImage;
        private byte[] _inputKeyInfo;
        public ImageSource ImageBinding { get; private set; }

        public ICommand OpenFileDialogCommand { get; }

        public ICommand DecryptCommand { get; }

        public ICommand EncryptCommand { get; }

        public ICommand OpenFileDialogKeyCommand { get; }

        public ICommand OpenFileDialogTextCommand { get; }

        private string _message;

        /// <summary>
        /// Message for encryption
        /// </summary>
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        private string _statusText;

        /// <summary>
        /// Text for status textblock
        /// </summary>
        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                RaisePropertyChanged("StatusText");
            }
        }

        public Bitmap bm { set; get; }

        /// <summary>
        /// Constructor that instantiate the commands
        /// </summary>
        public MainCalculations()
        {
            OpenFileDialogCommand = new Command(ExecuteOpenFileDialog);
            OpenFileDialogKeyCommand = new Command(ExecuteOpenFileKeyDialog);
            DecryptCommand = new Command(ExecuteDecrypt);
            EncryptCommand = new Command(ExecuteEncrypt);
            OpenFileDialogTextCommand = new Command(ExecuteOpenFileDialogText);
        }

        private void ExecuteOpenFileDialogText()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Message = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void ExecuteOpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "Image files (*.BMP, *.JPG, *.GIF, *.TIF, *.PNG, *.ICO, *.EMF, *.WMF)|*.bmp;*.jpg;*.gif; *.tif; *.png; *.ico; *.emf; *.wmf"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                using (var stream = new FileStream(openFileDialog.FileName, FileMode.Open))
                {

                    ImageBinding = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    RaisePropertyChanged("ImageBinding");
                    if (ImageBinding.Width < 4 || ImageBinding.Height < 4)
                    {
                        MessageBox.Show("Please choose a better image");
                        return;
                    }
                    Image image = Image.FromStream(stream);
                    Bitmap bitmap = new Bitmap(stream);
                    _downloadedImage = new ImageInfo(image, bitmap, openFileDialog.FileName);
                    _downloadedImage.Pixels = ImageProcessing.BitmapToArray2D(_downloadedImage.Bitmap);

                }
            }
        }

        private void ExecuteOpenFileKeyDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "Text files (*.txt)|*.txt"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _inputKeyInfo = File.ReadAllBytes(openFileDialog.FileName);
            }
        }

        private void ExecuteEncrypt()
        {
            if (_downloadedImage == null || String.IsNullOrWhiteSpace(Message))
            {
                MessageBox.Show("Please choose an image and/or provide a text for encrypting.");
                return;
            }
            ImageEncoder encoder = new ImageEncoder(_downloadedImage);
            Pixel firstCoordinates = encoder.GetFirstRandomCoordinates();
            string textBlock = Message;
            int offsetCount = encoder.CalcuteOffset(firstCoordinates);
            var symbolsAndCoordinates = encoder.GetAllColors(textBlock, firstCoordinates);
            //TODO add color analyzer
            //symbolsAndCoordinates = encoder.AddUniqueNumberToPixel(symbolsAndCoordinates);
            var symbolsAndHashes = encoder.CreateOutputHashesInfo(symbolsAndCoordinates);
            var noise = encoder.GenerateSymbolNoise(symbolsAndCoordinates);
            OutputInfo outputInfo = new OutputInfo(firstCoordinates, offsetCount,
                symbolsAndHashes, noise, textBlock.Length);
            OutputProcessing outputProcessing = new OutputProcessing(outputInfo);
            string outputData = outputProcessing.CreateOutputString();
            RijndaelAlgorithm rijndaelAlgorithm = new RijndaelAlgorithm(_downloadedImage);
            byte[] dataForSaving = rijndaelAlgorithm.Encrypt(outputData);
            SaveOutput(dataForSaving);
        }

        private void ExecuteDecrypt()
        {
            if (_inputKeyInfo == null || _downloadedImage == null)
            {
                MessageBox.Show("Please choose an image and/or key file.");
                return;
            }
            RijndaelAlgorithm rijndaelAlgorithm = new RijndaelAlgorithm(_downloadedImage);
            string decriptedData;
            try
            {
                decriptedData = rijndaelAlgorithm.Decrypt(_inputKeyInfo);
            }
            catch (CryptographicException)
            {
                MessageBox.Show("An error occurred during the decrypting!");
                return;
            }
            Parser parseData = new Parser(decriptedData);
            Color firstColor = parseData.GetColorFromString();
            int offsetOfFirstColor = parseData.GetOffsetOfcolor();
            var symbolsAndHashes = parseData.GetInfoAboutAllSymbols();
            int lengthOfText = parseData.GetLengthOfText();
            ParsedData parsedData = new ParsedData(firstColor, offsetOfFirstColor,
                symbolsAndHashes, lengthOfText);

            ImageDecoder imageDecoder = new ImageDecoder(parsedData, _downloadedImage);
            Pixel firstValue = imageDecoder.GetCoordinatesOfFirst();

            string text = imageDecoder.DecryptText(firstValue);
            SaveOutput(text);
        }

        private void SaveOutput(object data)
        {
            SaveFileDialog svd = new SaveFileDialog()
            {
                Filter = "Text files (*.txt)|*.txt",
                DefaultExt = "txt"
            };
            if (svd.ShowDialog() == true)
            {
                if (data is byte[])
                {
                    File.WriteAllBytes(svd.FileName, (byte[])data);
                }
                else if (data is string)
                {
                    File.WriteAllText(svd.FileName, data.ToString());
                }
            }
        }

        /// <summary>
        /// Notify when property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Command : ICommand
    {
        public Command(Action action)
        {
            this.action = action;
        }

        Action action;

        EventHandler _canExecuteChanged;
        event EventHandler ICommand.CanExecuteChanged
        {
            add => _canExecuteChanged += value;
            remove
            {
                if (_canExecuteChanged != null) _canExecuteChanged -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action();
        }
    }
}

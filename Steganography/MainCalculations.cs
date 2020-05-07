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
using Brush = System.Windows.Media.Brush;
using Color = System.Drawing.Color;
using System.Windows.Controls.Primitives;

namespace Steganography
{
    public class MainCalculations : INotifyPropertyChanged, IDisposable
    {
        private ImageInfo _downloadedImage;
        private byte[] _inputKeyInfo;
        public ImageSource ImageBinding { get; private set; }

        public ICommand OpenFileDialogCommand { get; }

        public ICommand DecryptCommand { get; }

        public ICommand EncryptCommand { get; }

        public ICommand OpenFileDialogKeyCommand { get; }

        public ICommand OpenFileDialogTextCommand { get; }

        public string TextLimit
        {
            get => $"Limit: {LengthLimitation} symbols.";
        }

        private readonly int _lengthLimitation = 10000;

        /// <summary>
        /// Limit of length for text
        /// </summary>
        public int LengthLimitation
        {
            get => _lengthLimitation;
        }

        private string _message;

        /// <summary>
        /// Message for encryption
        /// </summary>
        public string Message
        {
            get => _message;
            set
            {
                if (value.Length > LengthLimitation)
                {
                    value = value.Substring(0, LengthLimitation);
                }
                for (int i= 0; i< value.Length; i++) {
                    int convertedValue = Convert.ToInt32(value[i]);
                    if (convertedValue>127 || convertedValue<32)
                    {
                        _message = string.Empty;
                        Notify.Invoke(StatusStrings.ErrorDuringReading, Color.IndianRed);
                        return;
                    }
                }
                _message = value;
                Notify?.Invoke(StatusStrings.TextRead, Color.SeaGreen);
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

        private Brush _statusColor;

        public Brush StatusColor
        {
            get => _statusColor;
            set
            {
                _statusColor = value;
                RaisePropertyChanged("StatusColor");
            }
        }

        private void ChangeStatusSettings(string message, Color color)
        {
            StatusText = message;
            StatusColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public delegate void StatusHandler(string message, Color color);

        public static event StatusHandler Notify;

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
            Notify += ChangeStatusSettings;
        }

        private void ExecuteOpenFileDialogText()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            Notify?.Invoke(StatusStrings.ReadingTextFromFile, Color.DeepSkyBlue);
            if (openFileDialog.ShowDialog() == true)
            {
                Message = File.ReadAllText(openFileDialog.FileName);
            }
            else
            {
                Notify?.Invoke(StatusStrings.OperationStopped, Color.IndianRed);
            }
        }

        private void ExecuteOpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "Image files (*.BMP, *.JPG, *.GIF, *.TIF, *.PNG, *.ICO, *.EMF, *.WMF)|*.bmp;*.jpg;*.gif; *.tif; *.png; *.ico; *.emf; *.wmf"
            };
            Notify?.Invoke(StatusStrings.ImageAdding, Color.DeepSkyBlue);
            if (openFileDialog.ShowDialog() == true)
            {
                using (var stream = new FileStream(openFileDialog.FileName, FileMode.Open))
                {

                    ImageBinding = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                    if (ImageBinding.Width*ImageBinding.Height < 10000)
                    {
                        MessageBox.Show("Please choose a better image");
                        return;
                    }
                    Image image = Image.FromStream(stream);
                    Bitmap bitmap = new Bitmap(stream);
                    _downloadedImage = new ImageInfo(image, bitmap, openFileDialog.FileName);
                    _downloadedImage.Pixels = ImageProcessing.BitmapToArray2D(_downloadedImage.Bitmap);
                    RaisePropertyChanged("ImageBinding");
                    Notify?.Invoke(StatusStrings.ImageAdded, Color.SeaGreen);
                }
            }
            else
            {
                Notify?.Invoke(StatusStrings.OperationStopped, Color.IndianRed);
            }
        }

        private void ExecuteOpenFileKeyDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "Text files (*.txt)|*.txt"
            };
            Notify?.Invoke(StatusStrings.ReadingKeyFile, Color.DeepSkyBlue);
            if (openFileDialog.ShowDialog() == true)
            {
                _inputKeyInfo = File.ReadAllBytes(openFileDialog.FileName);
                Notify?.Invoke(StatusStrings.KeyFileAdded, Color.SeaGreen);
            }
            else
            {
                Notify?.Invoke(StatusStrings.OperationStopped, Color.IndianRed);
            }
        }

        private void ExecuteEncrypt()
        {
            if (_downloadedImage == null || String.IsNullOrWhiteSpace(Message))
            {
                MessageBox.Show("Please choose an image and/or provide a text for encrypting.");
                Notify?.Invoke(StatusStrings.MissingFile, Color.IndianRed);
                return;
            }
            Notify?.Invoke(StatusStrings.Encryption, Color.DeepSkyBlue);
            ImageEncoder encoder = new ImageEncoder(_downloadedImage);
            Pixel firstCoordinates = encoder.GetFirstRandomCoordinates();
            string textBlock = Message;
            int offsetCount = encoder.CalcuteOffset(firstCoordinates);
            var symbolsAndCoordinates = encoder.GetAllColors(textBlock, firstCoordinates);
            Notify?.Invoke(StatusStrings.CreatingHashes, Color.DeepSkyBlue);
            var symbolsAndHashes = encoder.CreateOutputHashesInfo(symbolsAndCoordinates);
            var noise = encoder.GenerateSymbolNoise(symbolsAndCoordinates);
            OutputInfo outputInfo = new OutputInfo(firstCoordinates, offsetCount,
                symbolsAndHashes, noise, textBlock.Length);
            OutputProcessing outputProcessing = new OutputProcessing(outputInfo);
            string outputData = outputProcessing.CreateOutputString();
            RijndaelAlgorithm rijndaelAlgorithm = new RijndaelAlgorithm(_downloadedImage);
            byte[] dataForSaving = rijndaelAlgorithm.Encrypt(outputData);
            Notify?.Invoke(StatusStrings.SavingData, Color.DeepSkyBlue);
            SaveOutput(dataForSaving);
        }

        private void ExecuteDecrypt()
        {
            if (_inputKeyInfo == null || _downloadedImage == null)
            {
                MessageBox.Show("Please choose an image and/or key file.");
                Notify?.Invoke(StatusStrings.MissingFile, Color.IndianRed);
                return;
            }
            RijndaelAlgorithm rijndaelAlgorithm = new RijndaelAlgorithm(_downloadedImage);
            string decriptedData;
            try
            {
                decriptedData = rijndaelAlgorithm.Decrypt(_inputKeyInfo);
            }
            catch (Exception)
            {
                MessageBox.Show(StatusStrings.ErrorDecrypting);
                Notify?.Invoke(StatusStrings.ErrorDecrypting, Color.IndianRed);
                return;
            }
            Notify?.Invoke(StatusStrings.StartedParsing, Color.DeepSkyBlue);
            Parser parseData = new Parser(decriptedData);
            Color firstColor = parseData.GetColorFromString();
            int offsetOfFirstColor = parseData.GetOffsetOfcolor();
            var symbolsAndHashes = parseData.GetInfoAboutAllSymbols();
            int lengthOfText = parseData.GetLengthOfText();
            ParsedData parsedData = new ParsedData(firstColor, offsetOfFirstColor,
                symbolsAndHashes, lengthOfText);
            Notify?.Invoke(StatusStrings.StartingDecoding, Color.DeepSkyBlue);
            ImageDecoder imageDecoder = new ImageDecoder(parsedData, _downloadedImage);
            Pixel firstValue = imageDecoder.GetCoordinatesOfFirst();
            Notify?.Invoke(StatusStrings.DataDecoded, Color.SeaGreen);
            string text = string.Empty;
            try
            {
                text = imageDecoder.DecryptText(firstValue);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to decrypt this file." + e.Message);
                return;
            }
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
            Notify?.Invoke(StatusStrings.DataSaved, Color.SeaGreen);
        }

        /// <summary>
        /// Notify when property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            Notify -= ChangeStatusSettings;
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
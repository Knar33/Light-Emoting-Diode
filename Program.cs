using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using RestSharp;
using System.Configuration;
using RestSharp.Deserializers;
using WebEye;
using WebEye.Controls.Wpf;
using System.Windows;
using System.Threading;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using LedCSharp;

namespace Light_Emoting_Diode
{
    class Program
    {
        static SerialPort port;

        [STAThread]
        static void Main(string[] args)
        {


            int baud = 9600;
            string portName = "COM6";

            Console.Write("Using ports {0}. Change ports? (y/n) ", portName);
            string changePort = Console.ReadLine();
            if (changePort == "y")
            {
                Console.WriteLine(" ");
                Console.WriteLine("Available ports:");
                if (SerialPort.GetPortNames().Count() >= 0)
                {
                    foreach (string port in SerialPort.GetPortNames())
                    {
                        Console.WriteLine(port);
                    }
                }
                else
                {
                    Console.WriteLine("No Ports available.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("Choose Port:");
                portName = Console.ReadLine();
            }
            
            Console.WriteLine(" ");
            Console.Write("Using baud rate {0}. Change rate? (y/n) ", baud);
            string changeRate = Console.ReadLine();
            if (changeRate == "y")
            {
                Console.WriteLine(" ");
                Console.Write("Baud rate: ");
                baud = GetBaudRate();
            }

            Console.WriteLine(" ");
            Console.WriteLine("Beging Serial...");
            BeginSerial(baud, portName);
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            port.Open();
            Console.WriteLine("Serial Started.");
            Console.WriteLine(" ");
            Console.WriteLine("Sending emotions");
            
            Thread newWindowThread = new Thread(new ThreadStart(() =>  
            {
                System.Windows.Threading.Dispatcher.Run();  
            }));  
            newWindowThread.SetApartmentState(ApartmentState.STA);  
            newWindowThread.IsBackground = true;  
            newWindowThread.Start();

            System.Drawing.Bitmap image = null;
            var webCameraControl1 = new WebCameraControl();
            List<WebCameraId> cameras = new List<WebCameraId>(webCameraControl1.GetVideoCaptureDevices());
            webCameraControl1.StartCapture(cameras[0]);

            var client = new RestClient("https://api-us.faceplusplus.com/facepp/v3");
            client.AddHandler("application/json", new JsonDeserializer());
            string apiKey = ConfigurationManager.AppSettings["api_key"];
            string apiSecret = ConfigurationManager.AppSettings["api_secret"];

            DateTime compareTime = DateTime.Now;

            LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_ALL);
            while (true)
            {
                DateTime currentTime = DateTime.Now;
                if (currentTime > compareTime.AddSeconds(5))
                {
                    image = webCameraControl1.GetCurrentImage();
                    image.Save("C:\\Users\\USER_0137\\Documents\\whatever.bmp");

                    MemoryStream ms = new MemoryStream();
                    image.Save(ms, ImageFormat.Jpeg);
                    byte[] byteImage = ms.ToArray();
                    string imageString = Convert.ToBase64String(byteImage);

                    var request = new RestRequest("detect", Method.POST);
                    request.AddParameter("api_key", apiKey);
                    request.AddParameter("api_secret", apiSecret);
                    request.AddParameter("image_base64", imageString);
                    request.AddParameter("return_attributes", "emotion");

                    try
                    {
                        var response = client.Execute<FPPResponse>(request);
                        var fppResponse = response.Data;
                        var emotionValue = CalculateEmotion(fppResponse.faces[0].attributes.emotion);

                        //Debug stuff
                        Console.WriteLine(emotionValue.Item2);
                        LogitechGSDK.LogiLedSetLighting(0, 0, 0);

                        port.WriteLine(emotionValue.Item1.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    compareTime = DateTime.Now;
                }
            }
        }

        static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            for (int i = 0; i < (10000 * port.BytesToRead) / port.BaudRate; i++);
            Console.Write(port.ReadExisting());
            Console.WriteLine("");
            Console.Write("> ");
        }

        static void BeginSerial(int baud, string name)
        {
            port = new SerialPort(name, baud);
        }

        static int GetBaudRate()
        {
            try
            {
                return int.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Invalid integer.  Please try again:");
                return GetBaudRate();
            }
        }

        public static Tuple<int, string> CalculateEmotion(Emotion emotion)
        {
            int emotionValue = 0;
            Tuple<int, string> returnValue = null;

            if (emotion.anger > emotionValue)
            {
                emotionValue = emotion.anger;
                returnValue = new Tuple<int, string>(1, "anger");
            }
            if (emotion.fear > emotionValue)
            {
                emotionValue = emotion.fear;
                returnValue = new Tuple<int, string>(2, "fear");
            }
            if (emotion.happiness > emotionValue)
            {
                emotionValue = emotion.happiness;
                returnValue = new Tuple<int, string>(3, "happiness");
            }
            if (emotion.disgust > emotionValue)
            {
                emotionValue = emotion.disgust;
                returnValue = new Tuple<int, string>(4, "disgust");
            }
            if (emotion.neutral > emotionValue)
            {
                emotionValue = emotion.neutral;
                returnValue = new Tuple<int, string>(5, "neutral");
            }
            if (emotion.sadness > emotionValue)
            { 
                emotionValue = emotion.sadness;
                returnValue = new Tuple<int, string>(6, "sadness");
            }
            if (emotion.surprise > emotionValue)
            {
                emotionValue = emotion.surprise;
                returnValue = new Tuple<int, string>(7, "surprise");
            }

            return returnValue;
        }
    }
}

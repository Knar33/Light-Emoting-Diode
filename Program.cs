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
using Spectral;

namespace Light_Emoting_Diode
{
    class Program
    {
        static SerialPort port;

        [STAThread]
        static void Main(string[] args)
        {
            int[] red = new int[3] { 255, 0, 0 };
            int[] orange = new int[3] { 255, 50, 0 };
            int[] yellow = new int[3] { 255, 200, 0 };
            int[] green = new int[3] { 0, 255, 0 };
            int[] teal = new int[3] { 0, 127, 255 };
            int[] blue = new int[3] { 0, 0, 255 };
            int[] purple = new int[3] { 255, 0, 255 };

            int[] currentColor = new int[3] { 0, 0, 0 };
            int[] futureColor = null;

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
                        
                        switch(emotionValue)
                        {
                            case 1:
                                futureColor = red;
                                break;
                            case 2:
                                futureColor = orange;
                                break;
                            case 3:
                                futureColor = yellow;
                                break;
                            case 4:
                                futureColor = green;
                                break;
                            case 5:
                                futureColor = teal;
                                break;
                            case 6:
                                futureColor = blue;
                                break;
                            case 7:
                                futureColor = purple;
                                break;
                        }
                        
                        port.WriteLine(emotionValue.ToString());

                        for (int i = 0; i < 255; i++)
                        {
                            //change red
                            if (futureColor[0] != currentColor[0])
                            {
                                if (futureColor[0] > currentColor[0])
                                {
                                    currentColor[0]++;
                                }
                                else
                                {
                                    currentColor[0]--;
                                }
                            }

                            //change green
                            if (futureColor[1] != currentColor[1])
                            {
                                if (futureColor[1] > currentColor[1])
                                {
                                    currentColor[1]++;
                                }
                                else
                                {
                                    currentColor[1]--;
                                }
                            }

                            //change blue
                            if (futureColor[2] != currentColor[2])
                            {
                                if (futureColor[2] > currentColor[2])
                                {
                                    currentColor[2]++;
                                }
                                else
                                {
                                    currentColor[2]--;
                                }
                            }
                            
                            Led.SetColorForDevice(Spectral.DeviceType.Keyboard, (byte)currentColor[0], (byte)currentColor[1], (byte)currentColor[2]);

                            Thread.Sleep(4);
                        }
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

        public static int CalculateEmotion(Emotion emotion)
        {
            int emotionValue = 0;

            Dictionary<int, int> emotions = new Dictionary<int, int>()
            {
                { 1, emotion.anger },
                { 2, emotion.fear },
                { 3, emotion.happiness },
                { 4, emotion.disgust },
                { 5, emotion.neutral },
                { 6, emotion.sadness },
                { 7, emotion.surprise },
            };

            int returnValue = emotions.OrderByDescending(x => x.Value).First().Key;

            return returnValue;
        }
    }
}

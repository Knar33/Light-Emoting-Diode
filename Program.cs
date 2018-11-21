using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using RestSharp;
using System.Configuration;
using RestSharp.Deserializers;

namespace Light_Emoting_Diode
{
    class Program
    {
        static SerialPort port;
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

            var client = new RestClient("https://api-us.faceplusplus.com/facepp/v3");
            client.AddHandler("application/json", new JsonDeserializer());
            string apiKey = ConfigurationManager.AppSettings["api_key"];
            string apiSecret = ConfigurationManager.AppSettings["api_secret"];

            DateTime compareTime = DateTime.Now;

            while (true)
            {
                DateTime currentTime = DateTime.Now;
                if (currentTime > compareTime.AddSeconds(2))
                {
                    Console.WriteLine("tick");

                    var request = new RestRequest("detect", Method.POST);
                    request.AddParameter("api_key", apiKey);
                    request.AddParameter("api_secret", apiSecret);
                    request.AddParameter("image_url", "https://images-na.ssl-images-amazon.com/images/I/61kYheRISzL._AC_UL320_SR268,320_.jpg");
                    request.AddParameter("return_attributes", "emotion");

                    var response = client.Execute<FPPResponse>(request);
                    var name = response.Data;

                    compareTime = DateTime.Now;
                }

                //Console.WriteLine(" ");
                //Console.Write("> ");
                //port.WriteLine(Console.ReadLine());

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

        public int CalculateEmotion(Emotion emotion)
        {
            int emotionValue = 0;
            int returnValue = 0;

            if (emotion.anger > emotionValue)
            {
                emotionValue = emotion.anger;
                returnValue = 1;
            }
            if (emotion.fear > emotionValue)
            {
                emotionValue = emotion.fear;
                returnValue = 2;
            }
            if (emotion.happiness > emotionValue)
            {
                emotionValue = emotion.happiness;
                returnValue = 3;
            }
            if (emotion.disgust > emotionValue)
            {
                emotionValue = emotion.disgust;
                returnValue = 4;
            }
            if (emotion.neutral > emotionValue)
            {
                emotionValue = emotion.neutral;
                returnValue = 5;
            }
            if (emotion.sadness > emotionValue)
            { 
                emotionValue = emotion.sadness;
                returnValue = 6;
            }
            if (emotion.surprise > emotionValue)
            {
                emotionValue = emotion.surprise;
                returnValue = 7;
            }

            return emotionValue;
        }
    }
}

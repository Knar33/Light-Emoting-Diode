using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace Light_Emoting_Diode
{
    class Program
    {
        static SerialPort port;
        static void Main(string[] args)
        {
            int baud = 1203;
            string name = "COM3";

            Console.WriteLine("Using ports {0}. Change ports? (y/n)", name);
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
                name = Console.ReadLine();
            }
            
            Console.WriteLine(" ");
            Console.WriteLine("Using baud rate {0}. Change rate? (y/n)", name);
            string changeRate = Console.ReadLine();
            if (changeRate == "y")
            {
                Console.WriteLine(" ");
                Console.WriteLine("Baud rate:");
                baud = GetBaudRate();
            }

            Console.WriteLine(" ");
            Console.WriteLine("Beging Serial...");
            BeginSerial(baud, name);
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            port.Open();
            Console.WriteLine("Serial Started.");
            Console.WriteLine(" ");
            Console.WriteLine("Send:");

            for (; ; )
            {
                Console.WriteLine(" ");
                Console.WriteLine("> ");
                port.WriteLine(Console.ReadLine());
            }
        }

        static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            for (int i = 0; i < (10000 * port.BytesToRead) / port.BaudRate; i++)
                ;       //Delay a bit for the serial to catch up
            Console.Write(port.ReadExisting());
            Console.WriteLine("");
            Console.WriteLine("> ");
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
    }
}

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
            Console.WriteLine("Send:");

            for (; ; )
            {
                Console.WriteLine(" ");
                Console.Write("> ");
                port.WriteLine(Console.ReadLine());
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
    }
}

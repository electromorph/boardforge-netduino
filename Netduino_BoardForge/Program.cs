using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO.Ports;

namespace BoardForgeFirmware
{
    public class Program
    {
        static SerialPort serial;
        
        public static void Main()
        {
            // initialize the serial port for COM1 (using D0 & D1)
            serial = new SerialPort(SerialPorts.COM1, 9600, Parity.None, 8, StopBits.One);
            // open the serial-port, so we can send & receive data
            serial.Open();
            // add an event-handler for handling incoming data
            serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);
            OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
            for (int i = 0; i < 3; i++)
            {
                led.Write(true); // turn on the LED
                Thread.Sleep(250); // sleep for 250ms
                led.Write(false); // turn off the LED
                Thread.Sleep(250); // sleep for 250ms
            }

            // wait forever...
            Thread.Sleep(Timeout.Infinite);
            
            // write your code here
            //OutputPort BlueLed = new OutputPort(Pins.ONBOARD_LED, false);
            //OutputPort UVLed = new OutputPort(Pins.GPIO_PIN_D8, false);
            //Motor LedMotor = new Motor(
            //    new DigitalPin(Pins.GPIO_PIN_D0),
            //    new DigitalPin(Pins.GPIO_PIN_D1),
            //    new DigitalPin(Pins.GPIO_PIN_D2),
            //    new DigitalPin(Pins.GPIO_PIN_D3)
            //    );
            //Motor RollerMotor = new Motor(
            //    new DigitalPin(Pins.GPIO_PIN_D4),
            //    new DigitalPin(Pins.GPIO_PIN_D5),
            //    new DigitalPin(Pins.GPIO_PIN_D6),
            //    new DigitalPin(Pins.GPIO_PIN_D7)
            //    );
            //while (true)
            //{
            //    BlueLed.Write(true);
            //    UVLed.Write(true);
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        Roll(RollerMotor, LedMotor);
            //    }
            //    BlueLed.Write(false);
            //    UVLed.Write(false);
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        Roll(RollerMotor, LedMotor);
            //    }
            //}
        }

        static void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // create a single byte array
            byte[] bytes = new byte[1];
            // as long as there is data waiting to be read
            while (serial.BytesToRead > 0)
            {
                // read a single byte
                serial.Read(bytes, 0, bytes.Length);
                // send the same byte back
                serial.Write(bytes, 0, bytes.Length);
                OutputPort led1 = new OutputPort(Pins.ONBOARD_LED, false);
                led1.Write(true); // turn on the LED
                Thread.Sleep(250); // sleep for 250ms
                led1.Write(false); // turn off the LED
                Thread.Sleep(250); // sleep for 250ms
            }
        }

        public static void ProcessCommand(byte[] command)
        {
            string commandString = command.ToString();

        }
        //public static void Roll(Motor RollerMotor, Motor LedMotor)
        //{
        //    RollerMotor.Step(false);
        //    LedMotor.Step(false);
        //    Thread.Sleep(1);
        //    LedMotor.Step(false);
        //    RollerMotor.Step(false);
        //    Thread.Sleep(1);
        //}
    }
}

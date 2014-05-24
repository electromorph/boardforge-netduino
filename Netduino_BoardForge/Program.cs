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
        static OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
        static StepperMotor LedMotor;
        static StepperMotor RollerMotor;
        static OutputPort UVLedTop;
        static OutputPort UVLedBottom;

        public static void Main()
        {
            // initialize the serial port for COM1 (using D0 & D1)
            serial = new SerialPort(SerialPorts.COM1, 9600, Parity.None, 8, StopBits.One);
            // open the serial-port, so we can send & receive data
            serial.Open();
            // add an event-handler for handling incoming data
            serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);
            
            //Confirmation that we're alive! 7 flashes of the LED.
            for (int i = 0; i < 7; i++)
            {
                led.Write(true);   // turn on the LED
                Thread.Sleep(250); // sleep for 250ms
                led.Write(false);  // turn off the LED
                Thread.Sleep(250); // sleep for 250ms
            }
            
            // write your code here
            //OutputPort BlueLed = new OutputPort(Pins.ONBOARD_LED, false);
            UVLedTop = new OutputPort(Pins.GPIO_PIN_D10, false);
            UVLedTop = new OutputPort(Pins.GPIO_PIN_D11, false);
            LedMotor = new StepperMotor(
                new DigitalPin(Pins.GPIO_PIN_D2),
                new DigitalPin(Pins.GPIO_PIN_D3),
                new DigitalPin(Pins.GPIO_PIN_D4),
                new DigitalPin(Pins.GPIO_PIN_D5)
                );
            RollerMotor = new StepperMotor(
                new DigitalPin(Pins.GPIO_PIN_D6),
                new DigitalPin(Pins.GPIO_PIN_D7),
                new DigitalPin(Pins.GPIO_PIN_D8),
                new DigitalPin(Pins.GPIO_PIN_D9)
                );
            while (true)
            {
                //Just spin around awaiting a command.
                led.Write(true);
                Thread.Sleep(100); // sleep for 100ms
                //UVLed.Write(false);
                led.Write(false);
                Thread.Sleep(100); // sleep for 100ms
            }
            // wait forever...
            //Thread.Sleep(Timeout.Infinite);
        }

        //Commands
        //A - Turn blue LED on or off
        //B - Turn motor left x steps.
        //C - Turn motor right x steps.
        //D = Turn UV LED1 on or off
        //E = Turn UV LED2 on or off
        //F = Write a line of dots (100 dots each 20 steps wide)
        //G = Pulse drill
        
        static void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // create a single byte array
            byte[] bytes = new byte[128];
            // as long as there is data waiting to be read
            
            while (serial.BytesToRead > 0)
            {
                // read as many bytes as they are.
                serial.Read(bytes, 0, bytes.Length);
                byte command = bytes[0];
                int number = 0;
                if (command != 69)
                {
                    number = 1000 * AsciiToNumber(bytes[1]) + 100 * AsciiToNumber(bytes[2]) + 
                            10 * AsciiToNumber(bytes[3]) + AsciiToNumber(bytes[4]);
                }
                bool direction = (bytes[5] == 82);
                // A - Turn Blue LED on/off
                if (command == 65)
                {
                    led.Write(direction); // turn on the LED
                }
                // B - Roller motor  e.g. B0024R  - Turn 24 steps to the right.
                if (command == 66)
                {
                    Roll(RollerMotor, number, direction);
                }
                // C - LED Motor  e.g. C0012L - Turn 12 steps to the left
                if (command == 67)
                {
                    Roll(LedMotor, number, direction);
                }
                // D - Turn UV LED on/off  DxxxxL - turn it off / DxxxxR - turn it on
                if (command == 68)
                {
                    UVLedTop.Write(direction);
                }
                // E - Turn UV LED on/off  DxxxxL - turn it off / DxxxxR - turn it on
                if (command == 68)
                {
                    UVLedBottom.Write(direction);
                }
                // F - write line
                if (command == 69)
                {
                    UVWriteLine(direction, bytes);
                }
                // send the same byte back
                //serial.Write(bytes, 0, bytes.Length);
                string totalCommand = (char)command + "-" + number.ToString() + "-" + direction.ToString();
                byte[] reply = StringToBytes(totalCommand);
                serial.Write(reply, 0, reply.Length);
            }
        }

        private class UVSpot
        {
            public bool TopSpot { get; set; }
            public bool BottomSpot { get; set; }
        }

        static void UVWriteLine(bool direction, byte[] bytes)
        {
            UVSpot[] spots = new UVSpot[64];
            //i starts at 3.  First byte is crap, second is the command, the third is the direction.
            for (int i = 2; i < bytes.Length; i=i+2)
            {
                spots[i / 2].TopSpot = (bytes[i] == 1);
                spots[i / 2].BottomSpot = (bytes[i + 1] == 1);
            }
            foreach (UVSpot spot in spots)
            {
                UVLedTop.Write(spot.TopSpot);
                UVLedBottom.Write(spot.BottomSpot);
                //Roll 10 steps
                Roll(LedMotor, 20, direction);
            }
        }

        static byte[] StringToBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            int position=0;
            foreach (char c in str.ToCharArray())
            {
                bytes[position++] = (byte)c;
            }
            return bytes;
        }

        public static int AsciiToNumber(byte inNumber)
        {
            return ((inNumber < 48) || (inNumber > 57)) ? 0 : (inNumber - 48);
        }

        public static void ProcessCommand(byte[] command)
        {
            string commandString = command.ToString();
        }

        public static void Roll(StepperMotor motor, int NumSteps, bool ToRight)
        {
            for (int i = 0; i < NumSteps; i++)
            {
                motor.Step(ToRight);
                Thread.Sleep(3);
            }
            
        }
    }
}

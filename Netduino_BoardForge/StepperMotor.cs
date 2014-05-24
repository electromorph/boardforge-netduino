using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace BoardForgeFirmware
{
    public class DigitalPin
    {
        OutputPort _digitalPin;
        bool _state = false;
        public Cpu.Pin PinNumber { get; set; }

        public DigitalPin(Cpu.Pin pin)
        {
            _digitalPin = new OutputPort(pin, false);
        }

        public bool State {  get { return _state; } set { SetPinState(value); _state = value; } }
        
        private void SetPinState(bool State)
        {
            _digitalPin.Write(State);
        }
    }

    public class StepperMotor
    {
        //pins contain integer representing the pin on the chip
        DigitalPin [] _motorWires;
        int position;

        public StepperMotor(DigitalPin Wire1, DigitalPin Wire2, DigitalPin Wire3, DigitalPin Wire4)
        {
            _motorWires = new DigitalPin[] { Wire1, Wire2, Wire3, Wire4 };
            position = 0;
        }
        
        public void Reset()
        {
            position = 0;
        }

        public void Step(bool ToRight)
        {
            _motorWires[position].State = false;
            if (ToRight) { position++; }
            else { position--; }
            if (position > 3) { position = 0; }
            if (position < 0) { position = 3; }
            _motorWires[position].State = true;
        }
    }
}

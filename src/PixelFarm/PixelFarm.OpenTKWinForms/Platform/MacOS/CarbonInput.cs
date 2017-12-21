using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
namespace OpenTK.Platform.MacOS
{
    using Input;
    class CarbonInput : IInputDriver
    {
        List<KeyboardDevice> dummy_keyboard_list = new List<KeyboardDevice>(1);
        List<MouseDevice> dummy_mice_list = new List<MouseDevice>(1);
        List<JoystickDevice> dummy_joystick_list = new List<JoystickDevice>(1);
        internal CarbonInput()
        {
            dummy_mice_list.Add(new MouseDevice());
            dummy_keyboard_list.Add(new KeyboardDevice());
            dummy_joystick_list.Add(new JoystickDevice<object>(0, 0, 0));
        }


        public void Poll()
        {
        }



        public IList<KeyboardDevice> Keyboard
        {
            get { return dummy_keyboard_list; }
        }



        public IList<MouseDevice> Mouse
        {
            get { return dummy_mice_list; }
        }



        public IList<JoystickDevice> Joysticks
        {
            get { return dummy_joystick_list; }
        }



        public void Dispose()
        {
        }

    }
}

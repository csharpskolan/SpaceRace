using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceRace
{
    class MenuChoice
    {
        public string Title { get; set; }
        public bool Selected { get; set; }

        public Action PressedAction { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceRace
{
    [Flags]
    public enum GameState
    {
        LOADING = 1,
        READY = 2,
        PLAYING = 4,
        WON = 8,
        GAMEOVER = 16,
        MAINMENU = 32,
        ALL = LOADING | READY | PLAYING | WON | GAMEOVER | MAINMENU
    }

    public enum MenuState
    {
        INTRO = 1,
        MAINMENU = 2
    }
}

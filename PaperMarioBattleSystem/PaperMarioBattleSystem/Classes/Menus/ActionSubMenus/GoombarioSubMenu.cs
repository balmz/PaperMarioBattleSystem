﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    public class GoombarioSubMenu : ActionSubMenu
    {
        public GoombarioSubMenu()
        {
            Position = new Vector2(210, 150);
            Initialize(new List<MoveAction> { new Bonk(), new TidalWave(), new Gulp() });
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfinityUnderground.API.Map
{
    public class BossRoom : Room
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BossRoom"/> class.
        /// </summary>
        public BossRoom()
        {
            Path = 0;
            NbOfNPC = 1;
            NameOfMap = "BossRoom";
        }

     }
}
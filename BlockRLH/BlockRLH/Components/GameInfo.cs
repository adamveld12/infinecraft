using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockRLH.Actors;
using Microsoft.Xna.Framework.GamerServices;
using RLH.Components;
using Microsoft.Xna.Framework;

namespace BlockRLH.Components
{
    /// <summary>
    /// Holds information about the game's players
    /// </summary>
    public class GameInfo
    {
        private List<PlayerController> localPlayers;
        private static PlayerController Player;
        private WorldInfo worldInfo;
        private bool initialized;

        public GameInfo()
        { initialized = false; }

        /// <summary>
        /// Begins initializing a local game
        /// </summary>
        public void BeginLocalGame(WorldInfo info)
        {
            var localGamers = Gamer.SignedInGamers;
            this.worldInfo = info;
            localPlayers = new List<PlayerController>(localGamers.Count);

            localPlayers.Add(new PlayerController(info, PlayerIndex.One));

            foreach (var item in localPlayers)
            {
                info.AddActor(item);
                info.AddActor(item.Camera);
            }
            initialized = true;
        }

        public static PlayerController GetPlayerController()
        {
            return Player;
        }

        /// <summary>
        /// Gets a player controller based on index
        /// <remarks>
        /// Uses linq, so use sparingly per update
        /// </remarks>
        /// </summary>
        public PlayerController GetPlayer(PlayerIndex index)
        {
            if (initialized)
            {
                Player = this.localPlayers.Where(plyr => plyr.PlayerIndex == index).First();
                return this.localPlayers.Where(plyr => plyr.PlayerIndex == index).First();
            }
            else
            {
                throw new InvalidOperationException("Cannot retrieve a player when the local game has not started");
            }
        }
    }
}
